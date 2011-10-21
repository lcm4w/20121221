using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.Dialogs;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction;
using CellEventArgs = Infragistics.Win.UltraWinGrid.CellEventArgs;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace TourWriter.Modules.SupplierModule
{
    /// <summary>
    /// Summary description for ServiceEditor.
    /// </summary>
    public partial class ServiceEditor : UserControl
    {
        public event OnSupplierSelectedHandler OnSupplierSelected;
        public event OnServiceSelectedHandler OnServiceSelected;
        public event OnRateSelectedHandler OnRateSelected;
        public event OnOptionSelectedHandler OnOptionSelected;
        public event OnOptionCheckedChangedHandler OnOptionCheckedChanged;

        private SupplierSet supplierSet;
        private bool allowEditing = true;
        private bool isFirstTimeVisible = true;
        private DateTime defaultDate = DateTime.Now.Date;
        private DateTime? _selectedStartDate, _selectedEndDate;

        public bool ExpiredSeasonsHidden
        {
            get { return chkHideExpiredSeasons.Checked; }
        }

        public bool AllowEditing
        {
            get { return allowEditing; }
            set
            {
                allowEditing = value;

                tsServices.Visible = value;
                tsServiceTimes.Visible = value;
                tsRates.Visible = value;
                tsOptions.Visible = value;
                tsFoc.Visible = value;

                if (gridOptions.DisplayLayout.Bands[0].Columns.Exists("Edit"))
                    gridOptions.DisplayLayout.Bands[0].Columns["Edit"].Hidden = value;

                txtCheckinMinutesEarly.ReadOnly = !value;
                txtServiceComments.ReadOnly = !value;
                txtServiceDescription.ReadOnly = !value;
            }
        }

        internal SupplierSet SupplierSet
        {
            get { return supplierSet; }
            set
            {
                if (supplierSet == null)
                {
                    supplierSet = value;
                    DataBind();
                }
                else
                {
                    // Merge to retain bindings.
                    supplierSet.Clear();
                    supplierSet.Merge(value);
                }

                // Notify listners.
                if (OnSupplierSelected != null)
                    OnSupplierSelected(
                        new SupplierSelectedEventArgs(supplierSet.Supplier[0].SupplierID));


                if (App.IsDebugMode && ModifierKeys == Keys.Control)
                {
                    gridServices.AddExportHooks(SupplierSet);
                    gridOptions.AddExportHooks(SupplierSet);
                }
            }
        }

        internal UltraGrid ServicesGrid
        {
            get { return gridServices; }
        }

        internal UltraGrid OptionsGrid
        {
            get { return gridOptions; }
        }

        internal DateTime DefaultDate
        {
            get { return defaultDate; }
            set { defaultDate = value; }
        }

        public ServiceEditor()
        {
            InitializeComponent();
            gridRates.Click += gridRates_Click;
        }

        internal void CommitOpenEdits()
        {
            gridServices.UpdateData();
            gridRates.UpdateData();
            gridOptions.UpdateData();
        }

        private void DataBind()
        {
            // set the IsDeleted row filters
            string filter = "IsDeleted = 'FALSE' OR IsDeleted IS NULL";
            supplierSet.DefaultViewManager.DataViewSettings[supplierSet.Service.TableName].RowFilter = filter;
            supplierSet.DefaultViewManager.DataViewSettings[supplierSet.Rate.TableName].RowFilter = filter;
            supplierSet.DefaultViewManager.DataViewSettings[supplierSet.Option.TableName].RowFilter = filter;


            // tax types
            cmbNetTaxType.DataSource = Cache.ToolSet.TaxType;
            cmbNetTaxType.ValueMember = "TaxTypeID";
            cmbNetTaxType.DisplayMember = "TaxTypeCode";
            cmbGrossTaxType.DataSource = Cache.ToolSet.TaxType;
            cmbGrossTaxType.ValueMember = "TaxTypeID";
            cmbGrossTaxType.DisplayMember = "TaxTypeCode";

            // Services
            gridServices.DisplayLayout.ValueLists.Add("ServiceTypeList");
            gridServices.DisplayLayout.ValueLists["ServiceTypeList"].SortStyle = ValueListSortStyle.Ascending;
            foreach (ToolSet.ServiceTypeRow r in Cache.ToolSet.ServiceType)
            {
                // only add the item if if IsDeleted isn't true
                if (r.IsIsDeletedNull() || !r.IsDeleted)
                {
                    gridServices.DisplayLayout.ValueLists["ServiceTypeList"].ValueListItems.Add(
                        r.ServiceTypeID, r.ServiceTypeName);
                }
            }

            // create currency data list
            gridServices.DisplayLayout.ValueLists.Add("CurrencyList");
            gridServices.DisplayLayout.ValueLists["CurrencyList"].SortStyle = ValueListSortStyle.Ascending;
            gridServices.DisplayLayout.ValueLists["CurrencyList"].ValueListItems.Add(DBNull.Value, "");
            foreach (ToolSet.CurrencyRow r in Cache.ToolSet.Currency.Where(c => c.Enabled))
                gridServices.DisplayLayout.ValueLists["CurrencyList"].ValueListItems.Add(
                    r.CurrencyCode, r.DisplayName);

            // create charge type data list
            gridServices.DisplayLayout.ValueLists.Add("ChargeTypeList");
            gridServices.DisplayLayout.ValueLists["ChargeTypeList"].SortStyle = ValueListSortStyle.Ascending;
            gridServices.DisplayLayout.ValueLists["ChargeTypeList"].ValueListItems.Add(DBNull.Value, "");
            foreach (string chargeType in ChargeTypeList.ChargeTypes)
                gridServices.DisplayLayout.ValueLists["ChargeTypeList"].ValueListItems.Add(
                    chargeType, chargeType);

            // create option type data list
            gridOptions.DisplayLayout.ValueLists.Add("OptionTypeList");
            gridOptions.DisplayLayout.ValueLists["OptionTypeList"].SortStyle = ValueListSortStyle.Ascending;
            gridOptions.DisplayLayout.ValueLists["OptionTypeList"].ValueListItems.Add(DBNull.Value, "");
            foreach (ToolSet.OptionTypeRow optionType in Cache.ToolSet.OptionType)
                gridOptions.DisplayLayout.ValueLists["OptionTypeList"].ValueListItems.Add(
                    optionType.OptionTypeID, optionType.OptionTypeName);

            gridServices.SetDataBinding(supplierSet, "Supplier.SupplierService");

            // Service details
            cmbNetTaxType.DataBindings.Add("Value", supplierSet, "Supplier.SupplierService.NetTaxTypeID");
            cmbGrossTaxType.DataBindings.Add("Value", supplierSet, "Supplier.SupplierService.GrossTaxTypeID");
            gridServiceTimes.SetDataBinding(supplierSet, "Supplier.SupplierService.ServiceServiceTime");
            txtCheckinMinutesEarly.DataBindings.Add("Value", supplierSet, "Supplier.SupplierService.CheckinMinutesEarly");
            txtServiceDescription.DataBindings.Add("Text", supplierSet, "Supplier.SupplierService.Description");
            txtServiceComments.DataBindings.Add("Text", supplierSet, "Supplier.SupplierService.Comments");
            txtServiceWarning.DataBindings.Add("Text", supplierSet, "Supplier.SupplierService.Warning");
            txtMaxPax.DataBindings.Add("Text", supplierSet, "Supplier.SupplierService.MaxPax");
            txtLatitude.DataBindings.Add("Value", supplierSet, "Supplier.SupplierService.Latitude");
            txtLongitude.DataBindings.Add("Value", supplierSet, "Supplier.SupplierService.Longitude");

            // TODO : hiding spatial except for ENZ, WalshGroup
            pnlSpatial.Visible =
                Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "d949d605-e05f-47a7-9a9d-0d5fca50b2b4" ||
                Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "0d83ad36-b7c5-402f-8e41-84744b6c9991";

            // Rates
            gridRates.SetDataBinding(supplierSet, "Supplier.SupplierService.ServiceRate");

            // Options
            gridOptions.SetDataBinding(supplierSet, "Supplier.SupplierService.ServiceRate.RateOption");

            // FOCs
            gridFoc.SetDataBinding(supplierSet, "Supplier.SupplierService.ServiceDiscount");

            // Select initial rows
            HideNonActiveItems(chkHideNonActiveItems.Checked);
            if (gridServices.Rows.VisibleRowCount > 0)
                gridServices.Rows[0].Activate(); // fires HideExpiredSeasons

            gridOptions.KeyDown += gridOptions_KeyDown;
            supplierSet.Service.ColumnChanged += Service_ColumnChanged;
            supplierSet.Option.ColumnChanged += Option_ColumnChanged;

            InitializeCurrenciesGridView();
            InitializeGroupsGridView();
        }

        private void Option_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "IsSelected")
            {
                int optionId = (int)e.Row["OptionID"];
                bool isChecked = (bool)e.ProposedValue;

                if (OnOptionCheckedChanged != null)
                    OnOptionCheckedChanged(new OptionCheckedChangedEventArgs(optionId, isChecked));
            }
        }
        
        private void Service_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column == supplierSet.Service.ServiceTypeIDColumn)
            {
                UpdateServiceTimeColumnHeaders((e.Row as SupplierSet.ServiceRow).ServiceTypeID);
                UpdateServiceTypeConfigs((e.Row as SupplierSet.ServiceRow).ServiceID,
                                         (e.Row as SupplierSet.ServiceRow).ServiceTypeID, true);
            }
            else if (e.Column == supplierSet.Service.CurrencyCodeColumn)
            {
                UpdateServiceCurrencyInfo((e.Row as SupplierSet.ServiceRow).CurrencyCode);
            }
        }

        public void SetSelectedServiceRow(int serviceId)
        {
            foreach (UltraGridRow row in gridServices.Rows)
            {
                if ((int)row.Cells["ServiceID"].Value == serviceId)
                {
                    row.Activate();
                    break;
                }
            }
        }

        private DateTime? _defaultDate;
        public void SetSelectedRateRow(DateTime rateDate)
        {
            _defaultDate = rateDate;

            // select rate based on specified date
            foreach (UltraGridRow row in gridRates.Rows)
            {
                if (!row.IsFilteredOut
                    && rateDate.Date >= ((DateTime)row.Cells["ValidFrom"].Value).Date
                    && rateDate.Date <= ((DateTime)row.Cells["ValidTo"].Value).Date)
                {
                    if (row.Cells["ValidFrom"].Value != DBNull.Value)
                        _selectedStartDate = (DateTime)row.Cells["ValidFrom"].Value;
                    if (row.Cells["ValidTo"].Value != DBNull.Value)
                        _selectedEndDate = (DateTime)row.Cells["ValidTo"].Value;

                    row.Activate();
                    break;
                }
            }
        }

        public void SetSelectedOptionRow(int optionId)
        {
            foreach (UltraGridRow row in gridOptions.Rows)
            {
                if ((int)row.Cells["OptionID"].Value == optionId)
                {
                    if (!row.IsFilteredOut)
                        row.Activate();
                    break;
                }
            }
        }

        private void InitializeCurrenciesGridView()
        {
            // check service table for currencies data
            foreach (SupplierSet.ServiceRow row in supplierSet.Service)
            {
                if (!row.IsCurrencyCodeNull())
                {
                    menuItemCurrencies.Checked = true;
                    break;
                }
            }
        }

        private void InitializeGroupsGridView()
        {
            // check service table for groups data
            foreach (SupplierSet.ServiceRow row in supplierSet.Service)
            {
                if (!row.IsChargeTypeNull())
                {
                    menuItemGroups.Checked = true;
                    break;
                }
            }

            // check options table for groups data
            foreach (SupplierSet.OptionRow row in supplierSet.Option)
            {
                if (!row.IsOptionTypeIDNull())
                {
                    menuItemGroups.Checked = true;
                    break;
                }
            }
        }

        private void AutoSelectRateRow()
        {
            gridRates.ActiveRow = null;
            gridOptions.ActiveRow = null;

            // try for exact match
            UltraGridRow exactMatch = null;
            UltraGridRow closeMatch = null;
            foreach (var row in gridRates.Rows)
            {
                if (row.IsFilteredOut) continue;

                if (_selectedStartDate != null && _selectedEndDate != null &&
                    ((DateTime)row.Cells["ValidFrom"].Value).Date == ((DateTime)_selectedStartDate).Date &&
                    ((DateTime)row.Cells["ValidTo"].Value).Date == ((DateTime)_selectedEndDate).Date)
                {
                    exactMatch = row;
                    break;
                }
                if (closeMatch == null &&
                    _defaultDate != null && 
                    ((DateTime)row.Cells["ValidFrom"].Value).Date <= ((DateTime)_defaultDate).Date &&
                    ((DateTime)row.Cells["ValidTo"].Value).Date >= ((DateTime)_defaultDate).Date)
                {
                    closeMatch = row; 
                }
            }

            if (exactMatch != null) exactMatch.Activate();
            else if (closeMatch != null) closeMatch.Activate();

        }
        
        private void SelectFirstVisibleOptionRow()
        {
            gridOptions.ActiveRow = null;

            foreach (UltraGridRow row in gridOptions.Rows)
            {
                if (!row.IsFilteredOut)
                {
                    row.Activate();
                    return;
                }
            }
        }

        public void EditOptionPrice(int optionId)
        {
            if (gridOptions.ActiveRow == null) return;

            int rateId = supplierSet.Option.FindByOptionID(optionId).RateID;
            int serviceId = supplierSet.Rate.FindByRateID(rateId).ServiceID;
            SupplierSet.ServiceRow serviceRow = supplierSet.Service.FindByServiceID(serviceId);
            string currencyCode = serviceRow.CurrencyCode;

            PriceEditorForm dialog = new PriceEditorForm(currencyCode);

            dialog.Net = gridOptions.ActiveRow.Cells["Net"].Value != DBNull.Value
                             ?
                         (decimal)gridOptions.ActiveRow.Cells["Net"].Value
                             : 0;

            dialog.Gross = gridOptions.ActiveRow.Cells["Gross"].Value != DBNull.Value
                               ?
                           (decimal)gridOptions.ActiveRow.Cells["Gross"].Value
                               : 0;

            dialog.PricingOption = gridOptions.ActiveRow.Cells["PricingOption"].Value != DBNull.Value
                                       ?
                                   (string)gridOptions.ActiveRow.Cells["PricingOption"].Value
                                       : "";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                gridOptions.ActiveRow.Cells["Net"].Value = dialog.Net;
                gridOptions.ActiveRow.Cells["Gross"].Value = dialog.Gross;
                gridOptions.ActiveRow.Cells["PricingOption"].Value = dialog.PricingOption;
            }

            dialog.Dispose();
        }

        private void HideExpiredSeasons(bool doHide)
        {
            if (doHide)
            {
                // filter expired rates
                gridRates.DisplayLayout.Bands[0].ColumnFilters["ValidTo"].
                    FilterConditions.Add(FilterComparisionOperator.GreaterThanOrEqualTo, DateTime.Now.Date);

                // filter expired options
                foreach (UltraGridRow row in gridRates.Rows)
                {
                    if (row.IsFilteredOut) // expired
                    {
                        gridOptions.DisplayLayout.Bands[0].ColumnFilters["RateID"].
                            FilterConditions.Add(FilterComparisionOperator.NotEquals, row.Cells["RateID"].Value);
                    }
                }
            }
            else
            {
                // reset filter on expired options
                foreach (UltraGridRow row in gridRates.Rows)
                {
                    if (row.IsFilteredOut)
                    {
                        gridOptions.DisplayLayout.Bands[0].ColumnFilters["RateID"].
                            ClearFilterConditions();
                    }
                }
                // reset filter on expired rates
                gridRates.DisplayLayout.Bands[0].ColumnFilters["ValidTo"].
                    ClearFilterConditions();
            }
        }

        private void HideNonActiveItems(bool doHide)
        {
            // Filter visible columns.
            if (doHide)
            {
                gridServices.DisplayLayout.Bands[0].ColumnFilters["IsRecordActive"].
                    FilterConditions.Add(FilterComparisionOperator.Equals, doHide);
                gridOptions.DisplayLayout.Bands[0].ColumnFilters["IsRecordActive"].
                    FilterConditions.Add(FilterComparisionOperator.Equals, doHide);
            }
            else
            {
                gridServices.DisplayLayout.Bands[0].ColumnFilters["IsRecordActive"].
                    ClearFilterConditions();
                gridOptions.DisplayLayout.Bands[0].ColumnFilters["IsRecordActive"].
                    ClearFilterConditions();
            }

            // Show / hide column.
            gridServices.Rows.Band.Columns["IsRecordActive"].Hidden = doHide;
            gridOptions.Rows.Band.Columns["IsRecordActive"].Hidden = doHide;
        }

        internal void ResetFormLayout()
        {
            splitter1.SplitPosition = (int)(Width / 2.3);
            splitter2.SplitPosition = (int)(Height / 2.3);
        }

        internal int? GetSelectedOptionId()
        {
            if (gridOptions.ActiveRow != null)
            {
                return (int?)gridOptions.ActiveRow.Cells["OptionID"].Value;
            }
            else
            {
                return null;
            }
        }

        internal int? GetSelectedRateId()
        {
            if (gridRates.ActiveRow != null)
            {
                return (int?)gridRates.ActiveRow.Cells["RateID"].Value;
            }
            else
            {
                return null;
            }
        }

        internal int? GetSelectedServiceId()
        {
            if (gridServices.ActiveRow != null)
            {
                return (int?)gridServices.ActiveRow.Cells["ServiceID"].Value;
            }
            else
            {
                return null;
            }
        }

        private int? GetSelectedServicePaymentTermId()
        {
            int? paymentTermID = null;
            if (gridServices.ActiveRow != null)
            {
                object id = gridServices.ActiveRow.Cells["PaymentTermID"].Value;
                if (id != DBNull.Value)
                    paymentTermID = (int?)id;
            }
            return paymentTermID;
        }

        internal string GetSelectedServiceWarningMessage()
        {
            if (gridServices.ActiveRow != null)
            {
                int? id = GetSelectedServiceId();
                if (id.HasValue)
                    return supplierSet.Service.FindByServiceID((int)id).Warning;
            }
            return "";
        }

        private void SelectAllOptions()
        {
            bool unselectAll = true;

            foreach (UltraGridRow row in gridOptions.Rows)
            {
                if (row.VisibleIndex < 0) continue;

                if ((bool)row.Cells["IsSelected"].Value)
                    continue;

                row.Cells["IsSelected"].Value = true;
                unselectAll = false;
            }
            if (unselectAll)
            {
                // if all of the rows are already selected, then unselect all
                foreach (UltraGridRow row in gridOptions.Rows)
                {
                    row.Cells["IsSelected"].Value = false;
                }
            }
        }

        internal void SetSelectedOnActiveOptionRow(bool selected)
        {
            if (gridOptions.ActiveRow != null)
                gridOptions.ActiveRow.Cells["IsSelected"].Value = selected;
        }

        private void UpdateServiceTimeColumnHeaders(int serviceTypeId)
        {
            ToolSet.ServiceTypeRow serviceType =
                Cache.ToolSet.ServiceType.FindByServiceTypeID(serviceTypeId);

            gridServiceTimes.DisplayLayout.Bands[0].Columns["StartTime"].Header.Caption =
                serviceType != null ? serviceType.BookingStartName : "<none>";
            gridServiceTimes.DisplayLayout.Bands[0].Columns["EndTime"].Header.Caption =
                serviceType != null ? serviceType.BookingEndName : "<none>";
        }

        private void UpdateServiceCurrencyInfo(string currencyCode)
        {
            var currency = CurrencyService.GetCurrency(currencyCode);
            var format = currency != null ? currency.DisplayFormat : "c";
            var message = currency != null ? currency.CurrencyCode + ", " + currency.CurrencyName : "";

            lblCurrencyInfo.Visible = message.Length > 0;
            lblCurrencyInfo.Text = message;

            gridOptions.DisplayLayout.Bands[0].Columns["Net"].Format = format;
            gridOptions.DisplayLayout.Bands[0].Columns["Gross"].Format = format;
            gridOptions.Refresh();
        }

        private void UpdateServiceTypeConfigs(int serviceId, int serviceTypeId, bool deleteExistingConfigsForService)
        {
            if (deleteExistingConfigsForService)
            {
                // Delete all existing configurations for a specified service.
                SupplierSet.ServiceConfigRow[] rows = (SupplierSet.ServiceConfigRow[])
                    supplierSet.ServiceConfig.Select("ServiceID = " + serviceId);

                foreach (SupplierSet.ServiceConfigRow row in rows)
                    row.Delete();
            }

            // Load service type configs for a specified service.
            treeServiceConfigs.Override.Sort = SortType.Default;
            treeServiceConfigs.Nodes.Clear();

            foreach (ToolSet.ServiceConfigTypeRow config in Cache.ToolSet.ServiceConfigType)
            {
                if (config.RowState == DataRowState.Deleted)
                    continue;

                if (config.ServiceTypeID != serviceTypeId)
                    continue;

                UltraTreeNode node = new UltraTreeNode(
                    config.ServiceConfigTypeID.ToString(), config.ServiceConfigTypeName);

                SupplierSet.ServiceConfigRow existingConfig =
                    supplierSet.ServiceConfig.FindByServiceIDServiceConfigTypeID(
                        serviceId, config.ServiceConfigTypeID);

                if (existingConfig != null)
                    node.CheckedState = CheckState.Checked;

                treeServiceConfigs.Nodes.Add(node);
            }
            treeServiceConfigs.Override.Sort = SortType.Ascending;
        }

        private void UpdatePaymentTermCustomText()
        {
            int? paymentTermID = GetSelectedServicePaymentTermId();
            if (paymentTermID == null)
            {
                txtPaymentTerms.Text = String.Empty;
                return;
            }

            SupplierSet.PaymentTermRow row = supplierSet.PaymentTerm.FindByPaymentTermID((int)paymentTermID);
            txtPaymentTerms.Text = row.GetCustomText(Cache.ToolSet.PaymentDue);
        }

        /// <summary>
        /// Initializes the values in a PaymentTermsEditor
        /// </summary>
        /// <param name="termsEditor">The PaymentTermsEditor to initialize</param>
        private void InitializePaymentTermsEditor(PaymentTermsEditor termsEditor)
        {
            int? paymentTermID = GetSelectedServicePaymentTermId();

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
            int? paymentTermID = GetSelectedServicePaymentTermId();

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
                        SupplierSet.ServiceRow serviceRow =
                            supplierSet.Service.FindByServiceID((int)GetSelectedServiceId());

                        // add the row to the table
                        supplierSet.PaymentTerm.AddPaymentTermRow(paymentTermRow);
                        serviceRow.PaymentTermID = paymentTermRow.PaymentTermID;
                    }
                }
            }
        }

        private void ToggleCurrencyColumns(bool enable)
        {
            gridServices.DisplayLayout.Bands[0].Columns["CurrencyCode"].Hidden = !enable;
        }

        private void ToggleGroupColumns(bool enable)
        {
            gridServices.DisplayLayout.Bands[0].Columns["ChargeType"].Hidden = !enable;
            gridOptions.DisplayLayout.Bands[0].Columns["OptionTypeID"].Hidden = !enable;
            gridOptions.DisplayLayout.Bands[0].Columns["IsDefault"].Hidden = !enable;
        }

        #region Events

        private void linkEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (gridServices.ActiveRow == null) return;
            var termsEditor = new PaymentTermsEditor();
            InitializePaymentTermsEditor(termsEditor);
            OpenPaymentTermsEditor(termsEditor);
            UpdatePaymentTermCustomText();
        }

        private void ServiceEditor_VisibleChanged(object sender, EventArgs e)
        {
            if (isFirstTimeVisible)
            {
                ResetFormLayout();
                isFirstTimeVisible = false;
            }
        }

        private void gridFoc_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "UnitsUsed")
                {
                    c.Header.Caption = "Used";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "UnitsFree")
                {
                    c.Header.Caption = "Free";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                {
                    c.Hidden = true;
                }
            }

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void btnFocAdd_Click(object sender, EventArgs e)
        {
            if (gridServices.ActiveRow == null)
                return;

            // create new foc row
            var foc = supplierSet.Discount.NewDiscountRow();
            foc.ServiceID = (int)GetSelectedServiceId();
            foc.UnitsUsed = 0;
            foc.UnitsFree = 0;
            foc.DiscountType = "foc";
            supplierSet.Discount.AddDiscountRow(foc);

            // select the newly created row
            gridFoc.ActiveRow = gridFoc.Rows[gridFoc.Rows.Count - 1];
        }

        private void btnFocDel_Click(object sender, EventArgs e)
        {
            if (gridFoc.ActiveRow == null)
                return;

            if (App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridFoc, true);
        }

        #region Services

        private void gridServices_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "IsRecordActive")
                {
                    c.Width = 1;
                    c.Header.Caption = "Active";
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                }
                else if (c.Key == "ServiceName")
                {
                    c.Width = 100;
                    c.Header.Caption = "Name";
                    c.Band.SortedColumns.Add(c, false);
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                }
                else if (c.Key == "ServiceTypeID")
                {
                    c.Width = 60;
                    c.Header.Caption = "Type";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridServices.DisplayLayout.ValueLists["ServiceTypeList"];
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                }
                else if (c.Key == "CurrencyCode")
                {
                    c.Hidden = true; // hidden by default, made visible in InitialiseRow() if not null.
                    c.Width = 40;
                    c.Header.Caption = "Currency";
                    c.Header.ToolTipText = "Currency. No value (null) is treated as your default currency";

                    // Embed databound combo into the cell.
                    //c.ValueList = GridHelper.GetEmbeddableCurrencyDropDown(sender as UltraGrid, c);
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridServices.DisplayLayout.ValueLists["CurrencyList"];

                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                }
                else if (c.Key == "ChargeType")
                {
                    c.Width = 40;
                    c.Header.Caption = "Charge";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridServices.DisplayLayout.ValueLists["ChargeTypeList"];

                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                    c.Hidden = true;
                }
                else
                    c.Hidden = true;
            }

            int index = 0;
            e.Layout.Bands[0].Columns["IsRecordActive"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ServiceName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ServiceTypeID"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void gridServices_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // Cross out text on inactive service rows.
            bool isServiceActive =
                e.Row.Cells["IsRecordActive"].Value.Equals(DBNull.Value) ||
                (bool)e.Row.Cells["IsRecordActive"].Value;
            if (isServiceActive)
            {
                e.Row.Cells["ServiceName"].Appearance.FontData.Strikeout = DefaultableBoolean.False;
                e.Row.Cells["ServiceName"].Activation = Activation.AllowEdit;
                e.Row.Cells["ServiceTypeID"].Activation = Activation.AllowEdit;
            }
            else
            {
                e.Row.Cells["ServiceName"].Appearance.FontData.Strikeout = DefaultableBoolean.True;
                e.Row.Cells["ServiceName"].Activation = Activation.NoEdit;
                e.Row.Cells["ServiceTypeID"].Activation = Activation.NoEdit;
            }
        }

        private void gridServices_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            // Fire update immediately.
            if (e.Cell.Column.Key == "ServiceTypeID" || e.Cell.Column.Key == "CurrencyCode")
                e.Cell.EditorResolved.ExitEditMode(false, true);
        }
        
        private void gridServices_AfterRowActivate(object sender, EventArgs e)
        {
            if (gridServices.ActiveRow.IsFilteredOut)
            {
                gridServices.ActiveRow = null;
            }
            else
            {
                HideExpiredSeasons(chkHideExpiredSeasons.Checked);
                HideNonActiveItems(chkHideNonActiveItems.Checked);

                // Update unbound controls.
                SupplierSet.ServiceRow service = supplierSet.Service.FindByServiceID(
                    (int)gridServices.ActiveRow.Cells["ServiceID"].Value);
                
                AutoSelectRateRow();
                UpdateServiceTimeColumnHeaders(service.ServiceTypeID);
                UpdateServiceTypeConfigs(service.ServiceID, service.ServiceTypeID, false);
                UpdateServiceCurrencyInfo(service.CurrencyCode);
                UpdatePaymentTermCustomText();

                // Notify listeners.
                if (OnServiceSelected != null)
                    OnServiceSelected(
                        new ServiceSelectedEventArgs((int)gridServices.ActiveRow.Cells["ServiceID"].Value));
            }
        }

        private void gridServices_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            if (e.Cell.Column.Key == "ServiceTypeID")
            {
                // When changing service type, warn that existing configs 
                // for previous servicetype will be deleted.

                SupplierSet.ServiceConfigRow[] existingConfigs;
                existingConfigs = (SupplierSet.ServiceConfigRow[])supplierSet.ServiceConfig.Select(
                    "ServiceID = " + (int)e.Cell.Row.Cells["ServiceID"].Value);

                if (existingConfigs.Length > 0)
                {
                    string msg = "Existing configurations for this service will be lost, do you want to continue?";

                    // Cancel edit if NO.
                    if (!App.AskYesNo(msg))
                        e.Cancel = true;
                }
            }
        }
        
        private void gridServices_AddRow()
        {
            CommitOpenEdits();

            string currencyCode = "";
            if (gridServices.Rows.Count > 0)
            {
                // Copy currency code from last service row.
                currencyCode = gridServices.Rows[gridServices.Rows.Count - 1].
                    Cells["CurrencyCode"].Value.ToString();
            }

            SupplierSet.ServiceRow row = supplierSet.AddService(
                App.CreateUniqueNameValue(gridServices.Rows, "ServiceName", "New Service"),
                Cache.User.UserID, currencyCode);

            if (Cache.ToolSet.ServiceType.Rows.Count > 0)
                row.ServiceTypeID = Cache.ToolSet.ServiceType[0].ServiceTypeID;

            GridHelper.SetActiveRow(gridServices, "ServiceID", row.ServiceID, "ServiceName");
        }

        private void gridServices_DeleteSelectedRow()
        {
            if (gridServices.ActiveRow == null) return;

            if (App.AskDeleteRow())
            {
                GridHelper.DeleteActiveRow(gridServices, true);
            }
        }

        private void gridServices_CopySelectedRow()
        {
            CommitOpenEdits();

            if (gridServices.ActiveRow == null) return;

            SupplierSet.ServiceRow copyRow = supplierSet.Service.FindByServiceID(
                (int)gridServices.ActiveRow.Cells["ServiceID"].Value);

            if (App.AskYesNo("Copy service: " + copyRow.ServiceName + "?"))
            {
                SupplierSet.ServiceRow newServiceRow =
                    supplierSet.CopyServiceRow(copyRow, "Copy of", Cache.User.UserID, true);

                GridHelper.SetActiveRow(gridServices, "ServiceID", newServiceRow.ServiceID, "ServiceName");
            }
        }

        private void btnServiceAdd_Click(object sender, EventArgs e)
        {
            gridServices_AddRow();
        }

        private void btnServiceDelete_Click(object sender, EventArgs e)
        {
            gridServices_DeleteSelectedRow();
        }

        private void btnServiceCopy_Click(object sender, EventArgs e)
        {
            gridServices_CopySelectedRow();
        }

        private void gridServiceTimes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "StartTime")
                {
                    c.SortIndicator = SortIndicator.Ascending;
                }
                if (c.Key == "StartTime" || c.Key == "EndTime")
                {
                    c.Header.Caption = "";
                    c.Width = 80;
                    c.Format = "t";
                    c.MaskInput = "{time}";
                    DateTimeEditor editor = c.Editor as DateTimeEditor;
                    if (editor != null)
                        editor.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.EditAndSelectText;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                }
                else if (c.Key == "Comment")
                {
                    c.Header.Caption = "Comments";
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                }
                else
                    c.Hidden = true;
            }
            int index = 0;
            e.Layout.Bands[0].Columns["StartTime"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["EndTime"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Comment"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
        }

        private void btnServiceTimeAdd_Click(object sender, EventArgs e)
        {
            if (gridServices.ActiveRow == null)
                return;

            // add new row
            SupplierSet.ServiceTimeRow r = supplierSet.ServiceTime.NewServiceTimeRow();
            r.ServiceID = (int)gridServices.ActiveRow.Cells["ServiceID"].Value;
            r.StartTime = DateTime.Parse("1799-1-1 12:00");
            r.EndTime = DateTime.Parse("1799-1-1 12:00");
            supplierSet.ServiceTime.AddServiceTimeRow(r);
            GridHelper.SetActiveRow(gridServiceTimes, "ServiceTimeID", r.ServiceTimeID, "StartTime");
        }

        private void btnServiceTimeDelete_Click(object sender, EventArgs e)
        {
            if (gridServiceTimes.ActiveRow == null)
                return;

            if (App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridServiceTimes, true);
        }

        private void treeServiceConfigs_AfterCheck(object sender, NodeEventArgs e)
        {
            // Save changes.

            SupplierSet.ServiceRow currentService = supplierSet.Service.FindByServiceID(
                (int)gridServices.ActiveRow.Cells["ServiceID"].Value);

            SupplierSet.ServiceConfigRow existingConfig =
                supplierSet.ServiceConfig.FindByServiceIDServiceConfigTypeID(
                    currentService.ServiceID, int.Parse(e.TreeNode.Key));

            if (existingConfig == null && e.TreeNode.CheckedState == CheckState.Checked)
            {
                supplierSet.ServiceConfig.AddServiceConfigRow(
                    currentService, int.Parse(e.TreeNode.Key), "", DateTime.Now, TourWriter.Global.Cache.User.UserID, null);
            }
            else if (existingConfig != null) existingConfig.Delete();
        }

        private void cmbNetTaxType_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "TaxTypeCode")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        private void cmbGrossTaxType_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "TaxTypeCode")
                    c.Band.SortedColumns.Add(c, false);
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.Configure_OLD(e, true, false, false);
        }

        #endregion

        #region Rates

        private void gridRates_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ValidFrom")
                {
                    c.Header.Caption = "Valid From";
                    c.Band.SortedColumns.Add(c, false);
                    c.Width = 120;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                }
                else if (c.Key == "ValidTo")
                {
                    c.Header.Caption = "Valid To";
                    c.Width = 120;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                }
                else
                    c.Hidden = true;
            }

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.AutoFitStyle = AutoFitStyle.None;
        }
        
        private void gridRates_Click(object sender, EventArgs e)
        {
            var row = GridHelper.GetValidClickRow(gridRates);
            if (row == null) return;

            if (row.Cells["ValidFrom"].Value != DBNull.Value)
                _selectedStartDate = (DateTime)row.Cells["ValidFrom"].Value;
            if (row.Cells["ValidTo"].Value != DBNull.Value)
                _selectedEndDate = (DateTime)row.Cells["ValidTo"].Value;
        }

        private void gridRates_AfterRowActivate(object sender, EventArgs e)
        {
            if (gridRates.ActiveRow.IsFilteredOut)
            {
                gridRates.ActiveRow = null;
            }
            else
            {
                SelectFirstVisibleOptionRow();
                if (OnRateSelected != null)
                    OnRateSelected(
                        new RateSelectedEventArgs((int)gridRates.ActiveRow.Cells["RateID"].Value));
            }
        }

        private void gridRates_AddRow()
        {
            CommitOpenEdits();
            try
            {
                if (gridServices.ActiveRow == null)
                {
                    MessageBox.Show(App.GetResourceString("ShowRowNotSelected"));
                    return;
                }

                string editCell = "";
                SupplierSet.RateRow newRateRow;
                int serviceId = (int)gridServices.ActiveRow.Cells[0].Value;

                newRateRow = supplierSet.Rate.NewRateRow();
                newRateRow.ServiceID = (int)gridServices.ActiveRow.Cells[0].Value;
                newRateRow.AddedOn = DateTime.Now;
                newRateRow.AddedBy = Cache.User.UserID;

                // try to set next default date
                DataRow[] rows = supplierSet.Rate.Select("ServiceID = " + serviceId, "ValidTo DESC");
                if (rows.Length > 0 && gridRates.Rows.VisibleRowCount > 0)
                {
                    newRateRow.ValidFrom =
                        (rows[0] as SupplierSet.RateRow).ValidTo.AddDays(1); // set to next season
                    editCell = "ValidTo";
                }
                else
                {
                    newRateRow.ValidFrom = DateTime.Now; // set season start
                    editCell = "ValidFrom";
                }
                newRateRow.ValidTo = newRateRow.ValidFrom;
                supplierSet.Rate.AddRateRow(newRateRow);

                //gridRates.Rows.Band.SortedColumns.RefreshSort(false);
                GridHelper.SetActiveRow(gridRates, "RateID", newRateRow.RateID, editCell);
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void gridRates_DeleteSelectedRow()
        {
            if (gridRates.ActiveRow == null) return;

            if (App.AskDeleteRow())
            {
                GridHelper.DeleteActiveRow(gridRates, true);
            }
        }

        private void gridRates_CopySelectedRow()
        {
            CommitOpenEdits();
            if (gridRates.ActiveRow == null) return;

            SupplierSet.RateRow copyRow = supplierSet.Rate.FindByRateID(
                (int)gridRates.ActiveRow.Cells["RateID"].Value);

            if (App.AskYesNo(String.Format("Copy season: {0} - {1} to next start date?",
                                           copyRow.ValidFrom.ToShortDateString(), copyRow.ValidTo.ToShortDateString())))
            {
                SupplierSet.RateRow newRow =
                    supplierSet.CopyRate(copyRow, copyRow.ServiceRow.ServiceID, Cache.User.UserID, true, true);

                GridHelper.SetActiveRow(gridRates, "RateID", newRow.RateID, "ValidTo");
            }
        }

        private void btnRateAdd_Click(object sender, EventArgs e)
        {
            gridRates_AddRow();
        }

        private void btnRateDelete_Click(object sender, EventArgs e)
        {
            gridRates_DeleteSelectedRow();
        }

        private void btnRateCopy_Click(object sender, EventArgs e)
        {
            gridRates_CopySelectedRow();
        }

        #endregion

        #region Options

        private void gridOptions_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.Add("Markup");
            e.Layout.Bands[0].Columns.Add("Commission");
            e.Layout.Bands[0].Columns.Add("Edit");

            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "IsSelected")
                {
                    c.Header.Caption = String.Empty;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.Header.Appearance.Image = Properties.Resources.CheckBox;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.Width = 30;
                    c.MaxWidth = 30;
                    c.SortIndicator = SortIndicator.Disabled;
                    c.Hidden = allowEditing;
                }
                else if (c.Key == "IsRecordActive")
                {
                    c.Width = 1;
                    c.Header.Caption = "Active";
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                    c.TabStop = true;
                }
                else if (c.Key == "OptionName")
                {
                    c.Header.Caption = "Option";
                    c.Header.ToolTipText = "Option name";
                    c.Band.SortedColumns.Add(c, false);
                    c.CellMultiLine = DefaultableBoolean.True;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                    c.TabStop = true;
                }
                else if (c.Key == "OptionTypeID")
                {
                    c.Width = 30;
                    c.Header.Caption = "Type";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridOptions.DisplayLayout.ValueLists["OptionTypeList"];

                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                    c.Hidden = true;
                }
                else if (c.Key == "Net")
                {
                    c.Header.ToolTipText = "Net price";
                    c.Width = 30;
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellAppearance.ForeColor = SystemColors.ControlDarkDark;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                    c.TabStop = false;
                }
                else if (c.Key == "Markup")
                {
                    c.Header.Caption = "Mup";
                    c.Header.ToolTipText = "Markup percentage";
                    c.Width = 30;
                    c.DataType = typeof(Decimal);
                    c.Format = "##0.00\\%";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellAppearance.ForeColor = SystemColors.ControlDarkDark;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                    c.TabStop = false;
                }
                else if (c.Key == "Gross")
                {
                    c.Header.ToolTipText = "Gross price";
                    c.Width = 30;
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellAppearance.ForeColor = SystemColors.ControlDarkDark;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                    c.TabStop = false;
                }
                else if (c.Key == "Commission")
                {
                    c.Header.Caption = "Comm";
                    c.Header.ToolTipText = "Commission percentage";
                    c.Width = 30;
                    c.DataType = typeof(Decimal);
                    c.Format = "##0.00\\%";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellAppearance.ForeColor = SystemColors.ControlDarkDark;
                    if (!allowEditing)
                    {
                        c.CellActivation = Activation.NoEdit;
                        c.CellClickAction = CellClickAction.RowSelect;
                    }
                    c.TabStop = false;
                }
                else if (c.Key == "Edit")
                {
                    c.Header.Caption = "";
                    c.Width = 8;
                    c.Style = ColumnStyle.Button;
                    c.CellButtonAppearance.Image = TourWriter.Properties.Resources.PageEdit;
                    c.CellButtonAppearance.ImageHAlign = HAlign.Center;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;

                    if (!allowEditing)
                        c.Hidden = true;
                }
                else if (c.Key == "IsDefault")
                {
                    c.Header.Caption = "Default";
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.Width = 50;
                    c.MaxWidth = 50;
                    c.Hidden = true;
                }
                else
                    c.Hidden = true;
            }

            int index = 0;
            e.Layout.Bands[0].Columns["IsSelected"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["IsRecordActive"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["OptionName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["OptionTypeID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["IsDefault"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Net"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Markup"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Gross"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Commission"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Edit"].Header.VisiblePosition = index++;

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void gridOptions_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // Set active style for name column.
            bool isActive =
                e.Row.Cells["IsRecordActive"].Value.Equals(DBNull.Value) ||
                (bool)e.Row.Cells["IsRecordActive"].Value;

            if (isActive)
            {
                e.Row.Cells["OptionName"].Appearance.FontData.Strikeout = DefaultableBoolean.False;
                e.Row.Cells["OptionName"].Activation = Activation.AllowEdit;
                e.Row.Cells["Edit"].Hidden = false;
            }
            else
            {
                e.Row.Cells["OptionName"].Appearance.FontData.Strikeout = DefaultableBoolean.True;
                e.Row.Cells["OptionName"].Activation = Activation.NoEdit;
                e.Row.Cells["Edit"].Hidden = true;
            }

            // Set values for net and gross columns.
            if (e.Row.Cells["Net"].Value != DBNull.Value && e.Row.Cells["Gross"].Value != DBNull.Value)
            {
                decimal net = (decimal)e.Row.Cells["Net"].Value;
                decimal gross = (decimal)e.Row.Cells["Gross"].Value;
                e.Row.Cells["Markup"].Value =
                    Info.Services.Common.CalcMarkupByNetGross(net, gross);
                e.Row.Cells["Commission"].Value =
                    Info.Services.Common.CalcCommissionByNetGross(net, gross);
            }
        }

        private void gridOptions_AfterRowActivate(object sender, EventArgs e)
        {
            if (gridOptions.ActiveRow.IsFilteredOut)
            {
                gridOptions.ActiveRow = null;
            }
            else
            {
                if (OnOptionSelected != null)
                    OnOptionSelected(
                        new OptionSelectedEventArgs((int)gridOptions.ActiveRow.Cells["OptionID"].Value));
            }
        }

        private void gridOptions_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() != typeof(UltraGridEmptyRow))
            {
                if (allowEditing) gridOptions_HandleEditRequest(e.Row);
                else e.Row.Cells["IsSelected"].Value = !(bool)e.Row.Cells["IsSelected"].Value;
            }
        }

        private void gridOptions_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "Edit") 
                gridOptions_HandleEditRequest(e.Cell.Row);
        }

        private void gridOptions_HandleEditRequest(UltraGridRow optionGridRow)
        {
            if (optionGridRow == null || !AllowEditing) return;
            EditOptionPrice((int)optionGridRow.Cells["OptionID"].Value);
        }

        private void gridOptions_KeyDown(object sender, KeyEventArgs e)
        {
            if (gridOptions.ActiveRow == null)
                return;

            if (gridOptions.ActiveCell != null &&
                gridOptions.ActiveCell.IsInEditMode &&
                gridOptions.ActiveCell.Column.DataType == typeof(string))
                return;

            // handle new/copy/delete shortcut keys
            if (e.Control && e.KeyCode == Keys.N)
            {
                gridOptions_AddRow();
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                gridOptions_CopySelectedRow();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                gridOptions_DeleteSelectedRow();
            }

            // handle shortcut key to open price editor
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                if (gridOptions.ActiveCell != null
                    && gridOptions.ActiveCell.Column.Key == "Edit")
                {
                    gridOptions_HandleEditRequest(gridOptions.ActiveRow);
                }
            }
        }

        private void gridOptions_AddRow()
        {
            CommitOpenEdits();
            try
            {
                // add new option row

                if (gridRates.ActiveRow == null)
                {
                    MessageBox.Show(App.GetResourceString("ShowRowNotSelected"));
                }
                else
                {
                    // add new row
                    SupplierSet.OptionRow r = supplierSet.Option.NewOptionRow();
                    r.RateID = (int)gridRates.ActiveRow.Cells[0].Value;
                    r.OptionName = App.CreateUniqueNameValue(gridOptions.Rows, "OptionName", "New Option");
                    r.IsRecordActive = true;
                    r.AddedOn = DateTime.Now;
                    r.AddedBy = Cache.User.UserID;

                    // if this is the first row to be added, make it the default.
                    if (gridOptions.Rows.Count == 0)
                        r.IsDefault = true;

                    supplierSet.Option.AddOptionRow(r);

                    GridHelper.SetActiveRow(gridOptions, "OptionID", r.OptionID, "OptionName");
                }
            }
            catch (Exception ex)
            {
                App.Error(ex);
            }
        }

        private void gridOptions_DeleteSelectedRow()
        {
            if (gridOptions.ActiveRow == null) return;

            if (App.AskDeleteRow())
            {
                GridHelper.DeleteActiveRow(gridOptions, true);
            }
        }

        private void gridOptions_CopySelectedRow()
        {
            CommitOpenEdits();
            if (gridOptions.ActiveRow == null) return;

            SupplierSet.OptionRow copyRow = supplierSet.Option.FindByOptionID(
                (int)gridOptions.ActiveRow.Cells["OptionID"].Value);

            if (App.AskYesNo(String.Format("Copy option: {0}?", copyRow.OptionName)))
            {
                SupplierSet.OptionRow newRow =
                    supplierSet.CopyOption(copyRow, copyRow.RateRow.RateID, "Copy of", Cache.User.UserID);

                GridHelper.SetActiveRow(gridOptions, "OptionID", newRow.OptionID, "");
            }
        }

        private void btnOptionAdd_Click(object sender, EventArgs e)
        {
            gridOptions_AddRow();
        }

        private void btnOptionDelete_Click(object sender, EventArgs e)
        {
            gridOptions_DeleteSelectedRow();
        }

        private void btnOptionCopy_Click(object sender, EventArgs e)
        {
            gridOptions_CopySelectedRow();
        }

        private void gridOptions_MouseClick(object sender, MouseEventArgs e)
        {
            UIElement clickedElement =
                gridOptions.DisplayLayout.UIElement.ElementFromPoint(gridOptions.PointToClient(MousePosition));

            if (clickedElement == null)
                return;

            // if a header is clicked, check if it is the "IsSelected" column
            HeaderUIElement headerElement = (HeaderUIElement)clickedElement.GetAncestor(typeof(HeaderUIElement));
            if (headerElement != null)
            {
                UltraGridColumn column = (UltraGridColumn)headerElement.GetContext(typeof(UltraGridColumn));
                if (column.Key == "IsSelected")
                {
                    SelectAllOptions();
                }
            }
        }

        private void gridOptions_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "IsSelected")
            {
                // handle checkbox change immediatly
                e.Cell.EditorResolved.ExitEditMode(true, true);
            }
            else
            {
                // if default option changed, unselect any others
                if (e.Cell.Column.Key == "IsDefault")
                    if ((bool) e.Cell.Value == false)
                        foreach (var o in gridOptions.Rows)
                            if (o.Cells["OptionID"].Value != e.Cell.Row.Cells["OptionID"].Value)
                                if ((bool) o.Cells["IsDefault"].Value)
                                    o.Cells["IsDefault"].Value = false;
            }
        }

        #endregion

        #region Viewing options

        private bool isExpiredMessageShown;
        private void pnlMain_VisibleChanged(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            if (supplierSet == null)
                return;

            if (pnlMain.Visible && !isExpiredMessageShown)
            {
                if (supplierSet.Rate.Count > gridRates.Rows.VisibleRowCount)
                {
                    errorProvider.Icon = Properties.Resources.WarningIcon;
                    errorProvider.SetError(chkHideExpiredSeasons, "Hiding expired rates, untick to show");
                    isExpiredMessageShown = true;
                }
            }
            else
            {
                errorProvider.SetError(chkHideExpiredSeasons, "");
            }
        }

        private void chkHideExpiredSeasons_CheckedChanged(object sender, EventArgs e)
        {
            errorProvider.SetError(chkHideExpiredSeasons, ""); // reset warning
            HideExpiredSeasons(chkHideExpiredSeasons.Checked);
            AutoSelectRateRow();
        }

        private void chkHideNonActiveItems_CheckedChanged(object sender, EventArgs e)
        {
            HideNonActiveItems(chkHideNonActiveItems.Checked);
        }

        private void menuItemCurrencies_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCurrencyColumns(menuItemCurrencies.Checked);
        }

        private void menuItemGroups_CheckedChanged(object sender, EventArgs e)
        {
            ToggleGroupColumns(menuItemGroups.Checked);
        }

        #endregion

        #endregion
    }

    public delegate void OnOptionCheckedChangedHandler(OptionCheckedChangedEventArgs e);
    public class OptionCheckedChangedEventArgs
    {
        public int OptionID { get; private set; }
        public bool IsChecked { get; private set; }

        public OptionCheckedChangedEventArgs(int optionId, bool isChecked)
        {
            OptionID = optionId;
            IsChecked = isChecked;
        }
    }

    public delegate void OnSupplierSelectedHandler(SupplierSelectedEventArgs e);
    public class SupplierSelectedEventArgs
    {
        public int SuppliereId;

        public SupplierSelectedEventArgs(int supplierId)
        {
            SuppliereId = supplierId;
        }
    }

    public delegate void OnServiceSelectedHandler(ServiceSelectedEventArgs e);
    public class ServiceSelectedEventArgs
    {
        public int ServiceId;

        public ServiceSelectedEventArgs(int serviceId)
        {
            ServiceId = serviceId;
        }
    }

    public delegate void OnRateSelectedHandler(RateSelectedEventArgs e);
    public class RateSelectedEventArgs
    {
        public int RateId;

        public RateSelectedEventArgs(int rateId)
        {
            RateId = rateId;
        }
    }

    public delegate void OnOptionSelectedHandler(OptionSelectedEventArgs e);
    public class OptionSelectedEventArgs
    {
        public int OptionId;

        public OptionSelectedEventArgs(int optionId)
        {
            OptionId = optionId;
        }
    }
}
