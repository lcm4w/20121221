using System;
using System.Linq;
using System.Windows.Forms;

namespace TourWriter.UserControls.DatabaseConnection
{
    public partial class ChooseDatabase : BaseUserControl
    {
        public ChooseDatabase()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            NextButton.Visible = false;
            BackButton.Visible = false;
            CancelButton.Visible = true;
        }
        
        private void OnRemoteClick(object sender, EventArgs e)
        {
            NextControl = new OnlineDatabase();
            GoNext();
        }

        private void OnLocalClick(object sender, EventArgs e)
        {
            NextControl = new LocalDatabase();
            GoNext();
        }

        private void OnTrialClick(object sender, EventArgs e)
        {
            var admin = App.IsDebugMode && ModifierKeys == Keys.Control; // debug mode and CTRL key is down
            if (!admin && ConnectionInfo.DbConnections.Where(x => x.Type == "remote").Count() > 0)
            {
                App.ShowInfo("You already have an online database configured.\r\n\r\nPlease contact Support to discuss (support@tourwriter.com).");
                return;
            }

            NextControl = new TrialDatabase();
            GoNext();
        }
    }
}
