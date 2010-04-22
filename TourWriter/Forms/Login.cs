using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Properties;
using TourWriter.Utilities.Encryption;

namespace TourWriter.Forms
{
    public partial class Login : Form
    {
        #region Properties

        private static readonly string newServerText = "<add server...>";
        private Exception serverError;
        private UserSet authenticatedUser;
        private BackgroundWorker getUserThread;
        private string tempServerName;
        private enum LoginResult
        {
            Success,
            Failed,
            Disabled,
            ServerError
        }

        #endregion

        public Login()
        {
            InitializeComponent();

            Icon = Resources.TourWriter16;

            UpgradeSettings();
            LoadSettings();

            cmbServers.SelectedIndexChanged += cmbServers_SelectedIndexChanged;
            if (txtUsername.Text == "")
                txtUsername.Select();
            else txtPassword.Select();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            // Check commandline input params
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 2) // account for exe path and update flag
            {
                ProcessInputArgs(args);
                btnLogin.PerformClick();
            }
        }

        private void ProcessInputArgs(string[] args)
        {
            if (args.Length == 0) return;

            for (var i = 1; i < args.Length-1; ++i)
            {
                var arg = args[i];

                if (arg == "/s" || arg == "-s")
                    AddServer(args[++i]);
                else if (arg == "/u" || arg == "-u")
                    txtUsername.Text = args[++i];
                else if (arg == "/p" || arg == "-p")
                    txtPassword.Text = args[++i];
            }
        }
        
        #region Authenticatation

        private void Authenticate()
        {
            Thread.Sleep(250); // delay to let other processes get head start
            getUserThread = new BackgroundWorker();
            getUserThread.DoWork += getUserThread_DoWork;
            getUserThread.RunWorkerCompleted += getUserThread_RunWorkerCompleted;
            getUserThread.RunWorkerAsync();
        }

        private LoginResult Authenticate(string connection, string username, string password)
        {
            LoginResult loginResult;
            authenticatedUser = null;
            var userSet = new UserSet();

            // get user info.
            try
            {
                if (connection == App.RemoteConnectionName)
                    userSet.AuthenticateRemote(Settings.Default.RemoteConnection, username, EncryptionHelper.EncryptString(password));
                else
                    userSet.AuthenticateLocal(connection, username, EncryptionHelper.EncryptString(password));
            }
            catch (Exception ex)
            {
                serverError = ex;
                return LoginResult.ServerError;
            }

            // process user info.
            if (userSet.User.Rows.Count == 0)
            {
                loginResult = LoginResult.Failed;
            }
            else if (userSet.User[0].IsRecordActive)
            {
                loginResult = LoginResult.Disabled;
            }
            else
            {
                authenticatedUser = userSet;
                loginResult = LoginResult.Success; 
            }
            return loginResult;
        }

