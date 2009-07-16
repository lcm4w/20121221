using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Info.Services;
using TourWriter.Services;

namespace TourWriter.Modules.ItineraryModule
{
    public class ExportToExcel
    {
        private readonly ItinerarySet itinerarySetOrig;
        private ItinerarySet itinerarySetCopy;
        private DataTable itineraryDetail;
        private Excel.Application excelApp;
        private Excel.Workbook excelBook;
        private Excel.Worksheet excelSheet;

        public ExportToExcel(ItinerarySet itinerarySet)
        {
            itinerarySetOrig = itinerarySet;
        }

        /// <summary>
        /// Creates the excel sheet and saves it to the specified file.
        /// </summary>
        public void ExportItinerary(string templateFile, string saveFile)
        {
            if (itinerarySetCopy == null)
            {
                itinerarySetCopy = (ItinerarySet)itinerarySetOrig.Copy();
                ExtendItineraryTable();
                ExtendPurchaseItemTable();
            }

            // initialize excel
            excelApp = new Excel.Application();
            excelBook = excelApp.Workbooks.Open(templateFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing,
                                    ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);
            excelSheet = (Excel.Worksheet)excelBook.Worksheets[1];

            // fill in all the itinerary info
            ReplaceItineraryTags();
            ReplaceBookingTags();

            excelBook.SaveAs(saveFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, Excel.XlSaveAsAccessMode.xlShared,
                ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);

            Cleanup();

            Process.Start(saveFile);
        }

