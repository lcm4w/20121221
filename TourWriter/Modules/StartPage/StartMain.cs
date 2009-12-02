using Timer=System.Threading.Timer;

namespace TourWriter.Modules.StartPage
{
	/// <summary>
	/// Summary description for StartPage.
	/// </summary>
	public partial class StartMain : ModuleBase
	{
        private Timer _timer;
        private const int TimerPeriod = 30 * 60000; // 30 min
        private const string TagText = "Start page";
		private const string HeaderText = "Welome to TourWriter";

        public StartMain()
        {
            _doLicenseCheck = false;
            Icon = Properties.Resources.TourWriter16;
            InitializeComponent();
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;
        }

        private void StartMain_Load(object sender, System.EventArgs e)
        {
            LoadUrl(Properties.Settings.Default.AppStartPageUri);
            Text = HeaderText;
        }

        private void LoadUrl(string url)
        {
            _timer = null;
            webBrowser1.Navigate(url);
            StartRefreshTimer();
        }

        internal void StartRefreshTimer()
        {
            if (_timer == null) _timer = new Timer(RefreshTimerFired, null, TimerPeriod, TimerPeriod);
        }

        private void RefreshTimerFired(object state)
        {
            if (webBrowser1.IsDisposed) return;

            try
            {
                if (!webBrowser1.Url.Equals("about:blank"))
                    webBrowser1.Refresh();
            }
            catch (System.Exception ex)
            {
                TourWriter.Services.ErrorHelper.SendEmail(ex, true);
            }
        }

        protected override string GetDisplayName()
        {
            return TagText;
        }

        protected override bool IsDataDirty()
        {
            return false;
        }
	    
        private void menuClose_Click(object sender, System.EventArgs e)
        {
            Close();

        }

        private void menuHelp_Click(object sender, System.EventArgs e)
        {
            App.ShowHelp();
        }

        private void StartMain_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            _timer.Dispose();
        }
	}
}