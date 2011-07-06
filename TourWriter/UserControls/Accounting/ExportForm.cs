using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info.Services;
using TourWriter.Properties;

namespace TourWriter.UserControls.Accounting
{
    public partial class ExportForm : Form
    {
        private string _templateFile;
        private string _exportFileName;
        private IEnumerable<DataRow> _purchases;
        private IEnumerable<DataRow> _allocations;
        private bool IsSales { get { return _purchases == null; } }

        public ExportForm()
        {
            InitializeComponent();

            myobLabel.Visible = myobStatus.Visible = (Cache.ToolSet.AppSettings[0].AccountingSystem.ToLower() == "myob");
        }

        public void ExportPurchases(IEnumerable<DataRow> purchases, string templateFile, string exportFileName)
        {
            _purchases = purchases;
            _templateFile = templateFile;
            _exportFileName = exportFileName;
            textBox1.Text = GetDefaultFilePath();
            lblHeading.Text = "Purchases accounting export";
        }

        public void ExportSales(IEnumerable<DataRow> sales, string templateFile, string exportFileName)
        {
            _templateFile = templateFile;
            _exportFileName = exportFileName;
            textBox1.Text = GetDefaultFilePath();
            lblHeading.Text = "Purchases accounting export";

            // start loading the sale allocations (the actual export data) in the background
            _timestamp = DateTime.Now;
            var saleIds = sales.Select(x => x.Field<int>("ItinerarySaleID"));
            new Thread(() => { _allocations = LoadAllocationData(saleIds, _templateFile); }) {Name = "Load_accounting_allocations"}.Start();
        }

        internal IEnumerable<DataRow> LoadAllocationData(IEnumerable<int> saleIds, string templateFile)
        {
            if (saleIds.Count() == 0) return null;

            var cols = string.Join(",", Accounting.GetTemplateTagNames("Sales").Where(x => x != "SaleStatus").ToArray());
            var ids = string.Join(",", saleIds.Select(x => x.ToString()).ToArray());
            var sql = string.Format("select {0} from ItinerarySaleAllocationDetail where ItinerarySaleID in ({1})", cols, ids);
            
            var ds = DataSetHelper.FillDataSetFromSql(sql);
            App.PrepareDataTableForExport(ds.Tables[0]);

            IEnumerable<DataRow> rows = ds.Tables[0].AsEnumerable();
            return rows;
        }

        private DateTime _timestamp;
        private void WaitForAllocationLoad(int seconds)
        {
            if (_allocations == null)
            {
                Cursor = Cursors.WaitCursor;

                while (_allocations == null && ((DateTime.Now - _timestamp).Seconds < seconds))
                    Thread.Sleep(1000);

                Cursor = Cursors.Default;
            }
        }

        private void Export()
        {
            if (IsSales)
            {
                WaitForAllocationLoad(60);
                if (_allocations == null) return;
            }
            
            // get accounting system interface
            Services.Accounting.IExport exportInterface = null;
            var type = Cache.ToolSet.AppSettings[0].AccountingSystem.ToLower();
            switch (type)
            {
                case "myob":
                    exportInterface = new Services.Accounting.CsvExportMyob();
                    break;
                case "xero":
                    exportInterface = new Services.Accounting.CsvExportXero();
                    break;
                default:
                    App.ShowError("Accounting system interface not set, see Tools > Setup > Accounting.");
                    return;
            }
            
            // export
            var outputFile = textBox1.Text;
            var dataSource = IsSales ? _allocations : _purchases;
            InsertMyobData(dataSource, IsSales ? "SaleStatus" : "PurchaseStatus", myobStatus.SelectedItem);
            exportInterface.DataSource = dataSource.CopyToDataTable();
            exportInterface.TemplateFile = _templateFile;
            exportInterface.ExportTo(outputFile);

            var dir = new FileInfo(textBox1.Text).DirectoryName;
            if (checkBox1.Checked) System.Diagnostics.Process.Start(dir);
        }

        private void InsertMyobData(IEnumerable<DataRow> rows, string statusColumnName, object value)
        {
            if (rows.Count() == 0) return;
            var table = rows.First().Table;

            // add column
            DataColumn col;
            if (!table.Columns.Contains(statusColumnName))
            {
                col = new DataColumn(statusColumnName);
                table.Columns.Add(col);
            }
            else col = table.Columns[statusColumnName];

            // set data
            col.Expression = (value == null || string.IsNullOrEmpty(value.ToString())) ? "" : string.Format("'{0}'", value);
            int i = 2;
        }


        private string GetDefaultFilePath()
        {
            if (!string.IsNullOrEmpty(textBox1.Text)) return textBox1.Text;

            var dir = "";
            if (!string.IsNullOrEmpty(Settings.Default.LastAccountingDir))
                dir = Settings.Default.LastAccountingDir;
            else if (!Directory.Exists(dir) && Cache.ToolSet.AppSettings.Rows.Count > 0 && !Cache.ToolSet.AppSettings[0].IsExternalFilesPathNull())
                dir = Cache.ToolSet.AppSettings[0].ExternalFilesPath;
            return Path.Combine(dir, _exportFileName);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var fi = new FileInfo(GetDefaultFilePath());
            var dir = App.PromptChooseDir(fi.DirectoryName);
            if (Directory.Exists(dir))
            {
                textBox1.Text = Path.Combine(dir,  fi.Name);
                Settings.Default.LastAccountingDir = dir;
                Settings.Default.Save();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Export();
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
