using System;
using System.Windows.Forms;
using TourWriter.Properties;

namespace TourWriter.UserControls.DatabaseConnection
{
    public partial class Ui : Form
    {
        public Ui(UserControl defaultControl)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;

            LoadControl(defaultControl);
        }

        /// <summary>
        /// The forms Back button
        /// </summary>
        internal Button BackButton { get { return btnPrev; } }
        /// <summary>
        /// The forms Next button
        /// </summary>
        internal Button NextButton { get { return btnNext; } }
        /// <summary>
        /// The forms Cancel button
        /// </summary>
        internal Button EndButton { get { return btnCancel; } }

        internal DatabaseSetupResult DatabaseSetupResult;
        internal UserControl NextControl { get; set; }
        internal UserControl PrevControl { get; set; }
        
        private void LoadControl(UserControl control)
        {
            panel1.Controls.Clear();
            if (control != null)
            {
                control.Dock = DockStyle.Fill;
                panel1.Controls.Add(control);
            }
        }

        internal void GoBack()
        {
            GoTo(PrevControl);
        }

        internal void GoNext()
        {
            if (NextControl != null)
                GoTo(NextControl);
            else GoComplete();
        }

        internal void GoCancel()
        {
            DialogResult = DialogResult.Cancel;
        }

        internal void GoComplete()
        {
            var control = panel1.Controls[0] as IConnectionControl;
            if (control.ValidateAndFinalise())
            {
                DatabaseSetupResult = new DatabaseSetupResult(control);
                DialogResult = DialogResult.OK;
            }
        }

        internal void GoTo(UserControl control)
        {
            LoadControl(control);
        }


        private void OnPrevClick(object sender, EventArgs e)
        {
            GoBack();
        }

        private void OnNextClick(object sender, EventArgs e)
        {
            GoNext();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            GoCancel();
        }
    }

    public class DatabaseSetupResult
    {
        internal string UserName { get; private set; }
        internal string Password { get; set; }
        internal string LocalServerName { get; set; } // Environment.MachineName;
        internal string RemoteName { get; set; }
        internal string RemoteConnection { get; set; }

        public DatabaseSetupResult(IConnectionControl connectionControl)
        {
            UserName = connectionControl.GetUserName();
            Password = connectionControl.GetPassword();
            LocalServerName = connectionControl.GetServerName();
            RemoteName = connectionControl.GetRemoteName();
            RemoteConnection = connectionControl.GetRemoteConnection();
        }

        internal bool IsLocalDatabase
        {
            get { return !string.IsNullOrEmpty(LocalServerName); }
        }

        internal bool IsRemoteDatabase
        {
            get { return !string.IsNullOrEmpty(RemoteConnection); }
        }
    }
}
