using System;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinTree;
using TourWriter.BusinessLogic;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Modules.ContactModule;
using TourWriter.Services;
using ButtonDisplayStyle=Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using CellClickAction=Infragistics.Win.UltraWinGrid.CellClickAction;
using CellEventArgs=Infragistics.Win.UltraWinGrid.CellEventArgs;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;
using SelectType=Infragistics.Win.UltraWinGrid.SelectType;

namespace TourWriter.Modules.SupplierModule
{
    public partial class SupplierMain : ModuleBase
    {
        private bool serverConnectionError_UserNotified = false;

        internal SupplierSet supplierSet;

        private static ToolSet toolSet
        {
            get { return Cache.ToolSet; }
        }

        public SupplierMain()
        {
            InitializeComponent();
            displayTypeName = "Supplier";

            // Hide menus as they are merged with MdiParent form
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;

            tabsAdditional.ActiveTabChanged += (o, e) => { if (tabsAdditional.ActiveTab.Key == "Reports") App.ShowInfo(App.OldReportsExpiringText); };
            //if (!App.IsDebugMode) tabsAdditional.Tabs["Reports"].Visible = false; // Hide old reports tab. TODO: remove (hidden 2010.2.17)
        }

        private void SupplierMain_Load(object sender, EventArgs e)
        {
            DisplaySupplier();

            // Events to show if data needs saving
            if (supplierSet != null)
            {
                foreach (DataTable dt in supplierSet.Tables)
                {
                    dt.ColumnChanged += SupplierSet_ColumnChanged;
                    dt.RowDeleted += SupplierSet_RowDeleted;
                }
                reportControl.FilterReportExplorer("Supplier");
                reportControl.DefaultParameters.Add("@SupplierID", supplierSet.Supplier[0].SupplierID);
            }

            Cache.ToolSet.TemplateCategory.RowChanged += DataTable_RowChanged;
            Cache.ToolSet.TemplateCategory.ColumnChanged += DataTable_ColumnChanged;

            Cache.ToolSet.Template.RowChanged += DataTable_RowChanged;
            Cache.ToolSet.Template.ColumnChanged += DataTable_ColumnChanged;
        }

        protected override bool IsDataDirty()
        {
            CommitOpenEdits();
            return (base.Text.StartsWith("*") || (supplierSet != null && supplierSet.HasChanges()));
        }

        protected override void CommitOpenEdits()
        {
            if (supplierSet == null)
                return;
            
            Validate();

            App.CommitGridEdits(this);

            // tables
            foreach (DataTable dt in supplierSet.Tables)
                foreach (DataRow dr in dt.Rows)
                    dr.EndEdit();
        }

        protected override void SaveDataChanges()
        {
            if (!App.ShowCheckPermission(AppPermissions.Permissions.SupplierEdit))
                return;

            if (IsDataDirty())
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    // save changes
                    Supplier s = new Supplier();
                    SupplierSet changes = (SupplierSet)supplierSet.GetChanges();
                    if (changes != null)
                    {
                        SupplierSet fresh = s.SaveSupplierSet(changes);

                        // handle errors
                        App.DataSet_CheckForErrors(fresh);
                        if (App.DataSet_AskSaveDeleteConstraints(fresh))
                        {
                            supplierSet = fresh;
                            SaveDataChanges();
                            return;
                        }
                        // clear and merge to maintain data bindings
                        supplierSet.Clear();
                        supplierSet.Merge(fresh, false);
                    }

                    // Save the toolset.
                    if (Cache.ToolSet != null && Cache.ToolSet.HasChanges())
                    {
                        Cache.SaveToolSet();
                        reportControl.RefreshReportExplorer();
                        reportControl.FilterReportExplorer("Supplier");
                    }

                    UpdateMainForm(App.MainForm.SupplierMenu, supplierSet.Supplier[0].IsRecordActive);
                    SetDataCleanName();

                    accounting1.RefreshRequired = true;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        protected override void CancelDataChanges()
        {
            return;
        }

        protected override string GetDisplayName()
        {
            if (supplierSet != null && supplierSet.Supplier.Count > 0)
                return supplierSet.Supplier[0].SupplierName;
            return "";
        }

        private void RefreshData()
        {
            if (IsDataDirty())
            {
                DialogResult result = MessageBox.Show(App.GetResourceString("AskDoSave"), Text,
                                                      MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveDataChanges();
                    return;
                }
                else if (result == DialogResult.Cancel)
                    return;
            }
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // refresh supplier
                Supplier s = new Supplier();
                SupplierSet fresh = s.GetSupplierSet(supplierSet.Supplier[0].SupplierID);

                // clear current and merge to maintain databindings
                supplierSet.Clear();
                supplierSet.Merge(fresh, false);

                // refresh the toolset
                Cache.RefreshToolSet(false);
                reportControl.RefreshReportExplorer();
                reportControl.FilterReportExplorer("Supplier");

                SetDataCleanName();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof (SelectedNodesCollection)))
            {
                SelectedNodesCollection col =
                    e.Data.GetData(typeof (SelectedNodesCollection)) as SelectedNodesCollection;

                if (col != null && col.Count > 0)
                {
                    object tag = col[0].Tag;
                    if (tag != null)
                    {
                        NavigationTreeItemInfo item = tag as NavigationTreeItemInfo;
                        if(item != null && item.ItemType == NavigationTreeItemInfo.ItemTypes.Contact)
                            e.Effect = DragDropEffects.Link;
                    }
                }
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            Object obj;
            NavigationTreeItemInfo nodeInfo;
            SelectedNodesCollection nodeCollection;

            obj = e.Data.GetData(typeof (SelectedNodesCollection));
            nodeCollection = obj as SelectedNodesCollection;

            if (nodeCollection == null)
                return;

            for (int i = 0; i < nodeCollection.Count; i++)
            {
                nodeInfo = nodeCollection[i].Tag as NavigationTreeItemInfo;

                if (nodeInfo != null && nodeInfo.ItemType == NavigationTreeItemInfo.ItemTypes.Contact)
                {
                    int contactID = nodeInfo.ItemID;
                    string contactName = nodeInfo.ItemName;

                    // ensure row doesn't already exist
                    SupplierSet.SupplierContactRow row = supplierSet.SupplierContact.FindBySupplierIDContactID(
                        supplierSet.Supplier[0].SupplierID, contactID);

                    if (row != null)
                    {
                        App.ShowInfo(App.GetResourceString("ShowRowAlreadyExists"));
                    }
                    else
                    {
                        // first get the contact row to handle constraints
                        Contact c = new Contact();
                        ContactSet cs = c.GetContactSet(contactID);

                        // create new SupplierContact row
                        SupplierSet.SupplierContactRow sc = supplierSet.SupplierContact.NewSupplierContactRow();
                        sc.ContactID = contactID;
                        sc.SupplierID = supplierSet.Supplier[0].SupplierID;
                        sc.Description = contactName;
                        bool isFirstRow = (supplierSet.SupplierContact.Rows.Count == 0);
                        sc.IsDefaultContact = isFirstRow;
                        sc.IsDefaultBilling = isFirstRow;
                        sc.AddedOn = DateTime.Now;
                        sc.AddedBy = Cache.User.UserID;

                        // add the rows
                        supplierSet.Contact.ImportRow(cs.Contact[0]);
                        supplierSet.SupplierContact.AddSupplierContactRow(sc);

                        // activate new row
                        gridContact.Focus();
                        gridContact.ActiveCell = gridContact.Rows[gridContact.Rows.Count - 1].Cells["Description"];
                        gridContact.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    }
                }
            }
            TabControl_Main.SelectedTab = TabControl_Main.Tabs["Additional"];
            tabsAdditional.SelectedTab = tabsAdditional.Tabs["Contacts"];
        }

