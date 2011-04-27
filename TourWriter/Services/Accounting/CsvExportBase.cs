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
        public string TemplateColumnHeader { get; set; }
        public string TemplateColumnTags { get; set; }

        public DataTable DataSource { get; set; }


        public abstract void ExportTo(string filename);
        
        protected string ReadTemplateFile(string templateFile)
        {
            string text;
            using (var reader = new StreamReader(templateFile))
            {
                //read the first line, assume it as header
                 text = reader.ReadLine();
                TemplateColumnHeader = text;

                //read second line
                text = reader.ReadLine();
                if (text != null)
                {
                    return text;
                }
                text = TemplateColumnHeader;
                TemplateColumnHeader = "";
            }
            return text;
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
