using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Forms;
using TourWriter.Info;
using System.Data.SqlClient;
using TourWriter.Info.Services;
using System;
using TourWriter.Services;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;
using Resources=TourWriter.Properties.Resources;
using System.Text;

namespace TourWriter.Modules.ItineraryModule.DateKicker
{
    public partial class DateKickerForm : Form
    {
        private string _origButtonText;
        private readonly ItinerarySet itinerarySet;
        private readonly ItinerarySet.PurchaseItemDataTable purchaseItemTable;
        private readonly bool autoStart;
        bool stopUpdateProcess;

        private int _dayOffset = 0;

        /// <summary>
        /// Moves booking dates and attempts to update the rates.
        /// </summary>
        /// <param name="itinerarySet">The itinerary set to get data from.</param>
        /// <param name="dayOffset">The number of days to shift the dates by.</param>
        /// <param name="autoStart">Whether to start the update process as soon as the form is shown.</param>
        public DateKickerForm(ItinerarySet itinerarySet, int dayOffset, bool autoStart)
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;
            this.itinerarySet = itinerarySet;
            txtDayOffset.Value = dayOffset;
            this.autoStart = autoStart;
            purchaseItemTable = (ItinerarySet.PurchaseItemDataTable)itinerarySet.PurchaseItem.Copy();
            gridBookings.DataSource = purchaseItemTable;
            btnStartStop.Select();
            btnStartStop.Focus();
            _origButtonText = btnStartStop.Text;
            this._dayOffset = dayOffset;

            chkUpdateStartDate.Checked = true;
            gridBookings.AfterCellListCloseUp += gridBookings_AfterCellListCloseUp;
            
            if (!itinerarySet.Itinerary[0].IsArriveDateNull())
            {
                dtpOldStartDate.Value = itinerarySet.Itinerary[0].ArriveDate;
                dtpNewStartDate.Value = itinerarySet.Itinerary[0].ArriveDate.AddDays(dayOffset);
            }
            else dtpNewStartDate.Value = dtpOldStartDate.Value = null;

            if (!itinerarySet.Itinerary[0].IsDepartDateNull())
            {
                dtpOldEndDate.Value = itinerarySet.Itinerary[0].DepartDate;
                dtpNewEndDate.Value = itinerarySet.Itinerary[0].DepartDate.AddDays(dayOffset);
            }
            else dtpNewEndDate.Value = dtpOldEndDate.Value = null;

            dtpNewStartDate.ValueChanged += dtpNewStartDate_ValueChanged;
            
            SetNewBookingDates(dayOffset);
        }

