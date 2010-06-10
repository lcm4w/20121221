using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Info.Services;

namespace TourWriter.Modules.Plugins.NzGst
{
    public partial class GstAdjust : ModuleBase
    {
        private const string InfoMsg =
@"Use the NZ GST Adjust screen to adjust prices for the NZ GST change to 15%.
(to print this text, right-click > select all, right-click > copy, and paste into email/word for printing).

Important considerations:
1. Supplier rates that span 1 Oct will not be split, they will just be updated to 15% (if you choose to).
2. The above will probably not matter as pre 1 Oct quotes are most likely almost finished with?
3. Itinerary bookings that get updated will affect their Itinerary price (unless Itinerary has a final $ override).
4. For the above, you might choose to only update bookings where Itinerary has status 'Final' (or similar).

Getting started:
1. Use the combo box to choose which data to load.
2. Preview the data carefully, you may choose to do manual work (split?) on data that spans 1 Oct.
3. Consider exporting the list to Excel for future reference (save the trees).
4. Select which rows to update (click col header to un/select all).
5. You can update in batches, you don't have to update all at once.
6. Press the Update button to start the 3 step update process on selected rows.
7. Safety measures ensure that each row can only be updated once (on screen an in database).

Previewing the data:
1. System automatically loads only Countries that contain 'zealand' (case insensitive, we only want New Zealand suppliers).
2. System automatically loads only Gross values that are a valid and positive number.
3. Supplier data shows rate info as entered (net and markup, net and gross, or gross and commission).
4. Columns Net15 and Gross15 show the intended price adjustment.
5. The calculation for these is: ((Amount*100) / (100+12.5)) + (((Amount*100) / (100+12.5)) * 15/100)
    eg remove old GST 12.5% then add new GST 15%.
6. Data will be updated as shown, one update per row. Dates that span 1 Oct WILL NOT be split for you.

Running the update:
1. When you choose to update, only the rows that you select will be updated.
2. The update process is in 3 steps, you can pull out at any time before the end (ask boss to press the final one ;)
3. A database backup safety-snapshot is including in one of the update steps.
3. If the final update step fails, all changes will be rolled back for safety (all or nothing).
4. After a row is updated, it is protected (disabled) so that you cannot update it a second time.

After the update:
Processed rows will be disabled when previewed again, so they cannot be updated twice. They are also marked as processed in the database for further safety and future reference. So it is safe to continue to play with these screens and run further updates on any unprocessed rows as required.
";
        #region sql
        // ---------------------------------------------------------------------------------------------------------------
        const string SupplierSql = @"
select
    isnull(GstUpdated, cast(0 as bit)) as GstUpdated,
    s.ParentFolderId as SupplierFolderID,
	s.SupplierID, 
    o.OptionID,
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
	case when ((GstUpdated is null or GstUpdated = 'false') and (PricingOption = 'nm' or PricingOption = 'ng'))	
		then cast( ((Net * 100) / (100 + 12.5)) + 	(((Net * 100) / (100 + 12.5)) * 15/100) as money)
		else null end as Net15,	
	case when PricingOption = 'ng' or PricingOption = 'gc'	then Gross else null end as Gross,
	case when ((GstUpdated is null or GstUpdated = 'false') and (PricingOption = 'ng' or PricingOption = 'gc'))
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
--and o.GstUpdated is null or o.GstUpdated = 'false'
and Gross is not null and Gross > 0
order by SupplierName, ServiceName, ValidFrom, OptionName";

        // ---------------------------------------------------------------------------------------------------------------
        private const string ItinerarySql = @"
select 
    isnull(GstUpdated, cast(0 as bit)) as GstUpdated,
	i.ParentFolderID as ItineraryFolderID,
	s.ParentFolderID as SupplierFolderID,
	i.ItineraryID,
    p.PurchaseItemID,
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
	case when GstUpdated is null or GstUpdated = 'false' then 
        cast( ((Net * 100) / (100 + 12.5)) + (((Net * 100) / (100 + 12.5)) * 15/100) as money) else null end as Net15,	
	p.Gross,
	case when GstUpdated is null or GstUpdated = 'false' then 	
	    cast( ((Gross * 100) / (100 + 12.5)) + (((Gross * 100) / (100 + 12.5)) * 15/100) as money) else null end as Gross15
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
--and p.GstUpdated is null or p.GstUpdated = 'false'
and Gross is not null and Gross > 0
order by i.ItineraryName, l.PurchaseLineName, StartDate";

        const string UpdateSql = 
@"update [{0}] set
	Net =   ((Net   * 100) / (100 + 12.5)) + (((Net   * 100) / (100 + 12.5)) * 15/100),
	Gross = ((Gross * 100) / (100 + 12.5)) + (((Gross * 100) / (100 + 12.5)) * 15/100),
	GstUpdated = 1 
where {0}ID = {1};";

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
                                             "Supplier Rates that span 1 Oct", 
                                             "Supplier Rates after 1 Oct",
                                             "Itinerary Bookings that span 1 Oct", 
                                             "Itinerary Bookings after 1 Oct"
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

