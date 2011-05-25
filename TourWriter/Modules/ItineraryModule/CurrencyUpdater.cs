using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;
using Resources=TourWriter.Properties.Resources;
using System.Linq;

namespace TourWriter.Modules.ItineraryModule
{
    public partial class CurrencyUpdater : Form
    {
        private bool _userShouldUpdateRates;
        private readonly ItinerarySet itinerarySet;
        private readonly DataTable purchaseItemTable;

        public CurrencyUpdater(ItinerarySet itinerarySet)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            this.itinerarySet = itinerarySet;
            purchaseItemTable = itinerarySet.PurchaseItem.Copy();

            // currencies list with null row at index 0
            var list = Cache.ToolSet.Currency.Where(c => c.Enabled).ToList();
            var nullRow = Cache.ToolSet.Currency.NewCurrencyRow();
            nullRow.CurrencyCode = "";
            var ccy = Cache.ToolSet.Currency.Where(x => x.CurrencyCode == CurrencyService.GetApplicationCurrencyCodeOrDefault()).FirstOrDefault();
            nullRow.DisplayName = string.Format("-default- ({0} {1})", ccy.CurrencyCode, ccy.CurrencyName);
            list.Insert(0, nullRow);

            // bind currency
            cmbCurrency.DataSource = list;
            cmbCurrency.ValueMember = "CurrencyCode";
            cmbCurrency.DisplayMember = "DisplayName";
            cmbCurrency.SelectedValue = itinerarySet.Itinerary[0].CurrencyCode ?? "";
            cmbCurrency.SelectedIndexChanged += cmbCurrency_SelectedIndexChanged;
            cmbCurrency.Enabled = false;

            // bind purchase items
            gridBookings.DataSource = purchaseItemTable.DefaultView;

            // set default margin
            if (!itinerarySet.Itinerary[0].IsAgentIDNull())
            {
                var agent = Cache.ToolSet.Agent.FindByAgentID(itinerarySet.Itinerary[0].AgentID);
                if (agent != null && agent.Table.Columns.Contains("DefaultCurrencyMargin"))
                {
                    var margin = agent["DefaultCurrencyMargin"];
                    if (margin != DBNull.Value) txtRateAdjustment.Value = decimal.Parse(margin.ToString());
                            
                }
            }
            btnUpdate.Select();
            btnUpdate.Focus();
        }
        
        private void UpdateCurrencies()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var toCurrency = GetToCurrencyCode();
                var selectedRows = gridBookings.Rows.ToList().
                    Where(x => (bool)x.Cells["IsSelected"].Value && x.Cells["ToCurrency"].Value != null && x.Cells["FromCurrency"].Value != null);
                var toFromCurrencies = selectedRows.Select(from => from.Cells["FromCurrency"].Value.ToString()).Distinct().
                    Select(from => new CurrencyService.Currency { FromCurrency = from, ToCurrency = toCurrency }).ToList();

                foreach (var r in selectedRows) // reset status
                {
                    r.Cells["NewRate"].Value = null;
                    r.Cells["Result"].Value = null;
                    r.Cells["Result"].Appearance.Image = null;
                }

                CurrencyService.GetRates(toFromCurrencies);
                
                var adjust = Convert.ToDecimal(txtRateAdjustment.Value);
                adjust = 1 + ((adjust != 0) ? adjust / 100 : 0);

