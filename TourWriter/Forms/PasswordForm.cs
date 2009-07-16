using System;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.Forms
{
    public partial class PasswordForm : Form
    {
        public string Password
        {
            get { return txtNewPassword.Text; }
        }

        public PasswordForm()
        {
            InitializeComponent();

            Icon = Resources.TourWriter16;
        }

        private bool ValidatePassword()
        {
            // Trim the passwords
            txtNewPassword.Text = txtNewPassword.Text.Trim();
            txtConfirmPassword.Text = txtConfirmPassword.Text.Trim();

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show(App.GetResourceString("ShowPasswordMismatch"));
                txtConfirmPassword.Focus();
                txtConfirmPassword.SelectAll();
                return false;
            }

            if (txtNewPassword.Text.Length == 0 && 
                !App.AskYesNo("Are you sure you want to use a blank password?"))
            {
                return false;
            }
            return true;
        }

        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidatePassword())
                DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        #endregion
    }
}