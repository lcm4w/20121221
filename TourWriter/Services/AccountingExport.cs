using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.UserControls;
using System.Linq;

namespace TourWriter.Services
{
    public class AccountingExport
    {
        public static void ExportPurchasesToCsv(AccountingGrid gridPurchases, string exportFileName)
        {
            if (!ValidatePurchases(gridPurchases))
                return;

            string templateFileName = GetTemplateFileName("Accounting purchases");
            if (templateFileName == null)
                return;

            DataTable purchasesTable = gridPurchases.GetDataRowsTable();
            purchasesTable.Columns["Status"].ColumnName = "PurchaseStatus";

            // remove rows that aren't selected
            for (int i = purchasesTable.Rows.Count - 1; i >= 0; i--)
            {
                bool isSelected = Convert.ToBoolean(purchasesTable.Rows[i]["IsSelected"]);
                if (!isSelected)
                {
                    purchasesTable.Rows[i].Delete();
                }
            }

            // do the export
            ExportToCsv(purchasesTable, templateFileName, exportFileName, "PurchaseLineID");
        }

        public static void ExportSuppliersToCsv(AccountingGrid gridPurchases, DataTable supplierTable, string exportFileName)
        {
            string templateFileName = GetTemplateFileName("Accounting suppliers");
            if (templateFileName != null)
            {
                // do the export
                ExportToCsv(BuildSupplierTable(gridPurchases, supplierTable), templateFileName, exportFileName);
            }
        }

        public static void ExportSalesToCsv(AccountingGrid gridSales, AccountingGrid gridAllocations, string exportFileName)
        {
            if (!ValidateSales(gridSales, gridAllocations))
                return;

            string templateFileName = GetTemplateFileName("Accounting sales");
            if (templateFileName != null)
            {
                DataTable salesTable = gridSales.GetDataRowsTable();
                DataTable allocationsTable = gridAllocations.GetDataRowsTable();

                if (allocationsTable.Rows.Count == 0)
                    return;
                
                // remove unselected rows from allocations table
                foreach (var parentRow in from parentRow in salesTable.Rows.Cast<DataRow>()
                                          let unselected = !Convert.ToBoolean(parentRow["IsSelected"].ToString().ToLower())
                                          where unselected select parentRow)
                {
                    for (var i = allocationsTable.Rows.Count; i > 0; i--)
                    {
                        var childRow = allocationsTable.Rows[i-1];
                        var match = Convert.ToInt32(parentRow["ItinerarySaleID"].ToString()) == Convert.ToInt32(childRow["ItinerarySaleID"].ToString());
                        if (match) childRow.Delete();
                    }
                }

                // remove rows that have no service type allocation
                DataRow[] emptyRows = allocationsTable.Select("ServiceTypeName IS NULL");
                foreach (DataRow row in emptyRows)
                    row.Delete();

                // Copy the sale status
                allocationsTable.Columns.Add("SaleStatus", typeof(string));
                foreach (DataRow row in allocationsTable.Rows)
                {
                    int saleId = Convert.ToInt32(row["ItinerarySaleID"]);
                    var saleRows = salesTable.Select("ItinerarySaleID = '" + saleId + "'");
                    if (saleRows.Length > 0)
                        row["SaleStatus"] = saleRows[0]["Status"];
                }

                // do the export
                ExportToCsv(allocationsTable, templateFileName, exportFileName);
            }
        }

        public static void ExportClientsToCsv(DataTable itineraryTable, string exportFileName)
        {
            string templateFileName = GetTemplateFileName("Accounting clients");
            if (templateFileName != null)
            {
                // do the export
                ExportToCsv(itineraryTable, templateFileName, exportFileName);
            }
        }

        public static void ExportPaymentsToCsv(AccountingGrid gridPayments, string exportFileName)
        {
            string templateFileName = GetTemplateFileName("Accounting payments");
            if (templateFileName != null)
            {
                DataTable paymentsTable = gridPayments.GetDataRowsTable();

                // remove rows that aren't selected
                for (int i = paymentsTable.Rows.Count - 1; i >= 0; i--)
                {
                    bool isSelected = Convert.ToBoolean(paymentsTable.Rows[i]["IsSelected"]);
                    if (!isSelected)
                    {
                        paymentsTable.Rows[i].Delete();
                    }
                }

                var accounting = new AccountingService(paymentsTable, templateFileName);
                accounting.ExportToCsv(exportFileName);

                // do the export
                ExportToCsv(paymentsTable, templateFileName, exportFileName);
            }
        }

        /// <summary>
        /// Exports a data table to csv using the AccountingService class, with a grouping column.
        /// </summary>
        private static void ExportToCsv(DataTable dataTable, string templateFileName, string exportFileName, string groupColumn)
        {
            if (dataTable.Rows.Count == 0)
                return;

            AccountingService accounting;
            if (groupColumn != null)
                accounting = new AccountingService(dataTable, templateFileName, groupColumn);
            else
                accounting = new AccountingService(dataTable, templateFileName);

            accounting.ExportToCsv(exportFileName);

            // notify the user of any invalid tags
            if (accounting.InvalidTags.Count > 0)
            {
                string msg = String.Format("Template {0} contains invalid tags:\r\n", templateFileName);
                foreach (string tag in accounting.InvalidTags)
                {
                    msg += "[!" + tag + "]\r\n";
                }
                App.ShowWarning(msg);
            }
        }

