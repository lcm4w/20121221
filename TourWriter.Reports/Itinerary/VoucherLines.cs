using System;
using TourWriter.Info;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;

namespace TourWriter.Reports.Itinerary
{
	public class VoucherLines : ActiveReport
	{
		private int rowCount;
		private string dateFormat;
        private bool isQtyColumnEmpty;
        private bool isDaysColumnEmpty;
        private ToolSet toolSet;
		private ItinerarySet itinerarySet;
		private ItinerarySet.PurchaseItemRow[] purchaseItemArray;

		public VoucherLines(ItinerarySet itinerarySet, ToolSet toolSet,
		                    ItinerarySet.PurchaseItemRow[] purchaseItemArray, string dateFormat)
        {
            SetLicense(Lookup.Lic);
			InitializeReport();

			this.dateFormat = dateFormat;
            this.toolSet = toolSet;
			this.itinerarySet = itinerarySet;
			this.purchaseItemArray = purchaseItemArray;
		}


		private void VoucherLines_ReportStart(object sender, System.EventArgs eArgs)
		{
			rowCount = 0;
            isQtyColumnEmpty = true;
            isDaysColumnEmpty = true;
		}

		private void VoucherLines_DataInitialize(object sender, System.EventArgs eArgs)
		{
			Fields.Add("ItemDetail");
			Fields.Add("ArriveDate");
			Fields.Add("DepartDate");
			Fields.Add("Days");
			Fields.Add("Quantity");
			Fields.Add("SupplierRef");			
		}

		private void VoucherLines_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
			try
			{			 
				if(purchaseItemArray != null && rowCount < purchaseItemArray.Length)
				{
					ItinerarySet.PurchaseItemRow item = purchaseItemArray[rowCount++];
				    
					Fields["ItemDetail"].Value = !item.IsPurchaseItemNameNull() ? item.PurchaseItemName : "";
                    Fields["ArriveDate"].Value = GetArriveDateWithCustomFormat(item);
                    Fields["DepartDate"].Value = GetDepartDateWithCustomFormat(item);
					Fields["Days"	   ].Value = !item.IsNumberOfDaysNull() ? item.NumberOfDays.ToString() : "";
					Fields["Quantity"  ].Value = !item.IsQuantityNull() ? item.Quantity.ToString() : "";
					Fields["SupplierRef"].Value = !item.IsBookingReferenceNull() ? item.BookingReference :"";

                    if (!item.IsQuantityNull())
                        isQtyColumnEmpty = false;
                    if (!item.IsNumberOfDaysNull())
                        isDaysColumnEmpty = false;

					eArgs.EOF = false;
				}
			}
			catch(Exception)
			{
				eArgs.EOF = true;
				throw;
			}
		}

	
		private string GetArriveDateWithCustomFormat(ItinerarySet.PurchaseItemRow item)
		{
            return itinerarySet.GetPurchaseItemClientCheckinDateTimeString(
                item.PurchaseItemID, dateFormat, "h:mmtt");
		}

        private string GetDepartDateWithCustomFormat(ItinerarySet.PurchaseItemRow item)
        {
            DateTime date;
            var datetimeSting = itinerarySet.GetPurchaseItemClientCheckoutDateTimeString(item.PurchaseItemID, System.Threading.Thread.CurrentThread.CurrentCulture);
            if (DateTime.TryParse(datetimeSting, out date))
                return date.ToString(dateFormat) + (date.Hour > 0 && date.Minute > 0 ? " " + date.ToString("h:mmtt") : "");

            return string.Empty;
        }


		private void GroupHeader1_Format(object sender, EventArgs eArgs)
		{
		    // Show service type labels based on the first booking item in the list.
            if (purchaseItemArray != null && purchaseItemArray.Length > 0)
            {
                int itemId = purchaseItemArray[0].PurchaseItemID;

                ToolSet.ServiceTypeRow row = toolSet.ServiceType.FindByServiceTypeID(
                    itinerarySet.GetPurchaseItemServiceTypeId(itemId));

                Label2.Text = !row.IsBookingStartNameNull() ? row.BookingStartName : "";
                Label3.Text = !row.IsBookingEndNameNull() ? row.BookingEndName : "";
                
            }
		}

        void GroupHeader1_BeforePrint(object sender, EventArgs e)
        {
            Label4.Visible = !isDaysColumnEmpty;
            Label5.Visible = !isQtyColumnEmpty;
        }
	    
		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.GroupHeader GroupHeader1 = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.Label Label5 = null;
		private DataDynamics.ActiveReports.Label Label6 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox TextBox = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.TextBox TextBox2 = null;
		private DataDynamics.ActiveReports.TextBox TextBox3 = null;
		private DataDynamics.ActiveReports.TextBox TextBox4 = null;
		private DataDynamics.ActiveReports.TextBox TextBox5 = null;
		private DataDynamics.ActiveReports.GroupFooter GroupFooter1 = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.VoucherLines.rpx");
			this.GroupHeader1 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader1"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.GroupFooter1 = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["GroupFooter1"]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.GroupHeader1.Controls[0]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.GroupHeader1.Controls[1]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.GroupHeader1.Controls[2]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.GroupHeader1.Controls[3]));
			this.Label5 = ((DataDynamics.ActiveReports.Label)(this.GroupHeader1.Controls[4]));
			this.Label6 = ((DataDynamics.ActiveReports.Label)(this.GroupHeader1.Controls[5]));
			this.TextBox = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.TextBox3 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			this.TextBox4 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
			this.TextBox5 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[5]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.VoucherLines_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.VoucherLines_FetchData);
			this.ReportStart += new System.EventHandler(this.VoucherLines_ReportStart);
			this.GroupHeader1.Format += new System.EventHandler(this.GroupHeader1_Format);
            this.GroupHeader1.BeforePrint += new EventHandler(GroupHeader1_BeforePrint);
		}

        

		#endregion
	}
}
