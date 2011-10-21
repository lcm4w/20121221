using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using Travelmesh;

namespace TourWriter.Services
{
    static class DataSyncUi
    {
        private const bool ENABLED = false;
        
        private const string ImgGrey = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAXBJREFUeNrUUz1Lw1AUvUnT5sOWQqAgJbZkENwqnQQnoeIv0UlnJ8Hd1dEf4D8IQsngLoh1qeJmaVWkWvryndTzYgXRxKWTDw653HfvuffcmyfMZjNa5Ii04FmYQMpydrvdCj4NYAVQgFfgAXjudDrR91jh5wyQXINvO47j43K53CjiBEHAXNc9F0XxFCF9kPiZHSBZQ/KWIAhnpmmqkiQRbIKvwhjbHQwGBvgO5t1kzmDZ87zDer2uIpAKhQKhagpN00jX9R3ErKOQlEdQk2V5jVfm0iAjRZIk6SUkiZCyAbOUN8TYcx1hOBySoihp+5wsij7nNp1OKQp9jVQ1dwujyeT9sSQrqxjcr+04jFGxKF/A9PMkvDDmHr2Nxwmv+iWBw/d9ur/r3yiqeoUtxJkdzNdj2ba9X61WTwzDWOISHMehXq932W6393D/9Od/MF+nzDdiWdZmGIZ6s9m8brVat/BNUCTJJEDSP31MHwIMAFeXpQ+WTSC5AAAAAElFTkSuQmCC";
        private const string ImgYellow = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAX5JREFUeNrMUk1LAlEUPW/GGXUGJ9MUBT9oEbgM3ATRwkUE/YD+Qbtatwr6ES1btugfRCuhXQuhpE2LQsIwE8mv0Rl98163LwLTiNx04bz3eJx77ieTUmIWY/9UoJgP0ZkhpAkBQpNwS3hCocR/FijmY1Lx1vtW+wDhZAa6qWHQtrVG60R3QofEuCER95PuG3M2JOMFOzo40hObQUULgDFAGghxo77tVC5SAdfa/cjmzZSx5BOOaO35rbWgT+pQuYBCUIWApsYg5xY3JMQyBfJNE4iBWTnVM6AMOFifYL/fisuhaRll5NorxNMnlwB46NUYqsfUujh1SCOGBfAOGARktw7PiRnwfzmMCzzygVX1m5UlyPa34UjHgBYwz+jpTiuhIfn8/rAXFePD8UYqug+pss+PEk3BmyxA47GSOG3fp3ee77K224lgaIfQb8ZRL+fOzai5Raz6bxbptcpE7UquCo6IuYDLcJZd01+HgojJAsX8n1ZZwYw2s8CLAAMAE+udFLu7tM0AAAAASUVORK5CYII=";
        private const string ImgGreen = "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";
        
        public static void AddExportHooks(this UltraGrid grid, SupplierSet supplierSet)
        {
            InitializeLayout(grid, supplierSet);
            grid.InitializeRow += (sender, e) => OnInitializeRow(e.Row, supplierSet);
            grid.AfterCellListCloseUp += (sender, e) => OnCellListCloseUp(e, supplierSet);
            grid.Rows.Refresh(RefreshRow.FireInitializeRow);
        }
        
        private static void InitializeLayout(UltraGrid grid, SupplierSet supplierSet)
        {
            var col = grid.DisplayLayout.Bands[0].Columns.Exists("Export") ? grid.DisplayLayout.Bands[0].Columns["Export"] : grid.DisplayLayout.Bands[0].Columns.Add("Export");
            //col.Width = 6;
            //col.Hidden = false;
            //col.Header.Caption = "";
            //col.Header.VisiblePosition = 0;
            //col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            //col.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            //col.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            //grid.ClickCellButton += (sender, e) => OnExportClick(e, supplierSet);
            //grid.InitializeRow += (sender, e) => OnInitializeRow(e.Row, supplierSet);
            //grid.Rows.Refresh(Infragistics.Win.UltraWinGrid.RefreshRow.FireInitializeRow);
            
            Infragistics.Win.ValueList list;
            if (!grid.DisplayLayout.ValueLists.Exists("syncOptionsList"))
            {
                list = grid.DisplayLayout.ValueLists.Add("syncOptionsList");
                //list.ValueListItems.Add("Push");
                list.ValueListItems.Add("Sync");
                list.ValueListItems.Add("Remove");
            }
            col.Width = 18;
            col.MinWidth = 18;
            col.MaxWidth = 18;
            col.Hidden = false;
            col.Header.Caption = "Sync";
            col.ProportionalResize = false;
            col.Style = ColumnStyle.DropDownList;
            
            col.CellActivation = Activation.AllowEdit;
            col.Header.ToolTipText = "Sync with online data";
            col.ValueList = grid.DisplayLayout.ValueLists["syncOptionsList"];
            col.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
            col.Header.VisiblePosition = 0;
            //col.ButtonDisplayStyle = ButtonDisplayStyle.OnCellActivate;
        }