        private bool _isSelectAll;
        private void SelectAll()
        {
            _isSelectAll = !_isSelectAll;
            foreach (var row in grid.UltraGrid.Rows)
            {
                if (row == null || row.GetType() == typeof(UltraGridEmptyRow) || row.GetType() == typeof(UltraGridGroupByRow))
                    continue;

                if (_isSelectAll && row.Cells["GstUpdated"].Value != DBNull.Value && (bool)row.Cells["GstUpdated"].Value)
                    continue;

                row.Cells["IsSelected"].Value = _isSelectAll;
            }
        }
        
        private void ProcessUpdate()
        {
            string sqlBatch;

            // prepare --------------------
            if (!App.AskYesNo("Step 1 of 3:\r\n\r\nReady to prepare data for selected rows.\r\n\r\nContinue?")) return;
            try
            {
                Cursor = Cursors.WaitCursor;

                var list = grid.GetDataRowsList();
                if (list.Count == 0) { App.ShowInfo("No rows selected, cancelling process..."); return; }

                sqlBatch = CreateUpdateScript(list);
                if (sqlBatch.Length == 0) { App.ShowInfo("Update script is empty (no rows selected?), cancelling process..."); return; }
            } 
            finally { Cursor = Cursors.Default; }
            
            // backup ----------------------
            if (!App.AskYesNo("Step 2 of 3:\r\n\r\nReady to create database restore point.\r\n\r\nContinue?")) return;
            try
            {
                Cursor = Cursors.WaitCursor;
                BackupDb();
            } 
            finally { Cursor = Cursors.Default; }

            // update -----------------------
            if (!App.AskYesNo("Step 3 of 3:\r\n\r\nReady to commit selected rows to the database.\r\n\r\n*FINAL* Continue?")) return;
            try
            {
                Cursor = Cursors.WaitCursor;
                UpdateToDb(sqlBatch);
                btnLoad.PerformClick();
                App.ShowInfo("Update completed.\r\n\r\nNOTE: updated rows will now be disabled.");
            }
            catch (Exception ex) { App.Error("Oops, an error occurred! But its OK, all changes have been ROLLED BACK to previous state. Please work through this issue with TourWriter support staff.", ex, true); }
            finally { Cursor = Cursors.Default; }
        }

        private static string CreateUpdateScript(IList<UltraGridRow> list)
        {
            var isItinerary = list[0].Cells.Exists("PurchaseItemID");

            var batch = new StringBuilder();
            foreach (var row in list)
            {
                if (row == null) continue;
                if (row.Cells["IsSelected"].Value != DBNull.Value && !(bool)row.Cells["IsSelected"].Value) continue;
                if (row.Cells["GstUpdated"].Value != DBNull.Value && (bool)row.Cells["GstUpdated"].Value) continue;

                var name = isItinerary
                               ? "PurchaseItem"
                               : "Option";
                var id = isItinerary
                             ? (int)row.Cells["PurchaseItemID"].Value
                             : (int)row.Cells["OptionID"].Value;

                batch.AppendLine(string.Format(UpdateSql, name, id));
            }
            return batch.ToString();
        }

        private static void BackupDb()
        {
            DatabaseHelper.Backup("TourWriter-PreGst-" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".bak");
        }

        private static void UpdateToDb(string sqlBatch)
        {
            DatabaseHelper.RunScriptWithTransaction(sqlBatch);
        }


        private static void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("IsSelected"))
                e.Layout.Bands[0].Columns.Insert(0, "IsSelected");

            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "OptionID" || 
                    c.Key == "PurchaseItemID" ||
                    c.Key == "SupplierFolderID" ||
                    c.Key == "SupplierFolderID") { c.Hidden = true; }

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
                else if (c.Key == "GstUpdated")
                {
                    c.Header.Caption = "Done";
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
            if (e.Row.Cells["IsSelected"].Value == null) e.Row.Cells["IsSelected"].Value = true;

            var isUpdated = e.Row.Cells["GstUpdated"].Value != DBNull.Value && (bool) e.Row.Cells["GstUpdated"].Value;
            if (isUpdated) e.Row.Activation = Activation.Disabled;
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

            LoadData(comboBox1.SelectedIndex);

            btnItinerary.Enabled = comboBox1.SelectedIndex > 2;
            btnGo.Enabled = btnSupplier.Enabled = btnLoad.Enabled = comboBox1.SelectedIndex > 0;

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

        private void btnGo_Click(object sender, EventArgs e)
        {
            ProcessUpdate();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            var form = new Form {Size = new System.Drawing.Size(900, 900)};
            var txt = new TextBox
                          {
                              Multiline = true,
                              Dock = DockStyle.Fill,
                              ReadOnly = true,
                              Text = InfoMsg,
                              SelectedText = "",
                              Font = new System.Drawing.Font("Ariel", 12),
                              ScrollBars = ScrollBars.Both
                          };
            txt.SelectionStart = 0;
            txt.SelectionLength = 0;
            form.Controls.Add(txt);
            form.Show();
        }
    }
}
