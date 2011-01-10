using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;

namespace TourWriter.Modules.DataExtract.UserControls
{
    public partial class PaymentsExport : UserControl
    {
        public PaymentsExport()
        {
            InitializeComponent();

            txtFrom.DateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, 1);
            txtTo.DateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

            grid.ExportFileName = "PaymentsData.xls";
            grid.InitializeLayoutEvent += grid_InitializeLayoutEvent;
            grid.UltraGrid.DoubleClickRow += grid_DoubleClickRow;
        }

        private void LoadData()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                string sql =
                    @"
select * from PurchaseItemPaymentsDetail
where paymentduedate >= @dateFrom
and paymentduedate <= @dateTo
order by paymentduedate";

                SqlParameter p1 = new SqlParameter("@dateFrom", txtFrom.Value);
                SqlParameter p2 = new SqlParameter("@dateTo", txtTo.Value);

                grid.DataSource = Info.Services.DatabaseHelper.ExecuteDataset(
                    sql, new SqlParameter[] {p1, p2});
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private static void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            //todo: make this work
            //if (e.Row.GetType() == typeof(UltraGridEmptyRow))
            //    return;

            //NavigationTreeItemInfo info = new NavigationTreeItemInfo(
            //    (int)e.Row.Cells["ItineraryID"].Value,
            //    (string)e.Row.Cells["ItineraryName"].Value,
            //    NavigationTreeItemInfo.ItemTypes.Itinerary,
            //    (int)e.Row.Cells["ItineraryParentFolderID"].Value,
            //    true);

            //UltraTreeNode node = App.MainForm.BuildMenuNode(info);
            //App.MainForm.Load_ItineraryForm(node);
        }

        private static void grid_InitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.CellActivation = Activation.NoEdit;

                if (c.Key == "PaymentAmount")
                {
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "BookingDate" || c.Key == "PaymentDate")
                {
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.Format = App.GetLocalShortDateFormat();
                }
                if (c.Key == "ItineraryParentFolderID")
                {
                    c.Hidden = true;
                }
            }
            grid_InitializeSummaries(e);
        }

        private static void grid_InitializeSummaries(InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            if (!band.Summaries.Exists("GroupPaymentAmounts"))
            {
                band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
                band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter;

                SummarySettings summary;
                summary = band.Summaries.Add(SummaryType.Sum, band.Columns["PaymentAmount"]);
                summary.Key = "GroupPaymentAmounts";
                summary.DisplayFormat = "{0:#0.00}";
                summary.Appearance.TextHAlign = HAlign.Right;
                e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