        void  gridBookings_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "StartDate" && e.Cell.EditorResolved != null)
            {
                e.Cell.Row.Cells["Result"].Appearance.Image = null;
                e.Cell.Row.Cells["Result"].ToolTipText = "";
                e.Cell.Row.Cells["Result"].Value = "";
                
                if (App.AskYesNo("Also move following booking dates?"))
                {
                    var id = (int)e.Cell.Row.Cells["PurchaseItemID"].Value;
                    var oldDate = (DateTime)e.Cell.Row.Cells["StartDate"].Value;
                    var newDate = (DateTime)e.Cell.Row.Cells["StartDate"].EditorResolved.Value;
                    var offset = (newDate.Date - oldDate.Date).Days;
                    SetSelectedRow(id, offset);
                }
            }
        }

        public void SetSelectedRow(int purchaseItemId, int dayOffset)
        {
            // select row and all following rows (date order, eg move following bookings too)
            var item = purchaseItemTable.FirstOrDefault(x => x.RowState != DataRowState.Deleted && x.PurchaseItemID == purchaseItemId);
            if (item != null)
            {
                var items = purchaseItemTable.Where(x => x.RowState != DataRowState.Deleted &&
                    (x == item || x.StartDate > item.StartDate || (x.StartDate == item.StartDate && x.PurchaseItemID > purchaseItemId))).
                    Select(x => x.PurchaseItemID);
                SetSelectedRows(items.ToList());
                
                SetNewBookingDates(dayOffset);
            }
        }

        public void SetSelectedRows(List<int> purchaseItemIdList)
        {
            foreach(UltraGridRow row in gridBookings.Rows)
            {
                bool isInSelectList = purchaseItemIdList.Contains((int)row.Cells["PurchaseItemID"].Value);
                bool isRowLocked = row.Cells["IsLockedAccounting"].Value != DBNull.Value &&
                                   (bool)row.Cells["IsLockedAccounting"].Value;

                row.Cells["IsSelected"].Value = isInSelectList && !isRowLocked;
            }
        }

        private void SetNewBookingDates(int daysOffset)
        {
            foreach (UltraGridRow row in gridBookings.Rows)
            {
                // set date
                if ((bool)row.Cells["IsSelected"].Value)
                    row.Cells["StartDate"].Value = ((DateTime)row.Cells["OldDate"].Value).AddDays(daysOffset);
                else // reset date
                    row.Cells["StartDate"].Value = ((DateTime)row.Cells["OldDate"].Value);

                // reset price
                if (row.Cells["OldNet"].Value != null && row.Cells["OldNet"].Value != DBNull.Value)
                    row.Cells["Net"].Value = row.Cells["OldNet"].Value;

                // reset status
                row.Cells["Result"].Appearance.Image = null;
                row.Cells["Result"].ToolTipText = "";
                row.Cells["Result"].Value = "";
            }
        }

        private void UpdateOptions()
        {
            foreach (UltraGridRow row in gridBookings.Rows)
            {
                if (stopUpdateProcess)
                    break;

                // reset result column
                Invoke(new MethodInvoker(
                delegate
                {
                    row.Cells["Result"].Value = null;
                    row.Cells["Result"].Appearance.Image = null;
                    row.Cells["Result"].ToolTipText = string.Empty;
                }));

                if ((bool)row.Cells["IsSelected"].Value && row.Cells["StartDate"].Value != DBNull.Value)
                {
                    //DateTime startDate = ((DateTime) row.Cells["OldDate"].Value).AddDays((int) txtDayOffset.Value);
                    var startDate = (DateTime) row.Cells["StartDate"].Value;

                    DateTime? endDate = null;
                    if (row.Cells["EndDate"].Value != DBNull.Value)
                        endDate = ((DateTime)row.Cells["EndDate"].Value).AddDays((int)txtDayOffset.Value);

                    Invoke(new MethodInvoker(
                    delegate
                    {
                        row.Cells["StartDate"].Value = startDate;
                        if (endDate != null)
                            row.Cells["EndDate"].Value = (DateTime)endDate;
                    }));

                    SqlParameter paramOptionID = new SqlParameter("@OptionID", row.Cells["OptionID"].Value);
                    SqlParameter paramStartDate = new SqlParameter("@NewDate", startDate);
                    ItinerarySet.OptionLookupDataTable optionLookup = new ItinerarySet.OptionLookupDataTable();
                    DataSetHelper.FillDataTable(optionLookup, "_Option_GetNewFromDate", paramOptionID, paramStartDate);

                    if (optionLookup.Rows.Count > 0)
                    {
                        Invoke(new MethodInvoker(
                        delegate
                        {
                            // if the end date is null, calculate it based on NumberOfDays
                            if (endDate == null && row.Cells["NumberOfDays"].Value != DBNull.Value)
                            {
                                double numberOfDays = (double)row.Cells["NumberOfDays"].Value;
                                endDate = startDate.AddDays(numberOfDays);
                            }

                            if (endDate != null && endDate > optionLookup[0].ValidTo)
                            {
                                // booking spans multiple rates
                                row.Cells["Result"].Appearance.Image = Resources.Warning;
                                row.Cells["Result"].Appearance.Cursor = Cursors.Hand;
                                row.Cells["Result"].ToolTipText = "Booking spans multiple rates";
                            }
                            else
                            {
                                row.Cells["Result"].Appearance.Image = Resources.Tick;
                            }
                        }));

                        // commented out below to update existing Rates too, with possible updates from the same underlying Option
                        //if (optionLookup[0].OptionID != (int) row.Cells["OptionID"].Value)
                        //{
                            // new rates are different so update the booking data
                            itinerarySet.OptionLookup.Merge(optionLookup, false);
                            itinerarySet.OptionLookup.AcceptChanges();

                            Invoke(new MethodInvoker(
                            delegate
                            {
                                row.Cells["OptionID"].Value = optionLookup[0].OptionID;
                                row.Cells["Net"].Value = (!optionLookup[0].IsNetNull()) ? optionLookup[0].Net : 0;
                                row.Cells["Gross"].Value = (!optionLookup[0].IsGrossNull()) ? optionLookup[0].Gross : 0;
                            }));
                        //}
                    }
                    else
                    {
                        // failed to update rates
                        Invoke(new MethodInvoker(
                        delegate
                        {
                            row.Cells["Result"].Value = "Update...";
                            row.Cells["Result"].Appearance.Image = Resources.Cross;
                            row.Cells["Result"].Appearance.Cursor = Cursors.Hand;
                            row.Cells["Result"].ToolTipText = "No rates found for this date";
                        }));
                    }

                    // temporary code to show warnings
                    Invoke(new MethodInvoker(
                    delegate
                        {
                            const string sql = @"
select * from ServiceWarning
where ServiceID in (	
	select distinct r.ServiceID
	from [Option] o left outer join [Rate] r on r.RateID = o.RateID
	where o.OptionID = @optionId
) 
AND ValidFrom <= convert(char(8),@date, 112) + ' 23:59' 
AND ValidTo >= convert(char(8),@date, 112) + ' 00:00';";

                        var warnings = DatabaseHelper.ExecuteDataset(sql,
                            new SqlParameter("@optionId", (int)row.Cells["OptionID"].Value),
                            new SqlParameter("@date", (DateTime)row.Cells["StartDate"].Value));
                       
                        if (warnings.Tables[0].Rows.Count != 0)
                        {                                                        
                            row.Cells["Warnings"].Appearance.Image = Resources.Warning;
                            row.Cells["Warnings"].Appearance.Cursor = Cursors.Hand;
                            row.Cells["Warnings"].ToolTipText = "Click to view warnings";
                            row.Cells["Warnings"].Tag = warnings.Tables[0].Rows;
                        }                                          
                    }));

                    // push row changes to the datasource
                    if (InvokeRequired)
                        Invoke(new MethodInvoker(delegate { row.Update(); }));
                    else
                        row.Update();


                    if (InvokeRequired)
                        Invoke(new MethodInvoker(delegate { row.Activate(); }));
                    else
                        row.Activate();
                }
            }

            if (InvokeRequired)
                Invoke(new MethodInvoker(UnlockControls));
            else
                UnlockControls();
        }

        /// <summary>
        /// Toggles the "IsSelected" column on all rows.
        /// </summary>
        private void SelectAll()
        {
            bool unselectAll = true;

            foreach (UltraGridRow row in gridBookings.Rows)
            {
                bool isRowLocked = row.Cells["IsLockedAccounting"].Value != DBNull.Value &&
                                   (bool)row.Cells["IsLockedAccounting"].Value;

                if (isRowLocked || (bool)row.Cells["IsSelected"].Value)
                    continue;

                row.Cells["IsSelected"].Value = true;
                unselectAll = false;
            }
            if (unselectAll)
            {
                // if all of the rows are already selected, then unselect all
                foreach (UltraGridRow row in gridBookings.Rows)
                {
                    row.Cells["IsSelected"].Value = false;
                }
            }
        }

        /// <summary>
        /// Locks the controls, so they cannot be used while the update thread is running.
        /// </summary>
        private void LockControls()
        {
            btnStartStop.Text = "Stop";
            btnOk.Enabled = false;
            btnCancel.Enabled = false;
            txtDayOffset.ReadOnly = true;
        }

        /// <summary>
        /// Unlocks the controls that were locked during the update process;
        /// </summary>
        private void UnlockControls()
        {
            btnStartStop.Text = _origButtonText;
            btnOk.Enabled = true;
            btnCancel.Enabled = true;
            txtDayOffset.ReadOnly = false;
            stopUpdateProcess = false;
        }

        private void UpdateOptionsOnThread()
        {
            LockControls();

            Thread thread = new Thread(UpdateOptions) {Name = "DateKicker_UpdateOptions"};
            thread.Start();
        }

        private void ChooseOption(UltraGridRow row)
        {
            int purchaseLineId = (int)row.Cells["PurchaseLineID"].Value;
            ItinerarySet.PurchaseLineRow purchaseLine =
                itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineId);

            OptionPicker optionPicker = new OptionPicker(
                purchaseLine.SupplierID, (int) row.Cells["OptionID"].Value, (DateTime) row.Cells["StartDate"].Value);

            if (optionPicker.ShowDialog() == DialogResult.OK)
            {
                if (optionPicker.SelectedOption == null)
                    return;

                var oldItem = purchaseItemTable.FindByPurchaseItemID((int)row.Cells["PurchaseItemID"].Value);
                var newItem = purchaseItemTable.ChangePurchaseItemOption(oldItem, optionPicker.SelectedService, optionPicker.SelectedOption, Global.Cache.ToolSet);

                foreach (UltraGridRow gridRow in gridBookings.Rows)
                {
                    if (newItem.PurchaseItemID == (int)gridRow.Cells["PurchaseItemID"].Value)
                    {
                        gridBookings.ActiveRow = row = gridRow;
                        break;
                    }
                }
                // reset notifications
                row.Cells["Result"].Value = null;
                row.Cells["Result"].ToolTipText = string.Empty;
                row.Cells["Result"].Appearance.Image = Resources.Tick;
            }
        }

        private void ShowWarnings(UltraGridRow row)
        {
            DataRowCollection dataRows = (DataRowCollection)row.Cells["Warnings"].Tag;
            var sb = new StringBuilder();
            int ctr = 0;
            foreach (DataRow dataRow in dataRows)
            {
                ++ctr;
                sb.AppendLine(string.Format("{0}. {1}", ctr.ToString(), dataRow["Description"].ToString()));
            }

            string warning = string.Format("Booking warning message:\r\n\r\n{0}\r\n\r\n", sb.ToString().TrimEnd());
            MessageBox.Show(warning, "BookingEditor Warning Message");
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text == _origButtonText)
            {
                // this is the start button
                UpdateOptionsOnThread();
            }
            else if (btnStartStop.Text == "Stop")
            {
                // this is the stop button
                stopUpdateProcess = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidateRows()) return;
            
            if (dtpNewStartDate.Value != null)
                itinerarySet.Itinerary[0].ArriveDate = (DateTime) dtpNewStartDate.Value;
         
            if (dtpNewEndDate.Value != null)
                itinerarySet.Itinerary[0].DepartDate = (DateTime) dtpNewEndDate.Value;
         
            //itinerarySet.Itinerary[0].AcceptChanges();

            // booking dates
            itinerarySet.PurchaseItem.Merge(purchaseItemTable, false);
            DialogResult = DialogResult.OK;
        }

        private bool ValidateRows()
        {
            var i = 0;
            foreach (var row in gridBookings.Rows)
            {
                i++;
                if (!(bool)row.Cells["IsSelected"].Value) continue;
                
                // warn if setting row to null or zero
                if (row.Cells["OldNet"].Value != DBNull.Value   // not already null
                    && (row.Cells["Net"].Value == DBNull.Value   // and setting null
                        || ((decimal)row.Cells["Net"].Value == 0 && (decimal)row.Cells["OldNet"].Value != 0))) // or changing it to zero
                {
                    var msg = string.Format("Warning: row {0} sets new price to {1}. You can exclude this row by unticking it.\r\n\r\nContinue anyway?", i, row.Cells["Net"].Value == DBNull.Value ? "NOTHING" : "ZERO");
                    var stop = MessageBox.Show(msg, App.MessageCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel;
                    if (stop) return false;
                }
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // reset arrive date
            if (!itinerarySet.Itinerary[0].IsArriveDateNull() && dtpOldStartDate.Value != null && 
                itinerarySet.Itinerary[0].ArriveDate != (DateTime)dtpOldStartDate.Value)
                itinerarySet.Itinerary[0].ArriveDate = (DateTime)dtpOldStartDate.Value;

            // reset depart date
            if (!itinerarySet.Itinerary[0].IsDepartDateNull() && dtpOldEndDate.Value != null &&
                itinerarySet.Itinerary[0].DepartDate != (DateTime)dtpOldEndDate.Value)
                itinerarySet.Itinerary[0].DepartDate = (DateTime)dtpOldEndDate.Value;
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("IsSelected"))
                e.Layout.Bands[0].Columns.Add("IsSelected");
            if (!e.Layout.Bands[0].Columns.Exists("Result"))
                e.Layout.Bands[0].Columns.Add("Result");
            if (!e.Layout.Bands[0].Columns.Exists("PurchaseLineName"))
                e.Layout.Bands[0].Columns.Add("PurchaseLineName");
            if (!e.Layout.Bands[0].Columns.Exists("OldDate"))
                e.Layout.Bands[0].Columns.Add("OldDate");
            if (!e.Layout.Bands[0].Columns.Exists("OldNet"))
                e.Layout.Bands[0].Columns.Add("OldNet");
            if (!e.Layout.Bands[0].Columns.Exists("Warnings"))
                e.Layout.Bands[0].Columns.Add("Warnings");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "IsSelected")
                {
                    c.Header.Caption = String.Empty;
                    c.DataType = typeof (bool);
                    c.CellClickAction = CellClickAction.Edit;
                    c.Header.Appearance.Image = Resources.CheckBox;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.Width = 30;
                    c.MaxWidth = 30;
                    c.SortIndicator = SortIndicator.Disabled;
                }
                else if (c.Key == "Result")
                {
                    c.Width = 80;
                    c.MaxWidth = 80;
                    c.CellActivation = Activation.ActivateOnly;
                    c.CellAppearance.ForeColor = Color.Blue;
                    c.CellAppearance.FontData.Underline = DefaultableBoolean.True;                    
                    c.CellAppearance.ImageHAlign = HAlign.Left;
                    c.CellAppearance.ImageVAlign = VAlign.Middle;
                }
                else if (c.Key == "PurchaseLineName")
                {
                    c.Width = 80;
                    c.Header.Caption = "Booking name";
                }
                else if (c.Key == "PurchaseItemName")
                {
                    c.Width = 120;
                    c.Header.Caption = "Item description";
                }
                else if (c.Key == "OldDate")
                {
                    c.Width = 80;
                    c.Header.Caption = "Old date";
                    c.DataType = typeof(DateTime);
                    c.Style = ColumnStyle.Date;
                    c.Format = App.GetLocalShortDateFormat();
                    c.EditorControl = new UltraTextEditor();
                    c.Band.SortedColumns.Add(c, false);
                }
                else if (c.Key == "StartDate")
                {
                    c.Width = 80;
                    c.Header.Caption = "New date";
                    c.Style = ColumnStyle.Date;
                    c.Format = App.GetLocalShortDateFormat();
                    //c.EditorControl = new UltraTextEditor();
                    
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "NumberOfDays")
                {
                    c.Header.Caption = "Days/Nts";
                }
                else if (c.Key == "OldNet")
                {
                    c.Width = 80;
                    c.Format = "c";
                    c.Header.Caption = "Old net";
                    c.DataType = typeof(Decimal);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "Net")
                {
                    c.Width = 80;
                    c.Format = "c";
                    c.Header.Caption = "New net";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "Warnings")
                {
                    c.Width = 60;
                    c.MaxWidth = 60;
                    c.MinWidth = 60;                    
                    c.CellAppearance.ImageHAlign = HAlign.Center;
                    c.CellAppearance.ImageVAlign = VAlign.Middle;
                }
                else
                {
                    c.Hidden = true;
                }
            }

            // set the column order
            int index = 0;
            e.Layout.Bands[0].Columns["IsSelected"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseLineName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["OldDate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["StartDate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NumberOfDays"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["OldNet"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Net"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Result"].Header.VisiblePosition = index;

            // set defaults
            GridHelper.SetDefaultGridAppearance(e);

            // override defaults
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void grid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.ReInitialize)
                return;

            int purchaseLineId = (int)e.Row.Cells["PurchaseLineID"].Value;
            ItinerarySet.PurchaseLineRow purchaseLine = 
                itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineId);
            bool isRowLocked = e.Row.Cells["IsLockedAccounting"].Value != DBNull.Value &&
                               (bool) e.Row.Cells["IsLockedAccounting"].Value;
            
            e.Row.Cells["IsSelected"].Value = !isRowLocked;
            e.Row.Cells["Result"].ToolTipText = string.Empty;
            e.Row.Cells["PurchaseLineName"].Value = purchaseLine.PurchaseLineName;
            e.Row.Cells["OldDate"].Value = e.Row.Cells["StartDate"].Value;
            e.Row.Cells["OldNet"].Value = e.Row.Cells["Net"].Value;
            e.Row.Activation = (isRowLocked) ? Activation.Disabled : Activation.AllowEdit;
        }

        private void gridBookings_MouseUp(object sender, MouseEventArgs e)
        {
            UIElement clickedElement =
                gridBookings.DisplayLayout.UIElement.ElementFromPoint(gridBookings.PointToClient(MousePosition));

            if (clickedElement == null)
                return;

            // if a cell is clicked, check if it is the "Choose..." link cell
            CellUIElement cellElement = (CellUIElement)clickedElement.GetAncestor(typeof (CellUIElement));
            if (cellElement != null)
            {
                UltraGridCell cell = (UltraGridCell)cellElement.GetContext(typeof (UltraGridCell));

                if (cell == null)
                    return;

                if (cell.Column.Key == "Result" && !String.IsNullOrEmpty((string)cell.Value))
                {
                    ChooseOption(cell.Row);
                }
                else if (cell.Column.Key == "Warnings" && cell.Tag != null)
                {
                    ShowWarnings(cell.Row);
                }
            }

            // if a header is clicked, check if it is the "IsSelected" column
            HeaderUIElement headerElement = (HeaderUIElement)clickedElement.GetAncestor(typeof (HeaderUIElement));
            if (headerElement != null)
            {
                UltraGridColumn column = (UltraGridColumn)headerElement.GetContext(typeof (UltraGridColumn));
                if (column.Key == "IsSelected")
                {
                    SelectAll();
                }
            }
        }        

        private void dtpNewStartDate_ValueChanged(object sender, EventArgs e)
        {
            if (chkUpdateStartDate.Checked)
            {
                var offset = ((DateTime)dtpNewStartDate.Value).Subtract((DateTime)dtpOldStartDate.Value).Days;
                txtDayOffset.Value = offset;
                SetNewBookingDates(offset);
                //UpdateOptionsOnThread();
            }
        }
    }
}