        private void ProcessLoginResult(LoginResult result)
        {
            switch (result)
            {
                case LoginResult.Success:
                    Global.Cache.SetCurrentUser(authenticatedUser, authenticatedUser.User[0].UserID);
                    SaveSettings(); // sets database server name for connection string
                    DialogResult = DialogResult.OK;
                    break;
                case LoginResult.Failed:
                    SetActivityDisplay(false);
                    MessageBox.Show("Login failed for that username and password",
                        "TourWriter message", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtPassword.SelectAll();
                    break;
                case LoginResult.Disabled:
                    SetActivityDisplay(false);
                    MessageBox.Show("Your user account has been disabled",
                        "TourWriter message", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;
                case LoginResult.ServerError:
                    SetActivityDisplay(false);
                    HandleConnectionError();
                    break;
            }
            serverError = null;
        }

        private void HandleConnectionError()
        {
            var computer = cmbServers.Text.Substring(0, 
                cmbServers.Text.Contains("\\") ? cmbServers.Text.LastIndexOf('\\') : cmbServers.Text.Length);
            var msg =
                "Failed to connect to your TourWriter database on computer: "+ computer +".\r\n\r\n" +
                " - Check spelling (is server information correct).\r\n" +
                " - Check network (can you access the server computer).\r\n" +
                " - Check server (is server computer and database running ok).\r\n\r\n" +
                "Possible quick fixes are; restart server computer, restart your network.\r\n\r\n" +
                "Do you want to view detailed connection error message?";
            
            if (MessageBox.Show(msg, "TourWriter message", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) 
                == DialogResult.Yes) App.Error(serverError, false);
            
        }

        private void SetActivityDisplay(bool isBusy)
        {
            pictureBox1.Visible = isBusy;
            label1.Enabled = !isBusy;
            label2.Enabled = !isBusy;
            label3.Enabled = !isBusy;
            txtUsername.Enabled = !isBusy;
            txtPassword.Enabled = !isBusy;
            cmbServers.Enabled = !isBusy;
            btnLogin.Enabled = !isBusy;
        }
        
        private bool ValidateForm()
        {
            bool isValid = true;

            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            if (string.IsNullOrEmpty(cmbServers.Text.Trim()) || cmbServers.Text == newServerText)
            {
                errorProvider.SetError(cmbServers, "Server name is required");
                cmbServers.Select();
                isValid = false;
            }
            else
                errorProvider.SetError(cmbServers, "");

            if (string.IsNullOrEmpty(txtUsername.Text.Trim()))
            {
                errorProvider.SetError(txtUsername, "User name is required");
                txtUsername.Select();
                isValid = false;
            }
            else
                errorProvider.SetError(txtUsername, "");

            return isValid;
        }

        #endregion
        
        #region Edit servers

        private void ManageServers()
        {
            var serverManager = new ServerManager();
            if (serverManager.ShowDialog() == DialogResult.OK)
            {
                AddServer(serverManager.ServerName);
            }
        }

        private void AddServer(string servername)
        {
            // add if new
            if (!cmbServers.Items.Contains(servername))
                cmbServers.Items.Insert(0, servername);
                        
            // select it
            cmbServers.Text = servername;
        }

        #endregion

        #region User settings

        private static void UpgradeSettings()
        {
            // Upgrade settings from old file when new version resets values to default. 
            if (Settings.Default.SettingsUpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpgradeRequired = false;
            }
        }

        private void LoadSettings()
        {
            txtUsername.Text = Settings.Default.Username;

            // Set server name
            if (Settings.Default.ServerNameHistory == null)
                Settings.Default.ServerNameHistory = new System.Collections.Specialized.StringCollection();
            if (Settings.Default.ServerNameHistory.Count > 0)
            {
                string[] servers = new string[Settings.Default.ServerNameHistory.Count];
                Settings.Default.ServerNameHistory.CopyTo(servers, 0);
                cmbServers.Items.AddRange(servers);
            }
            cmbServers.Items.Add(newServerText);
            if (cmbServers.Items.Count > 1)
                cmbServers.SelectedIndex = 0;
        }

        private void SaveSettings()
        {
            if (txtUsername.Text.Trim().Length > 0)
                Settings.Default.Username = txtUsername.Text;

            if (!string.IsNullOrEmpty(cmbServers.Text.Trim()))
            {
                string serverName = cmbServers.Text;
                Settings.Default.ServerNameHistory.Remove(serverName);
                Settings.Default.ServerNameHistory.Insert(0, serverName);
                if (Settings.Default.ServerNameHistory.Count == 5) // limit items to 5
                    Settings.Default.ServerNameHistory.RemoveAt(4);
            }
            Settings.Default.Save();

            App.Servername = cmbServers.Text;
        }

        #endregion
        
        #region Events

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            SetActivityDisplay(true);
            Authenticate();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void cmbServers_DropDown(object sender, EventArgs e)
        {
            tempServerName = cmbServers.Text;
        }

        void cmbServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbServers.SelectedIndex == cmbServers.Items.Count - 1)
                ManageServers();
        }

        void getUserThread_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get server name on UI thread.
            string connection = "";
            string user = "";
            string pass = "";
            Invoke(new MethodInvoker(delegate
                                         {
                                             connection = cmbServers.Text;
                                             user = txtUsername.Text;
                                             pass = txtPassword.Text;
                                         }));
            // Authenticate user.
            e.Result = Authenticate(connection, user, pass);
        }

        void getUserThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProcessLoginResult((LoginResult)e.Result);
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            if (txtUsername.Text != "")
                errorProvider.SetError(txtUsername, "");
        }

        private void cmbServers_TextChanged(object sender, EventArgs e)
        {
            if (cmbServers.Text != "")
                errorProvider.SetError(cmbServers, "");
        }

        #endregion
    }
}
