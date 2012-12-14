using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.BusinessLogic;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Modules.ItineraryModule.Bookings;
using TourWriter.Modules.ItineraryModule.DateKicker;
using TourWriter.Services;
using CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction;
using SupplierMessage = TourWriter.Reports.Itinerary.SupplierMessage;
using Infragistics.Win.UltraWinTabControl;

namespace TourWriter.Modules.ItineraryModule
{
    public partial class ItineraryMain : ModuleBase
    {
        #region Properties

        internal ItinerarySet ItinerarySet
        {
            get { return itinerarySet; }
        }

        private bool serverConnectionError_UserNotified = false;

        private bool isReadOnlyChecked
        {
            get { return (chkReadOnly.CheckState == CheckState.Checked); }
        }

        private AgentSet agentSetStore;

        private AgentSet agentSet
        {
            get
            {
                if (itinerarySet.Itinerary[0].IsAgentIDNull())
                {
                    agentSetStore = null;
                }
                else if (agentSetStore == null ||
                         agentSetStore.Agent.Count == 0 ||
                         agentSetStore.Agent[0].AgentID != itinerarySet.Itinerary[0].AgentID)
                {
                    Agent a = new Agent();
                    agentSetStore = a.GetAgentSet(itinerarySet.Itinerary[0].AgentID);
                }
                return agentSetStore;
            }
        }

        public BookingsViewer BookingsViewer
        {
            get { return bookingsViewer; }
        }

        internal QuoteTable GetRefreshQuoteTable
        {
            get { return bookingsQuote.GetRefreshedQuoteTable(); }
        }


        public BindingSource CurrencyBindingSource { get { return currencyBindingSource; } }

        #endregion

        public ItineraryMain()
        {
            InitializeComponent();
            displayTypeName = "Itinerary";

            // Hide menus, they are merged with parent form
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;

            txtArriveDate.Value = DateTime.Now;
            txtDepartDate.Value = DateTime.Now;

            ApplyLicenseStatus();
            if (!App.ShowOldReports) tabControlAdditional.Tabs["Reports"].Visible = false;

            tabControlAdditional.SelectedTabChanging += tabControlAdditional_SelectedTabChanging;
        }

        public void ApplyLicenseStatus()
        {
            if (IsReadOnly)
            {
                menuSave.Enabled = false;
                toolSave.Enabled = false;
                toolSaveClose.Enabled = false;
                menuRefresh.Enabled = false;
                toolRefresh.Enabled = false;
            }
        }
        
        private void RefreshAccounting()
        {
            //CommitOpenEdits();

            //if (accounting1.RefreshRequired || accounting1.SaveRequired)
            //{
            //    accounting1.RefreshDataAndControls();
            //}
        }

        private void DataBind()
        {
            cityBindingSource1.DataSource = toolSet.City.DefaultView;
            cityBindingSource2.DataSource = toolSet.City.DefaultView;
            agentBindingSource.DataSource = toolSet.Agent.DefaultView;

            toolSet.City.DefaultView.Sort = "CityName ASC";
            countryBindingSource.Sort = "CountryName ASC";
            userBindingSource.Sort = "UserName ASC";
            userBindingSource.Filter = "IsRecordActive <> 1";
            agentBindingSource.Sort = "AgentName ASC";
        }
        
