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

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class BookingsViewer : UserControl
    {
        public event OnBookingsViewerEditBookingHandler OnOpenBooking;

        private readonly string GridLayoutFileName;

        private ItinerarySet itinerarySet;

        internal ItinerarySet ItinerarySet  
        {
            get { return itinerarySet; }
            set
            {
                itinerarySet = value;
                SetDataBindings();
                LoadGridLayout();

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

            // TODO: flags hidden for now
            btnEditFlags.Visible = editFlagsToolStripMenuItem.Visible = false;
        }

        private void BookingsViewer_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                btnLockEdit.Enabled = AppPermissions.UserHasPermission(
                    AppPermissions.Permissions.AccountingEdit);
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
                    grid.DisplayLayout.ValueLists["StatusList"].ValueListItems.Add(r.RequestStatusID,
                                                                                   r.RequestStatusName);
            }
            grid.DataSource = itinerarySet.PurchaseItem;
            itineraryBindingSource.DataSource = itinerarySet.Itinerary;
            txtPriceOverride.ReadOnly = chkLockGrossOverride.Checked;
            SetNetOverrideText();
            SetPriceOverrideWarning();
        }

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

            // always hide by default
            grid.DisplayLayout.Bands[0].Columns["IsLockedAccounting"].Hidden = true;

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

        private void SetRowLocked(UltraGridRow row, bool locked)
        {
            foreach (UltraGridColumn column in grid.DisplayLayout.Bands[0].Columns)
            {
                if (column.Key != "IsLockedAccounting"
                    && column.Key != "PurchaseItemName"
                    && column.Key != "BookingReference"
                    && column.Key != "StartTime"
                    && column.Key != "EndTime")
                {
                    row.Cells[column].Activation = (locked) ? Activation.Disabled : Activation.AllowEdit;
                }
            }
        }

        internal static void SetGridSummaries(InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            band.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            SummarySettings summary;

            if (!band.Summaries.Exists("GroupGross"))
            {
                // Group gross total. 
                summary = band.Summaries.Add(SummaryType.Sum, band.Columns["GrossTotalConverted"]);
                summary.Key = "GroupGross";
                summary.DisplayFormat = "{0:c}";
                summary.SummaryDisplayArea = SummaryDisplayAreas.InGroupByRows; // group rows only
                summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
            }

            if (!band.Summaries.Exists("NetTotalConverted"))
            {
                // Net total.
                summary = band.Summaries.Add(SummaryType.Sum, band.Columns["NetTotalConverted"]);
                summary.Key = "NetTotalConverted";
                summary.DisplayFormat = "{0:c}";
                summary.ToolTipText = "Booking total net";
                summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                summary.SummaryDisplayArea = // fixed at bottom of grid
                    SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly;
            }

            if (!band.Summaries.Exists("GrossTotalConverted"))
            {
                // Gross total.
                summary = band.Summaries.Add(SummaryType.Sum, band.Columns["GrossTotalConverted"]);
                summary.Key = "GrossTotalConverted";
                summary.DisplayFormat = "{0:c}";
                summary.ToolTipText = "Bookings total gross";
                summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
                summary.SummaryDisplayArea = // fixed at bottom of grid
                    SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.RootRowsFootersOnly;
            }
        }

        public void SearchBookings()
        {
            BookingSelectorForm bookingSelector = new BookingSelectorForm(itinerarySet);
            bookingSelector.SetPage(BookingSelectorForm.PageType.Search);

            bookingSelector.ShowDialog();
            bookingSelector.Dispose();
        }

        private bool isLoadingSupplier;
        public void AddNewBooking(int supplierId)
        {
            // added to disable multiple drag-drop events queuing up
            if (isLoadingSupplier)
                return;

            isLoadingSupplier = true;
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            BookingSelectorForm bookingSelector = new BookingSelectorForm(itinerarySet);
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

        private void SendBookingRequest(IEnumerable<int> idList)
        {
            BookingEmailForm bookingEmailer = new BookingEmailForm(itinerarySet, idList);
            bookingEmailer.ShowDialog();
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
                    itinerarySet.Itinerary[0].SetGrossOverrideNull();
                    txtPriceOverride.Value = null;
                }
            }
        }

        private void SetPriceOverrideWarning()
        {
            if (itinerarySet.Itinerary[0].HasPriceOverrides)
            {
                overrideWarning.SetError(txtSell, "Adjusted sell price, base price has overrides.");
                overrideWarning.SetIconAlignment(txtSell, ErrorIconAlignment.MiddleRight);
                overrideWarning.SetIconPadding(txtSell, 25);
            }
            else
                overrideWarning.SetError(txtSell, ""); // reset
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

        public void RecalculateFinalPricing()
        {
            // net/gross adjustments
            if (!itinerarySet.Itinerary[0].IsNetMarginNull()
                || itinerarySet.ItineraryMarginOverride.Rows.Count > 0)
                txtGross1.Value = itinerarySet.GetNetBasePrice() * (1 + itinerarySet.GetNetMarkup() / 100);
            else
                txtGross1.Value = itinerarySet.GetGrossBasePrice();

            if (!itinerarySet.Itinerary[0].IsGrossMarkupNull())
                txtGross2.Value = (decimal)txtGross1.Value * (1 + itinerarySet.Itinerary[0].GrossMarkup / 100);
            else
                txtGross2.Value = txtGross1.Value;

            // final sell
            decimal total = itinerarySet.GetGrossFinalPrice();
            decimal net = itinerarySet.GetNetBasePrice();
            decimal yield = total - net;
            decimal yieldp = Common.CalcCommissionByNetGross(net, total);
            txtCommission.Text = String.Format("{0} ({1})", yield.ToString("c"), (yieldp / 100).ToString("p"));
            txtSell.Value = total;
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
            SetDataBindings();
        }

        private void SetFlags(UltraGridRow row)
        {
            int purchaseItemId = (int)row.Cells["PurchaseItemID"].Value;
            var notes = itinerarySet.PurchaseItemNote.Where(note => note.RowState != DataRowState.Deleted &&
                                                                    note.PurchaseItemID == purchaseItemId);

            var flagImageList = new List<Bitmap>();
            string message = String.Empty;
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

        #region Events

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // Hide all but the PurchaseItem table
            foreach (UltraGridBand band in e.Layout.Bands)
                band.Hidden = (band.Key != "PurchaseItem");

            // Add custom rows
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

            // Show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "PurchaseLineID")
                {
                    c.Header.Caption = "Bkg ID";
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
                    c.Header.ToolTipText = "The service name";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                }
                else if (c.Key == "OptionTypeName")
                {
                    c.Header.Caption = "Option type";
                    c.Header.ToolTipText = "The option type";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
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
                    c.TabStop = true;
                }
                else if (c.Key == "Grade")
                {
                    c.Header.Caption = "Grade 1";
                    c.Header.ToolTipText = "Supplier grade";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "GradeExternal")
                {
                    c.Header.Caption = "Grade 2";
                    c.Header.ToolTipText = "Supplier grade";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "NetTotalConverted")
                {
                    c.Header.Caption = "Total Net";
                    c.Header.ToolTipText = "Total net cost of item, in your currency";
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "GrossTotalConverted")
                {
                    c.Header.Caption = "Total Gross";
                    c.Header.ToolTipText = "Total gross cost of item, in your currency";
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "IsLockedAccounting")
                {
                    c.Header.Caption = String.Empty;
                    c.Header.Appearance.Image = TourWriter.Properties.Resources.LockEdit;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.TabStop = false;
                    c.Hidden = true;
                }
                else if (c.Key == "Flags")
                {
                    c.DataType = typeof (Bitmap);
                    c.CellAppearance.ImageHAlign = HAlign.Left;
                    c.CellAppearance.ImageVAlign = VAlign.Middle;
                    // TODO: flags hidden for now (see line 57 too)
                    c.Hidden = true;
                    c.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                }
                else
                {
                    c.Hidden = true;
                    c.TabStop = false;
                }
            }

            e.Layout.Bands[0].Columns["Flags"].Width = 23;
            e.Layout.Bands[0].Columns["PurchaseLineID"].Width = 10;
            e.Layout.Bands[0].Columns["PurchaseLineName"].Width = 60;
            e.Layout.Bands[0].Columns["ServiceName"].Width = 60;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Width = 100;
            e.Layout.Bands[0].Columns["CityName"].Width = 27;
            e.Layout.Bands[0].Columns["RegionName"].Width = 27;
            e.Layout.Bands[0].Columns["ServiceTypeName"].Width = 20;
            e.Layout.Bands[0].Columns["BookingReference"].Width = 20;
            e.Layout.Bands[0].Columns["RequestStatusID"].Width = 30;
            e.Layout.Bands[0].Columns["StartDate"].Width = 30;
            e.Layout.Bands[0].Columns["StartTime"].Width = 30;
            e.Layout.Bands[0].Columns["NumberOfDays"].Width = 10;
            e.Layout.Bands[0].Columns["Quantity"].Width = 10;
            e.Layout.Bands[0].Columns["Grade"].Width = 30;
            e.Layout.Bands[0].Columns["GradeExternal"].Width = 30;
            e.Layout.Bands[0].Columns["NetTotalConverted"].Width = 30;
            e.Layout.Bands[0].Columns["GrossTotalConverted"].Width = 30;
            e.Layout.Bands[0].Columns["IsLockedAccounting"].Width = 30;

            int index = 0;

            e.Layout.Bands[0].Columns["Flags"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["IsLockedAccounting"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseLineID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseLineName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ServiceName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["OptionTypeName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["CityName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["RegionName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ServiceTypeName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["BookingReference"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["RequestStatusID"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["StartDate"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["StartTime"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NumberOfDays"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Quantity"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Grade"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["GradeExternal"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["NetTotalConverted"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["GrossTotalConverted"].Header.VisiblePosition = index;

            // Set defaults
            GridHelper.SetDefaultGridAppearance(e);
            GridHelper.SetDefaultGroupByAppearance(e);

            // Override defaults
            e.Layout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
            e.Layout.Override.SelectTypeRow = SelectType.Extended;
            e.Layout.GroupByBox.Hidden = false;
            e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

            SetGridSummaries(e);
        }

        private void grid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Band.Key != "PurchaseItem")
                return;

            try
            {
                // Set the purchaseline name.
                e.Row.Cells["PurchaseLineName"].Value = itinerarySet.PurchaseLine.FindByPurchaseLineID(
                    (int) e.Row.Cells["PurchaseLineID"].Value).PurchaseLineName;

                int itemId = (int) e.Row.Cells["PurchaseItemID"].Value;

                // Set the city name.
                int? cityId = itinerarySet.GetPurchaseItemCityId(itemId);
                if (cityId.HasValue)
                {
                    ToolSet.CityRow city = Cache.ToolSet.City.FindByCityID((int) cityId);
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
                    ToolSet.RegionRow region = Cache.ToolSet.Region.FindByRegionID((int) regionId);
                    if (region != null && region.RegionName != null)
                    {
                        e.Row.Cells["RegionName"].Value = region.RegionName;
                    }
                    else
                    {
                        e.Row.Cells["RegionName"].Value = "<none>";
                    }
                }
                // Set the grades
                int? gradeId = itinerarySet.GetPurchaseItemGradeId(itemId);
                if (gradeId.HasValue)
                {
                    ToolSet.GradeRow grade = Cache.ToolSet.Grade.FindByGradeID((int) gradeId);
                    e.Row.Cells["Grade"].Value = (grade != null) ? grade.GradeName : null;
                }
                int? gradeExternalId = itinerarySet.GetPurchaseItemGradeExternalId(itemId);
                if (gradeExternalId.HasValue)
                {
                    ToolSet.GradeExternalRow gradeExternal
                        = Cache.ToolSet.GradeExternal.FindByGradeExternalID((int) gradeExternalId);
                    e.Row.Cells["GradeExternal"].Value = (gradeExternal != null) ? gradeExternal.GradeExternalName : null;
                }
                SetFlags(e.Row);

                // disable the row if it has been exported to accounting
                if (e.Row.Cells["IsLockedAccounting"].Value != DBNull.Value &&
                    (bool) e.Row.Cells["IsLockedAccounting"].Value)
                {
                    SetRowLocked(e.Row, true);
                    EnableDisableButtons(e.Row);
                }
                else
                {
                    SetRowLocked(e.Row, false);
                    EnableDisableButtons(e.Row);
                }
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("Key not found"))
                {
                    bool resetLayout = App.AskYesNo("Problem found with bookings grid layout which may cause bookings to not display correctly. Resetting the grid layout should fix the problem.\r\n\r\nWould you like to reset now?");
                    if (resetLayout)
                        ResetGridLayout();

                    ErrorHelper.SendEmail(ex, true);
                }
                else
                {
                    throw;
                }
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
                AddNewBooking(supplierId);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow == null || grid.ActiveRow.IsGroupByRow)
                return;

            if (grid.ActiveRow.ParentRow != null)
                grid.ActiveRow.ParentRow.Expanded = true;


            if (App.AskDeleteRow())
            {
                // Remember the parent line id.
                int lineId = (int)grid.ActiveRow.Cells["PurchaseLineID"].Value;

                // Delete the item.
                GridHelper.DeleteActiveRow(grid, true);

                // Handle empty booking.
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
            List<int> idList = new List<int>();

            foreach (ItinerarySet.PurchaseLineRow row in itinerarySet.PurchaseLine)
            {
                if (row.RowState != DataRowState.Deleted &&
                    row.GetPurchaseItemRows().Length > 0)
                {
                    idList.Add(row.PurchaseLineID); // don't add empty bookings
                }
            }

            if (idList.Count > 0)
                SendBookingRequest(idList);
        }

        private void btnBookSelected_Click(object sender, EventArgs e)
        {
            if (grid.Selected.Rows.Count == 0 && grid.ActiveRow != null)
                grid.ActiveRow.Selected = true; // Sometimes a row is active but not selected

            List<int> idList = new List<int>();

            foreach (UltraGridRow row in grid.Selected.Rows)
            {
                if (row.IsGroupByRow)
                    continue;

                int id = (int)row.Cells["PurchaseLineID"].Value;
                if (!idList.Contains(id))
                    idList.Add(id);
            }

            if (idList.Count > 0)
                SendBookingRequest(idList);
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
            dateKicker.ShowDialog();
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
            // Purchase line name is an unbound column, so need to catch edits
            // and manually update its underlying datasource.
            if (e.Cell.Column.Key == "PurchaseLineName")
            {
                int lineId = (int)e.Cell.Row.Cells["PurchaseLineID"].Value;
                ItinerarySet.PurchaseLineRow row = itinerarySet.PurchaseLine.FindByPurchaseLineID(lineId);
                if (row.PurchaseLineName != e.Cell.Text)
                    row.PurchaseLineName = e.Cell.Text;
                return;
            }

            // When the "Locked" checkbox is clicked, it needs to be forced out of edit mode
            // so that the row will immediately reflect the change (become enabled/disabled)
            if (e.Cell.Column.Key == "IsLockedAccounting")
            {
                e.Cell.EditorResolved.ExitEditMode(true, true);
                return;
            }
        }

        private void grid_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            // update booking rates when date changes
            if (e.Cell.Column.Key == "StartDate" && e.Cell.EditorResolved.Value != DBNull.Value)
            {
                if (App.AskYesNo("Auto-update rate also?"))
                {
                    e.Cancel = true; // let the date kicker handle the date change

                    int delta = (((DateTime)e.NewValue).Date -
                                ((DateTime)e.Cell.Row.Cells["StartDate"].Value).Date).Days;

                    DateKickerForm dateKicker = new DateKickerForm(itinerarySet, delta, true);
                    dateKicker.SetSelectedRow((int)e.Cell.Row.Cells["PurchaseItemID"].Value);
                    dateKicker.ShowDialog();
                }
            }
        }

        private void grid_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "StartDate") e.Cell.Row.Update(); // fire immediate update
        }

        private void BookingsViewer_Leave(object sender, EventArgs e)
        {
            SaveGridLayout();
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
            CurrencyUpdater currencyUpdater = new CurrencyUpdater(itinerarySet);

            if (currencyUpdater.ShowDialog() == DialogResult.OK)
            {
                foreach (ItinerarySet.PurchaseItemRow row in itinerarySet.PurchaseItem)
                {
                    if (row.RowState != DataRowState.Deleted)
                        row.RecalculateTotals();
                }
            }
        }

        private void chkLockGrossOverride_CheckedChanged(object sender, EventArgs e)
        {
            if (itinerarySet.Itinerary[0].IsGrossOverrideNull())
            {
                itinerarySet.Itinerary[0].GrossOverride = itinerarySet.GetGrossFinalPrice();
                txtPriceOverride.Value = itinerarySet.Itinerary[0].GrossOverride;
            }

            txtPriceOverride.ReadOnly = chkLockGrossOverride.Checked;
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
