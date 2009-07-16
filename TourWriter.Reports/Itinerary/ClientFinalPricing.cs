using System;
using System.Data;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;
using TourWriter.Info;

namespace TourWriter.Reports.Itinerary
{
	public class ClientFinalPricing : ActiveReport
    {
		private int rowCount;
        private bool showClientPayments;
		private string additionalText;
		private bool showGST;
		private ToolSet toolSet;
		private AgentSet agentSet;
		private ItinerarySet itinerarySet;
		private string listServiceTypes;
		private ItinerarySet.PurchaseItemRow[] purchaseItemArray;

		public ClientFinalPricing(ItinerarySet itinerarySet, ToolSet toolSet,
            AgentSet agentSet, string additionalText, bool showGST, string listServiceTypes, 
		                          bool showClientPayments)
		{
            SetLicense(Lookup.Lic);
            InitializeReport();

		    totalCosts = itinerarySet.GetGrossFinalPrice();

			this.itinerarySet = itinerarySet;
			this.toolSet = toolSet;
			this.agentSet = agentSet;
			this.additionalText = additionalText;
			this.showGST = showGST;
			this.listServiceTypes = listServiceTypes;
            this.showClientPayments = showClientPayments;

			string filter = "ServiceTypeID IN (" + listServiceTypes + ")";
			string sort   = "ServiceTypeID, StartDate";
			purchaseItemArray = (ItinerarySet.PurchaseItemRow[])
				itinerarySet.PurchaseItem.Select(filter, sort);
			
			Document.Name = String.Format("Itinerary Summary for {0}",
                itinerarySet.Itinerary[0].ItineraryName);
		}
        
		private void ClientFinalPricing_DataInitialize(object sender, System.EventArgs eArgs)
		{
			// report data fields
			Fields.Add("Itinerary");
			Fields.Add("Country");
			Fields.Add("ArriveCity");
			Fields.Add("DepartCity");
			Fields.Add("ArriveDate");
			Fields.Add("DepartDate");
			Fields.Add("Length");

			Fields.Add("ServiceTypeID");
			Fields.Add("ServiceTypeName");
			Fields.Add("Date");
			Fields.Add("Days");
			Fields.Add("Qty");
			Fields.Add("Location");
			Fields.Add("Description");

            Fields.Add("TotalCost");
            Fields.Add("TotalDue");				
			Fields.Add("AdditionalCosts");
			Fields.Add("Notes");

			this.txtGST.Visible = showGST;
			this.Label12.Visible = showGST;			
		}

		private void ClientFinalPricing_ReportStart(object sender, System.EventArgs eArgs)
		{
			rowCount = 0;
		}

		private void ClientFinalPricing_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
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

                    Fields["TotalCost"].Value = totalCosts;
					
					Fields["AdditionalCosts"].Value = !itin.IsPricingNoteNull() ? itin.PricingNote : "";
					Fields["Notes"].Value = BuildNotes();
				}

				if(rowCount < this.purchaseItemArray.Length)
				{
					// get data
					ItinerarySet.PurchaseItemRow item = purchaseItemArray[rowCount++] as ItinerarySet.PurchaseItemRow;
					ItinerarySet.PurchaseLineRow line = itinerarySet.PurchaseLine.FindByPurchaseLineID(item.PurchaseLineID);

					// header
					Fields["ServiceTypeID"].Value = item.ServiceTypeID.ToString();
					Fields["ServiceTypeName"].Value   = Lookup.GetServiceTypeName(item.ServiceTypeID, toolSet);

					// detail
					Fields["Date"].Value		= !item.IsStartDateNull() ? item.StartDate.ToShortDateString() :"";
					Fields["Days"].Value		= !item.IsNumberOfDaysNull() ? item.NumberOfDays.ToString() :"";
					Fields["Qty"].Value			= !item.IsQuantityNull() ? item.Quantity.ToString() :"";
					Fields["Location"].Value	= Lookup.GetSupplierCityName(line.SupplierID, itinerarySet, toolSet);
					Fields["Description"].Value = itinerarySet.GetPurchaseItemFullName(line, item); 

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
            if (!Lookup.ShowServiceTypeOnReport(Fields["ServiceTypeID"].Value, listServiceTypes))
                LayoutAction = DataDynamics.ActiveReports.LayoutAction.NextRecord;
        }
		