        private void ItineraryLoad(object sender, EventArgs e)
        {
            DataBind();

            // WIP - TOOLBARS: uncomment to see behaviour
            // return; 

            LoadData();

            if (itinerarySet.Itinerary.Count == 0)
            {
                if (!serverConnectionError_UserNotified)
                    App.ShowError("Failed to load Itinerary from database.");

                foreach (Control c in Controls)
                    c.Enabled = false; // disable top level controls
                return;
            }
            optReport.CheckedIndex = 0;
            setAddedByInfo();

            // set report sql params
            if (reportControl.GeneralParameters.ContainsKey("@ItineraryID")) reportControl.GeneralParameters.Remove("@ItineraryID");
            reportControl.GeneralParameters.Add("@ItineraryID", itinerarySet.Itinerary[0].ItineraryID);
            if (reportControl.GeneralParameters.ContainsKey("@PurchaseLineIDList")) reportControl.GeneralParameters.Remove("@PurchaseLineIDList");
            reportControl.GeneralParameters.Add("@PurchaseLineIDList", itinerarySet.PurchaseLine);
            reportControl.PoplulateReportExplorer(UserControls.Reports.ExplorerControl.ReportCategory.Itinerary);

            // bind user controls
            bookingsViewer.ItinerarySet = itinerarySet;
            bookingsViewer.SetBindingContext(BindingContext);
            bookingsQuote.ItinerarySet = itinerarySet;
            clientEditor.ItinerarySet = itinerarySet;            
            publisher1.ItinerarySet = itinerarySet;
            accounting1.ItinerarySet = itinerarySet;
            itineraryTasks1.ItinerarySet = itinerarySet;

            // add event to track when data changed
            foreach (DataTable dt in itinerarySet.Tables)
            {
                dt.ColumnChanged += ItinerarySet_ColumnChanged;
                dt.RowDeleted += ItinerarySet_RowDeleted;
            }
            // reset progress bar
            ProgressBar.Value = ProgressBar.Minimum;
            Application.DoEvents();

            toolSave.Enabled = toolSave.Enabled && !isReadOnlyChecked;
            toolSaveClose.Enabled = toolSaveClose.Enabled && !isReadOnlyChecked;
            menuSave.Enabled = menuSave.Enabled && !isReadOnlyChecked;
            toolRefresh.Enabled = toolRefresh.Enabled && !isReadOnlyChecked;
            menuRefresh.Enabled = menuRefresh.Enabled && !isReadOnlyChecked;

            RefreshAgentContactBindings();

            SetEventHandlers(true);

            // set permissions for read-only
            if (!AppPermissions.UserHasPermission(AppPermissions.Permissions.ItineraryReadOnly))
                chkReadOnly.Enabled = false;

            Cache.ToolSet.TemplateCategory.RowChanged += DataTable_RowChanged;
            Cache.ToolSet.TemplateCategory.ColumnChanged += DataTable_ColumnChanged;
            Cache.ToolSet.Template.RowChanged += DataTable_RowChanged;
            Cache.ToolSet.Template.ColumnChanged += DataTable_ColumnChanged;
            bookingsViewer.RecalculateFinalPricing();
            bookingsViewer.SetItineraryCurrencyInfo();

            //pnlAllocations.Visible = App.ShowItineraryAllocations;
            if (App.ShowItineraryAllocations)
            {
                pnlAllocations.Visible = true;
                if (!itinerarySet.Itinerary[0].IsAllocationsItineraryIDNull())
                {                    
                    lblAllocations.Text = "Master";                   
                    txtAgentAllocation.Text = itinerarySet.Itinerary[0].AllocationsItineraryID.ToString();                   
                    btnAgentAllocation.Image = Properties.Resources.Remove;
                }
                else
                {
                    GetAllocatedAgents();
                }              
            }        
        }
        
        private void ItineraryMain_Shown(object sender, EventArgs e)
        {
            if (chkReadOnly.CheckState == CheckState.Checked)
            {
                Application.DoEvents();
                App.ShowInfo("Itinerary is read-only.\r\n\r\nYou cannot save changes, Save button is disabled.");
            }
        }

