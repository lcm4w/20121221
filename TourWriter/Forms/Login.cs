using System;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Properties;
using TourWriter.UserControls.DatabaseConfig;
using TourWriter.Utilities.Encryption;

namespace TourWriter.Forms
{
    public partial class Login : Form
    {
        #region Properties

        private Exception serverError;
        private UserSet authenticatedUser;
        private BackgroundWorker getUserThread;
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
                return;
            }
            if (cmbServers.Items.Count == 0)
                ManageServers(); // open connections UI.
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

        private LoginResult Authenticate(DbConnection connection, string username, string password)
        {
            LoginResult loginResult;
            authenticatedUser = null;
            var userSet = new UserSet();

            try
            {
                if (connection.Type == "remote")
                    userSet.AuthenticateRemote(connection.Data, username, EncryptionHelper.EncryptString(password));
                else
                    userSet.AuthenticateLocal(connection.Data, username, EncryptionHelper.EncryptString(password));
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
            // TODO: check should be !=
            //else if (userSet.User[0].IsRecordActive)
            //{
            //    loginResult = LoginResult.Disabled;
            //}
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

            if (string.IsNullOrEmpty(cmbServers.Text.Trim()))
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
            var dbForm = new DatabaseMain(new StartPage(), Settings.Default.Connections);
            var dr = dbForm.ShowDialog();
            if (dr == DialogResult.OK)
            {
                cmbServers.Items.Clear();
                cmbServers.Items.AddRange(Settings.Default.Connections.Select(x => x.Name).OrderBy(x => x).ToArray());
                foreach (var item in cmbServers.Items.Cast<object>().Where(item => item.ToString() == Settings.Default.Connections.Last().Name))
                    cmbServers.SelectedItem = item.ToString();
            }
        }

        private void AddServer(string servername)
        {
            // add if new
            if (cmbServers.Items.Count == 0 || !cmbServers.Items.Contains(servername))
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
            if (Settings.Default.Connections == null)
                Settings.Default.Connections = new DbConnections();
            MigrateSetting(); //  TODO: remove this migration, added 10 Aug 2011
            
            txtUsername.Text = Settings.Default.Username;
            cmbServers.Items.AddRange(Settings.Default.Connections.Select(x => x.Name).OrderBy(x => x).ToArray());
            foreach (var item in cmbServers.Items.Cast<object>().Where(item => item.ToString() == Settings.Default.DefaultConnection))
                cmbServers.SelectedItem = item.ToString();
        }

        private void SaveSettings()
        {
            App.Servername = cmbServers.SelectedItem.ToString();

            Settings.Default.Username = txtUsername.Text;
            Settings.Default.DefaultConnection = cmbServers.SelectedItem.ToString();
            Settings.Default.Save();
        }

        private static void MigrateSetting()
        {
            if (Settings.Default.Connections.Count > 0 || Settings.Default.ServerNameHistory.Count == 0) return;

            // just get last connection (to clean up old list)
            var connection = Settings.Default.ServerNameHistory[0];
            Settings.Default.Connections.Add((connection == App.RemoteConnectionName) ? "remote" : "local",
                                             connection,
                                             (connection == App.RemoteConnectionName) ? Settings.Default.RemoteConnection : connection);

            Settings.Default.DefaultConnection = connection;
            Settings.Default.RemoteConnection = "";
            Settings.Default.ServerNameHistory.Clear();
            Settings.Default.Save();
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
        
        void getUserThread_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get server name on UI thread.
            DbConnection connection = null;
            var user = "";
            var pass = "";
            Invoke(new MethodInvoker(delegate
                                         {
                                             connection = Settings.Default.Connections.Where(x => x.Name == cmbServers.SelectedItem.ToString()).FirstOrDefault();
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

        private void btnSetup_Click(object sender, EventArgs e)
        {
            ManageServers();
        }

        #endregion
    }
}
