using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace TourWriter.Services
{
    public class DataTableExcelExporter
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

        private static Excel.Range GetBeginEndRange(ExcelWriter excelWriter)
        {
            // find the begin and end tags
            var beginRange = excelWriter.XlWorksheet.Cells.Find("[!Begin]", Missing.Value, Excel.XlFindLookIn.xlValues,
                Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false, Missing.Value, Missing.Value);
            if (beginRange == null)
                return null;

            var endRange = excelWriter.XlWorksheet.Cells.Find("[!End]", Missing.Value, Excel.XlFindLookIn.xlValues,
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

        private void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var excelWriter = new ExcelWriter(templateFileName))
            {
                while (true)
                {
                    var tagRange = GetBeginEndRange(excelWriter);
                    if (tagRange == null)
                        break;

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var row = dataTable.Rows[i];

                        Excel.Range replaceRange = (i < (dataTable.Rows.Count - 1))
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

    public delegate void ExportCompletedEventHandler(object sender, ExportCompletedEventArgs e);
    public class ExportCompletedEventArgs : EventArgs
    {
        public bool Cancelled { get; private set; }
        public string FileName { get; private set; }
        
        public ExportCompletedEventArgs(bool cancelled, string fileName)
        {
            Cancelled = cancelled;
            FileName = fileName;
        }
    }
}
