using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Forms;
using TourWriter.Info;
using TourWriter.Services;
using ButtonDisplayStyle=Infragistics.Win.ButtonDisplayStyle;

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
            //tabControl1.TabPages.Remove(tabPage2); // TODO: hide Rates tab for now...
        }

        private void Currency_Load(object sender, EventArgs e)
        {
            DataBind();
            RefreshCurrencyExample();
        }

        private void DataBind()
        {
            gridCurrency.SetDataBinding(toolSet, "Currency");
            cmbCurrencyFrom.DataSource = toolSet.Currency;
            cmbCurrencyFrom.DisplayMember = "CurrencyCode";
            cmbCurrencyTo.DataSource = toolSet.Currency;
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
            e.Layout.Bands[0].Columns["Enabled"].Width = 100;
            e.Layout.Bands[0].Columns["CurrencyCode"].Width = 100;
            e.Layout.Bands[0].Columns["CurrencyName"].Width = 400;
            e.Layout.Bands[0].Columns["DisplayFormat"].Width = 200;

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
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "CurrencyCodeFrom")
                {
                    c.Header.Caption = "Currency from";
                    c.SortIndicator = SortIndicator.Ascending;
                }
                else if (c.Key == "CurrencyCodeTo")
                {
                    c.Header.Caption = "Currency to";
                }
                else if (c.Key == "CurrencyRateDate")
                {
                    c.Header.Caption = "Date";
                    c.SortIndicator = SortIndicator.Ascending;
                    DateTimeEditor editor = c.Editor as DateTimeEditor;
                    if (editor != null)
                        editor.DropDownButtonDisplayStyle = ButtonDisplayStyle.Never;
                }
                else if (c.Key == "ForecastRate")
                {
                    c.Header.Caption = "Forecast rate";
                    c.Header.ToolTipText = "To enable forward bookings to be made.";
                    c.CellClickAction = CellClickAction.Edit;
                    c.MaskInput = "nnnn.nnnn";
                    c.Format = "###0.0000";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "ActualRate")
                {
                    c.Header.Caption = "Actual rate";
                    c.Header.ToolTipText = "Actual currency rate, as known after the date";
                    c.CellClickAction = CellClickAction.Edit;
                    c.MaskInput = "nnnn.nnnn";
                    c.Format = "###0.0000";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else
                    c.Hidden = true;
            }

            GridHelper.SetDefaultGridAppearance(e);

            // Allow multiple row selects to help in deleting rows.
            e.Layout.Bands[0].Override.SelectTypeRow = SelectType.Extended;
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

        private void btnDefault_Click(object sender, EventArgs e)
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

        private void btnList_Click(object sender, EventArgs e)
        {
            if (!App.AskYesNo("This will allow you to edit the currency data.\r\n\r\nPlease be careful editing the Display Format as this will affect all areas where currency amounts are shown, including client reports.\r\n\r\nDo you wish to continue?")) return;
            gridCurrency.DisplayLayout.Bands[0].Columns["CurrencyCode"].CellClickAction = CellClickAction.Edit;
            gridCurrency.DisplayLayout.Bands[0].Columns["CurrencyName"].CellClickAction = CellClickAction.Edit;
            gridCurrency.DisplayLayout.Bands[0].Columns["DisplayFormat"].CellClickAction = CellClickAction.Edit;
        }

        private void RefreshCurrencyExample()
        {
            var ccy = CurrencyService.GetApplicationCurrencyCodeOrDefault();
            txtCurrency.Text = toolSet.Currency.Where(x => x.CurrencyCode == ccy).Select(x => x.DisplayName).FirstOrDefault();

            var lang = !toolSet.AppSettings[0].IsLanguageCodeNull() ? toolSet.AppSettings[0].LanguageCode.Trim() : "";
            if (lang == "") lang = CultureInfo.CurrentCulture.Name;
            txtCurrency.Text += " (example:  " + 12345.6789f.ToString("c", CultureInfo.GetCultureInfo(lang)) + ").";
        }
   }
}