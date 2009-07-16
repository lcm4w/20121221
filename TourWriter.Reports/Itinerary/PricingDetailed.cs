using System;
using System.Data;
using TourWriter.Info;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;

namespace TourWriter.Reports.Itinerary
{
	public class PricingDetailed : ActiveReport
	{
		private int rowCount;
		private ToolSet toolSet;
		private ItinerarySet itinerarySet;
		private DataView itemsDataView;

		public PricingDetailed(ItinerarySet itinerarySet, ToolSet toolSet)
		{
            SetLicense(Lookup.Lic);
			InitializeReport();

			this.itinerarySet = itinerarySet;
			this.toolSet = toolSet;
			
			this.Document.Name = String.Format("Detailed Itinerary Costing for {0}",
                itinerarySet.Itinerary[0].ItineraryName);

			// sort items for servicetype grouping
			itinerarySet.PurchaseItem.DefaultView.Sort = "ServiceTypeID, StartDate";
			itemsDataView = itinerarySet.PurchaseItem.DefaultView;
		}


		private void PricingDetailed_DataInitialize(object sender, System.EventArgs eArgs)
		{
			rowCount = 0;

			// header fields
			Fields.Add("Itinerary");
			Fields.Add("Country");
			Fields.Add("ArriveCity");
			Fields.Add("DepartCity");
			Fields.Add("ArriveDate");
			Fields.Add("DepartDate");
			Fields.Add("Length");

			// row fields
			Fields.Add("ServiceTypeID");
			Fields.Add("ServiceType");
			Fields.Add("Date");
			Fields.Add("City");
			Fields.Add("Description");
			Fields.Add("Order");
			Fields.Add("Reference");
			Fields.Add("Status");
			Fields.Add("Rate");
			Fields.Add("Days");
			Fields.Add("Qty");
		    Fields.Add("ExchRate");
			Fields.Add("RowGross");
			Fields.Add("RowNet");

            // row summary total
			Fields.Add("GroupLabel");
			Fields.Add("GroupGross");
			Fields.Add("GroupNet");

			Fields.Add("GrossTotal");
			
		}

		private void PricingDetailed_FetchData(object sender, FetchEventArgs eArgs)
		{
			try
			{
				// report
				if(rowCount == 0)
				{
					ItinerarySet.ItineraryRow itin = itinerarySet.Itinerary[0];
					int days = itinerarySet.ItineraryLength();

				    Fields["Itinerary"].Value = itin.ItineraryName;
					Fields["Length"].Value = days>-1 ? days + " days" : "";
					Fields["Country"].Value = !itin.IsCountryIDNull() ? toolSet.Country.FindByCountryID(itin.CountryID).CountryName :"";
                    Fields["ArriveDate"].Value = !itin.IsArriveDateNull() ? itin.ArriveDate.ToString("dd MMM yyyy, hh:mm tt") : "(no date selected)";
                    Fields["DepartDate"].Value = !itin.IsDepartDateNull() ? itin.DepartDate.ToString("dd MMM yyyy, hh:mm tt") : "(no date selected)";
					Fields["ArriveDate"].Value += " " + GetArriveCity(itin);
					Fields["DepartDate"].Value += " " + GetDepartCity(itin);				
				}
				// group
				if(rowCount < itemsDataView.Count)
				{
					// get data
					ItinerarySet.PurchaseItemRow item = itemsDataView[rowCount++].Row as ItinerarySet.PurchaseItemRow;
					ItinerarySet.PurchaseLineRow line = itinerarySet.PurchaseLine.FindByPurchaseLineID(item.PurchaseLineID);
				
					// header
					Fields["ServiceTypeID"].Value = item.ServiceTypeID.ToString();
					Fields["ServiceType"].Value   = GetServiceType(item.ServiceTypeID);

					// detail
					Fields["Date"].Value		= !item.IsStartDateNull() ? item.StartDate.ToShortDateString() :"";
					Fields["City"].Value		= GetCity(line.SupplierID);
					Fields["Description"].Value = itinerarySet.GetPurchaseItemFullName(line, item);
					Fields["Order"].Value		= line.PurchaseLineID.ToString();
					Fields["Reference"].Value	= !item.IsBookingReferenceNull() ? item.BookingReference :""; 
					Fields["Status"].Value		= GetStatus(item); 
					Fields["Rate"].Value		= 
                        (!item.IsCurrencyCodeNull() ? item.CurrencyCode + " " : "") +
                        (!item.IsGrossNull() ? item.Gross.ToString("###,##0.00") : "");
                    Fields["Days"].Value		= !item.IsNumberOfDaysNull() ? item.NumberOfDays.ToString() :"";
				    Fields["Qty"].Value         = !item.IsQuantityNull() ? item.Quantity.ToString() : "";
				    Fields["ExchRate"].Value    = !item.IsCurrencyRateNull() ? item.CurrencyRate.ToString() :"";
                    Fields["RowGross"].Value    = item.GrossTotalConverted;
				    Fields["RowNet"].Value      = item.NetTotalConverted;
				
					// footer
					Fields["GroupLabel"].Value	= Fields["ServiceType"].Value + " Total:";	
					Fields["GroupGross"].Value	= Fields["RowGross"].Value;
					Fields["GroupNet"].Value	= Fields["RowNet"].Value;					

					eArgs.EOF = false;	
				}
			}
			catch(Exception)	
			{	
				eArgs.EOF = true;	
				throw;
			}
		}

