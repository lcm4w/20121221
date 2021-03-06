﻿using System;
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
                SaveGroupPrices(quote);
                _tempQuoteTable = quote;
                grid.DataSource = quote;               
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        internal void SaveGroupPrices(QuoteTable quote)
        {
            // suppliments (optiontypes)
            foreach (var option in Cache.ToolSet.OptionType)
            {
                var sum = string.Format("SUM([{0}])", option.OptionTypeName);
                var flt = string.Format("[{0}] IS NOT NULL", option.OptionTypeName);
                var obj = quote.Compute(sum, flt);
                var val = obj.ToString() != string.Empty ? (decimal?)obj : null;

                var col = ItinerarySet.GroupPrice.FirstOrDefault(x => !x.IsOptionTypeIDNull() && x.OptionTypeID == option.OptionTypeID);
                if (col == null)
                {
                    // add the row
                    col = ItinerarySet.GroupPrice.NewGroupPriceRow();
                    col.ItineraryID = ItinerarySet.Itinerary[0].ItineraryID;
                    col.GroupPriceName = "";
                    ItinerarySet.GroupPrice.AddGroupPriceRow(col);
                }
                // add/update values
                col.OptionTypeID = option.OptionTypeID;
                col.GroupPriceName = option.OptionTypeName;
                if (val.HasValue) col.Price = (decimal)val; 
                else col.SetPriceNull();
            }

            // pax breaks (itineraypax)
            foreach (var pax in ItinerarySet.ItineraryPax)
            {
                var sum = string.Format("SUM([{0}])", pax.ItineraryPaxName);
                var flt = string.Format("[{0}] IS NOT NULL", pax.ItineraryPaxName);
                var obj = quote.Compute(sum, flt);
                var val = obj.ToString() != string.Empty ? (decimal?)obj : null;

                var col = ItinerarySet.GroupPrice.FirstOrDefault(x => !x.IsItineraryPaxIDNull() && x.ItineraryPaxID == pax.ItineraryPaxID);
                if (col == null)
                {
                    // add the row
                    col = ItinerarySet.GroupPrice.NewGroupPriceRow();
                    col.ItineraryID = ItinerarySet.Itinerary[0].ItineraryID;
                    col.GroupPriceName = "";
                    ItinerarySet.GroupPrice.AddGroupPriceRow(col);
                }
                // add/update values
                col.ItineraryPaxID = pax.ItineraryPaxID;
                col.GroupPriceName = pax.ItineraryPaxName;
                if (val.HasValue) col.Price = (decimal)val;
                else col.SetPriceNull();
            }
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.CellActivation = Activation.NoEdit;

                c.Header.ToolTipText = GetColumnTooltipText(c.Key);

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
            e.Layout.AutoFitStyle = AutoFitStyle.None;

            GridHelper.SetDefaultSummaryAppearance(e);

            SetGridSummaries(e, this.ItinerarySet.ItineraryPax);
        }

        private string GetColumnTooltipText(string key)
        {
            var pax = ItinerarySet.ItineraryPax.Where(x => x.RowState != DataRowState.Deleted && x.ItineraryPaxName == key).FirstOrDefault();
            if (pax == null) return "";

            return string.Format("Members/rooms: {0} / {1} \r\nStaff/rooms: {2} / {3}",
                                 !pax.IsMemberCountNull() ? pax.MemberCount.ToString() : "",
                                 !pax.IsMemberRoomsNull() ? pax.MemberRooms.ToString() : "",
                                 !pax.IsStaffCountNull() ? pax.StaffCount.ToString() : "",
                                 !pax.IsStaffRoomsNull()? pax.StaffRooms.ToString() : "");

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
                    if (paxRows.Length == 0)
                    {
                        paxRows = ItinerarySet.GroupPrice.Select(string.Format("GroupPriceName = '{0}'", c.Header.Caption));
                    }
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

        private QuoteTable _tempQuoteTable;
        internal QuoteTable GetRefreshedQuoteTable()
        {
            DataBind();
            return _tempQuoteTable;
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

            //get the headers that will be passed to GroupQuoteOverride
            var headers = "";
            foreach(DataColumn col in dtQuote.Columns)
            {
                headers += col.ColumnName + ";";
            }
            // pass current column subtotals
            foreach (DataRow col in ItinerarySet.GroupPrice.Rows) 
            {
                var columnName = col["GroupPriceName"].ToString(); 
                var key = string.Concat("Sum", columnName);
                var subTotal = (from DataRow row in dtQuote.Rows
                                where !row.IsNull(columnName)
                                select Math.Round(Convert.ToDecimal(row[columnName]), 2)).Sum();

                if (_summaryItems.ContainsKey(key)) continue;
                var summaryItem = new QuoteSummaryItem { Key = key, Subtotal = subTotal };
                _summaryItems.Add(key, summaryItem);
            }

            // open override form
            using (var frm = new GroupQuoteOverride2(ItinerarySet, _summaryItems, headers))
            {
                if (frm.ShowDialog() != DialogResult.OK) return;
                foreach (SummarySettings s in grid.DisplayLayout.Bands[0].Summaries)
                    s.Refresh(); // ICustomSummaryCalculator requires each col to be refreshed
            }
            grid.DisplayLayout.RefreshSummaries();
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
                sb.Append("Sum");
                sb.AppendLine();               
                sb.Append("Markup %");
                sb.AppendLine();
                sb.Append("Subtotal");
                sb.AppendLine();
                sb.Append("Override $");
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
                var pax = _itinerarySet.GroupPrice.FirstOrDefault(x => x.RowState != DataRowState.Deleted && x.GroupPriceName == summarySettings.SourceColumn.Key);

                return string.Format("{0:#0.00}{2:#0.00}{1}{3}{1}{4:#0.00}{1}{5}{1}{6}", 
                                            _total, 
                                            Environment.NewLine,
                                            null,
                                            pax != null && !pax.IsMarkupNull() ? pax.Markup.ToString("#0.00") + "%" : "",
                                            GetSubTotal(pax),
                                            pax != null && !pax.IsPriceOverrideNull() ? pax.PriceOverride.ToString("$#0.00") : "", 
                                            FinalPrice(pax).ToString("$#0.00"));                              
            }

            private decimal GetSubTotal(ItinerarySet.GroupPriceRow pax)
            {
                if (pax.IsMarkupNull())
                {
                    return pax.IsPriceNull() ? 0 : pax.Price;
                }
                return pax.IsPriceNull() ? 0 : ((pax.Markup/100)*pax.Price) + pax.Price;                
            }                  

            private decimal FinalPrice(ItinerarySet.GroupPriceRow pax)
            {
                if (pax == null) return _total;
                if (!pax.IsPriceOverrideNull()) return pax.PriceOverride;
                if (!pax.IsMarkupNull())
                    return _total * (1 + pax.Markup / 100);
                return _total;
            }
        }

        private void btnPaxBreaks_Click(object sender, EventArgs e)
        {
            var state = ItinerarySet.ItineraryPax.GetChanges();
            var form = new PaxBreaksForm(ItinerarySet);
            
            form.ShowDialog();
            if (state != ItinerarySet.ItineraryPax.GetChanges())
                DataBind();
        }
    }
}

