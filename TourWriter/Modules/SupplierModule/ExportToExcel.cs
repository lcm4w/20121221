using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.SupplierModule
{
    public class ExportToExcel
    {
        private const char BlackSquare = '\u25A0';
        private const char WhiteSquare = '\u25A1';
        
        private readonly SupplierSet supplierSet;
        private Excel.Application excelApp;
        private Excel.Workbook excelBook;
        private Excel.Worksheet excelSheet;
        
        public ExportToExcel(SupplierSet supplierSet)
        {
            this.supplierSet = supplierSet;
        }

        /// <summary>
        /// Creates the excel sheet and saves it to the specified file.
        /// </summary>
        public void Export(string templateFile, string saveFile)
        {
            // initialize excel
            excelApp = new Excel.Application();
            excelBook = excelApp.Workbooks.Open(templateFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing,
                                    ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);
            excelSheet = (Excel.Worksheet)excelBook.Worksheets["Sheet1"];

            // fill in all the supplier info
            FillServiceInfo();
            FillSupplierConfigs();
            FillSupplierInfo();

            excelBook.SaveAs(saveFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, Excel.XlSaveAsAccessMode.xlShared,
                ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);

            Cleanup();

            Process.Start(saveFile);
        }

        /// <summary>
        /// Replaces all the supplier tags in the excel sheet.
        /// </summary>
        private void FillSupplierInfo()
        {
            foreach (DataColumn column in supplierSet.Supplier.Columns)
            {
                string tagName = String.Format("[!{0}]", column.ColumnName);
                object value = supplierSet.Supplier[0][column];
                if (value == DBNull.Value)
                    value = String.Empty;
                
                ExcelHelper.ReplaceTag(tagName, value, excelSheet.Cells);
            }
        }

        /// <summary>
        /// Replaces all the service tags in the excel sheet.
        /// </summary>
        private void FillServiceInfo()
        {
            while (true)
            {
                Excel.Range beginRange = excelSheet.Cells.Find("[!BeginServices]", ExcelHelper.Missing, Excel.XlFindLookIn.xlValues,
                    Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false, ExcelHelper.Missing, ExcelHelper.Missing);
                if (beginRange == null)
                    break;

                Excel.Range endRange = excelSheet.Cells.Find("[!EndServices]", beginRange, Excel.XlFindLookIn.xlValues,
                    Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false, ExcelHelper.Missing, ExcelHelper.Missing);
                if (endRange == null)
                    break;

                string beginCell = ExcelHelper.GetCellPosition(beginRange.Row + 1, beginRange.Column);
                string endCell = ExcelHelper.GetCellPosition(endRange.Row - 1, endRange.Column);

                Excel.Range tagRange = excelSheet.get_Range(beginCell, endCell);

                for (int i = 0; i < supplierSet.Service.Count; i++)
                {
                    Excel.Range replaceRange;

                    if (i < (supplierSet.Service.Count - 1))
                    {
                        tagRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, ExcelHelper.Missing);
                        string pasteCell1 = ExcelHelper.GetCellPosition((tagRange.Row - tagRange.Rows.Count), beginRange.Column);
                        string pasteCell2 = ExcelHelper.GetCellPosition(tagRange.Row - 1, endRange.Column);
                        Excel.Range pasteRange = excelSheet.get_Range(pasteCell1, pasteCell2);

                        tagRange.Copy(ExcelHelper.Missing);
                        pasteRange.PasteSpecial(Excel.XlPasteType.xlPasteAll, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, ExcelHelper.Missing, ExcelHelper.Missing);
                        replaceRange = pasteRange;
                    }
                    else
                    {
                        replaceRange = tagRange;
                    }

                    // get the service type name
                    string serviceTypeName = String.Empty;
                    ToolSet.ServiceTypeRow serviceType =
                        Cache.ToolSet.ServiceType.FindByServiceTypeID(supplierSet.Service[i].ServiceTypeID);
                    if (serviceType != null)
                        serviceTypeName = serviceType.ServiceTypeName;

                    // add custom tags
                    ExcelHelper.ReplaceTag("[!ServiceType]", serviceTypeName, replaceRange);

                    foreach (DataColumn column in supplierSet.Service.Columns)
                    {
                        string tagName = String.Format("[!{0}]", column.ColumnName);
                        object value = supplierSet.Service[i][column];
                        if (value == DBNull.Value)
                            value = String.Empty;
                        
                        ExcelHelper.ReplaceTag(tagName, value, replaceRange);
                    }
                }

                beginRange.Value2 = null;
                endRange.Value2 = null;
            }
        }

        /// <summary>
        /// Replaces the supplier config tags in the excel sheet.
        /// </summary>
        private void FillSupplierConfigs()
        {
            string configs = String.Empty;

            foreach (ToolSet.SupplierConfigTypeRow configType in Cache.ToolSet.SupplierConfigType)
            {
                SupplierSet.SupplierConfigRow config =
                    supplierSet.SupplierConfig.FindBySupplierIDSupplierConfigTypeID(supplierSet.Supplier[0].SupplierID, configType.SupplierConfigTypeID);

                configs += String.Format("{0} {1},    ", (config != null) ? BlackSquare : WhiteSquare, configType.SupplierConfigTypeName);
            }

            // strip the trailing ",    " from the string
            configs = configs.Remove(configs.LastIndexOf(",    "));

            ExcelHelper.ReplaceTag("[!SupplierConfigs]", configs, excelSheet.Cells);
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

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
