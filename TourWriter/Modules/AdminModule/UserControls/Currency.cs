using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace TourWriter.Modules.AdminModule.UserControls
{
    public partial class Currency : UserControl
    {
        private ToolSet toolSet
        {
            get
            {
                AdminMain admin = Tag as AdminMain;
                if (admin != null)
                    return admin.ToolSet;

                return null;
            }
        }

        public Currency()
        {
            InitializeComponent();

            var permission = AppPermissions.UserHasPermission(AppPermissions.Permissions.CurrencyRatesEdit);
            btnRateAdd.Enabled = btnRateDel.Enabled = permission;
        }
        
        private void Currency_Load(object sender, EventArgs e)
        {
            DataBind();
            RefreshCurrencyExample();
        }

        private void DataBind()
        {
            cmbRateSource.Items.AddRange(new object[] { "Spot rates: yahoo", "Spot rates: google", "Forward rates: user-defined" });
            cmbDatePoint.Items.AddRange(new object[] { "Booking start date", "Itinerary start date" });

            cmbRateSource.SelectedIndex = 
                Cache.ToolSet.AppSettings[0].IsCcyRateSourceNull() ? 0 :
                Cache.ToolSet.AppSettings[0].CcyRateSource == "yahoo" ? 0 :
                Cache.ToolSet.AppSettings[0].CcyRateSource == "google" ? 1 :
                Cache.ToolSet.AppSettings[0].CcyRateSource == "predefined" ? 2 : 0;
            
            cmbDatePoint.SelectedIndex =
                            Cache.ToolSet.AppSettings[0].IsCcyDatePointNull() ? 0 :
                            Cache.ToolSet.AppSettings[0].CcyRateSource == "booking" ? 0 :
                            Cache.ToolSet.AppSettings[0].CcyRateSource == "internet" ? 1 : 0;
            
            
            cmbRateSource.SelectedIndexChanged += cmbRateSource_SelectedIndexChanged;
            cmbDatePoint.SelectedIndexChanged += cmbDatePoint_SelectedIndexChanged;


            gridRate.DisplayLayout.ValueLists.Add("CurrencyList");
            gridRate.DisplayLayout.ValueLists["CurrencyList"].SortStyle = ValueListSortStyle.Ascending;
            gridRate.DisplayLayout.ValueLists["CurrencyList"].ValueListItems.Add(DBNull.Value, "");
            foreach (ToolSet.CurrencyRow r in Cache.ToolSet.Currency.Where(c => c.RowState != DataRowState.Deleted && c.Enabled))
                gridRate.DisplayLayout.ValueLists["CurrencyList"].ValueListItems.Add(r.CurrencyCode, r.DisplayName);

            gridCurrency.SetDataBinding(toolSet, "Currency");
            cmbCurrencyFrom.DataSource = Cache.ToolSet.Currency.Where(c => c.RowState != DataRowState.Added && c.Enabled).ToList();
            cmbCurrencyFrom.DisplayMember = "CurrencyCode";
            cmbCurrencyTo.DataSource = Cache.ToolSet.Currency.Where(c => c.RowState != DataRowState.Added && c.Enabled).ToList();
            cmbCurrencyTo.DisplayMember = "CurrencyCode";
            gridRate.DataSource = toolSet.CurrencyRate;
        }

        #region Currencies

        private void gridCurrency_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "Enabled")
                {
                    c.Header.Caption = "Enable";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "CurrencyCode")
                {
                    c.Header.Caption = "Currency Code";
                    c.MaskInput = ">AAA"; // 3 char required, converts to uppercase
                    //c.CellClickAction = CellClickAction.Edit;
                    c.SortIndicator = SortIndicator.Ascending;
                }
                else if (c.Key == "CurrencyName")
                {
                    c.Header.Caption = "Currency Name";
                    //c.CellClickAction = CellClickAction.Edit;
                } 
                else if (c.Key == "DisplayFormat")
                {
                    c.Header.Caption = "Display Format";
                    //c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
            }
            e.Layout.Bands[0].Columns["Enabled"].Width = 70;
            e.Layout.Bands[0].Columns["CurrencyCode"].Width = 70;
            e.Layout.Bands[0].Columns["CurrencyName"].Width = 200;
            e.Layout.Bands[0].Columns["DisplayFormat"].Width = 100;

            int index = 0;
            e.Layout.Bands[0].Columns["Enabled"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["CurrencyCode"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["CurrencyName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["DisplayFormat"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.AutoFitStyle = AutoFitStyle.None;

            e.Layout.Bands[0].SortedColumns.Add("CurrencyCode", false);
            e.Layout.Bands[0].SortedColumns.Add("Enabled", true, true);
        }
            
        private void gridCurrency_Error(object sender, ErrorEventArgs e)
        {
            if (e.ErrorType == ErrorType.Data &&
                e.DataErrorInfo.Cell.Column.Key == "CurrencyCode")
            {
                e.Cancel = true; // cancel default message box
                App.ShowInfo("Currency code must be 3 characters long.");
            }
        }
        
        #endregion

        #region Currency rates

        private void gridRate_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("EnabledToday"))
                e.Layout.Bands[0].Columns.Add("EnabledToday");

            var permission = AppPermissions.UserHasPermission(AppPermissions.Permissions.CurrencyRatesEdit);
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "CodeFrom")
                {
                    c.Header.Caption = "Currency from";
                    c.SortIndicator = SortIndicator.Ascending;
                    
                    c.Style = ColumnStyle.DropDown;
                    c.ValueList = gridRate.DisplayLayout.ValueLists["CurrencyList"];
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "CodeTo")
                {
                    c.Header.Caption = "Currency to";

                    c.Style = ColumnStyle.DropDown;
                    c.ValueList = gridRate.DisplayLayout.ValueLists["CurrencyList"];
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "Rate")
                {
                    c.Header.Caption = "Rate";
                    c.CellClickAction = CellClickAction.Edit;
                    c.MaskInput = "nnnn.nnnn";
                    c.Format = "###0.0000";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "ValidFrom")
                {
                    c.Header.Caption = "Valid from";

                    c.Style = ColumnStyle.Date;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "ValidTo")
                {
                    c.Header.Caption = "Valid to";
                    
                    c.Style = ColumnStyle.Date;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "EnabledFrom")
                {
                    c.Header.Caption = "Enabled from";

                    c.Style = ColumnStyle.Date;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "EnabledTo")
                {
                    c.Header.Caption = "Enabled to";

                    c.Style = ColumnStyle.Date;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "EnabledToday")
                {
                    c.Header.Caption = "Enabled Today";
                    c.Header.ToolTipText = "Can I use this rate today";
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = true;
                }
                else c.Hidden = true;

                // disable if no permission ----------------------------
                if (!permission) c.CellActivation = Activation.Disabled;
                // -----------------------------------------------------
            }

            GridHelper.SetDefaultGridAppearance(e);

            // Allow multiple row selects to help in deleting rows.
            e.Layout.Bands[0].Override.SelectTypeRow = SelectType.Extended;
        }

        private void gridRate_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["EnabledFrom"].Value != DBNull.Value && e.Row.Cells["EnabledTo"].Value != DBNull.Value)
            {
                var now = DateTime.Now;
                var from = (DateTime)e.Row.Cells["EnabledFrom"].Value;
                var to = (DateTime)e.Row.Cells["EnabledTo"].Value;
                e.Row.Cells["EnabledToday"].Value = from.Date <= now.Date && now.Date <= to.Date ? "YES" : "NO";
            }
        }

        private void btnRateAdd_Click(object sender, EventArgs e)
        {
            CurrencyRateEditor c = new CurrencyRateEditor(toolSet);
            c.ShowDialog();
        }

        private void btnRateDel_Click(object sender, EventArgs e)
        {
            GridHelper.DeleteSelectedRows(gridRate, true);
        }

        #endregion

        #region Currency rate search

        private void cmbCurrencyFrom_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            GridHelper.SetCurrencyComboAppearance(e);
        }

        private void cmbCurrencyTo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            GridHelper.SetCurrencyComboAppearance(e);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string fromCurrency = cmbCurrencyFrom.Text;
            string toCurrency = cmbCurrencyTo.Text;
            DateTime fromDate = txtDateFrom.Value;
            DateTime toDate = txtDateTo.Value;

            Cursor = Cursors.WaitCursor;
            toolSet.CurrencyRate.Load(fromCurrency, toCurrency, fromDate, toDate);
            Cursor = Cursors.Default;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if(App.AskYesNo("This will clear all rows that do not have unsaved changes. Continue?"))
            {
                DataTable changes = toolSet.CurrencyRate.GetChanges();
                toolSet.CurrencyRate.Clear();
                if (changes != null)
                    toolSet.CurrencyRate.Merge(changes);
            }
        }

        #endregion
        
        private void RefreshCurrencyExample()
        {
            var ccy = CurrencyService.GetApplicationCurrencyCodeOrDefault();
            txtCurrency.Text = toolSet.Currency.Where(x => x.CurrencyCode == ccy).Select(x => x.DisplayName).FirstOrDefault();

            var lang = !toolSet.AppSettings[0].IsLanguageCodeNull() ? toolSet.AppSettings[0].LanguageCode.Trim() : "";
            if (lang == "") lang = CultureInfo.CurrentCulture.Name;
            txtCurrency.Text += " (example:  " + 12345.6789f.ToString("c", CultureInfo.GetCultureInfo(lang)) + ").";
        }

        private void cmbRateSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cache.ToolSet.AppSettings[0].CcyRateSource =
                cmbRateSource.SelectedIndex == 0 ? "yahoo" :
                cmbRateSource.SelectedIndex == 1 ? "google" :
                cmbRateSource.SelectedIndex == 2 ? "predefined" : 
                "yahoo"; // default
        }

        private void cmbDatePoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cache.ToolSet.AppSettings[0].CcyDatePoint =
                cmbDatePoint.SelectedIndex == 0 ? "booking" :
                cmbDatePoint.SelectedIndex == 1 ? "itinerary" :
                "booking"; // default

            Cache.ToolSet.AppSettings[0].CcyDatePoint = cmbDatePoint.SelectedIndex == 0 ? "booking" : "itinerary";
        }

        private void btnCurrencyEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!App.AskYesNo("This will allow you to edit the currency data.\r\n\r\nPlease be careful editing the Display Format as this will affect all areas where currency amounts are shown, including client reports.\r\n\r\nDo you wish to continue?")) return;
            gridCurrency.DisplayLayout.Bands[0].Columns["CurrencyCode"].CellClickAction = CellClickAction.Edit;
            gridCurrency.DisplayLayout.Bands[0].Columns["CurrencyName"].CellClickAction = CellClickAction.Edit;
            gridCurrency.DisplayLayout.Bands[0].Columns["DisplayFormat"].CellClickAction = CellClickAction.Edit;
        }

        private void lnkDefaultCurrency_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var ccy = new CurrencyDefault();

            if (ccy.ShowDialog() == DialogResult.OK)
            {
                var changed = false;
                if (ccy.LanguageCode != toolSet.AppSettings[0].LanguageCode)
                {
                    changed = true;
                    toolSet.AppSettings[0].LanguageCode = ccy.LanguageCode;
                }
                if (ccy.CurrencyCode != toolSet.AppSettings[0].CurrencyCode)
                {
                    changed = true;
                    toolSet.AppSettings[0].CurrencyCode = ccy.CurrencyCode;
                }
                if (changed)
                {
                    RefreshCurrencyExample();
                    CurrencyService.SetUiCultureInfo();
                }
            }
        }
   }
}