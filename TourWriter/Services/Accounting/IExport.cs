
using System;
using System.Data;

namespace TourWriter.Services.Accounting
{
    public interface IExport
    {
        String TemplateFile { get; set; }
        DataTable DataSource { get; set; }

        void ExportTo(string filename);
    }
}
