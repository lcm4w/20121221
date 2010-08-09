using System.Data;

namespace TourWriter.Services.Accounting
{
    public class CsvExportMyob : CsvExportBase
    {
        private const string GroupByColumnName = "PurchaseLineID";
        public CsvExportMyob(DataTable dataSource, string templatename)
            : base(dataSource, templatename) { }

        public override void ExportTo(string filename)
        {
            var defineGroups = DataSource.Columns.Contains(GroupByColumnName);
            if (defineGroups) SortDataSource(GroupByColumnName + " ASC");

            var templateHelper = new TemplateHelper();
            foreach (DataRow row in DataSource.Rows)
            {
                if (defineGroups) InsertLineBetweenGroups(row);
                CsvContent.AppendLine(templateHelper.Replace(TemplateText, row));
            }
            WriteCsvFile(CsvContent.ToString(), filename);
        }

        private string _tempVal = "";
        private void InsertLineBetweenGroups(DataRow row)
        {
            var val = row[GroupByColumnName].ToString();
            if (_tempVal == "") _tempVal = val;
            if (_tempVal == val) return;
            CsvContent.AppendLine("");
            _tempVal = val;
        }
    }
}
