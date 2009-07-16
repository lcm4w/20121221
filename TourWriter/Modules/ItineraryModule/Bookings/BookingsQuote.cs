using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class BookingsQuote : UserControl
    {
        internal ItinerarySet ItinerarySet { get; set; }

        public BookingsQuote()
        {
            InitializeComponent();
        }

        private void BookingsQuote_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible || DesignMode) return;

            var quote = new QuoteTable(ItinerarySet, Cache.ToolSet.OptionType);
            quote.TablePopulate(ItinerarySet.PurchaseItem);
            grid.DataSource = quote;

        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.CellActivation = Activation.NoEdit;

                if (c.DataType == typeof(Decimal))
                {
                    c.Format = "#0.00";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
            }
            GridHelper.SetDefaultGridAppearance(e);

            // override default appearances
            e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
            e.Layout.Override.ActiveRowAppearance.BackColor = e.Layout.Override.RowAppearance.BackColor;
            e.Layout.Override.ActiveRowAppearance.ForeColor = e.Layout.Override.RowAppearance.ForeColor;
            e.Layout.Override.SelectedRowAppearance.BackColor = e.Layout.Override.RowAppearance.BackColor;
            e.Layout.Override.SelectedRowAppearance.ForeColor = e.Layout.Override.RowAppearance.ForeColor;
            e.Layout.Override.RowSelectors = DefaultableBoolean.True;
            e.Layout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;
            e.Layout.GroupByBox.Hidden = false;
            e.Layout.ColumnChooserEnabled = DefaultableBoolean.True;

            GridHelper.SetDefaultSummaryAppearance(e);

            SetGridSummaries(e);
        }

        internal static void SetGridSummaries(InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            band.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            SummarySettings summary;

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                string key = "Sum" + c.Header.Caption;
                if (c.DataType == typeof(Decimal) && !band.Summaries.Exists(key))
                {
                    summary = band.Summaries.Add(SummaryType.Sum, band.Columns[c.Header.Caption]);
                    summary.Key = key;
                    summary.DisplayFormat = "{0:c}";
                    summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                    e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

                    //summary.SummaryDisplayArea = SummaryDisplayAreas.InGroupByRows;
                    summary.SummaryDisplayArea = // fixed at bottom of grid
                        SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly;
                }
            }
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (btnExpand.Text == "Expand")
            {
                foreach (UltraGridRow row in grid.Rows)
                    row.ExpandAll();
                btnExpand.Image = Properties.Resources.Collapse;
                btnExpand.Text = "Collapse";
            }
            else
            {
                foreach (UltraGridRow row in grid.Rows)
                    row.CollapseAll();
                btnExpand.Image = Properties.Resources.Expand;
                btnExpand.Text = "Expand";
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            grid.DisplayLayout.Override.AllowRowFiltering = DefaultableBoolean.True;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var doc = new UltraGridPrintDocument();
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

            var preview = new UltraPrintPreviewDialog {Document = doc};
            preview.ShowDialog();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (grid.Rows.Count == 0)
                return;

            string templateFileName = "";//App.SelectExternalFile(false, "Select Excel template file", "Excel (*.xls)|*.xls", 0);
            string saveFileName = App.PromptSaveFile("Bookings Quote.xls", "Excel", ".xls", false);

            if (String.IsNullOrEmpty(saveFileName))
                return;

            try
            {
                Cursor = Cursors.WaitCursor;

                if (String.IsNullOrEmpty(templateFileName))
                {
                    // if no template specified, use standard ultra grid exporter
                    var excelExporter = new UltraGridExcelExporter();
                    excelExporter.Export(grid, saveFileName);
                }
                else
                {
                    var exporter = new GridExcelExporter(grid);
                    exporter.Export(templateFileName, saveFileName);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}
