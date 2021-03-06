using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Info.Services;
using TourWriter.Modules.ItineraryModule;
using TourWriter.Services;
using Resources = TourWriter.Properties.Resources;

namespace TourWriter.UserControls.Accounting
{
    public partial class Accounting : UserControl
    {
        private DataSet _cachedServiceTypes;
        private ItinerarySet _itinerarySet;
        public ItinerarySet ItinerarySet
        {
            set
            {
                _itinerarySet = value;

                var status = "";
                if (!_itinerarySet.Itinerary[0].IsItineraryStatusIDNull())
                {
                    var st = Cache.ToolSet.ItineraryStatus.Where(
                        x => x.ItineraryStatusID == _itinerarySet.Itinerary[0].ItineraryStatusID).FirstOrDefault();
                    if (st != null) status = ", Status: " + st.ItineraryStatusName;
                }
                pnlDates.Visible = false;
                lblHeading.Text = string.Format("Itinerary: {0}, ID: {1}{2}", 
                    _itinerarySet.Itinerary[0].ItineraryName, _itinerarySet.Itinerary[0].ItineraryID, status);

                AddToolStripButton(gridSales.ToolStrip, "Edit Sales", Resources.PageEdit, "Open the Sale Allocations editor", delegate { EditSales(); }, true, 3);
            }
        }
        private DataSet _purchasesDs, _salesDs;
        private bool _isItinerarySetDirty;
        #region grid display cols
        private readonly List<string> _defaultPurchaseColumns = new List<string>()
                           {
                               "IsLockedAccounting",
                               "ItineraryID",
                               "ItineraryName",
                               "PurchaseItemStartDate",
                               "SupplierName",
                               "PurchaseItemName",
                               "NetBaseTotal",
                               "GrossBaseTotal",
                               "CurrencyCode",
                               "NetTaxTypeCode",
                               "PurchaseLineID",
                               "NetAccountingCategoryCode",
                               "ServiceTypeName",
                               "PaymentDueDate",
                               "RequestStatusName",
                               "PurchaseItemID",
                               "ArriveDate",
                               "SaleDate",
                               "IsInvoiced",
                           };

        private readonly List<string> _defaultSalesColumns = new List<string>()
                           {
                               "SaleIsLockedAccounting",
                               "ItineraryID",
                               "ItineraryName",
                               "SaleDate",
                               "SaleAmount",
                               "SaleNet",
                               "ItineraryCurrencyCode",
                               "ItineraryGrossFinalTotal",
                               "ItinerarySaleID",
                               "ArriveDate",
                               "ItineraryBalanceDueDate",
                           };
        #endregion

        public Accounting()
        {
            InitializeComponent();
            if (App.IsInDesignMode) return;

            txtFrom.Value = DateTime.Now.AddMonths(-1).Date;
            txtTo.Value = DateTime.Now.Date;

            gridPurchases.InitializeLayoutEvent += GridInitializeLayoutEvent;
            gridPurchases.UltraGrid.InitializeRow += GridInitializeRow;
            gridPurchases.UltraGrid.DoubleClickRow += gridPurchases_DoubleClickRow;
            gridSales.InitializeLayoutEvent += GridInitializeLayoutEvent;
            gridSales.UltraGrid.InitializeRow += GridInitializeRow;
            gridSales.UltraGrid.DoubleClickRow += gridSales_DoubleClickRow;

            // add purchase grid buttons
            AddToolStripButton(gridPurchases.ToolStrip, "", Resources.CheckBox, "Select/unselect all grid rows", delegate { SelectAllRows(gridPurchases); }, false, 0);
            AddToolStripButton(gridPurchases.ToolStrip, "Export  ", Resources.Export, "Export selected Purchases data", delegate { ExportPurchases(); }, true, 0);
            AddToolStripButton(gridPurchases.ToolStrip, "Load  ", Resources.Load, "Load or refresh Purchases data", delegate { BindPurchasesGrid(); }, false, 0);
            
            // add sale grid buttons
            AddToolStripButton(gridSales.ToolStrip, "", Resources.CheckBox, "Select/unselect all grid rows", delegate { SelectAllRows(gridSales); }, false, 0);
            AddToolStripButton(gridSales.ToolStrip, "Export  ", Resources.Export, "Export selected Sales data", delegate { ExportSales(); }, true, 0);
            AddToolStripButton(gridSales.ToolStrip, "Load  ", Resources.Load, "Load or refresh Sales data", delegate { BindSalesGrid(); }, false, 0);

            // bind combo1
            var list = new List<string> {"Booking / Sale dates", "Itinerary dates"};
            comboBox1.DataSource = list;

            // bind request status
            var dict = new Dictionary<string, string> {{"", "any"}};
            foreach (var r in Cache.ToolSet.RequestStatus) dict.Add(r.RequestStatusID.ToString(), r.RequestStatusName);
            comboBox2.DataSource = new BindingSource(dict, null);
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";

            // bind itinerary status
            var dict2 = new Dictionary<string, string> { { "", "any" } };
            foreach (var r in Cache.ToolSet.ItineraryStatus) dict2.Add(r.ItineraryStatusID.ToString(), r.ItineraryStatusName);
            comboBox3.DataSource = new BindingSource(dict2, null);
            comboBox3.DisplayMember = "Value";
            comboBox3.ValueMember = "Key";
        }
        
