using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.BusinessLogic;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Modules.ContactModule;
using TourWriter.Services;
using ButtonDisplayStyle=Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace TourWriter.Modules.ItineraryModule
{
    /// <summary>
    /// Summary description for MembersEditor.
    /// </summary>
    public partial class ClientEditor : UserControl
    {
        private ItinerarySet itinerarySet = null;

        internal ItinerarySet ItinerarySet
        {
            get { return itinerarySet; }
            set
            {
                itinerarySet = value;
                DataBind();
            }
        }
        
        public ClientEditor()
        {
            InitializeComponent();

            // TODO: remove old currency stuff (was here just to user could store client currency)
            label4.Visible = false;
            cmbCurrency.Visible = false;
            label5.Visible = false;
            txtCurrencyRate.Visible = false;
            btnUpdateRates.Visible = false;
        }
        
        // TODO: hidden group row required before can add clients/client notes etc
        private void AddFirstItineraryGroup()
        {
            if (itinerarySet.ItineraryGroup.Count == 0)
            {
                var row = itinerarySet.ItineraryGroup.NewItineraryGroupRow();
                row.ItineraryID = itinerarySet.Itinerary[0].ItineraryID;
                row.ItineraryGroupName = itinerarySet.Itinerary[0].ItineraryName + " Group";
                itinerarySet.ItineraryGroup.AddItineraryGroupRow(row);
            }
        }

        public bool EnableGroups
        {
            get { return panelPax.Enabled; }
            set { panelPax.Enabled = value; }
        }

        private void DataBind()
        {
            // Pax
            txtPaxOverride.DataBindings.Add("Value", itinerarySet.Itinerary, "PaxOverride");
            gridPax.DataSource = itinerarySet.ItineraryPax;

            // Members
            
            gridMembers.DisplayLayout.ValueLists.Add("AgeGroupsList");
            gridMembers.DisplayLayout.ValueLists["AgeGroupsList"].SortStyle = ValueListSortStyle.Ascending;
            gridMembers.DisplayLayout.ValueLists["AgeGroupsList"].ValueListItems.Add(DBNull.Value, "(none)");
            foreach (ToolSet.AgeGroupRow r in Cache.ToolSet.AgeGroup.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                    continue;

                gridMembers.DisplayLayout.ValueLists["AgeGroupsList"].ValueListItems.Add(r.AgeGroupID, r.AgeGroupName);
            }

            gridMembers.SetDataBinding(itinerarySet, "Itinerary.ItineraryItineraryGroup.ItineraryGroupGroupMember");
            
            // Notes
            
            txtComments.DataBindings.Add("Text", itinerarySet, "ItineraryGroup.Comments");
            txtNoteToClient.DataBindings.Add("Text", itinerarySet, "ItineraryGroup.NoteToClient");
            txtNoteToSupplier.DataBindings.Add("Text", itinerarySet, "ItineraryGroup.NoteToSupplier");

            currencyBindingSource.DataSource = Cache.ToolSet.Currency.Where(c => c.Enabled).ToList();
            itineraryGroupBindingSource.DataSource = itinerarySet.ItineraryGroup;
            if (itinerarySet.ItineraryGroup.Rows.Count > 0 && itinerarySet.ItineraryGroup[0].IsCurrencyCodeNull())
                cmbCurrency.SelectedValue = DBNull.Value;

            RefreshPaymentTypesList();
            RefreshMembersList();
            RefreshSalesList();

            gridPayments.SetDataBinding(itinerarySet, "ItineraryPayment");
        }

        private static string GetSaleDisplayString(ItinerarySet.ItinerarySaleRow saleRow)
        {
            return String.Format("{0}; {1:c}; {2}",
                saleRow.ItinerarySaleID, saleRow.Amount, (!saleRow.IsCommentsNull() ? saleRow.Comments : String.Empty));
        }

        internal void AddExistingContact(int contactID)
        {
            // Ensure contact not already in list
            ItinerarySet.ItineraryMemberRow[] rows2 = (ItinerarySet.ItineraryMemberRow[])
                                                         itinerarySet.ItineraryMember.Select("ContactID = " + contactID);

            var rows = from member in itinerarySet.ItineraryMember
                       where member.ContactID == contactID
                       select member;

            if (rows.Count() > 0)
            {
                MessageBox.Show(App.GetResourceString("ShowRowAlreadyExists"));
                return;
            }

            // Import actual contact to handle dataset relation constraints
            if (itinerarySet.Contact.FindByContactID(contactID) == null)
            {
                Contact c = new Contact();
                ContactSet.ContactRow contact = c.GetContactSet(contactID).Contact[0];
                itinerarySet.Contact.ImportRow(contact);
            }

            // Initialise a new groupmember row
            ItinerarySet.ItineraryMemberRow r = itinerarySet.ItineraryMember.NewItineraryMemberRow();
            r.ItineraryGroupID = GetDefaultItinerayGroup().ItineraryGroupID;
            r.ItineraryMemberName = itinerarySet.Contact.FindByContactID(contactID).ContactName;
            r.AddedOn = DateTime.Now;
            r.AddedBy = TourWriter.Global.Cache.User.UserID;
            bool isFirstRow = (itinerarySet.ItineraryMember.Rows.Count == 0);
            r.IsDefaultContact = isFirstRow;
            r.IsDefaultBilling = isFirstRow;
            r.ContactID = contactID;
            itinerarySet.ItineraryMember.AddItineraryMemberRow(r);

            // select the new row
            gridMembers.ActiveRow = gridMembers.Rows[gridMembers.Rows.Count - 1];
        }

        private ItinerarySet.ItineraryGroupRow GetDefaultItinerayGroup()
        {
            if (itinerarySet.ItineraryGroup.Rows.Count == 0)
            {
                // Add new row
                ItinerarySet.ItineraryGroupRow r = itinerarySet.ItineraryGroup.NewItineraryGroupRow();
                r.ItineraryID = itinerarySet.Itinerary[0].ItineraryID;
                r.ItineraryGroupName = itinerarySet.Itinerary[0].ItineraryName;
                r.AddedBy = TourWriter.Global.Cache.User.UserID;
                r.AddedOn = DateTime.Now;
                itinerarySet.ItineraryGroup.AddItineraryGroupRow(r);
            }
            
            // Return the default itinerary group.
            return itinerarySet.ItineraryGroup[0];
        }

        private void RefreshPaymentTypesList()
        {
            if (!gridPayments.DisplayLayout.ValueLists.Exists("PaymentTypeList"))
            {
                gridPayments.DisplayLayout.ValueLists.Add("PaymentTypeList");
                gridPayments.DisplayLayout.ValueLists["PaymentTypeList"].SortStyle = ValueListSortStyle.Ascending;
            }

            gridPayments.DisplayLayout.ValueLists["PaymentTypeList"].ValueListItems.Clear();
            foreach (ToolSet.PaymentTypeRow r in Cache.ToolSet.PaymentType)
            {
                if (r.RowState == DataRowState.Deleted)
                    continue;

                gridPayments.DisplayLayout.ValueLists["PaymentTypeList"].ValueListItems.Add(r.PaymentTypeID, r.PaymentTypeName);
            }
        }

        private void RefreshMembersList()
        {
            if (!gridPayments.DisplayLayout.ValueLists.Exists("MembersList"))
            {
                gridPayments.DisplayLayout.ValueLists.Add("MembersList");
                gridPayments.DisplayLayout.ValueLists["MembersList"].SortStyle = ValueListSortStyle.Ascending;
            }

            gridPayments.DisplayLayout.ValueLists["MembersList"].ValueListItems.Clear();
            foreach (ItinerarySet.ItineraryMemberRow r in itinerarySet.ItineraryMember)
            {
                if (r.RowState == DataRowState.Deleted)
                    continue;

                gridPayments.DisplayLayout.ValueLists["MembersList"].ValueListItems.Add(r.ItineraryMemberID, r.ItineraryMemberName);
            }
        }

        private void RefreshSalesList()
        {
            if (!gridPayments.DisplayLayout.ValueLists.Exists("SalesList"))
            {
                gridPayments.DisplayLayout.ValueLists.Add("SalesList");
                gridPayments.DisplayLayout.ValueLists["SalesList"].SortStyle = ValueListSortStyle.Ascending;
            }

            gridPayments.DisplayLayout.ValueLists["SalesList"].ValueListItems.Clear();
            foreach (ItinerarySet.ItinerarySaleRow r in itinerarySet.ItinerarySale)
            {
                if (r.RowState == DataRowState.Deleted)
                    continue;

                gridPayments.DisplayLayout.ValueLists["SalesList"].ValueListItems.Add(r.ItinerarySaleID, GetSaleDisplayString(r));
            }
        }

        internal void AddContact(int? contactId)
        {
            int groupId = GetDefaultItinerayGroup().ItineraryGroupID;
            ItinerarySet.ItineraryMemberRow r = itinerarySet.ItineraryMember.NewItineraryMemberRow();
            
            r.ItineraryGroupID = groupId;
            r.AddedOn = DateTime.Now;
            r.AddedBy = TourWriter.Global.Cache.User.UserID;
            bool isFirstRow = (itinerarySet.ItineraryMember.Rows.Count == 0);
            r.IsDefaultContact = isFirstRow;
            r.IsDefaultBilling = isFirstRow;
            if (Cache.ToolSet.AgeGroup.Rows.Count > 0)
                r.AgeGroupID = (int)Cache.ToolSet.AgeGroup.Rows[0]["AgeGroupID"];
            
            if(contactId.HasValue)
            {
                if (itinerarySet.Contact.FindByContactID((int)contactId) == null)
                {
                    // Import contact to handle constraints
                    Contact c = new Contact();
                    ContactSet.ContactRow contact = c.GetContactSet((int)contactId).Contact[0];
                    itinerarySet.Contact.ImportRow(contact);
                }
                r.ItineraryMemberName = itinerarySet.Contact.FindByContactID((int) contactId).ContactName;
                r.ContactID = (int)contactId;
            }
            else
            {
                r.ItineraryMemberName = App.CreateUniqueNameValue(
                    gridMembers.Rows, "ItineraryMemberName", "New Member");
            }
            
            itinerarySet.ItineraryMember.AddItineraryMemberRow(r);

            GridHelper.SetActiveRow(
                gridMembers, "ItineraryMemberID", r.ItineraryMemberID, "ItineraryMemberName");
        }

        internal void AddPax()
        {
            ItinerarySet.ItineraryPaxRow pax = itinerarySet.ItineraryPax.NewItineraryPaxRow();
            pax.ItineraryID = itinerarySet.Itinerary[0].ItineraryID;
            pax.ItineraryPaxName = App.CreateUniqueNameValue(gridPax.Rows, "ItineraryPaxName", "New pax");
            itinerarySet.ItineraryPax.AddItineraryPaxRow(pax);

            GridHelper.SetActiveRow(gridPax, "ItineraryPaxID", pax.ItineraryPaxID, "ItineraryPaxName");
        }

        private void LockUnlockPaymentsRow(UltraGridRow row)
        {
            // disable row if it's locked
            bool isLocked = row.Cells["IsLockedAccounting"].Value != DBNull.Value
                            && (bool)row.Cells["IsLockedAccounting"].Value;

            foreach (UltraGridColumn column in gridPayments.DisplayLayout.Bands[0].Columns)
                if (column.Key != "IsLockedAccounting")
                    row.Cells[column].Activation = (isLocked) ? Activation.Disabled : Activation.AllowEdit;
        }

        private void gridPax_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ItineraryPaxName")
                {
                    c.Width = 80;
                    c.Header.Caption = "Pax name";
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MemberCount")
                {
                    c.Width = 30;
                    c.Header.Caption = "Member count";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MemberRooms")
                {
                    c.Width = 30;
                    c.Header.Caption = "Member rooms";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "StaffCount")
                {
                    c.Width = 30;
                    c.Header.Caption = "Staff count";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "StaffRooms")
                {
                    c.Width = 30;
                    c.Header.Caption = "Staff rooms";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else
                    c.Hidden = true;
            }

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void btnPaxAdd_Click(object sender, EventArgs e)
        {
            AddPax();
        }

        private void btnPaxDel_Click(object sender, EventArgs e)
        {
            if (gridPax.ActiveRow != null && App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridPax, true);
        }

        private void gridMembers_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.Insert(0, "Edit");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ItineraryMemberName")
                {
                    c.Width = 100;
                    c.Header.Caption = "Person name";
                    c.Header.ToolTipText = "Client name";
                    c.Band.SortedColumns.Add(c, false);
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "Comments")
                {
                    c.Width = 200;
                    c.Header.Caption = "Comments";
                    c.Header.ToolTipText = "Comments (private)";
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.VertScrollBar = true;
                }
                else if (c.Key == "AgeGroupID")
                {
                    c.Width = 65;
                    c.MinWidth = 65;
                    c.MaxWidth = 65;
                    c.Header.Caption = "Age-group";
                    c.Header.ToolTipText = "Age-group category";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridMembers.DisplayLayout.ValueLists["AgeGroupsList"];
                }
                else if (c.Key == "Age")
                {
                    c.Width = 35;
                    c.MinWidth = 35;
                    c.MaxWidth = 35;
                    c.MaskInput = "nnn";
                    c.Header.ToolTipText = "Client age";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "Edit")
                {
                    c.Width = 35;
                    c.MinWidth = 35;
                    c.MaxWidth = 35;
                    c.Header.Caption = "More";
                    c.Header.ToolTipText = "Full contact details";
                    c.Style = ColumnStyle.Button;
                    c.CellButtonAppearance.Image = TourWriter.Properties.Resources.PageEdit;
                    c.CellButtonAppearance.ImageHAlign = HAlign.Center;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                }
                else
                    c.Hidden = true;
            }

            int index = 0;
            e.Layout.Bands[0].Columns["ItineraryMemberName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Comments"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["AgeGroupID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Age"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Edit"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Override.RowSizing = RowSizing.AutoFree;
        }
        
        private void gridMembers_ClickCellButton(object sender, CellEventArgs e)
        {
            gridMembers_HandleEditRequest(e.Cell.Row);
        }

        private void gridMembers_HandleEditRequest(UltraGridRow row)
        {
            if (row == null)
                return;
            
            if(row.Cells["ContactID"].Value == DBNull.Value)
            {
                // Contact detail does not exist
                if (App.AskCreateRow())
                {
                    // Open contact dialog to create new contact row
                    ContactMain contact = new ContactMain();

                    if (contact.ShowDialog() == DialogResult.OK)
                    {
                        // Get new Contact row
                        ContactSet.ContactRow c = (ContactSet.ContactRow)contact.ContactRow;

                        // Load contact row into this itinerarySet
                        itinerarySet.Contact.BeginLoadData();
                        itinerarySet.Contact.LoadDataRow(c.ItemArray, true);
                        itinerarySet.Contact.EndLoadData();
                        
                        // Add FK value
                        row.Cells["ContactID"].Value = c.ContactID;
                    }
                }
            }
            else
            {
                // Open existing contact record
                ContactMain contact = new ContactMain((int)row.Cells["ContactID"].Value);

                if (contact.ShowDialog() == DialogResult.OK)
                {
                    // Reload contact to reflect changes
                    itinerarySet.Contact.BeginLoadData();
                    itinerarySet.Contact.LoadDataRow(contact.ContactRow.ItemArray, true);
                    itinerarySet.Contact.EndLoadData();
                }
                contact.Dispose();
            }
        }

        private void gridMembers_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "IsDefaultContact")
            {
                if (!(bool) e.Cell.Value) // underlying is not ticked so this a tick
                {
                    // Make others false
                    foreach (UltraGridRow r in gridMembers.Rows)
                        if (r != e.Cell.Row && (bool) r.Cells["IsDefaultContact"].Value)
                            r.Cells["IsDefaultContact"].Value = false;
                }
            }
            else if (e.Cell.Column.Key == "IsDefaultBilling")
            {
                if (!(bool) e.Cell.Value) // underlying is not ticked so this a tick
                {
                    // Make others false
                    foreach (UltraGridRow r in gridMembers.Rows)
                        if (r != e.Cell.Row && (bool) r.Cells["IsDefaultBilling"].Value)
                            r.Cells["IsDefaultBilling"].Value = false;
                }
            }
        }
        
        private void gridMembers_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() != typeof(UltraGridEmptyRow))
                gridMembers_HandleEditRequest(e.Row);
        }

        private void btnMemberAdd_Click(object sender, EventArgs e)
        {
            AddContact(null);
        }

        private void btnMemberDelete_Click(object sender, EventArgs e)
        {
            if (gridMembers.ActiveRow != null && App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridMembers, true);
        }

        private void btnMemberEdit_Click(object sender, EventArgs e)
        {
            gridMembers_HandleEditRequest(gridMembers.ActiveRow);
        }

        private void gridPayments_CellChange(object sender, CellEventArgs e)
        {
            // When the "Locked" checkbox is clicked, it needs to be forced out of edit mode
            // so that the row will immediately reflect the change (become enabled/disabled)
            if (e.Cell.Column.Key == "IsLockedAccounting")
            {
                e.Cell.EditorResolved.ExitEditMode(true, true);
            }
        }

        private void gridPayments_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "IsLockedAccounting")
                {
                    c.Header.Caption = String.Empty;
                    c.Header.Appearance.Image = Properties.Resources.LockEdit;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.CellClickAction = CellClickAction.Edit;
                    c.Width = 30;
                    c.MaxWidth = 30;
                }
                else if (c.Key == "ItineraryMemberID")
                {
                    c.Width = 100;
                    c.Header.Caption = "Person name";
                    c.Header.ToolTipText = "The persons name";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridPayments.DisplayLayout.ValueLists["MembersList"];
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "ItinerarySaleID")
                {
                    c.Width = 100;
                    c.Header.Caption = "Sale";
                    c.Header.ToolTipText = "The sale associated with this payment";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridPayments.DisplayLayout.ValueLists["SalesList"];
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "PaymentTypeID")
                {
                    c.Width = 100;
                    c.Header.Caption = "Type";
                    c.Header.ToolTipText = "Payment type";
                    c.ValueList = gridPayments.DisplayLayout.ValueLists["PaymentTypeList"]; ;
                    c.Style = ColumnStyle.DropDownList;
                    c.VertScrollBar = true;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "Comments")
                {
                    c.Width = 300;
                    c.Header.Caption = "Details";
                    c.Header.ToolTipText = "Payment details";
                    c.VertScrollBar = true;
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "PaymentDate")
                {
                    c.Width = 80;
                    c.MinWidth = 80;
                    c.MaxWidth = 80;
                    c.Header.Caption = "Date";
                    c.Header.ToolTipText = "Date of payment";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "Amount")
                {
                    c.Width = 80;
                    c.MinWidth = 80;
                    c.MaxWidth = 80;
                    c.Header.Caption = "Amount";
                    c.Header.ToolTipText = "Value of payment";
                    c.Format = "#0.00";
                    c.MaskInput = "{LOC}-nnnnnn.nn";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
            }

            int index = 0;
            e.Layout.Bands[0].Columns["IsLockedAccounting"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ItineraryMemberID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PaymentTypeID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Comments"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ItinerarySaleID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PaymentDate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Amount"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Override.RowSizing = RowSizing.AutoFree;
            gridPayments_InitializeSummaries(e);
        }

        internal void SetCurrencyFormat()
        {
            var itinerary = itinerarySet.Itinerary[0];
            var currencyInfo = CurrencyService.Single(itinerary.CurrencyCode);
            var format = currencyInfo != null ? currencyInfo.DisplayFormat : "c";

            gridPayments.DisplayLayout.Bands[0].Columns["Amount"].Format = format;
            gridPayments.DisplayLayout.Bands[0].Summaries["TotalAmount"].DisplayFormat = "{0:" + format + "}";

        }

        private void gridPayments_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            LockUnlockPaymentsRow(e.Row);
        }

        private void gridPayments_InitializeSummaries(InitializeLayoutEventArgs e)
        {
            GridHelper.SetDefaultSummaryAppearance(e);

            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            band.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            SummarySettings summary;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Amount"]);
            summary.Key = "TotalAmount";
            summary.DisplayFormat = "{0:#0.00}";
            summary.SummaryPosition = SummaryPosition.Right;
            summary.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            summary.Lines = 2;
        }

        private void gridPayments_BeforeCellListDropDown(object sender, CancelableCellEventArgs e)
        {
            if (e.Cell.Column.Key == "PaymentTypeID")
                RefreshPaymentTypesList();

            if (e.Cell.Column.Key == "ItineraryMemberID")
                RefreshMembersList();

            if (e.Cell.Column.Key == "ItinerarySaleID")
                RefreshSalesList();
        }

        private void btnPaymentAdd_Click(object sender, EventArgs e)
        {
            if (gridMembers.Rows.Count == 0)
            {
                App.ShowInfo("You must first add a person to the clients list");
                return;
            }

            // Ensure gridMembers.ActiveRow is in the members list.
            RefreshMembersList();
            
            UltraGridRow memberRow = gridMembers.ActiveRow ?? gridMembers.Rows[0];

            ItinerarySet.ItineraryPaymentRow r = itinerarySet.ItineraryPayment.NewItineraryPaymentRow();
            r.ItineraryMemberID = (int)memberRow.Cells["ItineraryMemberID"].Value;
            r.PaymentDate = DateTime.Now;

            if (Cache.ToolSet.PaymentType.DefaultPaymentTypeRow != null)
            {
                // set the default payment type
                r.PaymentTypeID = Cache.ToolSet.PaymentType.DefaultPaymentTypeRow.PaymentTypeID;
            }

            itinerarySet.ItineraryPayment.AddItineraryPaymentRow(r);

            GridHelper.SetActiveRow(gridPayments, "ItineraryPaymentID", r.ItineraryPaymentID, "");
        }

        private void btnPaymentDelete_Click(object sender, EventArgs e)
        {
            if (gridPayments.ActiveRow != null && App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridPayments, true);
        }

        private void ultraTabControl2_VisibleChanged(object sender, EventArgs e)
        {
            if (!App.IsInDesignMode && Visible) AddFirstItineraryGroup();
        }
    }
}
