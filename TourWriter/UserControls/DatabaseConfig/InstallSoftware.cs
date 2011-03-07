using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConfig
{
    public partial class InstallSoftware : UiControlBase, IConnectionControl
    {
        private WebClient _downloader;
        private bool _restoreUserDatabase;
        internal string InstallFile { get; set; }
        internal string RestoreFile { get; set; }

        public InstallSoftware()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = true;
            BackButton.Visible = true;
            CancelButton.Visible = true;

            NextButton.Text = "Next >";
            NextButton.Enabled = false;
            BackButton.Enabled = false;
            CancelButton.Enabled = false;

            var configureControl = new InstallOptions();
            configureControl.RestoreUserOptions(InstallFile, RestoreFile);
            PrevControl = configureControl;
            NextControl = new InstallConfigure();

            ActionStart();
        }

        private void OnStopClick(object sender, EventArgs e)
        {
            BackButton.Enabled = true;
            NextButton.Enabled = false;
            CancelButton.Enabled = true;
            btnStop.Enabled = false;
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = 0;
            if (_downloader != null) _downloader.CancelAsync();
        }
        
        private void Log(string text)
        {
            Log(text, 100);
        }

        private void Log(string text, int delayAfter)
        {
            txtLog.Text += string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("yyyy.MM.dd HH:mm"), text);
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            Application.DoEvents();
            Thread.Sleep(delayAfter);
        }
        

        void ActionStart()
        {
            var error = "";
            Log("Checking system...", 2000);
            var existingServer = InstallHelper.GetSqlCmdPath().Length > 0 && InstallHelper.TestLocalServerConnect(out error);
            if (existingServer)
            {
                Log("Found existing TourWriter server, skipping install");
                SetupDatabase();
                return;
            }
            var isLocalFile = !InstallFile.StartsWith("http://");
            if (isLocalFile)
            {
                Log("User provided install file, skipping download");
                InstallStart();
                return;
            }
            Download();
        }
        
        void Download()
        {
            progressBar.Value = 0;
            progressBar.Style = ProgressBarStyle.Blocks;
            var localFile = Path.Combine(Path.GetTempPath(), "sqlsrvr.exe");
            Log(string.Format("Downloading file from: {0}\r\n\t to: {1} ...", InstallFile, localFile));

            InstallHelper.DeleteFile(localFile);
            _downloader = new WebClient();
            _downloader.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
                                                       {
                                                           progressBar.Value = e.ProgressPercentage;
                                                       };
            _downloader.DownloadFileCompleted += delegate(object sender, AsyncCompletedEventArgs e)
                                                     {
                                                         if (e.Cancelled) Log("Download cancelled, cancelling install also");
                                                         else
                                                         {
                                                             InstallFile = localFile;
                                                             progressBar.Value = progressBar.Maximum;
                                                             Log("Download complete");
                                                             InstallStart();
                                                         }
                                                     };
            _downloader.DownloadFileAsync(new Uri(InstallFile), localFile);
        }
        
        void InstallStart()
        {
            btnStop.Enabled = false;
            progressBar.Style = ProgressBarStyle.Marquee;
            Log("Install file is: " + InstallFile);
            Log("Installing Sql Server, this stage might take 10 minutes...");

            var args = InstallHelper.InstallArgs.Replace("\r\n", " ").Trim();
            var pi = new ProcessStartInfo { FileName = InstallFile, Arguments = args };
            //var pi = new ProcessStartInfo {FileName = @"C:\Program Files (x86)\Notepad++\Notepad++.exe"};
            var p = new Process { StartInfo = pi, EnableRaisingEvents = true };
            p.Exited += (sender, e) => Invoke((MethodInvoker)(() => InstallComplete(sender as Process)));
            p.Start();
        }

        void InstallComplete(Process process)
        {
            //ParentForm.TopMost = true;
            //ParentForm.TopMost = false;

            var error = "";
            var exitCode = process.ExitCode;

            Log("Sql installer exited with code" + exitCode);
            Log("Testing server connection...");
            if (InstallHelper.TestLocalServerConnect(out error))
            {
                Log("Server install successful");
                SetupDatabase();
            }
            else HandleConnectionError("server", error);
        }
        
        void SetupDatabase()
        {
            var error = "";
            btnStop.Enabled = false;
            if (InstallHelper.TestDatabaseConnect(out error))
            {
                Log("Found existing TourWriter database");
                var cancel = MessageBox.Show("An existing TourWriter database was found.\r\n\r\nDo you want to delete all existing data and recreate the database?",
                                             "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No;
                if (cancel)
                {
                    BackButton.Enabled = true;
                    CancelButton.Enabled = true;
                    btnStop.Enabled = false;
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Value = 0;
                    Log("User cancelled database setup.");
                    return;
                }
            }
            progressBar.Style = ProgressBarStyle.Marquee;

            // restore
            var isDefaultDb = false;
            if (!string.IsNullOrEmpty(RestoreFile))
                Log("Restoring database from user backup file: " + RestoreFile);
            else
            {
                Log("Creating new default database...");
                RestoreFile = GetDefaultRestoreFile();
                isDefaultDb = true;
            }

            Log(InstallHelper.RunSql(string.Format(InstallHelper.RestoreDbSql, RestoreFile)));

            // login
            Log("Configuring database...");
            Log(InstallHelper.RunSql(InstallHelper.CreateLoginUserSql));
            
            if (!InstallHelper.TestDatabaseConnect(out error))
            {
                HandleConnectionError("database", error);
                return;
            }

            // data 
            if (!_restoreUserDatabase)
            {
                Log("Setting initial data...");
                Log(InstallHelper.RunSql(InstallHelper.InitialiseNewDbSql));
                Log("Database setup complete.\r\nClick Next to continue.");
            }
            else
            {
                Log("Database restore complete.");
                NextControl = null;
                NextButton.Text = "Finish";
            }

            if (isDefaultDb) InstallHelper.DeleteFile(RestoreFile); // remove default db file from temp folder

            NextButton.Enabled = true;
            CancelButton.Enabled = false;
            BackButton.Enabled = false;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = progressBar.Maximum;

        }
        
        void HandleConnectionError(string level, string error)
        {
            var msg = level == "server" ? "ERROR: Failed to connect to server. Server software install may have failed, check Sql install logs and/or system logs. Error message: " + error :
                      level == "database" ? "ERROR: Failed to connect to database. After the Server install the database setup stage may have failed. Error message: " + error : "";
            Log(msg);

            BackButton.Enabled = true;
            NextButton.Enabled = false;
            CancelButton.Enabled = true;
            btnStop.Enabled = false;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;

            var log = Path.Combine(new FileInfo("Assembly.GetExecutingAssembly().Location").DirectoryName, "dbsetup.log");
            var txt = txtLog.Text;

            Log("\r\nSetup cancelled, saving this log to: " + log);
            File.WriteAllText(log, txt);
            Log("Finished with errors");
        }

        static string GetDefaultRestoreFile()
        {
            using (var zip = new Ionic.Zip.ZipFile(App.File_DefaultDatabaseFile))
                zip.ExtractAll(App.Path_TempFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
            return Path.Combine(App.Path_TempFolder, "TourWriter.bak");
        }

        #region IConnectionControl members
        
        public string GetServerName()
        {
            return Environment.MachineName + "\\TourWriter";
        }

        public string GetUserName()
        {
            return "";
        }

        public string GetPassword()
        {
            return "";
        }

        public string GetRemoteName()
        {
            return "";
        }

        public string GetRemoteConnection()
        {
            return "";
        }

        public bool ValidateAndFinalise()
        {
            return true;
        }

        #endregion
    }
}
