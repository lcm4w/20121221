using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System;

namespace TourWriter.Services
{
    internal class AccountingService
    {
        private class DataRowComparer : IComparer<DataRow>
        {
            private readonly string columnName;

            public DataRowComparer(string columnName)
            {
                this.columnName = columnName;
            }

            public int Compare(DataRow row1, DataRow row2)
            {
                return row1[columnName].ToString().CompareTo(row2[columnName].ToString());
            }
        }

        private DataTable dataTable;
        private readonly string template;

        internal List<string> InvalidTags { get; private set; }
        
        public AccountingService(DataTable dataTable, string templateFile)
        {
            this.dataTable = dataTable.Copy();
            template = LoadTemplate(templateFile);
            InvalidTags = new List<string>();
        }

        public AccountingService(DataTable dataTable, string templateFile, string groupColumn)
            : this(dataTable, templateFile)
        {
            SortAndGroupTable(groupColumn);
        }

        internal static string LoadTemplate(string templateFile)
        {
            // load the template
            string template;
            using (TextReader reader = new StreamReader(templateFile))
            {
                template = reader.ReadLine();
            }
            return template;
        }

        /// <summary>
        /// Exports all rows to a CSV file to be read by accounting software.
        /// </summary>
        internal void ExportToCsv(string filename)
        {
            StringBuilder sb = CreateCsvRows();
            WriteCsv(sb, filename);
        }

        private StringBuilder CreateCsvRows()
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(template))
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (dataRow.RowState == DataRowState.Deleted)
                        continue;

                    TemplateHelper templateHelper = new TemplateHelper();
                    string csvRow = !IsRowEmpty(dataRow) ? templateHelper.Replace(template, dataRow) : String.Empty;

                    // remove line breaks
                    csvRow = csvRow.Replace(Environment.NewLine, " ");

                    sb.AppendLine(csvRow);
                    InvalidTags = templateHelper.InvalidTags;
                }
            }
            return sb;
        }

        private static void WriteCsv(StringBuilder sb, string fileName)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(fileName))
                {
                    writer.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                if (ErrorHelper.IsFileAccessError(ex))
                    App.ShowError("Failed to write file, ensure file with same name is not in use/open.");
                else
                    throw;
            }
        }

        /// <summary>
        /// Sorts and groups a table and puts an empty row after each group.
        /// </summary>
        private void SortAndGroupTable(string columnName)
        {
            // convert the DataTable to a List to sort
            List<DataRow> rowList = new List<DataRow>();
            rowList.AddRange(dataTable.Select());
            rowList.Sort(new DataRowComparer(columnName));

            // convert the List back into a DataTable
            DataTable sortedTable = dataTable.Clone();
            string lastValue = rowList[0][columnName].ToString();
            foreach (DataRow row in rowList)
            {
                // add an empty row when the value changes, so the tag replacer knows to put a blank line there
                string value = row[columnName].ToString();
                if (value != lastValue)
                {
                    sortedTable.Rows.Add();
                    lastValue = value;
                }

                sortedTable.Rows.Add(row.ItemArray);
            }
            dataTable = sortedTable;
        }

        /// <summary>
        /// returns true if all columns in a row are DBNull, otherwise false.
        /// </summary>
        private static bool IsRowEmpty(DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                if (row[column] != DBNull.Value)
                    return false;
            }
            return true;
        }
    }
}
