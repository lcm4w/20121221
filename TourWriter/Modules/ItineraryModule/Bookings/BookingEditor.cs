using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using TourWriter.BusinessLogic;
using TourWriter.Dialogs;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class BookingEditor : UserControl
    {
        #region Properties

        public event OnBookingEditorOpenSupplierHandler OnOpenSupplier;
        private List<string> supplierNotesList;
        private const string emptySupplierNotesMessage = "< none available >";
        private BindingSource _bindingSource;

        public BindingSource BindingSource
        {
            get { return _bindingSource; }
            set
            {
                _bindingSource = value;

                if (value != null)
                {
                    SetDataBindings();

                    GridHelper.SetNumberOfDaysPicker(gridItems);

                    CurrencyManager cm;
                    cm = BindingContext[BindingSource, "PurchaseLineID"] as CurrencyManager;
                    if (cm != null)
                        cm.CurrentChanged += PurchaseLine_CurrentChanged;

                    cm = BindingContext[BindingSource, "PurchaseLinePurchaseItem"] as CurrencyManager;
                    if (cm != null)
                        cm.CurrentChanged += PurchaseItem_CurrentChanged;

                    itinerarySet.PurchaseItem.PurchaseItemRowChanged +=
                        PurchaseItem_PurchaseItemRowChanged;
                }
            }
        }

        private ItinerarySet itinerarySet
        {
            get { return BindingSource.DataSource as ItinerarySet; }
        }

        public ItineraryMain ItineraryMain { get; set; }

        #endregion

        #region Methods

        public BookingEditor()
        {
            InitializeComponent();
        }

        private void BookingEditor_Load(object sender, EventArgs e)
        {
            // Dropdowns need any control for the dropdown event to fire.

            DropDownEditorButton dd;
            dd = txtNoteVoucher.ButtonsRight[0] as DropDownEditorButton;
            if (dd != null)
                dd.Control = new Panel();

            dd = txtNoteSupplier.ButtonsRight[0] as DropDownEditorButton;
            if (dd != null)
                dd.Control = new Panel();

            dd = txtNoteClient.ButtonsRight[0] as DropDownEditorButton;
            if (dd != null)
                dd.Control = new Panel();

            dd = txtNotePrivate.ButtonsRight[0] as DropDownEditorButton;
            if (dd != null)
                dd.Control = new Panel();
        }

        private void PurchaseLine_CurrentChanged(object sender, EventArgs e)
        {
            supplierNotesList = null;
            SetNetSummary();
        }

        private static void PurchaseItem_CurrentChanged(object sender, EventArgs e)
        {
            // empty
        }

        public void SetActiveRow(int? purchaseItemId)
        {
            GridHelper.SetActiveRow(gridItems, "PurchaseItemID", purchaseItemId, "");
        }

        public void CommitEdits()
        {
            BindingSource.EndEdit();
            BindingContext[BindingSource].EndCurrentEdit();
            Validate();
            gridItems.PerformAction(UltraGridAction.ExitEditMode);
            gridItems.UpdateData();
        }

        private void SetDataBindings()
        {
            cmbItemStatus.DataSource = Cache.ToolSet.RequestStatus;
            cmbItemStatus.DisplayMember = "RequestStatusName";
            cmbItemStatus.ValueMember = "RequestStatusID";
            gridItems.DisplayLayout.ValueLists.Add("StatusList");
            gridItems.DisplayLayout.ValueLists["StatusList"].SortStyle = ValueListSortStyle.Ascending;
            gridItems.DisplayLayout.ValueLists["StatusList"].ValueListItems.Add(DBNull.Value, "(none)");
            foreach (ToolSet.RequestStatusRow r in Cache.ToolSet.RequestStatus.Rows)
                gridItems.DisplayLayout.ValueLists["StatusList"].ValueListItems.
                    Add(r.RequestStatusID, r.RequestStatusName);

            // Lines
            txtLineId.DataBindings.Add("Text", BindingSource, "PurchaseLineID");
            txtLineName.DataBindings.Add("Text", BindingSource, "PurchaseLineName");
            txtNotePrivate.DataBindings.Add("Text", BindingSource, "Comments");
            txtNoteSupplier.DataBindings.Add("Text", BindingSource, "NoteToSupplier");
            txtNoteVoucher.DataBindings.Add("Text", BindingSource, "NoteToVoucher");
            txtNoteClient.DataBindings.Add("Text", BindingSource, "NoteToClient");

            // Items
            gridItems.SetDataBinding(BindingSource, "PurchaseLinePurchaseItem");
            txtItemDesc.DataBindings.Add("Text", BindingSource, "PurchaseLinePurchaseItem.PurchaseItemName");
            cmbItemStatus.DataBindings.Add("Value", BindingSource, "PurchaseLinePurchaseItem.RequestStatusID");
            txtItemRef.DataBindings.Add("Text", BindingSource, "PurchaseLinePurchaseItem.BookingReference");
            txtItemQty.DataBindings.Add("Value", BindingSource, "PurchaseLinePurchaseItem.Quantity");
            txtItemDays.DataBindings.Add("Value", BindingSource, "PurchaseLinePurchaseItem.NumberOfDays");
            txtItemStartDate.DataBindings.Add("Value", BindingSource, "PurchaseLinePurchaseItem.StartDate");
            txtItemEndDate.DataBindings.Add("Value", BindingSource, "PurchaseLinePurchaseItem.EndDate");
            txtItemStartTime.DataBindings.Add("Value", BindingSource, "PurchaseLinePurchaseItem.StartTime");
            txtItemEndTime.DataBindings.Add("Value", BindingSource, "PurchaseLinePurchaseItem.EndTime");

            // Force edit updates for some controls.
            App.BindingsForceEndEdit(txtLineName, "Text");
            App.BindingsForceEndEdit(txtItemDesc, "Text");
            App.BindingsForceEndEdit(cmbItemStatus, "Value");
            App.BindingsForceEndEdit(txtItemRef, "Text");
            App.BindingsForceEndEdit(txtItemStartDate, "Value");
            App.BindingsForceEndEdit(txtItemEndTime, "Value");
            App.BindingsForceEndEdit(txtItemEndDate, "Value");
            App.BindingsForceEndEdit(txtItemEndTime, "Value");
            App.BindingsForceEndEdit(txtItemQty, "Value");
            App.BindingsForceEndEdit(txtItemDays, "Value");
            
            // summaries
            var itinerary = itinerarySet.Itinerary[0];
            var currencyInfo = CurrencyService.GetCurrency(itinerary.CurrencyCode);
            var format = "{0:" + (currencyInfo != null ? currencyInfo.DisplayFormat : "c") + "}";
            gridItems.DisplayLayout.Bands[0].Summaries["GrossFinal"].DisplayFormat = format;
        }

        private void OnPurchaseItemActivated(int purchaseItemId)
        {
            // Handle unbound purchase item editors.

            ItinerarySet.PurchaseItemRow itemRow =
                itinerarySet.PurchaseItem.FindByPurchaseItemID(purchaseItemId);

            if (itemRow != null)
            {
                SetItemPriceInfo(itemRow);

                // Show early checkin info.
                ItinerarySet.OptionLookupRow option =
                    itinerarySet.OptionLookup.FindByOptionID(itemRow.OptionID);

                if (option != null)
                {
                    picEarlyCheckinInfo.Visible =
                        !option.IsCheckinMinutesEarlyNull() && option.CheckinMinutesEarly > 0;
                }

                // Set end date override.
                txtItemEndDate.Enabled = chkEndDateOverride.Checked = !itemRow.IsEndDateNull();
                var span = !itemRow.IsNumberOfDaysNull() ? itemRow.NumberOfDays : 0;
                if (!itemRow.IsStartDateNull())
                    txtItemEndDate.NullText = itemRow.StartDate.Date.AddDays(span).ToShortDateString();

                // Set start/end labels for service type.
                ToolSet.ServiceTypeRow serviceType = Cache.ToolSet.ServiceType.FindByServiceTypeID(itemRow.ServiceTypeID);

                if (serviceType != null)
                {
                    lblStartDate.Text = serviceType.BookingStartName;
                    lblEndDate.Text = serviceType.BookingEndName;

                    if (!serviceType.IsNumberOfDaysNameNull())
                        lblLength.Text = serviceType.NumberOfDaysName;
                    else
                        lblLength.Text = "Nights";
                }
            }
        }

        private void SetItemPriceInfo(ItinerarySet.PurchaseItemRow itemRow)
        {
            listPrice.Items.Clear();
            listPrice.Columns.Clear();

            int col1Width = 60;
            int col2Width = listPrice.Width - col1Width;
            listPrice.Columns.Add("", col1Width, HorizontalAlignment.Left);
            listPrice.Columns.Add("", col2Width, HorizontalAlignment.Left);

            var hasOverride = CurrencyService.GetPurchaseItemCurrencyCode(itemRow) != null;
            var format = "{0:" + (hasOverride ? CurrencyService.GetCurrency(itemRow.CurrencyCode).DisplayFormat : "c") + "}";

            var listItem = new ListViewItem {Text = "Net:"};
            listItem.SubItems.Add(string.Format(format, itemRow.Net));
            listPrice.Items.Add(listItem);

            listItem = new ListViewItem {Text = "Markup:"};
            listItem.SubItems.Add(string.Format("{0:p}", itinerarySet.GetMarkup(itemRow.Net, itemRow.Gross) / 100));
            listPrice.Items.Add(listItem);

            listItem = new ListViewItem {Text = "Gross:"};
            listItem.SubItems.Add(string.Format(format, itemRow.Gross));
            listPrice.Items.Add(listItem);

            listItem = new ListViewItem {Text = "Commission:"};
            listItem.SubItems.Add(string.Format("{0:p}", itinerarySet.GetCommission(itemRow.Net, itemRow.Gross) / 100));
            listPrice.Items.Add(listItem);
            
            if (!itemRow.IsPaymentTermIDNull())
            {
                listItem = new ListViewItem { Text = "Terms:" };
                listPrice.Items.Add(listItem);

                var terms = itinerarySet.PaymentTerm.FindByPaymentTermID(itemRow.PaymentTermID);
                var text = Info.Services.Common.GetPaymentTermsFullText( !terms.IsPaymentDueIDNull() ? terms.PaymentDueID : (int?)null,
                                                                         !terms.IsPaymentDuePeriodNull() ? terms.PaymentDuePeriod : (int?)null,
                                                                         !terms.IsDepositAmountNull() ? terms.DepositAmount : (decimal?)null,
                                                                         !terms.IsDepositTypeNull() ? terms.DepositType : (char?)null,
                                                                         !terms.IsDepositDueIDNull() ? terms.DepositDueID : (int?)null,
                                                                         !terms.IsDepositDuePeriodNull() ? terms.DepositDuePeriod : (int?)null,
                                                                         Cache.ToolSet.PaymentDue);
               
                foreach (var line in System.Text.RegularExpressions.Regex.Split(text, "\r\n"))
                {
                    listItem = new ListViewItem {Text = ""};
                    listItem.SubItems.Add(line);
                    listPrice.Items.Add(listItem);
                }
            }
        }

        private int GetSupplierIdForBooking()
        {
            return itinerarySet.PurchaseLine.FindByPurchaseLineID(
                int.Parse(txtLineId.Text)).SupplierID;
        }

        private void OpenBookingSupplier()
        {
            int supplierId = GetSupplierIdForBooking();
            int? itemId = gridItems.ActiveRow != null
                              ?
                          (int?)gridItems.ActiveRow.Cells["PurchaseItemID"].Value
                              : null;

            if (OnOpenSupplier != null)
                OnOpenSupplier(new BookingEditorOpenSupplierEventArgs(supplierId, itemId));
        }

        private void EditCurrentItemPrice()
        {
            if (gridItems.ActiveRow == null)
                return; // no booking items exist for booking

            int optionID = (int)gridItems.ActiveRow.Cells["OptionID"].Value;
            int purchaseItemID = (int)gridItems.ActiveRow.Cells["PurchaseItemID"].Value;
            ItinerarySet.PurchaseItemRow item =
                itinerarySet.PurchaseItem.FindByPurchaseItemID(purchaseItemID);

            PriceEditorForm editor = new PriceEditorForm(item.CurrencyCode);
            editor.Net = !item.IsNetNull() ? item.Net : 0;
            editor.Gross = !item.IsGrossNull() ? item.Gross : 0;

            var opt = itinerarySet.OptionLookup.FindByOptionID(optionID);
            if (opt != null && !opt.IsPricingOptionNull()) 
                editor.PricingOption = opt.PricingOption;

            if (editor.ShowDialog() == DialogResult.OK)
            {
                if (item.Net != editor.Net)
                    item.Net = editor.Net;
                if (item.Gross != editor.Gross)
                    item.Gross = editor.Gross;

                SetItemPriceInfo(item);
            }
            editor.Dispose();
        }

        internal void SetBindingContext(BindingContext bindingContext)
        {
            BindingContext = bindingContext;
        }

        private void DoSupplierNotesDropdown(Control dropdownOwner)
        {
            TextEditorControlBase te = dropdownOwner as TextEditorControlBase;
            if (te == null)
                return;

            DropDownEditorButton dropdown = te.ButtonsRight[0] as DropDownEditorButton;
            if (dropdown == null)
                return;

            UltraGrid grid = new UltraGrid();
            dropdown.Control = grid;

            // Handle selected.
            grid.MouseUp += delegate(object sender, MouseEventArgs e)
            {
                UIElement el = grid.DisplayLayout.UIElement.ElementFromPoint(
                    new Point(e.X, e.Y));

                if (el != null)
                {
                    UltraGridRow row = el.GetContext(typeof (UltraGridRow)) as UltraGridRow;

                    if (row != null)
                    {
                        if (row.GetType() != typeof (UltraGridEmptyRow))
                        {
                            string text = row.Cells[0].Value.ToString();
                            if (text != emptySupplierNotesMessage)
                                dropdownOwner.Text += ((dropdownOwner.Text.Length > 0) ? "\r\n" : "") + text;
                        }
                        dropdown.CloseUp();
                    }
                }
            };

            // Do layout.
            grid.InitializeLayout += delegate(object sender, InitializeLayoutEventArgs e)
            {
                foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
                    c.CellMultiLine = DefaultableBoolean.True;
                GridHelper.SetDefaultGridAppearance(e);
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Rows.Band.HeaderVisible = false;
                e.Layout.Override.RowSelectors = DefaultableBoolean.False;
            };

            // Load data.
            grid.DataSource = GetSupplierNotes(GetSupplierIdForBooking());

            // Set size.
            grid.Height = grid.DisplayRectangle.Height; // GetPreferredSize(new Size(10, 10)).Height;
            grid.Width = txtNoteVoucher.Bounds.Width;

            grid.ActiveRow = null;
        }

        private List<string> GetSupplierNotes(int supplierId)
        {
            if (supplierNotesList != null) // made null when purchase line (supplier) changes
                return supplierNotesList;

            supplierNotesList = new List<string>();
            Supplier supplier = new Supplier();

            Cursor = Cursors.WaitCursor;
            try
            {
                supplierNotesList = supplier.GetSupplierNotes(supplierId);
            }
            catch
            {
                supplierNotesList.Clear();
                supplierNotesList.Add(emptySupplierNotesMessage);
            }
            Cursor = Cursors.Default;

            if (supplierNotesList.Count == 0)
                supplierNotesList.Add(emptySupplierNotesMessage);

            return supplierNotesList;
        }

        private int? GetSelectedPurchaseItemId()
        {
            int? purchaseItemID = null;
            if (gridItems.ActiveRow != null)
            {
                object id = gridItems.ActiveRow.Cells["PurchaseItemID"].Value;
                if (id != DBNull.Value)
                    purchaseItemID = (int?)id;
            }
            return purchaseItemID;
        }

        private int? GetSelectedPurchaseItemPaymentTermId()
        {
            int? paymentTermID = null;
            if (gridItems.ActiveRow != null)
            {
                object id = gridItems.ActiveRow.Cells["PaymentTermID"].Value;
                if (id != DBNull.Value)
                    paymentTermID = (int?)id;
            }
            return paymentTermID;
        }

        private void ChangePurchaseItemOption()
        {
            int? purchaseItemId = GetSelectedPurchaseItemId();
            if (purchaseItemId == null) return;

            ItinerarySet.PurchaseItemRow purchaseItemRow = itinerarySet.PurchaseItem.FindByPurchaseItemID((int)purchaseItemId);
            var optionPicker = new OptionPicker(
                purchaseItemRow.PurchaseLineRow.SupplierID, purchaseItemRow.OptionID, purchaseItemRow.StartDate);
            if (optionPicker.ShowDialog() == DialogResult.OK)
            {
                if (optionPicker.SelectedOption == null) return;
                itinerarySet.PurchaseItem.ChangePurchaseItemOption(
                    purchaseItemRow, optionPicker.SelectedService, optionPicker.SelectedOption, Global.Cache.ToolSet);
            }
        }

        private void EnableDisableControls(UltraGridRow row)
        {
            bool isLocked = (row.Cells["IsLockedAccounting"].Value != DBNull.Value
                            && (bool)row.Cells["IsLockedAccounting"].Value);

            btnDelete.Enabled = !isLocked;
            btnChangeOption.Enabled = !isLocked;

            foreach (Control control in pnlItemDetail.Controls)
            {
                if (control != txtItemDesc &&
                    control != txtItemStartTime &&
                    control != txtItemEndTime &&
                    control != txtItemRef &&
                    control != lblItemDesc &&
                    control != lblItemStartTime &&
                    control != lblItemEndTime &&
                    control != lblItemRef
                    )
                {
                    control.Enabled = !isLocked;

                    if (control == txtItemEndDate 
                        && !isLocked && row.Cells["EndDate"].Value == DBNull.Value) // ensure disabled end date if null
                        txtItemEndDate.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Initializes the values in a PaymentTermsEditor
        /// </summary>
        /// <param name="termsEditor">The PaymentTermsEditor to initialize</param>
        private void InitializePaymentTermsEditor(PaymentTermsEditor termsEditor)
        {
            int? paymentTermID = GetSelectedPurchaseItemPaymentTermId();

            // if there's no existing terms then don't bother
            bool hasExistingTerms = (paymentTermID != null);
            if (hasExistingTerms)
            {
                ItinerarySet.PaymentTermRow paymentTermRow
                    = itinerarySet.PaymentTerm.FindByPaymentTermID((int)paymentTermID);

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
            ItinerarySet.PaymentTermRow paymentTermRow;
            int? paymentTermID = GetSelectedPurchaseItemPaymentTermId();

            // create a new row if there's no existing terms, otherwise use the existing row
            bool hasExistingTerms = (paymentTermID != null);
            if (hasExistingTerms)
            {
                paymentTermRow = itinerarySet.PaymentTerm.FindByPaymentTermID((int)paymentTermID);
            }
            else
            {
                paymentTermRow = itinerarySet.PaymentTerm.NewPaymentTermRow();
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
                        ItinerarySet.PurchaseItemRow purchaseItemRow =
                            itinerarySet.PurchaseItem.FindByPurchaseItemID((int)GetSelectedPurchaseItemId());

                        // add the row to the table
                        itinerarySet.PaymentTerm.AddPaymentTermRow(paymentTermRow);
                        purchaseItemRow.PaymentTermID = paymentTermRow.PaymentTermID;
                    }
                }
            }
        }
        
        private void SetNetSummary()
        {
            UltraGridBand band = gridItems.DisplayLayout.Bands[0];

            // add summary
            if (!band.Summaries.Exists("NetTotal") && band.Columns.Exists("NetTotal"))
            {
                SummarySettings summary = band.Summaries.Add(SummaryType.Formula, band.Columns["NetTotal"]);
                summary.Key = "NetTotal";
                summary.Formula = "Sum([NetBaseTotal])";
                summary.DisplayFormat = "{0:C}";
                summary.Appearance.TextHAlign = HAlign.Right;
                summary.ToolTipText = "Total net sum, in base currency";
                summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                summary.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly;
            }

            // set currency format
            var cm = BindingContext[BindingSource, "PurchaseLineID"] as CurrencyManager;
            if (cm != null && cm.Current is int)
            {
                var ccyCodes = itinerarySet.PurchaseItem.Where(x => x.RowState != DataRowState.Deleted && x.PurchaseLineID == (int)cm.Current).Select(x => x.CurrencyCode).Distinct();

                if (ccyCodes.Count() == 1) // custom format
                {
                    var currencyInfo = CurrencyService.GetCurrency(ccyCodes.First());
                    var format = "{0:" + (currencyInfo != null ? currencyInfo.DisplayFormat : "c") + "}";
                    band.Summaries["NetTotal"].DisplayFormat = format;
                }
                else if (ccyCodes.Count() > 1) // not valid (don't sum multiple currencies)
                {
                    band.Summaries["NetTotal"].DisplayFormat = "n/a";
                }
                else // default format
                {
                    band.Summaries["NetTotal"].DisplayFormat = "{0:C}";
                }
            }
        }

        #endregion

        #region Events

        private void gridItems_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // Hide PurchaseItemCharge table
            if (e.Layout.Bands.Exists("PurchaseItemPurchaseItemCharge"))
                e.Layout.Bands["PurchaseItemPurchaseItemCharge"].Hidden = true;

            if (!e.Layout.Bands[0].Columns.Exists("NetTotal"))
                e.Layout.Bands[0].Columns.Add("NetTotal");
            if (!e.Layout.Bands[0].Columns.Exists("GrossFinal"))
                e.Layout.Bands[0].Columns.Add("GrossFinal");
            if (!e.Layout.Bands[0].Columns.Exists("Discount"))
                e.Layout.Bands[0].Columns.Add("Discount");

            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "PurchaseItemName")
                {
                    c.Header.Caption = "Description";
                    c.Header.ToolTipText = "Description of booking item";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                }
                else if (c.Key == "BookingReference")
                {
                    c.Header.Caption = "Ref";
                    c.Header.ToolTipText = "Suppliers reference";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                }
                else if (c.Key == "RequestStatusID")
                {
                    c.Header.Caption = "Status";
                    c.Header.ToolTipText = "Status of item";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridItems.DisplayLayout.ValueLists["StatusList"];
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "StartDate")
                {
                    c.Header.Caption = "Date";
                    c.Header.ToolTipText = "Item booking date";
                    c.Style = ColumnStyle.Date;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "NumberOfDays")
                {
                    c.Header.Caption = "Nts";
                    c.Header.ToolTipText = "Number of days/nights required";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    
                }
                else if (c.Key == "Quantity")
                {
                    c.Header.Caption = "Qty";
                    c.Header.ToolTipText = "Quantity required";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "Discount")
                {
                    c.Hidden = true; // TODO: feature hidden for now
                    c.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True; // TODO: feature hidden for now
                    c.Header.Caption = "Free";
                    c.Header.ToolTipText = "Discount units (FOC or Stay-Pay)";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "NetTotal")
                {
                    c.Header.Caption = "Net (total)";
                    c.Header.ToolTipText = "Total Net in Supplier currency";
                    c.DataType = typeof(string);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "GrossFinal")
                {
                    c.Header.Caption = "Gross (final)";
                    c.Header.ToolTipText = "Final Total Gross in Itinerary currency";
                    c.DataType = typeof(string);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else
                    c.Hidden = true;
            }

            // layout
            e.Layout.Bands[0].Columns["PurchaseItemName"].Width = 100;
            e.Layout.Bands[0].Columns["BookingReference"].Width = 50;
            e.Layout.Bands[0].Columns["RequestStatusID"].Width = 40;
            e.Layout.Bands[0].Columns["StartDate"].Width = 40;
            e.Layout.Bands[0].Columns["NumberOfDays"].Width = 10;
            e.Layout.Bands[0].Columns["Quantity"].Width = 10;
            e.Layout.Bands[0].Columns["Discount"].Width = 10;
            e.Layout.Bands[0].Columns["NetTotal"].Width = 60;
            e.Layout.Bands[0].Columns["GrossFinal"].Width = 60;

            var index = 0;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["BookingReference"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["RequestStatusID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["StartDate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NumberOfDays"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Quantity"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Discount"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NetTotal"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["GrossFinal"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
            GridHelper.SetDefaultGroupByAppearance(e);

            // add summaries
            BookingsViewer.SetGridSummaries(e, CurrencyService.GetItineraryCurrencyCodeOrDefault(itinerarySet.Itinerary[0]));
            SetNetSummary();
        }

        private void gridItems_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // disable the row if it has been exported to accounting
            var isLocked = (e.Row.Cells["IsLockedAccounting"].Value != DBNull.Value && (bool) e.Row.Cells["IsLockedAccounting"].Value);
            e.Row.Activation = isLocked ? Activation.Disabled : Activation.AllowEdit;
            
            var itemId = (int)e.Row.Cells["PurchaseItemID"].Value;
            var item = itinerarySet.PurchaseItem.Where(i => i.RowState != DataRowState.Deleted && i.PurchaseItemID == itemId).FirstOrDefault();
            if (item == null) return;

            // set base prices
            var hasOverride = CurrencyService.GetPurchaseItemCurrencyCode(item) != null;
            var format = "{0:" + (hasOverride ? CurrencyService.GetCurrency(item.CurrencyCode).DisplayFormat : "c") + "}";
            if (e.Row.Band.Columns.Exists("NetTotal")) e.Row.Cells["NetTotal"].Value = string.Format(format, item.NetTotal);

            //item.DiscountUnits = 1.5;
            //item.SetDiscountUnitsNull
            if (e.Row.Band.Columns.Exists("Discount")) e.Row.Cells["Discount"].Value = !item.IsDiscountUnitsNull() ? item.DiscountUnits.ToString() : ""; ;

            // set final prices
            var itinerary = item.PurchaseLineRow.ItineraryRow;
            hasOverride = CurrencyService.GetItineraryCurrencyCode(itinerary) != null;
            format = "{0:" + (hasOverride ? CurrencyService.GetCurrency(itinerary.CurrencyCode).DisplayFormat : "c") + "}";
            if (e.Row.Band.Columns.Exists("GrossFinal")) e.Row.Cells["GrossFinal"].Value = string.Format(format, item.GrossTotalConverted);
        }

        private void gridItems_AfterRowActivate(object sender, EventArgs e)
        {
            if (!Disposing && itinerarySet != null)
                OnPurchaseItemActivated((int)gridItems.ActiveRow.Cells["PurchaseItemID"].Value);

            EnableDisableControls(gridItems.ActiveRow);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenBookingSupplier();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (gridItems.ActiveRow == null) return;

            var copyRow = itinerarySet.PurchaseItem.FindByPurchaseItemID(
                (int)gridItems.ActiveRow.Cells["PurchaseItemID"].Value);

            if (!App.AskYesNo("Copy booking item: " + copyRow.PurchaseItemName + "?")) return;
            var newRow = itinerarySet.CopyPurchaseItem(
                copyRow, copyRow.PurchaseLineRow.PurchaseLineID, "Copy of", Cache.User.UserID);
            SetActiveRow(newRow.PurchaseItemID);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridItems.ActiveRow == null)
                return;

            if (App.AskDeleteRow())
            {
                // Remember the parent line id.
                int lineId = (int)gridItems.ActiveRow.Cells["PurchaseLineID"].Value;

                // Delete the item.
                GridHelper.DeleteActiveRow(gridItems, true);

                // Delete the parent line if it is now empty.
                ItinerarySet.PurchaseLineRow line = itinerarySet.PurchaseLine.FindByPurchaseLineID(lineId);
                if (line.GetPurchaseItemRows().Length == 0)
                {
                    if (App.AskYesNo("This booking is now empty, do you want to delete the empty booking?"))
                    {
                        line.Delete();
                    }
                }
            }
        }

        private void PurchaseItem_PurchaseItemRowChanged(
            object sender, ItinerarySet.PurchaseItemRowChangeEvent e)
        { 
            // Highlight a new row after it has been added.
            if (e.Action == DataRowAction.Add && gridItems.Rows != null)
            {
                foreach (UltraGridRow row in gridItems.Rows)
                {
                    if ((int)row.Cells["PurchaseItemID"].Value == (int)e.Row["PurchaseItemID"])
                    {
                        gridItems.ActiveRow = row;
                        break;
                    }
                }
            }           
        }

        private void gridItems_Validated(object sender, EventArgs e)
        {
            // To retain changes when grid has focus and navigator position changed.
            gridItems.UpdateData();
        }

        private void txtItemQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !App.IsKeyPressAllowedForNumber(e);
        }

        private void txtItemDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !App.IsKeyPressAllowedForNumber(e);
        }

        private void chkEndDateOverride_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEndDateOverride.Checked)
            {
                txtItemEndDate.Enabled = true;
            }
            else
            {
                txtItemEndDate.Value = null;
                txtItemEndDate.Refresh();
                txtItemEndDate.Enabled = false;
            }
        }

        private void txtNoteVoucher_BeforeEditorButtonDropDown(
            object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            DoSupplierNotesDropdown(sender as Control);
        }

        private void txtNoteSupplier_BeforeEditorButtonDropDown(
            object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            DoSupplierNotesDropdown(sender as Control);
        }

        private void txtNoteClient_BeforeEditorButtonDropDown(object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            DoSupplierNotesDropdown(sender as Control);
        }

        private void txtNotePrivate_BeforeEditorButtonDropDown(object sender, BeforeEditorButtonDropDownEventArgs e)
        {
            DoSupplierNotesDropdown(sender as Control);
        }

        private void txtItemStartDate_AfterDropDown(object sender, EventArgs e)
        {
            //txtItemStartDateBinding = txtItemStartDate.DataBindings["Value"];
            //txtItemStartDate.DataBindings.Clear();
        }

        private void txtItemStartDate_AfterCloseUp(object sender, EventArgs e)
        {
            //txtItemStartDate.DataBindings.Add(txtItemStartDateBinding);
            //txtItemStartDateBinding.WriteValue();
        }

        private void btnEditPrice_Click(object sender, EventArgs e)
        {
            if (gridItems.ActiveRow != null)
                EditCurrentItemPrice();
        }

        private void btnEditTerms_Click(object sender, EventArgs e)
        {
            if (gridItems.ActiveRow != null)
            {
                int purchaseItemID = (int)gridItems.ActiveRow.Cells["PurchaseItemID"].Value;
                ItinerarySet.PurchaseItemRow item = itinerarySet.PurchaseItem.FindByPurchaseItemID(purchaseItemID);
                PaymentTermsEditor termsEditor = new PaymentTermsEditor();
                InitializePaymentTermsEditor(termsEditor);
                OpenPaymentTermsEditor(termsEditor);

                SetItemPriceInfo(item);
            }
        }

        private void btnChangeOption_Click(object sender, EventArgs e)
        {
            ChangePurchaseItemOption();
        }

        #endregion        
    }

    public delegate void OnBookingEditorOpenSupplierHandler(BookingEditorOpenSupplierEventArgs e);
    public class BookingEditorOpenSupplierEventArgs
    {
        public int supplierId;
        public int? optionId;
        public BookingEditorOpenSupplierEventArgs(int supplierId, int? optionId)
        {
            this.supplierId = supplierId;
            this.optionId = optionId;
        }
    }
}