        private void LoadData()
        {
            // WIP - TOOLBARS: load done here
            // WIP - TOOLBARS: merge done here

            ItinerarySet ds = new ItinerarySet();
            try
            {
                toolSet.Merge(Cache.ToolSet, false);
                toolSet.Agent.PopulateAgentNameParentName();

                // make cities sort alphabetically
                toolSet.DefaultViewManager.DataViewSettings[toolSet.City].Sort = "CityName ASC";

                foreach (DataTable dt in ds.Tables)
                    dt.RowChanged += ItinerarySet_RowChanged;

                // start progress
                ProgressBar.Minimum = 1;
                ProgressBar.Maximum = ds.Tables.Count;
                ProgressBar.Value = 1;

                // load and merge to retain designer bindings
                NavigationTreeItemInfo navigationTreeItem = MenuNode.Tag as NavigationTreeItemInfo;
                if (navigationTreeItem != null)
                {
                    int id = navigationTreeItem.ItemID;
                    Itinerary i = new Itinerary();

                    ds.EnforceConstraints = false;
                    i.GetItinerarySet(id, ds);

                    TestForPaymentTermsContraintError(ds);
                    ds.EnforceConstraints = true;

                    itinerarySet.Merge(ds, false);
                    
                    // refresh menu node (new itins might have name set from custom code in db when created)
                    MenuNode.Visible = true;
                    if (itinerarySet.Itinerary.Count > 0 && MenuNode.Text != itinerarySet.Itinerary[0].ItineraryName)
                        MenuNode.Text = itinerarySet.Itinerary[0].ItineraryName; 
                }

                cmbCurrency.Visible = false;
                lblCurrency.Visible = false;

                // end progress
                ProgressBar.Value = ProgressBar.Maximum;
                Application.DoEvents();
            }
            catch (ConstraintException ex)
            {
                var stream1 = new MemoryStream();
                itinerarySet.WriteXml(stream1, XmlWriteMode.DiffGram);
                stream1.Position = 0;
                var attach1 = new System.Net.Mail.Attachment(stream1, "ds1.xml", System.Net.Mime.MediaTypeNames.Text.Xml);

                var errors = "";
                foreach (var table in ds.Tables.Cast<DataTable>())
                    foreach (var error in table.GetErrors())
                        errors += "Table '" + table.TableName + "', " + error.RowError;

                if (errors != "")
                {
                    errors = "ItinerarySet: " + errors;
                }
                else // or was error in the ToolSet?
                {
                    foreach (var table in toolSet.Tables.Cast<DataTable>())
                        foreach (var error in table.GetErrors())
                            errors += "Table '" + table.TableName + "', " + error.RowError;
                    if (errors != "") errors = "ToolSet: " + errors;
                }

                ErrorHelper.SendEmail(ex.Message + errors, ex.ToString(), true, attach1);
                throw new ConstraintException(errors, ex);
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

        /// <summary>
        /// TODO: negative paymemts terms sometimes get saved to PurchaseItem table, why?
        /// This sets them to null before enforcing constraints. Saving the Itinerary will update then in the database too.
        /// </summary>
        /// <param name="ds"></param>
        private static void TestForPaymentTermsContraintError(DataSet ds)
        {
            try
            {
                ds.EnforceConstraints = true;
            }
            catch (ConstraintException ex)
            {
                var stream1 = new MemoryStream();
                ds.WriteXml(stream1, XmlWriteMode.DiffGram);
                stream1.Position = 0;
                var attach1 = new System.Net.Mail.Attachment(stream1, "ds1.xml", System.Net.Mime.MediaTypeNames.Text.Xml);
                var errors = "";
                foreach (var table in ds.Tables.Cast<DataTable>())
                    foreach (var error in table.GetErrors())
                        errors += "Table '" + table.TableName + "', " + error.RowError;
                ErrorHelper.SendEmail("(Note: silent PaymentTerms constraint error) " + ex.Message + errors, ex.ToString(), true, attach1);
                // TODO: fix negative payment terms, but why is this happening?
                foreach (var row in ((ItinerarySet)ds).PurchaseItem.
                    Where(x => !x.IsPaymentTermIDNull() && x.PaymentTermID < 0)) // invalid payment term id
                        row.SetPaymentTermIDNull(); // reset
                ds.EnforceConstraints = true;
            }
        }

        public bool IsSaveRequired()
        {
            return IsDataDirty();
        }

        public void SaveChanges()
        {
            SaveDataChanges();
        }
        
        private void SetReadOnly(bool readOnly)
        {
            if (readOnly)
            {
                string msg = "This will make the itinerary read only and will save any changes you have made.\r\n\r\nDo you want to continue?";
                if (App.AskYesNo(msg))
                {
                    SaveDataChanges();
                    toolSave.Enabled = false;
                    toolSaveClose.Enabled = false;
                }
                else
                {
                    // revert the checkbox
                    chkReadOnly.Checked = false;
                }
            }
            else
            {
                toolSave.Enabled = true;
                toolSaveClose.Enabled = true;
            }
        }

        private void ValidationHandler(object sender, BindingCompleteEventArgs e)
        {
            Control c = e.Binding.BindableComponent as Control;

            if (e.BindingCompleteState != BindingCompleteState.Success && c != null)
            {
                errorProvider1.SetError(c, e.ErrorText); // set error message
                e.Cancel = true; // cancel edit
                c.Focus(); // select text
            }
            else
                errorProvider1.SetError(c, ""); // reset error message 
        }

        private string loadingTable = "";

        private void SetEventHandlers(bool enabled)
        {
            if (enabled)
            {
                chkReadOnly.CheckStateChanged += chkReadOnly_CheckStateChanged;
                cmbAgent.SelectedValueChanged += cmbAgent_SelectedValueChanged;
                cmbAgentContact.SelectedIndexChanged += cmbAgentContact_SelectedIndexChanged;
            }
            else
            {
                chkReadOnly.CheckStateChanged -= chkReadOnly_CheckStateChanged;
                cmbAgent.SelectedValueChanged -= cmbAgent_SelectedValueChanged;
                cmbAgentContact.SelectedIndexChanged -= cmbAgentContact_SelectedIndexChanged;
            }
        }

        private void ItinerarySet_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            // Look for new table being loaded
            if (e.Action == DataRowAction.Commit && e.Row.RowState == DataRowState.Unchanged)
            {
                if (loadingTable != e.Row.Table.TableName)
                {
                    loadingTable = e.Row.Table.TableName;
                    App.Debug("==== Loading table: " + loadingTable);
                    //ProgressBar.Value = ProgressBar.Value + 1;

                    int val = ProgressBar.Value + 1;
                    ProgressBar.Value = val <= ProgressBar.Maximum ? val : ProgressBar.Value;
                    Application.DoEvents();
                }
            }
        }

        #region TODO - thread commenting out
        private void itinerarySetLoaderThread_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadData();
        }

