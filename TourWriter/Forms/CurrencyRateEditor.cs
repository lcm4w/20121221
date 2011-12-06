using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Properties;
using TourWriter.Services;

namespace TourWriter.Forms
{
    public partial class CurrencyRateEditor : Form
    {
        private ToolSet toolSet;

        public string CurrencyFrom
        {
            get { return (string)cmbCurrencyFrom.Value; }
            set { cmbCurrencyFrom.Value = value; }
        }

        public string CurrencyTo
        {
            get { return (string)cmbCurrencyTo.Value; }
            set { cmbCurrencyTo.Value = value; }
        }

        public DateTime DateFrom
        {
            get { return txtDateFrom.Value; }
            set { txtDateFrom.Value = value; }
        }

        public DateTime DateTo
        {
            get { return txtDateFrom.Value; }
            set { txtDateFrom.Value = value; }
        }

        public Decimal? Rate
        {
            get { return Convert.ToDecimal(txtRate.Value); }
            set { txtRate.Value = value; }
        }
        

        public CurrencyRateEditor(ToolSet toolSet)
        {
            this.toolSet = toolSet;
            InitializeComponent();

            cmbCurrencyFrom.Select();

            cmbCurrencyFrom.DataSource = Cache.ToolSet.Currency.Where(c => c.RowState != DataRowState.Added && c.Enabled).ToList();
            cmbCurrencyFrom.DisplayMember = "CurrencyCode";
            cmbCurrencyTo.DataSource = Cache.ToolSet.Currency.Where(c => c.RowState != DataRowState.Added && c.Enabled).ToList();
            cmbCurrencyTo.DisplayMember = "CurrencyCode";
            Icon = Resources.TourWriter16;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtDateFrom.Value.Date > txtDateTo.Value.Date)
            {
                App.ShowWarning("From date must be less than To date");
                return;
            }

            var row = toolSet.CurrencyRate.NewCurrencyRateRow();
            row.CodeFrom = CurrencyFrom;
            row.CodeTo = CurrencyTo;
            row.ValidFrom = txtDateFrom.Value.Date;
            row.ValidTo = txtDateTo.Value.Date;
            row.Rate = (decimal)Rate;
            toolSet.CurrencyRate.AddCurrencyRateRow(row);

            DialogResult = DialogResult.OK;

            // -----------------------------------------------------
            /*
            for (int i = 0; i < (txtDateTo.Value.Date - txtDateFrom.Value.Date).Days + 1; i++)
            {
                DateTime date = txtDateFrom.Value.Date.AddDays(i);
                string sql = "";
                ToolSet.CurrencyRateRow[] rows;

                sql = string.Format(
                    "CurrencyCodeFrom = '{0}' AND CurrencyCodeTo = '{1}' AND ForecastDate = '{2}'",
                    CurrencyFrom, CurrencyTo, date.ToString("yyyy/MM/dd"));
                rows = (ToolSet.CurrencyRateRow[])toolSet.CurrencyRate.Select(sql);

                if(rows.Length == 0)
                {
                    ToolSet.CurrencyRateRow row = toolSet.CurrencyRate.NewCurrencyRateRow();
                    row.CurrencyCodeFrom = CurrencyFrom;
                    row.CurrencyCodeTo = CurrencyTo;
                    row.CurrencyRateDate = date.Date;
                    row.ForecastDate = date.Date;
                    if (Rate.HasValue) row.ForecastRate = (decimal)Rate;
                    toolSet.CurrencyRate.AddCurrencyRateRow(row);
                }
                else if (rows[0].IsForecastRateNull() || rows[0].ForecastRate != Rate)
                {
                    if(Rate.HasValue)
                        rows[0].ForecastRate = (decimal)Rate;
                }
            }
            DialogResult = DialogResult.OK;
            */
        }

        private void cmbCurrencyFrom_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            GridHelper.SetCurrencyComboAppearance(e);
        }

        private void cmbCurrencyTo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            GridHelper.SetCurrencyComboAppearance(e);
        }
    }
}