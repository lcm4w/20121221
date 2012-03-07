using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using TourWriter.Global;
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
                if (_reportOptions == null) _reportOptions = new OptionsForm(_reportFile, _reportParams);
                return _reportOptions;
            }
        }
        private Dictionary<string, string> _dataSources;
        private Dictionary<string, object> _reportParams;

        public ViewerControl(string reportName, string reportFile) : this(reportName, reportFile, null) { }

        public ViewerControl(string reportName, string reportFile, ICollection<KeyValuePair<string, object>> generalParameters)
        {
            InitializeComponent();

            lblReportName.Text = reportName;
            _reportFile = reportFile;
            _dataSources = new Dictionary<string, string>();

            // clone params for this report
            _reportParams = new Dictionary<string, object>(generalParameters.Count);
            foreach (var param in generalParameters) _reportParams.Add(param.Key, param.Value);

            // Load event call RunReport();
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
                reportViewer.LocalReport.EnableHyperlinks = true;
                reportViewer.LocalReport.ExecuteReportInCurrentAppDomain(System.Reflection.Assembly.GetExecutingAssembly().Evidence);
                reportViewer.LocalReport.SubreportProcessing += LocalReportSubreportProcessing;

                // get params from report file, merge into our params list to ensure we have them all covered
                foreach (var param in reportViewer.LocalReport.GetParameters())
                {
                    var key = !param.Name.StartsWith("@") ? "@" + param.Name : param.Name;
                    if (!_reportParams.ContainsKey(key) && param.Values.Count > 0 && param.Values[0] != null)
                        _reportParams.Add(key, param.Values[0]);
                }

                // refresh report params (user filter options)
                ReportOptions.ProcessReportFilterOptions(ref _reportParams, ref _dataSources); // refresh params from user filters

                // refresh general params (possible changes in itinerary etc)
                RefreshGeneralParams();
                
                // set report embedded params
                SetReportParamValues();

                // set report sql and/or embedded datasources
                SetReportDataSources();
                
                // run report
                reportViewer.RefreshReport();
            }
            catch (LocalProcessingException ex)
            {
                var localEx = ex.InnerException;
                while (localEx.InnerException != null) 
                    localEx = localEx.InnerException;
                throw localEx;
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

        private void SetReportDataSources()
        {
            foreach (var dataSource in _dataSources)
            {
                DataTable data;
                var isClientDataSource = dataSource.Key.ToLower().EndsWith("_client");

                if (isClientDataSource) // load from client app
                {
                    data = GetClientAppDataSource(dataSource.Key);
                }
                else // isSqlDataSource, load with sql query
                {
                    var sql = dataSource.Value;
                    data = DataSetHelper.FillDataSetFromSql(sql).Tables[0];
                }
                var isDefaultReport = _reportFile.ToLower().Contains(App.Path_DefaultTemplatesFolder.ToLower());
                if (isDefaultReport) EnsureDefaultReportImagesAreFound(data);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSource.Key, data));
            }
        }

        private void RefreshGeneralParams()
        {
            // refresh some general params
            if (!_reportParams.ContainsKey("@ReportName")) _reportParams.Add("@ReportName", "");
            _reportParams["@ReportName"] = lblReportName.Text;
            
            // refresh itinerary general params (in case user has changed itineray)
            if (ParentForm is Modules.ItineraryModule.ItineraryMain)
            {
                var itineraryForm = ParentForm as Modules.ItineraryModule.ItineraryMain;
                var itinerary = itineraryForm.ItinerarySet.Itinerary[0];
                var agent = Global.Cache.ToolSet.Agent.Where(a => a.AgentID == itinerary.AgentID).FirstOrDefault();
                
                if (_reportParams.ContainsKey("@ItineraryID"))
                {
                    _reportParams["@ItineraryID"] = lblReportName.Text;
                }

                if (_reportParams.ContainsKey("@ItineraryID"))
                {
                    _reportParams["@ItineraryID"] = itinerary.ItineraryID;
                }
                if (_reportParams.ContainsKey("@ItineraryCurrencyFormat"))
                {
                    var format = CurrencyService.GetItineraryCurrencyCode(itinerary) != null ?
                        CurrencyService.GetCurrency(itinerary.CurrencyCode).DisplayFormat : "c";
                    _reportParams["@ItineraryCurrencyFormat"] = format;
                }
                if (_reportParams.ContainsKey("@CurrencyCode"))
                {
                    var code = CurrencyService.GetItineraryCurrencyCodeOrDefault(itinerary);
                    _reportParams["@CurrencyCode"] = code;
                }
                if (_reportParams.ContainsKey("@LogoFile") && agent != null)
                {
                    var logo = !agent.IsVoucherLogoFileNull() ? ExternalFilesHelper.ConvertToAbsolutePath(agent.VoucherLogoFile) : "";
                    _reportParams["@LogoFile"] = "file:\\\\\\" + logo;
                }
                if (_reportParams.ContainsKey("@AgentVoucherNote") && agent != null)
                {
                    var note = !agent.IsVoucherFooterNull() ? agent.VoucherFooter : "";
                    _reportParams["@AgentVoucherNote"] = note;
                }
                if (_reportParams.ContainsKey("@AgentClientFooter") && agent != null)
                {
                    var note = !agent.IsClientFooterNull() ? agent.ClientFooter : "";
                    _reportParams["@AgentClientFooter"] = note;
                }
                if (_reportParams.ContainsKey("@AgentHeader") && agent != null)
                {
                    var note = !agent.IsAgentHeaderNull() ? agent.AgentHeader : "";
                    _reportParams["@AgentHeader"] = note;
                }
            }
        }
        
        private void SetReportParamValues()
        {
            // set report param values
            var reportParams = new List<ReportParameter>();
            foreach (var param in reportViewer.LocalReport.GetParameters())
            {
                var key = !param.Name.StartsWith("@") ? "@" + param.Name : param.Name;
                if (_reportParams.ContainsKey(key))
                    reportParams.Add(new ReportParameter(key.TrimStart('@'), _reportParams[key].ToString()));
            }
            reportViewer.LocalReport.SetParameters(reportParams.ToArray());
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

        private DataTable GetClientAppDataSource(string key)
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
                var quoteTable = (ParentForm as Modules.ItineraryModule.ItineraryMain).GetRefreshQuoteTable;
         
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
                var quoteTable = (ParentForm as Modules.ItineraryModule.ItineraryMain).GetRefreshQuoteTable;
                dt = quoteTable.Copy();
                dt.TableName = key;
            }

            // dump the detail columns (name, desc etc - not prices) from the booking grid
            // note: these cols are static, so can be safely use in report at design time 
            else if (key == "GroupQuoteDumpDetail_client")
            {
                var quoteTable = (ParentForm as Modules.ItineraryModule.ItineraryMain).GetRefreshQuoteTable;
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
                var quoteTable = (ParentForm as Modules.ItineraryModule.ItineraryMain).GetRefreshQuoteTable;

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
            // for itinerary reports, add list of possible email recipients
            var emails = new Dictionary<string, string>();
            if (ParentForm is Modules.ItineraryModule.ItineraryMain)
            {
                var itin = (ParentForm as Modules.ItineraryModule.ItineraryMain).ItinerarySet.Itinerary[0];

                // add agent
                if (!itin.IsAgentIDNull())
                {
                    var agent = Cache.AgentSet.Agent.First(x => x.RowState != DataRowState.Deleted && x.AgentID == itin.AgentID);
                    if (agent != null && !agent.IsEmailNull())
                        emails.Add(agent.AgentName, agent.Email);
                }

                // add agent contact
                if (!itin.IsAgentContactIDNull())
                {
                    var contact = Cache.AgentSet.AgentContact.First(x => x.RowState != DataRowState.Deleted && x.ContactID == itin.AgentContactID);
                    if (contact != null && contact.ContactRow != null && !contact.ContactRow.IsEmail1Null())
                        emails.Add(contact.Description, contact.ContactRow.Email1);
                }

                // add clients
                foreach (var mem in (ParentForm as Modules.ItineraryModule.ItineraryMain).ItinerarySet.ItineraryMember)
                {
                    if (mem.ContactRow != null && !mem.ContactRow.IsEmail1Null())
                        emails.Add(mem.ItineraryMemberName, mem.ContactRow.Email1);
                }
            }

            var emailBuilder = new ReportEmailBuilder(reportViewer.LocalReport) {TemplateSubject = lblReportName.Text + (ParentForm != null ? ": " + ParentForm.Text : "")};
            var wizard = new Wizard(Settings.Default.EmailEditorSize) {Text = "TourWriter report email", Params = emailBuilder};

            wizard.AddPage(new TemplateForm());
            wizard.AddPage(new EmailForm(new List<string>(emails.Select(e => "\"" + e.Key + "\" <" + e.Value + ">"))));
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
        }

        #endregion
    }
}