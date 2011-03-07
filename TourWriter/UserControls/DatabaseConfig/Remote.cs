using System;

namespace TourWriter.UserControls.DatabaseConfig
{
    public partial class Remote : UiControlBase, IConnectionControl
    {
        public Remote()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = true;
            BackButton.Visible = true;
            CancelButton.Visible = true;

            NextButton.Text = "OK";
            NextButton.Enabled = false;
            NextControl = null;
            PrevControl = new Start();

            // load existing connection info
            txtInfo.Text = Properties.Settings.Default.RemoteConnection;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            NextButton.Enabled = txtInfo.Text.Length > 0;
        }
        
        #region IConnectionControl members

        public string GetServerName()
        {
            return null;
        }

        public string GetUserName()
        {
            return null;
        }

        public string GetPassword()
        {
            return null;
        }

        public string GetRemoteName()
        {
            return App.RemoteConnectionName;
        }

        public string GetRemoteConnection()
        {
            return txtInfo.Text.Trim().Replace("\r\n", " ");
        }

        public bool ValidateAndFinalise()
        {
            return true;
        }

        #endregion
    }
}
