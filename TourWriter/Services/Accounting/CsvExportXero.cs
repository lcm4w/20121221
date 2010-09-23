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
            foreach (DataRow row in DataSource.Rows)
                CsvContent.AppendLine(templateHelper.Replace(TemplateText, row));
            
            WriteCsvFile(CsvContent.ToString(), filename);
        }
    }
}