                foreach (var c in toFromCurrencies)
                {
                    var currency = c;
                    var updateRows = selectedRows.Where(x => x.Cells["FromCurrency"].Value.ToString() == currency.FromCurrency);
                    foreach (var row in updateRows)
                    {
                        var noAdjust = c.Rate == 1 && c.FromCurrency.Trim().ToLower() == c.ToCurrency.Trim().ToLower();
                        row.Cells["Result"].Appearance.Image = Resources.Tick;
                        row.Cells["NewRate"].Value = Decimal.Round(Convert.ToDecimal(currency.Rate) * (noAdjust ? 1 : adjust), 4);
                        if (!string.IsNullOrEmpty(c.ErrorMessage))
                        {
                            row.Cells["Result"].Value = c.ErrorMessage;
                            row.Cells["Result"].Appearance.Image = Resources.Warning;
                        }
                    }
                }
            }
            finally { Cursor = Cursors.Default; }
        }

        private void SelectAll()
        {
            bool unselectAll = true;

            foreach (UltraGridRow row in gridBookings.Rows)
            {
                bool isRowLocked = row.Cells["IsLockedAccounting"].Value != DBNull.Value &&
                                   (bool)row.Cells["IsLockedAccounting"].Value;

                if (isRowLocked || (bool)row.Cells["IsSelected"].Value)
                    continue;

                row.Cells["IsSelected"].Value = true;
                unselectAll = false;
            }
            if (unselectAll)
            {
                // if all of the rows are already selected, then unselect all
                foreach (UltraGridRow row in gridBookings.Rows)
                {
                    row.Cells["IsSelected"].Value = false;
                }
            }
        }

        private void SaveChanges()
        {
            // update ccy code
            if (itinerarySet.Itinerary[0].IsCurrencyCodeNull())
            {
                if (cmbCurrency.SelectedValue.ToString() != "")
                    itinerarySet.Itinerary[0].CurrencyCode = cmbCurrency.SelectedValue.ToString();
            }
            else if (itinerarySet.Itinerary[0].CurrencyCode != cmbCurrency.SelectedValue.ToString())
            {
                if (cmbCurrency.SelectedValue.ToString() == "") itinerarySet.Itinerary[0].SetCurrencyCodeNull();
                else itinerarySet.Itinerary[0].CurrencyCode = cmbCurrency.SelectedValue.ToString();
            }

            // move temp cols to the real cols in prep for merge
            foreach (var row in gridBookings.Rows)
            {
                if (row.Cells["NewRate"].Value != DBNull.Value)
                    row.Cells["CurrencyRate"].Value = row.Cells["NewRate"].Value;
                row.Update();
            }

            // merge
            var changes = purchaseItemTable.GetChanges();
            if (changes != null) itinerarySet.PurchaseItem.Merge(changes);
        }

        private string GetToCurrencyCode()
        {
            return cmbCurrency.SelectedValue.ToString() != "" ? cmbCurrency.SelectedValue.ToString() : CurrencyService.GetApplicationCurrencyCodeOrDefault();
        }
        
        #region Events

        private void gridBookings_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("IsSelected"))
                e.Layout.Bands[0].Columns.Add("IsSelected");
            if (!e.Layout.Bands[0].Columns.Exists("FromCurrency"))
                e.Layout.Bands[0].Columns.Add("FromCurrency");
            if (!e.Layout.Bands[0].Columns.Exists("ToCurrency"))
                e.Layout.Bands[0].Columns.Add("ToCurrency");
            if (!e.Layout.Bands[0].Columns.Exists("Result"))
                e.Layout.Bands[0].Columns.Add("Result");
            if (!e.Layout.Bands[0].Columns.Exists("PurchaseLineName"))
                e.Layout.Bands[0].Columns.Add("PurchaseLineName");
            if (!e.Layout.Bands[0].Columns.Exists("OldRate"))
                e.Layout.Bands[0].Columns.Add("OldRate");
            if (!e.Layout.Bands[0].Columns.Exists("NewRate"))
                e.Layout.Bands[0].Columns.Add("NewRate");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.CellActivation = Activation.NoEdit;

                if (c.Key == "IsSelected")
                {
                    c.Header.Caption = String.Empty;
                    c.DataType = typeof (Boolean);
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.Header.Appearance.Image = Resources.CheckBox;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.Width = 30;
                    c.MaxWidth = 30;
                    c.SortIndicator = SortIndicator.Disabled;
                }
                else if (c.Key == "Result")
                {
                    c.Width = 80;
                    c.DataType = typeof (String);
                    c.CellAppearance.ImageHAlign = HAlign.Left;
                    c.CellAppearance.ImageVAlign = VAlign.Middle;
                }
                else if (c.Key == "PurchaseLineName")
                {
                    c.Width = 80;
                    c.Header.Caption = "Booking name";
                    c.DataType = typeof (String);
                }
                else if (c.Key == "PurchaseItemName")
                {
                    c.Width = 120;
                    c.Header.Caption = "Item description";
                }
                else if (c.Key == "FromCurrency")
                {
                    c.Width = 80;
                    c.Header.Caption = "From";
                    c.Header.ToolTipText = "Service or booking currency";
                }
                else if (c.Key == "ToCurrency")
                {
                    c.Width = 80;
                    c.Header.Caption = "To";
                    c.Header.ToolTipText = "Itinerary currency";
                }
                else if (c.Key == "OldRate")
                {
                    c.Width = 80;
                    c.Header.Caption = "Old rate";
                    c.DataType = typeof (Decimal);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.Header.ToolTipText = "Exchange rate before update";
                }
                else if (c.Key == "NewRate")
                {
                    c.Width = 80;
                    c.Header.Caption = "New rate";
                    c.DataType = typeof(Decimal);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.Header.ToolTipText = "Exchange rate after update"; 
                }
                else
                {
                    c.Hidden = true;
                }
            }

            // set the column order
            int index = 0;
            e.Layout.Bands[0].Columns["IsSelected"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseLineName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["FromCurrency"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ToCurrency"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["OldRate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NewRate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Result"].Header.VisiblePosition = index;

            // set defaults
            GridHelper.SetDefaultGridAppearance(e);

            // override defaults
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

            // default sort
            e.Layout.Bands[0].SortedColumns.Add("ToCurrency", false);
        }

        private void gridBookings_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.ReInitialize)
                return;

            var id = (int)e.Row.Cells["PurchaseItemID"].Value;
            var item = itinerarySet.PurchaseItem.Where(x => x.RowState != DataRowState.Deleted && x.PurchaseItemID == id).FirstOrDefault();
            
            e.Row.Cells["IsSelected"].Value = true;
            e.Row.Cells["Result"].ToolTipText = String.Empty;
            e.Row.Cells["PurchaseLineName"].Value = item.PurchaseLineRow.PurchaseLineName;
            e.Row.Cells["OldRate"].Value = e.Row.Cells["CurrencyRate"].Value;
            e.Row.Cells["NewRate"].Value = DBNull.Value;
            e.Row.Cells["FromCurrency"].Value = CurrencyService.GetPurchaseItemCurrencyCodeOrDefault(item);
            e.Row.Cells["ToCurrency"].Value = GetToCurrencyCode();
        }
        
        void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            _userShouldUpdateRates = true;
            foreach(var row in gridBookings.Rows)
            {
                row.Cells["ToCurrency"].Value = cmbCurrency.SelectedValue.ToString() != "" ? cmbCurrency.SelectedValue : CurrencyService.GetApplicationCurrencyCodeOrDefault();
                row.Cells["NewRate"].Value = DBNull.Value;
                row.Cells["Result"].Value = DBNull.Value;
                row.Cells["Result"].Appearance.Image = null;
                row.Cells["Result"].ToolTipText = String.Empty;
            }
        }

        private void gridBookings_MouseClick(object sender, MouseEventArgs e)
        {
            UIElement clickedElement =
                gridBookings.DisplayLayout.UIElement.ElementFromPoint(gridBookings.PointToClient(MousePosition));

            if (clickedElement == null)
                return;

            // if a header is clicked, check if it is the "IsSelected" column
            HeaderUIElement headerElement = (HeaderUIElement)clickedElement.GetAncestor(typeof (HeaderUIElement));
            if (headerElement != null)
            {
                UltraGridColumn column = (UltraGridColumn)headerElement.GetContext(typeof (UltraGridColumn));
                if (column.Key == "IsSelected")
                {
                    SelectAll();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateCurrencies();
            _userShouldUpdateRates = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_userShouldUpdateRates && App.AskYesNo("You have not updated the currency rates.\r\n\r\nUpdate rates now?"))
            {
                _userShouldUpdateRates = false;
                UpdateCurrencies();
                return;
            }


            SaveChanges();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lnkEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            const string msg =
                "Client Payments exist, in your current Itinerary Currency\r\n\r\n" +
                "Remove existing payments by making each 0 (zero) or blank, or by removing the payments completly.\r\n\r\n" +
                "You can then re-enter them in the new Itinerary Currency amount after you change it.";

            var hasPayments = itinerarySet.ItineraryPayment.Where(x => x.RowState != DataRowState.Deleted && !x.IsAmountNull() && x.Amount > 0).Count() > 0;

            if (hasPayments) 
                App.ShowWarning(msg);
            else 
                cmbCurrency.Enabled = true;
        }

        #endregion
    }
}