        private void DisplaySupplier()
        {
            Supplier_LoadSupplierSet();
            
            if(supplierSet == null)
            {
                if (!serverConnectionError_UserNotified)
                    App.ShowError("Failed to load Supplier from database.");

                foreach (Control c in Controls)
                    c.Enabled = false; // disable top level controls
                return;
            }
            
            Supplier_DataBind();

            serviceEditor1.SupplierSet = supplierSet;
            accounting1.SupplierSet = supplierSet;

            UpdatePaymentTermCustomText();

            treeSupplierConfigs_Load();
            DoSecuritySettings();
        }

        private void DoSecuritySettings()
        {
            // supplier bank details
            if (!AppPermissions.UserHasPermission(AppPermissions.Permissions.SupplierBankDetail) &&
                txtBankDetails.Text.Length > 0)
            {
                txtBankDetails.DataBindings.Clear();
                txtBankDetails.Text = Environment.NewLine +
                                      "This data has been hidden by your security settings";
                txtBankDetails.Enabled = false;
            }
        }

        private void Supplier_LoadSupplierSet()
        {
            try
            {
                NavigationTreeItemInfo item = MenuNode.Tag as NavigationTreeItemInfo;
                if (item != null && item.ItemType == NavigationTreeItemInfo.ItemTypes.Supplier)
                {
                    Supplier s = new Supplier();
                    supplierSet = s.GetSupplierSet(item.ItemID);
                }
                else
                {
                    string param = "ItemType";
                    throw new ArgumentException(
                        "SupplierMain.Tag info ItemType is not of type NavigationTreeItemInfo.ItemTypes.Supplier", 
                        param);
                }
            }
            catch (Exception ex)
            {
                if (ErrorHelper.IsServerConnectionError(ex))
                {
                    App.ShowServerConnectionError();
                    serverConnectionError_UserNotified = true;
                }
                else throw;
            }
        }

