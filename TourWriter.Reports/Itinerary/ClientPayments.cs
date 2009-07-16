using System;
using TourWriter.Info;
using DataDynamics.ActiveReports;

namespace TourWriter.Reports.Itinerary
{
	public class ClientPayments : ActiveReport
	{
		private int rowCount;
        private ToolSet toolSet;
		private ItinerarySet itinerarySet;

		public ClientPayments(ItinerarySet itinerarySet, ToolSet toolSet)
		{
            SetLicense(Lookup.Lic);
            InitializeReport();
			this.itinerarySet = itinerarySet;
            this.toolSet = toolSet;
		}

        public decimal GetTotalPayments()
        {
            decimal total = 0;
            foreach (ItinerarySet.ItineraryPaymentRow payment in itinerarySet.ItineraryPayment)
                if(payment.RowState != System.Data.DataRowState.Deleted)
                    total += !payment.IsAmountNull() ? payment.Amount : 0;
            
            return total;
        }


	    private void ClientPayments_ReportStart(object sender, System.EventArgs eArgs)
	    {
	        rowCount = 0;
	    }

	    private void ClientPayments_DataInitialize(object sender, System.EventArgs eArgs)
	    {
	        Fields.Add("PaymentDate");
            Fields.Add("PaymentName");
            Fields.Add("Details");
	        Fields.Add("PaymentAmount");
	        Fields.Add("TotalPayments");				
	    }

	    private void ClientPayments_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
	    {
	        try
	        {			 
	            if(rowCount < itinerarySet.ItineraryPayment.Rows.Count)
	            {
	                ItinerarySet.ItineraryPaymentRow payment = 
	                    itinerarySet.ItineraryPayment[rowCount++];

                    if (payment.RowState == System.Data.DataRowState.Deleted)
                    {
                        LayoutAction = DataDynamics.ActiveReports.LayoutAction.NextRecord;
                    }
                    else
                    {
                        Fields["PaymentDate"].Value = payment.PaymentDate.ToShortDateString();

                        Fields["PaymentName"].Value =
                            itinerarySet.ItineraryMember.FindByItineraryMemberID(payment.ItineraryMemberID).
                                ItineraryMemberName;

                        Fields["Details"].Value = payment.Comments;

                        Fields["PaymentAmount"].Value =
                            (!payment.IsAmountNull())
                                ?
                            payment.Amount
                                : 0;

                        Fields["TotalPayments"].Value = Fields["PaymentAmount"].Value;

                        eArgs.EOF = false;
                    }
	            }
	        }
	        catch(Exception)
	        {
	            eArgs.EOF = true;
	            throw;
	        }
	    }

	    #region ActiveReports Designer generated code
	    private DataDynamics.ActiveReports.GroupHeader GroupHeader1 = null;
	    private DataDynamics.ActiveReports.Detail Detail = null;
	    private DataDynamics.ActiveReports.TextBox TextBox1 = null;
	    private DataDynamics.ActiveReports.TextBox TextBox5 = null;
	    private DataDynamics.ActiveReports.TextBox txtDate = null;
	    private DataDynamics.ActiveReports.TextBox TextBox = null;
	    private DataDynamics.ActiveReports.GroupFooter GroupFooter1 = null;
	    private DataDynamics.ActiveReports.TextBox TextBox2 = null;
	    public void InitializeReport()
	    {
	        this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.ClientPayments.rpx");
	        this.GroupHeader1 = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["GroupHeader1"]));
	        this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
	        this.GroupFooter1 = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["GroupFooter1"]));
	        this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
	        this.TextBox5 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
	        this.txtDate = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
	        this.TextBox = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
	        this.TextBox2 = ((DataDynamics.ActiveReports.TextBox)(this.GroupFooter1.Controls[0]));
	        // Attach Report Events
	        this.DataInitialize += new System.EventHandler(this.ClientPayments_DataInitialize);
	        this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.ClientPayments_FetchData);
	        this.ReportStart += new System.EventHandler(this.ClientPayments_ReportStart);
	    }

	    #endregion
	}
}
