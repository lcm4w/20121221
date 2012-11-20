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
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace TourWriter.Modules.ItineraryModule.RoomTypes
{
    public partial class RoomTypesControl : UserControl
    {        
        private ItinerarySet itinerarySet = null;

        internal ItinerarySet ItinerarySet
        {
            get { return itinerarySet; }
            set
            {
                itinerarySet = value;
                DataBind();                
                //itinerarySet.RoomType.ColumnChanged += RoomType_ColumnChanged;
            }
        }
        
        public RoomTypesControl()
        {
            InitializeComponent();
            if (App.ShowRoomTypes) return;
            splitContainer1.Panel1Collapsed = true;
            splitContainer1.Panel1.Hide();           
        }       

        private void DataBind()
        {
            // load Add items.
            if (App.ShowRoomTypes)
            {
                btnAdd.DropDownItems.Clear();
                btnAdd.DropDownItems.Add(new ToolStripMenuItem("Add for...") {Enabled = false});
                btnAdd.DropDownItems.Add(new ToolStripSeparator());
                foreach (var option in Global.Cache.ToolSet.OptionType)
                {
                    //btnAdd.DropDownItems.Add(new ToolStripMenuItem(option["OptionTypeName"].ToString(), null,btnAddRoomType_Click){Tag = option["OptionTypeID"].ToString()});
                    btnAdd.DropDownItems.Add(new ToolStripMenuItem(option["OptionTypeName"].ToString(), null, btnAddRoomType_Click) { Tag = option["OptionTypeID"].ToString() });
                }

                //gridRoomTypes.DataSource = itinerarySet.RoomType;
                gridRoomTypes.SetDataBinding(itinerarySet, "Itinerary.Itinerary_RoomType");//SetDataBinding(itinerarySet, "Itinerary.ItineraryItineraryGroup.ItineraryGroupGroupMember");
                gridRoomTypes.InitializeRow += gridRoomTypes_InitializeRow;
                gridMembers.InitializeRow += gridMembers_InitializeRow;
                gridMembers.DisplayLayout.ValueLists.Add("RoomList");

                UpdateRoomTypeDDL();
                itinerarySet.RoomType.ColumnChanged += RoomType_ColumnChanged;              
            }

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

            gridMembers.DisplayLayout.ValueLists.Add("AgentList");
            gridMembers.DisplayLayout.ValueLists["AgentList"].SortStyle = ValueListSortStyle.Ascending;
            gridMembers.DisplayLayout.ValueLists["AgentList"].ValueListItems.Add(DBNull.Value, "(none)");
            foreach(ToolSet.AgentRow r in Cache.ToolSet.Agent.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                    continue;
                gridMembers.DisplayLayout.ValueLists["AgentList"].ValueListItems.Add(r.AgentID, r.AgentName);
            }

            //gridMembers.DisplayLayout.ValueLists.Add("RoomList");         
            //UpdateRoomTypeDDL();            
            gridMembers.SetDataBinding(itinerarySet, "Itinerary.ItineraryItineraryGroup.ItineraryGroupGroupMember");
            CalculateRoomType();
        }

        private void gridMembers_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //if ((from DataRow r in itinerarySet.RoomType.Rows where r.RowState != DataRowState.Deleted && r["OptionTypeID"].ToString() == optionTypeID.ToString() select r).SingleOrDefault() == null)
            if (e.ReInitialize) return;
            if (e.Row.Cells["RoomTypeID"].Value != null)
            {
                if (string.IsNullOrEmpty(e.Row.Cells["RoomTypeID"].Value.ToString())) return;
                var roomType = itinerarySet.RoomType.FindByRoomTypeID((int) e.Row.Cells["RoomTypeID"].Value);               
                e.Row.Cells["RoomTypeCombo"].Value = roomType == null ? 0 : roomType.OptionTypeID; ;                               
            }
        }

        private void gridRoomTypes_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (!e.ReInitialize)
            {               
                CalculateRoomType();
            }            
        }
       
        private void UpdateRoomTypeDDL()
        {
            gridMembers.DisplayLayout.ValueLists["RoomList"].ValueListItems.Clear();
            gridMembers.DisplayLayout.ValueLists["RoomList"].SortStyle = ValueListSortStyle.Ascending;
            gridMembers.DisplayLayout.ValueLists["RoomList"].ValueListItems.Add(0, "(none)");
            foreach (var option in Global.Cache.ToolSet.OptionType)
            {
                //btnAdd.DropDownItems.Add(new ToolStripMenuItem(option["OptionTypeName"].ToString(), null, btnAddRoomType_Click) { Tag = option["OptionTypeID"].ToString() });
                gridMembers.DisplayLayout.ValueLists["RoomList"].ValueListItems.Add(option["OptionTypeID"], option["OptionTypeName"].ToString());
            }

            //foreach (DataRow r in itinerarySet.RoomType.Rows)
            //{
            //    if (r.RowState == DataRowState.Deleted)
            //        continue;

            //    gridMembers.DisplayLayout.ValueLists["RoomList"].ValueListItems.Add(r["OptionTypeID"], r["RoomTypeName"].ToString());
            //}
        }

        private void FilterMembersRow()
        {
            if (!App.ShowRoomTypes) return;
            if (gridRoomTypes.ActiveRow == null) return;
            gridMembers.DisplayLayout.Bands[0].ColumnFilters["RoomTypeID"].ClearFilterConditions(); //RoomTypeID
            gridMembers.DisplayLayout.Bands[0].ColumnFilters["RoomTypeID"].FilterConditions.Add(FilterComparisionOperator.Equals, gridRoomTypes.ActiveRow.Cells["RoomTypeName"].Value.ToString());//int.Parse(gridRoomTypes.ActiveRow.Cells["OptionTypeID"].Value.ToString())); //RoomTypeName          
        }

        private void CalculateRoomType()
        {
            if (gridRoomTypes.Rows.Count == 0) return;
            foreach (UltraGridRow r in gridRoomTypes.Rows)
            {                                                                                                                                                      //OptionTypeID
                //var roomTypeTotal = (from DataRow m in itinerarySet.ItineraryMember.Rows where m.RowState != DataRowState.Deleted && m["RoomTypeID"].ToString() == r.Cells["RoomTypeID"].Value.ToString() select m).Count();//Count(r => r.RowState != DataRowState.Deleted);
                var count=0;
                foreach (UltraGridRow m in gridMembers.Rows)
                {
                    if (m.Cells["RoomTypeCombo"].Value == null) continue;
                    if (r.Cells["OptionTypeID"].Value.ToString() == m.Cells["RoomTypeCombo"].Value.ToString())
                    {
                        count++;
                    }
                }
                r.Cells["Actual"].Value = count;//roomTypeTotal;
            }
            gridRoomTypes.UpdateData();            
        }

        private void RoomType_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            //if (e.Column.ColumnName == "RoomTypeName")
            //{
            //    UpdateRoomTypeDDL();
            //}
        }

        internal void AddContact(int? contactId)
        {
            CommitOpenEdits();

            int groupId = GetDefaultItinerayGroup().ItineraryGroupID;
            ItinerarySet.ItineraryMemberRow r = itinerarySet.ItineraryMember.NewItineraryMemberRow();

            r.ItineraryGroupID = groupId;            
            if (gridRoomTypes.ActiveRow != null)
            {
                r.RoomTypeID = (int)gridRoomTypes.ActiveRow.Cells["RoomTypeID"].Value;//int.Parse(gridRoomTypes.ActiveRow.Cells["OptionTypeID"].Value.ToString());
            }
                      
            r.AddedOn = DateTime.Now;
            r.AddedBy = TourWriter.Global.Cache.User.UserID;
            bool isFirstRow = (itinerarySet.ItineraryMember.Rows.Count == 0);
            r.IsDefaultContact = isFirstRow;
            r.IsDefaultBilling = isFirstRow;
            if (Cache.ToolSet.AgeGroup.Rows.Count > 0)
                r.AgeGroupID = (int)Cache.ToolSet.AgeGroup.Rows[0]["AgeGroupID"];

            if (contactId.HasValue)
            {
                if (itinerarySet.Contact.FindByContactID((int)contactId) == null)
                {
                    // Import contact to handle constraints
                    Contact c = new Contact();
                    ContactSet.ContactRow contact = c.GetContactSet((int)contactId).Contact[0];
                    itinerarySet.Contact.ImportRow(contact);
                }
                r.ItineraryMemberName = itinerarySet.Contact.FindByContactID((int)contactId).ContactName;
                r.ContactID = (int)contactId;
            }
            else
            {
                r.ItineraryMemberName = App.CreateUniqueNameValue(
                    gridMembers.Rows, "ItineraryMemberName", "New Member");
            }

            itinerarySet.ItineraryMember.AddItineraryMemberRow(r);
            GridHelper.SetActiveRow(gridMembers, "ItineraryMemberID", r.ItineraryMemberID, "ItineraryMemberName");
            if (App.ShowRoomTypes)
            {              
                CalculateRoomType();
            }            
        }
        
        private void CommitOpenEdits()
        {
            gridMembers.UpdateData();
            gridRoomTypes.UpdateData();
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

        #region gridRoomTypes      
        private void gridRoomTypes_AfterRowActivate(object sender, EventArgs e)
        {            
            //FilterMembersRow();           
        }

        private void gridRoomTypes_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            //add custom column         
            if (!e.Layout.Bands[0].Columns.Exists("Actual"))
                e.Layout.Bands[0].Columns.Add("Actual");

            // show/hide columns 
            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "RoomTypeID" || c.Key == "ItineraryID" || c.Key == "OptionTypeID")
                {
                    c.Hidden = true;
                }
                else if (c.Key == "RoomTypeName")
                {
                    c.Header.Caption = "Room Name";
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "Quantity")
                {
                    c.Header.Caption = "Pax Override";
                    c.Width = 115;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "Actual")
                {
                    
                }
            }
            GridHelper.SetDefaultGridAppearance(e);
            CalculateRoomType();
        }

        #endregion

        #region gridMembers

        private void gridMembers_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("RoomTypeCombo"))
                e.Layout.Bands[0].Columns.Add("RoomTypeCombo");
            if (!e.Layout.Bands[0].Columns.Exists("Edit"))
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
                else if (c.Key == "Title")
                {
                    c.Width = 40;
                    c.MinWidth = 40;
                    c.MaxWidth = 40;
                    c.Header.Caption = "Title";
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "Comments")
                {
                    c.Width = 140;
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
                    c.Width = 40;
                    c.MinWidth = 40;
                    c.MaxWidth = 40;
                    c.Header.Caption = "More";
                    c.Header.ToolTipText = "Full contact details";
                    c.Style = ColumnStyle.Button;
                    c.CellButtonAppearance.Image = TourWriter.Properties.Resources.PageEdit;
                    c.CellButtonAppearance.ImageHAlign = HAlign.Center;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                }
                else if (c.Key ==  "RoomTypeCombo")//"RoomTypeID") && App.ShowRoomTypes)
                {
                    c.Header.Caption = "Room Type";
                    c.Width = 70;
                    c.MinWidth = 70;
                    c.MaxWidth = 70;
                    c.Header.ToolTipText = "Roomtype category";
                    c.Style = ColumnStyle.DropDownList;
                    c.DataType = typeof (int);
                    c.ValueList = gridMembers.DisplayLayout.ValueLists["RoomList"];
                }
                else if (c.Key == "AgentID" && App.ShowRoomTypes)
                {
                    c.Header.Caption = "Agent";
                    c.Width = 70;
                    c.MinWidth = 70;
                    //c.MaxWidth = 70;
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridMembers.DisplayLayout.ValueLists["AgentList"];
                }
                else if (c.Key == "PriceOverride" && App.ShowRoomTypes)
                {                   
                    c.Width = 80;
                    c.MinWidth = 80;
                    c.MaxWidth = 80;
                    c.Header.Caption = "Price Override";
                    c.Header.ToolTipText = "Value of payment";
                    c.Format = "#0.00";
                    c.MaskInput = "{LOC}-nnnnnn.nn";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "RoomName" && App.ShowRoomTypes)
                {
                    c.Width = 80;
                    c.MinWidth = 80;
                    c.MaxWidth = 80;
                }
                else
                    c.Hidden = true;
                if (!App.ShowRoomTypes && gridMembers.Rows.Count > 0)
                {
                    gridMembers.ActiveRow = gridMembers.Rows[0];
                }
            }

            int index = 0;
            e.Layout.Bands[0].Columns["Title"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ItineraryMemberName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["AgentID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["RoomTypeCombo"].Header.VisiblePosition = index++; //ROOMTYPEID
            e.Layout.Bands[0].Columns["PriceOverride"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["RoomName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Comments"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["AgeGroupID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Age"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Edit"].Header.VisiblePosition = index++;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Override.RowSizing = RowSizing.AutoFree;            
        }

        private void gridMembers_AfterExitEditMode(object sender, EventArgs e)
        {
            if (gridMembers.ActiveRow == null) return;

            if (gridMembers.ActiveRow.Cells["RoomTypeCombo"].Value == null || (int)gridMembers.ActiveRow.Cells["RoomTypeCombo"].Value == 0) return;
            if (string.IsNullOrEmpty(gridMembers.ActiveRow.Cells["RoomTypeCombo"].Value.ToString())) return;
            var optionTypeID = (int)gridMembers.ActiveRow.Cells["RoomTypeCombo"].Value;
            var optionTypeName = gridMembers.ActiveRow.Cells["RoomTypeCombo"].Text;
            AddRoomType(optionTypeID, optionTypeName, false );
            //CalculateRoomType();
            //FilterMembersRow();           
        }

        private void gridMembers_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "IsDefaultContact")
            {
                if (!(bool)e.Cell.Value) // underlying is not ticked so this a tick
                {
                    // Make others false
                    foreach (UltraGridRow r in gridMembers.Rows)
                        if (r != e.Cell.Row && (bool)r.Cells["IsDefaultContact"].Value)
                            r.Cells["IsDefaultContact"].Value = false;
                }
            }
            else if (e.Cell.Column.Key == "IsDefaultBilling")
            {
                if (!(bool)e.Cell.Value) // underlying is not ticked so this a tick
                {
                    // Make others false
                    foreach (UltraGridRow r in gridMembers.Rows)
                        if (r != e.Cell.Row && (bool)r.Cells["IsDefaultBilling"].Value)
                            r.Cells["IsDefaultBilling"].Value = false;
                }
            }
        }

        private void gridMembers_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() != typeof(UltraGridEmptyRow))
                gridMembers_HandleEditRequest(e.Row);
        }

        private void gridMembers_ClickCellButton(object sender, CellEventArgs e)
        {
            gridMembers_HandleEditRequest(e.Cell.Row);
        }

        private void gridMembers_HandleEditRequest(UltraGridRow row)
        {
            if (row == null)
                return;

            if (row.Cells["ContactID"].Value == DBNull.Value)
            {
                // Contact detail does not exist
                if (App.AskCreateRow())
                {
                    // Open contact dialog to create new contact row
                    var contact = new ContactMain();
                    if (row.Cells["ItineraryMemberName"].Value != DBNull.Value)
                        if (row.Cells["ItineraryMemberName"].Value.ToString() != String.Empty)
                            contact.ContactRow["ContactName"] = row.Cells["ItineraryMemberName"].Value.ToString();

                    if (contact.ShowDialog() == DialogResult.OK)
                    {
                        // Get new Contact row
                        var c = (ContactSet.ContactRow)contact.ContactRow;

                        // Load contact row into this itinerarySet
                        itinerarySet.Contact.BeginLoadData();
                        itinerarySet.Contact.LoadDataRow(c.ItemArray, true);
                        itinerarySet.Contact.EndLoadData();

                        // Add FK value
                        row.Cells["ContactID"].Value = c.ContactID;
                        row.Cells["ItineraryMemberName"].Value = c.ContactName;
                    }
                }
            }
            else
            {
                // Open existing contact record
                var contact = new ContactMain((int)row.Cells["ContactID"].Value);

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
        #endregion

        #region RoomType Buttons
        private void btnAddRoomType_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripDropDownItem;
            if (item == null) return;

            var optionTypeID = int.Parse(item.Tag.ToString());
            AddRoomType(optionTypeID, item.Text, true);      
        }

        private void AddRoomType(int optionTypeID, string optionTypeName, bool IsFromRoomType)
        {
            var roomType = (from DataRow r in itinerarySet.RoomType.Rows where r.RowState != DataRowState.Deleted && r["OptionTypeID"].ToString() == optionTypeID.ToString() select r).SingleOrDefault();
            if (roomType == null)//((from DataRow r in itinerarySet.RoomType.Rows where r.RowState != DataRowState.Deleted && r["OptionTypeID"].ToString() == optionTypeID.ToString() select r).SingleOrDefault() == null)
            {             
                var rt = itinerarySet.RoomType.NewRoomTypeRow();
                rt.ItineraryID = itinerarySet.Itinerary[0].ItineraryID;
                rt.OptionTypeID = optionTypeID;
                rt.RoomTypeName = optionTypeName;
                itinerarySet.RoomType.AddRoomTypeRow(rt);

                GridHelper.SetActiveRow(gridRoomTypes, "RoomTypeID", rt.RoomTypeID, "RoomTypeName");
                if (gridMembers.ActiveRow != null && !IsFromRoomType)
                    gridMembers.ActiveRow.Cells["RoomTypeID"].Value = rt.RoomTypeID;
                //UpdateRoomTypeDDL();

            }
            else
            {
                if (gridMembers.ActiveRow != null && !IsFromRoomType)
                    gridMembers.ActiveRow.Cells["RoomTypeID"].Value = roomType["RoomTypeID"];
            }
            CalculateRoomType();          
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (gridRoomTypes.ActiveRow != null && App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridRoomTypes, true);
        }
             
        private void btnAll_Click(object sender, EventArgs e)
        {
            gridMembers.DisplayLayout.Bands[0].ColumnFilters["RoomTypeID"].ClearFilterConditions();
            if (gridRoomTypes.ActiveRow != null)
            {
                gridRoomTypes.ActiveRow.Selected = false;
                gridRoomTypes.ActiveRow = null;
            }
        }
        #endregion

        #region Members Buttons
        private void btnAddMember_Click(object sender, EventArgs e)
        {                  
            AddContact(null);
        }

        private void btnMemberDelete_Click(object sender, EventArgs e)
        {
            if (gridMembers.ActiveRow != null && App.AskDeleteRow())
            {
                GridHelper.DeleteActiveRow(gridMembers, true);
                CalculateRoomType();
                //CalculateRoomType(int.Parse(gridMembers.ActiveRow.Cells["RoomTypeID"].Value.ToString()));
            }
        }

        private void btnMemberEdit_Click(object sender, EventArgs e)
        {
            gridMembers_HandleEditRequest(gridMembers.ActiveRow);
        }
        #endregion                         
    }
}
