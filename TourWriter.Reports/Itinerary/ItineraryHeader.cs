using System;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;
using TourWriter.Info;

namespace TourWriter.Reports.Itinerary
{
	public class ItineraryHeader : ActiveReport
	{
		ToolSet toolSet;
		ItinerarySet.ItineraryRow itinerary;

		public ItineraryHeader(ItinerarySet itinerarySet, ToolSet toolSet)
		{
			this.itinerary = itinerarySet.Itinerary[0];
			this.toolSet = toolSet;

            SetLicense(Lookup.Lic);
            InitializeReport();
		}

		private void ItineraryHeader_DataInitialize(object sender, System.EventArgs eArgs)
		{
			Fields.Add("ItineraryName");
			Fields.Add("ArriveDate");
			Fields.Add("Length");
			Fields.Add("Origin");
		}

		private void ItineraryHeader_ReportStart(object sender, System.EventArgs eArgs)
		{
			
		}

		private void ItineraryHeader_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
            Fields["ItineraryName"].Value = itinerary.GetDisplayNameOrItineraryName();

			Fields["ArriveDate"].Value = !itinerary.IsArriveDateNull() ? 
				itinerary.ArriveDate.ToShortDateString() : "n/a";
 

			Fields["Origin"].Value = !itinerary.IsCountryIDNull() ? 
				toolSet.Country.FindByCountryID(itinerary.CountryID).CountryName :"n/a";
			
			//int days = itinerarySet.ItineraryLength();
			//Fields["Length"].Value     = days>-1 ? days.ToString() + " days" : "";
		}

		
		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.ReportHeader ReportHeader = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox2 = null;
		private DataDynamics.ActiveReports.TextBox TextBox3 = null;
		private DataDynamics.ActiveReports.Line Line1 = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.TextBox TextBox4 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.ItineraryHeader.rpx");
			this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.ReportFooter = ((DataDynamics.ActiveReports.ReportFooter)(this.Sections["ReportFooter"]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[0]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[1]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[2]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[3]));
			this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[4]));
			this.TextBox3 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[5]));
			this.Line1 = ((DataDynamics.ActiveReports.Line)(this.ReportHeader.Controls[6]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[7]));
			this.TextBox4 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[8]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.ItineraryHeader_DataInitialize);
			this.ReportStart += new System.EventHandler(this.ItineraryHeader_ReportStart);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.ItineraryHeader_FetchData);
		}

		#endregion
	}
}
