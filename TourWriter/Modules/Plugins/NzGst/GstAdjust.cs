using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Info.Services;
using Resources = Infragistics.Win.UltraWinGrid.Resources;

namespace TourWriter.Modules.Plugins.NzGst
{
    public partial class GstAdjust : ModuleBase
    {
        #region sql
        // ---------------------------------------------------------------------------------------------------------------
        const string SupplierSql = @"
select
    s.ParentFolderId as SupplierFolderID,
	s.SupplierID, 
	SupplierName,
	c.CountryName as Country,
	v.ServiceName, 
	ValidFrom, 
	ValidTo, 
	OptionName,
	PricingOption,
	case when (PricingOption = 'nm' and Net is not null and Net <> 0) then (Gross - Net)/Net*100 else null end as Markup,	
	case when (PricingOption = 'gc' and Gross is not null and Gross <> 0) then (Gross - Net)/Gross*100 else null end as Commission,		
	case when PricingOption = 'nm' or PricingOption = 'ng'	then Net else null end as Net,	
	case when PricingOption = 'nm' or PricingOption = 'ng'	
		then cast( ((Net * 100) / (100 + 12.5)) + 	(((Net * 100) / (100 + 12.5)) * 15/100) as money)
		else null end as Net15,	
	case when PricingOption = 'ng' or PricingOption = 'gc'	then Gross else null end as Gross,
	case when PricingOption = 'ng' or PricingOption = 'gc'	
		then cast( ((Gross * 100) / (100 + 12.5)) + (((Gross * 100) / (100 + 12.5)) * 15/100) as money)
		else null end as Gross15	
	--,GST_old      = ((Gross * 100) / (100 + 12.5)) * 12.5/100
	--,Gross_exc	=  (Gross * 100) / (100 + 12.5)
	--,GST_new	    = ((Gross * 100) / (100 + 12.5)) * 15/100
	--,Gross_new	= ((Gross * 100) / (100 + 12.5)) + 
	--			     (((Gross * 100) / (100 + 12.5)) * 15/100)				
from [option] o
inner join [rate] r on  o.RateID = r.RateID
inner join [Service] v on r.ServiceID = v.ServiceID
inner join [Supplier] s on v.SupplierID = s.SupplierID
inner join [Country] c on s.CountryID = c.CountryID and c.CountryName like '%zealand%'
where {0}
--where ValidFrom <= '2010.10.1 00:00:00' and ValidTo >= '2010.10.1 00:00:00'
--where ValidFrom >= '2010.10.1 00:00:00'
and o.GstUpdated is null or o.GstUpdated = 'false'
and Gross is not null and Gross > 0
order by SupplierName, ServiceName, ValidFrom, OptionName";

        // ---------------------------------------------------------------------------------------------------------------
        private const string ItinerarySql = @"
select 
	i.ParentFolderID as ItineraryFolderID,
	s.ParentFolderID as SupplierFolderID,
	i.ItineraryID,
	i.ItineraryName,
	i.GrossOverride,
	t.ItineraryStatusName,
    s.SupplierID,	
    s.SupplierName, 
	l.PurchaseLineName,
	p.PurchaseItemName,
	c.CountryName,
	r.RequestStatusName,
	p.Net,
	cast( ((Net * 100) / (100 + 12.5)) + (((Net * 100) / (100 + 12.5)) * 15/100) as money) as Net15,	
	p.Gross,	
	cast( ((Gross * 100) / (100 + 12.5)) + (((Gross * 100) / (100 + 12.5)) * 15/100) as money) as Gross15		
from PurchaseItem p
inner join PurchaseLine l on p.PurchaseLineID = l.PurchaseLineID
left join RequestStatus r on p.RequestStatusID = r.RequestStatusID
inner join Itinerary i on l.ItineraryID = i.ItineraryID
left join ItineraryStatus t on i.ItineraryStatusID = t.ItineraryStatusID
inner join Supplier s on l.SupplierID = s.SupplierID
inner join [Country] c on s.CountryID = c.CountryID and c.CountryName like '%zealand%'
where {0}
--where p.StartDate <= '2010.10.1 00:00:00' and EndDate >= '2010.10.1 00:00:00'
--where StartDate >= '2010.10.1 00:00:00'
and p.GstUpdated is null or p.GstUpdated = 'false'
and Gross is not null and Gross > 0
order by i.ItineraryName, l.PurchaseLineName, StartDate";
        // ---------------------------------------------------------------------------------------------------------------
        #endregion

