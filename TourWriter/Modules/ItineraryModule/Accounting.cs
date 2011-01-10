using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Modules.ItineraryModule
{
    public partial class Accounting : UserControl
    {
        private const string GridColumnRegex = "(?<=Key id=\"ref-\\d*\">){0}(?=</Key>)";

        private ItinerarySet itinerarySet;
        private readonly string gridLayoutsPath;
        private readonly string allocationsGridLayoutFileName;
        private readonly string paymentsGridLayoutFileName;
        private readonly string purchasesGridLayoutFileName;
        private readonly Dictionary<string, string> GridColumnRenames = new Dictionary<string, string>();

        #region Properties

        public ItinerarySet ItinerarySet
        {
            set
            {
                itinerarySet = value;
                DataBind();
            }
        }

        internal bool RefreshRequired
        {
            get
            {
                return (gridSales.DataSource == null
                        && gridAllocations.DataSource == null
                        && gridPayments.DataSource == null
                        && gridPurchases.DataSource == null);
            }
            set
            {
                if (value)
                {
                    gridSales.DataSource = null;
                    gridAllocations.DataSource = null;
                    gridPayments.DataSource = null;
                    gridPurchases.DataSource = null;
                    EnableControls = false;

                    if (Visible)
                    {
                        RefreshDataAndControls();
                    }
                }
            }
        }

        /// <summary>
        /// Tests whether underlying data edits have occured that could affect this accounting data 
        /// </summary>
        internal bool SaveRequired
        {
            get
            {
                return (itinerarySet != null && itinerarySet.HasAccountingChanges());
            }
        }

        /// <summary>
        /// Enables/disables the controls, and hides/shows the refresh message
        /// </summary>
        internal bool EnableControls
        {
            set
            {
                tabControl.Enabled = value;
                btnTransfer.Enabled = value;
                btnSaveLayout.Enabled = value;
            }
        }

        #endregion

        public Accounting()
        {
            InitializeComponent();

            EnableControls = false;

            // This is needed to stop ItineraryMain designer throwing an error,
            // DesignMode is always false aswell, so it will not work.
            if (Cache.IsUserLoggedIn)
                gridLayoutsPath = Cache.ToolSet.AppSettings[0].ExternalFilesPath + @"\Templates\Accounting\";

            allocationsGridLayoutFileName = gridLayoutsPath + @"AllocationsGridLayout.xml";
            paymentsGridLayoutFileName = gridLayoutsPath + @"PaymentsGridLayout.xml";
            purchasesGridLayoutFileName = gridLayoutsPath + @"PurchasesGridLayout.xml";

            // add column renames
            GridColumnRenames.Add("PurchaseStatus", "Status");

            // TODO: hiding Receipts tab, probably not required...
            tabControl.Tabs["Receipts"].Visible = false; 
        }

        public void DataBind()
        {
            // bind the itinerary status combo box
            cmbItineraryStatus.DataSource = Cache.ToolSet.ItineraryStatus;
            cmbItineraryStatus.ValueMember = "ItineraryStatusID";
            cmbItineraryStatus.DisplayMember = "ItineraryStatusName";

            cmbItineraryStatus.DataBindings.Clear();
            cmbItineraryStatus.DataBindings.Add(new Binding("SelectedValue", itinerarySet.Itinerary, "ItineraryStatusID", true));
        }

        /// <summary>
        /// Manages refreshing data and useability of controls based if 
        /// underlying data has changed
        /// </summary>
        public void RefreshDataAndControls()
        {
            Application.DoEvents();

            if (SaveRequired)
            {
                if (!App.AskYesNo("You must save in order to refresh the accounting data.\r\nDo you want to save?"))
                    return;

                if (ParentForm != null)
                    ((ItineraryMain)ParentForm).SaveChanges();

                RefreshDataAndControls(); // retry in case save failed
                return;
            }
    
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                
                LoadData();

                EnableControls = true;
                LoadGridLayouts();

                // set all the rows to selected by default
                gridPurchases.SelectAllRows();
                gridSales.SelectAllRows();
                gridPayments.SelectAllRows();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void InitializeGrids()
        {
            gridPurchases.InitializeLayoutEvent += gridPurchases_InitializeLayoutEvent;
            gridAllocations.InitializeLayoutEvent += gridAllocations_InitializeLayoutEvent;
            //gridPayments.InitializeLayoutEvent += gridPayments_InitializeLayoutEvent;
            gridSales.InitializeLayoutEvent += gridSales_InitializeLayoutEvent;

            gridSales.UltraGrid.DoubleClickRow += gridSales_DoubleClickRow;
            gridSales.UltraGrid.InitializeRow += gridSales_InitializeRow;

            gridSales.AddToolStripButton("Sales edit", "Open the sales editor", delegate { OpenSalesEditor(); }, true);
        }

        private DataSet tempAllocDataSet;
        private void LoadData()
        {
            var data = new AccountingDataLoad(itinerarySet.Itinerary[0].ItineraryID);
            data.LoadDataAsync();

            gridPurchases.DataSource = null;
            gridSales.DataSource = null;
            gridAllocations.DataSource = null;
            gridPayments.DataSource = null;

            App.PrepareDataTableForExport(data.PurchasesDataSet.Tables[0]);
            App.PrepareDataTableForExport(data.AllocationsDataSet.Tables[0]);
            //App.PrepareDataTableForExport(data.ReceiptsDataSet.Tables[0]);

            gridPurchases.DataSource = data.PurchasesDataSet;
            gridAllocations.DataSource = data.AllocationsDataSet;
            gridPayments.DataSource = data.ReceiptsDataSet;
            gridSales.DataSource = itinerarySet.ItinerarySale;

            tempAllocDataSet = data.ServiceTypesDataSet;
        }

        private void OpenSalesEditor()
        {
            SaleAllocationForm allocationForm;

            if (gridSales.UltraGrid.ActiveRow != null)
            {
                int saleId = (int)gridSales.UltraGrid.ActiveRow.Cells["ItinerarySaleID"].Value;
                allocationForm = new SaleAllocationForm((ItineraryMain)ParentForm, itinerarySet, tempAllocDataSet, saleId);
            }
            else
            {
                allocationForm = new SaleAllocationForm((ItineraryMain)ParentForm, itinerarySet, tempAllocDataSet);
            }

            allocationForm.ShowDialog();
        }

        private void LoadGridLayouts()
        {
            if (File.Exists(allocationsGridLayoutFileName))
            {
                UpdateGridLayouts(allocationsGridLayoutFileName);

                gridAllocations.UltraGrid.DisplayLayout.LoadFromXml(
                    allocationsGridLayoutFileName, PropertyCategories.Groups | PropertyCategories.SortedColumns);
            }
            if (File.Exists(paymentsGridLayoutFileName))
            {
                UpdateGridLayouts(paymentsGridLayoutFileName);

                gridPayments.UltraGrid.DisplayLayout.LoadFromXml(
                    paymentsGridLayoutFileName, PropertyCategories.Groups | PropertyCategories.SortedColumns);
            }
            if (File.Exists(purchasesGridLayoutFileName))
            {
                UpdateGridLayouts(purchasesGridLayoutFileName);

                gridPurchases.UltraGrid.DisplayLayout.LoadFromXml(
                    purchasesGridLayoutFileName, PropertyCategories.Groups | PropertyCategories.SortedColumns);
            }
        }

        private void SaveGridLayouts()
        {
            if (!Directory.Exists(gridLayoutsPath))
                Directory.CreateDirectory(gridLayoutsPath);

            gridAllocations.UltraGrid.DisplayLayout.SaveAsXml(
                allocationsGridLayoutFileName, PropertyCategories.Groups | PropertyCategories.SortedColumns);

            gridPayments.UltraGrid.DisplayLayout.SaveAsXml(
                paymentsGridLayoutFileName, PropertyCategories.Groups | PropertyCategories.SortedColumns);

            gridPurchases.UltraGrid.DisplayLayout.SaveAsXml(
                purchasesGridLayoutFileName, PropertyCategories.Groups | PropertyCategories.SortedColumns);
        }

        /// <summary>
        /// Renames columns in the grid layout file so column changes don't break old layouts.
        /// </summary>
        private void UpdateGridLayouts(string layoutFile)
        {
            string xml;
            using (TextReader reader = new StreamReader(layoutFile))
            {
                xml = reader.ReadToEnd();
            }

            foreach (var kvp in GridColumnRenames)
            {
                string oldColumn = kvp.Key;
                string newColumn = kvp.Value;
                string pattern = String.Format(GridColumnRegex, oldColumn);

                xml = Regex.Replace(xml, pattern, newColumn);
            }

            using (TextWriter writer = new StreamWriter(layoutFile))
            {
                writer.Write(xml);
            }
        }

        /// <summary>
        /// Open the accounting export dialog, and export everything that the user selected to CSV.
        /// </summary>
        private void ExportAllToCsv()
        {
            if (!AppPermissions.UserHasPermission(AppPermissions.Permissions.AccountingEdit))
            {
                App.ShowError(App.GetResourceString("ShowPermissionDenied"));
                return;
            }

            string itineraryName = itinerarySet.Itinerary[0].ItineraryName;

            var export = new AccountingExportForm();
            export.PurchasesFilename = string.Format("{0}-Purchases.txt", itineraryName);
            export.SuppliersFilename = string.Format("{0}-Suppliers.txt", itineraryName);
            export.SalesFilename = string.Format("{0}-Sales.txt", itineraryName);
            export.PaymentsFilename = string.Format("{0}-Payments.txt", itineraryName);
            export.ClientsFilename = string.Format("{0}-Clients.txt", itineraryName);

            if (export.ShowDialog() == DialogResult.OK)
            {
                if (export.IncludePurchases)
                {
                    AccountingExport.ExportPurchasesToCsv(gridPurchases, export.PurchasesFilename);
                    LockExportedBookings();
                }

                if (export.IncludeSuppliers)
                    AccountingExport.ExportSuppliersToCsv(gridPurchases, itinerarySet.SupplierLookup, export.SuppliersFilename);

                if (export.IncludeSales)
                {
                    AccountingExport.ExportSalesToCsv(gridSales, gridAllocations, export.SalesFilename);
                    LockExportedSales();
                }

                if (export.IncludePayments)
                    AccountingExport.ExportPaymentsToCsv(gridPayments, export.PaymentsFilename);

                if (export.IncludeClients)
                    AccountingExport.ExportClientsToCsv(itinerarySet.Itinerary, export.ClientsFilename);
            }
        }

        private void LockExportedBookings()
        {
            var rowList = GridHelper.GetDataRowsList(gridPurchases.UltraGrid);

            // lock all of the exported booking items
            foreach (var row in rowList)
            {
                bool isSelected = Convert.ToBoolean(row.Cells["IsSelected"].Value);
                if (!isSelected)
                    continue;

                int purchaseItemId = Convert.ToInt32(row.Cells["PurchaseItemID"].Value);
                ItinerarySet.PurchaseItemRow purchaseItem =
                    itinerarySet.PurchaseItem.FindByPurchaseItemID(purchaseItemId);
                if (purchaseItem != null) purchaseItem.IsLockedAccounting = true;
            }
        }

        private void LockExportedSales()
        {
            DataTable salesTable = gridSales.GetDataRowsTable();
            DataTable allocationsTable = gridAllocations.GetDataRowsTable();

            // remove rows that aren't selected
            for (int i = salesTable.Rows.Count - 1; i >= 0; i--)
            {
                bool isSelected = Convert.ToBoolean(salesTable.Rows[i]["IsSelected"]);
                if (!isSelected)
                {
                    // delete all sale detail rows that match the unselected sale row's id
                    // and ones that have no service type allocation
                    int saleId = Convert.ToInt32(salesTable.Rows[i]["ItinerarySaleID"]);
                    DataRow[] results = allocationsTable.Select("ItinerarySaleID = " + saleId);

                    foreach (DataRow row in results)
                        row.Delete();
                }
            }

            if (salesTable.Rows.Count == 0 || allocationsTable.Rows.Count == 0)
                return;

            var selectedRows = salesTable.Select("IsSelected = 'TRUE'");

            string saleIds = App.DataRowsToCsv(selectedRows, "ItinerarySaleID");
            var allocationRows = allocationsTable.Select("ServiceTypeName IS NOT NULL AND ItinerarySaleID IN (" + saleIds + ")");

            // lock all of the exported sales
            foreach (DataRow row in allocationRows)
            {
                int saleId = Convert.ToInt32(row["ItinerarySaleID"]);
                ItinerarySet.ItinerarySaleRow saleRow =
                    itinerarySet.ItinerarySale.FindByItinerarySaleID(saleId);
                saleRow.IsLockedAccounting = true;
            }
        }

        private static void InitializeGridSummaries(InitializeLayoutEventArgs e, string column, string summaryColumn)
        {
            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter;
            band.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;

            if (!band.Summaries.Exists(summaryColumn))
            {
                SummarySettings summary = band.Summaries.Add(SummaryType.Sum, band.Columns[column]);
                summary.Key = summaryColumn;
                summary.DisplayFormat = "{0:#0.00}";
                summary.Appearance.TextHAlign = HAlign.Right;
                e.Layout.Override.GroupBySummaryDisplayStyle = GroupBySummaryDisplayStyle.SummaryCells;
            }
        }

        #region Events

        private void Accounting_Load(object sender, EventArgs e)
        {
            InitializeGrids();
        }

        private static void gridPurchases_InitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {
            InitializeGridSummaries(e, "PaymentAmount", "AmountTotal");
        }

        private static void gridAllocations_InitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                GridHelper.SetDefaultCellAppearance(c);
                c.CellActivation = Activation.NoEdit;

                if (c.Key == "IsLockedAccounting")
                {
                    c.Header.Caption = String.Empty;
                    c.Header.Appearance.Image = Resources.LockEdit;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.Width = 30;
                    c.MaxWidth = 30;
                    c.Header.VisiblePosition = 999;
                }
            }
        }

        private static void gridPayments_InitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {   
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                GridHelper.SetDefaultCellAppearance(c);
                c.CellActivation = Activation.NoEdit;

                if (c.Key == "IsLockedAccounting")
                {
                    c.Header.Caption = String.Empty;
                    c.Header.Appearance.Image = Resources.LockEdit;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.Width = 30;
                    c.MaxWidth = 30;
                    c.Header.VisiblePosition = 999;
                }
            }

            InitializeGridSummaries(e, "PaymentAmount", "AmountTotal");
        }

        private static void gridSales_InitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.Add("Amount");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "Amount")
                {
                    c.DataType = typeof(Decimal);
                    c.Format = "c";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "IsLockedAccounting")
                {
                    c.Hidden = true;
                }
            }

            e.Layout.Bands[0].Override.RowSelectors = DefaultableBoolean.False;
            InitializeGridSummaries(e, "Amount", "AmountTotal");
        }

        private void gridSales_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // calculate the total sales for the active sale row
            int saleId = (int)e.Row.Cells["ItinerarySaleID"].Value;
            decimal totalSales = 0;
            DataRow[] saleTotals = itinerarySet.ItinerarySaleAllocation.Select("ItinerarySaleID = " + saleId);
            foreach (DataRow row in saleTotals)
            {
                totalSales += row["Amount"] != DBNull.Value ? (decimal)row["Amount"] : 0;
            }
            e.Row.Cells["Amount"].Value = totalSales;
        }

        private void gridSales_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            OpenSalesEditor();
        }

        private void btnSaveLayout_Click(object sender, EventArgs e)
        {
            SaveGridLayouts();
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            ExportAllToCsv();
        }

        #endregion
    }
}
