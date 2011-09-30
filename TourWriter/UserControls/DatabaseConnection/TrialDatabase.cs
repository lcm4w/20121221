using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConnection
{
    public partial class TrialDatabase : BaseUserControl, IConnectionControl
    {
        private const string MsgSuccess = @"
Your database is ready.

The database is hosted by Travelmesh Web Services, and you will receive a confirmation email from them shortly.

Click OK to login in now :)
";

        public TrialDatabase()
        {
            Travelmesh.ApiWrapper.PingServerAysnc(); // wake

            InitializeComponent();
            msgCompany.Visible = false;
            lnkInfo.Visible = false; // TODO: WIP: need to update webpage that this links too!!
        }

        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = true;
            BackButton.Visible = true;
            CancelButton.Visible = true;

            NextButton.Text = "Login";
            NextButton.Enabled = true;
            BackButton.Enabled = true;
            CancelButton.Enabled = true;
            NextControl = null;
            PrevControl = new ChooseDatabase();

            txtCompany.Select();
            txtCompany.SelectAll();
        }
        
        private bool Save()
        {
            txtCompany.Enabled = false;
            txtUser.Enabled = false;
            txtEmail.Enabled = false;
            txtEmail2.Enabled = false;
            NextButton.Enabled = false;
            BackButton.Enabled = false;
            CancelButton.Enabled = false;

            var prog = new TrialProgress(PointToScreen(panel1.Location))
                        {
                            Size = panel1.Size,
                            CompanyName = txtCompany.Text.Trim(),
                            UserName = txtUser.Text.Trim(),
                            UserEmail = txtEmail.Text.Trim()
                        };

            if (prog.ShowDialog() == DialogResult.OK)
            {
                var key = prog.Key;
                var conn = ConnectionInfo.DbConnections.Add("remote", App.OnlineConnectionName, key);
                ConnectionInfo.UserName = txtUser.Text;
                ConnectionInfo.Password = ""; // use blank
                ConnectionInfo.SelectedConnection = conn.Name;
                ConnectionInfo.AutoLogin = true;
                App.ShowInfo(MsgSuccess.Trim());
                return true;
            }

            // problem...
            txtCompany.Enabled = true;
            txtUser.Enabled = true;
            txtEmail.Enabled = true;
            txtEmail2.Enabled = true;
            NextButton.Enabled = true;
            BackButton.Enabled = true;
            CancelButton.Enabled = true;
            if (prog.Error is Exception)
                App.Error(prog.Error as Exception);
            return false;
        }
        
        public bool ValidateAndFinalise()
        {
            if (!IsValid())
            {
                App.ShowInfo("Please ensure all data is valid");
                return false;
            }
            return Save();
        }

        private bool IsValid()
        {
            ValidateCompany();
            ValidateUser();
            ValidateEmail();
            ValidateEmail2();

            return !msgCompany.Visible && !msgName.Visible && !msgEmail.Visible && !msgEmail2.Visible;
        }

        private void ValidateCompany()
        {
            msgCompany.Visible = txtCompany.Text.Trim() == "";
        }

        private void ValidateUser()
        {
            if (txtUser.Text.Trim() == "")
            {
                msgName.Visible = true;
                msgName.Text = "required";
            }
            else if (txtUser.Text.Trim().ToLower() == "admin")
            {
                msgName.Visible = true;
                msgName.Text = "not valid";
            }
            else msgName.Visible = false;
        }

        private void ValidateEmail()
        {
            if (txtEmail.Text.Trim() == "")
            {
                msgEmail.Visible = true;
                msgEmail.Text = "required";
            }
            else if (!App.ValidateEmailAddresses(txtEmail.Text.Trim()))
            {
                msgEmail.Visible = true;
                msgEmail.Text = "not valid";
            }
            else
            {
                msgEmail.Visible = false;
            }
        }

        private void ValidateEmail2()
        {
            msgEmail2.Visible = txtEmail2.Text.Trim().ToLower() != txtEmail.Text.Trim().ToLower();
        }

        private void txtCompany_TextChanged(object sender, EventArgs e)
        {
            ValidateCompany();
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            ValidateUser();
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (msgEmail.Visible) ValidateEmail();
        }

        private void txtEmail2_TextChanged(object sender, EventArgs e)
        {
            if (msgEmail2.Visible) ValidateEmail2();
        }

        private void txtEmail_Enter(object sender, EventArgs e)
        {
            lblEmail2.Visible = txtEmail2.Visible = true;
        }

        private void lnkInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://cloud.travelmesh.com/tourwriter");
        }
    }
}