        private void itinerarySetLoaderThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void itinerarySetLoaderThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //// Call method on the UI thread.

            //Invoke(new MethodInvoker(
            //           delegate
            //           {
            //               DisplayData();
            //           }));
        }

        #endregion

        #region Base class overrides

        protected override bool IsDataDirty()
        {
            CommitOpenEdits();
            return (base.Text.StartsWith("*") || (itinerarySet != null && itinerarySet.HasChanges()));
        }

        protected override void CommitOpenEdits()
        {
            if (itinerarySet == null)
                return;

            Validate();

            App.CommitGridEdits(this);

            foreach (DataTable dt in itinerarySet.Tables)
                foreach (DataRow dr in dt.Rows)
                    dr.EndEdit();
        }

        protected override void SaveDataChanges()
        {
            if (!toolSave.Enabled) { App.ShowError("Cannot save this itinerary because it is read only."); return; }
            if (!Validate() || !App.ShowCheckPermission(AppPermissions.Permissions.ItineraryEdit)) return;

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                if (IsDataDirty())
                {
                    try
                    {
                        SetEventHandlers(false); // temporarily detach event handlers

                        var i = new Itinerary();

                        // take a copy of the lookup tables
                        var supplierLookup = itinerarySet.SupplierLookup.Copy();
                        var optionLookup = itinerarySet.OptionLookup.Copy();

                        // Get data changes.
                        if (itinerarySet.HasChanges())
                        {
                            // Save data changes.
                            ItinerarySet fresh = i.SaveItinerarySet(itinerarySet);

                            // merge the lookup tables
                            fresh.SupplierLookup.Merge(supplierLookup, true);
                            fresh.OptionLookup.Merge(optionLookup, true);
                            fresh.SupplierLookup.AcceptChanges();
                            fresh.OptionLookup.AcceptChanges();                          
                            // Handle any errors.
                            if (App.DataSet_CheckForErrors(fresh) && App.DataSet_AskSaveDeleteConstraints(fresh))
                            {
                                // Clear dataset and merge to maintain any databindings
                                itinerarySet.Clear();
                                itinerarySet.Merge(fresh, false);                           
                                // and save again
                                SaveDataChanges();                              
                                return;
                            }
                            // Clear dataset and merge to maintain any databindings                          
                            itinerarySet.Clear();
                            itinerarySet.Merge(fresh, false);
                            bookingsViewer.SetGridExpanded();
                        }
                        // Update main form
                        UpdateMainForm(App.MainForm.ItineraryMenu, itinerarySet.Itinerary[0].IsRecordActive);
                        if (itinerarySet.Itinerary[0].IsAllocationsItineraryIDNull())
                        {
                            GetAllocatedAgents();
                        }
                        SetDataCleanName();                     
                        //accounting1.RefreshRequired = true;
                    }
                    catch (ConstraintException ex)
                    {
                        var stream1 = new MemoryStream();
                        itinerarySet.WriteXml(stream1, XmlWriteMode.DiffGram);
                        stream1.Position = 0;
                        var attach1 = new System.Net.Mail.Attachment(stream1, "ds1.xml", System.Net.Mime.MediaTypeNames.Text.Xml);

                        var errors = "";
                        foreach (var table in itinerarySet.Tables.Cast<DataTable>())
                            foreach (var error in table.GetErrors())
                                errors += "Table '" + table.TableName + "', " + error.RowError;
                        ErrorHelper.SendEmail(ex.Message + errors, ex.ToString(), true, attach1);
                        throw;
                    }
                    finally
                    {
                        SetEventHandlers(true); // re-attach the event handlers
                    }
                }
                // Save the toolset.
                if (Cache.ToolSet != null && Cache.ToolSet.HasChanges())
                {
                    Cache.SaveToolSet();
                    reportControl.RefreshReportExplorer();
                    reportControl.PoplulateReportExplorer(UserControls.Reports.ExplorerControl.ReportCategory.Itinerary);
                    bookingsViewer.RefreshEmailTemplateMenu();
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        protected override void CancelDataChanges()
        {
            return;
        }

        protected override string GetDisplayName()
        {
            if (itinerarySet != null && itinerarySet.Itinerary.Count > 0)
                return itinerarySet.Itinerary[0].ItineraryName;
            return "";
        }

        #endregion

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(SelectedNodesCollection)))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            Object obj;
            NavigationTreeItemInfo nodeInfo;
            SelectedNodesCollection nodeCollection;

            obj = e.Data.GetData(typeof(SelectedNodesCollection));
            nodeCollection = obj as SelectedNodesCollection;
            if (nodeCollection == null)
                return;

