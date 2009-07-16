using System;
using System.Data.SqlClient;
using DataDynamics.ActiveReports;

namespace TourWriter.Reports.General
{
	public class ItineraryYield : ActiveReport
	{
		public enum GroupBy
		{
			AssignedTo = 0,
			Department = 1,
			Branch = 2
		}
		private DateTime dateFrom;
		private DateTime dateTo;
		private int groupByIndex;
		private SqlDataReader dataReader;

		public ItineraryYield(DateTime dateFrom, DateTime dateTo, string itineraryStatusList, string userList, GroupBy groupBy, string branchList, string departmentList)
		{
            SetLicense(Lookup.Lic);
            InitializeReport();

			this.dateFrom = dateFrom;
			this.dateTo = dateTo;
			SetGroupByIndex(groupBy);
			
			TourWriter.BusinessLogic.Reports r = new TourWriter.BusinessLogic.Reports();
			dataReader = r.ItineraryYield(
				dateFrom, dateTo, itineraryStatusList, userList, branchList, departmentList);

			this.Document.Name = String.Format("Itineraries Yield: {0} to {1}", 
				dateFrom.ToShortDateString(), dateTo.ToShortDateString());
		}

		private void SetGroupByIndex(GroupBy groupBy)
		{
			switch(groupBy)
			{
				case GroupBy.AssignedTo :
					groupByIndex = 3;
					break;
				case GroupBy.Department :
					groupByIndex = 4;
					break;
				case GroupBy.Branch :
					groupByIndex = 5;
					break;
			}
		}

		
		private void SupplierPurchases_ReportStart(object sender, System.EventArgs eArgs)
		{
		}

		private void SupplierPurchases_DataInitialize(object sender, System.EventArgs eArgs)
		{
			Fields.Add("Title");
			Fields.Add("GroupingName");

			Fields.Add("GrossSum");	
			Fields.Add("YieldAvg");	
			Fields.Add("MarginSum");	
			
			Fields.Add("GrossSumTotal");	
			Fields.Add("YieldAvgTotal");	
			Fields.Add("MarginSumTotal");	

			Fields.Add("Date");
			Fields.Add("ItineraryName");
			Fields.Add("Status");
			Fields.Add("Gross");
			Fields.Add("Yield");
			Fields.Add("Margin");		
		}
		
		private void SupplierPurchases_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
			try
			{
				if(dataReader.Read())
				{
					Fields["Title"].Value = this.Document.Name;
					
					// DataReader results:			
					// ArriveDate,ItineraryName,ItineraryStatusName,UserName,DepartmentName,BranchName,Gross,Net
				
					string groupingName = dataReader.GetString(groupByIndex);
					decimal nett   = dataReader.GetDecimal(6);
					decimal gross  = dataReader.GetDecimal(7);
					decimal yield  = (gross > 0) ? 100 * ((gross-nett)/gross) : 0;
					decimal margin = gross - nett;

					Fields["GroupingName"].Value	= groupingName.Length>0 ? groupingName : "<unassigned>";
					Fields["Date"].Value			= dataReader.GetDateTime(0).ToShortDateString();
					Fields["ItineraryName"].Value   = dataReader.GetString(1);
					Fields["Status"].Value			= dataReader.GetString(2);
					Fields["Gross"].Value			= gross;
					Fields["Yield"].Value			= yield;
					Fields["Margin"].Value			= margin;

					Fields["GrossSum"].Value		= gross;
					Fields["YieldAvg"].Value		= yield;
					Fields["MarginSum"].Value		= margin;
					
					Fields["GrossSumTotal"].Value	= gross;
					Fields["YieldAvgTotal"].Value	= yield;
					Fields["MarginSumTotal"].Value	= margin;

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
		private DataDynamics.ActiveReports.TextBox TextBox = null;
		private DataDynamics.ActiveReports.TextBox TextBox3 = null;
		private DataDynamics.ActiveReports.TextBox TextBox4 = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.PageHeader PageHeader = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.Line Line1 = null;
		private DataDynamics.ActiveReports.Label Label6 = null;
		private DataDynamics.ActiveReports.Label Label7 = null;
		private DataDynamics.ActiveReports.Label Label5 = null;
		private DataDynamics.ActiveReports.Label Label8 = null;
		private DataDynamics.ActiveReports.GroupHeader ghGroup = null;
		private DataDynamics.ActiveReports.Shape Shape1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox5 = null;
		private DataDynamics.ActiveReports.TextBox TextBox6 = null;
		private DataDynamics.ActiveReports.TextBox TotalGross1 = null;
		private DataDynamics.ActiveReports.TextBox GrossSum1 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox2 = null;
		private DataDynamics.ActiveReports.TextBox SupplierTotal1 = null;
		private DataDynamics.ActiveReports.TextBox SupplierTotal2 = null;
		private DataDynamics.ActiveReports.TextBox ItineraryGross1 = null;
		private DataDynamics.ActiveReports.TextBox Yield1 = null;
		private DataDynamics.ActiveReports.GroupFooter gfSupplier = null;
		private DataDynamics.ActiveReports.PageFooter PageFooter = null;
		private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.General.ItineraryYield.rpx");
			this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
			this.PageHeader = ((DataDynamics.ActiveReports.PageHeader)(this.Sections["PageHeader"]));
			this.ghGroup = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghGroup"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.gfSupplier = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfSupplier"]));
			this.PageFooter = ((DataDynamics.ActiveReports.PageFooter)(this.Sections["PageFooter"]));
			this.ReportFooter = ((DataDynamics.ActiveReports.ReportFooter)(this.Sections["ReportFooter"]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[0]));
			this.TextBox = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[1]));
			this.TextBox3 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[2]));
			this.TextBox4 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[3]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[4]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[0]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[1]));
			this.Line1 = ((DataDynamics.ActiveReports.Line)(this.PageHeader.Controls[2]));
			this.Label6 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[3]));
			this.Label7 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[4]));
			this.Label5 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[5]));
			this.Label8 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[6]));
			this.Shape1 = ((DataDynamics.ActiveReports.Shape)(this.ghGroup.Controls[0]));
			this.TextBox5 = ((DataDynamics.ActiveReports.TextBox)(this.ghGroup.Controls[1]));
			this.TextBox6 = ((DataDynamics.ActiveReports.TextBox)(this.ghGroup.Controls[2]));
			this.TotalGross1 = ((DataDynamics.ActiveReports.TextBox)(this.ghGroup.Controls[3]));
			this.GrossSum1 = ((DataDynamics.ActiveReports.TextBox)(this.ghGroup.Controls[4]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.SupplierTotal1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.SupplierTotal2 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			this.ItineraryGross1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
			this.Yield1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[5]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.SupplierPurchases_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.SupplierPurchases_FetchData);
			this.ReportStart += new System.EventHandler(this.SupplierPurchases_ReportStart);
			this.ReportEnd += new System.EventHandler(this.SupplierPurchases_ReportEnd);
		}

		#endregion
	}
}
