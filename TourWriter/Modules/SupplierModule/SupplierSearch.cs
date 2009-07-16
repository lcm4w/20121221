using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.BusinessLogic;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.SupplierModule
{
    public partial class SupplierSearch : UserControl
    {
        public event OnSupplierPickedHandler OnSupplierPicked;
        
        public SupplierSearch()
        {
            InitializeComponent();
        }
        
        private void DataBind()
        {
            cmbService.DataSource = Cache.ToolSet.ServiceType;
            cmbService.DisplayMember = "ServiceTypeName";
            cmbService.ValueMember = "ServiceTypeID";

            cmbCity.DataSource = Cache.ToolSet.City;
            cmbCity.ValueMember = "CityID";
            cmbCity.DisplayMember = "CityName";

            cmbRegion.DataSource = Cache.ToolSet.Region;
            cmbRegion.ValueMember = "RegionID";
            cmbRegion.DisplayMember = "RegionName";

            cmbGrade.DataSource = Cache.ToolSet.Grade;
            cmbGrade.ValueMember = "GradeID";
            cmbGrade.DisplayMember = "GradeName";
        }

        private DataTable GetSearchResults(int? cityId, int? regionId, int? gradeId, int? serviceTypeId)
        {
            return new Itinerary().SearchServices(cityId, regionId, gradeId, serviceTypeId);
        }

        private void FireSupplierPickedEvent(int supplierId, int serviceId)
        {
            if (OnSupplierPicked != null)
                OnSupplierPicked(new SupplierSearchEventArgs(supplierId, serviceId));
        }
        
        public SupplierSearchEventArgs GetSearchResults()
        {
            if (gridResults.ActiveRow != null)
                return new SupplierSearchEventArgs(
                    (int) gridResults.ActiveRow.Cells["SupplierID"].Value,
                    (int) gridResults.ActiveRow.Cells["ServiceID"].Value);
            
            return null;
        }
        
        #region Events

        private void SupplierSearch_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            DataBind();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            gridResults.DataSource = GetSearchResults(
                (int?)cmbCity.Value, (int?)cmbRegion.Value, (int?)cmbGrade.Value, (int?)cmbService.Value);

            if (gridResults.Rows.Count == 0)
                MessageBox.Show(App.GetResourceString("ShowNoSearchData"));
            else
                gridResults.ActiveRow = gridResults.Rows[0];
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            cmbService.Value = null;
            cmbCity.Value = null;
            cmbRegion.Value = null;
            cmbGrade.Value = null;
        }

        private void gridResults_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "SupplierName")
                {
                    c.Width = 60;
                    c.Header.Caption = "Supplier";
                    c.Band.SortedColumns.Add(c, false);
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                }
                else if (c.Key == "ServiceName")
                {
                    c.Width = 80;
                    c.Header.Caption = "Service";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                }
                else
                    c.Hidden = true;
            }
            // configure
            //GridHelper.Configure_OLD(e, true, true, false);
            GridHelper.SetDefaultGridAppearance(e);
           
        }

        private void gridResults_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow))
                return;

            FireSupplierPickedEvent(
                (int)e.Row.Cells["SupplierID"].Value,
                (int)e.Row.Cells["ServiceID"].Value);
        }

        private void cmbCity_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "CityName")
                {
                    c.SortIndicator = SortIndicator.Ascending;
                    c.NullText = "<all>";
                }
                else
                    c.Hidden = true;
            }
            GridHelper.SetDefaultComboAppearance(e, false);
        }

        private void cmbRegion_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "RegionName")
                {
                    c.SortIndicator = SortIndicator.Ascending;
                    c.NullText = "<all>";
                }
                else
                    c.Hidden = true;
            }
            GridHelper.SetDefaultComboAppearance(e, false);
        }

        private void cmbGrade_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "GradeName")
                {
                    c.SortIndicator = SortIndicator.Ascending;
                    c.NullText = "<all>";
                }
                else
                    c.Hidden = true;
            }
            GridHelper.SetDefaultComboAppearance(e, false);
        }

        private void cmbService_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ServiceTypeName")
                {
                    c.SortIndicator = SortIndicator.Ascending;
                    c.NullText = "<all>";
                }
                else
                    c.Hidden = true;
            }
            GridHelper.SetDefaultComboAppearance(e, false);
        }

        private void cmbCity_ValueChanged(object sender, EventArgs e)
        {
            // autofill connected region
            if (cmbCity.ActiveRow != null)
                cmbRegion.Value = cmbCity.ActiveRow.Cells["RegionID"].Value;
            else
                cmbRegion.Value = null;
        }
        
        #endregion
    }

    public delegate void OnSupplierPickedHandler(SupplierSearchEventArgs e);
    public class SupplierSearchEventArgs
    {
        public int supplierId;
        public int serviceId;
        
        public SupplierSearchEventArgs(int supplierId, int serviceId)
        {
            this.supplierId = supplierId;
            this.serviceId = serviceId;
        }
    }
}