	    private void Detail_Format(object sender, System.EventArgs eArgs)
	    {
	        if(!Lookup.ShowServiceTypeOnReport(Fields["ServiceTypeID"].Value, listServiceTypes))
	            this.LayoutAction = DataDynamics.ActiveReports.LayoutAction.NextRecord;
	    }

	    private void gfAdditionalCosts_Format(object sender, EventArgs e)
	    {
	        gfAdditionalCosts.Visible = txtAdditionalCosts.Text.Trim().Length > 0;
	    }

	    private void gfNotes_Format(object sender, EventArgs e)
	    {
	        gfNotes.Visible = txtNotes.Text.Trim().Length > 0;	
	    }

	    private string BuildNotes()
	    {
	        string s = "";
			
	        s += additionalText != "" ? additionalText + Environment.NewLine : "";

            if (agentSet != null && agentSet.Agent.Count > 0)
	        {
	            s += !agentSet.Agent[0].IsClientFooterNull() ? 
	                 agentSet.Agent[0].ClientFooter + Environment.NewLine : "";
	        }
	        return s;
	    }




	    private decimal totalCosts = 0;
	    private decimal totalPayments = 0;
	    private void GroupFooterTotalCost_Format(object sender, System.EventArgs eArgs)
	    {
	        if (showClientPayments)
	        {
	            txtTotalCost.Value = totalCosts;
	        }
	        else
	            GroupFooterTotalCost.Visible = false;
	    }

	    private void GroupFooterTotalPayments_Format(object sender, System.EventArgs eArgs)
	    {
	        if (showClientPayments)
	        {
	            ClientPayments payments = new ClientPayments(itinerarySet, toolSet);
	            SubReport.Report = payments;
	            totalPayments = payments.GetTotalPayments();
	        }
	        else
	            GroupFooterTotalPayments.Visible = false;
	    }

	    private void GroupFooterTotalDue_Format(object sender, System.EventArgs eArgs)
	    {
	        decimal totalDue = 0;

	        if (showClientPayments)
	        {
	            totalDue = totalCosts - totalPayments;
	        }
	        else
	        {
	            totalDue = totalCosts;
	        }

	        txtTotalDue.Value = totalDue;
	        txtGST.Value = totalDue/9; // TODO remove hardcoded tax amount
	    }


