namespace TourWriter.Modules.StartPage
{
	/// <summary>
	/// Summary description for StartPage.
	/// </summary>
	public partial class StartMain : ModuleBase
	{
        private const string tagText = "Start page";
		private const string headerText = "Welome to TourWriter";

        public StartMain()
        {
            Icon = TourWriter.Properties.Resources.TourWriter16;
            InitializeComponent();
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;
        }

        private void StartMain_Load(object sender, System.EventArgs e)
        {
            LoadUrl(Properties.Settings.Default.AppStartPageUri);
            Text = headerText;
        }

        private void LoadUrl(string url)
        {
            webBrowser1.Navigate(url);
        }
        

        protected override string GetDisplayName()
        {
            return tagText;
        }

        protected override bool IsDataDirty()
        {
            return false;
        }
	    

        private void webBrowser1_ProgressChanged(object sender, System.Windows.Forms.WebBrowserProgressChangedEventArgs e)
        {
            ProgressBar.Value = (int) (((double) e.CurrentProgress/e.MaximumProgress)*100);
        }

        private void webBrowser1_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            ProgressBar.Value = ProgressBar.Minimum;
        }

        private void menuClose_Click(object sender, System.EventArgs e)
        {
            Close();

        }

        private void menuHelp_Click(object sender, System.EventArgs e)
        {
            App.ShowHelp();
        }
	}
}