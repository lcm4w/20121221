using System;
using System.Data;
using TourWriter.Info;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;

namespace TourWriter.Reports.Itinerary
{
	public class ClientNoPricing : ActiveReport
	{
		private int rowCount;
		private string additionalText;
		private ToolSet toolSet;
		private AgentSet agentSet;
		private ItinerarySet itinerarySet;
		private string listServiceTypes;
		private ItinerarySet.PurchaseItemRow[] purchaseItemArray;

		public ClientNoPricing(ItinerarySet itinerarySet, ToolSet toolSet, 
			AgentSet agentSet, string additionalText, string listServiceTypes)
		{
            SetLicense(Lookup.Lic);
            InitializeReport();

			this.Document.Name = "Itinerary Summary";
			this.itinerarySet = itinerarySet;
			this.toolSet = toolSet;
			this.agentSet = agentSet;
			this.additionalText = additionalText;
			this.listServiceTypes = listServiceTypes;

			string filter = "ServiceTypeID IN (" + listServiceTypes + ")";
			string sort   = "ServiceTypeID, StartDate";
			purchaseItemArray = (ItinerarySet.PurchaseItemRow[])
				itinerarySet.PurchaseItem.Select(filter, sort);
			
			this.Document.Name = String.Format("Itinerary Summary for {0}",
                itinerarySet.Itinerary[0].ItineraryName);
		}


		private void ClientNoPricing_DataInitialize(object sender, System.EventArgs eArgs)
		{
			Fields.Add("Itinerary");
			Fields.Add("Country");
			Fields.Add("ArriveCity");
			Fields.Add("DepartCity");
			Fields.Add("ArriveDate");
			Fields.Add("DepartDate");
			Fields.Add("Length");
			
			Fields.Add("ServiceTypeID");
			Fields.Add("ServiceTypeName");

			Fields.Add("InDate");
			Fields.Add("OutDate");
			Fields.Add("SupplierName");
			Fields.Add("SupplierAddress");
			Fields.Add("SupplierPhone");
			Fields.Add("SupplierFax");
			
			Fields.Add("Notes");
		}

		private void ClientNoPricing_ReportStart(object sender, System.EventArgs eArgs)
		{
			rowCount = 0;
		}

		private void ClientNoPricing_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{	
			try
			{
				if(rowCount == 0) // run these once
				{					
					ItinerarySet.ItineraryRow itin = itinerarySet.Itinerary[0];
					int days = itinerarySet.ItineraryLength();

                    Fields["Itinerary"].Value = itin.GetDisplayNameOrItineraryName();
					Fields["Length"].Value     = days>-1 ? days.ToString() + " days" : "";
					Fields["Country"].Value    = !itin.IsCountryIDNull() ? toolSet.Country.FindByCountryID(itin.CountryID).CountryName :"";
                    Fields["ArriveDate"].Value = !itin.IsArriveDateNull() ? itin.ArriveDate.ToString("dd MMM yyyy, hh:mm tt") : "(no date selected)";
                    Fields["DepartDate"].Value = !itin.IsDepartDateNull() ? itin.DepartDate.ToString("dd MMM yyyy, hh:mm tt") : "(no date selected)";
					Fields["ArriveDate"].Value += " " + Lookup.GetArriveCity(itin, toolSet);
					Fields["DepartDate"].Value += " " + Lookup.GetDepartCity(itin, toolSet);
					
					Fields["Notes"].Value = BuildNotes();
				}

				if(rowCount < this.purchaseItemArray.Length)
				{
					// get data
					ItinerarySet.PurchaseItemRow item = purchaseItemArray[rowCount++] as ItinerarySet.PurchaseItemRow;
					ItinerarySet.PurchaseLineRow line = itinerarySet.PurchaseLine.FindByPurchaseLineID(item.PurchaseLineID);
					ItinerarySet.SupplierLookupRow supplier = itinerarySet.SupplierLookup.FindBySupplierID(line.SupplierID);
					
					// group header
					Fields["ServiceTypeID"].Value   = item.ServiceTypeID.ToString();
					Fields["ServiceTypeName"].Value = Lookup.GetServiceTypeName(item.ServiceTypeID, toolSet);

					// detail
					Fields["InDate"].Value = !item.IsStartDateNull() ? item.StartDate.ToShortDateString() :"";
					Fields["OutDate"].Value = !item.IsStartDateNull() && !item.IsNumberOfDaysNull() ? 
												  item.StartDate.AddDays(item.NumberOfDays).ToShortDateString() : "";
					Fields["SupplierName"].Value = supplier.SupplierName;
					Fields["SupplierAddress"].Value	= BuildSupplierAddress(supplier);
					Fields["SupplierPhone"].Value = !supplier.IsPhoneNull() ? supplier.Phone.Trim() : "";
					Fields["SupplierFax"].Value = !supplier.IsFaxNull() ? supplier.Fax.Trim() : ""; 

					eArgs.EOF = false;	
				}
			}
			catch(Exception)	
			{
				eArgs.EOF = true;	
				throw;
			}
		}
		
	
		private void ghServiceType_Format(object sender, System.EventArgs eArgs)
		{			
			if(!Lookup.ShowServiceTypeOnReport(Fields["ServiceTypeID"].Value, listServiceTypes))
				this.LayoutAction = DataDynamics.ActiveReports.LayoutAction.NextRecord;
		}		
		
