using System;
using System.IO;
using System.Data;
using TourWriter.Info;
using DataDynamics.ActiveReports;

namespace TourWriter.Reports.Itinerary
{
	public class SupplierMessage : ActiveReport
	{
		public enum ReportType
		{
			Request = 0,
			Confirmation = 1,
			Remittance = 2
		}
	    
        private ReportType reportType;
		private int rowCount;
		private string additionalText;
		private int purchaseLineID;
		private UserSet userSet;
		private AgentSet agentSet;
		private ItinerarySet itinerarySet;
	    private ToolSet toolSet;
		private ItinerarySet.PurchaseItemRow[] itemRows;
		
		private string GetTitle()
		{
			switch(reportType)
			{
				case ReportType.Request:
					return "Booking Request for {0}";
				case ReportType.Confirmation:
					return "Booking Confirmation for {0}";
				case ReportType.Remittance:
					return "Booking Invoice Remittance for {0}";
				default:
					return "";
			}
		}


		public SupplierMessage(ReportType reportType, int purchaseLineID, ItinerarySet itinerarySet, 
			AgentSet agentSet, UserSet userSet, string additionalText, ToolSet toolSet)
        {
            SetLicense(Lookup.Lic);
			InitializeReport();

            this.reportType = reportType;
			this.purchaseLineID = purchaseLineID;
			this.userSet = userSet;
			this.agentSet = agentSet;
			this.itinerarySet = itinerarySet;
			this.additionalText = additionalText;
		    this.toolSet = toolSet;

            Document.Name = String.Format(GetTitle(), itinerarySet.Itinerary[0].ItineraryName);

			itemRows = (ItinerarySet.PurchaseItemRow[])
				itinerarySet.PurchaseItem.Select("PurchaseLineID = " + purchaseLineID.ToString());
		}


		private void SupplierMessage_DataInitialize(object sender, System.EventArgs eArgs)
		{
			rowCount = 0;

			Fields.Add("TodayDate");
			Fields.Add("SupplierInfo");
			Fields.Add("AgentInfo");
			Fields.Add("AgentLogo");
			Fields.Add("Title");

			Fields.Add("BookingName");
			Fields.Add("SupplierName");
			Fields.Add("AttentionTo");
			Fields.Add("Order");
			Fields.Add("User");
			Fields.Add("Notes");
			Fields.Add("Footer");

			Fields.Add("Date");
			Fields.Add("Description");
			Fields.Add("Reference");
			Fields.Add("Price");
			Fields.Add("Comm");
			Fields.Add("Qty");
			Fields.Add("Days");
			Fields.Add("ItemTotal");
			Fields.Add("LineTotal");
		}

