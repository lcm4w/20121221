using System;
using System.Collections.Generic;
using System.Data;
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

        private readonly string _reportFile;
        private OptionsForm _reportOptions;
        private OptionsForm ReportOptions
        {
            get
            {
                if (_reportOptions == null) _reportOptions = new OptionsForm(_reportFile, _defaultParams);
                return _reportOptions;
            }
        }
        private Dictionary<string, string> _dataSources;
        private Dictionary<string, object> _defaultParams;

        public ViewerControl(string reportName, string reportFile) : this(reportName, reportFile, null) { }

        public ViewerControl(string reportName, string reportFile, ICollection<KeyValuePair<string, object>> defaultParameters)
        {
            InitializeComponent();

            lblReportName.Text = reportName;
            _reportFile = reportFile;
            _dataSources = new Dictionary<string, string>();

            // clone params
            _defaultParams = new Dictionary<string, object>(defaultParameters.Count);
            foreach (var param in defaultParameters) _defaultParams.Add(param.Key, param.Value);
            _defaultParams.Add("@ReportName", reportName);
        }

        public void RunReport()
        {
            try
            {
                btnRefresh.Enabled = btnOptions.Enabled = btnEmail.Enabled = false;
                if (TopLevelControl != null) TopLevelControl.Cursor = Cursors.WaitCursor;
                else Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                // initialise
                reportViewer.Reset();
                reportViewer.LocalReport.ReportPath = _reportFile;
                reportViewer.LocalReport.DisplayName = lblReportName.Text;
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.EnableExternalImages = true;
                reportViewer.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence);
                reportViewer.LocalReport.SubreportProcessing += LocalReportSubreportProcessing;
                // get report params, merge into our default params list to ensure we have them all covered
                foreach (var param in reportViewer.LocalReport.GetParameters())
                {
                    var key = !param.Name.StartsWith("@") ? "@" + param.Name : param.Name;
                    if (!_defaultParams.ContainsKey(key) && param.Values.Count > 0 && param.Values[0] != null)
                        _defaultParams.Add(key, param.Values[0]);
                }

                // load dynamic options
                ReportOptions.ProcessOptions(ref _defaultParams, ref _dataSources);

                // set report param values
                var reportParams = new List<ReportParameter>();
                foreach (var param in reportViewer.LocalReport.GetParameters())
                {
                    var key = !param.Name.StartsWith("@") ? "@" + param.Name : param.Name;
                    if (_defaultParams.ContainsKey(key))
                        reportParams.Add(new ReportParameter(key.TrimStart('@'), _defaultParams[key].ToString()));
                }
                reportViewer.LocalReport.SetParameters(reportParams.ToArray());
                
                // set data
                foreach (var dataSource in _dataSources)
                {
                    var sql = dataSource.Value;
                    var data = DataSetHelper.FillDataSetFromSql(sql).Tables[0];
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSource.Key, data));  
                }
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
            catch (Exception ex)
            {
                App.Error(string.Format("Report error for '{0}' ({1}). Message: {2}", reportViewer.LocalReport.DisplayName, reportViewer.LocalReport.ReportPath, ex.Message), ex, true);
            }
            finally
            {
                btnRefresh.Enabled = btnOptions.Enabled = btnEmail.Enabled = true;
                if (TopLevelControl != null) TopLevelControl.Cursor = Cursors.Default;
                else Cursor = Cursors.Default;
            }
        }


        private void ShowOptionsForm()
        {
            if (ReportOptions.ShowDialog() == DialogResult.OK)
                RunReport();
        }

        private void SendEmail()
        {
            var emailBuilder = new ReportEmailBuilder(reportViewer.LocalReport)
                                   {
                                       TemplateSubject =
                                           lblReportName.Text + (ParentForm != null ? ": " + ParentForm.Text : "")
                                   };

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

        private void viewerControl_Load(object sender, EventArgs e)
        {
            RunReport();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RunReport();
        }

        private void btnOptions_Click(object sender, EventArgs e)
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

        private static void LocalReportSubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            //string dataSourceName = e.DataSourceNames[0];
            //if (!_dataSourcesCache.ContainsKey(dataSourceName))
            //{
            //    string reportFolder = Path.GetDirectoryName(_reportFile);
            //    string filename = String.Format(@"{0}\{1}.rdlc", reportFolder, e.ReportPath);

            //    var options = new OptionsForm(filename, _defaultParameters);
            //    var dataSource = GetData(options.ProcessOptions());
            //    _dataSourcesCache.Add(dataSourceName, dataSource);
            //}
            //e.DataSources.Clear();
            //e.DataSources.Add(new ReportDataSource(dataSourceName, _dataSourcesCache[dataSourceName]));
        }

        #endregion
    }
}