using System;

namespace TourWriter.UserControls.DatabaseConfig
{
    public partial class StartPage : UiControlBase
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = false;
            BackButton.Visible = false;
            CancelButton.Visible = true;
        }

        private void OnLocalClick(object sender, EventArgs e)
        {
            NextControl = new LocalSettings();
            GoNext();
        }

        private void OnRemoteClick(object sender, EventArgs e)
        {
            NextControl = new RemoteSettings();
            GoNext();
        }

        private void OnInstallClick(object sender, EventArgs e)
        {
            NextControl = new InstallChooseOptions();
            GoNext();
        }
    }
}
