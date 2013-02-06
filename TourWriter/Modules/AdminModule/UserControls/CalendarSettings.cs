using System;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.AdminModule.UserControls
{
    public partial class CalendarSettings : UserControl
    {

        private ToolSet toolSet
        {
            get { return (Tag as AdminMain).ToolSet; }
        }

        public CalendarSettings()
        {
            InitializeComponent();
        }

        private void CalendarSettings_Load(object sender, EventArgs e)
        {
            // bindings
            txtUserName.DataBindings.Add("Text", toolSet, "AppSettings.GCalUser");
            txtPassword.DataBindings.Add("Text", toolSet, "AppSettings.GCalPass");
        }

        private void EndAllEdits()
        {
            Validate();
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            EndAllEdits();
            base.OnValidating(e);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            CalendarService cs;
            try
            {
                Cursor = Cursors.WaitCursor;
                cs = new CalendarService(txtUserName.Text, txtPassword.Text);
                cs.RefreshAllEvents(DateTime.Today, DateTime.Today);
                App.ShowInfo("Login successful.");
            }
            catch (Exception ex)
            {
                cs = null;
                App.ShowError(ex.Message.ToString());
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}