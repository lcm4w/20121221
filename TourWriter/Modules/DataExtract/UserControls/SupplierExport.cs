using System;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;
using TourWriter.Info.Services;

namespace TourWriter.Modules.DataExtract.UserControls
{
    public partial class SupplierExport : UserControl
    {
        public SupplierExport()
        {
            InitializeComponent();

            grid.ExportFileName = "SupplierData.xls";
            grid.InitializeLayoutEvent += grid_InitializeLayoutEvent;
            grid.UltraGrid.DoubleClickRow += grid_DoubleClickRow;
        }

        private void LoadData()
        {
            grid.DataSource = DataSetHelper.FillDataSet("_DataExtract_GetSupplierDetails");
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            LoadData();
            Cursor = Cursors.Default;
        }

        private static void grid_InitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ParentFolderID")
                {
                    c.Hidden = true;
                }
            }
        }

        private static void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow) ||
                e.Row.GetType() == typeof(UltraGridGroupByRow))
                return;

            NavigationTreeItemInfo info = new NavigationTreeItemInfo(
                (int)e.Row.Cells["SupplierID"].Value,
                (string)e.Row.Cells["SupplierName"].Value,
                NavigationTreeItemInfo.ItemTypes.Supplier,
                (int)e.Row.Cells["ParentFolderID"].Value,
                true);

            UltraTreeNode node = App.MainForm.BuildMenuNode(info);
            App.MainForm.Load_SupplierForm(node);
        }
    }
}
