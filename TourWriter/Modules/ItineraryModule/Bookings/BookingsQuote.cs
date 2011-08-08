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
        readonly Dictionary<string, QuoteSummaryItem> _summaryItems = new Dictionary<string, QuoteSummaryItem>();

        internal ItinerarySet ItinerarySet { get; set; }

        public BookingsQuote()
        {
            InitializeComponent();

            cmbPriceType.SelectedIndex = 0;
            cmbPriceType.SelectedIndexChanged += delegate { DataBind(); };
        }
        
        private void BookingsQuote_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible || DesignMode) return;

            DataBind();
        }

        private void DataBind()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                var priceType = cmbPriceType.SelectedItem.ToString().ToLower().Contains("gross") ? QuoteTable.PriceTypes.Gross : QuoteTable.PriceTypes.Net;
                var quote = new QuoteTable(ItinerarySet, Cache.ToolSet.OptionType, priceType);
                quote.TablePopulate(ItinerarySet.PurchaseItem);
                grid.DataSource = quote;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
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

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                string key = "Sum" + c.Header.Caption;

                SummarySettings summary = null;
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
                            new QuoteSummary(ItinerarySet),
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
            var dtQuote = (QuoteTable)grid.DataSource;

            // pass current column subtotals
            foreach (DataRow col in ItinerarySet.ItineraryPax.Rows)
            {
                var columnName = col["ItineraryPaxName"].ToString();
                var key = string.Concat("Sum", columnName);
                var subTotal = (from DataRow row in dtQuote.Rows where !row.IsNull(columnName) 
                                select Math.Round(Convert.ToDecimal(row[columnName]), 2)).Sum();

                if (_summaryItems.ContainsKey(key)) continue;
                var summaryItem = new QuoteSummaryItem {Key = key, Subtotal = subTotal};
                _summaryItems.Add(key, summaryItem);
            }

            // open override form
            using (var frm = new GroupQuoteOverride(ItinerarySet, _summaryItems))
            {
                if (frm.ShowDialog() != DialogResult.OK) return;
                foreach (SummarySettings s in grid.DisplayLayout.Bands[0].Summaries)
                    s.Refresh(); // ICustomSummaryCalculator requires each col to be refreshed
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
            decimal _total;
            private readonly ItinerarySet _itinerarySet;

            public QuoteSummary(ItinerarySet itinerarySet)
            {
                _itinerarySet = itinerarySet;
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
                var pax = _itinerarySet.ItineraryPax.
                    Where(x => x.ItineraryPaxName == summarySettings.SourceColumn.Key).FirstOrDefault();

                return string.Format("{0:#0.00}{1}{2:#0.00}{1}{3}{1}{4}{1}{5}",
                    _total,
                    Environment.NewLine,
                    null,
                    pax != null && !pax.IsGrossMarkupNull() ? pax.GrossMarkup.ToString("#0.00") + "%" : "",
                    pax != null && !pax.IsGrossOverrideNull() ? pax.GrossOverride.ToString("$#0.00") : "",
                    FinalPrice(pax).ToString("$#0.00")
                    );
            }

            private decimal FinalPrice(ItinerarySet.ItineraryPaxRow pax)
            {
                if (pax == null) return _total;
                if (!pax.IsGrossOverrideNull()) return pax.GrossOverride;
                if (!pax.IsGrossMarkupNull())
                    return _total * (1 + pax.GrossMarkup / 100);
                return _total;
            }
        }
    }
}

