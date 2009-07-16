using System;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;

namespace TourWriter.Reports.Itinerary
{
	/// <summary>
	/// Privides a blank report for holding multiple reports for batch processing
	/// </summary>
	public class BatchReportBlank : ActiveReport
	{
		public BatchReportBlank()
		{
            SetLicense(Lookup.Lic);
            InitializeReport();
		}


		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.PageHeader PageHeader = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.PageFooter PageFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.BatchReportBlank.rpx");
			this.PageHeader = ((DataDynamics.ActiveReports.PageHeader)(this.Sections["PageHeader"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.PageFooter = ((DataDynamics.ActiveReports.PageFooter)(this.Sections["PageFooter"]));
		}

		#endregion
	}
}
