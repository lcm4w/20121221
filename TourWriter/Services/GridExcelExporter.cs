using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using TourWriter.Services;
using Infragistics.Win.UltraWinGrid;

namespace TourWriter.Services
{
    public class GridExcelExporter
    {
        private Excel.Application excelApp;
        private Excel.Workbook excelBook;
        private Excel.Worksheet excelSheet;
        private Excel.Workbook templateBook;
        private Excel.Worksheet templateSheet;
        private readonly UltraGrid grid;

        public GridExcelExporter(UltraGrid grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Creates the excel sheet and saves it to the specified file.
        /// </summary>
        public void Export(string templateFile, string saveFile)
        {
            // initialize excel
            excelApp = new Excel.Application();

            // open template
            templateBook = excelApp.Workbooks.Open(templateFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing,
                ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);

            templateSheet = (Excel.Worksheet)templateBook.Worksheets[1];

            if (File.Exists(saveFile))
            {
                // open existing workbook
                excelBook = excelApp.Workbooks.Open(saveFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing,
                    ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);

                Excel.Worksheet lastSheet = (Excel.Worksheet)excelBook.Worksheets[excelBook.Worksheets.Count];
                templateSheet.Copy(ExcelHelper.Missing, lastSheet);
                excelSheet = (Excel.Worksheet)excelBook.Worksheets[excelBook.Worksheets.Count];
            }
            else
            {
                // create new
                excelBook = excelApp.Workbooks.Add(ExcelHelper.Missing);
                Excel.Worksheet firstSheet = (Excel.Worksheet)excelBook.Worksheets[1];
                templateSheet.Copy(firstSheet, ExcelHelper.Missing);
                excelSheet = (Excel.Worksheet)excelBook.Worksheets[1];
            }
            
            // export the grid
            FillGridInfo();

            excelBook.SaveAs(saveFile, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, Excel.XlSaveAsAccessMode.xlShared,
                ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);

            Cleanup();

            Process.Start(saveFile);
        }

        private void FillGridInfo()
        {
            Dictionary<string, int> columnsList = new Dictionary<string, int>();
            for (int i = 1; i <= 50; i++)
            {
                string columnName = Convert.ToString(((Excel.Range)excelSheet.Cells[1, i]).Value2);
                if (!String.IsNullOrEmpty(columnName) && !columnsList.ContainsKey(columnName))
                    columnsList.Add(columnName, i);
            }

            foreach (UltraGridRow row in grid.Rows)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (columnsList.ContainsKey(cell.Column.Key))
                    {
                        ((Excel.Range)excelSheet.Cells[row.Index + 2, columnsList[cell.Column.Key]]).Value2 = cell.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Closes the excel app and releases all the excel objects.
        /// </summary>
        public void Cleanup()
        {
            excelBook.Close(ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);
            templateBook.Close(ExcelHelper.Missing, ExcelHelper.Missing, ExcelHelper.Missing);
            excelApp.Quit();

            Marshal.ReleaseComObject(excelSheet);
            Marshal.ReleaseComObject(excelBook);
            Marshal.ReleaseComObject(templateSheet);
            Marshal.ReleaseComObject(templateBook);
            Marshal.ReleaseComObject(excelApp);

            excelSheet = null;
            excelBook = null;
            templateSheet = null;
            templateBook = null;
            excelApp = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