        /// <summary>
        /// Add additional columns to the itinerary table, for the tag replacer to use.
        /// </summary>
        private void ExtendItineraryTable()
        {
            // add additional columns
            itinerarySetCopy.Itinerary.Columns.Add("TotalNetBase");
            itinerarySetCopy.Itinerary.Columns.Add("TotalGrossBase");
            itinerarySetCopy.Itinerary.Columns.Add("NetMarkup");
            itinerarySetCopy.Itinerary.Columns.Add("Subtotal1");
            itinerarySetCopy.Itinerary.Columns.Add("Subtotal2");
            itinerarySetCopy.Itinerary.Columns.Add("FinalOverride");
            itinerarySetCopy.Itinerary.Columns.Add("Sell");
            itinerarySetCopy.Itinerary.Columns.Add("TotalMarkup");
            itinerarySetCopy.Itinerary.Columns.Add("Commission");
            itinerarySetCopy.Itinerary.Columns.Add("ArriveCity");
            itinerarySetCopy.Itinerary.Columns.Add("DepartCity");
            itinerarySetCopy.Itinerary.Columns.Add("Length");
            itinerarySetCopy.Itinerary.Columns.Add("Origin");
            itinerarySetCopy.Itinerary.Columns.Add("Agent");
            itinerarySetCopy.Itinerary.Columns.Add("NoteToSupplier");

            itinerarySetCopy.Itinerary.Columns["GrossMarkup"].ColumnName = "GrossMarkup_OLD";
            itinerarySetCopy.Itinerary.Columns.Add("GrossMarkup");

            // fill the columns with data
            DataRow row = itinerarySetCopy.Itinerary[0];

            decimal totalNet = itinerarySetCopy.GetNetBasePrice();
            row["TotalNetBase"] = totalNet.ToString("c");

            decimal totalGross = itinerarySetCopy.GetGrossBasePrice();
            row["TotalGrossBase"] = totalGross.ToString("c");

            decimal sell = itinerarySetCopy.GetGrossFinalPrice();
            row["Sell"] = sell.ToString("c");

            decimal markup = Common.CalcMarkupByNetGross(totalNet, sell);
            row["TotalMarkup"] = (markup / 100).ToString("p");

            string commission = String.Format("{0:c} ({1:p})", (sell - totalNet), (sell != 0) ? (((sell - totalNet) / sell)) : 0);
            row["Commission"] = commission;

            string length = "0 days";
            if (row["DepartDate"] != DBNull.Value)
            {
                length = ((DateTime)row["DepartDate"] - (DateTime)row["ArriveDate"]).Days + " days";
            }
            row["Length"] = length;

            // calculate gross overrides
            decimal gross1;
            decimal gross2;

            if (row["NetMargin"] != DBNull.Value || itinerarySetCopy.ItineraryMarginOverride.Rows.Count > 0)
                gross1 = itinerarySetCopy.GetNetBasePrice() * (1 + itinerarySetCopy.GetNetMarkup() / 100);
            else
                gross1 = itinerarySetCopy.GetGrossBasePrice();

            if (!itinerarySetCopy.Itinerary[0].IsGrossMarkupNull())
                gross2 = gross1 * (1 + itinerarySetCopy.Itinerary[0].GrossMarkup / 100);
            else
                gross2 = gross1;

            row["Subtotal1"] = gross1;
            row["Subtotal2"] = gross2;

            // markup overrides
            row["NetMarkup"] = (itinerarySetCopy.GetNetMarkup() / 100).ToString("p");

            if (row["GrossMarkup_OLD"] != DBNull.Value)
                row["GrossMarkup"] = ((decimal)row["GrossMarkup_OLD"] / 100).ToString("p");

            // final override
            row["FinalOverride"] = (!itinerarySetCopy.Itinerary[0].IsGrossOverrideNull())
                                   ? itinerarySetCopy.Itinerary[0].GrossOverride.ToString("c")
                                   : String.Empty;
            row["Origin"] = (!itinerarySetCopy.Itinerary[0].IsCountryIDNull())
                             ? Cache.ToolSet.Country.FindByCountryID(itinerarySetCopy.Itinerary[0].CountryID).CountryName
                             : String.Empty;
            row["ArriveCity"] = (!itinerarySetCopy.Itinerary[0].IsArriveCityIDNull())
                                ? Cache.ToolSet.City.FindByCityID(itinerarySetCopy.Itinerary[0].ArriveCityID).CityName
                                : String.Empty;
            row["DepartCity"] = (!itinerarySetCopy.Itinerary[0].IsDepartCityIDNull())
                                ? Cache.ToolSet.City.FindByCityID(itinerarySetCopy.Itinerary[0].DepartCityID).CityName
                                : String.Empty;
            row["Agent"] = Cache.AgentSet.Agent.FindByAgentID(itinerarySetCopy.Itinerary[0].AgentID).AgentName;

            if (itinerarySetCopy.ItineraryGroup.Count > 0)
            {
                row["NoteToSupplier"] = (!itinerarySetCopy.ItineraryGroup[0].IsNoteToSupplierNull())
                                        ? itinerarySetCopy.ItineraryGroup[0].NoteToSupplier : String.Empty;
            }

            App.PrepareDataTableForExport(itinerarySetCopy.Itinerary);
        }