		private void SupplierMessage_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
			try
			{
				// document header
				if(rowCount == 0)
				{
					ItinerarySet.PurchaseLineRow line = itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineID);
					ItinerarySet.SupplierLookupRow supplier = itinerarySet.SupplierLookup.FindBySupplierID(line.SupplierID);
					
					Fields["TodayDate"].Value = DateTime.Now.ToLongDateString();
					Fields["SupplierInfo"].Value = (supplier != null && !supplier.IsPostAddressNull()) ? supplier.PostAddress : "";;
                    if (agentSet != null && agentSet.Agent.Count > 0)
						Fields["AgentInfo"].Value = !agentSet.Agent[0].IsAgentHeaderNull() ? agentSet.Agent[0].AgentHeader : "";
					
                    // header details					
					Fields["Title"].Value = this.Document.Name;
                    Fields["BookingName"].Value = itinerarySet.Itinerary[0].GetDisplayNameOrItineraryName();
					Fields["SupplierName"].Value = (supplier != null) ? supplier.SupplierName : "";
					Fields["AttentionTo"].Value = line.IsSupplierReferenceNull() ? "" : line.SupplierReference;
					Fields["Order"].Value = purchaseLineID.ToString();

				    var user = toolSet.User.FindByUserID(itinerarySet.Itinerary[0].AssignedTo);
					Fields["User"].Value = !user.IsEmailNull() ? user.Email : user.UserName;

					Fields["Notes"].Value = this.GetNotes(line);
					Fields["Footer"].Value = (agentSet != null && agentSet.Agent.Count > 0) ? 
                        this.GetFooter(agentSet.Agent[0]) : "";
				}
				// document content
				if(rowCount < this.itemRows.Length)
				{
					ItinerarySet.PurchaseItemRow item = itemRows[rowCount++];
					
					Fields["Date"].Value = !item.IsStartDateNull() ? item.StartDate.ToShortDateString() : "";
					Fields["Description"].Value = !item.IsPurchaseItemNameNull() ? item.PurchaseItemName : "";
					Fields["Reference"].Value = !item.IsBookingReferenceNull() ? item.BookingReference :"";					
					Fields["Qty"].Value = !item.IsQuantityNull() ? item.Quantity.ToString() : "";
					Fields["Days"].Value = !item.IsNumberOfDaysNull() ? item.NumberOfDays.ToString() : "";

					ItinerarySet.OptionLookupRow lookup = itinerarySet.OptionLookup.FindByOptionID(item.OptionID);			
					
					// pricing calcs
					if(lookup != null)
					{
                        decimal net = !item.IsNetNull() ? item.Net : 0;
						
						if(lookup.PricingOption == "gc") // GrossCommission
						{
							// Earning commission on gross, so show commission rate.
                            decimal gross = !item.IsGrossNull() ? item.Gross : 0;
							decimal comm = itinerarySet.GetCommission(net, gross);
							Fields["Comm"].Value = comm.ToString();
                            Fields["Price"].Value =
                                (!item.IsCurrencyCodeNull() ? item.CurrencyCode + " " : "") +
                                gross.ToString("###,##0.00");
						}
						else
						{
							// Marking up net, so don't show effective commission.
							Fields["Comm"].Value = "";
                            Fields["Price"].Value =
                                (!item.IsCurrencyCodeNull() ? item.CurrencyCode + " " : "") +
                                net.ToString("###,##0.00");
						}

                        Fields["ItemTotal"].Value = 
                            (!item.IsCurrencyCodeNull() ? item.CurrencyCode + " " : "") +
                            item.NetTotal.ToString("###,##0.00");
                        Fields["LineTotal"].Value = item.NetTotal;
					}
					eArgs.EOF = false;
				}
			}
			catch(Exception)
			{
				eArgs.EOF = true;
				throw;
			}
		}

		private string GetNotes(ItinerarySet.PurchaseLineRow line)
		{			
			// purchase line notes
			string notes = !line.IsNoteToSupplierNull() ? line.NoteToSupplier + Environment.NewLine : "";

			// clients notes
			foreach(ItinerarySet.ItineraryGroupRow grp in itinerarySet.ItineraryGroup)
				if(grp.RowState != DataRowState.Deleted)
					notes += !grp.IsNoteToSupplierNull() ? grp.NoteToSupplier + Environment.NewLine : "";

			// additional notes
			notes += this.additionalText;

			// set visibility
			Label16.Visible = (notes.Trim() != "");
			txtNotes.Visible = (notes.Trim() != "");

			return notes;
		}

        private string GetFooter(AgentSet.AgentRow agent)
		{
            switch (reportType)
			{
                case ReportType.Request:
					if(!agent.IsRequestFooterNull())
                        return agent.RequestFooter;
					break;
                case ReportType.Confirmation:
					if(!agent.IsConfirmFooterNull())
                        return agent.ConfirmFooter;
					break;
                case ReportType.Remittance:
					if(!agent.IsRemitFooterNull())
                        return agent.RemitFooter;
                    break;
                default:
                    return "";
			}
            return "";
		}
        
		private void Detail_Format(object sender, EventArgs eArgs)
		{
            if (agentSet != null && agentSet.Agent.Count > 0 && !agentSet.Agent[0].IsLogoFileNull())
			{							
				FileInfo f = new FileInfo(Lookup.ConvertToFullPath(
                    agentSet.Agent[0].LogoFile, toolSet.AppSettings[0].ExternalFilesPath));

				if(f.Exists)
				{
					try
					{
						picLogo.Image = System.Drawing.Image.FromFile(f.FullName);
					}
					catch(OutOfMemoryException ex)
					{
						throw new FileLoadException(
							"Invalid file format for file " + f.FullName, ex);
					}
				}
			}
			
		}

		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.PageHeader PageHeader = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.Picture picLogo = null;
		private DataDynamics.ActiveReports.TextBox AgentInfo1 = null;
		private DataDynamics.ActiveReports.GroupHeader ghDetails = null;
		private DataDynamics.ActiveReports.Label Label = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.TextBox txtBookingName = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.TextBox txtOrderNumber = null;
		private DataDynamics.ActiveReports.Label Label6 = null;
		private DataDynamics.ActiveReports.TextBox txtUsername = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.TextBox txtSupplierName = null;
		private DataDynamics.ActiveReports.Label Label5 = null;
		private DataDynamics.ActiveReports.TextBox txtReference = null;
		private DataDynamics.ActiveReports.GroupHeader GroupHeader1 = null;
		private DataDynamics.ActiveReports.GroupHeader ghItems = null;
		private DataDynamics.ActiveReports.Label Label14 = null;
		private DataDynamics.ActiveReports.Label Label13 = null;
		private DataDynamics.ActiveReports.Label Label12 = null;
		private DataDynamics.ActiveReports.Label Label11 = null;
		private DataDynamics.ActiveReports.Label Label10 = null;
		private DataDynamics.ActiveReports.Label Label8 = null;
		private DataDynamics.ActiveReports.Label Label7 = null;
		private DataDynamics.ActiveReports.Label Label9 = null;
		private DataDynamics.ActiveReports.Line Line5 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox txtName = null;
		private DataDynamics.ActiveReports.TextBox txtQty = null;
		private DataDynamics.ActiveReports.TextBox txtDays = null;
		private DataDynamics.ActiveReports.TextBox txtDate = null;
		private DataDynamics.ActiveReports.TextBox txtPrice = null;
		private DataDynamics.ActiveReports.TextBox txtItemTotal = null;
		private DataDynamics.ActiveReports.TextBox txtComm = null;
		private DataDynamics.ActiveReports.TextBox Description1 = null;
		private DataDynamics.ActiveReports.GroupFooter gfItems = null;
		private DataDynamics.ActiveReports.TextBox txtLineTotal = null;
		private DataDynamics.ActiveReports.Label Label15 = null;
		private DataDynamics.ActiveReports.Line Line3 = null;
		private DataDynamics.ActiveReports.Line Line4 = null;
		private DataDynamics.ActiveReports.GroupFooter GroupFooter2 = null;
		private DataDynamics.ActiveReports.Label Label16 = null;
		private DataDynamics.ActiveReports.TextBox txtNotes = null;
		private DataDynamics.ActiveReports.GroupFooter GroupFooter1 = null;
		private DataDynamics.ActiveReports.TextBox txtFooter = null;
		private DataDynamics.ActiveReports.PageFooter PageFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.SupplierMessage.rpx");
			this.PageHeader = ((DataDynamics.ActiveReports.PageHeader)(this.Sections["PageHeader"]));
			this.ghDetails = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghDetails"]));
			this.GroupHeader1 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader1"]));
			this.ghItems = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghItems"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.gfItems = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfItems"]));
			this.GroupFooter2 = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["GroupFooter2"]));
			this.GroupFooter1 = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["GroupFooter1"]));
			this.PageFooter = ((DataDynamics.ActiveReports.PageFooter)(this.Sections["PageFooter"]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.PageHeader.Controls[0]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.PageHeader.Controls[1]));
			this.picLogo = ((DataDynamics.ActiveReports.Picture)(this.PageHeader.Controls[2]));
			this.AgentInfo1 = ((DataDynamics.ActiveReports.TextBox)(this.PageHeader.Controls[3]));
			this.Label = ((DataDynamics.ActiveReports.Label)(this.ghDetails.Controls[0]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.ghDetails.Controls[1]));
			this.txtBookingName = ((DataDynamics.ActiveReports.TextBox)(this.ghDetails.Controls[2]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.ghDetails.Controls[3]));
			this.txtOrderNumber = ((DataDynamics.ActiveReports.TextBox)(this.ghDetails.Controls[4]));
			this.Label6 = ((DataDynamics.ActiveReports.Label)(this.ghDetails.Controls[5]));
			this.txtUsername = ((DataDynamics.ActiveReports.TextBox)(this.ghDetails.Controls[6]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.ghDetails.Controls[7]));
			this.txtSupplierName = ((DataDynamics.ActiveReports.TextBox)(this.ghDetails.Controls[8]));
			this.Label5 = ((DataDynamics.ActiveReports.Label)(this.ghDetails.Controls[9]));
			this.txtReference = ((DataDynamics.ActiveReports.TextBox)(this.ghDetails.Controls[10]));
			this.Label14 = ((DataDynamics.ActiveReports.Label)(this.ghItems.Controls[0]));
			this.Label13 = ((DataDynamics.ActiveReports.Label)(this.ghItems.Controls[1]));
			this.Label12 = ((DataDynamics.ActiveReports.Label)(this.ghItems.Controls[2]));
			this.Label11 = ((DataDynamics.ActiveReports.Label)(this.ghItems.Controls[3]));
			this.Label10 = ((DataDynamics.ActiveReports.Label)(this.ghItems.Controls[4]));
			this.Label8 = ((DataDynamics.ActiveReports.Label)(this.ghItems.Controls[5]));
			this.Label7 = ((DataDynamics.ActiveReports.Label)(this.ghItems.Controls[6]));
			this.Label9 = ((DataDynamics.ActiveReports.Label)(this.ghItems.Controls[7]));
			this.Line5 = ((DataDynamics.ActiveReports.Line)(this.ghItems.Controls[8]));
			this.txtName = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.txtQty = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.txtDays = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.txtDate = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			this.txtPrice = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
			this.txtItemTotal = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[5]));
			this.txtComm = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[6]));
			this.Description1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[7]));
			this.txtLineTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfItems.Controls[0]));
			this.Label15 = ((DataDynamics.ActiveReports.Label)(this.gfItems.Controls[1]));
			this.Line3 = ((DataDynamics.ActiveReports.Line)(this.gfItems.Controls[2]));
			this.Line4 = ((DataDynamics.ActiveReports.Line)(this.gfItems.Controls[3]));
			this.Label16 = ((DataDynamics.ActiveReports.Label)(this.GroupFooter2.Controls[0]));
			this.txtNotes = ((DataDynamics.ActiveReports.TextBox)(this.GroupFooter2.Controls[1]));
			this.txtFooter = ((DataDynamics.ActiveReports.TextBox)(this.GroupFooter1.Controls[0]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.SupplierMessage_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.SupplierMessage_FetchData);
			this.Detail.Format += new System.EventHandler(this.Detail_Format);
		}

		#endregion
	}
}