        private void BindPurchasesGrid()
        {
            if (!HandleDirtyItinerarySet()) return;
            try
            {
                Cursor = Cursors.WaitCursor;
                gridPurchases.DataSource = null;
                _purchasesDs = null;
                LoadPurchasesData();
                gridPurchases.DataSource = _purchasesDs;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BindSalesGrid()
        {
            if (!HandleDirtyItinerarySet()) return;
            try
            {
                Cursor = Cursors.WaitCursor;
                gridSales.DataSource = null;
                _salesDs = null;
                LoadSalesData();
                gridSales.DataSource = _salesDs;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        
        private void LoadPurchasesData()
        {
            // get query columns, default + template cols
            var columnNames = new HashSet<string>(_defaultPurchaseColumns).
                Union(GetTemplateTagNames("Purchases").Where(x => x != "PurchaseStatus")); // also include additional columns from template file
            var cols = string.Join(",", columnNames.ToArray());
            
            // run query
            var sql = string.Format("select cast(1 as bit) IsSelected, {0} from PurchaseItemPaymentsDetail where {1}", cols, GetSqlFilter("Purchases"));
            _purchasesDs = DataSetHelper.FillDataSetFromSql(sql, 120);
        }
        
        private void LoadSalesData()
        {
            // get query columns, just default cols as no template for Sales (only for their Allocations)
            var cols = string.Join(",", _defaultSalesColumns.ToArray());
            
            // run query
            var sql = string.Format("select cast(1 as bit) IsSelected, {0} from ItinerarySaleDetail where {1}", cols, GetSqlFilter("Sales"));
            _salesDs = DataSetHelper.FillDataSetFromSql(sql, 120);
        }
        
        private void LoadServiceType()
        {
            var sql = string.Format(
                "select ServiceTypeID, Gross from ItineraryServiceTypeDetail where ItineraryID = {0}", _itinerarySet.Itinerary[0].ItineraryID);

            _cachedServiceTypes = null;
            _cachedServiceTypes = DataSetHelper.FillDataSetFromSql(sql, 120);
        }
        
        private void ExportPurchases()
        {
            if (!EnsurePermissions()) return;
            if (!HandleDirtyItinerarySet()) return;

            if (_purchasesDs == null) BindPurchasesGrid();

            if (_purchasesDs != null && CanExport(gridPurchases))
            {
                var template = GetTemplateFileName("Purchases");
                if (!File.Exists(template))
                {
                    App.ShowWarning("Purchases export template file not found: " + template);
                    return;
                }
                
                gridPurchases.UltraGrid.UpdateData();
                var data = _purchasesDs.Copy().Tables[0];
                App.PrepareDataTableForExport(data);
           
                // get visible and selected row ids
                var ids = gridPurchases.UltraGrid.Rows.Where(x => !x.IsFilteredOut && (bool)x.Cells["IsSelected"].Value).Select(x => (int)x.Cells["PurchaseItemID"].Value);
                var rows = data.AsEnumerable().Where(x => ids.Contains(x.Field<int>("PurchaseItemID")));
                var export = new ExportForm();
                export.ExportPurchases(rows, template, GetExportFileName("Purchases"));
                if (export.ShowDialog() == DialogResult.OK)
                    LockExportedPurchases(rows);
            }
        }

        private void ExportSales()
        {
            if (!EnsurePermissions()) return;
            if (!HandleDirtyItinerarySet()) return;

            if (_salesDs == null) BindSalesGrid();

            if (_salesDs != null &&CanExport(gridSales))
            {
                var template = GetTemplateFileName("Sales");
                if (!File.Exists(template))
                {
                    App.ShowWarning("Sales export template file not found: " + template);
                    return;
                }

                gridSales.UltraGrid.UpdateData();
                var data = _salesDs.Copy().Tables[0];
                App.PrepareDataTableForExport(data);
               
                // get visible and selected row ids
                var ids = gridSales.UltraGrid.Rows.Where(x => !x.IsFilteredOut && (bool)x.Cells["IsSelected"].Value).Select(x => (int)x.Cells["ItineraryID"].Value);
                var rows = data.AsEnumerable().Where(x => ids.Contains(x.Field<int>("ItineraryID")));
                var export = new ExportForm();
                export.ExportSales(rows, template, GetExportFileName("Sales"));
                if (export.ShowDialog() == DialogResult.OK)
                    LockExportedSales(rows);
            }
        }
        
        private void EditSales()
        {
            if (_itinerarySet == null) return;

            // load service types
            if (_cachedServiceTypes == null)
            {
                Cursor = Cursors.WaitCursor;
                try { LoadServiceType(); }
                finally { Cursor = Cursors.Default; }
            }

            // show allocations screen
            SaleAllocationForm allocationForm;
            if (gridSales.UltraGrid.ActiveRow != null)
            {
                var saleId = (int)gridSales.UltraGrid.ActiveRow.Cells["ItinerarySaleID"].Value;
                allocationForm = new SaleAllocationForm(_itinerarySet, _cachedServiceTypes, saleId);
            }
            else allocationForm = new SaleAllocationForm(_itinerarySet, _cachedServiceTypes);

            if (allocationForm.ShowDialog() == DialogResult.OK)
            {
                // save itinerary and reload
                if (_itinerarySet.ItinerarySale.GetChanges() != null ||
                    _itinerarySet.ItinerarySaleAllocation.GetChanges() != null)
                {
                    _isItinerarySetDirty = true;
                    BindSalesGrid();
                }
                // if not loaded, load now
                else if (_salesDs == null)
                    BindSalesGrid();
            }
        }

        private bool HandleDirtyItinerarySet()
        {
            if (_itinerarySet == null || !_isItinerarySetDirty) return true;

            if (App.AskYesNo("Itinerary needs to be saved, click YES to save now"))
            {
                var x = this;
                var y = x.ParentForm;
                (ParentForm as ItineraryMain).SaveChanges();
                _isItinerarySetDirty = false;
                return true;
            }
            return false;
        }

        private static string GetTemplateFileName(string accountingType)
        {
            var templateName = accountingType == "Purchases" ? "Accounting purchases" : "Accounting sales";
            var r = Cache.ToolSet.Template.Where(x => x.TemplateName == templateName).FirstOrDefault();
            if (r == null) return "";
            
            var fileName = ExternalFilesHelper.ConvertToAbsolutePath(r.FilePath);
            return fileName;
        }
        
        private string GetExportFileName(string typePrefix)
        {
            var ext = "csv";
            if (!Cache.ToolSet.AppSettings[0].IsAccountingSystemNull() && Cache.ToolSet.AppSettings[0].AccountingSystem.ToLower() == "myob")
                ext = "txt";

            return _itinerarySet == null
                       ? string.Format("{0}_{1}_{2}.{3}",
                                       typePrefix, txtFrom.DateTime.Date.ToString("yyyyMMdd"), txtTo.DateTime.Date.ToString("yyyyMMdd"), ext)
                       : string.Format("{0}_{1}_{2}.{3}",
                                        typePrefix, _itinerarySet.Itinerary[0].ItineraryName, DateTime.Now.ToString("yyyyMMdd_HHmmss"), ext);
        }
        
        internal static IEnumerable<string> GetTemplateTagNames(string accountingType)
        {
            var templateFile = GetTemplateFileName(accountingType);
            if (string.IsNullOrEmpty(templateFile)) return new HashSet<string>();

            // read template file
            StreamReader sr = null;
            var sb = new StringBuilder();
            try
            {
                string input;
                sr = File.OpenText(templateFile);
                while ((input = sr.ReadLine()) != null) sb.AppendLine(input);
            }
            catch(Exception ex)
            {
                App.ShowInfo("Opps, either your template file could not be found, or it is open in another program (Excel?).\r\n\r\nTemplate file is: " + templateFile);
            }
            finally
            {
                if (sr != null) sr.Close();
            }

            // extract tag names
            var mc = Regex.Matches(sb.ToString(), @"(?<=\[!).*?(?=\])");
            return new HashSet<string>(mc.OfType<Match>().Select(x => x.Value));
        }

        private string GetSqlFilter(string queryType)
        {
            string filter;

            if (_itinerarySet == null)
            {
                // dates
                var dateCol = queryType == "Purchases"
                              ? (comboBox1.SelectedIndex == 0 ? "ArriveDate" : "PurchaseItemStartDate") // purchases
                              : (comboBox1.SelectedIndex == 0 ? "ArriveDate" : "SaleDate");             // sales
                var from = txtFrom.DateTime.Date.ToString("yyyy.MM.dd 00:00:00");
                var to = txtTo.DateTime.Date.ToString("yyyy.MM.dd 23:59:59");
                filter = string.Format("{0} >= '{1}' and {0} <= '{2}'", dateCol, from, to);

                // status
                if (queryType == "Purchases")
                {
                    var requestStatus = comboBox2.SelectedValue == "" ? (int?)null : int.Parse(comboBox2.SelectedValue.ToString());
                    if (requestStatus.HasValue) filter += " and RequestStatusID = " + requestStatus;

                    var itineraryStatus = comboBox3.SelectedValue == "" ? (int?)null : int.Parse(comboBox3.SelectedValue.ToString());
                    if (itineraryStatus.HasValue) filter += " and ItineraryStatusID = " + itineraryStatus;
                }
            }
            else
            {
                // itinerary id
                filter = string.Format("ItineraryID = {0}", _itinerarySet.Itinerary[0].ItineraryID);
            }
            return filter;
        }

        private static bool CanExport(DataExtractGrid grid)
        {
            if (grid.UltraGrid.Rows.Count() == 0)
            {
                App.ShowInfo("No data loaded");
                return false;
            }
            if (grid.UltraGrid.Rows.Where(x => (bool)x.Cells["IsSelected"].Value == true).Count() == 0)
            {
                App.ShowInfo("No rows selected");
                return false;
            }
            return true;
        }
        
        private void GridInitializeLayoutEvent(object sender, InitializeLayoutEventArgs e)
        {
            foreach (var c in e.Layout.Bands[0].Columns)
            {
                c.CellActivation = Activation.NoEdit;
                c.PerformAutoResize(PerformAutoSizeType.AllRowsInBand);

                if (c.Key == "IsSelected")
                {
                    c.Header.Caption = "";
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "IsLockedAccounting" || c.Key == "SaleIsLockedAccounting")
                {
                    c.Header.Caption = "";
                    c.Header.Appearance.Image = Resources.LockEdit;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.Width = 30;
                    c.MaxWidth = 30;
                }

                if (c.Key == "ItineraryID" || c.Key == "ItineraryName") 
                    c.Hidden = _itinerarySet != null;

                if (c.Key != "IsSelected") 
                    c.CellAppearance.ForeColor = Color.Gray;

                // formatting
                if (c.DataType == typeof(int))
                {
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                if (c.DataType == typeof(decimal) || c.DataType == typeof(double))
                {
                    c.Format = "#0.00";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                if (c.DataType == typeof(DateTime))
                {
                    c.Format = App.GetLocalShortDateFormat();
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
            }
            e.Layout.Bands[0].Override.RowSelectors = DefaultableBoolean.False;
        }
        
        private void GridInitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("IsLockedAccounting") && e.Row.Cells["IsLockedAccounting"].Value == DBNull.Value)
                e.Row.Cells["IsLockedAccounting"].Value = false;
            if (e.Row.Cells.Exists("SaleIsLockedAccounting") && e.Row.Cells["SaleIsLockedAccounting"].Value == DBNull.Value)
                e.Row.Cells["SaleIsLockedAccounting"].Value = false;
        }

        private void gridSales_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            EditSales();
        }
        
        private void gridPurchases_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow) ||
                e.Row.GetType() == typeof(UltraGridGroupByRow))
                return;            

            // Sets the ParentFolderID to 0 for now, needs to be update if it causes an issue
            var info = new NavigationTreeItemInfo(
                (int)e.Row.Cells["ItineraryID"].Value,
                (string)e.Row.Cells["ItineraryName"].Value,
                NavigationTreeItemInfo.ItemTypes.Itinerary,
                0, true);

            var node = App.MainForm.BuildMenuNode(info);
            App.MainForm.Load_ItineraryForm(node);
        }

        public ToolStripButton AddToolStripButton(ToolStrip toolstrip, string text, Image image, string toolTipText, EventHandler onClick, bool insertSeperator, int position)
        {
            var button = new ToolStripButton(text, image, onClick);
            button.ImageScaling = ToolStripItemImageScaling.None;
            button.ToolTipText = toolTipText;
            if (insertSeperator) toolstrip.Items.Insert(position, new ToolStripSeparator());
            toolstrip.Items.Insert(position, button);
            return button;
        }
        
        private bool _isSelectAll = true;
        public void SelectAllRows(DataExtractGrid dataGrid)
        {
            _isSelectAll = !_isSelectAll;
            var grid = dataGrid.UltraGrid;

            foreach (var row in grid.Rows)
            {
                if (!row.Cells.Exists("IsSelected")) return;
                if (row.IsGroupByRow) continue;
                row.Cells["IsSelected"].Value = _isSelectAll;
            }
        }

        private bool EnsurePermissions()
        {
            if (!AppPermissions.UserHasPermission(AppPermissions.Permissions.AccountingEdit))
            {
                App.ShowError(App.GetResourceString("ShowPermissionDenied"));
                return false;
            }
            return true;
        }

        internal void LockExportedPurchases(IEnumerable<DataRow> exportRows)
        {
            var ids = new List<int>();
            foreach (var row in exportRows)
            {
                var id = Convert.ToInt32(row["PurchaseItemID"]);
                ids.Add(id);
                row["IsLockedAccounting"] = true;
                if (_itinerarySet != null)
                {
                    var item = _itinerarySet.PurchaseItem.FindByPurchaseItemID(id);
                    if (item != null) item.IsLockedAccounting = true;
                }
            }

            // save
            if (_itinerarySet == null)
            {
                DatabaseHelper.ExecuteScalar(string.Format(
                    "update PurchaseItem set IsLockedAccounting = 1 where PurchaseItemID in ({0})",
                    string.Join(",", ids.Select(x => x.ToString()).ToArray())));
            }
            else _isItinerarySetDirty = true;
        }

        private void LockExportedSales(IEnumerable<DataRow> exportRows)
        {
            var ids = new List<int>();
            foreach (var row in exportRows)
            {
                var id = Convert.ToInt32(row["ItinerarySaleID"]);
                ids.Add(id);
                row["SaleIsLockedAccounting"] = true;
                if (_itinerarySet != null)
                {
                    var sale = _itinerarySet.ItinerarySale.FindByItinerarySaleID(id);
                    if (sale != null) sale.IsLockedAccounting = true;
                }
            }

            // save
            if (_itinerarySet == null)
            {
                DatabaseHelper.ExecuteScalar(string.Format(
                    "update ItinerarySale set IsLockedAccounting = 1 where ItinerarySaleID in ({0})",
                    string.Join(",", ids.Select(x => x.ToString()).ToArray())));
            }
            else _isItinerarySetDirty = true;
        }
    }
}