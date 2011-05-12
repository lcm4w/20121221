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
    internal partial class GroupQuoteOverride : Form
    {
        private readonly ItinerarySet _itinerarySet;
        private Dictionary<string, QuoteSummaryItem> _summaryItems;

        public GroupQuoteOverride(ItinerarySet itinerarySet, Dictionary<string, QuoteSummaryItem> summaryItems)
        {
            InitializeComponent();
            Icon = Properties.Resources.TourWriter16;
            _itinerarySet = itinerarySet;
            _summaryItems = summaryItems;
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
            dt.Columns.Add("Description", typeof(string));

            foreach (DataRow row in _itinerarySet.ItineraryPax.Rows)
            {
                dt.Columns.Add(row["ItineraryPaxName"].ToString(), typeof(string));
            }

            var newRow = dt.NewRow();
            newRow["Description"] = "Subtotal";

            foreach (DataRow row in _itinerarySet.ItineraryPax.Rows)
            {
                columnName = row["ItineraryPaxName"].ToString();
                key = string.Format("Sum{0}", columnName);
                item = _summaryItems[key];

                newRow[columnName] = item.Subtotal.ToString("#0.00");
            }

            dt.Rows.Add(newRow);

            newRow = dt.NewRow();
            newRow["Description"] = "Margin";

            foreach (DataRow row in _itinerarySet.ItineraryPax.Rows)
            {
                columnName = row["ItineraryPaxName"].ToString();
                key = string.Format("Sum{0}", columnName);
                item = _summaryItems[key];

                newRow[columnName] = item.Margin.ToString("#0.00");
            }

            dt.Rows.Add(newRow);

            newRow = dt.NewRow();
            newRow["Description"] = "Markup";

            foreach (DataRow row in _itinerarySet.ItineraryPax.Rows)
            {
                columnName = row["ItineraryPaxName"].ToString();
                key = string.Format("Sum{0}", columnName);
                item = _summaryItems[key];

                newRow[columnName] = item.Markup.ToString("#0.00");
            }

            dt.Rows.Add(newRow);

            newRow = dt.NewRow();
            newRow["Description"] = "Override";

            foreach (DataRow row in _itinerarySet.ItineraryPax.Rows)
            {
                columnName = row["ItineraryPaxName"].ToString();
                key = string.Format("Sum{0}", columnName);
                item = _summaryItems[key];

                newRow[columnName] = item.Override.ToString("#0.00");
            }

            dt.Rows.Add(newRow);

            return dt;
        }

        private void SetGridCellStyles()
        {
            // Margin
            var row = grid.Rows[1];

            for (int i = 1; i < row.Cells.Count; i++)
            {
                var cell = row.Cells[i];
                //cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Double;
            }

            // Markup
            row = grid.Rows[2];

            for (int i = 1; i < row.Cells.Count; i++)
            {
                var cell = row.Cells[i];
                cell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Double;
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
                var paxRows = _itinerarySet.ItineraryPax.Select(string.Format("ItineraryPaxName = '{0}'", c.Header.Caption));

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
            grid.DisplayLayout.RefreshSummaries();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // Start at pax value
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                var row = grid.Rows[i];

                // Start at margin
                for (int j = 1; j < row.Cells.Count; j++)
                {
                    var cell = row.Cells[j];
                    var key = string.Concat("Sum", cell.Column.Key);
                    var item = _summaryItems[key];

                    if (item != null)
                    {
                        switch (i)
                        {
                            case 0: // Subtotal
                                item.Subtotal = cell.Value == DBNull.Value ? 0 : Convert.ToDecimal(cell.Value);
                                break;
                            case 1: // Margin
                                item.Margin = cell.Value == DBNull.Value ? 0 : Convert.ToDecimal(cell.Value);
                                break;
                            case 2: // Markup
                                item.Markup = cell.Value == DBNull.Value ? 0 : Convert.ToDecimal(cell.Value);
                                break;
                            case 3: // Override
                                item.Override = cell.Value == DBNull.Value ? 0 : Convert.ToDecimal(cell.Value);
                                break;
                        }
                    }
                }
            }

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
                var value = Convert.ToDecimal(row.GetCellValue(summarySettings.SourceColumn));

                // Override
                if (_rowIndex == 3 && value > 0)
                {
                    _total = value;
                }
                else
                {
                    // Markup
                    if (_rowIndex == 2)
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