        /// <summary>
        /// Add additional columns to the purchaseitem table, for the tag replacer to use.
        /// </summary>
        private void ExtendPurchaseItemTable()
        {
            // add additional columns
            itinerarySetCopy.PurchaseItem.Columns.Add("BookingName");
            itinerarySetCopy.PurchaseItem.Columns.Add("BookingNote");
            itinerarySetCopy.PurchaseItem.Columns.Add("SupplierCity");

            // rename existing columns
            itinerarySetCopy.PurchaseItem.Columns["PurchaseItemName"].ColumnName = "Description";

            // rename old columns and replace with new columns
            itinerarySetCopy.PurchaseItem.Columns["Net"].ColumnName = "_Net";
            itinerarySetCopy.PurchaseItem.Columns.Add("Net");
            itinerarySetCopy.PurchaseItem.Columns["Gross"].ColumnName = "_Gross";
            itinerarySetCopy.PurchaseItem.Columns.Add("Gross");
            itinerarySetCopy.PurchaseItem.Columns["StartTime"].ColumnName = "_StartTime";
            itinerarySetCopy.PurchaseItem.Columns.Add("StartTime");
            itinerarySetCopy.PurchaseItem.Columns["EndTime"].ColumnName = "_EndTime";
            itinerarySetCopy.PurchaseItem.Columns.Add("EndTime");

            // fill the new columns with data
            foreach (ItinerarySet.PurchaseItemRow row in itinerarySetCopy.PurchaseItem)
            {
                row["BookingName"] = row.PurchaseLineRow.PurchaseLineName;
                row["BookingNote"] = row.PurchaseLineRow.NoteToSupplier;
                row["Net"] = row.NetTotalConverted.ToString("c");
                row["Gross"] = row.GrossTotalConverted.ToString("c");

                if (row["_StartTime"] != DBNull.Value)
                    row["StartTime"] = ((DateTime)row["_StartTime"]).TimeOfDay.ToString();

                if (row["_EndTime"] != DBNull.Value)
                    row["EndTime"] = ((DateTime)row["_EndTime"]).TimeOfDay.ToString();

                // get the supplier city
                int supplierId = row.GetSupplierId();
                var supplier = itinerarySetCopy.SupplierLookup.FindBySupplierID(supplierId);
                string supplierCity = (!supplier.IsCityIDNull()) ? Cache.ToolSet.City.FindByCityID(supplier.CityID).CityName : String.Empty;
                row["SupplierCity"] = supplierCity;
            }

            App.PrepareDataTableForExport(itinerarySetCopy.PurchaseItem);

            // add date/time columns for sorting purposes
            itinerarySetCopy.PurchaseItem.Columns.Add("SortDate", typeof (DateTime));
            itinerarySetCopy.PurchaseItem.Columns.Add("SortTime", typeof (TimeSpan));

            foreach (DataRow row in itinerarySetCopy.PurchaseItem)
            {
                row["SortDate"] = DateTime.Parse(row["StartDate"].ToString());

                if (!String.IsNullOrEmpty(row["StartTime"].ToString()))
                {
                    row["SortTime"] = TimeSpan.Parse(row["StartTime"].ToString());
                }
            }
        }

        /// <summary>
        /// Replaces all the itinerary tags in the excel sheet.
        /// </summary>
        private void ReplaceItineraryTags()
        {
            foreach (DataColumn column in itinerarySetCopy.Itinerary.Columns)
            {
                string tagName = String.Format("[!{0}]", column.ColumnName);
                object value = itinerarySetCopy.Itinerary[0][column];
                if (value == DBNull.Value)
                    value = String.Empty;

                ExcelHelper.ReplaceTag(tagName, value, excelSheet.Cells);
            }
        }