		private void gfPricingTable_Format(object sender, System.EventArgs eArgs)
		{
			PricingSummary rpt = new PricingSummary(itinerarySet, toolSet);
			rpt.Document.Printer.Landscape = true;
			this.subPricingTable.Report = rpt;
		}


		private string GetArriveCity(ItinerarySet.ItineraryRow itin)
		{
			string s = "";
			if(itin.IsArriveCityIDNull())
				s += "(city not selected)";
			else
			{
				ToolSet.CityRow row = toolSet.City.FindByCityID(itin.ArriveCityID);
				s += ((row != null) ? row.CityName : "(selected city not found)");
			}
			return s;
		}

		private string GetDepartCity(ItinerarySet.ItineraryRow itin)
		{	
			string s = "";
			if(itin.IsDepartCityIDNull())
				s += "(city not selected)";
			else
			{
				ToolSet.CityRow row = toolSet.City.FindByCityID(itin.DepartCityID);
				s += row != null ? row.CityName : "(selected city not found)";
			}			
			return s;
		}
				
		private string GetServiceType(int serviceTypeID)
		{
			ToolSet.ServiceTypeRow row = toolSet.ServiceType.FindByServiceTypeID(serviceTypeID);
			if(row != null)
				return row.ServiceTypeName;

			return "Service type not found";
		}
		
//		private string GetDescription(ItinerarySet.PurchaseLineRow line, ItinerarySet.PurchaseItemRow item)
//		{
//			string s = !line.IsPurchaseLineNameNull() ? line.PurchaseLineName + ", " : "";
//			s += !item.IsServiceNameNull() ? item.ServiceName + ", " : "";
//			s += !item.IsOptionNameNull() ? item.OptionName + ", " : "";
//			return s.Replace(",,",",").Trim(',');
//		}

		private string GetCity(int supplierID)
		{
			ItinerarySet.SupplierLookupRow supplier = itinerarySet.SupplierLookup.FindBySupplierID(supplierID);
            if (supplier != null)
            {
                if (!supplier.IsCityIDNull())
                {
                    ToolSet.CityRow city = toolSet.City.FindByCityID(supplier.CityID);
                    if (city != null)
                        return city.CityName;
                }
            }
		    return "";
		}

