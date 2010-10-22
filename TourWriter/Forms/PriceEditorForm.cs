using System;
using System.Drawing;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.Dialogs
{
    /// <summary>
    /// Summary description for CostingDialog.
    /// </summary>
    public partial class PriceEditorForm : Form
    {
        public string PricingOption
        {
            get { return grpOption.Value != null ? grpOption.Value.ToString() : ""; }
            set { grpOption.Value = value; }
        }
        public decimal Net
        {
            get { return decimal.Parse(txtNet.Value.ToString()); }
            set { txtNet.Value = value; }
        }
        public decimal Markup
        {
            get { return decimal.Parse(txtMarkup.Value.ToString()); }
            set { txtMarkup.Value = value; }
        }
        public decimal Gross
        {
            get { return decimal.Parse(txtGross.Value.ToString()); }
            set { txtGross.Value = value; }
        }
        public decimal Commission
        {
            get { return decimal.Parse(txtCommission.Value.ToString()); }
            set { txtCommission.Value = value; }
        }

        public PriceEditorForm(string currencyCode)
        {
            InitializeComponent();

            grpOption.Items.Clear();
            grpOption.Items.Add(App.PricingOptionNetMarkupText, "Net and Markup");
            grpOption.Items.Add(App.PricingOptionNetGrossText, "Net and Gross");
            grpOption.Items.Add(App.PricingOptionGrossCommissionText, "Gross and Commission");

            SetCurrencyInfo(currencyCode);

            Icon = Resources.TourWriter16;
        }

        private void SetCurrencyInfo(string currencyCode)
        {
            if (currencyCode != "")
            {
                lblCurrencyInfo.Visible = true;
                lblCurrencyInfo.Text = string.Format("Currency: {0}", currencyCode.ToUpper());
            }
            else lblCurrencyInfo.Visible = false;

            var cultureInfo = App.GetCultureInfo(currencyCode);
            if (cultureInfo == null) throw new NullReferenceException("CultureInfo not found for currencyCode: " + currencyCode);
            txtNet.FormatProvider = cultureInfo;
            txtGross.FormatProvider = cultureInfo;
            txtMarkup.FormatProvider = cultureInfo;
            txtCommission.FormatProvider = cultureInfo;

            txtMarkup.FormatString = string.Format("###\\{0}## {1}", cultureInfo.NumberFormat.PercentDecimalSeparator, cultureInfo.NumberFormat.PercentSymbol);
            txtMarkup.MaskInput = "{LOC}-nnnn.nn";
            txtNet.FormatString = "c";
            txtNet.MaskInput = "{LOC}-nnnnnnnnnn.nn";
            txtGross.FormatString = "c";
            txtGross.MaskInput = "{LOC}-nnnnnnnnnn.nn";
            txtCommission.FormatString = string.Format("###\\{0}## {1}", cultureInfo.NumberFormat.PercentDecimalSeparator, cultureInfo.NumberFormat.PercentSymbol);
            txtCommission.MaskInput = "{LOC}-nnnn.nn";
        }

        private void CostingDialog_Load(object sender, EventArgs e)
        {
            Markup = CalcMarkup(Net, Gross);
            Commission = CalcCommission(Net, Gross);
        }

        private void EnableEditors(string option)
        {
            // Start with no editors
            txtNet.Enabled = false;
            txtMarkup.Enabled = false;
            txtGross.Enabled = false;
            txtCommission.Enabled = false;

            // Enable only selected editors
            switch (option)
            {
                case App.PricingOptionNetMarkupText:
                    {
                        txtNet.Enabled = true;
                        txtMarkup.Enabled = true;
                        txtNet.Select();
                        txtNet.SelectAll();
                        pnlRounding.Enabled = true;
                        break;
                    }
                case App.PricingOptionNetGrossText:
                    {
                        txtNet.Enabled = true;
                        txtGross.Enabled = true;
                        txtNet.Select();
                        txtNet.SelectAll();
                        pnlRounding.Enabled = false;
                        break;
                    }
                case App.PricingOptionGrossCommissionText:
                    {
                        txtGross.Enabled = true;
                        txtCommission.Enabled = true;
                        txtGross.Select();
                        txtGross.SelectAll();
                        pnlRounding.Enabled = false;
                        break;
                    }
            }
        }

        private void CalculatePrice(string option)
        {
            switch (option)
            {
                case App.PricingOptionNetMarkupText: // NetMarkup
                    {
                        Gross = CalcGross(Net, Markup);
                        Commission = CalcCommission(Net, Gross);
                        break;
                    }
                case App.PricingOptionNetGrossText: // NetGross
                    {
                        Markup = CalcMarkup(Net, Gross);
                        Commission = CalcCommission(Net, Gross);
                        break;
                    }
                case App.PricingOptionGrossCommissionText: // GrossCommission
                    {
                        Net = CalcNet(Gross, Commission);
                        Markup = CalcMarkup(Net, Gross);
                        break;
                    }
            }
        }

        private decimal RoundValue(decimal value)
        {
            if (pnlRounding.Visible)
            {
                if (chkRoundOne.Checked)
                {
                    value = (Decimal.Truncate(value) == value)
                            ? Decimal.Round(value) : Decimal.Round(value + 0.5m);
                }
                else if (chkRoundFive.Checked)
                {
                    value = (Decimal.Truncate(value / 5) == (value / 5))
                            ? Decimal.Round(value / 5) * 5 : Decimal.Round((value / 5) + 0.5m) * 5;
                }
                else if (chkRoundTen.Checked)
                {
                    value = (Decimal.Truncate(value / 10) == (value / 10))
                            ? Decimal.Round(value / 10) * 10 : Decimal.Round((value / 10) + 0.5m) * 10;
                }
            }
            return value;
        }

        private decimal CalcGross(decimal net, decimal markup)
        {
            return RoundValue(net + (markup/100*net));
        }

        private decimal CalcNet(decimal gross, decimal commission)
        {
            return gross - (commission/100*gross);
        }

        private decimal CalcMarkup(decimal net, decimal gross)
        {
            if (net == 0) return 0;

            return ((gross - net)/net)*100;
        }

        private decimal CalcCommission(decimal net, decimal gross)
        {
            if (gross == 0) return 0;

            return ((gross - net)/gross)*100;
        }

        #region Events

        private void editor_AfterExitEditMode(object sender, EventArgs e)
        {
            CalculatePrice(grpOption.Value.ToString());
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            CalculatePrice(grpOption.Value.ToString());
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void editor_Enter(object sender, EventArgs e)
        {
            (sender as Infragistics.Win.UltraWinEditors.UltraNumericEditor).
                SelectAll();
        }

        private void grpOption_ValueChanged(object sender, EventArgs e)
        {
            EnableEditors(grpOption.Value.ToString());
        }

        private void chkRoundOne_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRoundOne.Checked)
            {
                chkRoundFive.Checked = false;
                chkRoundTen.Checked = false;
            }

            CalculatePrice(grpOption.Value.ToString());
        }

        private void chkRoundFive_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRoundFive.Checked)
            {
                chkRoundOne.Checked = false;
                chkRoundTen.Checked = false;
            }

            CalculatePrice(grpOption.Value.ToString());
        }

        private void chkRoundTen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRoundTen.Checked)
            {
                chkRoundOne.Checked = false;
                chkRoundFive.Checked = false;
            }

            CalculatePrice(grpOption.Value.ToString());
        }

        #endregion
    }
}