	    #region ActiveReports Designer generated code
	    private DataDynamics.ActiveReports.ReportHeader ReportHeader = null;
	    private DataDynamics.ActiveReports.Shape Shape1 = null;
	    private DataDynamics.ActiveReports.TextBox Itinerary1 = null;
	    private DataDynamics.ActiveReports.Label Label1 = null;
	    private DataDynamics.ActiveReports.Label Label3 = null;
	    private DataDynamics.ActiveReports.TextBox Country1 = null;
	    private DataDynamics.ActiveReports.TextBox TextBox3 = null;
	    private DataDynamics.ActiveReports.TextBox TextBox4 = null;
	    private DataDynamics.ActiveReports.Label Label2 = null;
	    private DataDynamics.ActiveReports.Label Label5 = null;
	    private DataDynamics.ActiveReports.Label Label4 = null;
	    private DataDynamics.ActiveReports.TextBox Days1 = null;
	    private DataDynamics.ActiveReports.PageHeader PageHeader = null;
	    private DataDynamics.ActiveReports.Label Label6 = null;
	    private DataDynamics.ActiveReports.Label Label10 = null;
	    private DataDynamics.ActiveReports.Line Line1 = null;
	    private DataDynamics.ActiveReports.Label Label7 = null;
	    private DataDynamics.ActiveReports.Label Label8 = null;
	    private DataDynamics.ActiveReports.Label Label9 = null;
	    private DataDynamics.ActiveReports.GroupHeader GroupHeader1 = null;
	    private DataDynamics.ActiveReports.GroupHeader GroupHeader2 = null;
	    private DataDynamics.ActiveReports.GroupHeader GroupHeader3 = null;
	    private DataDynamics.ActiveReports.GroupHeader GroupHeader4 = null;
	    private DataDynamics.ActiveReports.GroupHeader GroupHeader6 = null;
	    private DataDynamics.ActiveReports.GroupHeader ghServiceType = null;
	    private DataDynamics.ActiveReports.Label txtServiceType = null;
	    private DataDynamics.ActiveReports.Detail Detail = null;
	    private DataDynamics.ActiveReports.TextBox txtDate = null;
	    private DataDynamics.ActiveReports.TextBox txtDescription = null;
	    private DataDynamics.ActiveReports.TextBox txtCity = null;
	    private DataDynamics.ActiveReports.TextBox txtDays = null;
	    private DataDynamics.ActiveReports.TextBox txtQty = null;
	    private DataDynamics.ActiveReports.GroupFooter gfServiceType = null;
	    private DataDynamics.ActiveReports.GroupFooter GroupFooterTotalCost = null;
	    private DataDynamics.ActiveReports.Label Label16 = null;
	    private DataDynamics.ActiveReports.TextBox txtTotalCost = null;
	    private DataDynamics.ActiveReports.Label Label18 = null;
	    private DataDynamics.ActiveReports.GroupFooter GroupFooterTotalPayments = null;
	    private DataDynamics.ActiveReports.Label Label = null;
	    private DataDynamics.ActiveReports.SubReport SubReport = null;
	    private DataDynamics.ActiveReports.GroupFooter GroupFooterTotalDue = null;
	    private DataDynamics.ActiveReports.Label Label11 = null;
	    private DataDynamics.ActiveReports.TextBox txtTotalDue = null;
	    private DataDynamics.ActiveReports.Label Label12 = null;
	    private DataDynamics.ActiveReports.TextBox txtGST = null;
	    private DataDynamics.ActiveReports.Label Label15 = null;
	    private DataDynamics.ActiveReports.GroupFooter gfAdditionalCosts = null;
	    private DataDynamics.ActiveReports.Label Label13 = null;
	    private DataDynamics.ActiveReports.TextBox txtAdditionalCosts = null;
	    private DataDynamics.ActiveReports.GroupFooter gfNotes = null;
	    private DataDynamics.ActiveReports.Label Label14 = null;
	    private DataDynamics.ActiveReports.TextBox txtNotes = null;
	    private DataDynamics.ActiveReports.PageFooter PageFooter = null;
	    private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
	    public void InitializeReport()
	    {
	        this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.ClientFinalPricing.rpx");
	        this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
	        this.PageHeader = ((DataDynamics.ActiveReports.PageHeader)(this.Sections["PageHeader"]));
	        this.GroupHeader1 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader1"]));
	        this.GroupHeader2 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader2"]));
	        this.GroupHeader3 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader3"]));
	        this.GroupHeader4 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader4"]));
	        this.GroupHeader6 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader6"]));
	        this.ghServiceType = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghServiceType"]));
	        this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
	        this.gfServiceType = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfServiceType"]));
	        this.GroupFooterTotalCost = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["GroupFooterTotalCost"]));
	        this.GroupFooterTotalPayments = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["GroupFooterTotalPayments"]));
	        this.GroupFooterTotalDue = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["GroupFooterTotalDue"]));
	        this.gfAdditionalCosts = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfAdditionalCosts"]));
	        this.gfNotes = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfNotes"]));
	        this.PageFooter = ((DataDynamics.ActiveReports.PageFooter)(this.Sections["PageFooter"]));
	        this.ReportFooter = ((DataDynamics.ActiveReports.ReportFooter)(this.Sections["ReportFooter"]));
	        this.Shape1 = ((DataDynamics.ActiveReports.Shape)(this.ReportHeader.Controls[0]));
	        this.Itinerary1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[1]));
	        this.Label1 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[2]));
	        this.Label3 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[3]));
	        this.Country1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[4]));
	        this.TextBox3 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[5]));
	        this.TextBox4 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[6]));
	        this.Label2 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[7]));
	        this.Label5 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[8]));
	        this.Label4 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[9]));
	        this.Days1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[10]));
	        this.Label6 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[0]));
	        this.Label10 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[1]));
	        this.Line1 = ((DataDynamics.ActiveReports.Line)(this.PageHeader.Controls[2]));
	        this.Label7 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[3]));
	        this.Label8 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[4]));
	        this.Label9 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[5]));
	        this.txtServiceType = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[0]));
	        this.txtDate = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
	        this.txtDescription = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
	        this.txtCity = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
	        this.txtDays = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
	        this.txtQty = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
	        this.Label16 = ((DataDynamics.ActiveReports.Label)(this.GroupFooterTotalCost.Controls[0]));
	        this.txtTotalCost = ((DataDynamics.ActiveReports.TextBox)(this.GroupFooterTotalCost.Controls[1]));
	        this.Label18 = ((DataDynamics.ActiveReports.Label)(this.GroupFooterTotalCost.Controls[2]));
	        this.Label = ((DataDynamics.ActiveReports.Label)(this.GroupFooterTotalPayments.Controls[0]));
	        this.SubReport = ((DataDynamics.ActiveReports.SubReport)(this.GroupFooterTotalPayments.Controls[1]));
	        this.Label11 = ((DataDynamics.ActiveReports.Label)(this.GroupFooterTotalDue.Controls[0]));
	        this.txtTotalDue = ((DataDynamics.ActiveReports.TextBox)(this.GroupFooterTotalDue.Controls[1]));
	        this.Label12 = ((DataDynamics.ActiveReports.Label)(this.GroupFooterTotalDue.Controls[2]));
	        this.txtGST = ((DataDynamics.ActiveReports.TextBox)(this.GroupFooterTotalDue.Controls[3]));
	        this.Label15 = ((DataDynamics.ActiveReports.Label)(this.GroupFooterTotalDue.Controls[4]));
	        this.Label13 = ((DataDynamics.ActiveReports.Label)(this.gfAdditionalCosts.Controls[0]));
	        this.txtAdditionalCosts = ((DataDynamics.ActiveReports.TextBox)(this.gfAdditionalCosts.Controls[1]));
	        this.Label14 = ((DataDynamics.ActiveReports.Label)(this.gfNotes.Controls[0]));
	        this.txtNotes = ((DataDynamics.ActiveReports.TextBox)(this.gfNotes.Controls[1]));
	        // Attach Report Events
	        this.ReportStart += new System.EventHandler(this.ClientFinalPricing_ReportStart);
	        this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.ClientFinalPricing_FetchData);
	        this.DataInitialize += new System.EventHandler(this.ClientFinalPricing_DataInitialize);
	        this.gfAdditionalCosts.Format += new System.EventHandler(this.gfAdditionalCosts_Format);
	        this.gfNotes.Format += new System.EventHandler(this.gfNotes_Format);
	        this.Detail.Format += new System.EventHandler(this.Detail_Format);
	        this.ghServiceType.Format += new System.EventHandler(this.ghServiceType_Format);
	        this.GroupFooterTotalCost.Format += new System.EventHandler(this.GroupFooterTotalCost_Format);
	        this.GroupFooterTotalPayments.Format += new System.EventHandler(this.GroupFooterTotalPayments_Format);
	        this.GroupFooterTotalDue.Format += new System.EventHandler(this.GroupFooterTotalDue_Format);
	    }

	    #endregion

    }
}
