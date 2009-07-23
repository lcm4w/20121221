using System.IO;
using Microsoft.Reporting.WinForms;
using TourWriter.Info;
using TourWriter.Services;
using TourWriter.Modules.Emailer;

namespace TourWriter.UserControls.Reports
{
	public class ReportEmailBuilder : EmailBuilderBase
	{	
		private const string DefaultSubject = "TourWriter Report attached.";
		private const string DefaultBody = "Please refer to the attached report file (Adobe PDF).";
		public ItinerarySet ItinerarySet;

		public ReportEmailBuilder(LocalReport report)
		{
            SourceList.Add(report);
	        TemplateSubject = DefaultSubject;
			TemplateBody = DefaultBody;
		}

		public override void BuildEmails()
		{
			EmailList.Clear();	
			foreach(LocalReport report in SourceList)
                EmailList.Add(BuildEmail(report));
		}

        public override void RecordSavedEmail(EmailMessage email, string savedFilename) { }

		private EmailMessage BuildEmail(LocalReport report)
		{							
			const string to = "";
            var email = new EmailMessage
                            {
                                _To = to,
                                _From = TemplateFrom,
                                _Bcc = TemplateBcc,
                                Subject = TemplateSubject,
                                Body = TemplateBody,
                                _SaveWhenSent = SaveSendMessages
                            };
		    email.AddAttachment(GetPdfAttachment(report), report.DisplayName.Trim() + ".pdf");
		    return email;
		}		
		

		private static byte[] GetPdfAttachment(Report report)
		{
            Warning[] warnings;
            string[] streamIds;
            string mimeType;
            string encoding;
            string extension;
            return report.Render("pdf", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
		}

	}
}
