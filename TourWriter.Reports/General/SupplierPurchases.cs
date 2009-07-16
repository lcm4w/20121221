using System;
using System.Data.SqlClient;
using DataDynamics.ActiveReports;

namespace TourWriter.Reports.General
{
	public class SupplierPurchases : ActiveReport
	{
		private DateTime dateFrom;
		private DateTime dateTo;
		string itineraryStatusList;
		string requestStatusList; 
		private SqlDataReader dataReader;

		public SupplierPurchases(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string requestStatusList)
		{
            SetLicense(Lookup.Lic);
            InitializeReport();

			this.dateFrom = dateFrom;
			this.dateTo = dateTo;
			this.itineraryStatusList = itineraryStatusList;
			this.requestStatusList = requestStatusList; 

			this.Document.Name = String.Format("Supplier Purchases: {0} to {1}", 
				dateFrom.ToShortDateString(), dateTo.ToShortDateString());
		}

		
		private void SupplierPurchases_ReportStart(object sender, System.EventArgs eArgs)
		{
			TourWriter.BusinessLogic.Reports r = new TourWriter.BusinessLogic.Reports();
			dataReader = r.SupplierPurchases(dateFrom, dateTo, itineraryStatusList, requestStatusList);
		}

		private void SupplierPurchases_DataInitialize(object sender, System.EventArgs eArgs)
		{
			Fields.Add("Title");
			Fields.Add("SupplierName");
			Fields.Add("BookingDate");
			Fields.Add("Description");
			Fields.Add("RequestStatus");
			Fields.Add("ItineraryGross");
			Fields.Add("ItineraryNet");
			Fields.Add("SupplierTotalNet");	
			Fields.Add("ReportTotalNet");			
		}
		
		private void SupplierPurchases_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
			try
			{
				if(dataReader.Read())
				{	
					Fields["Title"].Value = this.Document.Name;
					
					// DataReader results:
					// SupplierName, StartDate, ItineraryName, Gross, Net, Total nett
					
					Fields["SupplierName"].Value   = dataReader.GetString(0);
					Fields["BookingDate"].Value    = dataReader.GetDateTime(1).ToShortDateString();
					Fields["Description"].Value    = dataReader.GetString(2) + ", " + dataReader.GetString(3);
					Fields["RequestStatus"].Value  = dataReader.GetString(4);
					Fields["ItineraryGross"].Value = dataReader.GetDecimal(5).ToString();
					Fields["ItineraryNet"].Value  = dataReader.GetDecimal(6).ToString();

					// add each nett to the totals
                    Fields["SupplierTotalNet"].Value = dataReader.GetDecimal(6).ToString();
                    Fields["ReportTotalNet"].Value = dataReader.GetDecimal(6).ToString();

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

		private void SupplierPurchases_ReportEnd(object sender, System.EventArgs eArgs)
		{
			if(!dataReader.IsClosed)
				dataReader.Close();

			dataReader = null;
		}

		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.ReportHeader ReportHeader = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox4 = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.PageHeader PageHeader = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.Line Line1 = null;
		private DataDynamics.ActiveReports.Label Label6 = null;
		private DataDynamics.ActiveReports.Label Label7 = null;
		private DataDynamics.ActiveReports.Label Label8 = null;
		private DataDynamics.ActiveReports.Label Label5 = null;
		private DataDynamics.ActiveReports.GroupHeader ghSupplier = null;
		private DataDynamics.ActiveReports.Shape Shape1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox5 = null;
		private DataDynamics.ActiveReports.TextBox TextBox6 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox2 = null;
		private DataDynamics.ActiveReports.TextBox SupplierTotal1 = null;
		private DataDynamics.ActiveReports.TextBox SupplierTotal2 = null;
		private DataDynamics.ActiveReports.TextBox ItineraryGross1 = null;
		private DataDynamics.ActiveReports.GroupFooter gfSupplier = null;
		private DataDynamics.ActiveReports.PageFooter PageFooter = null;
		private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.General.SupplierPurchases.rpx");
			this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
			this.PageHeader = ((DataDynamics.ActiveReports.PageHeader)(this.Sections["PageHeader"]));
			this.ghSupplier = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghSupplier"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.gfSupplier = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfSupplier"]));
			this.PageFooter = ((DataDynamics.ActiveReports.PageFooter)(this.Sections["PageFooter"]));
			this.ReportFooter = ((DataDynamics.ActiveReports.ReportFooter)(this.Sections["ReportFooter"]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[0]));
			this.TextBox4 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[1]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[2]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[0]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[1]));
			this.Line1 = ((DataDynamics.ActiveReports.Line)(this.PageHeader.Controls[2]));
			this.Label6 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[3]));
			this.Label7 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[4]));
			this.Label8 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[5]));
			this.Label5 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[6]));
			this.Shape1 = ((DataDynamics.ActiveReports.Shape)(this.ghSupplier.Controls[0]));
			this.TextBox5 = ((DataDynamics.ActiveReports.TextBox)(this.ghSupplier.Controls[1]));
			this.TextBox6 = ((DataDynamics.ActiveReports.TextBox)(this.ghSupplier.Controls[2]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.SupplierTotal1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.SupplierTotal2 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			this.ItineraryGross1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.SupplierPurchases_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.SupplierPurchases_FetchData);
			this.ReportStart += new System.EventHandler(this.SupplierPurchases_ReportStart);
			this.ReportEnd += new System.EventHandler(this.SupplierPurchases_ReportEnd);
		}

		#endregion
	}
}
