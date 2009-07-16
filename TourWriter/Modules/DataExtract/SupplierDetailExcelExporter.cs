using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using TourWriter.Services;

namespace TourWriter.Modules.DataExtract
{
    public class SupplierDetailExcelExporter
    {
         public event ProgressChangedEventHandler ProgressChanged;
        public event ExportCompletedEventHandler ExportCompleted;

        private BackgroundWorker workerThread;
        private DataTable dataTable;
        private string templateFileName;
        private string saveFileName;

        public void StartExport(DataTable table, string templateFile, string saveFile)
        {
            if (workerThread != null && workerThread.IsBusy)
                return; // an export thread is already running

            // copy the table, so the original is not altered in any way
            dataTable = table.Copy();
            App.PrepareDataTableForExport(dataTable);

            templateFileName = templateFile;
            saveFileName = saveFile;

            workerThread = new BackgroundWorker();
            workerThread.WorkerReportsProgress = true;
            workerThread.WorkerSupportsCancellation = true;
            workerThread.DoWork += workerThread_DoWork;
            workerThread.RunWorkerCompleted += workerThread_RunWorkerCompleted;
            workerThread.ProgressChanged += workerThread_ProgressChanged;
            workerThread.RunWorkerAsync();
        }

        public void StopExport()
        {
            workerThread.CancelAsync();
        }

        private static Excel.Range GetBeginEndRange(ExcelWriter excelWriter, string beginTag, string endTag, Excel.Range searchRange)
        {
            // find the begin and end tags
            var beginRange = searchRange.Find(beginTag, Missing.Value, Excel.XlFindLookIn.xlValues,
                Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false, Missing.Value, Missing.Value);
            if (beginRange == null)
                return null;

            var endRange = searchRange.Find(endTag, Missing.Value, Excel.XlFindLookIn.xlValues,
                Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false, Missing.Value, Missing.Value);
            if (endRange == null)
                return null;


            // get the cell range between the begin and end tags
            string beginCell = ExcelHelper.GetCellPosition(beginRange.Row + 1, beginRange.Column);
            string endCell = ExcelHelper.GetCellPosition(endRange.Row - 1, endRange.Column);
            var beginEndRange = excelWriter.XlWorksheet.get_Range(beginCell, endCell);


            // delete the begin and end tags properly
            var deleteRange1 = excelWriter.XlWorksheet.get_Range(
                ExcelHelper.GetCellPosition(beginRange.Row, beginRange.Column),
                ExcelHelper.GetCellPosition(beginRange.Row, endRange.Column));

            var deleteRange2 = excelWriter.XlWorksheet.get_Range(
                ExcelHelper.GetCellPosition(endRange.Row, beginRange.Column),
                ExcelHelper.GetCellPosition(endRange.Row, endRange.Column));

            deleteRange1.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
            deleteRange2.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);

            return beginEndRange;
        }

        private static Excel.Range GetBeginEndRange(ExcelWriter excelWriter, string beginTag, string endTag)
        {
            return GetBeginEndRange(excelWriter, beginTag, endTag, excelWriter.XlWorksheet.Cells);
        }

        private void ReplaceServiceTags(ExcelWriter excelWriter, int rowIndex, Excel.Range searchRange)
        {
            while (true)
            {
                var tagRange = GetBeginEndRange(excelWriter, "[!BeginService]", "[!EndService]", searchRange);
                if (tagRange == null)
                    break;

                int supplierId = (int)dataTable.Rows[rowIndex]["SupplierID"];
                var serviceRows = new List<DataRow>();
                
                for (int x = rowIndex; x < dataTable.Rows.Count && supplierId == (int)dataTable.Rows[x]["SupplierID"]; x++)
                {
                    serviceRows.Add(dataTable.Rows[x]);
                }

                for (int i = 0; i < serviceRows.Count; i++)
                {
                    var row = serviceRows[i];

                    Excel.Range replaceRange = (i < (serviceRows.Count - 1))
                                                   ? excelWriter.CopyCells(tagRange)
                                                   : tagRange;

                    // replace tags
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        string tagName = String.Format("[!{0}]", column.ColumnName);
                        object value = row[column];
                        if (value == DBNull.Value)
                            value = String.Empty;

                        excelWriter.Replace(tagName, value, replaceRange);
                    }
                }
            }
        }

        private void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var excelWriter = new ExcelWriter(templateFileName))
            {
                while (true)
                {
                    var tagRange = GetBeginEndRange(excelWriter, "[!BeginSupplier]", "[!EndSupplier]");
                    if (tagRange == null)
                        break;

                    int lastSupplierId = int.MinValue;

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var row = dataTable.Rows[i];

                        Excel.Range replaceRange = (i < (dataTable.Rows.Count - 1))
                                                       ? excelWriter.CopyCells(tagRange)
                                                       : tagRange;

                        if (lastSupplierId != (int)row["SupplierID"])
                        {
                            lastSupplierId = (int)row["SupplierID"];

                            // replace service tags first
                            ReplaceServiceTags(excelWriter, i, replaceRange);

                            // replace tags
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                string tagName = String.Format("[!{0}]", column.ColumnName);
                                object value = row[column];
                                if (value == DBNull.Value)
                                    value = String.Empty;

                                excelWriter.Replace(tagName, value, replaceRange);
                            }
                        }
                        else
                        {
                            replaceRange.Delete(Missing.Value);
                        }

                        // update progress
                        int progress = (int)(i * (100.0 / dataTable.Rows.Count));
                        workerThread.ReportProgress(progress);

                        // check if the thread needs to be cancelled
                        if (workerThread.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                }

                // save to file
                excelWriter.Save(saveFileName);
            }
        }

        private void workerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ExportCompleted != null)
                ExportCompleted(this, new ExportCompletedEventArgs(e.Cancelled, saveFileName));
        }

        private void workerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(sender, e);
        }
    }
}
