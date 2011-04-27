using System;
using System.Data;

namespace TourWriter.Services.Accounting
{
    public class CsvExportXero : CsvExportBase
    {
        private const string SortByColumnName = "PurchaseLineID";
        
        public override void ExportTo(string filename)
        {
            var hasSortCol = DataSource.Columns.Contains(SortByColumnName);
            if (hasSortCol) SortDataSource(SortByColumnName + " ASC");

            var templateHelper = new TemplateHelper();
            if (TemplateColumnHeader == "")
            {
                App.ShowError("Your Xero template does not contain a header row. The first row in a Xero template must contain the column-headers");
                return;
            }
            CsvContent.AppendLine(TemplateColumnHeader);
            foreach (DataRow row in DataSource.Rows)
                CsvContent.AppendLine(templateHelper.Replace(TemplateText, row));
            
            WriteCsvFile(CsvContent.ToString(), filename);
        }
    }
}
