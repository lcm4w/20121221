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

namespace TourWriter.Modules.DataExtract.UserControls
{
    public partial class RatesExport : UserControl
    {
        private readonly SupplierDetailExcelExporter excelExporter;

        private bool isExporting
        {
            get { return (btnExcel.Text == "Cancel"); }
            set { btnExcel.Text = (value) ? "Cancel" : "Excel"; }
        }

        private void RatesExport_Load(object sender, EventArgs e)
        {
            gridReport.UltraGrid.InitializeLayout += gridReport_InitializeLayout;
        }

        public RatesExport()
        {
            InitializeComponent();

            // initialize the excel exporter
            excelExporter = new SupplierDetailExcelExporter();
            excelExporter.ProgressChanged += excelExporter_ProgressChanged;
            excelExporter.ExportCompleted += excelExporter_ExportCompleted;
        }

        private void LoadData()
        {
            if (txtStartDate.Value == null || txtEndDate.Value == null)
            {
                App.ShowError("Please select a start/end date");
                return;
            }

            string sql =
@"declare @StartDate datetime
 declare @EndDate  datetime
 set @StartDate = '{0}'
 set @EndDate = '{1}'
      
 select * from SupplierRatesDetail
 where convert(char(8), @StartDate, 112) <= convert(char(8), RateValidTo, 112)
 and convert(char(8), @EndDate, 112) >= convert(char(8), RateValidFrom, 112)";

            string startDate = ((DateTime)txtStartDate.Value).ToString("yyyy-MM-dd");
            string endDate = ((DateTime)txtEndDate.Value).ToString("yyyy-MM-dd");
            sql = String.Format(sql, startDate, endDate);

            try
            {
                Cursor.Current = Cursors.WaitCursor;

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
            var dataTable = GridHelper.GetDataRowsTable(gridReport.UltraGrid, true);

            isExporting = true;
            excelExporter.StartExport(dataTable, templateFile, saveFile);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            LoadData();
            Cursor = Cursors.Default;
        }

        private void btnExcel_Click(object sender, EventArgs e)
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

        private void gridReport_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!(sender is UltraGrid)) return;

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
            gridReport.UltraGrid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

            e.Layout.Override.ActiveRowAppearance.ForeColor = System.Drawing.Color.Black;
            e.Layout.Override.ActiveRowAppearance.BackColor = System.Drawing.Color.White;


            e.Layout.Override.GroupByRowAppearance.ForeColor = System.Drawing.Color.DimGray;
            e.Layout.Override.GroupByRowAppearance.BackColor = System.Drawing.SystemColors.Control;
            e.Layout.Override.GroupByRowAppearance.BackColor2 = System.Drawing.SystemColors.Control;
        }
    }
}
