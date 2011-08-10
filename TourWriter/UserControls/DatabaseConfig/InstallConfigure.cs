using System;
using System.Collections.Specialized;
using TourWriter.Utilities.Encryption;

namespace TourWriter.UserControls.DatabaseConfig
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

            var name = Environment.MachineName + "TourWriter";
            Connections.Add("local", name, name);
        }

        private bool IsValid()
        {
            var msg = "";

            if (txtCompany.Text.Trim() == "") msg += "Company name is required\r\n";

            if (txtEmail.Text.Trim() == "") msg += "Email address is required\r\n";
            else if (!App.ValidateEmailAddresses(txtEmail.Text.Trim())) msg += "Email address is not valid\r\n";

            if (txtUser.Text.Trim() == "") msg += "User name is required\r\n";

            if (txtUser.Text.Trim().ToLower() == "admin") msg += "User name is not valid, please choose a different User name\r\n";

            lblMsg.Text = msg;
            lblMsg.Visible = lblMsg.Text.Length > 0;

            return msg.Length == 0; 
        }
        
        public bool ValidateAndFinalise()
        {
            if (!IsValid())
                return false;
            Save();
            return true;
        }
    }
}
