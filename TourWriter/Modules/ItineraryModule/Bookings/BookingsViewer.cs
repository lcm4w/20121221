using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinCalcManager;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Info.Services;
using TourWriter.Modules.ItineraryModule.Bookings.Email;
using TourWriter.Modules.ItineraryModule.DateKicker;
using TourWriter.Services;
using TourWriter.Utilities;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;
using System.Linq;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class BookingsViewer : UserControl
    {
        private const string GridLayoutVersion = "v2.7"; // bump this (any new name) to cause grid to reset to pick up changes
        public event OnBookingsViewerEditBookingHandler OnOpenBooking;

        private readonly string GridLayoutFileName;

        private ItinerarySet itinerarySet;

        private ItineraryMain itineraryMain;

        internal ItinerarySet ItinerarySet
        {
            get { return itinerarySet; }
            set
            {
                itinerarySet = value;
                SetDataBindings();
                LoadGridLayout();
                GridHelper.SetNumberOfDaysPicker(grid);

                itinerarySet.Itinerary.ColumnChanged += Itinerary_ColumnChanged;
                itinerarySet.PurchaseLine.ColumnChanged += PurchaseLine_ColumnChanged;
                itinerarySet.PurchaseItem.RowChanged += PurchaseItem_RowChanged;
                itinerarySet.PurchaseItem.RowDeleted += PurchaseItem_RowDeleted;
                itinerarySet.PurchaseItem.ColumnChanged += PurchaseItem_ColumnChanged;
            }
        }

        public BookingsViewer()
        {
            InitializeComponent();

            GridLayoutFileName = App.Path_UserApplicationData + "BookingGridLayout.xml";
            HandleDestroyed += BookingsViewer_HandleDestroyed;
            itineraryMain = ParentForm as ItineraryMain;

            // TODO: hide flags feature for now, see also line 1004 ---
            btnEditFlags.Visible = editFlagsToolStripMenuItem.Visible = false;
    		Initialize_DragDrop();
        }

        private void BookingsViewer_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                //btnLockEdit.Enabled = AppPermissions.UserHasPermission(AppPermissions.Permissions.AccountingEdit);
                RefreshEmailTemplateMenu();
            }
        }
        
        internal void RefreshEmailTemplateMenu()
        {
            btnBookings.DropDownItems.Clear();

            AddEmailTemplateMenuItem(btnBookings, null); // default

            var templates = Cache.ToolSet.Template.Where(t => t.RowState != DataRowState.Deleted &&
                t.ParentTemplateCategoryID == App.TemplateCategoryBookingEmail).OrderBy(t => t.TemplateName).ToList();
            if (templates.Count() > 0)
            {
                btnBookings.DropDownItems.Add(new ToolStripSeparator());
                foreach (var template in templates)
                {
                    var customTemplateButton = new ToolStripMenuItem(template.TemplateName);
                    AddEmailTemplateMenuItem(customTemplateButton, template);
                    btnBookings.DropDownItems.Add(customTemplateButton);
                }
            }
            btnBookings.DropDownItems.Add(new ToolStripSeparator());
            btnBookings.DropDownItems.Add("Add template...", null, btnAddNewTemplate_Click);
        }

        private void AddEmailTemplateMenuItem(ToolStripDropDownItem parent, ToolSet.TemplateRow customTemplate)
        {
            var b1 = parent.DropDownItems.Add("Book all..", null, btnBookAll_Click);
            var b2 = parent.DropDownItems.Add("Book selected...", null, btnBookSelected_Click);

            if (customTemplate != null)
            {
                b1.Tag = b2.Tag = customTemplate.FilePath;
                parent.DropDownItems.Add(new ToolStripSeparator());
                var b = parent.DropDownItems.Add("Remove template", null, btnRemoveTemplate_Click);
                b.Tag = customTemplate.TemplateID;
            }
        }

        private void SetDataBindings()
        {
            // Purchase item request status
            if (!grid.DisplayLayout.ValueLists.Exists("StatusList"))
            {
                grid.DisplayLayout.ValueLists.Add("StatusList");
                grid.DisplayLayout.ValueLists["StatusList"].SortStyle = ValueListSortStyle.Ascending;
                grid.DisplayLayout.ValueLists["StatusList"].ValueListItems.Add(DBNull.Value, "(none)");
                foreach (ToolSet.RequestStatusRow r in Cache.ToolSet.RequestStatus.Rows)
                {
                    if (r.RowState == DataRowState.Deleted) continue;
                    grid.DisplayLayout.ValueLists["StatusList"].ValueListItems.Add(r.RequestStatusID, r.RequestStatusName);
                }
            }
            var list = Cache.ToolSet.Currency.Where(c => c.Enabled).ToList();
            var nullRow = Cache.ToolSet.Currency.NewCurrencyRow();
            nullRow.CurrencyCode = nullRow.DisplayName = "";
            list.Insert(0, nullRow);

            grid.DataSource = itinerarySet.PurchaseItem;
            itineraryBindingSource.DataSource = itinerarySet.Itinerary;
            txtPriceOverride.ReadOnly = chkLockGrossOverride.Checked;
            SetNetOverrideText();
            SetPriceOverrideWarning();

            // if current grid version not found (in cols), reset the grid (to remove old version)
            if (!grid.DisplayLayout.Bands[0].Columns.Exists(GridLayoutVersion))
                ResetGridLayout();
        }

        #region CCY

        //private void HandleCurrencyCodeChanged()
        //{
        //    var newValue = cmbCurrency.SelectedValue != null ? cmbCurrency.SelectedValue.ToString() : null;
        //    if (itinerarySet.Itinerary[0].CurrencyCode != newValue)
        //    {
        //        if (newValue != null && newValue != "" && newValue.Length != 3)
        //            throw new ArgumentException("Invalid currency code: " + newValue);
        //        itinerarySet.Itinerary[0].CurrencyCode = newValue;
        //    }

        //    // update UI
        //    SetItineraryCurrencyInfo();
        //    FormatFinalYieldText();

        //    // set reports param
        //    (ParentForm as ItineraryMain).SetItineraryReportsParameter("@CurrencyCode", newValue); 
        //}


        internal void SetItineraryCurrencyInfo()
        {
            var itinerary = itinerarySet.Itinerary[0];
            var currencyInfo = CurrencyService.GetCurrency(itinerary.CurrencyCode);
            var pattern = currencyInfo != null ? currencyInfo.DisplayFormat : "c";

            // final prices
            var format = pattern;
            txtPriceOverride.FormatString = format;
            txtGross2.FormatString = format;
            txtGross1.FormatString = format;
            txtSell.FormatString = format;
            txtItineraryCurrency.Text =
                (currencyInfo == null || CurrencyService.GetApplicationCurrencyCode() == currencyInfo.CurrencyCode) ?
                "default: " + CurrencyService.GetApplicationCurrencyCode() : currencyInfo.DisplayName;

            // grid summaries
            format = "{0:" + pattern + "}";
            grid.DisplayLayout.Bands[0].Summaries["NetFinal"].DisplayFormat = format;
            grid.DisplayLayout.Bands[0].Summaries["GrossFinal"].DisplayFormat = format;

            // grid rows
            RefreshGrid();

            (ParentForm as ItineraryMain).clientEditor.SetCurrencyFormat();
        }

        private void lnkItineraryCurrency_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RunCurrencyUpdater();
        }


        internal void RunCurrencyUpdater()
        {
            var currencyUpdater = new CurrencyUpdater(itinerarySet);
            if (currencyUpdater.ShowDialog() != DialogResult.OK) return;

            // update purchase item prices
            itinerarySet.PurchaseItem.ColumnChanged -= PurchaseItem_ColumnChanged;
            foreach (var row in Enumerable.Where(itinerarySet.PurchaseItem, row => row.RowState != DataRowState.Deleted))
                row.RecalculateTotals();
            itinerarySet.PurchaseItem.ColumnChanged += PurchaseItem_ColumnChanged;

            RefreshGrid();
            RecalculateFinalPricing();
            WarnIfPriceOverrideExists();
            SetItineraryCurrencyInfo();
        }

        #endregion
        
        private void LoadGridLayout()
        {
            /** Load from file **/
            if (File.Exists(GridLayoutFileName))
            {
                grid.DisplayLayout.LoadFromXml(
                    GridLayoutFileName,
                    PropertyCategories.Groups | PropertyCategories.SortedColumns);

                SetGridExpanded();
            }
            else
                grid.DisplayLayout.Bands[0].SortedColumns.Add("StartDate", false);

            // check accting permission
            grid.DisplayLayout.Bands[0].Columns["IsLockedAccounting"].CellActivation =
                AppPermissions.UserHasPermission(AppPermissions.Permissions.AccountingEdit) ? Activation.AllowEdit : Activation.Disabled;

            // custom sort comparer for date column
            grid.DisplayLayout.Bands[0].Columns["StartDate"].SortComparer = new DateSortComparer();

            // custom sort comparer for time column
            grid.DisplayLayout.Bands[0].Columns["StartTime"].SortComparer = new TimeSortComparer();
        }

        private void SaveGridLayout()
        {
            try
            {
                /** Save to file **/
                grid.DisplayLayout.SaveAsXml(
                    GridLayoutFileName,
                    PropertyCategories.Groups | PropertyCategories.SortedColumns);
            }
            catch (Exception ex)
            {
                ErrorHelper.SendEmail(ex, true);
            }
        }

        internal void SetGridExpanded()
        {
            // Expand all rows in the grid.
            foreach (UltraGridRow row in grid.Rows)
                row.ExpandAll();

            btnExpand.Text = "Collapse";
            btnExpand.Image = TourWriter.Properties.Resources.Collapse;
            btnExpand.ToolTipText = "Collapse all group rows";
        }

        private void SetGridCollapsed()
        {
            // Collapse all rows in the grid.
            foreach (UltraGridRow row in grid.Rows)
                row.CollapseAll();

            btnExpand.Text = "Expand";
            btnExpand.Image = TourWriter.Properties.Resources.Expand;
            btnExpand.ToolTipText = "Expand all group rows";
        }

        private void OpenBookingEditor(int? purchaseLineId, int? purchaseItemId)
        {
            // Let listeners handle this event.
            grid.PerformAction(UltraGridAction.ExitEditMode);
            if (OnOpenBooking != null)
                OnOpenBooking(new BookingsViewerEditBookingEventArgs(purchaseLineId, purchaseItemId));
        }

        internal static void SetRowLocked(UltraGridRow row, bool locked)
        {
            var excludeCols = new[] { "IsLockedAccounting", "PurchaseItemName", "BookingReference", "StartTime", "EndTime", "IsInvoiced" };
            foreach (var column in row.Band.Columns.Cast<UltraGridColumn>().Where(column => !excludeCols.Contains(column.Key)))
                row.Cells[column].Activation = (locked) ? Activation.Disabled : Activation.AllowEdit;
        }

        internal static void SetGridSummaries(InitializeLayoutEventArgs e, string itineraryCurrencyCode)
        {
            e.Layout.Grid.CalcManager = new UltraCalcManager(e.Layout.Grid.Container);

            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells; // show groupby summaries

            SummarySettings summary;
            //if (!band.Summaries.Exists("GroupGross") && band.Columns.Exists("GrossTotalConverted"))
            //{
            //    // Group gross total. 
            //    summary = band.Summaries.Add(SummaryType.Sum, band.Columns["GrossTotalConverted"]);
            //    summary.Key = "GroupGross";
            //    summary.DisplayFormat = "{0:C}";// #0.00}";
            //    summary.Appearance.TextHAlign = HAlign.Right;
            //    summary.ToolTipText = "Total gross sum, in Itinerary currency";
            //    summary.SummaryDisplayArea = SummaryDisplayAreas.InGroupByRows; // group rows only
            //    summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            //}
            if (!band.Summaries.Exists("NetFinal") && band.Columns.Exists("NetFinal"))
            {
                // Net total.
                summary = band.Summaries.Add(SummaryType.Formula, band.Columns["NetFinal"]);
                summary.Key = "NetFinal";
                summary.DisplayFormat = "{0:C}";
                summary.Appearance.TextHAlign = HAlign.Right;
                summary.Formula = "Sum([NetTotalConverted])";
                summary.ToolTipText = "Total net sum, in Itinerary currency";
                summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                summary.SummaryDisplayArea = // fixed at bottom of grid
                    SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly | SummaryDisplayAreas.InGroupByRows;

                // groupby row summaries
                summary.GroupBySummaryValueAppearance.TextHAlign = HAlign.Right;
                summary.GroupBySummaryValueAppearance.FontData.Bold = DefaultableBoolean.True;
                summary.GroupBySummaryValueAppearance.ForeColor = Color.DimGray;
            }

            if (!band.Summaries.Exists("GrossFinal") && band.Columns.Exists("GrossFinal"))
            {
                // Gross total.
                summary = band.Summaries.Add(SummaryType.Formula, band.Columns["GrossFinal"]);
                summary.Key = "GrossFinal";
                summary.DisplayFormat = "{0:C}";
                summary.Appearance.TextHAlign = HAlign.Right;
                summary.Formula = "Sum([GrossTotalConverted])";
                summary.ToolTipText = "Total gross sum, in Itinerary currency";
                summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                summary.SummaryDisplayArea = // fixed at bottom of grid
                    SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly | SummaryDisplayAreas.InGroupByRows;

                // groupby row summaries
                summary.GroupBySummaryValueAppearance.TextHAlign = HAlign.Right;
                summary.GroupBySummaryValueAppearance.FontData.Bold = DefaultableBoolean.True;
                summary.GroupBySummaryValueAppearance.ForeColor = Color.DimGray;
            }
            if (!band.Summaries.Exists("BaseCurrency") && band.Columns.Exists("BaseCurrency"))
            {
                // Gross total.
                summary = band.Summaries.Add(SummaryType.Formula, band.Columns["BaseCurrency"]);
                summary.Key = "BaseCurrency";
                summary.DisplayFormat = itineraryCurrencyCode;
                summary.ToolTipText = "Output currency of the itinerary";
                summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                summary.SummaryDisplayArea = // fixed at bottom of grid
                    SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly;
            }
        }

        public void SearchBookings()
        {
            BookingSelectorForm bookingSelector = new BookingSelectorForm(itinerarySet, itineraryMain);
            bookingSelector.SetPage(BookingSelectorForm.PageType.Search);

            bookingSelector.ShowDialog();
            bookingSelector.Dispose();
        }

        private bool isLoadingSupplier;
        public void AddNewBooking(int supplierId, ItineraryMain itineraryMain)
        {
            this.itineraryMain = itineraryMain;
            // added to disable multiple drag-drop events queuing up
            if (isLoadingSupplier)
                return;

            isLoadingSupplier = true;
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            BookingSelectorForm bookingSelector = new BookingSelectorForm(itinerarySet, itineraryMain);
            bookingSelector.LoadSupplier(supplierId, null, null);
            bookingSelector.SetPage(BookingSelectorForm.PageType.Select);

            Cursor = Cursors.Default;
            Application.DoEvents();
            isLoadingSupplier = false;

            bookingSelector.ShowDialog();
            bookingSelector.Dispose();
        }

        private void SetActiveRow(RowsCollection rows, int purchaseItemId)
        {
            foreach (UltraGridRow row in rows)
            {
                if (row.IsGroupByRow)
                {
                    UltraGridGroupByRow groupByRow = row as UltraGridGroupByRow;
                    if (groupByRow != null)
                        SetActiveRow(groupByRow.Rows, purchaseItemId);
                }
                else
                {
                    if ((int)row.Cells["PurchaseItemID"].Value == purchaseItemId)
                    {
                        row.Activate();
                        grid.Selected.Rows.Clear();
                        row.Selected = true;
                        return;
                    }
                }
            }
        }

        private void SendBookingRequest(List<int> idList, string defaultTemplate)
        {
            var isDirty = idList.Any(id => id < 0);
            if (isDirty)
                App.ShowWarning("Please save your bookings before sending booking email(s).");
            else
            {
                var bookingEmailer = new BookingEmailForm(itinerarySet, idList, defaultTemplate);
                bookingEmailer.Show();
			}
        }

        internal void SetBindingContext(BindingContext bindingContext)
        {
            BindingContext = bindingContext;
        }

        private static void ExpandRowGroup(UltraGridRow row)
        {
            // Expand group by row.
            UltraGridRow groupRow;
            groupRow = row.IsGroupByRow ? row : row.ParentRow ?? null;

            if (groupRow != null)
                groupRow.ExpandAll();
        }

        internal void RefreshGrid()
        {
            grid.Rows.Refresh(RefreshRow.FireInitializeRow, true);
        }

        internal UltraGridLayout GridLayout
        {
            get
            {
                return grid.DisplayLayout.Clone(PropertyCategories.Groups);
            }
            set
            {
                grid.DisplayLayout.Load(value, PropertyCategories.Groups);
            }
        }

        internal UltraGrid Grid
        {
            get { return grid; }
        }

        private void WarnIfPriceOverrideExists()
        {
            bool hasOverride = !itinerarySet.Itinerary[0].IsGrossOverrideNull();
            if (hasOverride)
            {
                string price = decimal.Round(itinerarySet.Itinerary[0].GrossOverride, 2).ToString();
                price = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol + price;

                string s = App.GetResourceString("PriceOverrideConflict");
                if (App.AskYesNo(String.Format(s, price)))
                {
                    chkLockGrossOverride.Checked = false;

                    // remove override
                    txtPriceOverride.Value = null;
                    itinerarySet.Itinerary[0].SetGrossOverrideNull();
                    itinerarySet.Itinerary[0].IsLockedGrossOverride = false;

                    RecalculateFinalPricing();
                }
            }
        }

        private void SetPriceOverrideWarning()
        {
            if (itinerarySet.Itinerary[0].HasPriceOverrides)
            {
                overrideWarning.SetError(label8, "Adjusted sell price, base price has overrides.");
                overrideWarning.SetIconAlignment(label8, ErrorIconAlignment.BottomRight);
                overrideWarning.SetIconPadding(label8, 3);
            }
            else overrideWarning.SetError(label8, ""); // reset
        }

        private void EditNetOverride()
        {
            NetOverrideForm overrideForm = new NetOverrideForm(itinerarySet);
            overrideForm.ShowDialog();
            SetNetOverrideText();
            txtNetOverride.Editor.ExitEditMode(true, false);
        }

        private void SetNetOverrideText()
        {
            // check if there is at least one row in the table (that hasn't been deleted)
            bool hasRows = false;
            foreach (ItinerarySet.ItineraryMarginOverrideRow row in itinerarySet.ItineraryMarginOverride.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    hasRows = true;
                    break;
                }
            }

            if (hasRows)
            {
                // custom overrides exist
                txtNetOverride.Value = null;
                txtNetOverride.NullText = "(custom)";
            }
            else
            {
                // default empty
                txtNetOverride.NullText = String.Empty;
            }
            RecalculateFinalPricing();
        }

        private decimal yieldAmount;
        private decimal yieldPercent;
        public void RecalculateFinalPricing()
        {
            // -- testing -- WARNING: might be out of date after Seba changes to this method body on 13 Sept 2012.

            //var hasOverrides = !itinerarySet.Itinerary[0].IsNetMarginNull() || itinerarySet.ItineraryMarginOverride.Rows.Count > 0;

            //// service-type overrides
            //decimal postServiceType;
            //if (hasOverrides)
            //{
            //    var basePrice = itinerarySet.GetNetBasePrice();
            //    var markup = itinerarySet.GetNetMarkup();
            //    postServiceType = basePrice * (1 + markup / 100);
            //    App.Debug(string.Format("Recalc Itinerary, base: {0}, markup: {1}, price: {2}", basePrice, markup, postServiceType));
            //}
            //else
            //{
            //    postServiceType = itinerarySet.GetGrossBasePrice();
            //    App.Debug(string.Format("Recalc Itinerary, price: {0}", postServiceType));
            //}
            //txtGross1.Value = postServiceType;

            // -------------

            var net = itinerarySet.GetNetBasePrice();
            var final = itinerarySet.GetGrossFinalPrice();
            var hasMasterOverride = !itinerarySet.Itinerary[0].IsNetMarginNull(); // override-all
            var hasServiceTypeOverrides = itinerarySet.ItineraryMarginOverride.Rows.Count > 0; // override per service type

            if (hasMasterOverride || hasServiceTypeOverrides) // has any overrides
            {
                // special case when override-all is 'grs' (discount)
                if (hasMasterOverride && !itinerarySet.Itinerary[0].IsNetComOrMupNull() && itinerarySet.Itinerary[0].NetComOrMup == "grs")
                {
                    var gross = itinerarySet.GetGrossBasePrice();
                    var margin = itinerarySet.GetMarginOverride();
                    txtGross1.Value = Common.CalcGrossByGrossCommission(gross, margin);
                }
                else // master override-all com or mup (not grs), or any override at service type level
                {
                    var markup = itinerarySet.GetMarginOverride();
                    txtGross1.Value = Common.CalcGrossByNetMarkup(net, markup);
                }
            }
            else // no margin overrides
            {
                var gross = itinerarySet.GetGrossBasePrice();
                txtGross1.Value = gross;
            }

            // process any final markup/down
            if (!itinerarySet.Itinerary[0].IsGrossMarkupNull())
            {
                var val = decimal.Parse(txtGross1.Value.ToString());
                txtGross2.Value = val * (1 + itinerarySet.Itinerary[0].GrossMarkup / 100);
            }
            else // no final markup/down override
            {
                txtGross2.Value = txtGross1.Value;
            }

            // process final sell price and yield
            yieldAmount = final - net;
            yieldPercent = Common.CalcCommissionByNetGross(net, final);
            txtSell.Value = final;
            FormatFinalYieldText();
        }

        internal void FormatFinalYieldText()
        {
            var itinerary = itinerarySet.Itinerary[0];
            var currencyInfo = CurrencyService.GetCurrency(itinerary.CurrencyCode);
            var format = "{0:" + (currencyInfo != null ? currencyInfo.DisplayFormat : "c") + "}";

            var amount = string.Format(format, yieldAmount);
            var percent = (yieldPercent / 100).ToString("p");
            txtCommission.Text = string.Format("{0} ({1})", amount, percent);
        }

        private int? GetSelectedPurchaseItemId()
        {
            int? purchaseItemID = null;
            if (grid.ActiveRow != null && !(grid.ActiveRow is UltraGridGroupByRow))
            {
                object id = grid.ActiveRow.Cells["PurchaseItemID"].Value;
                if (id != DBNull.Value)
                    purchaseItemID = (int?)id;
            }
            return purchaseItemID;
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

        private void EnableDisableButtons(UltraGridRow row)
        {
            if (row == null || row is UltraGridGroupByRow)
                return;

            var isLocked = row.Cells.Exists("IsLockedAccounting") &&
                row.Cells["IsLockedAccounting"] != null &&
                row.Cells["IsLockedAccounting"].Value != DBNull.Value &&
                row.Cells["IsLockedAccounting"].Value != null &&
                (bool)row.Cells["IsLockedAccounting"].Value;

            btnCopy.Enabled = !row.IsGroupByRow;
            btnDelete.Enabled = !(row.IsGroupByRow || isLocked);
            btnChangeOption.Enabled = !isLocked;
        }

        private void ResetGridLayout()
        {
            File.Delete(GridLayoutFileName);
            grid.DataSource = null;
            grid.DataSource = itinerarySet.PurchaseItem;
            SetItineraryCurrencyInfo();
        }

        private bool ValidatePurchaseItem(ItinerarySet.PurchaseItemRow item)
        {
            var i = 1;
            var message = string.Empty;

            // rates
            if (!item.IsStartDateNull())
            {
                var option = itinerarySet.OptionLookup.FindByOptionID(item.OptionID);
                if (option != null)
                {
                    var numDays = !item.IsNumberOfDaysNull() ? item.NumberOfDays : 0;

                    if (item.StartDate.Date < option.ValidFrom.Date || item.StartDate.Date > option.ValidTo.Date)
                        message += i++ + ". Start date does not match rates\r\n";
                    else if (item.StartDate.AddDays(numDays).Date > option.ValidTo.Date)
                        message += i++ + ". End date does not match rates\r\n";
                }
            }

            // discounts
            var calcDiscount = Discounts.CalcDiscount((decimal)item.Quantity, item.GetDiscountRows());
            if ((decimal)item.DiscountUnits != calcDiscount)
            {
                message += string.Format(i++ + ". Discount does not match rates (should be: {0})\r\n", calcDiscount);
            }

            // currencies
            if (!item.IsCurrencyCodeNull() && !string.IsNullOrEmpty(item.CurrencyCode.Trim()) &&
                item.IsCurrencyRateNull() &&
                item.CurrencyCode != CurrencyService.GetItineraryCurrencyCodeOrDefault(itinerarySet.Itinerary[0]))
            {
                message += i++ + ". Currency conversion rate is missing\r\n";
            }

            // apply
            item.RowError = message;
            return message.Length == 0;
        }

        private void SetFlags(UltraGridRow row)
        {
            var purchaseItemId = (int)row.Cells["PurchaseItemID"].Value;
            var notes = itinerarySet.PurchaseItemNote.Where(note => note.RowState != DataRowState.Deleted &&
                                                                    note.PurchaseItemID == purchaseItemId);

            var flagImageList = new List<Bitmap>();
            var message = String.Empty;

            foreach (var note in notes)
            {
                // add the flag
                var flag = Cache.ToolSet.Flag.FindByFlagID(note.FlagID);
                flagImageList.Add((Bitmap)ImageHelper.ByteArrayToImage(flag.FlagImage));

                // append to the message
                message += note.Note + "\r\n";
            }

            if (row.Cells.Exists("Flags"))
            {
                row.Cells["Flags"].Value = ImageHelper.CombineBitmaps(flagImageList);
                row.Cells["Flags"].ToolTipText = message;
            }
        }

        private static void AddTemplate(string name, string file)
        {
            var template = Cache.ToolSet.Template.NewTemplateRow();
            template.TemplateName = name;
            template.FilePath = file;
            template.ParentTemplateCategoryID = App.TemplateCategoryBookingEmail;
            Cache.ToolSet.Template.AddTemplateRow(template);
        }

        void BookingsViewer_HandleDestroyed(object sender, EventArgs e)
        {
            SaveGridLayout();
        }

        #region Events

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // Hide all but the PurchaseItem table
            foreach (UltraGridBand band in e.Layout.Bands)
                band.Hidden = (band.Key != "PurchaseItem");

            // Add custom rows
            if (!e.Layout.Bands[0].Columns.Exists(GridLayoutVersion))
                e.Layout.Bands[0].Columns.Add(GridLayoutVersion);
            if (!e.Layout.Bands[0].Columns.Exists("PurchaseLineName"))
                e.Layout.Bands[0].Columns.Add("PurchaseLineName");
            if (!e.Layout.Bands[0].Columns.Exists("CityName"))
                e.Layout.Bands[0].Columns.Add("CityName");
            if (!e.Layout.Bands[0].Columns.Exists("RegionName"))
                e.Layout.Bands[0].Columns.Add("RegionName");
            if (!e.Layout.Bands[0].Columns.Exists("Grade"))
                e.Layout.Bands[0].Columns.Add("Grade");
            if (!e.Layout.Bands[0].Columns.Exists("GradeExternal"))
                e.Layout.Bands[0].Columns.Add("GradeExternal");
            if (!e.Layout.Bands[0].Columns.Exists("Flags"))
                e.Layout.Bands[0].Columns.Add("Flags");
            if (!e.Layout.Bands[0].Columns.Exists("NetUnit"))
                e.Layout.Bands[0].Columns.Add("NetUnit");
            if (!e.Layout.Bands[0].Columns.Exists("GrossUnit"))
                e.Layout.Bands[0].Columns.Add("GrossUnit");
            if (!e.Layout.Bands[0].Columns.Exists("NetTotal"))
                e.Layout.Bands[0].Columns.Add("NetTotal");
            if (!e.Layout.Bands[0].Columns.Exists("GrossTotal"))
                e.Layout.Bands[0].Columns.Add("GrossTotal");
            if (!e.Layout.Bands[0].Columns.Exists("NetFinal"))
                e.Layout.Bands[0].Columns.Add("NetFinal");
            if (!e.Layout.Bands[0].Columns.Exists("GrossFinal"))
                e.Layout.Bands[0].Columns.Add("GrossFinal");
            if (!e.Layout.Bands[0].Columns.Exists("DefaultEndDate"))
                e.Layout.Bands[0].Columns.Add("DefaultEndDate");
            if (!e.Layout.Bands[0].Columns.Exists("BaseCurrency"))
                e.Layout.Bands[0].Columns.Add("BaseCurrency");
            if (!e.Layout.Bands[0].Columns.Exists("BookingCurrency"))
                e.Layout.Bands[0].Columns.Add("BookingCurrency");
            if (!e.Layout.Bands[0].Columns.Exists("CustomSort"))
                e.Layout.Bands[0].Columns.Add("CustomSort");

            // Show/hide columns )
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "PurchaseLineID")
                {
                    c.Header.Caption = "BkID";
                    c.Header.ToolTipText = "Booking ID";
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "PurchaseLineName")
                {
                    c.Header.Caption = "Booking name";
                    c.Header.ToolTipText = "Booking supplier name";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = true;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "PurchaseItemName")
                {
                    c.Header.Caption = "Item description";
                    c.Header.ToolTipText = "Description of the booking item";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = true;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "ServiceName")
                {
                    c.Header.Caption = "Service name";
                    c.Header.ToolTipText = "The service name, from the underlying Service";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "OptionTypeName")
                {
                    c.Header.Caption = "Option type";
                    c.Header.ToolTipText = "The option type, from Supplier/Service Options";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "CityName")
                {
                    c.Header.Caption = "City";
                    c.Header.ToolTipText = "City of supplier";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "RegionName")
                {
                    c.Header.Caption = "Region";
                    c.Header.ToolTipText = "Region of supplier";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "ServiceTypeName")
                {
                    c.Header.Caption = "Type";
                    c.Header.ToolTipText = "Service type of the booking";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "BookingReference")
                {
                    c.Header.Caption = "Ref";
                    c.Header.ToolTipText = "Suppliers reference";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "RequestStatusID")
                {
                    c.Header.Caption = "Status";
                    c.Header.ToolTipText = "Status of item";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = grid.DisplayLayout.ValueLists["StatusList"];
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
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
                    c.TabStop = true;
                    // custom sort comparer for date column
                    c.SortComparer = new DateSortComparer();

                }
                else if (c.Key == "StartTime")
                {
                    c.Header.Caption = "Time";
                    c.Header.ToolTipText = "Item booking time";
                    c.Style = ColumnStyle.TimeWithSpin;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                    // custom sort comparer for time column
                    c.SortComparer = new TimeSortComparer();
                }
                else if (c.Key == "SortDate")
                {
                    c.Header.Caption = "Sort Date";
                    c.Header.ToolTipText = "Sort Date";
                    c.Style = ColumnStyle.DateTime;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "CustomSort")
                {
                    c.Header.Caption = "Sort";
                    c.Header.ToolTipText = "Custom sort order";
                    c.Style = ColumnStyle.DateTime;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "DefaultEndDate")
                {
                    c.Header.Caption = "End Date";
                    c.Header.ToolTipText = "The booking end date (calculated if not overridden)";
                    c.Style = ColumnStyle.Date;
                    c.MergedCellContentArea = MergedCellContentArea.VirtualRect;
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = true;
                    // custom sort comparer for date column
                    c.SortComparer = new DateSortComparer();
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "NumberOfDays")
                {
                    c.Header.Caption = "Nts";
                    c.Header.ToolTipText = "Number of days/nights required";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.TabStop = true;
                }
                else if (c.Key == "Quantity")
                {
                    c.Header.Caption = "Qty";
                    c.Header.ToolTipText = "Quantity required";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                    c.TabStop = true;
                }
                else if (c.Key == "DiscountUnits")
                {
                    //c.Hidden = true; c.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True; // TODO: feature hidden for now
                    c.Header.Caption = "Free";
                    c.Header.ToolTipText = "Discount units (FOC or Stay-Pay)";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = true;
                }
                else if (c.Key == "Grade")
                {
                    c.Header.Caption = "Grade1";
                    c.Header.ToolTipText = "Supplier grade 1";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "GradeExternal")
                {
                    c.Header.Caption = "Grade2";
                    c.Header.ToolTipText = "Supplier grade 2";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "BaseCurrency")
                {
                    c.Header.Caption = "Itin Currency";
                    c.Header.ToolTipText = "Base currency of the itinerary";
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "BookingCurrency")
                {
                    c.Header.Caption = "Bkg Currency";
                    c.Header.ToolTipText = "Currency of the booking or service";
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "CurrencyRate")
                {
                    c.Header.Caption = "Exch rate";
                    c.Header.ToolTipText = "The currency exchange rate";
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Format = "##0.0000";
                    c.NullText = "1.000";
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "NetUnit")
                {
                    c.Header.Caption = "Net (unit)";
                    c.Header.ToolTipText = "Base net unit cost in Supplier base currency";
                    c.DataType = typeof(string);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "GrossUnit")
                {
                    c.Header.Caption = "Gross (unit)";
                    c.Header.ToolTipText = "Base gross unit cost in Supplier base currency";
                    c.DataType = typeof(string);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "NetTotal")
                {
                    c.Header.Caption = "Net (total)";
                    c.Header.ToolTipText = "Base net total cost in Supplier base currency";
                    c.DataType = typeof(string);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "GrossTotal")
                {
                    c.Header.Caption = "Gross (total)";
                    c.Header.ToolTipText = "Base gross total cost in Supplier base currency";
                    c.DataType = typeof(string);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                    c.Hidden = true; // default hide
                }
                else if (c.Key == "NetFinal")
                {
                    c.Header.Caption = "Net (final)";
                    c.Header.ToolTipText = "Final net total cost in Itinerary base currency";
                    c.DataType = typeof(string);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "GrossFinal")
                {
                    c.Header.Caption = "Gross (final)";
                    c.Header.ToolTipText = "Final gross total cost in Itinerary base currency";
                    c.DataType = typeof(string);
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "IsLockedAccounting")
                {
                    c.Header.Caption = String.Empty;
                    c.Header.Appearance.Image = Properties.Resources.LockEdit;
                    c.Header.Appearance.ImageHAlign = HAlign.Right;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = false;
                    c.Hidden = true;
                    c.ColumnChooserCaption = "Accounting";
                }
                else if (c.Key == "Flags")
                {
                    c.Hidden = true; c.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True; // TODO: feature hidden for now, see also line 66
                    c.Header.Caption = "";
                    c.Header.ToolTipText = "Your custom warning messages";
                    c.DataType = typeof(Bitmap);
                    c.CellAppearance.ImageHAlign = HAlign.Left;
                    c.CellAppearance.ImageVAlign = VAlign.Middle;
                }
                else if (c.Key == "IsInvoiced")
                {
                    c.Header.Caption = "Invoiced";
                    c.Header.ToolTipText = "Is the booking invoiced. Double-click to edit.";
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                {
                    c.Hidden = true;
                    c.TabStop = false;
                    c.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                }
            }

            e.Layout.Bands[0].Columns["Flags"].Width = 23;
            e.Layout.Bands[0].Columns["IsLockedAccounting"].Width = 22;
            e.Layout.Bands[0].Columns["PurchaseLineID"].Width = 40;
            e.Layout.Bands[0].Columns["PurchaseLineName"].Width = 150;
            e.Layout.Bands[0].Columns["ServiceName"].Width = 100;
            e.Layout.Bands[0].Columns["OptionTypeName"].Width = 100;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Width = 150;
            e.Layout.Bands[0].Columns["OptionTypeName"].Width = 80;
            e.Layout.Bands[0].Columns["CityName"].Width = 80;
            e.Layout.Bands[0].Columns["RegionName"].Width = 80;
            e.Layout.Bands[0].Columns["ServiceTypeName"].Width = 80;
            e.Layout.Bands[0].Columns["BookingReference"].Width = 80;
            e.Layout.Bands[0].Columns["RequestStatusID"].Width = 80;
            e.Layout.Bands[0].Columns["StartDate"].Width = 80;
            e.Layout.Bands[0].Columns["StartTime"].Width = 80;
            e.Layout.Bands[0].Columns["SortDate"].Width = 150;
            e.Layout.Bands[0].Columns["CustomSort"].Width = 150;
            e.Layout.Bands[0].Columns["DefaultEndDate"].Width = 80;
            e.Layout.Bands[0].Columns["NumberOfDays"].Width = 30;
            e.Layout.Bands[0].Columns["Quantity"].Width = 30;
            e.Layout.Bands[0].Columns["DiscountUnits"].Width = 32;
            e.Layout.Bands[0].Columns["Grade"].Width = 50;
            e.Layout.Bands[0].Columns["GradeExternal"].Width = 50;
            e.Layout.Bands[0].Columns["BaseCurrency"].Width = 40;
            e.Layout.Bands[0].Columns["BookingCurrency"].Width = 40;
            e.Layout.Bands[0].Columns["CurrencyRate"].Width = 50;
            e.Layout.Bands[0].Columns["NetUnit"].Width = 120;
            e.Layout.Bands[0].Columns["GrossUnit"].Width = 120;
            e.Layout.Bands[0].Columns["NetTotal"].Width = 120;
            e.Layout.Bands[0].Columns["GrossTotal"].Width = 120;
            e.Layout.Bands[0].Columns["NetFinal"].Width = 120;
            e.Layout.Bands[0].Columns["GrossFinal"].Width = 120;
            e.Layout.Bands[0].Columns["IsInvoiced"].Width = 60;

            var index = 0;
            e.Layout.Bands[0].Columns["Flags"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["IsLockedAccounting"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseLineID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseLineName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["CityName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["RegionName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ServiceTypeName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ServiceName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["OptionTypeName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Grade"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["GradeExternal"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["BookingReference"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["RequestStatusID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["StartDate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["StartTime"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["SortDate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["CustomSort"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["DefaultEndDate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["BookingCurrency"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NetUnit"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["GrossUnit"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NumberOfDays"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Quantity"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["DiscountUnits"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NetTotal"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["GrossTotal"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["BaseCurrency"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["CurrencyRate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NetFinal"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["GrossFinal"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["IsInvoiced"].Header.VisiblePosition = index++;


            // Set defaults
            GridHelper.SetDefaultGridAppearance(e);
            GridHelper.SetDefaultGroupByAppearance(e);

            // Override defaults
            e.Layout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
            e.Layout.Override.SelectTypeRow = SelectType.SingleAutoDrag;
            e.Layout.GroupByBox.Hidden = false;
            e.Layout.AutoFitStyle = AutoFitStyle.None;
            e.Layout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;

            SetGridSummaries(e, CurrencyService.GetItineraryCurrencyCodeOrDefault(itinerarySet.Itinerary[0]));
        }

        private void grid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Band.Key != "PurchaseItem") return;

            try
            {
                // Set the purchaseline name.
                e.Row.Cells["PurchaseLineName"].Value = itinerarySet.PurchaseLine.FindByPurchaseLineID(
                    (int)e.Row.Cells["PurchaseLineID"].Value).PurchaseLineName;

                var itemId = (int)e.Row.Cells["PurchaseItemID"].Value;
                var item = itinerarySet.PurchaseItem.Where(i => i.RowState != DataRowState.Deleted && i.PurchaseItemID == itemId).FirstOrDefault();
                if (item == null) return;

                // Set the city name.
                int? cityId = itinerarySet.GetPurchaseItemCityId(itemId);
                if (cityId.HasValue)
                {
                    ToolSet.CityRow city = Cache.ToolSet.City.FindByCityID((int)cityId);
                    if (city != null && city.CityName != null)
                    {
                        e.Row.Cells["CityName"].Value = city.CityName;
                    }
                    else
                    {
                        e.Row.Cells["CityName"].Value = "<none>";
                    }
                }
                // Set the region name.
                int? regionId = itinerarySet.GetPurchaseItemRegionId(itemId);
                if (regionId.HasValue)
                {
                    ToolSet.RegionRow region = Cache.ToolSet.Region.FindByRegionID((int)regionId);
                    if (region != null && region.RegionName != null)
                        e.Row.Cells["RegionName"].Value = region.RegionName;
                    else
                        e.Row.Cells["RegionName"].Value = "<none>";
                }
                // Set the grades
                int? gradeId = itinerarySet.GetPurchaseItemGradeId(itemId);
                if (gradeId.HasValue)
                {
                    ToolSet.GradeRow grade = Cache.ToolSet.Grade.FindByGradeID((int)gradeId);
                    e.Row.Cells["Grade"].Value = (grade != null) ? grade.GradeName : null;
                }
                int? gradeExternalId = itinerarySet.GetPurchaseItemGradeExternalId(itemId);
                if (gradeExternalId.HasValue)
                {
                    ToolSet.GradeExternalRow gradeExternal
                        = Cache.ToolSet.GradeExternal.FindByGradeExternalID((int)gradeExternalId);
                    e.Row.Cells["GradeExternal"].Value = (gradeExternal != null) ? gradeExternal.GradeExternalName : null;
                }

                // set base prices
                var hasOverride = CurrencyService.GetPurchaseItemCurrencyCode(item) != null;
                var format = "{0:" + (hasOverride ? CurrencyService.GetCurrency(item.CurrencyCode).DisplayFormat : "c") + "}";
                if (e.Row.Band.Columns.Exists("NetUnit")) e.Row.Cells["NetUnit"].Value = string.Format(format, item.Net);
                if (e.Row.Band.Columns.Exists("GrossUnit")) e.Row.Cells["GrossUnit"].Value = string.Format(format, item.Gross);
                if (e.Row.Band.Columns.Exists("NetTotal")) e.Row.Cells["NetTotal"].Value = string.Format(format, item.NetTotal);
                if (e.Row.Band.Columns.Exists("GrossTotal")) e.Row.Cells["GrossTotal"].Value = string.Format(format, item.GrossTotal);

                // set final prices
                var itinerary = item.PurchaseLineRow.ItineraryRow;
                hasOverride = CurrencyService.GetItineraryCurrencyCode(itinerary) != null;
                format = "{0:" + (hasOverride ? CurrencyService.GetCurrency(itinerary.CurrencyCode).DisplayFormat : "c") + "}";
                if (e.Row.Band.Columns.Exists("NetFinal")) e.Row.Cells["NetFinal"].Value = string.Format(format, item.NetTotalConverted);
                if (e.Row.Band.Columns.Exists("GrossFinal")) e.Row.Cells["GrossFinal"].Value = string.Format(format, item.GrossTotalConverted);

                // Set default EndDate
                if (e.Row.Band.Columns.Exists("DefaultEndDate"))
                {
                    e.Row.Cells["DefaultEndDate"].Value = e.Row.Cells["EndDate"].Value;
                    if (!item.IsStartDateNull() && !item.IsNumberOfDaysNull())
                        e.Row.Cells["DefaultEndDate"].Value = item.StartDate.Date.AddDays(item.NumberOfDays).ToShortDateString();
                }

                // disable the row if it has been exported to accounting
                if (e.Row.Cells["IsLockedAccounting"].Value != DBNull.Value &&
                    (bool)e.Row.Cells["IsLockedAccounting"].Value)
                {
                    SetRowLocked(e.Row, true);
                    EnableDisableButtons(e.Row);
                }
                else
                {
                    SetRowLocked(e.Row, false);
                    EnableDisableButtons(e.Row);
                }
                e.Row.Cells["BaseCurrency"].Value = CurrencyService.GetItineraryCurrencyCodeOrDefault(itinerarySet.Itinerary[0]);
                e.Row.Cells["BookingCurrency"].Value = CurrencyService.GetPurchaseItemCurrencyCodeOrDefault(item);

                ValidatePurchaseItem(item);
                SetFlags(e.Row);

                e.Row.Cells["CustomSort"].Value = item.CustomSortDate;
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("Key not found"))
                {
                    ResetGridLayout();
                    ErrorHelper.SendEmail(ex, true);
                }
                else { throw; }
            }
        }

        private void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (!GridHelper.HandleInvalidGridEdits(grid, false))
                return;

            if (e.Row.GetType() == typeof(UltraGridEmptyRow))
                return;

            if (e.Row.IsGroupByRow)
            {
                if (!e.Row.Expanded)
                    e.Row.CollapseAll();
                else
                    e.Row.ExpandAll();
            }
            else if (e.Row.Cells.Count > 0 &&
                e.Row.Cells.Exists("PurchaseLineID") && e.Row.Cells["PurchaseLineID"].Value != DBNull.Value &&
                e.Row.Cells.Exists("PurchaseItemID") && e.Row.Cells["PurchaseItemID"].Value != DBNull.Value)
            {
                OpenBookingEditor(
                    (int)e.Row.Cells["PurchaseLineID"].Value,
                    (int)e.Row.Cells["PurchaseItemID"].Value);
            }
        }


        private void grid_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (ModifierKeys == 0 && e.KeyChar == (char)13) // 'Enter' key
            {
                if (!GridHelper.HandleInvalidGridEdits(grid, false))
                    return;

                grid.UpdateData();
            }
            else if (grid.ActiveCell != null && grid.ActiveCell.IsInEditMode)
            {
                // Mask input for these columns.
                if (grid.ActiveCell.Column.Key == "NumberOfDays" ||
                    grid.ActiveCell.Column.Key == "Quantity")
                {
                    // We want to show integer (2 nights) when possible, double ('2.5') if we have to.
                    // Using this as mask as with grids MaskInput ('{double:i.f:c}') it always shows 
                    // as type double (eg '1.0' nights).
                    e.Handled = !App.IsKeyPressAllowedForNumber(e);
                }
            }
        }

        private void grid_BeforeSelectChange(object sender, BeforeSelectChangeEventArgs e)
        {
            // Don't allow group-by rows to be selected.
            if (e.NewSelections.Rows.Count > 0 && e.NewSelections.Rows[0].IsGroupByRow)
                e.Cancel = true;
        }

        private void grid_BeforeRowActivate(object sender, RowEventArgs e)
        {
            EnableDisableButtons(e.Row);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null || grid.ActiveRow.IsGroupByRow)
            {
                SearchBookings();
            }
            else
            {
                if (grid.ActiveRow.ParentRow != null)
                    grid.ActiveRow.ParentRow.Expanded = true;

                int supplierId = itinerarySet.PurchaseLine.FindByPurchaseLineID(
                    (int)grid.ActiveRow.Cells["PurchaseLineID"].Value).SupplierID;
                itineraryMain = ParentForm as ItineraryMain;
                AddNewBooking(supplierId, this.itineraryMain);
            }
        }

        private void grid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
            if (!_isDeleteButtonPress)
            {
                // override grids default DEL key behaviour
                e.Cancel = true;
                btnDelete.PerformClick();
            }
        }

        private bool _isDeleteButtonPress;
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow != null && grid.ActiveRow.IsGroupByRow) return;

            if (grid.ActiveRow != null && grid.ActiveRow.ParentRow != null)
                grid.ActiveRow.ParentRow.Expanded = true;

            _isDeleteButtonPress = true;
            DeleteSelectedBookingRows();
            _isDeleteButtonPress = false;
        }

        private void DeleteSelectedBookingRows()
        {
            if (grid.Selected.Rows.Count == 0 && grid.ActiveRow != null)
                grid.ActiveRow.Selected = true;
            var rows = grid.Selected.Rows;

            var doDelete = rows.Count == 1 ? App.AskDeleteRow() : rows.Count > 1 ? App.AskDeleteRows(rows.Count) : false;
            if (!doDelete) return;

            for (var i = rows.Count - 1; i >= 0; i--)
            {
                var id = (int)rows[i].Cells["PurchaseLineID"].Value;
                var line = itinerarySet.PurchaseLine.FindByPurchaseLineID(id);

                rows[i].Delete(false);
                if (line.GetPurchaseItemRows().Length == 0)
                    line.Delete();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null || grid.ActiveRow.IsGroupByRow)
                return;

            if (grid.ActiveRow.ParentRow != null)
                grid.ActiveRow.ParentRow.Expanded = true;
            Application.DoEvents();

            ItinerarySet.PurchaseItemRow copyRow = itinerarySet.PurchaseItem.FindByPurchaseItemID(
                (int)grid.ActiveRow.Cells["PurchaseItemID"].Value);

            if (App.AskYesNo("Copy booking item: " + copyRow.PurchaseItemName + "?"))
            {
                ItinerarySet.PurchaseItemRow newRow =
                    itinerarySet.CopyPurchaseItem(
                        copyRow, copyRow.PurchaseLineRow.PurchaseLineID, "Copy of", Cache.User.UserID);

                SetActiveRow(grid.Rows, newRow.PurchaseItemID);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null)
                return;

            if (!GridHelper.HandleInvalidGridEdits(grid, false))
                return;

            if (grid.ActiveRow.IsGroupByRow)
                OpenBookingEditor(null, null);
            else
                OpenBookingEditor(
                    (int)grid.ActiveRow.Cells["PurchaseLineID"].Value,
                    (int)grid.ActiveRow.Cells["PurchaseItemID"].Value);
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (btnExpand.Text == "Expand")
                SetGridExpanded();
            else
                SetGridCollapsed();
        }

        private void btnBookings_Click(object sender, EventArgs e)
        {
            btnBookings.ShowDropDown();
        }

        private void btnBookAll_Click(object sender, EventArgs e)
        {
            var idList = new List<int>();
            foreach (var row in itinerarySet.PurchaseLine)
            {
                if (row.RowState != DataRowState.Deleted &&
                    row.GetPurchaseItemRows().Length > 0)
                {
                    idList.Add(row.PurchaseLineID); // don't add empty bookings
                }
            }
            var item = (ToolStripItem)sender;
            var templateFilePath = (string)item.Tag;
            if (idList.Count > 0) SendBookingRequest(idList, templateFilePath);
        }

        private void btnBookSelected_Click(object sender, EventArgs e)
        {
            if (grid.Selected.Rows.Count == 0 && grid.ActiveRow != null)
                grid.ActiveRow.Selected = true; // Sometimes a row is active but not selected

            var idList = new List<int>();

            foreach (var row in grid.Selected.Rows)
            {
                if (row.IsGroupByRow) continue;

                var id = (int)row.Cells["PurchaseLineID"].Value;
                if (!idList.Contains(id)) idList.Add(id);
            }
            var item = (ToolStripItem)sender;
            var templateFilePath = (string)item.Tag;
            if (idList.Count > 0) SendBookingRequest(idList, templateFilePath);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            UltraGridPrintDocument doc = new UltraGridPrintDocument();
            doc.RowProperties = RowPropertyCategories.All;
            doc.DefaultPageSettings.Landscape = true;
            doc.Grid = grid;

            /********************************************************** 
             * UltraGridPrintDocument issue is causing grid data to not
             * print properly if FitWidthToPages = 1 and either header 
             * or footer text is added.
             **********************************************************/
            doc.FitWidthToPages = 1;
            /*
            doc.DocumentName = "BookingList_" + itinerarySet.Itinerary[0].ItineraryName.Substring(0, 3);
            doc.Header.TextLeft =
                String.Format("Bookings for: {0}\r\n{1} - {2}",
                              itinerarySet.Itinerary[0].ItineraryName,
                              itinerarySet.Itinerary[0].ArriveDate.ToShortDateString(),
                              itinerarySet.Itinerary[0].DepartDate.ToShortDateString());
            doc.Header.TextRight = DateTime.Now.ToShortDateString();
            doc.Footer.TextCenter = "[Page #]";               
             */

            UltraPrintPreviewDialog preview;
            preview = new UltraPrintPreviewDialog();
            preview.Document = doc;
            preview.ShowDialog();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            string filename = App.PromptSaveFile("", "", ".xls");
            if (filename.Length > 0)
            {
                Cursor = Cursors.WaitCursor;

                new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter().
                    Export(grid, filename);
                Process.Start(filename);

                Cursor = Cursors.Default;
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            grid.DisplayLayout.Override.AllowRowFiltering = DefaultableBoolean.True;
        }

        private void btnMoveDates_Click(object sender, EventArgs e)
        {
            DateKickerForm dateKicker = new DateKickerForm(itinerarySet, 0, false);
            if (dateKicker.ShowDialog() == DialogResult.OK)
            {
                RefreshGrid();
                RecalculateFinalPricing();
            }
        }

        private void btnUpdateQuantities_Click(object sender, EventArgs e)
        {
            UpdateQuantitiesForm frm = new UpdateQuantitiesForm(itinerarySet, false);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                RefreshGrid();
            }
        }

        private void btnResetGrid_Click(object sender, EventArgs e)
        {
            ResetGridLayout();
        }

        private void btnEditFlags_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null)
                return;

            var flagEditor = new BookingFlagEditor(itinerarySet, (int)GetSelectedPurchaseItemId());
            if (flagEditor.ShowDialog() == DialogResult.OK)
            {
                grid.Rows.Refresh(RefreshRow.FireInitializeRow);
            }
        }

        private void Itinerary_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            SetPriceOverrideWarning();
        }

        private void PurchaseLine_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // When purchase line name is changed (booking editor etc)
            // ensure grid is updated to show the change.
            if (e.Column.ColumnName == "PurchaseLineName")
                grid.Rows.Refresh(RefreshRow.FireInitializeRow, true);
        }

        private void PurchaseItem_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Row.RowState == DataRowState.Added)
            {
                // highlight new row and resort rows
                ItinerarySet.PurchaseItemRow item = e.Row as ItinerarySet.PurchaseItemRow;
                if (item != null)
                {
                    SetActiveRow(grid.Rows, item.PurchaseItemID);
                    if (grid.ActiveRow != null)
                    {
                        grid.ActiveRow.RefreshSortPosition();
                        ExpandRowGroup(grid.ActiveRow);
                    }
                }
            }
        }

        private void PurchaseItem_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            WarnIfPriceOverrideExists();
            RecalculateFinalPricing();
        }

        private void PurchaseItem_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column == ItinerarySet.PurchaseItem.GrossTotalConvertedColumn)
            {
                WarnIfPriceOverrideExists();
                RecalculateFinalPricing();
            }
        }

        private void grid_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "PurchaseLineName")
            {
                // Purchase line name is an unbound column, so need to catch edits
                // and manually update its underlying datasource.
                int lineId = (int)e.Cell.Row.Cells["PurchaseLineID"].Value;
                ItinerarySet.PurchaseLineRow row = itinerarySet.PurchaseLine.FindByPurchaseLineID(lineId);
                if (row.PurchaseLineName != e.Cell.Text)
                    row.PurchaseLineName = e.Cell.Text;
            }
            else if (e.Cell.Column.Key == "IsLockedAccounting")
            {
                // When the "Locked" checkbox is clicked, it needs to be forced out of edit mode
                // so that the row will immediately reflect the change (become enabled/disabled)
                e.Cell.EditorResolved.ExitEditMode(true, true);
            }

            else if (e.Cell.Column.Key == "IsInvoiced")
            {
                // if this PurchaseItem is one of multiple in this booking (PurchaseLine) then open editor
                // so that user can see and handle the other related items.
                var row = e.Cell.Row;
                var hasSiblings = itinerarySet.PurchaseItem.Count(x => 
                    x.RowState != DataRowState.Deleted && x.PurchaseLineID == (int)row.Cells["PurchaseLineID"].Value) > 1;

                if (hasSiblings)
                    OpenBookingEditor((int)row.Cells["PurchaseLineID"].Value, (int)row.Cells["PurchaseItemID"].Value);
            }
        }

        private void grid_BeforeExitEditMode(object sender, Infragistics.Win.UltraWinGrid.BeforeExitEditModeEventArgs e)
        {
            var activeCell = grid.ActiveCell;
            if (activeCell != null && activeCell.Column.Key == "StartDate")
            {
                DateTime newDate, origDate;
                if (activeCell.Value != null && !(activeCell.Value is DBNull) &&
                    activeCell.EditorResolved != null && !(activeCell.EditorResolved.Value is DBNull) &&
                    DateTime.TryParse(activeCell.EditorResolved.Value.ToString(), out newDate) &&
                    DateTime.TryParse(activeCell.Value.ToString(), out origDate) &&
                    newDate != origDate &&
                    App.AskYesNo("Auto-update rate also?"))
                {
                    var delta = (newDate.Date - origDate.Date).Days;
                    var dateKicker = new DateKickerForm(itinerarySet, 0, true);
                    dateKicker.SetSelectedRow((int)activeCell.Row.Cells["PurchaseItemID"].Value, delta);
                    if (dateKicker.ShowDialog() == DialogResult.OK)
                    {
                        RecalculateFinalPricing();
                        RefreshGrid();
                    }
                    else // revert 
                        activeCell.EditorResolved.Value = origDate;
                }
            }
        }

        private void grid_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "StartDate" && e.Cell.EditorResolved != null)
                e.Cell.EditorResolved.ExitEditMode(true, true);
        }

        private void txtMarkupOverride_Validated(object sender, EventArgs e)
        {
            RecalculateFinalPricing();
        }

        private void txtPriceAdjustment_Validated(object sender, EventArgs e)
        {
            RecalculateFinalPricing();
        }

        private void txtOverride_Validated(object sender, EventArgs e)
        {
            RecalculateFinalPricing();
        }

        private void btnLockEdit_Click(object sender, EventArgs e)
        {
            grid.DisplayLayout.Bands[0].Columns["IsLockedAccounting"].Hidden =
                !grid.DisplayLayout.Bands[0].Columns["IsLockedAccounting"].Hidden;
        }

        private void btnNetMarkup_Click(object sender, EventArgs e)
        {
            EditNetOverride();
        }

        private void txtNetOverride_Click(object sender, EventArgs e)
        {
            EditNetOverride(); // has service type overrides so open editor
        }

        private void txtNetOverride_BeforeEnterEditMode(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // make it appear like disabled
        }

        private void btnChangeOption_Click(object sender, EventArgs e)
        {
            ChangePurchaseItemOption();
        }

        private void btnUpdateCurrency_Click(object sender, EventArgs e)
        {
            RunCurrencyUpdater();
        }

        private void chkLockGrossOverride_CheckedChanged(object sender, EventArgs e)
        {

            if (chkLockGrossOverride.Checked)
            {
                // lock it
                txtPriceOverride.ReadOnly = true;
                itinerarySet.Itinerary[0].IsLockedGrossOverride = true;

                // and set override (if to set already) to current gross price
                if (itinerarySet.Itinerary[0].IsGrossOverrideNull())
                {
                    var price = itinerarySet.GetGrossFinalPrice();
                    txtPriceOverride.Value = price;
                    itinerarySet.Itinerary[0].GrossOverride = price;
                }
            }
            else
            {
                // unlock it but leave override price for user to remove
                txtPriceOverride.ReadOnly = false;
                itinerarySet.Itinerary[0].IsLockedGrossOverride = false;
            }
        }

        private void grid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var row = GridHelper.GetValidClickRow(grid);
                if (row != null)
                {
                    SetActiveRow(grid.Rows, (int)row.Cells["PurchaseItemID"].Value);
                    grid.ContextMenuStrip = bookingGridMenu;
                }
            }
        }

        private void grid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var row = GridHelper.GetValidClickRow(grid);
                if (row != null)
                {
                    grid.ContextMenuStrip = null;
                }
            }
        }

        private void btnAddNewTemplate_Click(object sender, EventArgs e)
        {
            var addTemplateForm = new AddBookingTemplateForm();
            if (addTemplateForm.ShowDialog() == DialogResult.OK)
            {
                AddTemplate(addTemplateForm.TemplateName, addTemplateForm.TemplateFile);
                RefreshEmailTemplateMenu();
            }
        }

        private void btnRemoveTemplate_Click(object sender, EventArgs e)
        {
            int templateId;
            if (int.TryParse(((ToolStripItem)sender).Tag.ToString(), out templateId))
            {
                var row = Cache.ToolSet.Template.FindByTemplateID(templateId);
                if (row != null && App.AskDeleteRow())
                {
                    row.Delete();
                    RefreshEmailTemplateMenu();
                }
            }
        }

        private void paxOverrideMenuItem_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null) return;

            var id = Convert.ToInt32(grid.ActiveRow.Cells["PurchaseItemID"].Value);
            var item = itinerarySet.PurchaseItem.Where(i => i.RowState != DataRowState.Deleted && i.PurchaseItemID == id).FirstOrDefault();
            new ItineraryPaxOverride(item).ShowDialog();
        }

        private void replaceSupplierMenuItem_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null) return;

            DateTime startDate;
            bool hasStartDate = DateTime.TryParse(grid.ActiveRow.Cells["StartDate"].Value.ToString(), out startDate);
            var origItemId = (int)grid.ActiveRow.Cells["PurchaseItemID"].Value;

            var selector = new BookingSelectorForm(itinerarySet, itineraryMain, hasStartDate ? startDate : (DateTime?)null);
            if (selector.ShowDialog() == DialogResult.OK)
            {
                var origItem = itinerarySet.PurchaseItem.FirstOrDefault(x => x.PurchaseItemID == origItemId);
                if (origItem != null) origItem.Delete();
            }
        }

        private void moveToNewBookingMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (cmbBookings.ComboBox == null) return;
            cmbBookings.SelectedIndexChanged -= cmbBookings_SelectedIndexChanged;
            cmbBookings.Items.Clear();
            cmbBookings.ComboBox.Width = 200;
            cmbBookings.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBookings.ComboBox.ValueMember = "Key";
            cmbBookings.ComboBox.DisplayMember = "Value";

            var currentLineId = (int)grid.ActiveRow.Cells["PurchaseLineID"].Value;
            var supplierId = itinerarySet.PurchaseLine.FindByPurchaseLineID(currentLineId).SupplierID;
            cmbBookings.Items.Add(new KeyValuePair<int?, string>(null, "Create new..."));
            foreach (var line in itinerarySet.GetSupplierBookings(supplierId))
            {
                var item = new KeyValuePair<int?, string>(
                    line.PurchaseLineID, string.Format("({0}) {1}", line.PurchaseLineID, line.PurchaseLineName));
                cmbBookings.Items.Add(item);
                if (line.PurchaseLineID == currentLineId) cmbBookings.SelectedIndex = cmbBookings.Items.Count - 1;
            }
            cmbBookings.SelectedIndexChanged += cmbBookings_SelectedIndexChanged;
        }

        private void cmbBookings_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newLineId = ((KeyValuePair<int?, string>)cmbBookings.SelectedItem).Key;
            var oldLineId = (int)grid.ActiveRow.Cells["PurchaseLineID"].Value;

            if (newLineId != oldLineId)
            {
                grid.SuspendLayout();

                var oldItem = itinerarySet.PurchaseItem.First(x => x.RowState != DataRowState.Deleted &&
                    x.PurchaseItemID == (int)grid.ActiveRow.Cells["PurchaseItemID"].Value);
                var oldLine = itinerarySet.PurchaseLine.First(x => x.RowState != DataRowState.Deleted &&
                    x.PurchaseLineID == (int)grid.ActiveRow.Cells["PurchaseLineID"].Value);

                if (!newLineId.HasValue) 
                    newLineId = itinerarySet.CopyPurchaseLine(oldLine, null, Cache.User.UserID, false).PurchaseLineID;
                // have to always create a 'new' item row (not just update line Id) or dataset will barf a constraint error
                var newItem = itinerarySet.CopyPurchaseItem(oldItem, (int)newLineId, "", Cache.User.UserID);
               
                // clean up
                oldItem.Delete(); // delete old item
                if (oldLine.GetPurchaseItemRows().Count(x => x.RowState != DataRowState.Deleted) == 0)
                    oldLine.Delete(); // delete old line if empty

                grid.ResumeLayout();
            }
            bookingGridMenu.Close();
        }

        private void btnCalendar_Click(object sender, EventArgs e)
        {
            var idList = new List<int>();
            foreach (var row in itinerarySet.PurchaseLine)
            {
                if (row.RowState != DataRowState.Deleted &&
                    row.GetPurchaseItemRows().Length > 0)
                {
                    idList.Add(row.PurchaseLineID); // don't add empty bookings
                }
            }

            string strEventName;
            string strDescription;
            ArrayList rowList;
            var templateForm = new TemplateForm(itinerarySet.Itinerary[0].ItineraryName, Environment.CurrentDirectory + @"\BookingRequest-Plain.html");
            var templateSettings = templateForm.GetTemplateSettings();

            ToolSet.AppSettingsRow settings = Global.Cache.ToolSet.AppSettings[0];
            CalendarService cs;
            if (settings.GCalUser.Length <= 0 || settings.GCalPass.Length <= 0)
                App.ShowInfo("Please set your Google login information first in Tools > Setup.");
            else
            {
                try
                {
                    cs = new CalendarService(settings.GCalUser, settings.GCalPass);

                    foreach (var purchaseLine in idList.Select(id => itinerarySet.PurchaseLine.FindByPurchaseLineID(id)))
                    {
                        rowList = new ArrayList();
                        rowList.AddRange(itinerarySet.PurchaseItem.Select("PurchaseLineID = " + purchaseLine.PurchaseLineID));
                        rowList.Sort(new DateTimeSortComparer());
                        var item = (ItinerarySet.PurchaseItemRow)rowList[0];
                        strEventName = purchaseLine.PurchaseLineID.ToString() + " " + (!purchaseLine.ItineraryRow.IsAgentIDNull() ? Cache.ToolSet.Agent.FindByAgentID(purchaseLine.ItineraryRow.AgentID).AgentName : "");
                        strDescription = new BookingEmailInfo(new List<ItinerarySet.PurchaseLineRow> { purchaseLine }).BuildEmailText(templateSettings.Body, purchaseLine);
                        strDescription = strDescription.Replace("<BODY contentEditable=true>", "").Replace("</BODY>", "").Replace("<BR>", "\r\n");
                        if (!cs.IsExisting(purchaseLine.PurchaseLineID.ToString()))
                        {
                            cs.CreateEvent(strEventName, strDescription, "", item.StartDate, item.StartDate.AddHours(23).AddMinutes(59));
                            App.ShowInfo("Successfully added " + strEventName + ".");
                        }
                        else
                        {
                            cs.UpdateEvent(purchaseLine.PurchaseLineID.ToString(), strDescription, "", item.StartDate, item.StartDate.AddHours(23).AddMinutes(59));
                            App.ShowInfo("Successfully updated " + strEventName + ".");
                        }
                    }
                    App.ShowInfo("Done!");
                }
                catch (Exception ex)
                {
                    cs = null;
                    App.ShowError(ex.Message.ToString());
                }
            }
        }

        #endregion
        
        #region CustomSort

        private void Initialize_DragDrop()
        {
            var menuItem = new ToolStripMenuItem("Remove Custom Sort");
            menuItem.Click += tsb_Click;
            bookingGridMenu.Items.Add(menuItem);

            grid.AllowDrop = true;
            grid.SelectionDrag += grid_SelectionDrag;
            grid.DragOver += grid_DragOver;
            grid.DragDrop += grid_DragDrop;
        }

        private void tsb_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null)
                return;

            if (!GridHelper.HandleInvalidGridEdits(grid, false))
                return;

            var row = itinerarySet.PurchaseItem.FindByPurchaseItemID((int)grid.ActiveRow.Cells["PurchaseItemID"].Value);
            grid.ActiveRow.Cells["SortDate"].Value = DBNull.Value;
            grid.ActiveRow.Cells["CustomSort"].Value = row.CustomSortDate;
        }

        private void grid_SelectionDrag(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (grid.DisplayLayout.Bands[0].Columns["CustomSort"].SortIndicator != SortIndicator.Ascending && grid.DisplayLayout.Bands[0].Columns["CustomSort"].SortIndicator != SortIndicator.Descending)
            {
                grid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortSingle;
                grid.DisplayLayout.Bands[0].Columns["CustomSort"].SortIndicator = SortIndicator.Ascending;
                grid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.Default;
            }
            grid.DoDragDrop(grid.Selected.Rows, DragDropEffects.Move);
        }

        private void grid_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            UltraGrid gridobj = sender as UltraGrid;
            Point pointInGridCoords = gridobj.PointToClient(new Point(e.X, e.Y));
            if (pointInGridCoords.Y < 20)
                // Scroll up
                grid.ActiveRowScrollRegion.Scroll(RowScrollAction.LineUp);
            else if (pointInGridCoords.Y > gridobj.Height - 20)
                // Scroll down
                grid.ActiveRowScrollRegion.Scroll(RowScrollAction.LineDown);
        }

        private void grid_DragDrop(object sender, DragEventArgs e)
        {
            int dropIndex;

            // Get the position on the grid where the dragged row(s) are to be dropped.
            //get the grid coordinates of the row (the drop zone)
            UIElement uieOver = grid.DisplayLayout.UIElement.ElementFromPoint(grid.PointToClient(new Point(e.X, e.Y)));

            //get the row that is the drop zone/or where the dragged row is to be dropped
            UltraGridRow ugrOver = uieOver.GetContext(typeof(UltraGridRow), true) as UltraGridRow;
            if (ugrOver != null)
            {
                dropIndex = ugrOver.Index;    //index/position of drop zone in grid

                //get the dragged row(s)which are to be dragged to another position in the grid
                SelectedRowsCollection SelRows = (SelectedRowsCollection)e.Data.GetData(typeof(SelectedRowsCollection)) as SelectedRowsCollection;
                //get the count of selected rows and drop each starting at the dropIndex
                foreach (UltraGridRow aRow in SelRows)
                {
                    if (dropIndex == aRow.Index) break;
                    const int factor = -1;
                    if (grid.Rows[dropIndex].Cells["SortDate"].Value == DBNull.Value)
                    {
                        if (grid.Rows[dropIndex].Cells["StartTime"].Value != DBNull.Value)
                        {
                            if (dropIndex == 0)
                                aRow.Cells["SortDate"].Value = DateTime.Parse(grid.Rows[dropIndex].Cells["CustomSort"].Value.ToString()).AddHours(factor);
                            else
                                aRow.Cells["SortDate"].Value = this.ComputeMiddleDate("CustomSort", dropIndex);
                        }
                        else
                        {
                            var row = itinerarySet.PurchaseItem.FindByPurchaseItemID((int)aRow.Cells["PurchaseItemID"].Value);
                            string dropZoneSortDate = DateTime.Parse(grid.Rows[dropIndex].Cells["StartDate"].Value.ToString()).ToShortDateString();
                            aRow.Cells["SortDate"].Value = DateTime.Parse(dropZoneSortDate + " 1:00 AM");
                            aRow.Cells["CustomSort"].Value = row.CustomSortDate;
                            if (grid.Rows[dropIndex].Cells["SortDate"].Value == DBNull.Value && DateTime.Parse(aRow.Cells["SortDate"].Value.ToString()).ToShortDateString() == dropZoneSortDate)
                            {
                                var row2 = itinerarySet.PurchaseItem.FindByPurchaseItemID((int)grid.Rows[dropIndex].Cells["PurchaseItemID"].Value);
                                grid.Rows[dropIndex].Cells["SortDate"].Value = DateTime.Parse(DateTime.Parse(aRow.Cells["SortDate"].Value.ToString()).ToShortDateString() + " 2:00 AM");
                                grid.Rows[dropIndex].Cells["CustomSort"].Value = row2.CustomSortDate;
                            }
                        }
                    }
                    else
                    {
                        if (dropIndex == 0)
                        {
                            aRow.Cells["SortDate"].Value = DateTime.Parse(grid.Rows[dropIndex].Cells["SortDate"].Value.ToString()).AddHours(factor);
                        }
                        else
                        {
                            if (grid.Rows[dropIndex - 1].Cells["SortDate"].Value.ToString() != grid.Rows[dropIndex].Cells["SortDate"].Value.ToString())
                            {
                                aRow.Cells["SortDate"].Value = this.ComputeMiddleDate("SortDate", dropIndex);
                            }
                            else
                            {
                                string dropZoneSortDate = DateTime.Parse(grid.Rows[dropIndex].Cells["StartDate"].Value.ToString()).ToShortDateString();
                                if (DateTime.Parse(grid.Rows[dropIndex - 1].Cells["SortDate"].Value.ToString()).ToShortTimeString() == "12:00 AM")
                                {
                                    aRow.Cells["SortDate"].Value = DateTime.Parse(dropZoneSortDate + " 1:00 AM");
                                    grid.Rows[dropIndex].Cells["SortDate"].Value = DateTime.Parse(dropZoneSortDate + " 2:00 AM");
                                }
                                else if (DateTime.Parse(grid.Rows[dropIndex - 1].Cells["SortDate"].Value.ToString()).ToShortTimeString() == "11:59 PM")
                                {
                                    aRow.Cells["SortDate"].Value = DateTime.Parse(dropZoneSortDate + " 11:00 PM");
                                    grid.Rows[dropIndex - 1].Cells["SortDate"].Value = DateTime.Parse(dropZoneSortDate + " 10:00 PM");
                                }
                                else
                                {
                                    aRow.Cells["SortDate"].Value = DateTime.Parse(dropZoneSortDate + " 2:00 AM");
                                    grid.Rows[dropIndex].Cells["SortDate"].Value = DateTime.Parse(dropZoneSortDate + " 3:00 AM");
                                    grid.Rows[dropIndex - 1].Cells["SortDate"].Value = DateTime.Parse(dropZoneSortDate + " 1:00 AM");
                                }
                            }
                        }
                        var row = itinerarySet.PurchaseItem.FindByPurchaseItemID((int)aRow.Cells["PurchaseItemID"].Value);
                        aRow.Cells["CustomSort"].Value = row.CustomSortDate;
                    }
                    //move the selected row(s) to the drop zone
                    if (aRow.Index < dropIndex)
                        dropIndex--;
                    grid.Rows.Move(aRow, dropIndex);
                }
            }
        }

        private DateTime ComputeMiddleDate(string column, int dropIndex)
        {
            TimeSpan ts = DateTime.Parse(grid.Rows[dropIndex - 1].Cells[column].Value.ToString()).Subtract(DateTime.Parse(grid.Rows[dropIndex].Cells[column].Value.ToString()));
            double milli = ts.TotalMilliseconds;
            DateTime middleDate = DateTime.Parse(grid.Rows[dropIndex - 1].Cells[column].Value.ToString()).AddMilliseconds(-(milli / 2));
            return middleDate;
        }

        #endregion
    }

    public delegate void OnBookingsViewerEditBookingHandler(BookingsViewerEditBookingEventArgs e);

    public class BookingsViewerEditBookingEventArgs
    {
        public int? purchaseLineId;
        public int? purchaseItemId;

        public BookingsViewerEditBookingEventArgs(int? purchaseLineId, int? purchaseItemId)
        {
            this.purchaseLineId = purchaseLineId;
            this.purchaseItemId = purchaseItemId;
        }
    }

    class TimeSortComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            object oTime1 = ((UltraGridCell)x).Value;
            object oTime2 = ((UltraGridCell)y).Value;

            // handle nulls
            if (oTime1 == DBNull.Value && oTime2 == DBNull.Value)
                return 0;
            if (oTime1 != DBNull.Value && oTime2 == DBNull.Value)
                return 1;
            if (oTime1 == DBNull.Value && oTime2 != DBNull.Value)
                return -1;

            DateTime d1 = (DateTime)oTime1;
            DateTime d2 = (DateTime)oTime2;

            // compare times
            if (d1.TimeOfDay > d2.TimeOfDay)
                return 1;
            if (d1.TimeOfDay < d2.TimeOfDay)
                return -1;
            return 0;
        }
    }

    class DateSortComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            object oDate1 = ((UltraGridCell)x).Value;
            object oDate2 = ((UltraGridCell)y).Value;

            // handle nulls
            if (oDate1 == DBNull.Value && oDate2 == DBNull.Value)
                return 0;
            if (oDate1 != DBNull.Value && oDate2 == DBNull.Value)
                return 1;
            if (oDate1 == DBNull.Value && oDate2 != DBNull.Value)
                return -1;

            DateTime d1 = (DateTime)oDate1;
            DateTime d2 = (DateTime)oDate2;

            // compare times
            if (d1.Date > d2.Date)
                return 1;
            if (d1.Date < d2.Date)
                return -1;
            return 0;
        }
    }
}