        public GstAdjust()
        {
            InitializeComponent();

            //txtFrom.DateTime = new DateTime(DateTime.Now.Year, 1, 1);
            //txtTo.DateTime = new DateTime(DateTime.Now.Year, 12, 31);
            
            grid.ExportFileName = "nz-gst.xls";
            grid.InitializeLayoutEvent += grid_InitializeLayout;
            grid.UltraGrid.InitializeRow += grid_InitializeRow;
            //grid.UltraGrid.DoubleClickRow += grid_DoubleClickRow;
            grid.UltraGrid.MouseClick += gridBookings_MouseClick;

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

        private static string GetSql(int index)
        {
            switch (index)
            {
                case 1:
                    return string.Format(SupplierSql, "ValidFrom <= '2010.10.1 00:00:00' and ValidTo >= '2010.10.1 00:00:00'");
                case 2:
                    return string.Format(SupplierSql, "ValidFrom >= '2010.10.1 00:00:00'");
                case 3:
                    return string.Format(ItinerarySql, "StartDate <= '2010.10.1 00:00:00' and EndDate >= '2010.10.1 00:00:00'");
                case 4:
                    return string.Format(ItinerarySql, "StartDate >= '2010.10.1 00:00:00'");
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
        }

        private bool _isSelectAll = true;
        private void SelectAll()
        {
            _isSelectAll = !_isSelectAll;
            foreach (var row in grid.UltraGrid.Rows)
            {
                if (row == null || row.GetType() == typeof(UltraGridEmptyRow) || row.GetType() == typeof(UltraGridGroupByRow))
                    return;

                row.Cells["IsSelected"].Value = _isSelectAll;
            }
        }

        private static void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("IsSelected"))
                e.Layout.Bands[0].Columns.Insert(0, "IsSelected");

            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "SupplierFolderID" || c.Key == "ItineraryFolderID") { c.Hidden = true; }

                else if (c.Key == "IsSelected")
                {
                    c.Header.Caption = String.Empty;
                    c.DataType = typeof(Boolean);
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.Header.Appearance.Image = Properties.Resources.CheckBox;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.Width = 30;
                    c.MaxWidth = 30;
                    c.SortIndicator = SortIndicator.Disabled;
                }
                else if (c.Key.Contains("Net") || c.Key.Contains("Gross"))
                {
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key.Contains("Markup") || c.Key.Contains("Commission"))
                {
                    c.Format = "##0.00\\%";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
            }
        }

        private void grid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["IsSelected"].Value == null)
                e.Row.Cells["IsSelected"].Value = true;
        }

        private void gridBookings_MouseClick(object sender, MouseEventArgs e)
        {
            var clickedElement = grid.UltraGrid.DisplayLayout.UIElement.ElementFromPoint(grid.UltraGrid.PointToClient(MousePosition));
            if (clickedElement == null) return;

            var headerElement = (HeaderUIElement)clickedElement.GetAncestor(typeof(HeaderUIElement));
            if (headerElement == null) return;

            var column = (UltraGridColumn)headerElement.GetContext(typeof(UltraGridColumn));
            if (column.Key == "IsSelected") SelectAll();
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
            btnSupplier.Enabled = btnItinerary.Enabled = true;

            LoadData(comboBox1.SelectedIndex);
            if (comboBox1.SelectedIndex < 3) btnItinerary.Enabled = false;

            Cursor = Cursors.Default;
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            var row = grid.UltraGrid.ActiveRow;

            if (row == null ||
                row.GetType() == typeof(UltraGridEmptyRow) ||
                row.GetType() == typeof(UltraGridGroupByRow))
                return;

            if (!row.Cells.Exists("SupplierID")) return;

            var info = new NavigationTreeItemInfo((int)row.Cells["SupplierID"].Value,
                                                  (string)row.Cells["SupplierName"].Value,
                                                  NavigationTreeItemInfo.ItemTypes.Supplier,
                                                  (int)row.Cells["SupplierFolderID"].Value, true);

            var node = App.MainForm.BuildMenuNode(info);
            App.MainForm.Load_SupplierForm(node);
        }

        private void btnItinerary_Click(object sender, EventArgs e)
        {
            var row = grid.UltraGrid.ActiveRow;

            if (row == null ||
                row.GetType() == typeof(UltraGridEmptyRow) ||
                row.GetType() == typeof(UltraGridGroupByRow))
                return;

            if (!row.Cells.Exists("ItineraryID")) return;
            var info = new NavigationTreeItemInfo((int)row.Cells["ItineraryID"].Value,
                                                  (string)row.Cells["ItineraryName"].Value,
                                                  NavigationTreeItemInfo.ItemTypes.Itinerary,
                                                  (int)row.Cells["ItineraryFolderID"].Value, true);

            var node = App.MainForm.BuildMenuNode(info);
            App.MainForm.Load_ItineraryForm(node);
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
    }
}
