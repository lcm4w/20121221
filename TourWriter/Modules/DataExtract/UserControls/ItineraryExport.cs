using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;

namespace TourWriter.Modules.DataExtract.UserControls
{
    public partial class ItineraryExport : UserControl
    {
        public ItineraryExport()
        {
            InitializeComponent();

            txtFrom.DateTime = new DateTime(DateTime.Now.Year, 1, 1);
            txtTo.DateTime = new DateTime(DateTime.Now.Year, 12, 31);

            grid.ExportFileName = "ItineraryData.xls";
            grid.InitializeLayoutEvent += grid_InitializeLayout;
            grid.UltraGrid.DoubleClickRow += grid_DoubleClickRow;
        }

        private void LoadData()
        {
            BusinessLogic.Reports reports = new BusinessLogic.Reports();
            SqlDataReader reader = reports.ItineraryExport(txtFrom.DateTime, txtTo.DateTime);

            DataTable table = new DataTable();
            table.Load(reader);

            if (grid.DataSource == null)
            {
                grid.DataSource = table.DefaultView.Table;
            }
            else
            {
                // merge to retain grid layout
                DataTable dt = grid.DataSource as DataTable;
                if (dt != null)
                {
                    dt.Clear();
                    dt.Merge(table, false);
                }
            }
        }

        private static void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow) ||
                e.Row.GetType() == typeof(UltraGridGroupByRow))
                return;

            NavigationTreeItemInfo info = new NavigationTreeItemInfo(
                (int)e.Row.Cells["ItineraryID"].Value,
                (string)e.Row.Cells["ItineraryName"].Value,
                NavigationTreeItemInfo.ItemTypes.Itinerary,
                (int)e.Row.Cells["ParentFolderID"].Value,
                true);

            UltraTreeNode node = App.MainForm.BuildMenuNode(info);
            App.MainForm.Load_ItineraryForm(node);
        }

        private static void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.CellActivation = Activation.NoEdit;

                if (c.Key == "ItineraryName")
                {
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                }
                else if (c.Key == "ArriveDate")
                {
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.Format = App.GetLocalShortDateFormat() + " " + App.GetLocalShortTime24HrFormat();
                }
                else if (c.Key == "CreateDate")
                {
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.Format = App.GetLocalShortDateFormat();
                }
                else if (c.Key == "Net" || c.Key == "Gross" ||
                         c.Key == "Deposits" || c.Key == "Balance" || c.Key == "Margin")
                {
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "Yield")
                {
                    c.Format = "##0.00\\%";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "ItineraryID" || c.Key == "ParentFolderID")
                {
                    c.Hidden = true;
                }
            }

            grid_InitializeSummaries(e);
        }

        private static void grid_InitializeSummaries(InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter;
            SummarySettings summary;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Net"]);
            summary.Key = "GroupNet";
            summary.DisplayFormat = "{0:c}";
            summary.Appearance.TextHAlign = HAlign.Right;
            e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Gross"]);
            summary.Key = "GroupGross";
            summary.DisplayFormat = "{0:c}";
            summary.Appearance.TextHAlign = HAlign.Right;
            e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Deposits"]);
            summary.Key = "GroupDeposits";
            summary.DisplayFormat = "{0:c}";
            summary.Appearance.TextHAlign = HAlign.Right;
            e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Balance"]);
            summary.Key = "GroupBalance";
            summary.DisplayFormat = "{0:c}";
            summary.Appearance.TextHAlign = HAlign.Right;
            e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Margin"]);
            summary.Key = "GroupMargin";
            summary.DisplayFormat = "{0:c}";
            summary.Appearance.TextHAlign = HAlign.Right;
            e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            summary = band.Summaries.Add(SummaryType.Average, band.Columns["Yield"]);
            summary.Key = "GroupYield";
            summary.DisplayFormat = "{0:##0.00\\%}";
            summary.Appearance.TextHAlign = HAlign.Right;
            e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            LoadData();
            Cursor = Cursors.Default;
        }
    }
}
