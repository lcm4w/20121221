using System;

namespace TourWriter.UserControls.DatabaseConfig
{
    public partial class Start : UiControlBase
    {
        public Start()
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
            NextControl = new Local();
            GoNext();
        }

        private void OnRemoteClick(object sender, EventArgs e)
        {
            NextControl = new Remote();
            GoNext();
        }

        private void OnInstallClick(object sender, EventArgs e)
        {
            NextControl = new InstallOptions();
            GoNext();
        }
    }
}
