using System;
using System.Data;
using System.IO;
using System.Text;

namespace TourWriter.Services.Accounting
{
    public abstract class CsvExportBase : IExport
    {
        private string _templateFile;
        private StringBuilder _csvContent;

        public string TemplateFile
        {
            get { return _templateFile; }
            set
            {
                _templateFile = value;
                TemplateText = ReadTemplateFile(value);
            }
        }

        protected StringBuilder CsvContent
        {
            get { return _csvContent ?? (_csvContent = new StringBuilder()); }
        }

        public string TemplateText { get; set; }

        public DataTable DataSource { get; set; }


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
