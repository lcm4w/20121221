using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Modules.SupplierModule;
using TourWriter.Properties;
using TourWriter.Services;
using ButtonDisplayStyle=Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;
using System.Linq;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class BookingSelectorForm : Form
    {
        public event OnPurchaseItemAddedHandler OnPurchaseItemAdded;

        private SupplierSet supplierSet;
        private readonly ItinerarySet itinerarySet;
        private readonly ItinerarySet tempItinerarySet;
        private ItinerarySet.PurchaseLineRow purchaseLine;

        public enum PageType { Search, Select }
        
        public BookingSelectorForm(ItinerarySet itinerarySet)
        {
            InitializeComponent();

            Size = Settings.Default.BookingSelectorSize;
            Location = Settings.Default.BookingSelectorLocation;
            panel1.Height = Settings.Default.BookingSelectorGridHeight;

            this.itinerarySet = itinerarySet;
            tempItinerarySet = (ItinerarySet)itinerarySet.Copy();
            gridBookings.DataSource = tempItinerarySet.PurchaseItem;

            SetPage(PageType.Search);
        }

        private void BookingSelectorForm_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.TourWriter16;
            Text = Text.Trim() + ": " + itinerarySet.Itinerary[0].ItineraryName;

            AcceptButton = btnNext;
            CancelButton = btnCancel;
            btnOk.Enabled = gridBookings.Rows.Count > 0;
            chkIncrementDates.Checked = true;
        }

        private DateTime GetNextDefaultDate()
        {
            // get from grid rows
            if (gridBookings.Rows.Count > 0 && gridBookings.Rows[gridBookings.Rows.Count - 1].Cells["StartDate"].Value != DBNull.Value)
            {
                var date = (DateTime)gridBookings.Rows[gridBookings.Rows.Count - 1].Cells["StartDate"].Value;
                if (chkIncrementDates.Checked)
                {
                    var delta = 1;
                    if (gridBookings.Rows[gridBookings.Rows.Count - 1].Cells["NumberOfDays"].Value != DBNull.Value)
                        delta = Convert.ToInt32(gridBookings.Rows[gridBookings.Rows.Count - 1].Cells["NumberOfDays"].Value);
                    return date.AddDays(delta);
                }
                return date;
            }
            // no rows yet, get from underlying itinerary
            var items = itinerarySet.PurchaseItem.Where(i => i.RowState != DataRowState.Deleted && !i.IsStartDateNull());
            if (items.Count() > 0)
            {
                var date = items.Max(i => i.StartDate);
                if (chkIncrementDates.Checked)
                {
                    var delta = items.Where(i => i.StartDate == date).Max(i => i.NumberOfDays);
                    return date.AddDays(delta);
                }
                return date;
            }
            return itinerarySet.Itinerary[0].ArriveDate.Date;
        }

        public void LoadSupplier(int supplierId, int? serviceId, int? optionId)
        {
            // Load supplier if not already loaded.
            if (serviceEditor1.SupplierSet == null ||
               serviceEditor1.SupplierSet.Supplier[0].SupplierID != supplierId)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    Application.DoEvents();
                    supplierSet = new BusinessLogic.Supplier().GetSupplierSet(supplierId);
                    serviceEditor1.SupplierSet = supplierSet; 
                    tempItinerarySet.ItineraryPaxOverride.Clear();
                    tempItinerarySet.PurchaseItem.Clear();
                }
                catch (Exception ex)
                {
                    if (ErrorHelper.IsServerConnectionError(ex))
                        App.ShowServerConnectionError();

                    else throw;
                }
                finally
                {
                    Application.DoEvents();
                    Cursor = Cursors.Default;
                }
            }
            // Set selected service.
            if (serviceId.HasValue)
                serviceEditor1.SetSelectedServiceRow((int)serviceId);

            // Set selected rate.
            serviceEditor1.SetSelectedRateRow(GetNextDefaultDate());

            // Set selected option.
            if (optionId.HasValue)
                serviceEditor1.SetSelectedOptionRow((int)optionId);
        }

        public void SetPage(PageType defaultPage)
        {
            if (defaultPage == PageType.Search)
            {
                tabControl.SelectedTab = tabControl.Tabs["Search"];
            }
            else if (defaultPage == PageType.Select)
            {
                tabControl.SelectedTab = tabControl.Tabs["Select"];
            }
        }

        private void ResetInvalidSeasonDatesWarning()
        {
            // Show/hide warning label.
            errorProvider1.SetError(
                serviceEditor1.lblRates, // lblDate, 
                ((IsSelectedRateExpired)
                     ? "Selected rate dates do not match the booking date"
                     : ""));

        }

        private bool IsSelectedRateExpired
        {
            get
            {
                int? rateId = serviceEditor1.GetSelectedRateId();
                if (!rateId.HasValue)
                    return false;
                
                SupplierSet.RateRow rate = supplierSet.Rate.FindByRateID((int)rateId);
                DateTime itemDate = GetNextDefaultDate();
                return !(itemDate.Date >= rate.ValidFrom.Date && itemDate.Date <= rate.ValidTo.Date);
            }
        }

        private ItinerarySet.PurchaseLineRow GetOrCreatePurchaseLine(int? purchaseLineId)
        {
            ItinerarySet.PurchaseLineRow line = null;

            // Get existing purchase line.
            if (purchaseLineId.HasValue)
                line = itinerarySet.PurchaseLine.FindByPurchaseLineID((int)purchaseLineId);

            // Create new purchase line.
            if (line == null)
                line = tempItinerarySet.PurchaseLine.Add(
                    itinerarySet.Itinerary[0].ItineraryID,
                    supplierSet.Supplier[0].SupplierID,
                    supplierSet.Supplier[0].SupplierName, Cache.User.UserID);

            return line;
        }

        private void UpdatePurchaseLine()
        {
            var oldPurchaseLine = purchaseLine;

            purchaseLine = GetOrCreatePurchaseLine((int?)cmbBookings.Value);

            foreach (ItinerarySet.PurchaseItemRow item in tempItinerarySet.PurchaseItem)
                if (item.RowState != DataRowState.Deleted)
                    item.PurchaseLineID = purchaseLine.PurchaseLineID;

            if (oldPurchaseLine != null)
            {
                // delete the old purchase line if it's empty, and unsaved
                int itemCount = oldPurchaseLine.GetPurchaseItemRows().Length;
                if (itemCount == 0 && oldPurchaseLine.PurchaseLineID < 0)
                    oldPurchaseLine.Delete();
            }

        }

        private ItinerarySet.PurchaseItemRow AddNewPurchaseItem(int optionId)
        {
            // additional data.
            SupplierSet.OptionRow option = supplierSet.Option.FindByOptionID(optionId);
            SupplierSet.ServiceRow service = option.RateRow.ServiceRow;
            ToolSet.ServiceTypeRow stype = Cache.ToolSet.ServiceType.FindByServiceTypeID(service.ServiceTypeID);
            
            // add item.
            var newItem = tempItinerarySet.PurchaseItem.Add(
                supplierSet,
                purchaseLine.PurchaseLineID,
                service.ServiceName + ((option.OptionName != "") ? (", " + option.OptionName) : ""),
                null,
                null,
                option.Net,
                option.Gross,
                GetNextDefaultDate(),
                null,
                null,
                Cache.User.UserID,
                service.ServiceTypeID,
                option.OptionID,
                option.RateID,
                service.ServiceID,
                stype.ServiceTypeName,
                !option.IsOptionTypeIDNull() ? (int?) option.OptionTypeID : null,
                service.ServiceName,
                option.OptionName,
                !option.IsOptionTypeIDNull() ? Cache.ToolSet.OptionType.FindByOptionTypeID(option.OptionTypeID).OptionTypeName : "",
                !service.IsChargeTypeNull() ? service.ChargeType : "", 
                option.IsDefault,
                service.CurrencyCode);

            btnOk.Enabled = true;
            return newItem;
        }

        private void RemovePurchaseItem(int optionId)
        {
            for (var i = 0; i < tempItinerarySet.PurchaseItem.Count; i++)
            {
                if (tempItinerarySet.PurchaseItem[i].OptionID == optionId)
                    tempItinerarySet.PurchaseItem[i].Delete();
            }
            btnOk.Enabled = gridBookings.Rows.Count > 0;
        }

        private void PopulateBookingsList(int supplierId)
        {
            // Get existing bookings for this supplier.
            var lines = from line in itinerarySet.PurchaseLine
                        where (line.RowState != DataRowState.Deleted && line.SupplierID == supplierId)
                        select line;

            // Add supplier bookings.
            cmbBookings.Items.Clear();
            cmbBookings.Items.Add(null, "Create new booking (new voucher)");
            foreach (ItinerarySet.PurchaseLineRow line in lines)
            {
                cmbBookings.Items.Add(
                    line.PurchaseLineID,
                    String.Format("({0}) {1}", line.PurchaseLineID, line.PurchaseLineName));
            }
            // Select the last booking.
            cmbBookings.SelectedIndex = cmbBookings.Items.Count - 1;

            // Warn if multiple booking choices.
            cmbBookings.Enabled = (cmbBookings.Items.Count > 1);
        }

        /// <summary>
        /// Climbs up the chain to find what the current payment terms are.
        /// </summary>
        /// <param name="serviceRow">ServiceRow is needed to check for overrides.</param>
        /// <returns>The relevant payment term row, or null if there is none.</returns>
        private DataRow GetRelevantPaymentTermRow(SupplierSet.ServiceRow serviceRow)
        {
            // check for service terms
            if (!serviceRow.IsPaymentTermIDNull())
            {
                int id = serviceRow.PaymentTermID;
                DataRow row = supplierSet.PaymentTerm.FindByPaymentTermID(id);
                return row;
            }

            // check for supplier terms
            if (!supplierSet.Supplier[0].IsPaymentTermIDNull())
            {
                int id = supplierSet.Supplier[0].PaymentTermID;
                DataRow row = supplierSet.PaymentTerm.FindByPaymentTermID(id);
                return row;
            }

            // check for agent terms
            if (!itinerarySet.Itinerary[0].IsAgentIDNull())
            {
                int agentId = itinerarySet.Itinerary[0].AgentID;

                AgentSet.AgentRow agentRow = Cache.AgentSet.Agent.FindByAgentID(agentId);
                if (agentRow != null && !agentRow.IsPurchasePaymentTermIDNull())
                {
                    int id = agentRow.PurchasePaymentTermID;
                    DataRow row = Cache.AgentSet.PaymentTerm.FindByPaymentTermID(id);
                    return row;
                }
            }

            return null;
        }

        /// <summary>
        /// Makes a copy of the relevant PaymentTermRow.
        /// </summary>
        /// <param name="serviceRow">ServiceRow is needed to check for overrides.</param>
        /// <returns>The new PaymentTermRow.</returns>
        private ItinerarySet.PaymentTermRow CopyRelevantPaymentTermRow(SupplierSet.ServiceRow serviceRow)
        {
            DataRow row = GetRelevantPaymentTermRow(serviceRow);
            if (row == null)
                return null;

            ItinerarySet.PaymentTermRow newRow = tempItinerarySet.PaymentTerm.NewPaymentTermRow();
            int newId = newRow.PaymentTermID;
            newRow.ItemArray = row.ItemArray;
            newRow.PaymentTermID = newId;

            tempItinerarySet.PaymentTerm.AddPaymentTermRow(newRow);
            return newRow;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (tempItinerarySet.PurchaseItem.Count == 0)
            {
                App.ShowError("Please select at least one service option.");
                DialogResult = DialogResult.None;
                return;
            }

            foreach (ItinerarySet.PurchaseItemRow item in tempItinerarySet.PurchaseItem)
            {
                var option = supplierSet.Option.FindByOptionID(item.OptionID);
                var service = option.RateRow.ServiceRow;

                // Add items payment terms and link to item
                ItinerarySet.PaymentTermRow paymentTermRow = CopyRelevantPaymentTermRow(service);
                if (paymentTermRow != null)
                    item.PaymentTermID = paymentTermRow.PaymentTermID;

                // Notify listeners.
                if (OnPurchaseItemAdded != null)
                    OnPurchaseItemAdded(
                        new PurchaseItemAddedEventArgs(item.PurchaseLineID, item.PurchaseItemID));
            }

            try
            {
                // possible merge conflicts with ItineraryMarginOverride table due to no idendtity seed on PK
                tempItinerarySet.ItineraryMarginOverride.Clear();

                itinerarySet.Merge(tempItinerarySet);
            }
            catch (ConstraintException ex)
            {
                var stream1 = new System.IO.MemoryStream();
                itinerarySet.WriteXml(stream1, XmlWriteMode.DiffGram);
                stream1.Position = 0;
                var attach1 = new System.Net.Mail.Attachment(stream1, "ds1.xml", System.Net.Mime.MediaTypeNames.Text.Xml);

                var stream2 = new System.IO.MemoryStream();
                tempItinerarySet.WriteXml(stream2, XmlWriteMode.DiffGram);
                stream2.Position = 0;
                var attach2 = new System.Net.Mail.Attachment(stream2, "ds2.xml", System.Net.Mime.MediaTypeNames.Text.Xml);

                ErrorHelper.SendEmail(ex, true, attach1, attach2);
                throw;
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab.Key == "Select")
            {
                SetPage(PageType.Search);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab.Key == "Search")
            {
                SupplierSearchEventArgs result = supplierSearch1.GetSearchResults();
                if (result != null)
                    LoadSupplier(result.supplierId, result.serviceId, null);

                SetPage(PageType.Select);
            }
        }

        private void tabControl_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            btnBack.Enabled = e.Tab.Key != "Search";
            btnNext.Enabled = e.Tab.Key != "Select";

            switch (e.Tab.Key)
            {
                case "Search":
                    AcceptButton = btnNext;
                    btnOk.Visible = false;
                    break;
                case "Select":
                    AcceptButton = btnNext;
                    btnOk.Visible = true;
                    break;
            }
        }

        private void supplierSearch1_OnSupplierPicked(SupplierSearchEventArgs e)
        {
            LoadSupplier(e.supplierId, e.serviceId, null);
            SetPage(PageType.Select);
        }

        private void serviceEditor1_OnSupplierSelected(SupplierSelectedEventArgs e)
        {
            // Set supplier name
            txtSupplierName.Text = supplierSet.Supplier[0].SupplierName;

            // Populate existing bookings list.
            PopulateBookingsList(supplierSet.Supplier[0].SupplierID);
        }

        private void serviceEditor1_OnRateSelected(RateSelectedEventArgs e)
        {
            ResetInvalidSeasonDatesWarning();
        }

        private void serviceEditor1_OnOptionSelected(OptionSelectedEventArgs e)
        {
            // Reset option warning.
            errorProvider1.SetError(serviceEditor1.lblOptions, "");
        }

        private void serviceEditor1_OnOptionCheckedChanged(OptionCheckedChangedEventArgs e)
        {
            if (!e.IsChecked) // unchecking so remove
            {
                RemovePurchaseItem(e.OptionID);
                return;
            }
            
            // show any warnings
            var warning = serviceEditor1.GetSelectedServiceWarningMessage();
            if (warning != string.Empty)
            {
                warning = string.Format("Booking warning message:\r\n\r\n{0}\r\n\r\nContinue?", warning);
                if (MessageBox.Show(warning, "BookingEditor Warning Message", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                {
                    serviceEditor1.SetSelectedOnActiveOptionRow(false);
                    return;
                }
            }

            // add purchase item
            var id = AddNewPurchaseItem(e.OptionID).PurchaseItemID;
            UltraGridRow newRow = null;
            foreach (var row in gridBookings.Rows)
                if ((int)row.Cells["PurchaseItemID"].Value == id)
                {
                    newRow = row;
                    newRow.Selected = true;
                    newRow.Activate();
                    break;
                }
            if (newRow == null) return;

            // populate servicetime cell dropdown
            ValueList list;
            var cell = newRow.Cells["ServiceTime"];
            var key = "ServiceTimes" + e.OptionID;
            if (gridBookings.DisplayLayout.ValueLists.Exists(key))
            {   // add existing
                list = gridBookings.DisplayLayout.ValueLists[key];
            }
            else
            {   // create list
                list = gridBookings.DisplayLayout.ValueLists.Add(key);
                list.ValueListItems.Add(DBNull.Value, "(none)");

                var times = supplierSet.Option.FindByOptionID(e.OptionID).RateRow.ServiceRow.
                    GetServiceTimeRows().Where(t => t.RowState != DataRowState.Deleted && !t.IsStartTimeNull()).OrderBy(t => t.StartTime);

                foreach (var time in times)
                {
                    var text =
                        time.StartTime.ToShortTimeString() +
                        (!time.IsEndTimeNull() ? " (out: " + time.EndTime.ToShortTimeString() + ")" : "") +
                        (!time.IsCommentNull() && !string.IsNullOrEmpty(time.Comment) ? ", " + time.Comment : "");
                    list.ValueListItems.Add(time.StartTime, text);
                }
            }
            if (list.ValueListItems.Count <= 1) return;
            cell.Style = ColumnStyle.DropDownList; // convert cell to drop-down
            cell.ValueList = list;

            cell.Value = list.ValueListItems[1].DataValue;
            cell.Row.Update();
        }
        
        private void btnOptionDel_Click(object sender, EventArgs e)
        {
            if (gridBookings.ActiveRow != null)
            {
                var optionId = (int)gridBookings.ActiveRow.Cells["OptionID"].Value;
                GridHelper.DeleteActiveRow(gridBookings, true);
                var option = supplierSet.Option.FindByOptionID(optionId);
                option.IsSelected = false;
            }
        }

        private void BookingSelectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.BookingSelectorSize =
                WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;
            Settings.Default.BookingSelectorLocation =
                WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;
            Settings.Default.BookingSelectorGridHeight = panel1.Height;

            Settings.Default.Save();
            App.ClearBindings(this);
        }

        private void gridBookings_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("ServiceTime"))
                e.Layout.Bands[0].Columns.Add("ServiceTime");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "PurchaseItemName")
                {
                    c.Header.Caption = "Item description";
                    c.Header.ToolTipText = "Description of the booking item";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "StartDate")
                {
                    c.Width = 100;
                    c.Header.Caption = "Date";
                    c.Header.ToolTipText = "Item booking date";
                    c.Style = ColumnStyle.Date;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "ServiceTime")
                {
                    c.Width = 100;
                    c.MaskInput = "{time}";
                    c.DataType = typeof(DateTime);
                    c.Header.Caption = "Time";
                    c.Header.ToolTipText = "Booking time";
                    c.Style = ColumnStyle.TimeWithSpin;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                    c.SortComparer = new TimeSortComparer();
                }
                else if (c.Key == "NumberOfDays")
                {
                    c.Width = 60;
                    c.Header.Caption = "Nts/days";
                    c.Header.ToolTipText = "Number of days/nights required";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.TabStop = true;
                }
                else if (c.Key == "Quantity")
                {
                    c.Width = 60;
                    c.Header.Caption = "Qty";
                    c.Header.ToolTipText = "Quantity required";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else
                {
                    c.Hidden = true;
                    c.TabStop = false;
                }
            }
            int pos = 0;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Header.VisiblePosition = pos++;
            e.Layout.Bands[0].Columns["StartDate"].Header.VisiblePosition = pos++;
            e.Layout.Bands[0].Columns["ServiceTime"].Header.VisiblePosition = pos++;
            e.Layout.Bands[0].Columns["NumberOfDays"].Header.VisiblePosition = pos++;
            e.Layout.Bands[0].Columns["Quantity"].Header.VisiblePosition = pos;

            // Set defaults
            GridHelper.SetDefaultGridAppearance(e);
        }

        private void cmbBookings_ValueChanged(object sender, EventArgs e)
        {
            UpdatePurchaseLine();
        }

        private void gridBookings_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "ServiceTime") 
            {
                DateTime d;
                if (e.Cell.Value != null && DateTime.TryParse(e.Cell.Value.ToString(), out d))
                    e.Cell.Row.Update(); // fire immediate edit
            }
        }

        private void gridBookings_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "ServiceTime")
            {
                switch (e.Cell.Style)
                {
                    case ColumnStyle.Default:
                        e.Cell.Row.Cells["StartTime"].Value = e.Cell.Value;
                        break;
                    case ColumnStyle.DropDownList:
                        {
                            DateTime time;
                            DateTime? start = null;
                            DateTime? end = null;

                            if (e.Cell.Value != null && DateTime.TryParse(e.Cell.Value.ToString(), out time))
                            {
                                // find service time record that matches selected start time value
                                var id = (int) e.Cell.Row.Cells["OptionID"].Value;
                                var row = supplierSet.Option.FindByOptionID(id).RateRow.ServiceRow.
                                    GetServiceTimeRows().Where(t => t.StartTime == time).First();

                                if (row != null)
                                {
                                    if (!row.IsStartTimeNull()) start = row.StartTime;
                                    if (!row.IsEndTimeNull()) end = row.EndTime;
                                }
                            }
                            e.Cell.Row.Cells["StartTime"].Value = start.HasValue ? (object) start : DBNull.Value;
                            e.Cell.Row.Cells["EndTime"].Value = end.HasValue ? (object) end : DBNull.Value;
                        }
                        break;
                }
            }
        }

        private void gridBookings_CellDataError(object sender, CellDataErrorEventArgs e)
        {
            e.RaiseErrorEvent = true;
            e.RestoreOriginalValue = true;
        }
    }

    public delegate void OnPurchaseItemAddedHandler(PurchaseItemAddedEventArgs e);
    public class PurchaseItemAddedEventArgs
    {
        public int PurchaseLineId;
        public int PurchaseItemId;

        public PurchaseItemAddedEventArgs(int purchaseLineId, int purchaseItemId)
        {
            PurchaseLineId = purchaseLineId;
            PurchaseItemId = purchaseItemId;
        }
    }
}
