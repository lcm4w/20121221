using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using TourWriter.Info.Services;
using TourWriter.Modules.Emailer;
using TourWriter.Properties;

namespace TourWriter.UserControls.Reports
{
    public partial class ViewerControl : UserControl
    {
        public event EventHandler ViewerControlClosed;

        private readonly string reportFile;
        private OptionsForm _reportOptions;
        private OptionsForm reportOptions
        {
            get
            {
                if (_reportOptions == null) 
                    _reportOptions = new OptionsForm(reportFile, defaultParameters);
                return _reportOptions;
            }
        }
        private readonly Dictionary<string, object> defaultParameters;
        private readonly Dictionary<string, DataTable> dataSourcesCache;

        public ViewerControl(string reportName, string reportFile) : this(reportName, reportFile, null) { }

        public ViewerControl(string reportName, string reportFile, ICollection<KeyValuePair<string, object>> defaultParameters)
        {
            InitializeComponent();

            lblReportName.Text = reportName;
            this.reportFile = reportFile;
            this.defaultParameters = DefaultParamsClone(defaultParameters);
            this.defaultParameters.Add("@ReportName", reportName);
            dataSourcesCache = new Dictionary<string, DataTable>();

            InitialiseReport();
        }

        private static Dictionary<string, object> DefaultParamsClone(ICollection<KeyValuePair<string, object>> srcParameters)
        {
            var clone = new Dictionary<string, object>(srcParameters.Count);
            foreach (var param in srcParameters)
                clone.Add(param.Key, param.Value);
            return clone;
        }

        private void InitialiseReport()
        {
            reportViewer.Reset();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence);
            reportViewer.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
        }

        private void viewerControl_Load(object sender, EventArgs e)
        {
            if (chkRunImmediate.Checked)
                RunReport();
        }

        private string sqlCached;
        public void RunReport()
        {
            if (!LoadReport()) return;
            try
            {
                if (TopLevelControl != null) TopLevelControl.Cursor = Cursors.WaitCursor;
                else Cursor = Cursors.WaitCursor;

                Application.DoEvents();
                ProcessReport();
                reportViewer.RefreshReport();
            }
            catch (LocalProcessingException ex)
            {
                var localEx = ex.InnerException;
                while (localEx.InnerException != null) localEx = localEx.InnerException;

                if (localEx.Message.ToLower().StartsWith("the report definition is not valid"))
                    throw new FormatException(localEx.Message);
                throw;
            }
            finally
            {
                if (TopLevelControl != null) TopLevelControl.Cursor = Cursors.Default;
                else Cursor = Cursors.Default;
            }
        }

        private bool LoadReport()
        {
            // set report
            reportViewer.Reset();
            reportViewer.LocalReport.ReportPath = reportFile;
            reportViewer.LocalReport.DisplayName = lblReportName.Text;

            // load default params from report
            foreach (var param in reportViewer.LocalReport.GetParameters())
            {
                var key = !param.Name.StartsWith("@") ? "@" + param.Name : param.Name;
                if (!defaultParameters.ContainsKey(key) && param.Values.Count > 0 && param.Values[0] != null)
                    defaultParameters.Add(key, param.Values[0]);
            }
            return true;
        }

        private void ProcessReport()
        {
            // get default or user options
            var sql = reportOptions.ProcessOptions();

            // set report embedded parameters
            var reportParams = new List<ReportParameter>();
            foreach (var param in reportViewer.LocalReport.GetParameters())
            {
                var key = !param.Name.StartsWith("@") ? "@" + param.Name : param.Name;
                if (defaultParameters.ContainsKey(key))
                    reportParams.Add(new ReportParameter(key.TrimStart('@'), defaultParameters[key].ToString()));
            }
            reportViewer.LocalReport.SetParameters(reportParams.ToArray());

            // load sql data
            if (sql != sqlCached)
            {
                dataSourcesCache.Clear();
                sqlCached = sql;
            }

            var dataSourceName = reportViewer.LocalReport.GetDataSourceNames()[0];
            if (!dataSourcesCache.ContainsKey(dataSourceName))
            {
                var dataSource = LoadData(sql);
                dataSourcesCache.Add(dataSourceName, dataSource);
            }
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName, dataSourcesCache[dataSourceName]));
        }

        private void ShowOptionsForm()
        {
            if (reportOptions.ShowDialog() == DialogResult.OK)
                RunReport();
        }

        private DataTable LoadData(string sql)
        {
            try
            {
                Invoke(new MethodInvoker(delegate { Cursor = Cursors.WaitCursor; }));
                return DataSetHelper.FillDataSetFromSql(sql).Tables[0];
            }
            finally
            {
                Invoke(new MethodInvoker(delegate { Cursor = Cursors.Default; }));
            }
        }

        private void SendEmail()
        {
            var emailBuilder = new ReportEmailBuilder(reportViewer.LocalReport);
            emailBuilder.TemplateSubject = lblReportName.Text + (ParentForm != null ? ": " + ParentForm.Text : "");

            var wizard = new Wizard(Settings.Default.EmailEditorSize)
                             {
                                 Text = "TourWriter report email",
                                 Params = emailBuilder
                             };
            wizard.AddPage(new TemplateForm());
            wizard.AddPage(new EmailForm());
            wizard.AddPage(new SendForm());
            wizard.Show(this);
            wizard.Next();
        }

        #region Events

        private void btnRun_Click(object sender, EventArgs e)
        {
            RunReport();
        }

        private void btnChangeMargins_Click(object sender, EventArgs e)
        {
            ShowOptionsForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ViewerControlClosed(this, new EventArgs());
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            SendEmail();
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            string dataSourceName = e.DataSourceNames[0];
            if (!dataSourcesCache.ContainsKey(dataSourceName))
            {
                string reportFolder = Path.GetDirectoryName(reportFile);
                string filename = String.Format(@"{0}\{1}.rdlc", reportFolder, e.ReportPath);

                var options = new OptionsForm(filename, defaultParameters);
                var dataSource = LoadData(options.ProcessOptions());
                dataSourcesCache.Add(dataSourceName, dataSource);
            }
            e.DataSources.Clear();
            e.DataSources.Add(new ReportDataSource(dataSourceName, dataSourcesCache[dataSourceName]));
        }

        #endregion
    }
}