        /// <summary>
        /// Replaces all the booking tags in the excel sheet.
        /// </summary>
        private void ReplaceBookingTags()
        {
            while (true)
            {
                // find the begin and end booking tags, and perform operations ONLY on the cells between these tags
                Excel.Range beginRange = excelSheet.Cells.Find("[!BeginBookings]", ExcelHelper.Missing,
                                                               Excel.XlFindLookIn.xlValues,
                                                               Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows,
                                                               Excel.XlSearchDirection.xlNext, false,
                                                               ExcelHelper.Missing, ExcelHelper.Missing);
                if (beginRange == null)
                    break;

                Excel.Range endRange = excelSheet.Cells.Find("[!EndBookings]", beginRange, Excel.XlFindLookIn.xlValues,
                                                             Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows,
                                                             Excel.XlSearchDirection.xlNext, false, ExcelHelper.Missing,
                                                             ExcelHelper.Missing);
                if (endRange == null)
                    break;

                string beginCell = ExcelHelper.GetCellPosition(beginRange.Row + 1, beginRange.Column);
                string endCell = ExcelHelper.GetCellPosition(endRange.Row - 1, endRange.Column);

                Excel.Range tagRange = excelSheet.get_Range(beginCell, endCell);

                // sort the purchase item table
                var dataView = new DataView(itinerarySetCopy.PurchaseItem);
                dataView.Sort = "SortDate ASC, SortTime ASC";

                for (int i = 0; i < dataView.Count; i++)
                {
                    var purchaseItem = (ItinerarySet.PurchaseItemRow) dataView[i].Row;

                    // ignore deleted rows
                    if (purchaseItem.RowState == DataRowState.Deleted)
                        continue;

                    Excel.Range replaceRange;

                    if (i < (dataView.Count - 1))
                    {
                        tagRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, ExcelHelper.Missing);
                        string pasteCell1 = ExcelHelper.GetCellPosition((tagRange.Row - tagRange.Rows.Count),
                                                                        beginRange.Column);
                        string pasteCell2 = ExcelHelper.GetCellPosition(tagRange.Row - 1, endRange.Column);
                        Excel.Range pasteRange = excelSheet.get_Range(pasteCell1, pasteCell2);

                        tagRange.Copy(ExcelHelper.Missing);
                        pasteRange.PasteSpecial(Excel.XlPasteType.xlPasteAll,
                                                Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone,
                                                ExcelHelper.Missing, ExcelHelper.Missing);
                        replaceRange = pasteRange;
                    }
                    else
                    {
                        replaceRange = tagRange;
                    }

                    // replace tags
                    foreach (DataColumn column in itinerarySetCopy.PurchaseItem.Columns)
                    {
                        string tagName = String.Format("[!{0}]", column.ColumnName);
                        object value = purchaseItem[column];
                        if (value == DBNull.Value)
                            value = String.Empty;

                        ExcelHelper.ReplaceTag(tagName, value, replaceRange);
                    }
                }

                // delete the begin/end tags properly
                Excel.Range deleteRange1 = excelSheet.get_Range(
                    ExcelHelper.GetCellPosition(beginRange.Row, beginRange.Column),
                    ExcelHelper.GetCellPosition(beginRange.Row, endRange.Column));

                Excel.Range deleteRange2 = excelSheet.get_Range(
                    ExcelHelper.GetCellPosition(endRange.Row, beginRange.Column),
                    ExcelHelper.GetCellPosition(endRange.Row, endRange.Column));

                deleteRange1.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                deleteRange2.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
            }
        }

        /// <summary>
        /// Closes the excel app and releases all the excel objects.
        /// </summary>
        public void Cleanup()
        {
            excelBook.Close(ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);
            excelApp.Quit();

            Marshal.ReleaseComObject(excelSheet);
            Marshal.ReleaseComObject(excelBook);
            Marshal.ReleaseComObject(excelApp);

            excelSheet = null;
            excelBook = null;
            excelApp = null;
            itinerarySetCopy = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #region Financial

        /// <summary>
        /// Creates the excel sheet and saves it to the specified file.
        /// </summary>
        public void ExportFinancial(string templateFile, string saveFile)
        {
            LoadFinancialData();

            // initialize excel
            excelApp = new Excel.Application();
            excelBook = excelApp.Workbooks.Open(templateFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing,
                                    ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);
            excelSheet = (Excel.Worksheet)excelBook.Worksheets["Sheet1"];

            // fill in all the financial info
            ReplaceFinancialTags();

            excelBook.SaveAs(saveFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, Excel.XlSaveAsAccessMode.xlShared,
                ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);

            Cleanup();

            Process.Start(saveFile);
        }

        /// <summary>
        /// Loads data from the ItineraryDetail view.
        /// </summary>
        private void LoadFinancialData()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                string sql = "SELECT * FROM ItineraryDetail WHERE ItineraryID = '" + itinerarySetOrig.Itinerary[0].ItineraryID + "'";
                var ds = DataSetHelper.FillDataset(new DataSet(), sql);
                itineraryDetail = ds.Tables[0];
                App.PrepareDataTableForExport(itineraryDetail);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Replaces all the financial tags in the excel sheet.
        /// </summary>
        private void ReplaceFinancialTags()
        {
            ExcelHelper.ReplaceTag("[!TodayDate]", DateTime.Today.ToShortDateString(), excelSheet.Cells);

            foreach (DataColumn column in itineraryDetail.Columns)
            {
                string tagName = String.Format("[!{0}]", column.ColumnName);
                object value = itineraryDetail.Rows[0][column];
                if (value == DBNull.Value)
                    value = String.Empty;

                ExcelHelper.ReplaceTag(tagName, value, excelSheet.Cells);
            }
        }

        #endregion
    }
}