		private string GetStatus(ItinerarySet.PurchaseItemRow item)
		{
			if(!item.IsRequestStatusIDNull())
			{
				ToolSet.RequestStatusRow row = toolSet.RequestStatus.FindByRequestStatusID(item.RequestStatusID);
				if(row != null)
					return row.RequestStatusName;
			}
			return "";
		}
        
		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.ReportHeader ReportHeader = null;
		private DataDynamics.ActiveReports.Shape Shape1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.Label Label22 = null;
		private DataDynamics.ActiveReports.Label Label23 = null;
		private DataDynamics.ActiveReports.TextBox TextBox2 = null;
		private DataDynamics.ActiveReports.TextBox TextBox3 = null;
		private DataDynamics.ActiveReports.TextBox TextBox4 = null;
		private DataDynamics.ActiveReports.Label Label24 = null;
		private DataDynamics.ActiveReports.Label Label25 = null;
		private DataDynamics.ActiveReports.Label Label26 = null;
		private DataDynamics.ActiveReports.TextBox Days1 = null;
		private DataDynamics.ActiveReports.GroupHeader GroupHeader1 = null;
		private DataDynamics.ActiveReports.GroupHeader GroupHeader2 = null;
		private DataDynamics.ActiveReports.GroupHeader ghServiceType = null;
		private DataDynamics.ActiveReports.Label txtServiceType = null;
		private DataDynamics.ActiveReports.Label lblPrice = null;
		private DataDynamics.ActiveReports.Label Label6 = null;
		private DataDynamics.ActiveReports.Label Label7 = null;
		private DataDynamics.ActiveReports.Label Label8 = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.Label lblSupplierName = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.Label Label20 = null;
		private DataDynamics.ActiveReports.Label Label21 = null;
		private DataDynamics.ActiveReports.Label Label27 = null;
		private DataDynamics.ActiveReports.Label Label28 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox txtDate = null;
		private DataDynamics.ActiveReports.TextBox txtDescription = null;
		private DataDynamics.ActiveReports.TextBox txtCity = null;
		private DataDynamics.ActiveReports.TextBox txtGross = null;
		private DataDynamics.ActiveReports.TextBox txtDays = null;
		private DataDynamics.ActiveReports.TextBox txtQty = null;
		private DataDynamics.ActiveReports.TextBox txtSubTotal = null;
		private DataDynamics.ActiveReports.TextBox txtStatus = null;
		private DataDynamics.ActiveReports.TextBox txtReference = null;
		private DataDynamics.ActiveReports.TextBox Order1 = null;
		private DataDynamics.ActiveReports.TextBox txtNet = null;
		private DataDynamics.ActiveReports.GroupFooter gfServiceType = null;
		private DataDynamics.ActiveReports.TextBox txtTotal = null;
		private DataDynamics.ActiveReports.Label lblTotal = null;
		private DataDynamics.ActiveReports.TextBox Total1 = null;
		private DataDynamics.ActiveReports.Line Line4 = null;
		private DataDynamics.ActiveReports.GroupFooter gfSpaceBeforePricingTable = null;
		private DataDynamics.ActiveReports.GroupFooter gfPricingTable = null;
		private DataDynamics.ActiveReports.SubReport subPricingTable = null;
		private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.PricingDetailed.rpx");
			this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
			this.GroupHeader1 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader1"]));
			this.GroupHeader2 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader2"]));
			this.ghServiceType = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghServiceType"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.gfServiceType = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfServiceType"]));
			this.gfSpaceBeforePricingTable = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfSpaceBeforePricingTable"]));
			this.gfPricingTable = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfPricingTable"]));
			this.ReportFooter = ((DataDynamics.ActiveReports.ReportFooter)(this.Sections["ReportFooter"]));
			this.Shape1 = ((DataDynamics.ActiveReports.Shape)(this.ReportHeader.Controls[0]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[1]));
			this.Label22 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[2]));
			this.Label23 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[3]));
			this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[4]));
			this.TextBox3 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[5]));
			this.TextBox4 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[6]));
			this.Label24 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[7]));
			this.Label25 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[8]));
			this.Label26 = ((DataDynamics.ActiveReports.Label)(this.ReportHeader.Controls[9]));
			this.Days1 = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[10]));
			this.txtServiceType = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[0]));
			this.lblPrice = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[1]));
			this.Label6 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[2]));
			this.Label7 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[3]));
			this.Label8 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[4]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[5]));
			this.lblSupplierName = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[6]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[7]));
			this.Label20 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[8]));
			this.Label21 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[9]));
			this.Label27 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[10]));
			this.Label28 = ((DataDynamics.ActiveReports.Label)(this.ghServiceType.Controls[11]));
			this.txtDate = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.txtDescription = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.txtCity = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.txtGross = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			this.txtDays = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
			this.txtQty = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[5]));
			this.txtSubTotal = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[6]));
			this.txtStatus = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[7]));
			this.txtReference = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[8]));
			this.Order1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[9]));
			this.txtNet = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[10]));
			this.txtTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfServiceType.Controls[0]));
			this.lblTotal = ((DataDynamics.ActiveReports.Label)(this.gfServiceType.Controls[1]));
			this.Total1 = ((DataDynamics.ActiveReports.TextBox)(this.gfServiceType.Controls[2]));
			this.Line4 = ((DataDynamics.ActiveReports.Line)(this.gfServiceType.Controls[3]));
			this.subPricingTable = ((DataDynamics.ActiveReports.SubReport)(this.gfPricingTable.Controls[0]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.PricingDetailed_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.PricingDetailed_FetchData);
			this.gfPricingTable.Format += new System.EventHandler(this.gfPricingTable_Format);
		}

		#endregion
	}
}
