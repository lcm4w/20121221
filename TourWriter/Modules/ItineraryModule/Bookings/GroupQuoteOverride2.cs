using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TourWriter.Info;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Services;
using Infragistics.Win;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    internal partial class GroupQuoteOverride2 : Form
    {
        private readonly ItinerarySet _itinerarySet;
        private readonly Dictionary<string, QuoteSummaryItem> _summaryItems;
        private string[] GridQuoteHeaders { get; set; }

        public GroupQuoteOverride2(ItinerarySet itinerarySet, Dictionary<string, QuoteSummaryItem> summaryItems, string headers)
        {
            InitializeComponent();
            Icon = Properties.Resources.TourWriter16;
            _itinerarySet = itinerarySet;
            _summaryItems = summaryItems;
            GridQuoteHeaders = headers.Split(';');
            DataBind();
        }

        public Dictionary<string, QuoteSummaryItem> SummaryItems
        {
            get
            {
                return _summaryItems;
            }
        }

        private void DataBind()
        {
            var dt = SetupDataSource();
            grid.AfterExitEditMode+=grid_AfterExitEditMode;
            grid.DataSource = dt;
            grid.Enabled = true;

            SetGridCellStyles();
        }

        private DataTable SetupDataSource()
        {
            var columnName = string.Empty;
            var key = string.Empty;
            var item = default(QuoteSummaryItem);
            var dt = new DataTable();

            // add table columns
            dt.Columns.Add("Description", typeof(string));           
            for(int i=0;i < GridQuoteHeaders.Length;i++)
            {
                var headerGroupPrice = _itinerarySet.GroupPrice.Where(x => x.GroupPriceName == GridQuoteHeaders[i]).Select(x => x.GroupPriceName).SingleOrDefault();
                if (string.IsNullOrEmpty(headerGroupPrice)) continue;
                dt.Columns.Add(headerGroupPrice, typeof(decimal));
            }          
            // add Sum
            var newRow = dt.NewRow();
            newRow["Description"] = "Sum";//"Subtotal";
            foreach (DataRow row in _itinerarySet.GroupPrice.Rows) 
            {               
                columnName = row["GroupPriceName"].ToString();
                key = string.Format("Sum{0}", columnName);
                item = _summaryItems[key];
                newRow[columnName] = item.Subtotal;
            }          
            dt.Rows.Add(newRow);                      
            
            // add Markup row
            newRow = dt.NewRow();
            newRow["Description"] = "Markup %";
            foreach (var row in _itinerarySet.GroupPrice.AsEnumerable())
            {
                columnName = row["GroupPriceName"].ToString();
                if (!row.IsMarkupNull()) 
                    newRow[columnName] = row.Markup;
            }
            dt.Rows.Add(newRow);

            //add subtotal row
            newRow = dt.NewRow();
            newRow["Description"] = "Subtotal";
            foreach (DataRow row in _itinerarySet.GroupPrice.Rows)
            {                
                columnName = row["GroupPriceName"].ToString();
                newRow[columnName] = 0;                   
            }
            dt.Rows.Add(newRow);

            // add Override row
            newRow = dt.NewRow();
            newRow["Description"] = "Override $";
            foreach (var row in _itinerarySet.GroupPrice.AsEnumerable())//_itinerarySet.ItineraryPax.AsEnumerable())
            {
                columnName = row.GroupPriceName; //row.ItineraryPaxName; 
                if (!row.IsPriceOverrideNull()) //if (!row.IsGrossOverrideNull()) 
                    newRow[columnName] = row.PriceOverride;
            }
            dt.Rows.Add(newRow);

            return dt;
        }

        private void SetGridCellStyles()
        {
            //// Margin
            //var row = grid.Rows[1];

            //for (int i = 1; i < row.Cells.Count; i++)
            //{
            //    var cell = row.Cells[i];
            //    //cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            //    cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Double;
            //    cell.Activation = Activation.NoEdit;
            //}

            // Markup
            var row = grid.Rows[1];

            for (int i = 1; i < row.Cells.Count; i++)
            {
                var cell = row.Cells[i];
                cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Double;
                //cell.Appearance.ForeColor = Color.Blue;
            }

            // Subtotal
            row = grid.Rows[2];

            for (int i = 1; i < row.Cells.Count; i++)
            {
                var cell = row.Cells[i];
                cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Double;
                cell.Activation = Activation.NoEdit;
                //cell.Appearance.ForeColor = Color.Blue;
            }

            // Override
            row = grid.Rows[3];

            for (int i = 1; i < row.Cells.Count; i++)
            {
                var cell = row.Cells[i];
                cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Double;
                //cell.Appearance.ForeColor = Color.Blue;
            }
        }

        internal void SetGridSummaries(InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            band.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            SummarySettings summary = null;

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                string key = "Sum" + c.Header.Caption;               

                var paxRows = _itinerarySet.GroupPrice.Select(string.Format("GroupPriceName = '{0}'", c.Header.Caption));//_itinerarySet.ItineraryPax.Select(string.Format("ItineraryPaxName = '{0}'", c.Header.Caption));

                if (paxRows.Length > 0 && !band.Summaries.Exists(key))
                {
                    summary = band.Summaries.Add(
                            string.Format("TW_SUM_{0}", c.Header.Caption),
                            SummaryType.Custom,
                            new QuoteSummary(),
                            band.Columns[c.Header.Caption],
                            SummaryPosition.UseSummaryPositionColumn,
                            null
                            );

                    summary.DisplayFormat = "{0:#0.00}";                   
                }
                else
                {
                    summary = band.Summaries.Add(
                        string.Format("TW_SUM_{0}", c.Header.Caption),
                        SummaryType.Custom,
                        new QuoteSummaryDescription(),
                        band.Columns[c.Key],
                        SummaryPosition.UseSummaryPositionColumn,
                        null
                        );

                    summary.DisplayFormat = "{0}";                  
                }

                summary.Key = key;
                summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                summary.Lines = 1;
                e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

                summary.SummaryDisplayArea = // fixed at bottom of grid
                    SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly;
            }
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            GridHelper.SetDefaultGridAppearance(e);

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (string.Compare(c.Key, "Description", true) == 0)
                {
                    c.Width = 80;
                    c.Header.Caption = string.Empty;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.FormattedText;
                    c.CellDisplayStyle = CellDisplayStyle.FormattedText;
                }
                else
                {
                    c.Width = 50;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    c.CellButtonAppearance.BackColor = Color.Gray;
                    c.CellClickAction = CellClickAction.Edit;
                    c.MaskInput = "nnnnnn.nn";
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.FormattedText;
                    c.CellDisplayStyle = CellDisplayStyle.FormattedText;
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
            }

            GridHelper.SetDefaultSummaryAppearance(e);

            SetGridSummaries(e);
        }

        private void grid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            //if (e.Cell.Column.Key == "")
            //CalcSubtotal(e.Cell);
            grid.DisplayLayout.RefreshSummaries();
        }

        private void grid_AfterExitEditMode(object sender, EventArgs e)
        {
            CalcSubtotal(grid.ActiveCell);
        }

        private void CalcSubtotal(UltraGridCell currentCell)
        {
            if (string.IsNullOrEmpty(currentCell.Value.ToString())) return;
            //markup
            if (currentCell.Row.Index == 1)
            {
                if (grid.Rows[0].Cells[currentCell.Column.Index].Value == null) return;
                var sum = (decimal) grid.Rows[0].Cells[currentCell.Column.Index].Value;
                var mup = (1 + (decimal) currentCell.Value/100);

                grid.Rows[2].Cells[currentCell.Column.Index].Value = sum*mup;
            }
            //override
            if (currentCell.Row.Index == 3)
            {
                if (grid.Rows[0].Cells[currentCell.Column.Index].Value == null) return;
                var sum = (decimal)grid.Rows[0].Cells[currentCell.Column.Index].Value;
                var mup = ((decimal)currentCell.Value - sum) / (decimal)currentCell.Value;
             
                grid.Rows[1].Cells[currentCell.Column.Index].Value = mup;
                grid.Rows[2].Cells[currentCell.Column.Index].Value = sum * (mup+1);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var cols = grid.DisplayLayout.Bands[0].Columns;
            for (var i = 1; i < cols.Count; i++)
            {
                var markup = grid.Rows[1].Cells[i].Value;
                var overide = grid.Rows[3].Cells[i].Value;

                var key = cols[i].Header.Caption;
                var pax = _itinerarySet.GroupPrice.Where(x => x.GroupPriceName == key).FirstOrDefault();//_itinerarySet.ItineraryPax.Where(x => x.ItineraryPaxName == key).FirstOrDefault();
                if (pax == null) continue;
               
                // set markup
                //var markup = grid.Rows[1].Cells[i].Value;
                if (markup != DBNull.Value && overide == DBNull.Value)
                    pax.Markup = Convert.ToDecimal(markup); //pax.GrossMarkup = Convert.ToDecimal(markup);
                else pax.SetMarkupNull(); //pax.SetGrossMarkupNull();

                // set override
                //var overide = grid.Rows[3].Cells[i].Value;
                if (overide != DBNull.Value)
                    pax.PriceOverride = Convert.ToDecimal(overide); // pax.GrossOverride = Convert.ToDecimal(overide);
                else pax.SetPriceOverrideNull();//pax.SetGrossOverrideNull();
                
            }


            //// Start at pax value
            //for (int i = 0; i < grid.Rows.Count; i++)
            //{
            //    var row = grid.Rows[i];

            //    // Start at margin
            //    for (int j = 1; j < row.Cells.Count; j++)
            //    {
            //        var cell = row.Cells[j];
            //        var key = string.Concat("Sum", cell.Column.Key);
            //        var item = _summaryItems[key];

            //        if (item != null)
            //        {
            //            switch (i)
            //            {
            //                case 0: // Subtotal
            //                    item.Subtotal = cell.Value == DBNull.Value ? (decimal?)null : Convert.ToDecimal(cell.Value);
            //                    break;
            //                case 1: // Margin
            //                    item.Margin = cell.Value == DBNull.Value ? (decimal?)null : Convert.ToDecimal(cell.Value);
            //                    break;
            //                case 2: // Markup
            //                    item.Markup = cell.Value == DBNull.Value ? (decimal?)null : Convert.ToDecimal(cell.Value);
            //                    break;
            //                case 3: // Override
            //                    item.Override = cell.Value == DBNull.Value ? (decimal?)null : Convert.ToDecimal(cell.Value);
            //                    break;
            //            }
            //        }
            //    }
            //}

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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
                return "Total";
            }
        }

        class QuoteSummary : ICustomSummaryCalculator
        {
            decimal _total;
            int _rowIndex;

            public void AggregateCustomSummary(SummarySettings summarySettings, UltraGridRow row)
            {
                var obj = row.GetCellValue(summarySettings.SourceColumn);
                var value = obj != DBNull.Value  && obj.ToString() != "" ? Convert.ToDecimal(obj) : 0;

                // Override
                if (_rowIndex == 3 && value > 0)
                {
                    _total = value;
                }
                else if (_rowIndex == 2)
                {
                    //_total = _total - value;
                }
                else
                {
                    // Markup
                    if (_rowIndex == 1) //2
                    { 
                        _total = _total * (1 + value / 100);
                    }
                    else
                    {
                        _total += value;
                    }
                }

                _rowIndex++;
            }

            public void BeginCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                _total = 0;
                _rowIndex = 0;
            }

            public object EndCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                return _total;
            }
        }

      
        
    }
}
