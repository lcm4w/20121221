using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Info.Services;

namespace TourWriter.Modules.Plugins.NzGst
{
    public partial class GstAdjust : ModuleBase
    {
        const string Sql = "select top (10) * from supplier";

        public GstAdjust()
        {
            InitializeComponent();

            //txtFrom.DateTime = new DateTime(DateTime.Now.Year, 1, 1);
            //txtTo.DateTime = new DateTime(DateTime.Now.Year, 12, 31);
            
            grid.ExportFileName = "nz-gst.xls";
            grid.InitializeLayoutEvent += grid_InitializeLayout;
            grid.UltraGrid.DoubleClickRow += grid_DoubleClickRow;

            comboBox1.Items.AddRange(new[]
                                         {
                                             "-- choose --",
                                             "Supplier rates that span 1 Oct", 
                                             "Supplier rates after 1 Oct",
                                             "Itineary bookings that span 1 Oct", 
                                             "Itineary bookings after 1 Oct"
                                         });
            comboBox1.SelectedIndex = 0;
        }

        #region Override methods
        protected override bool IsDataDirty()
        {
            return false;
        }

        protected override string GetDisplayName()
        {
            return "Nz GST adjust";
        }
        #endregion

        private static string GetSql(int index)
        {
            var supplierSql =
                @"
select
	s.SupplierId, 
	SupplierName, 
	v.ServiceName, 
	ValidFrom, 
	ValidTo, 
	OptionName,
	PricingOption,
	case when PricingOption = 'nm' or PricingOption = 'ng'	then Net else null end as Net,
	case when PricingOption = 'nm'							then (Gross - Net)/Net*100 else null end as Markup,
	case when PricingOption = 'ng' or PricingOption = 'gc'	then Gross else null end as Gross,
	case when PricingOption = 'gc'							then (Gross - Net)/Gross*100 else null end as Commission			
from [option] o
inner join [rate] r on  o.RateID = r.RateID
inner join [Service] v on r.ServiceID = v.ServiceID
inner join [Supplier] s on v.SupplierID = s.SupplierID
where {0} 
and Gross is not null and Gross > 0
order by SupplierName, ServiceName, ValidFrom, OptionName";


            switch (index)
            {
                case 1:
                    return string.Format(supplierSql, "ValidFrom <= '2010.10.1 00:00:00' and ValidTo >= '2010.10.1 00:00:00'");
                case 2:
                    return string.Format(supplierSql, "ValidFrom >= '2010.10.1 00:00:00'");
                case 3:
                    return "select top (2) * from itinerary";
                case 4:
                    return "select top (2) * from itinerary";
            }
            return "";
        }

        private void LoadData(int index)
        {
            if (index < 1)
            {
                grid.DataSource = null;
                return;
            }
            var sql = GetSql(index);
            LoadData(sql);   
        }

        private void LoadData(string sql)
        {
            grid.DataSource = null;

            var ds = new DataSet();
            ds = DataSetHelper.FillDataset(ds, sql);
            var dv = ds.Tables[0].DefaultView;

            grid.DataSource = dv;

            //if (grid.DataSource == null)
            //{
            //    grid.DataSource = dv;
            //}
            //else
            //{
            //    var dt = grid.DataSource as DataTable;
            //    if (dt != null)
            //    {
            //        dt.Clear();
            //        dt.Merge(dv.Table, false);
            //    }
            //}
        }
        
        private static void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow) ||
                e.Row.GetType() == typeof(UltraGridGroupByRow))
                return;

            if (e.Row.Cells.Exists("SupplierID"))
            {
                var info = new NavigationTreeItemInfo((int) e.Row.Cells["SupplierID"].Value,
                                                      (string) e.Row.Cells["SupplierName"].Value,
                                                      NavigationTreeItemInfo.ItemTypes.Supplier,
                                                      (int) e.Row.Cells["ParentFolderID"].Value, true);

                var node = App.MainForm.BuildMenuNode(info);
                App.MainForm.Load_SupplierForm(node);
            }
            else if (e.Row.Cells.Exists("ItineraryID"))
            {
                var info = new NavigationTreeItemInfo((int)e.Row.Cells["ItineraryID"].Value,
                                                      (string)e.Row.Cells["ItineraryName"].Value,
                                                      NavigationTreeItemInfo.ItemTypes.Itinerary,
                                                      (int)e.Row.Cells["ParentFolderID"].Value, true);

                var node = App.MainForm.BuildMenuNode(info);
                App.MainForm.Load_ItineraryForm(node);
            }
        }

        private static void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            //foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            //{
            //    c.CellActivation = Activation.NoEdit;

            //    if (c.Key == "ItineraryName")
            //    {
            //        c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
            //    }
            //    else if (c.Key == "ArriveDate")
            //    {
            //        c.CellAppearance.TextHAlign = HAlign.Right;
            //        c.Format = App.GetLocalShortDateFormat() + " " + App.GetLocalShortTime24HrFormat();
            //    }
            //    else if (c.Key == "CreateDate")
            //    {
            //        c.CellAppearance.TextHAlign = HAlign.Right;
            //        c.Format = App.GetLocalShortDateFormat();
            //    }
            //    else if (c.Key == "Net" || c.Key == "Gross" ||
            //             c.Key == "Deposits" || c.Key == "Balance" || c.Key == "Margin")
            //    {
            //        c.Format = "c";
            //        c.CellAppearance.TextHAlign = HAlign.Right;
            //    }
            //    else if (c.Key == "Yield")
            //    {
            //        c.Format = "##0.00\\%";
            //        c.CellAppearance.TextHAlign = HAlign.Right;
            //    }
            //    else if (c.Key == "ItineraryID" || c.Key == "ParentFolderID")
            //    {
            //        c.Hidden = true;
            //    }
            //}

            //grid_InitializeSummaries(e);
        }

        private static void grid_InitializeSummaries(InitializeLayoutEventArgs e)
        {
            //UltraGridBand band = e.Layout.Bands[0];
            //band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            //band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter;
            //SummarySettings summary;
 
            //summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Net"]);
            //summary.Key = "GroupNet";
            //summary.DisplayFormat = "{0:c}";
            //summary.Appearance.TextHAlign = HAlign.Right;
            //e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            //summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Gross"]);
            //summary.Key = "GroupGross";
            //summary.DisplayFormat = "{0:c}";
            //summary.Appearance.TextHAlign = HAlign.Right;
            //e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            //summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Deposits"]);
            //summary.Key = "GroupDeposits";
            //summary.DisplayFormat = "{0:c}";
            //summary.Appearance.TextHAlign = HAlign.Right;
            //e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            //summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Balance"]);
            //summary.Key = "GroupBalance";
            //summary.DisplayFormat = "{0:c}";
            //summary.Appearance.TextHAlign = HAlign.Right;
            //e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            //summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Margin"]);
            //summary.Key = "GroupMargin";
            //summary.DisplayFormat = "{0:c}";
            //summary.Appearance.TextHAlign = HAlign.Right;
            //e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;

            //summary = band.Summaries.Add(SummaryType.Average, band.Columns["Yield"]);
            //summary.Key = "GroupYield";
            //summary.DisplayFormat = "{0:##0.00\\%}";
            //summary.Appearance.TextHAlign = HAlign.Right;
            //e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            LoadData(comboBox1.SelectedIndex);

            Cursor = Cursors.Default;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            LoadData(comboBox1.SelectedIndex);

            Cursor = Cursors.Default;
        }
    }
}