            //for (int i = 0; i < nodeCollection.Count; i++)
            if (nodeCollection.Count > 0)
            {
                var i = 0;
                nodeInfo = nodeCollection[i].Tag as NavigationTreeItemInfo;

                if (nodeInfo != null)
                {
                    if (nodeInfo.ItemType == NavigationTreeItemInfo.ItemTypes.Supplier)
                    {
                        // Add supplier booking
                        tabControl_Main.SelectedTab = tabControl_Main.Tabs["Bookings"];
                        //original code: bookingsViewer.AddNewBooking(nodeInfo.ItemID);
                        bookingsViewer.AddNewBooking(nodeInfo.ItemID, this);
                    }
                    else if (nodeInfo.ItemType == NavigationTreeItemInfo.ItemTypes.Contact)
                    {
                        // Add contact
                        tabControl_Main.SelectedTab = tabControl_Main.Tabs["Clients"];
                        clientEditor.AddContact(nodeInfo.ItemID);
                    }
                }
            }
        }

        private void ItinerarySet_Refresh()
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

                // temporarily detach event handlers
                SetEventHandlers(false);

                // refresh itinerary
                Itinerary s = new Itinerary();
                if (itinerarySet.Itinerary.Count == 0)
                {
                    App.ShowError("Itinerary may not have opened, please close and open again");
                    return;
                }
                ItinerarySet fresh = s.GetItinerarySet(itinerarySet.Itinerary[0].ItineraryID, new ItinerarySet());

                // clear current and merge to maintain databindings
                itinerarySet.Clear();
                itinerarySet.Merge(fresh, false);

                // refresh the toolset
                Cache.RefreshToolSet(false);
                reportControl.RefreshReportExplorer();
                reportControl.PoplulateReportExplorer(UserControls.Reports.ExplorerControl.ReportCategory.Itinerary);

                SetDataCleanName();
            }
            finally
            {
                Cursor.Current = Cursors.Default;

                // re-attach the event handlers
                SetEventHandlers(true);
            }
        }

        private void chkReadOnly_CheckStateChanged(object sender, EventArgs e)
        {
            itinerarySet.Itinerary[0].IsReadOnly = isReadOnlyChecked;
            SetReadOnly(isReadOnlyChecked);
        }

        private void DataTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            SetDataDirtyName(); 
        }

        private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            SetDataDirtyName();
        }


        #region Itinerary

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            chkActive.ForeColor = chkActive.Checked ? Color.Black : Color.Red;
        }

        private void setAddedByInfo()
        {
            try
            {
                txtAddedBy.Text = String.Format(
                    "{0}, {1}",
                    toolSet.User.FindByUserID(itinerarySet.Itinerary[0].AddedBy).UserName,
                    itinerarySet.Itinerary[0].AddedOn.ToShortDateString());
            }
            catch
            {
                txtAddedBy.Text = "[error]";
            }
        }

        private void moveArriveDate(DateTime oldDate, DateTime newDate)
        {
            if (oldDate.Date == newDate.Date || // no change
                itinerarySet.Itinerary[0].IsDepartDateNull() && itinerarySet.PurchaseItem.Count == 0) // nothing to change
                return;

            string msg =
                (itinerarySet.PurchaseItem.Count > 0)
                    ? "Move bookings and departure date also (opens rates management screen)?"
                    : "Move departure date also?";
            
            int dayOffset = (newDate.Date - oldDate.Date).Days;
            if (App.AskYesNo(msg))
            {
                // shift depart date
                if (txtDepartDate.Value != null) txtDepartDate.Value = ((DateTime)txtDepartDate.Value).AddDays(dayOffset);

                // check if any bookings to move also (filter out deleted)
                bool hasBookings = false;
                foreach (ItinerarySet.PurchaseItemRow row in itinerarySet.PurchaseItem)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        hasBookings = true;
                        break;
                    }
                }

                // shift bookings
                if (hasBookings)
                {
                    DateKickerForm dateKicker = new DateKickerForm(itinerarySet, dayOffset, true);
                    if(dateKicker.ShowDialog() == DialogResult.OK)
                    {
                        bookingsViewer.RefreshGrid();
                        bookingsViewer.RecalculateFinalPricing();
                    }
                    else // reset dates
                    {
                        txtArriveDate.Value = ((DateTime) txtArriveDate.Value).AddDays(-(dayOffset));
                        if (txtDepartDate.Value != null) txtDepartDate.Value = ((DateTime)txtDepartDate.Value).AddDays(-(dayOffset));
                    }
                }
                // sync datasource
                if (txtDepartDate.Value == null)
                {
                    if (txtArriveDate.Value != null)
                        itinerarySet.Itinerary[0].ArriveDate = (DateTime) txtArriveDate.Value;
                }
                else
                {
                    var arriveDate = (DateTime)txtArriveDate.Value;
                    var departDate = (DateTime)txtDepartDate.Value;

                    if (arriveDate > itinerarySet.Itinerary[0].DepartDate)
                    {
                        itinerarySet.Itinerary[0].DepartDate = departDate;
                        itinerarySet.Itinerary[0].ArriveDate = arriveDate;
                    }
                    else
                    {
                        itinerarySet.Itinerary[0].ArriveDate = arriveDate;
                        itinerarySet.Itinerary[0].DepartDate = departDate; 
                    }
                }
            }
        }

        private void txtArriveDate_Validating(object sender, CancelEventArgs e)
        {
            if (itinerarySet.Itinerary.Count > 0 &&
                !itinerarySet.Itinerary[0].IsArriveDateNull() &&
                txtArriveDate.Value != null)
            {
                moveArriveDate(itinerarySet.Itinerary[0].ArriveDate.Date, (DateTime)txtArriveDate.Value);
            }
        }

        private void txtArriveDate_CloseUp(object sender, EventArgs e)
        {
            Validate();
        }

        private void txtDepartDate_CloseUp(object sender, EventArgs e)
        {
            Validate();
        }
        
        private void txtDepartDate_Enter(object sender, EventArgs e)
        {
            if (txtDepartDate.Value != null) return;

            // default to arrive date
            var binding = txtDepartDate.DataBindings[0];
            txtDepartDate.DataBindings.Clear();
            txtDepartDate.Value = itinerarySet.Itinerary[0].ArriveDate;
            txtDepartDate.DataBindings.Add(binding);
        }
        
        private void tabControlAdditional_SelectedTabChanging(object sender, SelectedTabChangingEventArgs e)
        {
            if (e.Tab.Key == "Accounting")
            {
                if (!AppPermissions.UserHasPermission(AppPermissions.Permissions.AccountingView) &&
                    !AppPermissions.UserHasPermission(AppPermissions.Permissions.AccountingEdit))
                {
                    App.ShowError(App.GetResourceString("ShowPermissionDenied"));
                    e.Cancel = true;
                }
            }
        }
        
        void tabControl_Main_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Key == "Additional")
            {
                if (tabControlAdditional.SelectedTab.Key == "Accounting")
                {
                    RefreshAccounting();
                }
            }
        }

        private void tabControlAdditional_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Key == "Accounting")
            {
                RefreshAccounting();
            }
        }
        
        #region Agent
        private bool HasOverrides()
        {
            foreach (ItinerarySet.ItineraryMarginOverrideRow row in itinerarySet.ItineraryMarginOverride)
            {
                if (row.RowState != DataRowState.Deleted)
                    return true;
            }

            return false;
        }

        private void AutoPopulateNetOverrides()
        {
            var agentRow = Cache.AgentSet.Agent.FindByAgentID(itinerarySet.Itinerary[0].AgentID);
            if (agentRow == null) return;

            // delete all existing override rows
            for (int i = itinerarySet.ItineraryMarginOverride.Rows.Count - 1; i >= 0; i--)
            {
                itinerarySet.ItineraryMarginOverride[i].Delete();
            }

            // add the new override rows
            foreach (ToolSet.ServiceTypeRow row in Cache.ToolSet.ServiceType)
            {
                // get existing margin override row for this service type
                var agentMarginRow = Cache.AgentSet.AgentMargin.FindByAgentIDServiceTypeID(
                    itinerarySet.Itinerary[0].AgentID, row.ServiceTypeID);

                if (agentMarginRow == null && !agentRow.IsParentAgentIDNull())
                {
                    // check the parent agent for overrides
                    agentMarginRow = Cache.AgentSet.AgentMargin.FindByAgentIDServiceTypeID(
                        agentRow.ParentAgentID, row.ServiceTypeID);
                }
                if (agentMarginRow != null)
                {
                    // create new itinerary margin override row for this service type
                    var overrideRow = itinerarySet.ItineraryMarginOverride.NewItineraryMarginOverrideRow();
                    overrideRow.ItineraryID = itinerarySet.Itinerary[0].ItineraryID;
                    overrideRow.ServiceTypeID = row.ServiceTypeID;
                    overrideRow.Margin = agentMarginRow.Margin;
                    
                    itinerarySet.ItineraryMarginOverride.AddItineraryMarginOverrideRow(overrideRow);
                }
            }

            itinerarySet.Itinerary[0].NetComOrMup = (!agentRow.IsNetComOrMupNull()) ? agentRow.NetComOrMup : "mup";
        }
        
        private void RefreshAgentContactBindings()
        {
            if (itinerarySet.Itinerary[0].IsAgentIDNull())
                return;

            var agentId = itinerarySet.Itinerary[0].AgentID;

            var contacts = new Dictionary<string, string> { { "", "" } };
            foreach (var c in Cache.AgentSet.AgentContact.Where(x => x.RowState != DataRowState.Deleted && x.AgentID == agentId).OrderBy(x => x.Description))
                contacts.Add(c.ContactID.ToString(), c.Description);

            cmbAgentContact.ValueMember = "Key";
            cmbAgentContact.DisplayMember = "Value";
            cmbAgentContact.DataSource = new BindingSource(contacts, null);
            cmbAgentContact.SelectedValue = !itinerarySet.Itinerary[0].IsAgentContactIDNull() ? itinerarySet.Itinerary[0].AgentContactID.ToString() : "";
        }

        private void cmbAgent_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!itinerarySet.Itinerary[0].IsAgentIDNull())
                if (cmbAgent.SelectedValue == null || (int) cmbAgent.SelectedValue == itinerarySet.Itinerary[0].AgentID) 
					return;

            int id = (int) cmbAgent.SelectedValue;

            // force dataset value to update
            itinerarySet.Itinerary[0].AgentID = id;

            // reset agent contact
            itinerarySet.Itinerary[0].SetAgentContactIDNull();
            RefreshAgentContactBindings();

            // warn
            var hasOverrides = HasOverrides();
            var hasLocks = itinerarySet.PurchaseItem.Any(x => x.RowState != DataRowState.Deleted && !x.IsIsLockedAccountingNull() && x.IsLockedAccounting);
            if (hasOverrides || hasLocks)
            {
                var msg = "";
                if (hasLocks) msg =
                    "Warning: Itinerary contains locked bookings (exported to Accounting) - changing the Agent could change the pricing of the locked bookings.\r\n\r\n";
                
                msg += "Would you like to re-populate the service type price overrides for this agent?\r\n\r\nWarning: This will remove any existing service type price overrides.";

                if (!App.AskYesNo(msg))
                    return;
            }

            // set overrides
            AutoPopulateNetOverrides();
            bookingsViewer.RecalculateFinalPricing();
        }

        private void cmbAgentContact_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAgentContact.SelectedValue != "")
                itinerarySet.Itinerary[0].AgentContactID = int.Parse(cmbAgentContact.SelectedValue.ToString());
            else
                itinerarySet.Itinerary[0].SetAgentContactIDNull();
        }

        #endregion

        #endregion

        #region Bookings

        private void bookingsViewer_OnOpenBooking(BookingsViewerEditBookingEventArgs e)
        {
            var editor = new BookingEditorForm(itinerarySet,this);
            editor.SetBindingContext(BindingContext);
            editor.SetActiveRows(e.purchaseLineId, e.purchaseItemId);
            editor.ShowDialog();
            
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

            string file = ExternalFilesHelper.ConvertToAbsolutePath(
                e.Row.Cells["MessageFile"].Value.ToString());

            if (!File.Exists(file))
                App.ShowFileNotFound(file);
            else
                Process.Start(file);

            Cursor = Cursors.Default;
        }

        private void btnMessagesAdd_Click(object sender, EventArgs e)
        {
            // add new row
            ItinerarySet.MessageRow r = itinerarySet.Message.NewMessageRow();
            r.MessageName = App.CreateUniqueNameValue(gridMessages.Rows, "MessageName", "New Message");
            r.AddedOn = DateTime.Now;
            r.AddedBy = Cache.User.UserID;
            itinerarySet.Message.AddMessageRow(r);

            itinerarySet.ItineraryMessage.AddItineraryMessageRow(
                itinerarySet.Itinerary[0], r, DateTime.Now, Cache.User.UserID);

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

        #region Reports

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            if (txtTemplateFile.Text.Length == 0)
            {
                App.ShowError("Please select a template file first.");
                return;
            }

            string saveFile = App.PromptSaveFile("itinerary_export.xls", "Excel", ".xls", false);
            if (String.IsNullOrEmpty(saveFile))
                return;

            ExportToExcel export = new ExportToExcel(itinerarySet);
            export.ExportItinerary(txtTemplateFile.Text, saveFile);
        }

        private void btnChooseTemplate_Click(object sender, EventArgs e)
        {
            txtTemplateFile.Text = App.SelectExternalFile(false, "Select Excel template file", "Excel (*.xls)|*.xls", 0) ?? String.Empty;
        }

        private void optReport_ValueChanged(object sender, EventArgs e)
        {
            grpReports.Text = optReport.Text;

            switch (optReport.Value.ToString())
            {
                case "PricingDetailed":
                    LoadReportControl(new ReportControls.PricingDetail(itinerarySet));
                    break;

                case "PricingSummary":
                    LoadReportControl(new ReportControls.PricingSummary(itinerarySet));
                    break;

                case "ClientFinalPricing":
                    LoadReportControl(new ReportControls.ClientFinalPricing(
                            itinerarySet, agentSet, itinerarySet.GetGrossFinalPrice()));
                    break;

                case "ClientFullPricing":
                    LoadReportControl(new ReportControls.ClientFullPricing(
                            itinerarySet, agentSet, itinerarySet.GetGrossFinalPrice()));
                    break;

                case "ClientNoPricing":
                    LoadReportControl(new ReportControls.ClientNoPricing(itinerarySet, agentSet));
                    break;

                case "Vouchers":
                    LoadReportControl(new ReportControls.Vouchers(
                                          itinerarySet, agentSet, Cache.UserSet));
                    break;

                case "SupplierRequest":
                    LoadReportControl(new ReportControls.SupplierMessage(
                                          SupplierMessage.ReportType.Request, itinerarySet, agentSet, Cache.UserSet));
                    break;

                case "SupplierConfirmation":
                    LoadReportControl(new ReportControls.SupplierMessage(
                                          SupplierMessage.ReportType.Confirmation, itinerarySet, agentSet, Cache.UserSet));
                    break;

                case "SupplierRemittance":
                    LoadReportControl(new ReportControls.SupplierMessage(
                                          SupplierMessage.ReportType.Remittance,
                                          itinerarySet, agentSet, Cache.UserSet));
                    break;

                case "ExcelReport":
                    LoadReportControl(new ReportControls.ExcelReport(itinerarySet));
                    break;
            }
        }

        internal void LoadReportControl(UserControl uc)
        {
            Cursor = Cursors.WaitCursor;

            // clear current bindings
            App.ClearBindings(pnlReports);

            // clear current control
            pnlReports.Controls.Clear();

            try
            {
                uc.Tag = this;
                uc.Dock = DockStyle.Fill;
                pnlReports.Controls.Add(uc);
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

        #endregion

        #region Help

        private void OpenHelp()
        {
            // Add main tab name.
            string id = "Itinerary." + tabControl_Main.SelectedTab.Text;

            // Add additional tab name.
            if (tabControl_Main.SelectedTab.Key == "Additional")
                id += "." + tabControlAdditional.SelectedTab.Text;

            App.ShowHelp(id);
        }

        #endregion

        #region Helper methods

        private void ItinerarySet_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            SetDataDirtyName();
        }

        private void ItinerarySet_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            SetDataDirtyName();
        }

        internal void Itinerary_Rename(string newName)
        {
            itinerarySet.Itinerary[0].ItineraryName = newName;
        }

        #endregion

        #region Menu events

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveDataChanges();
        }

        private void menuRefresh_Click(object sender, EventArgs e)
        {
            ItinerarySet_Refresh();
        }

        private void menuSaveClose_Click(object sender, EventArgs e)
        {
            SaveDataChanges();
            Close();
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuDebugItinerary_Click(object sender, EventArgs e)
        {
            App.ViewDataSet(itinerarySet);
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            OpenHelp();
        }

        #endregion      

        #region Allocations
        private void btnAgentAllocation_Click(object sender, EventArgs e)
        {
            if (lblAllocations.Text == "Allocations")
            {
                var allocAgents = new AllocationAgentsForm();
                allocAgents.ItinerarySet = itinerarySet;
                if (allocAgents.ShowDialog() == DialogResult.OK)
                {
                    itinerarySet.Allocation[0].Quantity = allocAgents.TotalAllocations;
                    GetAllocatedAgents();
                }
            }
            else
            {
                if (App.AskYesNo("Do you want to remove the master link of this itinerary ?"))
                {

                    lblAllocations.Text = "Allocations";
                    btnAgentAllocation.Image = Properties.Resources.PageEdit;
                    itinerarySet.Itinerary[0].SetAllocationsItineraryIDNull();
                }
            }
        }

        private void GetAllocatedAgents()
        {           
            var totalAllocation = itinerarySet.Allocation.Rows.Count > 0 ? itinerarySet.Allocation[0].Quantity : 0;
            var totalAgents = 0;
            if (totalAllocation != 0)
            {
                //if (itinerarySet.Allocation[0].GetAllocationAgentRows().Any())
                //{                   
                    totalAgents += itinerarySet.Allocation[0].GetAllocationAgentRows().Count(r => r.Quantity != 0);
                //}
            }
            var agentText = totalAgents == 1 ? " agent)" : " agents)";
            txtAgentAllocation.Text = totalAllocation == 0
                                          ? "0"
                                          : totalAllocation.ToString() +
                                            (totalAgents == 0 ? "" : "(" + totalAgents.ToString() + agentText);           
        }
        #endregion
    }
}
