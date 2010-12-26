using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConnection
{
    public partial class InstallOptions : UiControlBase
    {
        private readonly InstallSoftware _installerControl;

        public InstallOptions()
        {
            InitializeComponent();

            _installerControl = new InstallSoftware(); // need ref to set install file later
            groupBox1.Visible = false;
            var is64 = InstallHelper.Is64BitOperatingSystem();
            rd64.Checked = is64;
            rd32.Checked = !is64;
        }

        public void RestoreUserOptions(string installFile, string restoreFile)
        {
            // restore user options after user clicked Back button

            rd32.Checked = installFile == InstallHelper.X32;
            rd64.Checked = installFile == InstallHelper.X64;
            rd86.Checked = installFile == InstallHelper.X86;

            var isUserInstallFile = !installFile.StartsWith("http://") && !installFile.StartsWith("ftp");
            lblInstallFile.Visible = isUserInstallFile;
            lblInstallFile.Text = installFile;
            pnlVersion.Enabled = !isUserInstallFile;

            lblRestoreFile.Visible = !string.IsNullOrEmpty(restoreFile);
            lblRestoreFile.Text = restoreFile;
            lblRestoreFile.Location = new System.Drawing.Point(lblRestoreFile.Location.X, btnRestoreFile.Location.Y); // weirdness

            _installerControl.InstallFile = installFile;
            _installerControl.RestoreFile = restoreFile;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = true;
            BackButton.Visible = true;
            CancelButton.Visible = true;

            NextButton.Text = "Install";
            NextButton.Enabled = true;
            NextControl = _installerControl;
            PrevControl = new Start();

            CheckPrereqs();
        }

        private void CheckPrereqs()
        {
            if (!InstallHelper.IsWindowsInstaller45Installed())
            {
                NextButton.Enabled = false;

                var download = MessageBox.Show("You need to upgrade Windows Installer (version 4.5 is required). Please download and install this small file from Microsoft.\r\n\r\nClick YES to open that webpage now...",
                                             "", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes;
                if (download)
                    Process.Start(InstallHelper.MSI45);
            }
        }

        private void OnOptionsLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            groupBox1.Visible = !groupBox1.Visible;
        }

        private void OnVersionChanged(object sender, EventArgs e)
        {
            _installerControl.InstallFile = rd32.Checked ? InstallHelper.X32 : rd64.Checked ? InstallHelper.X64 : rd86.Checked ? InstallHelper.X86 : "";
        }

        private void OnInstallFileClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Sql Server install file",
                Filter = "installer files (*.exe)|*.exe|All files (*.*)|*.*",
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                lblInstallFile.Visible = true;
                lblInstallFile.Text = dialog.FileName;
                _installerControl.InstallFile = dialog.FileName;
                pnlVersion.Enabled = false;
            }
        }

        private void OnRestoreFileClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select database backup file",
                Filter = "backup files (*.bak)|*.bak|All files (*.*)|*.*",
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                lblRestoreFile.Text = dialog.FileName;
                lblRestoreFile.Visible = true;
                _installerControl.RestoreFile = dialog.FileName;
                lblRestoreFile.Location = new System.Drawing.Point(lblRestoreFile.Location.X, btnRestoreFile.Location.Y); // weirdness
            }

        }
    }
}
