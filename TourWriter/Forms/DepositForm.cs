using System;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info;

namespace TourWriter.Forms
{
    public partial class DepositForm : Form
    {
        public char? DepositType
        {
            get { return (char?)cmbDepositType.SelectedValue; }
        }

        public double DepositAmount
        {
            get { return (double)txtDepositAmount.Value; }
        }

        public int? PaymentTermDay
        {
            get
            {
                int retVal;
                if (!int.TryParse(txtPaymentTermDays.Text, out retVal))
                    return null;

                return retVal;
            }
        }

        public int? PaymentTermID
        {
            get { return (int?)cmbPaymentTerms.SelectedValue; }
        }

        public DepositForm()
        {
            InitializeComponent();

            DataBind();

            SetDepositAmountFormat();
        }

        private void DataBind()
        {
            paymentTermsBindingSource.DataSource = Cache.ToolSet.PaymentDue;

            cmbDepositType.DataSource = DepositTypeList.DepositTypes;
            cmbDepositType.DisplayMember = "Text";
            cmbDepositType.ValueMember = "ID";
        }

        private void SetDepositAmountFormat()
        {
            if (cmbDepositType.SelectedValue == null)
            {
                // disable the TextBox if there is no selected DepositType
                txtDepositAmount.Enabled = false;
                return;
            }

            else
            {
                // set the TextBox's FormatString based on the DepositType
                txtDepositAmount.Enabled = true;
                string format = cmbDepositType.SelectedValue.ToString();
                txtDepositAmount.FormatString = (format != "p") ? format : "###\\.##%";
            }
        }

        private bool ValidateValues()
        {
            string msg = String.Empty;

            if (cmbDepositType.SelectedValue != null)
            {
                // ensure a deposit amount is entered
                if ((double)txtDepositAmount.Value <= 0)
                {
                    msg += "Please enter a deposit amount greater than 0.\r\n";
                }

                // ensure payment terms are selected
                if (cmbPaymentTerms.SelectedValue != null)
                {
                    if (PaymentTermRequiresDay(cmbPaymentTerms.Text))
                    {
                        // ensure a day is entered
                        int val;
                        if (!int.TryParse(txtPaymentTermDays.Text, out val))
                        {
                            msg += "Please enter a valid payment term day.\r\n";
                        }
                    }
                }
                else
                {
                    msg += "Please select a payment term.\r\n";
                }
            }

            if (msg.Length > 0)
            {
                App.ShowError(msg);
                return false;
            }

            return true;
        }

        private static bool PaymentTermRequiresDay(string paymentTerm)
        {
            // check if selected payment term requires a day to be entered
            bool requiresDay = false;
            switch (paymentTerm)
            {
                case "days before":
                    requiresDay = true;
                    break;
                case "days after":
                    requiresDay = true;
                    break;
                case "of month following":
                    requiresDay = true;
                    break;
                case "of month before":
                    requiresDay = true;
                    break;
                default:
                    break;
            }

            return requiresDay;
        }

        #region Events
        private void cmbDepositType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDepositAmountFormat();
        }

        private void cmbPaymentTerms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentTerms.SelectedValue == null ||
                !PaymentTermRequiresDay(cmbPaymentTerms.Text))
            {
                txtPaymentTermDays.Enabled = false;
            }
            else
            {
                txtPaymentTermDays.Enabled = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateValues())
                DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        #endregion
    }
}