        static void OnCellListCloseUp(CellEventArgs e, SupplierSet supplierSet)
        {
            var result = e.Cell.Text;
            ((UltraGrid) e.Cell.Row.Band.Layout.Grid).PerformAction(UltraGridAction.DeactivateCell);

            App.Debug("Sync click: " + result);
            if (result == "Sync")
                OnSyncClick(e, supplierSet);
            if (result == "Remove")
                OnRemoveClick(e, supplierSet);
            e.Cell.Value = null;
        }

        private static void OnInitializeRow(UltraGridRow row, SupplierSet supplierSet)
        {
            var cell = row.Cells["Export"];
            cell.ActiveAppearance.BackColor = Color.White;
            cell.ToolTipText = "Sync with Travelmesh";
            var exported = row.Cells["ImportID"].Value != DBNull.Value;
            cell.Appearance.Image = exported ? Base64ToImage(ImgGreen) : Base64ToImage(ImgGrey);
        }

        private static void OnSyncClick(CellEventArgs e, SupplierSet supplierSet)
        {
            if (e.Cell.Column.Key != "Export") return;
            if (supplierSet.HasChanges())
            {
                /** WARNING: must have no outstanding changes, as each item synced will call accept changes on the row **/
                App.ShowError("Must save data first"); // throw new ApplicationException("Must save data first");
                return;
            }

            var row = e.Cell.Row;
            row.Band.Layout.Grid.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            try
            {
                if (row.Cells.Exists("ServiceName"))
                {
                    var service = Enumerable.Where(supplierSet.Service, x => x.RowState != DataRowState.Deleted && x.ServiceID == (int)row.Cells["ServiceID"].Value).FirstOrDefault();
                    DataSync.SyncSupplier(service.SupplierRow);
                    DataSync.SyncService(service);
                    DataSync.CreateServiceRates(service);
                }
                else if (row.Cells.Exists("OptionName"))
                {
                    var option = Enumerable.Where(supplierSet.Option, x => x.RowState != DataRowState.Deleted && x.OptionID == (int)row.Cells["OptionID"].Value).FirstOrDefault();
                    if (option.RateRow.ValidTo < DateTime.Now.AddDays(1))
                    {
                        App.ShowInfo("You can only push current rates");
                        return;
                    }
                    var service = option.RateRow.ServiceRow;
                    var supplier = service.SupplierRow;

                    if (supplier.IsImportIDNull()) DataSync.SyncSupplier(supplier);
                    if (service.IsImportIDNull()) DataSync.SyncService(service);
                    DataSync.CreateServiceRates(service, option.OptionID);
                }
            }
            finally
            {
                row.Band.Layout.Grid.Cursor = System.Windows.Forms.Cursors.Default;
                row.Band.Layout.Grid.Rows.Refresh(Infragistics.Win.UltraWinGrid.RefreshRow.FireInitializeRow);
            }
        }
        
        private static void OnRemoveClick(CellEventArgs e, SupplierSet supplierSet)
        {
            if (e.Cell.Column.Key != "Export") return;
            
            var exported = e.Cell.Row.Cells["ImportID"].Value != DBNull.Value;
            if (!exported) return;

            e.Cell.Row.Cells["ImportID"].Value = DBNull.Value;
            //App.ShowInfo("Mapping removed.\r\n\r\nOnline data was not removed.");
        }

        private static Image Base64ToImage(string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            return Image.FromStream(ms, true);
        }
    }
}
