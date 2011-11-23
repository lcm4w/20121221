using System;
using System.IO;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Export.Pdf;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.ReportViewer
{
	public class ReportEmailBuilder : Emailer.EmailBuilderBase
	{	
		private const string DefaultSubject = "TourWriter Report attached.";
		private const string DefaultBody = "Please refer to the attached report file (Adobe PDF).";
		public ItinerarySet ItinerarySet;

		public ReportEmailBuilder(ActiveReport[] reportList)
		{
			foreach(ActiveReport report in reportList)
				SourceList.Add(report);
			
			// Set default Subject to name of first report
			if(reportList.Length > 0)
				this.TemplateSubject = reportList[0].Document.Name;
			else
				this.TemplateSubject = DefaultSubject;

			this.TemplateBody = DefaultBody;
		}

		public override void BuildEmails()
		{
			EmailList.Clear();	
			foreach(ActiveReport report in SourceList)
				EmailList.Add(BuildEmail(report));
		}

        public override void RecordSavedEmail(EmailMessage email, string savedFilename)
		{
			if(this.ItinerarySet != null)
			{
				// record the email in the message table
				ItinerarySet.MessageRow messageRow = ItinerarySet.Message.NewMessageRow();
				messageRow.MessageType = "Email";
				messageRow.MessageTo   = email._To;
				messageRow.MessageFrom = email._From;
				messageRow.MessageName = email.Subject;
                messageRow.MessageFile = savedFilename;
				messageRow.AddedOn     = DateTime.Now;
				messageRow.AddedBy     = TourWriter.Global.Cache.User.UserID;
				ItinerarySet.Message.AddMessageRow(messageRow);

				// join the ItineraryMessage table
				ItinerarySet.ItineraryMessage.AddItineraryMessageRow(
					ItinerarySet.Itinerary[0], messageRow, DateTime.Now, TourWriter.Global.Cache.User.UserID);

				// join the SupplierMessage table
				if(email._Tag != null)
				{
					int supplierId = (int)email._Tag;
					ItinerarySet.SupplierMessage.AddSupplierMessageRow(
						messageRow, supplierId, DateTime.Now, TourWriter.Global.Cache.User.UserID);
				}
			}
		}


		private EmailMessage BuildEmail(ActiveReport report)
		{							
			string to = "";
			if(report.Fields.Contains("EmailTo"))
				to = report.Fields["EmailTo"].Value.ToString();

            EmailMessage email = new EmailMessage();
			email._To		= to;
			email._From		= TemplateFrom;
			email._Bcc		= TemplateBcc;
			email.Subject	= TemplateSubject;
			email.Body		= TemplateBody;
            email._SaveWhenSent = SaveSendMessages;
            email.AddAttachment(GetPdfAttachment(report), report.Document.Name.Trim() + ".pdf");

			// Use the email tag to store the supplier id 
			// for recording saved email against the supplier.
			if(report.Fields.Contains("SupplierID"))
                email._Tag = report.Fields["SupplierID"].Value;
            
		    return email;
		}		
		

		private byte[] GetPdfAttachment(ActiveReport report)
		{
		    // TODO : this throwing 'index out of range' error with pdf and rtf export since upgrade to AR2.
			MemoryStream memStream = new MemoryStream();
	
			PdfExport pdf;
			pdf = new PdfExport();
		        
			pdf.Export(report.Document, memStream);
			return memStream.ToArray();
		}

	}
}