        private void Supplier_DataBind()
        {
            #region Init

            // city
            cmbCity.DataSource = toolSet.City;
            cmbCity.ValueMember = "CityID";
            cmbCity.DisplayMember = "CityName";
            // region
            cmbRegion.DataSource = toolSet.Region;
            cmbRegion.ValueMember = "RegionID";
            cmbRegion.DisplayMember = "RegionName";
            // state
            cmbState.DataSource = toolSet.State;
            cmbState.ValueMember = "StateID";
            cmbState.DisplayMember = "StateName";
            // country
            cmbCountry.DataSource = toolSet.Country;
            cmbCountry.ValueMember = "CountryID";
            cmbCountry.DisplayMember = "CountryName";
            // grade internal
            cmbGradeInternal.DataSource = toolSet.Grade;
            cmbGradeInternal.ValueMember = "GradeID";
            cmbGradeInternal.DisplayMember = "GradeName";
            // grade external
            cmbGradeExternal.DataSource = toolSet.GradeExternal;
            cmbGradeExternal.ValueMember = "GradeExternalID";
            cmbGradeExternal.DisplayMember = "GradeExternalName";
            // tax types
            cmbTaxType.DataSource = toolSet.TaxType;
            cmbTaxType.ValueMember = "TaxTypeID";
            cmbTaxType.DisplayMember = "TaxTypeCode";
            // credit cards
            treeCreditCards_Load();

            #endregion

            #region Bindings
            
            // supplier
            txtSupplierID.DataBindings.Add("Text", supplierSet, "Supplier.SupplierID");
            txtCustomID.DataBindings.Add("Text", supplierSet, "Supplier.CustomID");
            txtSupplierName.DataBindings.Add("Text", supplierSet, "Supplier.SupplierName");
            txtHosts.DataBindings.Add("Text", supplierSet, "Supplier.HostName");
            txtStreetAddress.DataBindings.Add("Text", supplierSet, "Supplier.StreetAddress");
            cmbCity.DataBindings.Add("Value", supplierSet, "Supplier.CityID");
            cmbRegion.DataBindings.Add("Value", supplierSet, "Supplier.RegionID");
            cmbState.DataBindings.Add("Value", supplierSet, "Supplier.StateID");
            cmbCountry.DataBindings.Add("Value", supplierSet, "Supplier.CountryID");
            txtPostcode.DataBindings.Add("Text", supplierSet, "Supplier.Postcode");
            txtPhone.DataBindings.Add("Text", supplierSet, "Supplier.Phone");
            txtMobilePhone.DataBindings.Add("Text", supplierSet, "Supplier.MobilePhone");
            txtFreePhone.DataBindings.Add("Text", supplierSet, "Supplier.FreePhone");
            txtFax.DataBindings.Add("Text", supplierSet, "Supplier.Fax");
            txtSupplierEmail.DataBindings.Add("Text", supplierSet, "Supplier.Email");
            txtSupplierWebsite.DataBindings.Add("Text", supplierSet, "Supplier.Website");
            txtSupplierPostAddress.DataBindings.Add("Text", supplierSet, "Supplier.PostAddress");
            cmbTaxType.DataBindings.Add("SelectedValue", supplierSet, "Supplier.TaxTypeID");
            cmbGradeInternal.DataBindings.Add("Value", supplierSet, "Supplier.GradeID");
            cmbGradeExternal.DataBindings.Add("Value", supplierSet, "Supplier.GradeExternalID");
            txtSupplierDescription.DataBindings.Add("Text", supplierSet, "Supplier.Description");
            txtSupplierComments.DataBindings.Add("Text", supplierSet, "Supplier.Comments");
            txtAccountingName.DataBindings.Add("Text", supplierSet, "Supplier.AccountingName");
            txtCancellationPolicy.DataBindings.Add("Text", supplierSet, "Supplier.CancellationPolicy");
            txtBankDetails.DataBindings.Add("Text", supplierSet, "Supplier.BankDetails");
            txtDefaultCheckin.DataBindings.Add("Value", supplierSet, "Supplier.DefaultCheckinTime");
            txtDefaultCheckout.DataBindings.Add("Value", supplierSet, "Supplier.DefaultCheckOutTime");
            txtDefaultMargin.DataBindings.Add("Value", supplierSet, "Supplier.DefaultMargin");
            txtImportID.DataBindings.Add("Text", supplierSet, "Supplier.ImportID");
            txtExternalLinkID.DataBindings.Add("Text", supplierSet, "Supplier.ExportID");
            txtBookingWebsite.DataBindings.Add("Text", supplierSet, "Supplier.BookingWebsite");
            chkIsSupplierActive.DataBindings.Add("Checked", supplierSet, "Supplier.IsRecordActive");
            gridNotes.SetDataBinding(supplierSet, "Supplier.SupplierSupplierNote");
            gridNotes.ActiveRow = null;

            txtLatitude.DataBindings.Add("Value", supplierSet, "Supplier.Latitude");
            txtLongitude.DataBindings.Add("Value", supplierSet, "Supplier.Longitude");

            // TODO : hiding spatial except for ENZ, WalshGroup
            pnlSpatial.Visible =
                Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "d949d605-e05f-47a7-9a9d-0d5fca50b2b4" ||
                Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "0d83ad36-b7c5-402f-8e41-84744b6c9991";

            gridContact.SetDataBinding(supplierSet, "Supplier.SupplierSupplierContact");
            gridMessages.SetDataBinding(supplierSet, "Message");
            gridPublishing.SetDataBinding(supplierSet, "Supplier.SupplierSupplierText");

            #endregion

            #region Formatting

            txtDefaultCheckin.FormatString = App.GetLocalShortTime24HrFormat();
            txtDefaultCheckout.FormatString = App.GetLocalShortTime24HrFormat();

            txtDefaultCheckin.MaskInput = App.GetLocalShortTime24HrMask();
            txtDefaultCheckout.MaskInput = App.GetLocalShortTime24HrMask();

            #endregion

            #region Events

            // constrain rate date ranges
            //this.txtRateFrom.Validating += new System.ComponentModel.CancelEventHandler(this.txtRateFrom_Validating);
            //this.txtRateTo.Validating += new System.ComponentModel.CancelEventHandler(this.txtRateTo_Validating);


            // we want changes to these combos to flow through each one
            cmbCountry.ValueChanged += cmbCountry_ValueChanged;
            cmbState.ValueChanged += cmbState_ValueChanged;
            cmbRegion.ValueChanged += cmbRegion_ValueChanged;
            cmbCity.ValueChanged += cmbCity_ValueChanged;

            #endregion
        }

        private void EditContactDetails(int contactId)
        {
            ContactMain contact = new ContactMain(contactId);

            if (contact.ShowDialog() == DialogResult.OK)
            {
                // Reload contact to reflect changes
                supplierSet.Contact.BeginLoadData();
                supplierSet.Contact.LoadDataRow(contact.ContactRow.ItemArray, true);
                supplierSet.Contact.EndLoadData();
            }
            contact.Dispose();
        }

