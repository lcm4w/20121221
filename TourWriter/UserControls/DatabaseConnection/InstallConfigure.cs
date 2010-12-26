using System;
using TourWriter.Utilities.Encryption;

namespace TourWriter.UserControls.DatabaseConnection
{
    public partial class InstallConfigure : UiControlBase, IConnectionControl
    {
        public InstallConfigure()
        {
            InitializeComponent();
            lblMsg.Visible = false;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = true;
            BackButton.Visible = true;
            CancelButton.Visible = true;

            NextButton.Text = "Finish";
            NextButton.Enabled = true;
            BackButton.Enabled = false;
            CancelButton.Enabled = false;
            NextControl = null;
        }
        
        private void Save()
        {
            InstallHelper.RunSql(string.Format(
                "use TourWriter; update AppSettings set InstallName = N'{0}'",
                txtCompany.Text.Trim()));

            InstallHelper.RunSql(string.Format(
                "use TourWriter; insert into [User] (UserName, Password, Email, DisplayName, IsRecordActive, MustChangePassword) values (N'{0}', N'{1}', N'{2}', N'{0}', 1, 0)",
                txtUser.Text.Trim(), EncryptionHelper.EncryptString(txtPass.Text), txtEmail.Text.Trim()));

            InstallHelper.RunSql(string.Format(
                "use TourWriter; insert into UserRole (UserID, RoleID, AddedBy) values ((select UserId from [User] where UserName = '{0}'), 1, 1)",
                txtUser.Text.Trim()));
        }

        private bool IsValid()
        {
            var msg = "";

            if (txtCompany.Text.Trim() == "") msg += "Company name is required\r\n";

            if (txtEmail.Text.Trim() == "") msg += "Email address is required\r\n";
            else if (!App.ValidateEmailAddresses(txtEmail.Text.Trim())) msg += "Email address is not valid\r\n";

            if (txtUser.Text.Trim() == "") msg += "User name is required\r\n";

            lblMsg.Text = msg;
            lblMsg.Visible = lblMsg.Text.Length > 0;

            return msg.Length == 0; 
        }
        
        #region IConnectionControl members

        public string GetServerName()
        {
            return Environment.MachineName + "\\TourWriter";
        }

        public string GetUserName()
        {
            return txtUser.Text.Trim();
        }

        public string GetPassword()
        {
            return txtPass.Text;
        }

        public string GetRemoteName()
        {
            return null;
        }

        public string GetRemoteConnection()
        {
            return null;
        }
        
        public bool ValidateAndFinalise()
        {
            if (!IsValid())
                return false;
            Save();
            return true;
        }

        #endregion
    }
}
