using System;
using System.Data.SqlClient;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;

namespace TourWriter.Reports.General
{
	public class ClientLocations : ActiveReport
	{
		private DateTime dateFrom;
		private DateTime dateTo;
		string itineraryStatusList;
		string requestStatusList; 
		private SqlDataReader dataReader;

		public ClientLocations(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string requestStatusList)
		{
            SetLicense(Lookup.Lic);
            InitializeReport();

			this.dateFrom = dateFrom;
			this.dateTo = dateTo;
			this.itineraryStatusList = itineraryStatusList;
			this.requestStatusList = requestStatusList;  
			
			this.Document.Name = String.Format("Client Travel Locations: {0} to {1}", 
				dateFrom.ToShortDateString(), dateTo.ToShortDateString());
		}

		
		private void ClientLocations_ReportStart(object sender, System.EventArgs eArgs)
		{
			TourWriter.BusinessLogic.Reports r = new TourWriter.BusinessLogic.Reports();
			dataReader = r.ClientLocations(dateFrom, dateTo, itineraryStatusList, requestStatusList);
		}

		private void ClientLocations_DataInitialize(object sender, System.EventArgs eArgs)
		{
			Fields.Add("Title");
			Fields.Add("Date");
			Fields.Add("City");
			Fields.Add("Itinerary");
			Fields.Add("Supplier");	
			Fields.Add("Status");			
		}

		private void ClientLocations_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
			try
			{
				Fields["Title"].Value = this.Document.Name;
					
				if(dataReader.Read())
				{
					Fields["Date"].Value = dataReader.GetDateTime(0).ToShortDateString();	
					Fields["City"].Value = dataReader.GetString(1);	
					Fields["Itinerary"].Value = dataReader.GetString(2);	
					Fields["Supplier"].Value = dataReader.GetString(3);		
					Fields["Status"].Value = dataReader.GetString(4);	

					eArgs.EOF = false;	
				}
				else
					eArgs.EOF = true;	
			}
			catch(Exception)	
			{	
				eArgs.EOF = true;	
				throw;
			}
		}

		private void ClientLocations_ReportEnd(object sender, System.EventArgs eArgs)
		{
			if(!dataReader.IsClosed)
				dataReader.Close();

			dataReader = null;
		}

		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.ReportHeader ReportHeader = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.PageHeader PageHeader = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.Line Line1 = null;
		private DataDynamics.ActiveReports.Label Label5 = null;
		private DataDynamics.ActiveReports.GroupHeader ghDay = null;
		private DataDynamics.ActiveReports.Shape Shape1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox TextBox2 = null;
		private DataDynamics.ActiveReports.TextBox TextBox3 = null;
		private DataDynamics.ActiveReports.TextBox TextBox4 = null;
		private DataDynamics.ActiveReports.TextBox Itinerary1 = null;
		private DataDynamics.ActiveReports.GroupFooter gfDay = null;
		private DataDynamics.ActiveReports.PageFooter PageFooter = null;
		private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.General.ClientLocations.rpx");
			this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
			this.PageHeader = ((DataDynamics.ActiveReports.PageHeader)(this.Sections["PageHeader"]));
			this.ghDay = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghDay"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.gfDay = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfDay"]));
			this.PageFooter = ((DataDynamics.ActiveReports.PageFooter)(this.Sections["PageFooter"]));
			this.ReportFooter = ((DataDynamics.ActiveReports.ReportFooter)(this.Sections["ReportFooter"]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[0]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[0]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[1]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[2]));
			this.Line1 = ((DataDynamics.ActiveReports.Line)(this.PageHeader.Controls[3]));
			this.Label5 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[4]));
			this.Shape1 = ((DataDynamics.ActiveReports.Shape)(this.ghDay.Controls[0]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.ghDay.Controls[1]));
			this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.TextBox3 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.TextBox4 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.Itinerary1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.ClientLocations_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.ClientLocations_FetchData);
			this.ReportStart += new System.EventHandler(this.ClientLocations_ReportStart);
			this.ReportEnd += new System.EventHandler(this.ClientLocations_ReportEnd);
		}

		#endregion
	}
}
