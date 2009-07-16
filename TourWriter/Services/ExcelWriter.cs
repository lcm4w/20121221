using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TourWriter.Services
{
    public class ExcelWriter : IDisposable
    {
        public Excel.Application XlApplication { get; private set; }
        public Excel.Workbook XlWorkbook { get; private set; }
        public Excel.Worksheet XlWorksheet { get; private set; }

        public ExcelWriter(string fileName)
        {
            XlApplication = new Excel.Application();

            if (!String.IsNullOrEmpty(fileName))
            {
                Open(fileName);
            }
        }

        public ExcelWriter(): this(null)
        {
            
        }

        ~ExcelWriter()
        {
            Cleanup();
        }

        /// <summary>
        /// Opens an excel workbook from file, and selects the first worksheet.
        /// If a workbook is already open, it will be closed.
        /// </summary>
        public void Open(string fileName)
        {
            if (XlWorkbook != null)
            {
                // if a workbook is already open, close it first
                XlWorkbook.Close(false, Missing.Value, Missing.Value);
            }

            XlWorkbook = XlApplication.Workbooks.Open(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            SelectWorksheet(1);
        }

        /// <summary>
        /// Saves the excel workbook to file.
        /// </summary>
        public void Save(string fileName)
        {
            XlWorkbook.SaveAs(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlShared,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
        }

        /// <summary>
        /// Selects the active worksheet, by the sheet's name.
        /// </summary>
        public void SelectWorksheet(string sheetName)
        {
            XlWorksheet = (Excel.Worksheet)XlWorkbook.Worksheets[sheetName];
        }

        /// <summary>
        /// Selects the active worksheet, by the sheet's index.
        /// Excel worksheet indexes start at 1.
        /// </summary>
        public void SelectWorksheet(int sheetIndex)
        {
            XlWorksheet = (Excel.Worksheet)XlWorkbook.Worksheets[sheetIndex];
        }

        /// <summary>
        /// Replaces all occurances of oldValue, with newValue, within the specified cell range.
        /// If the specified range is null, the range will be the entire worksheet.
        /// </summary>
        public void Replace(object oldValue, object newValue, Excel.Range range)
        {
            if (range == null)
                range = XlWorksheet.Cells;

            Excel.Range firstFind = null;
            Excel.Range currentFind = range.Find(oldValue, Missing.Value, Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false, Missing.Value, Missing.Value);

            while (currentFind != null)
            {
                if (firstFind == null)
                    firstFind = currentFind;

                // break if the finder returns to the first find
                else if (currentFind.get_Address(Missing.Value, Missing.Value, Excel.XlReferenceStyle.xlA1, Missing.Value, Missing.Value)
                    == firstFind.get_Address(Missing.Value, Missing.Value, Excel.XlReferenceStyle.xlA1, Missing.Value, Missing.Value))
                {
                    break;
                }

                currentFind.Value2 = currentFind.Value2.ToString().Replace(oldValue.ToString(), newValue.ToString());
                currentFind = range.FindNext(currentFind);
            }
        }

        /// <summary>
        /// Replaces all occurances of oldValue, with newValue, in the entire worksheet.
        /// </summary>
        public void Replace(object oldValue, object newValue)
        {
            Replace(oldValue, newValue, null);
        }

        /// <summary>
        /// Copies and pastes the specified cell range,
        /// returning the cell range of where the copied cells were pasted to.
        /// </summary>
        public Excel.Range CopyCells(Excel.Range copyRange)
        {
            // copy the cells to the clipboard
            copyRange.Copy(Missing.Value);

            // insert the copied cells
            copyRange.Insert(Excel.XlInsertShiftDirection.xlShiftDown, Missing.Value);

            // get the range where the cells were copied to
            string pasteCell1 = ExcelHelper.GetCellPosition(copyRange.Row - copyRange.Rows.Count, copyRange.Column);
            string pasteCell2 = ExcelHelper.GetCellPosition(copyRange.Row - 1, copyRange.Column + copyRange.Columns.Count - 1);
            return XlWorksheet.get_Range(pasteCell1, pasteCell2);;
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all Excel related resources.
        /// </summary>
        private void Cleanup()
        {
            XlWorkbook.Close(false, Missing.Value, Missing.Value);
            XlApplication.Quit();

            Marshal.ReleaseComObject(XlWorksheet);
            Marshal.ReleaseComObject(XlWorkbook);
            Marshal.ReleaseComObject(XlApplication);

            XlApplication = null;
            XlWorkbook = null;
            XlWorksheet = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