        private void RefreshAccounting()
        {
            CommitOpenEdits();

            if (accounting1.RefreshRequired)
            {
                accounting1.RefreshDataAndControls();
            }
        }

        #region Supplier

        private void cmbCity_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "CityName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbRegion_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "RegionName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbState_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "StateName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbCountry_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "CountryName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }


        private void cmbGrade_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "GradeName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbGradeExternal_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "GradeExternalName")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbCity_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCity.ActiveRow != null && (
                                                     supplierSet.Supplier[0].IsCityIDNull() ||
                                                     (int) cmbCity.ActiveRow.Cells["CityID"].Value !=
                                                     supplierSet.Supplier[0].CityID))
                {
                    // push change to dataset
                    supplierSet.Supplier[0].CityID = (int) cmbCity.ActiveRow.Cells["CityID"].Value;
                    // auto-fill parent list
                    cmbRegion.Value = (int) cmbCity.ActiveRow.Cells["RegionID"].Value;
                }
                else
                    // clear parent list
                    cmbRegion.Value = null;
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void cmbRegion_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbRegion.ActiveRow != null && (
                                                       supplierSet.Supplier[0].IsRegionIDNull() ||
                                                       (int) cmbRegion.ActiveRow.Cells["RegionID"].Value !=
                                                       supplierSet.Supplier[0].RegionID))
                {
                    // push change to dataset
                    supplierSet.Supplier[0].RegionID = (int) cmbRegion.ActiveRow.Cells["RegionID"].Value;
                    // auto-fill parent list
                    cmbState.Value = (int) cmbRegion.ActiveRow.Cells["StateID"].Value;
                }
                else
                    // clear parent list
                    cmbState.Value = null;
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void cmbState_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbState.ActiveRow != null && (
                                                      supplierSet.Supplier[0].IsStateIDNull() ||
                                                      (int) cmbState.ActiveRow.Cells["StateID"].Value !=
                                                      supplierSet.Supplier[0].StateID))
                {
                    // push change to dataset
                    supplierSet.Supplier[0].StateID = (int) cmbState.ActiveRow.Cells["StateID"].Value;
                    // auto-fill parent list
                    cmbCountry.Value = (int) cmbState.ActiveRow.Cells["CountryID"].Value;
                }
                else
                    // clear parent list
                    cmbCountry.Value = null;
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void cmbCountry_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCountry.ActiveRow != null && (
                                                        supplierSet.Supplier[0].IsCountryIDNull() ||
                                                        (int) cmbCountry.ActiveRow.Cells["CountryID"].Value !=
                                                        supplierSet.Supplier[0].CountryID))
                {
                    // push change to dataset
                    supplierSet.Supplier[0].CountryID = (int) cmbCountry.ActiveRow.Cells["CountryID"].Value;
                }
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void btnSupplierEmail_Click(object sender, EventArgs e)
        {
            Process.Start("mailto:" + txtSupplierEmail.Text.Replace("mailto:", ""));
        }

        private void btnSupplierWebsite_Click(object sender, EventArgs e)
        {
            App.OpenUrl(txtSupplierWebsite.Text);
        }

        private void chkIsSupplierActive_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsSupplierActive.Checked)
                {
                    chkIsSupplierActive.ForeColor = Color.Black;
                    if (MenuNode != null)
                        MenuNode.Override.NodeAppearance.FontData.Strikeout = DefaultableBoolean.False;
                }
                else
                {
                    chkIsSupplierActive.ForeColor = Color.Red;
                    if (MenuNode != null)
                        MenuNode.Override.NodeAppearance.FontData.Strikeout = DefaultableBoolean.True;
                }
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void DataTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            SetDataDirtyName();
        }

        private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            SetDataDirtyName();
        }

        #endregion

        #region Supplier info

        private void gridNotes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        { 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "Note")
                {
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
            }
            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Bands[0].ColHeadersVisible = false;
        }

        private void gridNotes_Leave(object sender, EventArgs e)
        {
            gridNotes.ActiveRow = null;
        }

        private void btnNoteAdd_Click(object sender, EventArgs e)
        {
            SupplierSet.SupplierNoteRow row = supplierSet.SupplierNote.NewSupplierNoteRow();
            row.SupplierID = supplierSet.Supplier[0].SupplierID;
            row.Note = "Enter note here...";
            supplierSet.SupplierNote.AddSupplierNoteRow(row);

            GridHelper.SetActiveRow(gridNotes, "SupplierNoteID", row.SupplierNoteID, "Note");
        }

        private void btnNoteDelete_Click(object sender, EventArgs e)
        {
            if (gridNotes.ActiveRow == null)
                return;

            if (App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridNotes, true);

        }
        

        private void txtDefaultCheckin_Enter(object sender, EventArgs e)
        {
            txtDefaultCheckin.SelectAll();
        }

        private void txtDefaultCheckin_Click(object sender, EventArgs e)
        {
            txtDefaultCheckin.SelectAll();
        }

        private void txtDefaultCheckout_Enter(object sender, EventArgs e)
        {
            txtDefaultCheckout.SelectAll();
        }

        private void txtDefaultCheckout_Click(object sender, EventArgs e)
        {
            txtDefaultCheckout.SelectAll();
        }

        private void txtDefaultMargin_Click(object sender, EventArgs e)
        {
            txtDefaultMargin.SelectAll();
        }

        private void txtDefaultMargin_Enter(object sender, EventArgs e)
        {
            txtDefaultMargin.SelectAll();
        }


        private void treeSupplierConfigs_Load()
        {
            try
            {
                treeSupplierConfigs.Override.Sort = SortType.Default;
                treeSupplierConfigs.Nodes.Clear();

                foreach (ToolSet.SupplierConfigTypeRow r in toolSet.SupplierConfigType)
                {
                    UltraTreeNode node = new UltraTreeNode(r.SupplierConfigTypeID.ToString(), r.SupplierConfigTypeName);
                    node.CheckedState = supplierSet.SupplierConfig.FindBySupplierIDSupplierConfigTypeID(
                                            supplierSet.Supplier[0].SupplierID, r.SupplierConfigTypeID) == null
                                            ? CheckState.Unchecked
                                            : CheckState.Checked;

                    treeSupplierConfigs.Nodes.Add(node);
                }
                treeSupplierConfigs.Override.Sort = SortType.Ascending;
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void treeSupplierConfigs_AfterCheck(object sender, NodeEventArgs e)
        {
            // save change to underlying datasource

            int configID = int.Parse(e.TreeNode.Key);
            bool accept = (e.TreeNode.CheckedState == CheckState.Checked);

            CurrencyManager cm = BindingContext[supplierSet, "Supplier"] as CurrencyManager;
            if (cm != null && cm.Position > -1)
            {
                DataRowView drv = cm.Current as DataRowView;
                if (drv == null)
                    return;

                SupplierSet.SupplierConfigRow r =
                    supplierSet.SupplierConfig.FindBySupplierIDSupplierConfigTypeID((int) drv["SupplierID"], configID);

                if (accept)
                {
                    if (r == null)
                        supplierSet.SupplierConfig.AddSupplierConfigRow(
                            (SupplierSet.SupplierRow) drv.Row,
                            configID, "",
                            DateTime.Now,
                            Cache.User.UserID,
                            null);
                }
                else r.Delete();
            }
        }


        private void treeCreditCards_Load()
        {
            try
            {
                treeCreditCards.Nodes.Clear();
                treeCreditCards.Override.Sort = SortType.Default;

                foreach (ToolSet.CreditCardRow r in toolSet.CreditCard.Rows)
                {
                    UltraTreeNode node = new UltraTreeNode(r.CreditCardID.ToString(), r.CreditCardName);
                    node.CheckedState = supplierSet.SupplierCreditCard.FindBySupplierIDCreditCardID(
                                            supplierSet.Supplier[0].SupplierID, r.CreditCardID) == null
                                            ?
                                        CheckState.Unchecked
                                            : CheckState.Checked;

                    treeCreditCards.Nodes.Add(node);
                }
                treeCreditCards.Override.Sort = SortType.Ascending;
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void treeCreditCards_AfterCheck(object sender, NodeEventArgs e)
        {
            try
            {
                int contactID = int.Parse(e.TreeNode.Key);
                bool accept = (e.TreeNode.CheckedState == CheckState.Checked);

                SupplierSet.SupplierCreditCardRow r =
                    supplierSet.SupplierCreditCard.FindBySupplierIDCreditCardID(
                        supplierSet.Supplier[0].SupplierID, contactID);

                if (accept)
                {
                    if (r == null) // add if not already exists
                        supplierSet.SupplierCreditCard.AddSupplierCreditCardRow(
                            supplierSet.Supplier[0],
                            contactID,
                            DateTime.Now,
                            Cache.User.UserID,
                            null);
                }
                else
                    r.Delete();
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }


        private void btnSetupCreditCards_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                App.MainForm.Load_SetupForm("DataLists");
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void btnSetSupplierConfigs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                App.MainForm.Load_SetupForm("SupplierConfigs");
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        #endregion

        #region Reports

        private void grpReports_VisibleChanged(object sender, EventArgs e)
        {
            // Load default report when first becomes visible
            if (grpReports.Visible && supplierSet != null && optReports.CheckedIndex == -1)
                optReports.CheckedIndex = 0;
        }

        private void optReport_ValueChanged(object sender, EventArgs e)
        {
            object[] args = null;

            switch (optReports.Value.ToString())
            {
                case "WhoUsedSupplier":
                    {
                        args = new object[] { supplierSet.Supplier[0].SupplierID, supplierSet.Supplier[0].SupplierName };
                        break;
                    }
            }
            grpReports.Text = optReports.Text;
            LoadReportControl(optReports.Value.ToString(), args);
        }

        internal void LoadReportControl(string controlToLoad, object[] args)
        {
            // load new control into pnlMain form
            if (controlToLoad.Trim() != "")
            {
                Cursor = Cursors.WaitCursor;

                // clear current bindings
                App.ClearBindings(grpReports);

                // clear current control
                grpReports.Controls.Clear();

                // load the controls dynamically based on what was clicked, check the Key property of the node
                try
                {
                    // since the form is not in the current MdiChildren collection, load it up using reflection
                    Assembly asm = Assembly.LoadFrom(Assembly.GetExecutingAssembly().CodeBase);
                    Type t = asm.GetType("TourWriter.Modules.SupplierModule.ReportControls." + controlToLoad);
                    UserControl uc = (UserControl)Activator.CreateInstance(t, args);
                    uc.Tag = this;
                    uc.Dock = DockStyle.Fill;
                    grpReports.Controls.Add(uc);
                }
                catch (Exception ex)
                {
                    App.Error(ex);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            if (txtTemplateFile.Text.Length == 0)
            {
                App.ShowError("Please select a template file first.");
                return;
            }

            string saveFile = App.PromptSaveFile("supplier_export.xls", "Excel", ".xls", false);
            if (String.IsNullOrEmpty(saveFile))
                return;

            ExportToExcel export = new ExportToExcel(supplierSet);
            export.Export(txtTemplateFile.Text, saveFile);
        }

        private void btnChooseTemplate_Click(object sender, EventArgs e)
        {
            txtTemplateFile.Text = App.SelectExternalFile(false, "Select Excel template file", "Excel (*.xls)|*.xls", 0) ?? String.Empty;
        }

        #endregion

        #region Messages

        private void gridMessages_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "MessageType")
                {
                    c.Width = 30;
                    c.Header.Caption = "Type";
                    c.Header.ToolTipText = "Type of message";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MessageName")
                {
                    c.Width = 80;
                    c.Header.Caption = "Description";
                    c.Header.ToolTipText = "Message description";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MessageTo")
                {
                    c.Width = 60;
                    c.Header.Caption = "To";
                    c.Header.ToolTipText = "Message to";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MessageFrom")
                {
                    c.Width = 60;
                    c.Header.Caption = "From";
                    c.Header.ToolTipText = "Message from";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MessageFile")
                {
                    c.Width = 80;
                    c.Header.Caption = "File link";
                    c.Header.ToolTipText = "Saved file (doubleclick row to open)";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "AddedOn")
                {
                    c.Width = 60;
                    c.Header.Caption = "Date";
                    c.Header.ToolTipText = "Date message record was added";
                    c.Format = "g";
                    c.Band.SortedColumns.Add(c, true);
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else
                    c.Hidden = true;
            }

            int index = 0;
            e.Layout.Bands[0].Columns["MessageType"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["MessageName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["MessageTo"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["MessageFrom"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["MessageFile"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["AddedOn"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void gridMessages_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow))
                return;

            Cursor = Cursors.WaitCursor;
            
            string file = ExternalFilesHelper.ConvertToAbsolutePath(e.Row.Cells["MessageFile"].Value.ToString());

            if (!File.Exists(file))
                App.ShowFileNotFound(file);
            else
                Process.Start(file);
            
            Cursor = Cursors.Default;
        }

        private void btnMessagesAdd_Click(object sender, EventArgs e)
        {
            // add new row
            SupplierSet.MessageRow r = supplierSet.Message.NewMessageRow();
            r.MessageName = App.CreateUniqueNameValue(gridMessages.Rows, "MessageName", "New Message");
            r.AddedOn = DateTime.Now;
            r.AddedBy = Cache.User.UserID;
            supplierSet.Message.AddMessageRow(r);

            supplierSet.SupplierMessage.AddSupplierMessageRow(
                supplierSet.Supplier[0], r, DateTime.Now, Cache.User.UserID);

            GridHelper.SetActiveRow(gridMessages, "MessageID", r.MessageID, "MessageName");
        }

        private void btnMessageDel_Click(object sender, EventArgs e)
        {
            UltraGridRow row;
            row = gridMessages.ActiveRow;
            if (row != null && App.AskDeleteRow())
            {
                // delete
                int i = row.Index;
                row.Delete(false);
                // select next row
                if ((i > gridMessages.Rows.Count - 1 ? --i : i) > -1)
                    gridMessages.ActiveRow = gridMessages.Rows[i];
            }
        }

        #endregion

        #region Publishing

        private void gridPublishing_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.Add("Choose");

            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "SupplierTextName")
                {
                    c.Width = 100;
                    c.CellActivation = Activation.AllowEdit;
                    c.Header.Caption = "Description";
                    c.Header.ToolTipText = "Description of link to file";
                    c.Band.SortedColumns.Add(c, false);
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "FileName")
                {
                    c.Width = 160;
                    c.CellActivation = Activation.AllowEdit;
                    c.Header.Caption = "File";
                    c.Header.ToolTipText = "File path and name (doubleclick to open)";
                    c.CellMultiLine = DefaultableBoolean.True;
                }
                else if (c.Key == "Choose")
                {
                    c.Width = 35;
                    c.MinWidth = 35;
                    c.MaxWidth = 35;
                    c.Header.Caption = "";
                    c.Header.ToolTipText = "Browse for linked file";
                    c.Style = ColumnStyle.Button;
                    c.CellButtonAppearance.Image = TourWriter.Properties.Resources.FolderExplore;
                    c.CellButtonAppearance.ImageHAlign = HAlign.Center;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                }
                else
                    c.Hidden = true;
            }
            int index = 0;
            e.Layout.Bands[0].Columns["SupplierTextName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["FileName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Choose"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Bands[0].Override.SelectTypeRow = SelectType.Extended;
        }

        private void gridPublishing_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow))
                return;

            Cursor = Cursors.WaitCursor;
            
            string fileName = ExternalFilesHelper.ConvertToAbsolutePath(e.Row.Cells["FileName"].Value.ToString());

            if (new FileInfo(fileName).Exists)
                Process.Start(fileName);
            else
                App.ShowFileNotFound(fileName);

            Cursor = Cursors.Default;
        }

        private void gridPublishing_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "Choose")
            {
                string fileFilter = "Word objects (*.doc;*.docx;*.rtf;*.jpg;*.jpeg;*.gif;*.png;*.bmp)|" +
                    "*.doc;*.docx;*.rtf;*.jpg;*.jpeg;*.gif;*.png;*.bmp|All files (*.*)|*.*";
                string file = App.SelectExternalFile(true, "Choose file", fileFilter, 1);
                if (file != null)
                    gridPublishing.ActiveRow.Cells["FileName"].Value = file;
            }
        }

        private void btnPublishingAdd_Click(object sender, EventArgs e)
        {
            const string fileFilter = "Word objects (*.doc;*.docx;*.rtf;*.jpg;*.jpeg;*.gif;*.png;*.bmp)|" +
                                      "*.doc;*.docx;*.rtf;*.jpg;*.jpeg;*.gif;*.png;*.bmp|All files (*.*)|*.*";
            var files = App.SelectExternalFiles(true, "Choose file", fileFilter, 1);
            if (files == null) return;

            SupplierSet.SupplierTextRow row = null;
            foreach (var file in files)
            {
                row = supplierSet.SupplierText.NewSupplierTextRow();
                row.SupplierID = supplierSet.Supplier[0].SupplierID;
                row.SupplierTextName = App.CreateUniqueNameValue(
                    gridPublishing.Rows, "SupplierTextName", "New Publishing Text");
                row.FileName = file;
                row.AddedOn = DateTime.Now;
                row.AddedBy = Cache.User.UserID;
                supplierSet.SupplierText.AddSupplierTextRow(row);
            }
            if(row != null)
                GridHelper.SetActiveRow(gridPublishing, "SupplierTextID", row.SupplierTextID, "SupplierTextName");
        }

        private void btnPublishingDel_Click(object sender, EventArgs e)
        {
            GridHelper.DeleteSelectedRows(gridPublishing, true);
        }

        #endregion

        #region Contacts

        private void gridContact_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.Insert(0, "Edit");

            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "Description")
                {
                    c.Header.Caption = "Description";
                    c.Header.ToolTipText = "Description for contact";
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.VertScrollBar = true;
                }
                else if (c.Key == "IsDefaultContact")
                {
                    c.Width = 50;
                    c.MinWidth = 50;
                    c.MaxWidth = 50;
                    c.Header.Caption = "Contact";
                    c.Header.ToolTipText = "Set the default contact in the list";
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "IsDefaultBilling")
                {
                    c.Width = 50;
                    c.MinWidth = 50;
                    c.MaxWidth = 50;
                    c.Header.Caption = "Billing";
                    c.Header.ToolTipText = "Set the default billing contact in the list";
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "Edit")
                {
                    c.Width = 35;
                    c.MinWidth = 35;
                    c.MaxWidth = 35;
                    c.Header.Caption = "More";
                    c.Header.ToolTipText = "Full contact details (doubleclick row to open)";
                    c.Style = ColumnStyle.Button;
                    c.CellButtonAppearance.Image = TourWriter.Properties.Resources.PageEdit;
                    c.CellButtonAppearance.ImageHAlign = HAlign.Center;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                }
                else
                    c.Hidden = true;
            }

            int index = 0;
            e.Layout.Bands[0].Columns["Description"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["IsDefaultContact"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["IsDefaultBilling"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Edit"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void gridContact_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() != typeof(UltraGridEmptyRow))
                EditContactDetails((int)e.Row.Cells["ContactID"].Value);
        }

        private void gridContact_ClickCellButton(object sender, CellEventArgs e)
        {
            EditContactDetails((int)e.Cell.Row.Cells["ContactID"].Value);
        }

        private void btnContactAdd_Click(object sender, EventArgs e)
        {
            // open contact dialog and create new contact row
            ContactMain contact = new ContactMain();

            if (contact.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // get new Contact row
                    ContactSet.ContactRow c = (ContactSet.ContactRow)contact.ContactRow;

                    // initialise new SupplierContact row
                    SupplierSet.SupplierContactRow sc = supplierSet.SupplierContact.NewSupplierContactRow();
                    sc.ContactID = c.ContactID;
                    sc.SupplierID = supplierSet.Supplier[0].SupplierID;
                    sc.Description = c.ContactName;
                    bool isFirstRow = (supplierSet.SupplierContact.Rows.Count == 0);
                    sc.IsDefaultContact = isFirstRow;
                    sc.IsDefaultBilling = isFirstRow;
                    sc.AddedOn = DateTime.Now;
                    sc.AddedBy = Cache.User.UserID;

                    // add the row
                    supplierSet.Contact.ImportRow(c);
                    supplierSet.SupplierContact.AddSupplierContactRow(sc);

                    GridHelper.SetActiveRow(gridContact, "ContactID", sc.ContactID, "Description");
                }
                catch (Exception ex)
                {
                    App.Error(ex);
                }
            }
            contact.Dispose();
        }

        private void btnContactDel_Click(object sender, EventArgs e)
        {
            if (gridContact.ActiveRow == null)
                return;

            if (App.AskDeleteRow())
            {
                supplierSet.Contact.RemoveContactRow(supplierSet.Contact.FindByContactID(
                                                         (int)gridContact.ActiveRow.Cells["ContactID"].Value));

                GridHelper.DeleteActiveRow(gridContact, true);
            }
        }

        private void btnContactEdit_Click(object sender, EventArgs e)
        {
            if(gridContact.ActiveRow != null)
                EditContactDetails((int)gridContact.ActiveRow.Cells["ContactID"].Value);
        }

        #endregion

        #region Helper methods

        private void SupplierSet_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            SetDataDirtyName();
        }

        private void SupplierSet_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            SetDataDirtyName();
        }

        internal void Supplier_Rename(string newName)
        {
            supplierSet.Supplier[0].SupplierName = newName;
        }

        private void OpenHelp()
        {
            // Add main tab name.
            string id = "Supplier." + TabControl_Main.SelectedTab.Text;

            // Add additional tab name.
            if (TabControl_Main.SelectedTab.Key == "Additional")
                id += "." + tabsAdditional.SelectedTab.Text;
            
            App.ShowHelp(id);
        }

        #endregion

        #region Menu events

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveDataChanges();
        }

        private void menuRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuDebugSupplier_Click(object sender, EventArgs e)
        {
            App.ViewDataSet(supplierSet);
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            OpenHelp();
        }

        #endregion

        #region PaymentTerms
        private void btnPaymentTerms_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PaymentTermsEditor termsEditor = new PaymentTermsEditor();
            InitializePaymentTermsEditor(termsEditor);
            OpenPaymentTermsEditor(termsEditor);
            UpdatePaymentTermCustomText();
        }

        private void UpdatePaymentTermCustomText()
        {
            if (supplierSet.Supplier[0].IsPaymentTermIDNull())
            {
                txtPaymentTerms.Text = String.Empty;
                return;
            }

            int id = supplierSet.Supplier[0].PaymentTermID;
            SupplierSet.PaymentTermRow row = supplierSet.PaymentTerm.FindByPaymentTermID(id);
            if (row != null) txtPaymentTerms.Text = row.GetCustomText(Cache.ToolSet.PaymentDue);
        }

        /// <summary>
        /// Initializes the values in a PaymentTermsEditor
        /// </summary>
        /// <param name="termsEditor">The PaymentTermsEditor to initialize</param>
        private void InitializePaymentTermsEditor(PaymentTermsEditor termsEditor)
        {
            int? paymentTermID = (!supplierSet.Supplier[0].IsPaymentTermIDNull())
                                 ? (int?)supplierSet.Supplier[0].PaymentTermID
                                 : null;

            // if there's no existing terms then don't bother
            bool hasExistingTerms = (paymentTermID != null);
            if (hasExistingTerms)
            {
                SupplierSet.PaymentTermRow paymentTermRow
                    = supplierSet.PaymentTerm.FindByPaymentTermID((int)paymentTermID);

                // fill in payment terms
                termsEditor.PaymentDueID = paymentTermRow.PaymentDueID;
                termsEditor.PaymentDuePeriod = (!paymentTermRow.IsPaymentDuePeriodNull())
                                               ? (int?)paymentTermRow.PaymentDuePeriod
                                               : null;

                if (!paymentTermRow.IsDepositAmountNull())
                {
                    // fill in deposit info
                    termsEditor.DepositRequired = true;
                    termsEditor.DepositAmount = paymentTermRow.DepositAmount;
                    termsEditor.DepositType = paymentTermRow.DepositType;
                    termsEditor.DepositDueID = paymentTermRow.DepositDueID;
                    termsEditor.DepositDuePeriod = (!paymentTermRow.IsDepositDuePeriodNull())
                                                   ? (int?)paymentTermRow.DepositDuePeriod
                                                   : null;
                }
            }
        }

        /// <summary>
        /// Opens a PaymentTermsEditor and if OK is clicked, it will either add
        /// a new PaymentTermRow to the table, or update the existing one.
        /// </summary>
        /// <param name="termsEditor">The PaymentTermsEditor to open.</param>
        private void OpenPaymentTermsEditor(PaymentTermsEditor termsEditor)
        {
            SupplierSet.PaymentTermRow paymentTermRow;
            int? paymentTermID = (!supplierSet.Supplier[0].IsPaymentTermIDNull())
                                 ? (int?)supplierSet.Supplier[0].PaymentTermID
                                 : null;

            // create a new row if there's no existing terms, otherwise use the existing row
            bool hasExistingTerms = (paymentTermID != null);
            if (hasExistingTerms)
            {
                paymentTermRow = supplierSet.PaymentTerm.FindByPaymentTermID((int)paymentTermID);
            }
            else
            {
                paymentTermRow = supplierSet.PaymentTerm.NewPaymentTermRow();
            }

            DialogResult result = termsEditor.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (termsEditor.DeleteTerms)
                {
                    paymentTermRow.Delete();
                }
                else
                {
                    // dump all the values from the editor into the row
                    paymentTermRow.PaymentDueID = (int)termsEditor.PaymentDueID;

                    if (termsEditor.PaymentDuePeriod.HasValue)
                        paymentTermRow.PaymentDuePeriod = (int)termsEditor.PaymentDuePeriod;
                    else
                        paymentTermRow.SetPaymentDuePeriodNull();

                    if (termsEditor.DepositRequired)
                    {
                        // fill in deposit fields
                        paymentTermRow.DepositAmount = (decimal)termsEditor.DepositAmount;
                        paymentTermRow.DepositType = (char)termsEditor.DepositType;
                        paymentTermRow.DepositDueID = (int)termsEditor.DepositDueID;

                        if (termsEditor.DepositDuePeriod.HasValue)
                            paymentTermRow.DepositDuePeriod = (int)termsEditor.DepositDuePeriod;
                        else
                            paymentTermRow.SetDepositDuePeriodNull();
                    }
                    else
                    {
                        // deposit not required, null all deposit related fields
                        paymentTermRow.SetDepositAmountNull();
                        paymentTermRow.SetDepositTypeNull();
                        paymentTermRow.SetDepositDueIDNull();
                        paymentTermRow.SetDepositDuePeriodNull();
                    }

                    if (!hasExistingTerms)
                    {
                        // add the row to the table
                        supplierSet.PaymentTerm.AddPaymentTermRow(paymentTermRow);
                        supplierSet.Supplier[0].PaymentTermID = paymentTermRow.PaymentTermID;
                    }
                }
            }
        }

        #endregion

        #region Accounting

        private void tabControl_Main_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Key == "Additional")
            {
                if (tabsAdditional.SelectedTab.Key == "Accounting")
                {
                    RefreshAccounting();
                }
            }
        }

        private void tabsAdditional_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Key == "Accounting")
            {
                RefreshAccounting();
            }
        }

        #endregion
    }
}
