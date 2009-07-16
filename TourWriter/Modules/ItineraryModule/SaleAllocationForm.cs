using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinCalcManager;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using System;
using TourWriter.Services;
using ButtonDisplayStyle=Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace TourWriter.Modules.ItineraryModule
{
    public partial class SaleAllocationForm : Form
    {
        private readonly ItinerarySet itinerarySet;
        private readonly ItinerarySet itinerarySetOrig;
        private readonly ItineraryMain itineraryMain; // needed for the save method
        private readonly DataTable serviceTypeTotalsTable; // needed for storing service type totals

        public SaleAllocationForm(ItineraryMain itineraryMain, ItinerarySet itinerarySet, DataSet dsAdditionalData, int? selectedSaleId)
        {
            InitializeComponent();
            
            Icon = Properties.Resources.TourWriter16;
            itinerarySetOrig = itinerarySet;
            this.itinerarySet = (ItinerarySet)itinerarySet.Copy();
            this.itineraryMain = itineraryMain;
            serviceTypeTotalsTable = dsAdditionalData.Tables[0];
            DataBind();
            CalculateTotals();

            if (selectedSaleId != null)
            {
                foreach (UltraGridRow row in gridSales.Rows)
                {
                    if (selectedSaleId == (int)row.Cells["ItinerarySaleID"].Value)
                    {
                        gridSales.ActiveRow = row;
                    }
                }
            }
        }

        public SaleAllocationForm(ItineraryMain itineraryMain, ItinerarySet itinerarySet, DataSet dsAdditionalData)
            : this(itineraryMain, itinerarySet, dsAdditionalData, null)
        {
            
        }

        private void DataBind()
        {
            gridSales.DataSource = itinerarySet.ItinerarySale;
            gridAllocations.DataSource = Cache.ToolSet.ServiceType;
            itineraryPaymentBindingSource.DataSource = itinerarySet.ItineraryPayment;
        }

        private void LoadSaleAllocations(int saleId)
        {
            EnableDisableControls(true);

            foreach (UltraGridRow row in gridAllocations.Rows)
            {
                int serviceTypeId = (int)row.Cells["ServiceTypeID"].Value;
                string selectString = String.Format("ItinerarySaleID = {0} AND ServiceTypeID = {1}", saleId, serviceTypeId);
                DataRow[] results = itinerarySet.ItinerarySaleAllocation.Select(selectString);
                row.Cells["Amount"].Value = (results.Length > 0) ? results[0]["Amount"] : DBNull.Value;
            }

            CalculateTotals();
        }

        private void ClearSaleAllocations(bool updateTable)
        {
            foreach (UltraGridRow row in gridAllocations.Rows)
            {
                row.Cells["Amount"].Value = DBNull.Value;
                row.Cells["IsAmountAdjusted"].Value = false;

                if (updateTable)
                    UpdateAllocationsTable(row);
            }
        }

        private void ClearSaleAllocations()
        {
            ClearSaleAllocations(false);
        }

        private void SaveChanges()
        {
            // check for rows that have been deleted, but do not exist in the database
            for (int i = itinerarySetOrig.ItinerarySale.Count - 1; i >= 0; i--)
            {
                var row = itinerarySetOrig.ItinerarySale[i];
                if (row.RowState != DataRowState.Deleted && row.ItinerarySaleID < 0)
                {
                    if (itinerarySet.ItinerarySale.FindByItinerarySaleID(row.ItinerarySaleID) == null)
                    {
                        itinerarySetOrig.ItinerarySale.RemoveItinerarySaleRow(row);
                    }
                }
            }

            var changes = itinerarySet.GetChanges();
            if (changes != null)
            {
                itinerarySetOrig.Merge(changes);
            }

            itineraryMain.SaveChanges();
        }

        private void CalculateTotals()
        {
            foreach (UltraGridRow row in gridSales.Rows)
            {
                // calculate the total sales for the each sale row
                int saleId = (int) row.Cells["ItinerarySaleID"].Value;

                var saleRow = itinerarySet.ItinerarySale.Where(s =>
                                                               s.RowState != DataRowState.Deleted &&
                                                               s.ItinerarySaleID == saleId).First();

                if (saleRow != null) row.Cells["Amount"].Value = Decimal.Round(saleRow.Amount, 2);
            }

            foreach (UltraGridRow row in gridAllocations.Rows)
            {
                int serviceTypeId = (int)row.Cells["ServiceTypeID"].Value;

                // calculate the total sale amount
                decimal total = 0;
                DataRow[] allocationTotals = itinerarySet.ItinerarySaleAllocation.Select("ServiceTypeID = " + serviceTypeId);
                foreach (DataRow r in allocationTotals)
                {
                    total += (decimal)r["Amount"];
                }
                row.Cells["TotalSales"].Value = total;

                if (serviceTypeTotalsTable != null)
                {
                    // retrieve the total gross for this service type
                    DataRow[] serviceTypeTotals = serviceTypeTotalsTable.Select("ServiceTypeID = " + serviceTypeId);
                    row.Cells["TotalGross"].Value = (serviceTypeTotals.Length > 0) ? serviceTypeTotals[0]["Gross"] : DBNull.Value;

                    // calculate the outstanding cost
                    decimal gross = row.Cells["TotalGross"].Value != DBNull.Value ? (decimal)row.Cells["TotalGross"].Value : 0;
                    decimal sales = (decimal)row.Cells["TotalSales"].Value;
                    row.Cells["Outstanding"].Value = gross - sales;
                }
            }
        }

        private void LockUnlockSalesRow(UltraGridRow row)
        {
            // disable row if it's locked
            bool isLocked = row.Cells["IsLockedAccounting"].Value != DBNull.Value
                            && (bool)row.Cells["IsLockedAccounting"].Value;

            foreach (UltraGridColumn column in gridSales.DisplayLayout.Bands[0].Columns)
            {
                if (column.Key == "IsLockedAccounting")
                {
                    continue;
                }
                row.Cells[column].Activation = (isLocked) ? Activation.Disabled : Activation.AllowEdit;
            }

            if (row.Activated)
            {
                if (isLocked)
                {
                    // if the row is active and locked, deactivate it
                    gridSales.ActiveRow = null;
                    EnableDisableControls(false);
                    ClearSaleAllocations();
                }
                else
                {
                    // if the row is active but not locked, load the sale allocations
                    int saleId = (int)gridSales.ActiveRow.Cells["ItinerarySaleID"].Value;
                    LoadSaleAllocations(saleId);
                }
            }
        }

        private void AutoAllocate(decimal amount)
        {
            List<UltraGridRow> autoCalcRows = new List<UltraGridRow>();
            foreach (UltraGridRow row in gridAllocations.Rows)
            {
                if (row.Cells["TotalGross"].Value == DBNull.Value)
                    continue;

                bool isAmountAdjusted = (bool)row.Cells["IsAmountAdjusted"].Value;
                if (isAmountAdjusted)
                {
                    // subtract the adjusted amount from the total amount
                    decimal adjustedAmount = (row.Cells["Amount"].Value != DBNull.Value)
                                             ? (decimal)row.Cells["Amount"].Value : 0;
                    amount -= adjustedAmount;
                }
                else
                {
                    // only auto allocate on rows that haven't been adjusted manually
                    autoCalcRows.Add(row);
                }
            }

            decimal totalTotalGross = 0;
            foreach (UltraGridRow row in autoCalcRows)
            {
                decimal totalGross = (decimal)row.Cells["TotalGross"].Value;
                if (totalGross > 0)
                {
                    totalTotalGross += totalGross;
                }
            }

            foreach (UltraGridRow row in autoCalcRows)
            {
                decimal totalGross = (decimal)row.Cells["TotalGross"].Value;
                if (totalGross > 0)
                {
                    decimal splitAmount = amount * (totalGross / totalTotalGross);
                    row.Cells["Amount"].Value = splitAmount;
                    UpdateAllocationsTable(row);
                }
            }

            CalculateTotals();
        }

        private void UpdateAllocationsTable(UltraGridRow row)
        {
            int saleId = (int)gridSales.ActiveRow.Cells["ItinerarySaleID"].Value;
            int serviceTypeId = (int)row.Cells["ServiceTypeID"].Value;
            decimal? amount = row.Cells["Amount"].Value != DBNull.Value ? (decimal?)row.Cells["Amount"].Value : null;
            itinerarySet.ItinerarySaleAllocation.AddInsertOrDelete(saleId, serviceTypeId, amount);
        }

        private void EnableDisableControls(bool enabled)
        {
            gridAllocations.Enabled = enabled;
        }

        private void BuildReceiptsList()
        {
            if (!gridSales.DisplayLayout.ValueLists.Exists("ReceiptsList"))
            {
                // create the value list if it doesn't exist
                gridSales.DisplayLayout.ValueLists.Add("ReceiptsList");
                gridSales.DisplayLayout.ValueLists["ReceiptsList"].SortStyle = ValueListSortStyle.Ascending;
            }

            // clear the value list
            gridSales.DisplayLayout.ValueLists["ReceiptsList"].ValueListItems.Clear();

            // populate the value list
            foreach (ItinerarySet.ItineraryPaymentRow row in itinerarySet.ItineraryPayment)
            {
                if (row.IsAmountNull())
                    continue;

                gridSales.DisplayLayout.ValueLists["ReceiptsList"].ValueListItems.Add(row.Amount, row.Amount.ToString("c"));
            }
        }

        #region Events

        private void gridSales_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            BuildReceiptsList();

            e.Layout.Bands[0].Columns.Add("Amount");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "IsLockedAccounting")
                {
                    c.Header.Caption = String.Empty;
                    c.Header.Appearance.Image = Properties.Resources.LockEdit;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.CellClickAction = CellClickAction.Edit;
                    c.Header.VisiblePosition = 0;
                    c.Width = 30;
                    c.MaxWidth = 30;
                }
                else if (c.Key == "Comments")
                {
                    c.CellClickAction = CellClickAction.EditAndSelectText;
                    c.Width = 100;
                }
                else if (c.Key == "SaleDate")
                {
                    c.Header.Caption = "Date";
                    c.Format = App.GetLocalShortDateFormat();
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.Width = 50;
                }
                else if (c.Key == "Amount")
                {
                    c.DataType = typeof(Decimal);
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.Style = ColumnStyle.DropDown;
                    c.ValueList = e.Layout.Grid.DisplayLayout.ValueLists["ReceiptsList"];
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                {
                    c.Hidden = true;
                }
            }

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Bands[0].Override.RowSelectors = DefaultableBoolean.False;
        }

        private void gridSales_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (!e.ReInitialize)
                LockUnlockSalesRow(e.Row);
        }

        private void gridSales_AfterRowActivate(object sender, EventArgs e)
        {
            bool isLocked = gridSales.ActiveRow.Cells["IsLockedAccounting"].Value != DBNull.Value
                            && (bool)gridSales.ActiveRow.Cells["IsLockedAccounting"].Value;

            if (isLocked)
            {
                UltraGridCell cell = GridHelper.GetValidClickCell(gridSales);
                bool isLockCellClick = (cell != null && cell.Column.Key == "IsLockedAccounting");
                if (!isLockCellClick)
                {
                    gridSales.ActiveRow = null;
                }
            }
            else
            {
                int saleId = (int)gridSales.ActiveRow.Cells["ItinerarySaleID"].Value;
                LoadSaleAllocations(saleId);
            }
        }

        private void gridSales_AfterExitEditMode(object sender, EventArgs e)
        {
            if (gridSales.ActiveCell == null)
                return;

            if (gridSales.ActiveCell.Column.Key == "Amount")
            {
                if (gridSales.ActiveCell.Value != null &&
                    gridSales.ActiveCell.Value != DBNull.Value)
                {
                    decimal amount = (decimal)gridSales.ActiveCell.Value;
                    ClearSaleAllocations(true);
                    AutoAllocate(amount);
                }
            }
            else if (gridSales.ActiveCell.Column.Key == "IsLockedAccounting")
            {
                LockUnlockSalesRow(gridSales.ActiveRow);
            }
        }

        private void gridSales_CellChange(object sender, CellEventArgs e)
        {
            // When the "Locked" checkbox is clicked, it needs to be forced out of edit mode
            // so that the row will immediately reflect the change (become enabled/disabled)
            if (e.Cell.Column.Key == "IsLockedAccounting")
            {
                e.Cell.EditorResolved.ExitEditMode(true, true);
            }
        }

        private void gridSales_Click(object sender, EventArgs e)
        {
            if (gridSales.ActiveRow == null)
            {
                EnableDisableControls(false);
                ClearSaleAllocations();
            }
        }

        private void btnSaleAdd_Click(object sender, EventArgs e)
        {
            ItinerarySet.ItinerarySaleRow saleRow = itinerarySet.ItinerarySale.AddItinerarySaleRow(itinerarySet.Itinerary[0].ItineraryID, String.Empty, DateTime.Now, false);
            saleRow.Comments = App.CreateUniqueNameValue(gridSales.Rows, "Comments", "New sale");

            if (itinerarySet.ItinerarySale.Rows.Count > 1)
            {
                // default to previous sale date
                for (int previousIndex = itinerarySet.ItinerarySale.Rows.Count - 2; previousIndex >= 0; previousIndex--)
                {
                    if (itinerarySet.ItinerarySale[previousIndex].RowState == DataRowState.Deleted)
                        continue;

                    saleRow.SaleDate = itinerarySet.ItinerarySale[previousIndex].SaleDate;
                    break;
                }
            }
            
            // select last row
            gridSales.ActiveRow = gridSales.Rows[gridSales.Rows.Count - 1];
            gridSales.ActiveCell = gridSales.ActiveRow.Cells["Comments"];
            gridSales.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }

        private void btnSaleDel_Click(object sender, EventArgs e)
        {
            if (gridSales.ActiveRow == null)
                return;

            int saleId = (int)gridSales.ActiveRow.Cells["ItinerarySaleID"].Value;
            ItinerarySet.ItinerarySaleRow saleRow = itinerarySet.ItinerarySale.FindByItinerarySaleID(saleId);
            saleRow.Delete();

            if (gridSales.ActiveRow == null)
            {
                EnableDisableControls(false);
                ClearSaleAllocations();
            }
        }

        private void gridAllocations_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.Add("TotalSales");
            e.Layout.Bands[0].Columns.Add("Amount");
            e.Layout.Bands[0].Columns.Add("TotalGross");
            e.Layout.Bands[0].Columns.Add("Outstanding");
            e.Layout.Bands[0].Columns.Add("IsAmountAdjusted");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.CellActivation = Activation.Disabled;

                if (c.Key == "ServiceTypeName")
                {
                    c.Header.Caption = "Service Type";
                }
                else if (c.Key == "TotalSales")
                {
                    c.Header.Caption = "Total Sales";
                    c.DataType = typeof (Decimal);
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "Amount")
                {
                    c.Header.Caption = "This Sale";
                    c.DataType = typeof(Decimal);
                    c.Format = "c";
                    c.MaskInput = "$-nnnnnnn.nn";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "TotalGross")
                {
                    c.Header.Caption = "Total Gross";
                    c.DataType = typeof(Decimal);
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "Outstanding")
                {
                    c.DataType = typeof(Decimal);
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "IsAmountAdjusted")
                {
                    c.DataType = typeof(Boolean);
                    c.Hidden = true;
                }
                else
                {
                    c.Hidden = true;
                }
            }

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Bands[0].Override.RowSelectors = DefaultableBoolean.False;
            gridAllocations_CreateSummaries(e);
        }

        private static void gridAllocations_CreateSummaries(InitializeLayoutEventArgs e)
        {
            GridHelper.SetDefaultSummaryAppearance(e);

            // UltraCalcManager required for formula summaries
            e.Layout.Grid.CalcManager = new UltraCalcManager(e.Layout.Grid.Container);

            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            band.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            SummarySettings summary;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["TotalSales"]);
            summary.Key = "TotalSalesTotal";
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "${0:###,###,###.00}";
            summary.Appearance.TextHAlign = HAlign.Right;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["TotalGross"]);
            summary.Key = "TotalGrossTotal";
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "${0:###,###,###.00}";
            summary.Appearance.TextHAlign = HAlign.Right;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Outstanding"]);
            summary.Key = "OutstandingTotal";
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "${0:###,###,###.00}";
            summary.Appearance.TextHAlign = HAlign.Right;
        }

        private void gridAllocations_AfterExitEditMode(object sender, EventArgs e)
        {
            UltraGridCell cell = gridAllocations.ActiveCell;

            if (cell.Column.Key == "Amount")
            {
                if (gridSales.ActiveRow == null)
                {
                    // if no sale row is selected, don't allow edits
                    cell.Value = DBNull.Value;
                    return;
                }

                if (gridSales.ActiveRow.Cells["Amount"].Value != DBNull.Value)
                {
                    cell.Row.Cells["IsAmountAdjusted"].Value = true;

                    // perform the auto calculate
                    decimal calcAmount = (decimal)gridSales.ActiveRow.Cells["Amount"].Value;
                    AutoAllocate(calcAmount);
                }

                UpdateAllocationsTable(cell.Row);
                CalculateTotals();
            }
        }

        private void gridAllocations_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // initalize to DBNull.Value to make calculations easier
            if (e.Row.Cells["TotalGross"].Value == null)
                e.Row.Cells["TotalGross"].Value = DBNull.Value;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!App.AskYesNo("This will save any changes you have made to this itinerary.\r\n\r\nDo you want to continue?"))
            {
                DialogResult = DialogResult.None;
                return;
            }

            SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}