		private void Detail_Format(object sender, System.EventArgs eArgs)
		{
			if(!Lookup.ShowServiceTypeOnReport(Fields["ServiceTypeID"].Value, listServiceTypes))
				this.LayoutAction = DataDynamics.ActiveReports.LayoutAction.NextRecord;
			
		}

		private void ReportFooter_Format(object sender, System.EventArgs eArgs)
		{		
			ReportFooter.Visible = txtNotes.Text.Trim().Length > 0;
		}


		private string BuildSupplierAddress(ItinerarySet.SupplierLookupRow supplier)
		{
		    string street = "";
		    string city = "";
		    string region = "";
		    string state = "";

            if (!supplier.IsStreetAddressNull())
            {
                street = supplier.StreetAddress.Trim();
            } 
            if (!supplier.IsCityIDNull())
            {
                var row = toolSet.City.FindByCityID(supplier.CityID);
                if (row != null) city = row.CityName.Trim();
            }
            if (!supplier.IsRegionIDNull())
            {
                var row = toolSet.Region.FindByRegionID(supplier.RegionID);
                if (row != null) region = row.RegionName.Trim();
            }
            if (!supplier.IsStateIDNull())
            {
                var row = toolSet.State.FindByStateID(supplier.StateID);
                if (row != null) state = row.StateName.Trim();
            }
			
			// remove any repeated info
			if(city   == region) city   = "";
			if(region == state ) region = "";
										
			return
				(street != "" ? street + Environment.NewLine : "") +
				(city   != "" ? city   + Environment.NewLine : "") +
				(region != "" ? region + ", " + state : state);
		}
		
		private string BuildNotes()
		{
			string s = "";
			
			s += additionalText != "" ? additionalText + Environment.NewLine : "";
			
			return s;
		}

		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.ReportHeader ReportHeader = null;
		private DataDynamics.ActiveReports.Shape Shape1 = null;
		private DataDynamics.ActiveReports.TextBox Itinerary1 = null;
		private DataDynamics.ActiveReports.Label LabelA = null;
		private DataDynamics.ActiveReports.Label LabelC = null;
		private DataDynamics.ActiveReports.TextBox Country1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox3 = null;
		private DataDynamics.ActiveReports.TextBox TextBox4 = null;
		private DataDynamics.ActiveReports.Label LabelB = null;
		private DataDynamics.ActiveReports.Label LabelE = null;
		private DataDynamics.ActiveReports.Label LabelD = null;
		private DataDynamics.ActiveReports.TextBox Days1 = null;
		private DataDynamics.ActiveReports.PageHeader PageHeader = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.Label Label5 = null;
		private DataDynamics.ActiveReports.Line Line1 = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.Label Label = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.GroupHeader ghServiceType = null;
		private DataDynamics.ActiveReports.Label txtServiceType = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox txtDate = null;
		private DataDynamics.ActiveReports.TextBox txtDescription = null;
		private DataDynamics.ActiveReports.TextBox TextBox = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox2 = null;
		private DataDynamics.ActiveReports.TextBox TextBox5 = null;
		private DataDynamics.ActiveReports.GroupFooter gfNotes = null;
		private DataDynamics.ActiveReports.PageFooter PageFooter = null;
		private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
		private DataDynamics.ActiveReports.Label Label9 = null;
		private DataDynamics.ActiveReports.TextBox txtNotes = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.ClientNoPricing.rpx");
			this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
			this.PageHeader = ((DataDynamics.ActiveReports.PageHeader)(this.Sections["PageHeader"]));
			this.ghServiceType = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghServiceType"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.gfNotes = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfNotes"]));
			this.PageFooter = ((DataDynamics.ActiveReports.PageFooter)(this.Sections["PageFooter"]));
			this.ReportFooter = ((DataDynamics.ActiveReports.ReportFooter)(this.Sections["ReportFooter"]));
			this.Shape1 = ((DataDynamics.ActiveReports.Shape)(this.ReportHeader.Controls[0]));
			this.Itinerary1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[1]));
			this.LabelA = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[2]));
			this.LabelC = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[3]));
			this.Country1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[4]));
			this.TextBox3 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[5]));
			this.TextBox4 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[6]));
			this.LabelB = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[7]));
			this.LabelE = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[8]));
			this.LabelD = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[9]));
			this.Days1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[10]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[0]));
			this.Label5 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[1]));
			this.Line1 = ((DataDynamics.ActiveReports.Line)(this.PageHeader.Controls[2]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[3]));
			this.Label = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[4]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[5]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[6]));
			this.txtServiceType = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[0]));
			this.txtDate = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.txtDescription = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.TextBox = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
			this.TextBox5 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[5]));
			this.Label9 = ((DataDynamics.ActiveReports.Label)(this.ReportFooter.Controls[0]));
			this.txtNotes = ((DataDynamics.ActiveReports.TextBox)(this.ReportFooter.Controls[1]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.ClientNoPricing_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.ClientNoPricing_FetchData);
			this.ReportStart += new System.EventHandler(this.ClientNoPricing_ReportStart);
			this.ReportFooter.Format += new System.EventHandler(this.ReportFooter_Format);
			this.ghServiceType.Format += new System.EventHandler(this.ghServiceType_Format);
			this.Detail.Format += new System.EventHandler(this.Detail_Format);
		}

		#endregion
	}
}
