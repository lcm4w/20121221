using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.Search
{
    public partial class SearchMain : ModuleBase
    {
        #region sql
        const string ItinearySql =
@"select ItineraryID, ItineraryName, CustomCode, ArriveDate, ItineraryStatusName [Status], CountryName [Origin], UserName [AssignedTo], DepartmentName [Department],
ParentFolderID, i.IsRecordActive, i.AddedOn
from Itinerary i
left join ItineraryStatus s on i.ItineraryStatusID = s.ItineraryStatusID
left join Country c on i.CountryID = c.CountryID
left join [User] u on i.AssignedTo = u.UserID
left join Department d on i.DepartmentId = d.DepartmentId
where (IsDeleted = 0 or IsDeleted is null)
and {0}";

        const string PurchaseLineSql =
@"select PurchaseLineID as BookingId, PurchaseLineName as BookingName, p.SupplierReference, 
i.ItineraryID, ItineraryName, CustomCode, ArriveDate, ItineraryStatusName [Status], CountryName [Origin], UserName [AssignedTo], DepartmentName [Department],
ParentFolderID, i.IsRecordActive, i.AddedOn
from Itinerary i
left join ItineraryStatus s on i.ItineraryStatusID = s.ItineraryStatusID
left join Country c on i.CountryID = c.CountryID
left join [User] u on i.AssignedTo = u.UserID
left join Department d on i.DepartmentId = d.DepartmentId
left join PurchaseLine p on p.ItineraryID = i.ItineraryID
where (IsDeleted = 0 or IsDeleted is null)
and {0}";

        const string SupplierSql =
@"select SupplierID, SupplierName, CityName [City], CountryName [Country], ParentFolderID, IsRecordActive, AddedOn 
from Supplier s
left join City c on s.CityID = c.CityID
left join Country t on s.CountryID = t.CountryID
--where (IsDeleted = 0 or IsDeleted is null) -- don't hide *deleted*, so that user can undelete
where {0}";

        const string ContactSql =
@"select ContactId, ContactName, CityName [City], CountryName [Country], ParentFolderID, IsRecordActive, AddedOn 
from Contact co
left join City ci on co.CityID = ci.CityID
left join Country cn on co.CountryID = cn.CountryID
where (IsDeleted = 0 or IsDeleted is null)
and {0}";
        #endregion
        
        public SearchMain()
        {
            Icon = Icon.FromHandle(Properties.Resources.Magnifier.GetHicon()); 
            
            InitializeComponent();
            displayTypeName = "Search";

            // Merged with parent.
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;


            cmbTableName.Items.Add("Itinerary", "Itineraries");
            cmbTableName.Items.Add("Supplier", "Suppliers");
            cmbTableName.Items.Add("Contact", "Contacts");
            cmbTableName.Items.Add("PurchaseLine", "Bookings");
            cmbTableName.SelectedIndex = 0;
            cmbColName.SelectedIndex = 0;
        }
        
        public void SetDefaultSearch(string tableName)
        {
            var index = -1;
            for (var i = 0; i < cmbTableName.Items.Count; i++)
            {
                if (cmbTableName.Items[i].DataValue.ToString() != tableName) continue;
                index = i;
                break;
            }
            cmbTableName.SelectedIndex = index;
            
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            var tableName = cmbTableName.SelectedItem.DataValue.ToString();
            var colName = cmbColName.SelectedItem.DataValue.ToString();
            var comparison = cmbComparison.SelectedItem.DataValue.ToString();
            var text = txtSearch.Text.ToLower().Replace("\'", "\'\'").
                Replace("update ", "").Replace("update/", "").
                Replace("delete ", "").Replace("delete/", "").
                Replace("drop ", "").Replace("drop/", "").
                Replace("--", "");

            var sql = "";

            switch (tableName)
            {
                case "Itinerary":
                    sql = ItinearySql;
                    break;
                case "Supplier":
                    sql = SupplierSql;
                    break;
                case "Contact":
                    sql = ContactSql;
                    break;
                case "PurchaseLine":
                    sql = PurchaseLineSql;
                    break;
            }
            if (sql == "")
            {
                App.ShowError("No sql statement found for that search");
                return;
            }

            var filter = colName + " ";
            switch (comparison)
            {
                case "contains":
                    filter += " like '%" + text + "%'";
                    break;
                case "starts":
                    filter += " like '" + text + "%'";
                    break;
                case "equals":
                    if (colName.EndsWith("ID"))
                    {
                        int i;
                        if (!int.TryParse(text, out i))
                        {
                            App.ShowInfo("ID must be a number");
                            return;
                        }
                        filter += " = " + i;
                    }
                    else
                        filter += " = '" + text + "'";
                    break;
            }
            sql = string.Format(sql, filter);
            
            Cursor = Cursors.WaitCursor;
            try
            {
                App.Debug(sql);
                Cursor = Cursors.WaitCursor;
                Thread.Sleep(500);  // show cursor...

                var ds = Info.Services.DatabaseHelper.ExecuteDataset(sql);
                if (ds != null) grid.DataSource = ds;
                SetFormActiveText();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        
        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ParentFolderID" || c.Key == "Type") 
                    c.Hidden = true;
                c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter; 
            }
            GridHelper.SetDefaultGridAppearance(e);
            
            // sort
            e.Layout.Bands[0].SortedColumns.Clear();
            var colName = cmbColName.SelectedItem.DataValue.ToString();
            if (colName == "PurchaseLineName") colName = "BookingName";
            if (colName == "PurchaseLineID") colName = "BookingId";
            e.Layout.Bands[0].SortedColumns.Add(e.Layout.Bands[0].Columns[colName], false);
        }

        private void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow)) return;
            
            UltraTreeNode node;
            NavigationTreeItemInfo info;
            NavigationTreeItemInfo.ItemTypes clickItem;
            var tableName = cmbTableName.SelectedItem.DataValue.ToString();

            switch (tableName)
            {
                case "Itinerary":
                case "PurchaseLine":
                case "PurchaseItem":
                    clickItem = NavigationTreeItemInfo.ItemTypes.Itinerary;
                    info = new NavigationTreeItemInfo(
                        (int)e.Row.Cells["ItineraryID"].Value, (string)e.Row.Cells["ItineraryName"].Value, clickItem, (int)e.Row.Cells["ParentFolderID"].Value, (bool)e.Row.Cells["IsRecordActive"].Value);
                    node = App.MainForm.BuildMenuNode(info);
                    App.MainForm.Load_ItineraryForm(node);
                    break;

                case "Supplier":
                    clickItem = NavigationTreeItemInfo.ItemTypes.Supplier;
                    info = new NavigationTreeItemInfo(
                        (int)e.Row.Cells["SupplierID"].Value, (string)e.Row.Cells["SupplierName"].Value, clickItem, (int)e.Row.Cells["ParentFolderID"].Value, (bool)e.Row.Cells["IsRecordActive"].Value);
                    node = App.MainForm.BuildMenuNode(info);
                    App.MainForm.Load_SupplierForm(node);
                    break;

                case "Contact":
                    clickItem = NavigationTreeItemInfo.ItemTypes.Contact;
                    info = new NavigationTreeItemInfo(
                        (int)e.Row.Cells["ContactID"].Value, (string)e.Row.Cells["ContactName"].Value, clickItem, (int)e.Row.Cells["ParentFolderID"].Value, (bool)e.Row.Cells["IsRecordActive"].Value);
                    node = App.MainForm.BuildMenuNode(info);
                    App.MainForm.Load_ContactForm(node);
                    break;
            }
        }
        
        private void menuHelp_Click(object sender, EventArgs e)
        {
            App.ShowHelp("SearchMain");
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmbTableName_ValueChanged(object sender, EventArgs e)
        {
            cmbColName.Items.Clear();
            var tableName = cmbTableName.SelectedItem.DataValue.ToString();
            switch (tableName)
            {
                case "Itinerary":
                    cmbColName.Items.Add("ItineraryName", "Name");
                    cmbColName.Items.Add("ItineraryID", "ID");
                    cmbColName.Items.Add("CustomCode", "Custom Code");
                    break;
                case "Supplier":
                    cmbColName.Items.Add("SupplierName", "Name");
                    cmbColName.Items.Add("SupplierID", "ID");
                    break;
                case "Contact":
                    cmbColName.Items.Add("ContactName", "Name");
                    cmbColName.Items.Add("ContactID", "ID");
                    break;
                case "PurchaseLine":
                    cmbColName.Items.Add("PurchaseLineName", "Name");
                    cmbColName.Items.Add("PurchaseLineID", "ID");
                    cmbColName.Items.Add("SupplierReference", "Supplier Ref");
                    break;
                case "PurchaseItem":
                    cmbColName.Items.Add("PurchaseItemName", "Name");
                    cmbColName.Items.Add("PurchaseItemID", "ID");
                    break;
            }
            cmbColName.SelectedIndex = 0;
        }

        private void cmbColName_ValueChanged(object sender, EventArgs e)
        {
            cmbComparison.Items.Clear();
            var colName = cmbColName.SelectedItem.DataValue.ToString();

            switch (colName)
            {
                case "ItineraryName":
                case "SupplierName":
                case "ContactName":
                case "PurchaseLineName":
                case "PurchaseItemName":
                case "CustomCode":
                case "SupplierReference":
                    cmbComparison.Items.Add("contains", "contains");
                    cmbComparison.Items.Add("starts", "starts with");
                    cmbComparison.Items.Add("equals", "equals");
                    break;
                case "ItineraryID":
                case "SupplierID":
                case "ContactID":
                case "PurchaseLineID":
                case "PurchaseItemID":
                    cmbComparison.Items.Add("equals", "equals");
                    break;
            }
            cmbComparison.SelectedIndex = 0;
        }
        
        #region Override methods

        protected override bool IsDataDirty()
        {
            return false;
        }

        protected override string GetDisplayName()
        {
            return string.Format("Search {0} '{1}'", cmbTableName.Text, txtSearch.Text);
        }

        #endregion
    }
}