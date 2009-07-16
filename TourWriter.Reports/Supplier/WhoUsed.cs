using System;
using System.Data.SqlClient;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;

namespace TourWriter.Reports.Supplier
{
	public class WhoUsed : ActiveReport
	{
		private int supplierID;
		private DateTime dateFrom;
		private DateTime dateTo;
		private SqlDataReader dataReader;

		public WhoUsed(int supplierID, DateTime dateFrom, DateTime dateTo)
		{
            SetLicense(Lookup.Lic);
			InitializeReport();

			this.supplierID = supplierID;
			this.dateFrom = dateFrom;
			this.dateTo = dateTo;

			this.Document.Name = String.Format("Supplier Usage: {0} to {1}", 
				dateFrom.ToShortDateString(), dateTo.ToShortDateString());
		}

				
		private void WhoUsed_ReportStart(object sender, System.EventArgs eArgs)
		{
			TourWriter.BusinessLogic.Reports r = new TourWriter.BusinessLogic.Reports();
			dataReader = r.WhoUsedSupplier(this.supplierID, this.dateFrom, this.dateTo);
		}

		private void WhoUsed_DataInitialize(object sender, System.EventArgs eArgs)
		{
			Fields.Add("Title");

			Fields.Add("ServiceName");
			Fields.Add("BookingDate");
			Fields.Add("ItineraryName");
			Fields.Add("LineName");
			Fields.Add("ItemName");		
		}

		private void WhoUsed_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
			try
			{
				Fields["Title"].Value = this.Document.Name;

				if(dataReader.Read())
				{	
					// srv.ServiceName, item.StartDate, itin.ItineraryName, line.PurchaseLineName,item.ServiceName, item.OptionName
					Fields["ServiceName"].Value   = dataReader.GetString(0);
					Fields["BookingDate"].Value   = dataReader.GetDateTime(1).ToShortDateString();
					Fields["ItineraryName"].Value = dataReader.GetString(2);
					Fields["LineName"].Value      = dataReader.GetString(3).ToString();
					Fields["ItemName"].Value      = dataReader.GetString(4).ToString();

					eArgs.EOF = false;	
				}
				else
					eArgs.EOF = true;	
			}
			catch(Exception ex)	
			{					
				eArgs.EOF = true;	
				throw new Exception(ex.ToString());
			}
		}

		private void WhoUsed_ReportEnd(object sender, System.EventArgs eArgs)
		{
			if(!dataReader.IsClosed)
				dataReader.Close();

			dataReader = null;
		}


		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.ReportHeader ReportHeader = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.PageHeader PageHeader = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.Line Line1 = null;
		private DataDynamics.ActiveReports.Label Label10 = null;
		private DataDynamics.ActiveReports.Label Label11 = null;
		private DataDynamics.ActiveReports.GroupHeader ghService = null;
		private DataDynamics.ActiveReports.Shape Shape1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox5 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox2 = null;
		private DataDynamics.ActiveReports.TextBox Description1 = null;
		private DataDynamics.ActiveReports.TextBox ItemDescription1 = null;
		private DataDynamics.ActiveReports.GroupFooter gfService = null;
		private DataDynamics.ActiveReports.PageFooter PageFooter = null;
		private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Supplier.WhoUsed.rpx");
			this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
			this.PageHeader = ((DataDynamics.ActiveReports.PageHeader)(this.Sections["PageHeader"]));
			this.ghService = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghService"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.gfService = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfService"]));
			this.PageFooter = ((DataDynamics.ActiveReports.PageFooter)(this.Sections["PageFooter"]));
			this.ReportFooter = ((DataDynamics.ActiveReports.ReportFooter)(this.Sections["ReportFooter"]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[0]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[0]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[1]));
			this.Line1 = ((DataDynamics.ActiveReports.Line)(this.PageHeader.Controls[2]));
			this.Label10 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[3]));
			this.Label11 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[4]));
			this.Shape1 = ((DataDynamics.ActiveReports.Shape)(this.ghService.Controls[0]));
			this.TextBox5 = ((DataDynamics.ActiveReports.TextBox)(this.ghService.Controls[1]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.Description1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.ItemDescription1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.WhoUsed_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.WhoUsed_FetchData);
			this.ReportStart += new System.EventHandler(this.WhoUsed_ReportStart);
			this.ReportEnd += new System.EventHandler(this.WhoUsed_ReportEnd);
		}

		#endregion
	}
}
