using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TourWriter.Services.Accounting
{
    public class CsvExportXero : CsvExportBase
    {
        private const string SortByColumnName = "PurchaseLineID";

        public CsvExportXero(DataTable dataSource, string templatename) : base(dataSource, templatename) { }

        public override void ExportTo(string filename)
        {
            // NOTE: this stuff needs to come from the database, eg: TotalNetExcl, TotalGrossExcl
            //ProcessMoneyColumns();

            var hasSortCol = DataSource.Columns.Contains(SortByColumnName);
            if (hasSortCol) SortDataSource(SortByColumnName + " ASC");

            var templateHelper = new TemplateHelper();
            foreach (DataRow row in DataSource.Rows)
                CsvContent.AppendLine(templateHelper.Replace(TemplateText, row));
            
            WriteCsvFile(CsvContent.ToString(), filename);
        }
    }
}
