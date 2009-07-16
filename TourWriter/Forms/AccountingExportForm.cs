using System;
using System.IO;
using System.Windows.Forms;

namespace TourWriter.Forms
{
    /// <summary>
    /// Provides selection options for exporting data to accounting software
    /// </summary>
    public partial class AccountingExportForm : Form
    {
        #region Properties

        #region Purchase

        internal bool EnablePurchases
        {
            get { return grpPurchases.Enabled; }
            set { grpPurchases.Enabled = value; }
        }

        /// <summary>
        /// Whether to include purchases data
        /// </summary>
        internal bool IncludePurchases
        {
            get { return chkPurchases.Checked; }
        }

        /// <summary>
        /// Whether to include suppliers data
        /// </summary>
        internal bool IncludeSuppliers
        {
            get { return chkSuppliers.Checked; }
        }

        /// <summary>
        /// The filename to write purchases to
        /// </summary>
        internal string PurchasesFilename
        {
            get { return txtPurchases.Text; }
            set { txtPurchases.Text = value; }
        }

        /// <summary>
        /// The filename to write suppliers to
        /// </summary>
        internal string SuppliersFilename
        {
            get { return txtSuppliers.Text; }
            set { txtSuppliers.Text = value; }
        }
        #endregion

        #region Sale

        internal bool EnableSales
        {
            get { return grpSales.Enabled; }
            set { grpSales.Enabled = value; }
        }

        /// <summary>
        /// Whether to include sales data
        /// </summary>
        internal bool IncludeSales
        {
            get { return chkSales.Checked; }
        }

        /// <summary>
        /// Whether to include clients data
        /// </summary>
        internal bool IncludeClients
        {
            get { return chkClients.Checked; }
        }

        /// <summary>
        /// The filename to write sales to
        /// </summary>
        internal string SalesFilename
        {
            get { return txtSales.Text; }
            set { txtSales.Text = value; }
        }

        /// <summary>
        /// The filename to write clients to
        /// </summary>
        internal string ClientsFilename
        {
            get { return txtClients.Text; }
            set { txtClients.Text = value; }
        }
        #endregion

        #region Payments
        /// <summary>
        /// Whether to include sales data
        /// </summary>
        internal bool IncludePayments
        {
            get { return chkPayments.Checked; }
        }

        /// <summary>
        /// The filename to write sales to
        /// </summary>
        internal string PaymentsFilename
        {
            get { return txtPayments.Text; }
            set { txtPayments.Text = value; }
        }
        #endregion

        #endregion

        public AccountingExportForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // get the path and add to filenames
            string path = App.PromptChooseDir("");
            if (path.Length > 0)
            {
                PurchasesFilename = Path.Combine(path, App.StripInvalidFileNameChars(PurchasesFilename));
                SuppliersFilename = Path.Combine(path, App.StripInvalidFileNameChars(SuppliersFilename));
                SalesFilename = Path.Combine(path, App.StripInvalidFileNameChars(SalesFilename));
                ClientsFilename = Path.Combine(path, App.StripInvalidFileNameChars(ClientsFilename));
                PaymentsFilename = Path.Combine(path, App.StripInvalidFileNameChars(PaymentsFilename));

                DialogResult = DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkPurchases_CheckedChanged(object sender, EventArgs e)
        {
            txtPurchases.Enabled = chkPurchases.Checked;
            btnOk.Enabled = chkPurchases.Checked || chkSuppliers.Checked || chkSales.Checked || chkClients.Checked || chkPayments.Checked;
        }

        private void chkSuppliers_CheckedChanged(object sender, EventArgs e)
        {
            txtSuppliers.Enabled = chkSuppliers.Checked;
            btnOk.Enabled = chkPurchases.Checked || chkSuppliers.Checked || chkSales.Checked || chkClients.Checked || chkPayments.Checked;
        }

        private void chkSales_CheckedChanged(object sender, EventArgs e)
        {
            txtSales.Enabled = chkSales.Checked;
            btnOk.Enabled = chkPurchases.Checked || chkSuppliers.Checked || chkSales.Checked || chkClients.Checked || chkPayments.Checked;
        }

        private void chkClients_CheckedChanged(object sender, EventArgs e)
        {
            txtClients.Enabled = chkClients.Checked;
            btnOk.Enabled = chkPurchases.Checked || chkSuppliers.Checked || chkSales.Checked || chkClients.Checked || chkPayments.Checked;
        }

        private void chkPayments_CheckedChanged(object sender, EventArgs e)
        {
            txtPayments.Enabled = chkPayments.Checked;
            btnOk.Enabled = chkPurchases.Checked || chkSuppliers.Checked || chkSales.Checked || chkClients.Checked || chkPayments.Checked;
        }
    }
}
