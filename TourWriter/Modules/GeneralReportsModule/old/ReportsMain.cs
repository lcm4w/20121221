using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info.Services;
using TourWriter.Services;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Modules.ReportsModule.old
{
    public partial class ReportsMain : ModuleBase
    {
        private readonly DataTableExcelExporter excelExporter;

        private bool isExporting
        {
            get { return (btnReport.Text == "Cancel"); }
            set { btnReport.Text = (value) ? "Cancel" : "Create Report"; }
        }

        public ReportsMain()
        {
            InitializeComponent();

            Icon = System.Drawing.Icon.FromHandle(Resources.Report.GetHicon());
            displayTypeName = "Reports";

            // initialize the excel exporter
            excelExporter = new DataTableExcelExporter();
            excelExporter.ProgressChanged += excelExporter_ProgressChanged;
            excelExporter.ExportCompleted += excelExporter_ExportCompleted;

            DataBind();
        }

        private void DataBind()
        {
            cmbReportTypes.DataSource = ReportType.GetList();
            cmbReportTypes.ValueMember = "SqlViewName";
            cmbReportTypes.DisplayMember = "DisplayName";
        }

        private void LoadData()
        {
            string viewName = (string)cmbReportTypes.SelectedValue;
            string sql = "select * from " + viewName;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                gridReport.DataSource = null;
                var ds = DataSetHelper.FillDataset(new DataSet(), sql);
                gridReport.DataSource = ds.Tables[0];
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void OpenExcelReport()
        {
            string templateFile = App.SelectExternalFile(false, "Select Excel template file", "Excel (*.xls)|*.xls", 0);
            if (String.IsNullOrEmpty(templateFile))
                return;

            string saveFile = Path.GetTempFileName() + ".xls";
            var dataTable = GridHelper.GetDataRowsTable(gridReport);

            isExporting = true;
            excelExporter.StartExport(dataTable, templateFile, saveFile);
        }

        protected override bool IsDataDirty()
        {
            return false;
        }

        protected override string GetDisplayName()
        {
            return "Reports";
        }

        #region Events

        private void excelExporter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void excelExporter_ExportCompleted(object sender, ExportCompletedEventArgs e)
        {
            if (!e.Cancelled)
                Process.Start(e.FileName);

            progressBar.Value = 0;
            isExporting = false;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if (!isExporting)
            {
                OpenExcelReport();
            }
            else
            {
                excelExporter.StopExport();
            }

        }

        private void menuHelp_Click(object sender, EventArgs e)
        {

        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void gridReport_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                GridHelper.SetDefaultCellAppearance(c);
                c.CellActivation = Activation.NoEdit;
            }

            GridHelper.SetDefaultGridAppearance(e);
            GridHelper.SetDefaultGroupByAppearance(e);

            // column chooser
            e.Layout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
            e.Layout.Override.RowSelectors = DefaultableBoolean.True;
            e.Layout.Override.RowSelectorWidth = 22;

            e.Layout.Override.RowSpacingBefore = 0;
            e.Layout.Override.SelectTypeRow = SelectType.None;
            e.Layout.AutoFitStyle = AutoFitStyle.None;

            // group-by
            e.Layout.GroupByBox.Hidden = false;
            gridReport.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

            e.Layout.Override.ActiveRowAppearance.ForeColor = System.Drawing.Color.Black;
            e.Layout.Override.ActiveRowAppearance.BackColor = System.Drawing.Color.White;


            e.Layout.Override.GroupByRowAppearance.ForeColor = System.Drawing.Color.DimGray;
            e.Layout.Override.GroupByRowAppearance.BackColor = System.Drawing.SystemColors.Control;
            e.Layout.Override.GroupByRowAppearance.BackColor2 = System.Drawing.SystemColors.Control;
        }

        #endregion
    }
}
