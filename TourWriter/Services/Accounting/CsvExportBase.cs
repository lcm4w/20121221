using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace TourWriter.Services.Accounting
{
    public abstract class CsvExportBase : IExport
    {
        protected string TemplateText { get; set; }
        protected DataTable DataSource { get; set; }
        protected StringBuilder CsvContent { get; set; }
        protected List<string> MoneyColumns = new List<string> {"", "", ""};

        protected CsvExportBase(DataTable dataSource, string templateFile)
        {
            DataSource = dataSource;
            CsvContent = new StringBuilder();
            TemplateText = ReadTemplateFile(templateFile);
        }

        public abstract void ExportTo(string filename);
        
        protected string ReadTemplateFile(string templateFile)
        {
            using (TextReader reader = new StreamReader(templateFile))
                return reader.ReadLine();
        }

        protected static void WriteCsvFile(string text, string fileName)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(fileName)) writer.Write(text);
            }
            catch (Exception ex)
            {
                if (ErrorHelper.IsFileAccessError(ex))
                    App.ShowError("Failed to write file, ensure file with same name is not in use/open.");
                else throw;
            }
        }

        protected void SortDataSource(string sortPhrase)
        {
            DataSource.DefaultView.Sort = sortPhrase;
            DataSource = DataSource.DefaultView.ToTable();
        }
    }
}
