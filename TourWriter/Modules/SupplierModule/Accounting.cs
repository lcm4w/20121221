using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Info.Services;
using TourWriter.Services;

namespace TourWriter.Modules.SupplierModule
{
    public partial class Accounting : UserControl
    {
        private const string GridColumnRegex = "(?<=Key id=\"ref-\\d*\">){0}(?=</Key>)";

        private readonly string gridLayoutsPath;
        private readonly string purchasesGridLayoutFileName;
        private readonly Dictionary<string, string> GridColumnRenames = new Dictionary<string, string>();

        #region Properties

        public SupplierSet SupplierSet { get; set; }

        internal bool RefreshRequired
        {
            get
            {
                return (gridPurchases.DataSource == null);
            }
            set
            {
                if (value)
                {
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
        /// Enables/disables the controls, and hides/shows the refresh message
        /// </summary>
        internal bool EnableControls
        {
            set
            {
                gridPurchases.Enabled = value;
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

            purchasesGridLayoutFileName = gridLayoutsPath + @"PurchasesGridLayout.xml";
        }

        /// <summary>
        /// Manages refreshing data and useability of controls based if 
        /// underlying data has changed
        /// </summary>
        public void RefreshDataAndControls()
        {
            Application.DoEvents();

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                LoadData();

                EnableControls = true;
                LoadGridLayouts();

                // set all the rows to selected by default
                gridPurchases.SelectAllRows();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void InitializeGrids()
        {
            gridPurchases.InitializeLayoutEvent += gridPurchases_InitializeLayoutEvent;
        }

        private void LoadData()
        {
            var data = new AccountingDataLoad(SupplierSet.Supplier[0].SupplierID);
            data.LoadDataAsync();

            App.PrepareDataTableForExport(data.PurchasesDataSet.Tables[0]);

            gridPurchases.DataSource = data.PurchasesDataSet;
        }

        private void LoadGridLayouts()
        {
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

            string supplierName = SupplierSet.Supplier[0].SupplierName;

            var export = new AccountingExportForm();
            export.EnableSales = false;
            export.PurchasesFilename = string.Format("{0}-Purchases.txt", supplierName);
            export.SuppliersFilename = string.Format("{0}-Suppliers.txt", supplierName);

            if (export.ShowDialog() == DialogResult.OK)
            {
                if (export.IncludePurchases)
                {
                    AccountingExport.ExportPurchasesToCsv(gridPurchases, export.PurchasesFilename);
                    LockExportedBookings();
                }

                if (export.IncludeSuppliers)
                    AccountingExport.ExportSuppliersToCsv(gridPurchases, SupplierSet.Supplier, export.SuppliersFilename);
            }
        }

        private void LockExportedBookings()
        {
            // lock all of the exported booking items
            string purchaseItemIds = String.Empty;

            var rowList = GridHelper.GetDataRowsList(gridPurchases.UltraGrid);
            
            foreach (var row in rowList)
            {
                bool isSelected = Convert.ToBoolean(row.Cells["IsSelected"].Value);
                if (!isSelected)
                    continue;

                if (!String.IsNullOrEmpty(purchaseItemIds))
                    purchaseItemIds += ",";

                purchaseItemIds += row.Cells["PurchaseItemID"].Value;
            }

            // return if there are no rows to update
            if (String.IsNullOrEmpty(purchaseItemIds))
                return;

            string updateSql = "UPDATE PurchaseItem SET IsLockedAccounting = 'TRUE' WHERE PurchaseItemID IN (" + purchaseItemIds + ")";

            DatabaseHelper.ExecuteScalar(updateSql);
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
