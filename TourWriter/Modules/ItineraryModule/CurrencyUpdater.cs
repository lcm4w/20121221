using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Services;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Modules.ItineraryModule
{
    public partial class CurrencyUpdater : Form
    {
        private readonly ItinerarySet itinerarySet;
        private readonly DataTable purchaseItemTable;

        public CurrencyUpdater(ItinerarySet itinerarySet)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            this.itinerarySet = itinerarySet;
            purchaseItemTable = itinerarySet.PurchaseItem.Copy();

            string currencyCode = CurrencyUpdateService.GetLocalCurrencyCode();
            purchaseItemTable.DefaultView.RowFilter = String.Format("CurrencyCode <> '{0}'", currencyCode);
            
            gridBookings.DataSource = purchaseItemTable.DefaultView;
        }

        private void UpdateCurrencies()
        {
            var currencyList = new List<CurrencyUpdateService.Currency>();
            foreach (UltraGridRow row in gridBookings.Rows)
            {
                if (!(bool)row.Cells["IsSelected"].Value)
                    continue;

                var currency = new CurrencyUpdateService.Currency();
                currency.Key = row;
                currency.FromCurrency = row.Cells["CurrencyCode"].Value.ToString();
                currency.ToCurrency = CurrencyUpdateService.GetLocalCurrencyCode();
                currencyList.Add(currency);
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                CurrencyUpdateService.GetRates(currencyList);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            foreach (var currency in currencyList)
            {
                UltraGridRow row = ((UltraGridRow)currency.Key);

                decimal value = Convert.ToDecimal(txtRateAdjustment.Value);
                decimal adjustment = 1 + ((value != 0) ? value / 100 : 0);

                row.Cells["NewRate"].Value = Decimal.Round(Convert.ToDecimal(currency.Rate) * adjustment, 4);

                if (String.IsNullOrEmpty(currency.ErrorMessage))
                {
                    // no errors
                    row.Cells["Result"].Value = null;
                    row.Cells["Result"].Appearance.Image = Resources.Tick;
                }
                else
                {
                    // there was an error
                    row.Cells["Result"].Value = null;
                    row.Cells["Result"].Appearance.Image = Resources.Cross;
                }

                row.Update();
            }
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
            foreach (UltraGridRow row in gridBookings.Rows)
            {
                if (row.Cells["NewRate"].Value != DBNull.Value)
                    row.Cells["CurrencyRate"].Value = row.Cells["NewRate"].Value;
                
                row.Update();
            }

            DataTable changes = purchaseItemTable.GetChanges();
            if (changes != null)
            {
                itinerarySet.PurchaseItem.Merge(changes);
            }
        }

        #region Events

        private void gridBookings_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("IsSelected"))
                e.Layout.Bands[0].Columns.Add("IsSelected");
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
                else if (c.Key == "CurrencyCode")
                {
                    c.Width = 80;
                    c.Header.Caption = "Currency";
                }
                else if (c.Key == "OldRate")
                {
                    c.Width = 80;
                    c.Header.Caption = "Old rate";
                    c.DataType = typeof (Decimal);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "NewRate")
                {
                    c.Width = 80;
                    c.Header.Caption = "New rate";
                    c.DataType = typeof (Decimal);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
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
            e.Layout.Bands[0].Columns["CurrencyCode"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["OldRate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NewRate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Result"].Header.VisiblePosition = index;

            // set defaults
            GridHelper.SetDefaultGridAppearance(e);

            // override defaults
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void gridBookings_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.ReInitialize)
                return;

            int purchaseLineId = (int)e.Row.Cells["PurchaseLineID"].Value;
            ItinerarySet.PurchaseLineRow purchaseLine =
                itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineId);

            e.Row.Cells["IsSelected"].Value = true;
            e.Row.Cells["Result"].ToolTipText = String.Empty;
            e.Row.Cells["PurchaseLineName"].Value = purchaseLine.PurchaseLineName;
            e.Row.Cells["OldRate"].Value = e.Row.Cells["CurrencyRate"].Value;
            e.Row.Cells["NewRate"].Value = DBNull.Value;
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
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveChanges();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}
