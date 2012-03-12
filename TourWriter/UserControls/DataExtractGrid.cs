using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using TourWriter.Services;

namespace TourWriter.UserControls
{
    public partial class DataExtractGrid : UserControl
    {
        #region Properties

        public event InitializeLayoutEventHandler InitializeLayoutEvent;

        private string exportFileName = String.Empty;

        public bool GridEnabled
        {
            get { return grid.Enabled;  }
            set { grid.Enabled = value; }
        }

        internal MyToolStrip ToolStrip
        {
            get { return myToolStrip1; }
        }
        
        public object DataSource
        {
            get { return grid.DataSource; }
            set { grid.DataSource = value; }
        }

        public string ExportFileName
        {
            get { return exportFileName; }
            set { exportFileName = value; }
        }

        public UltraGrid UltraGrid
        {
            get { return grid; }
        }

        /// <summary>
        /// Gets all the data rows in a data table, without group by rows etc.
        /// </summary>
        internal DataTable GetDataRowsTable()
        {
            return GridHelper.GetDataRowsTable(grid);
        }

        /// <summary>
        /// Gets all the data rows in a list, without group by rows etc.
        /// </summary>
        internal List<UltraGridRow> GetDataRowsList()
        {
            return GridHelper.GetDataRowsList(grid);
        }

        #endregion

        public DataExtractGrid()
        {
            InitializeComponent();
        }

        protected virtual void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (InitializeLayoutEvent != null)
                InitializeLayoutEvent(sender, e);

            Services.GridHelper.SetDefaultGridAppearance(e);
            Services.GridHelper.SetDefaultGroupByAppearance(e);

            // column chooser
            e.Layout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
            e.Layout.Override.RowSelectors = DefaultableBoolean.True;
            e.Layout.Override.RowSelectorWidth = 22;

            e.Layout.Override.RowSpacingBefore = 0;
            e.Layout.Override.SelectTypeRow = SelectType.None;// Extended;
            e.Layout.AutoFitStyle = AutoFitStyle.None;

            // group-by
            e.Layout.GroupByBox.Hidden = false;
            grid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

            e.Layout.Override.ActiveRowAppearance.ForeColor = System.Drawing.Color.Black;
            e.Layout.Override.ActiveRowAppearance.BackColor = System.Drawing.Color.White;


            e.Layout.Override.GroupByRowAppearance.ForeColor = System.Drawing.Color.DimGray;
            e.Layout.Override.GroupByRowAppearance.BackColor = System.Drawing.SystemColors.Control;
            e.Layout.Override.GroupByRowAppearance.BackColor2 = System.Drawing.SystemColors.Control;

        }
        
        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (btnExpand.Text == "Expand")
            {
                foreach (UltraGridRow row in grid.Rows)
                    row.ExpandAll();
                btnExpand.Image = TourWriter.Properties.Resources.Collapse;
                btnExpand.Text = "Collapse";
            }
            else
            {
                foreach (UltraGridRow row in grid.Rows)
                    row.CollapseAll();
                btnExpand.Image = TourWriter.Properties.Resources.Expand;
                btnExpand.Text = "Expand";
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            grid.DisplayLayout.Override.AllowRowFiltering = DefaultableBoolean.True;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            UltraGridPrintDocument doc = new UltraGridPrintDocument();
            doc.RowProperties = RowPropertyCategories.All;
            doc.DefaultPageSettings.Landscape = true;
            doc.Grid = grid;

            /********************************************************** 
             * UltraGridPrintDocument issue is causing grid data to not
             * print properly if FitWidthToPages = 1 and either header 
             * or footer text is added.
             **********************************************************/
            doc.FitWidthToPages = 1;
            /*
            doc.DocumentName = "BookingList_" + itinerarySet.Itinerary[0].ItineraryName.Substring(0, 3);
            doc.Header.TextLeft =
                String.Format("Bookings for: {0}\r\n{1} - {2}",
                              itinerarySet.Itinerary[0].ItineraryName,
                              itinerarySet.Itinerary[0].ArriveDate.ToShortDateString(),
                              itinerarySet.Itinerary[0].DepartDate.ToShortDateString());
            doc.Header.TextRight = DateTime.Now.ToShortDateString();
            doc.Footer.TextCenter = "[Page #]";               
             */

            UltraPrintPreviewDialog preview;
            preview = new UltraPrintPreviewDialog();
            preview.Document = doc;
            preview.ShowDialog();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (grid.Rows.Count == 0)
                return;

            string templateFileName = "";//App.SelectExternalFile(false, "Select Excel template file", "Excel (*.xls)|*.xls", 0);
            string saveFileName = App.PromptSaveFile(exportFileName, "Excel", ".xls", false);
            
            if (String.IsNullOrEmpty(saveFileName))
                return;

            try
            {
                Cursor = Cursors.WaitCursor;

                if (String.IsNullOrEmpty(templateFileName))
                {
                    // if no template specified, use standard ultra grid exporter
                    UltraGridExcelExporter excelExporter = new UltraGridExcelExporter();
                    excelExporter.Export(grid, saveFileName);
                }
                else
                {
                    GridExcelExporter exporter = new GridExcelExporter(grid);
                    exporter.Export(templateFileName, saveFileName);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnLoadLayout_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog {Title = "Open Grid Layout file", Filter = "layout files (*.xml)|*.xml|All files (*.*)|*.*", Multiselect = false};
            if (dlg.ShowDialog() == DialogResult.OK) 
                grid.DisplayLayout.LoadFromXml(dlg.FileName, PropertyCategories.All);
        }

        private void btnSaveLayout_Click(object sender, EventArgs e)
        {
            var twd = ExternalFilesHelper.GetTourWriterDataFolder();
            var dlg = new SaveFileDialog {FileName = "layout.xml", Filter = ".xml files (*.xml)|*.xml|All files (*.*)|*.*", RestoreDirectory = true};
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.FileName.Contains(twd) || 
                    App.AskYesNo("Are you sure you want to save this file outside of your TourWriterData folder (other users may not be able to access it)?"))
                    grid.DisplayLayout.SaveAsXml(dlg.FileName);
            }
        }
    }
}
