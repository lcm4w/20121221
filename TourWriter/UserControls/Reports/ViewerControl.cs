using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using TourWriter.Info.Services;
using TourWriter.Modules.Emailer;
using TourWriter.Modules.ItineraryModule.Bookings;
using TourWriter.Properties;
using TourWriter.Services;

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
                // reset default params
                // currency format
                var itinerary = (ParentForm as Modules.ItineraryModule.ItineraryMain).ItinerarySet.Itinerary[0];
                var hasOverride = CurrencyService.GetItineraryCurrencyCode(itinerary) != null;
                var format = hasOverride ? CurrencyService.GetCurrency(itinerary.CurrencyCode).DisplayFormat : "c";
                (ParentForm as Modules.ItineraryModule.ItineraryMain).SetItineraryReportsParameter("@ItineraryCurrencyFormat", format);
                
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
                    DataTable data;

                    // client datasource
                    if (dataSource.Key.ToLower().EndsWith("_client"))
                    {
                        data = GetDataSourceFromClient(dataSource.Key);
                    }
                    // sql datasource
                    else
                    {
                        var sql = dataSource.Value;
                        data = DataSetHelper.FillDataSetFromSql(sql).Tables[0];
                    }
                    var isDefaultReport = _reportFile.ToLower().Contains(App.Path_DefaultTemplatesFolder.ToLower());
                    if (isDefaultReport) EnsureDefaultReportImagesAreFound(data);
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSource.Key, data));
                }
                reportViewer.RefreshReport();
            }
            catch (LocalProcessingException ex)
            {
                var localEx = ex.InnerException;
                while (localEx.InnerException != null) localEx = localEx.InnerException;
                throw localEx;

                //if (localEx.Message.ToLower().StartsWith("the report definition is not valid"))
                //    throw new FormatException(localEx.Message);
                //throw;
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

        private void EnsureDefaultReportImagesAreFound(DataTable table)
        {
            var cols = table.Columns.Cast<DataColumn>().Where(x => x.ColumnName.IndexOf("Image") > -1).ToList();
            
            foreach (var row in table.Rows.Cast<DataRow>())
                foreach (var col in cols.Where(col => row[col.ColumnName] != DBNull.Value))
                    row[col.ColumnName] = EnsureDefaultReportImageIsFound(row[col.ColumnName].ToString());
        }

        private string EnsureDefaultReportImageIsFound(string imageFile)
        {
            var isUriFormat = imageFile.StartsWith("file://");
            if (isUriFormat) imageFile = imageFile.Replace("file://", "");

            if (!File.Exists(imageFile))
            {
                var imageName = new FileInfo(imageFile).Name;
                var newSrc = Path.Combine(Path.Combine(App.Path_DefaultTemplatesFolder, "Images"), imageName);
                if (File.Exists(newSrc)) imageFile = newSrc;
            }
            return isUriFormat ? "file://" + imageFile : imageFile;
        }

        private DataTable GetDataSourceFromClient(string key)
        {
            DataTable dt = null;

            #region Quotes table, as seen in booking quote UI

            // group bookings detail and/or price, in a 'matrix' schema (row, col, val) to suit 'dynamic' cols in RDL
            // this allows RDL to dynamically create grid cols (runtime), when it does not know the number/name of cols at designtime
            // more info: http://sonalimendis.blogspot.com/2011/07/dynamic-column-rdls.html
            if (key == "GroupQuoteDynamicAll_client" || key == "GroupQuoteDynamicPrices_client")
            {
                // create a 'matrix' table with cols: (row number, column name, data value)
                // note: using rownumber because no unique id in quote table
                dt = new DataTable(key);
                dt.Columns.AddRange(new[] { new DataColumn("row", typeof(int)), new DataColumn("col", typeof(string)), new DataColumn("val", typeof(string)) });
                var quoteTable = (ParentForm as Modules.ItineraryModule.ItineraryMain).BookingsQuoteTable;
         
                var cnt = 0;
                foreach (DataRow row in quoteTable.Rows)
                {
                    cnt++;
                    foreach (DataColumn col in quoteTable.Columns)
                    {
                        if (key == "GroupQuotePrices_client" && // only want prices
                            !(col is QuoteTable.PaxColumn || col is QuoteTable.SuppliementsColumn)) // not a price col
                            continue; // skip detail cols

                        // format
                        var val = row[col.ColumnName];
                        if (col.DataType == typeof(DateTime) && !string.IsNullOrEmpty(val.ToString()))
                            val = DateTime.Parse(val.ToString()).ToShortDateString();
                        if (col.DataType == typeof(Decimal) && !string.IsNullOrEmpty(val.ToString()))
                            val = Math.Round((decimal)val, 2).ToString("0.00");

                        dt.Rows.Add(cnt, col.ColumnName, val);
                    }
                }
            }

            // dump entire bookings grid
            else if (key == "GroupQuoteDumpAll_client")
            {
                var quoteTable = (ParentForm as Modules.ItineraryModule.ItineraryMain).BookingsQuoteTable;
                dt = quoteTable.Copy();
                dt.TableName = key;
            }

            // dump the detail columns (name, desc etc - not prices) from the booking grid
            // note: these cols are static, so can be safely use in report at design time 
            else if (key == "GroupQuoteDumpDetail_client")
            {
                var quoteTable = (ParentForm as Modules.ItineraryModule.ItineraryMain).BookingsQuoteTable;
                dt = quoteTable.Copy();
                dt.TableName = key;

                // remove price cols
                for (var i = dt.Columns.Count - 1; i > -1; i--)
                    if (dt.Columns[i] is QuoteTable.PaxColumn || dt.Columns[i] is QuoteTable.SuppliementsColumn)
                        dt.Columns.RemoveAt(i);
            }
            #endregion

            #region Pax and suppliment tables with subtotals and totals

            // pax and suppliments price summary in basic pax+suppliments table
            else if (key == "GroupQuotePriceSummary_client")
            {
                throw new NotImplementedException();
            }

            // pax price summary in basic pax table
            else if (key == "GroupQuotePriceSummaryPax_client")
            {
                var quoteTable = (ParentForm as Modules.ItineraryModule.ItineraryMain).BookingsQuoteTable;

                dt = new DataTable("ItineraryPax");
                dt.Columns.AddRange(new []
                                           {
                                               new DataColumn("PaxName", typeof(string)),
                                               new DataColumn("Subtotal", typeof(decimal)),
                                               new DataColumn("Total", typeof(decimal))
                                           });
                foreach (var row in (ParentForm as Modules.ItineraryModule.ItineraryMain).ItinerarySet.ItineraryPax)
                {
                    var name = row.ItineraryPaxName;
                    var subtotal = quoteTable.AsEnumerable().Sum(x => x.Field<decimal?>(name));
                    
                    decimal? total;   
                    if (!row.IsGrossOverrideNull())
                        total = row.GrossOverride;
                    else if (!row.IsGrossMarkupNull())
                        total = subtotal * (1 + row.GrossMarkup / 100);
                    else total = subtotal;

                    dt.Rows.Add(row.ItineraryPaxName, subtotal, total);
                }
            }

            // suppliments price summary in basic suppliments table
            else if (key == "GroupQuotePriceSummarySuppliments_client")
            {
                throw new NotImplementedException();
            }
            #endregion

            return dt;
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