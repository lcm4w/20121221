using System;
using System.Globalization;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info;

namespace TourWriter.Forms
{
    public partial class PaymentTermsEditor : Form
    {
        #region Public Properties

        public bool DeleteTerms
        {
            get { return chkRemoveTerms.Checked; }
        }

        public int? PaymentDueID
        {
            get { return (int?)cmbPaymentDue.SelectedValue; }
            set { cmbPaymentDue.SelectedValue = value; }
        }

        public int? PaymentDuePeriod
        {
            get
            {
                if (txtPaymentDuePeriod.Value == DBNull.Value)
                    return null;

                return (int?)txtPaymentDuePeriod.Value;
            }
            set { txtPaymentDuePeriod.Value = value; }
        }

        public bool DepositRequired
        {
            get { return chkDepositRequired.Checked; }
            set { chkDepositRequired.Checked = value; }
        }

        public decimal? DepositAmount
        {
            get { return txtDepositAmount.Value != null ? (decimal?)Convert.ToDecimal(txtDepositAmount.Value) : null; }
            set { txtDepositAmount.Value = value; }
        }

        public char? DepositType
        {
            get { return (char?)cmbDepositType.SelectedValue; }
            set { cmbDepositType.SelectedValue = value; }
        }

        public int? DepositDueID
        {
            get { return (int?)cmbDepositDue.SelectedValue; }
            set { cmbDepositDue.SelectedValue = value; }
        }

        public int? DepositDuePeriod
        {
            get
            {
                if (txtDepositDuePeriod.Value == DBNull.Value)
                    return null;

                return (int?)txtDepositDuePeriod.Value;
            }
            set { txtDepositDuePeriod.Value = value; }
        }

        #endregion

        public PaymentTermsEditor(CultureInfo cultureInfo)
        {
            InitializeComponent();

            // do some data binding
            paymentDueBindingSource.DataSource = Cache.ToolSet.PaymentDue;
            paymentDueBindingSource2.DataSource = Cache.ToolSet.PaymentDue;

            cmbDepositType.DataSource = DepositTypeList.GetDepositTypes(cultureInfo);
            cmbDepositType.DisplayMember = "Text";
            cmbDepositType.ValueMember = "ID";

            UpdateDepositControls();
        }

        /// <summary>
        /// Enables or disables deposit related controls
        /// depending on whether a deposit is required or not.
        /// </summary>
        private void UpdateDepositControls()
        {
            if (DepositRequired)
            {
                lblAmount.Enabled = true;
                lblTerms.Enabled = true;
                txtDepositAmount.Enabled = true;
                cmbDepositType.Enabled = true;
                txtDepositDuePeriod.Enabled = true;
                cmbDepositDue.Enabled = true;
            }

            else
            {
                lblAmount.Enabled = false;
                lblTerms.Enabled = false;
                txtDepositAmount.Enabled = false;
                cmbDepositType.Enabled = false;
                txtDepositDuePeriod.Enabled = false;
                cmbDepositDue.Enabled = false;
            }
        }

        /// <summary>
        /// Ensure all the entered values are valid.
        /// </summary>  
        /// <returns>true if everything is good, otherwise false.</returns>
        private bool ValidateValues()
        {
            string msg = String.Empty;

            if (PaymentDueID == null)
            {
                msg += "Please select a payment term.\r\n";
            }
            if (PaymentDuePeriod == null)
            {
                msg += "Please enter a payment term period.\r\n";
            }

            if (DepositRequired)
            {
                if (DepositAmount == null || DepositAmount <= 0)
                {
                    msg += "Please enter a valid deposit amount.\r\n";
                }
                if (DepositType == null)
                {
                    msg += "Please select a deposit type.\r\n";
                }
                if (DepositDueID == null)
                {
                    msg += "Please select a deposit payment term.\r\n";
                }
                if (DepositDuePeriod == null)
                {
                    msg += "Please enter a deposit payment term period.\r\n";
                }
            }

            if (msg.Length > 0)
            {
                App.ShowError(msg);
                return false;
            }

            return true;
        }

        #region Events

        private void chkDepositRequired_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDepositControls();
        }

        private void cmbPaymentDue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentDue.SelectedValue == null)
            {
                txtPaymentDuePeriod.Enabled = false;
            }
            else
            {
                txtPaymentDuePeriod.Enabled = true;
            }
        }

        private void cmbDepositDue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDepositDue.SelectedValue == null)
            {
                txtDepositDuePeriod.Enabled = false;
            }
            else
            {
                txtDepositDuePeriod.Enabled = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (DeleteTerms && App.AskYesNo("Are you sure you want to remove the payment terms?"))
            {
                DialogResult = DialogResult.OK;
            }

            // no need to validate values if the terms are being deleted
            else if (ValidateValues())
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void chkRemoveTerms_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = !chkRemoveTerms.Checked;
            groupBox2.Enabled = !chkRemoveTerms.Checked;
        }

        #endregion
    }
}