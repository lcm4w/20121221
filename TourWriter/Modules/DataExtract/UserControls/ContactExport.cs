using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Info.Services;

namespace TourWriter.Modules.DataExtract.UserControls
{
    public partial class ContactExport : UserControl
    {
        public ContactExport()
        {
            InitializeComponent();

            grid.ExportFileName = "ContactData.xls";
            grid.InitializeLayoutEvent += GridInitializeLayoutEvent;
            grid.UltraGrid.DoubleClickRow += GridoubleClickRow;
        }

        private void LoadData()
        {
            grid.DataSource = null;
            var ds = DataSetHelper.FillDataset(new DataSet(), "select * from ContactDetail");
            grid.DataSource = ds.Tables[0];
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try { LoadData(); }
            finally { Cursor = Cursors.Default; }
        }

        private static void GridInitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Exists("ParentFolderID"))
                e.Layout.Bands[0].Columns["ParentFolderID"].Hidden = true;
        }

        private static void GridoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow) ||
                e.Row.GetType() == typeof(UltraGridGroupByRow))
                return;

            var folderId = e.Row.Cells.Exists("ParentFolderID") ? (int) e.Row.Cells["ParentFolderID"].Value : -1;

            var info = new NavigationTreeItemInfo(
                (int) e.Row.Cells["ContactID"].Value, 
                (string) e.Row.Cells["ContactName"].Value,
                NavigationTreeItemInfo.ItemTypes.Contact,
                folderId,
                true);

            var node = App.MainForm.BuildMenuNode(info);
            App.MainForm.Load_ContactForm(node);
        }
    }
}