        /// <summary>
        /// Exports a data table to csv using the AccountingService class.
        /// </summary>
        private static void ExportToCsv(DataTable dataTable, string templateFileName, string exportFileName)
        {
            ExportToCsv(dataTable, templateFileName, exportFileName, null);
        }

        private static bool ValidatePurchases(AccountingGrid gridPurchases)
        {
            string msgList = String.Empty;
            string zeroNetPriceMsg = String.Format("- At least one payment contains a {0}0.00 amount.\r\n",
                                                   CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol);

            foreach (UltraGridRow row in gridPurchases.UltraGrid.Rows)
            {
                if (row.IsGroupByRow)
                    continue;

                if (!(bool)row.Cells["IsSelected"].Value)
                    continue;

                if (Convert.ToDecimal(row.Cells["PaymentAmount"].Value) == 0)
                {
                    if (!msgList.Contains(zeroNetPriceMsg))
                        msgList += zeroNetPriceMsg;
                }
            }

            if (msgList.Length > 0)
            {
                return App.AskYesNo(
                    String.Format(
                        "Warning, the selected records contain the following potential issues:\r\n{0}\r\nDo you want to continue?",
                        msgList));
            }

            return true;
        }

        private static bool ValidateSales(AccountingGrid gridSales, AccountingGrid gridAllocations)
        {
            const string missingCodeMsg = "- At least one service type is missing an accounting code.\r\n";
            string msgList = String.Empty;
            DataTable allocationsTable = gridAllocations.GetDataRowsTable();

            foreach (UltraGridRow row in gridSales.UltraGrid.Rows)
            {
                if (row.IsGroupByRow || !(bool)row.Cells["IsSelected"].Value) continue;

                var saleId = (int) row.Cells["ItinerarySaleID"].Value;
                foreach (DataRow r in allocationsTable.Rows)
                {
                    if (int.Parse(r["ItinerarySaleID"].ToString()) != saleId) continue;

                    if (r["AccountingCategoryCode"] == DBNull.Value && !msgList.Contains(missingCodeMsg))
                        msgList += missingCodeMsg;
                    break;
                }
            }

            if (msgList.Length > 0)
            {
                return App.AskYesNo(
                    String.Format(
                        "Warning, the selected records contain the following potential issues:\r\n{0}\r\n\r\nDo you want to continue?",
                        msgList));
            }

            return true;
        }

        private static string GetTemplateFileName(string templateName)
        {
            DataRow[] templateRows = Cache.ToolSet.Template.Select(String.Format("TemplateName = '{0}'", templateName));
            string templateFileName = templateRows.Length > 0 ? templateRows[0]["FilePath"].ToString() : "";

            if (!string.IsNullOrEmpty(templateFileName))
            {
                templateFileName = ExternalFilesHelper.ConvertToAbsolutePath(templateFileName);
                if (File.Exists(templateFileName))
                    return templateFileName;
            }
            App.ShowError(string.Format("Could not find {0} template at: {1}", templateName, templateFileName));
            return null;
        }

        private static DataTable BuildSupplierTable(AccountingGrid gridPurchases, DataTable supplierTable)
        {
            // only export the suppliers that are related to the selected purchases
            var supplierIdList = new List<int>();
            foreach (UltraGridRow gridRow in gridPurchases.UltraGrid.Rows)
            {
                if (gridRow.IsGroupByRow)
                    continue;

                if ((bool)gridRow.Cells["IsSelected"].Value)
                    supplierIdList.Add((int)gridRow.Cells["SupplierID"].Value);
            }

            DataTable newTable = supplierTable.Copy();
            newTable.Columns.Add("CityName");
            newTable.Columns.Add("StateName");
            newTable.Columns.Add("CountryName");

            for (int i = newTable.Rows.Count - 1; i >= 0; i--)
            {
                // remove suppliers that aren't in the list
                var dataRow = newTable.Rows[i];
                if (!supplierIdList.Contains((int)dataRow["SupplierID"]))
                {
                    dataRow.Delete();
                    continue;
                }

                if (dataRow["CityID"] != DBNull.Value)
                    dataRow["CityName"] = Cache.ToolSet.City.FindByCityID((int)dataRow["CityID"]).CityName;

                if (dataRow["StateID"] != DBNull.Value)
                    dataRow["StateName"] = Cache.ToolSet.State.FindByStateID((int)dataRow["StateID"]).StateName;

                if (dataRow["CountryID"] != DBNull.Value)
                    dataRow["CountryName"] = Cache.ToolSet.Country.FindByCountryID((int)dataRow["CountryID"]).CountryName;
            }

            return newTable;
        }
    }
}
