using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;
using System.Text;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class BookingsQuote : UserControl
    {
        Dictionary<string, QuoteSummaryItem> _summaryItems = new Dictionary<string, QuoteSummaryItem>();

        internal ItinerarySet ItinerarySet { get; set; }

        public BookingsQuote()
        {
            InitializeComponent();
        }

        private void BookingsQuote_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible || DesignMode) return;

            DataBind();
        }

        private void DataBind()
        {
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

            SetGridSummaries(e, this.ItinerarySet.ItineraryPax);
        }

        internal void SetGridSummaries(InitializeLayoutEventArgs e, DataTable dtItineraryPax)
        {
            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            band.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            SummarySettings summary = null;
            
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                string key = "Sum" + c.Header.Caption;

                if (key == "SumCategory" && !band.Summaries.Exists("SumCategory"))
                {
                    summary = band.Summaries.Add(
                        string.Format("TW_SUM_{0}", c.Header.Caption),
                        SummaryType.Custom,
                        new QuoteSummaryDescription(),
                        band.Columns[c.Header.Caption],
                        SummaryPosition.UseSummaryPositionColumn,
                        null
                        );

                    summary.Key = key;
                    summary.DisplayFormat = "{0}";
                    summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                    summary.Lines = 5;
                    e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
                    
                    //summary.SummaryDisplayArea = SummaryDisplayAreas.InGroupByRows;
                    summary.SummaryDisplayArea = // fixed at bottom of grid
                        SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly;
                }
                else if (c.DataType == typeof(Decimal) && !band.Summaries.Exists(key))
                {
                    var paxRows = dtItineraryPax.Select(string.Format("ItineraryPaxName = '{0}'", c.Header.Caption));
                    
                    if (paxRows.Length > 0)
                    {
                        summary = band.Summaries.Add(
                            string.Format("TW_SUM_{0}", c.Header.Caption),
                            SummaryType.Custom,
                            new QuoteSummary(new QuoteSummaryItem()),
                            band.Columns[c.Header.Caption],
                            SummaryPosition.UseSummaryPositionColumn,
                            null
                            );
                    }
                    else
                    {
                        summary = band.Summaries.Add(SummaryType.Sum, band.Columns[c.Header.Caption]);
                    }

                    summary.Key = key;
                    summary.DisplayFormat = "{0:#0.00}";
                    summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                    summary.Lines = 5;
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

        private void btnEditOverrides_Click(object sender, EventArgs e)
        {
            QuoteTable dtQuote = (QuoteTable)grid.DataSource;

            foreach (DataRow col in this.ItinerarySet.ItineraryPax.Rows)
            {
                var columnName = col["ItineraryPaxName"].ToString();
                var key = string.Concat("Sum", columnName);
                var subTotal = 0m;

                foreach (DataRow row in dtQuote.Rows)
                {
                    if (!row.IsNull(columnName))
                    {
                        subTotal += Math.Round(Convert.ToDecimal(row[columnName]), 2);
                    }
                }

                if (!_summaryItems.ContainsKey(key))
                {
                    var summaryItem = new QuoteSummaryItem();
                    summaryItem.Key = key;
                    summaryItem.Subtotal = subTotal;
                    _summaryItems.Add(key, summaryItem);
                }
            }

            using (var frm = new GroupQuoteOverride(this.ItinerarySet, _summaryItems))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    _summaryItems = frm.SummaryItems;

                    foreach (var key in frm.SummaryItems.Keys)
                    {
                        var si = frm.SummaryItems[key];

                        grid.DisplayLayout.Bands[0].Summaries[key].CustomSummaryCalculator = new QuoteSummary(si);
                    }

                    grid.DisplayLayout.RefreshSummaries();
                }
            }
        }

        class QuoteSummaryDescription : ICustomSummaryCalculator
        {
            public void AggregateCustomSummary(SummarySettings summarySettings, UltraGridRow row)
            {

            }

            public void BeginCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {

            }

            public object EndCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                var sb = new StringBuilder();
                sb.Append("Subtotal");
                sb.AppendLine();
                sb.Append("Margin");
                sb.AppendLine();
                sb.Append("Markup");
                sb.AppendLine();
                sb.Append("Override");
                sb.AppendLine();
                sb.Append("Total");

                return sb.ToString();
            }
        }

        class QuoteSummary : ICustomSummaryCalculator
        {
            QuoteSummaryItem _item;
            decimal _total;

            public QuoteSummary(QuoteSummaryItem item)
            {
                _item = item;
            }

            public void AggregateCustomSummary(SummarySettings summarySettings, UltraGridRow row)
            {
                var value = row.GetCellValue(summarySettings.SourceColumn);

                if (value != DBNull.Value)
                {
                    _total += Convert.ToDecimal(value);
                }
            }

            public void BeginCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                _total = 0;
            }

            public object EndCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                if (_item == null)
                {
                    return _total.ToString("#0.00");
                }

                return string.Format("{0:#0.00}{1}{2:#0.00}{1}{3:#0.00}{1}{4:#0.00}{1}{5:#0.00}",
                    _total,
                    Environment.NewLine,
                    _item.Margin,
                    _item.Markup,
                    _item.Override,
                    _item.Total
                    );
            }
        }
    }
}

