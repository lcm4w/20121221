using System;
using DataDynamics.ActiveReports;
using TourWriter.Info;
using TourWriter.Info.Services;

namespace TourWriter.Reports.Itinerary
{
	public class PricingSummary : ActiveReport
	{
		private int rowCount;
		private ToolSet toolSet;
		private ItinerarySet itinerarySet;

		public PricingSummary(ItinerarySet itinerarySet, ToolSet toolSet)
        {
            SetLicense(Lookup.Lic);
			InitializeReport();

			this.itinerarySet = itinerarySet;
		    this.toolSet = (ToolSet)toolSet.Copy();

		    RemoveDeletedServiceTypes();
			
			this.Document.Name = String.Format("Summary Itinerary Costing for {0}",
                itinerarySet.Itinerary[0].ItineraryName);
		}
		
        private void RemoveDeletedServiceTypes()
        {
            for (int i = toolSet.ServiceType.Count - 1; i >= 0; i --)
            {
                var row = toolSet.ServiceType[i];
                if (!row.IsIsDeletedNull() && row.IsDeleted)
                {
                    // only delete this service type if it hasn't been used in a booking
                    var rows = itinerarySet.PurchaseItem.Select("ServiceTypeID = " + row.ServiceTypeID);
                    if (rows.Length == 0)
                    {
                        toolSet.ServiceType.RemoveServiceTypeRow(row);
                        row.Delete();
                    }
                }
            }
        }

		private void PricingSummary_ReportStart(object sender, System.EventArgs eArgs)
		{
			//this.PrintWidth = this.Document.Printer.PaperWidth;			
		}

		private void PricingSummary_DataInitialize(object sender, System.EventArgs eArgs)
		{
			rowCount = 0;
			
			// report data fields
			Fields.Add("Itinerary");
			Fields.Add("Country");
			Fields.Add("ArriveCity");
			Fields.Add("DepartCity");
			Fields.Add("ArriveDate");
			Fields.Add("DepartDate");
			Fields.Add("Length");

			// group data fields
			Fields.Add("ServiceType");
			Fields.Add("Count");
			Fields.Add("Net");
			Fields.Add("Markup");
			Fields.Add("Gross");
			Fields.Add("Commission");
			Fields.Add("Margin");
			Fields.Add("Sell");	
			Fields.Add("CountTotal");	
			Fields.Add("NetTotal");
			Fields.Add("MarkupTotal");
			Fields.Add("GrossTotal");
			Fields.Add("CommTotal");
			Fields.Add("MarginTotal");
			Fields.Add("SellTotal");
			Fields.Add("MarginLabel");
		}

		private void PricingSummary_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
			try
			{		
				// report
				if(rowCount == 0)
				{
					ItinerarySet.ItineraryRow itin = itinerarySet.Itinerary[0];
					int days = itinerarySet.ItineraryLength();

                    Fields["Itinerary"].Value = itin.ItineraryName;
					Fields["Length"].Value = days>-1 ? days.ToString() + " days" : "";
					Fields["Country"].Value = !itin.IsCountryIDNull() ? toolSet.Country.FindByCountryID(itin.CountryID).CountryName :"";
                    Fields["ArriveDate"].Value = !itin.IsArriveDateNull() ? itin.ArriveDate.ToString("dd MMM yyyy, hh:mm tt") : "(no date selected)";
                    Fields["DepartDate"].Value = !itin.IsDepartDateNull() ? itin.DepartDate.ToString("dd MMM yyyy, hh:mm tt") : "(no date selected)";
					Fields["ArriveDate"].Value += " " + this.GetArriveCity(itin);
					Fields["DepartDate"].Value += " " + this.GetDepartCity(itin);
					Fields["MarginLabel"].Value = (!itin.IsNetComOrMupNull() && itin.NetComOrMup == "mup") ? "Adjusted Markup" : "Adjusted Commission";
				}

				//group
				if(rowCount < this.toolSet.ServiceType.Rows.Count)
				{
					// row data
                    ToolSet.ServiceTypeRow type = this.toolSet.ServiceType[rowCount++];

					// object datatype used to enable null values (eg. "")
					object nett = ""; object markup = ""; object gross = ""; 
					object comm = ""; object margin = ""; object sell = ""; object count = "";
					this.LoadServiceTypeValues(type.ServiceTypeID,ref count,ref nett,ref markup,ref gross,ref comm,ref margin,ref sell);

					// assign values
					Fields["ServiceType"].Value = type.ServiceTypeName;
					Fields["Count"].Value		= count.ToString();
					Fields["Net"].Value		= nett.ToString();
					Fields["Markup"].Value		= markup.ToString();
					Fields["Gross"].Value		= gross.ToString();
					Fields["Commission"].Value	= comm.ToString();
					Fields["Margin"].Value		= margin.ToString();
					Fields["Sell"].Value		= sell.ToString();
							
					Fields["CountTotal"].Value	= count.ToString();
					Fields["NetTotal"].Value	= nett.ToString();
					Fields["GrossTotal"].Value	= gross.ToString();
					Fields["SellTotal"].Value	= sell.ToString();
					if(margin.ToString() != "")
						Fields["MarginTotal"].Value	= margin.ToString();

					eArgs.EOF = false; 
				}
			}
			catch(Exception)	
			{	
				eArgs.EOF = true;	
				throw;	
			}
		}

		private void gfSummery_BeforePrint(object sender, System.EventArgs eArgs)
		{
			// total percents
			txtMarkupTotal.Value = this.CalcMarkup(txtNetTotal.Value, txtGrossTotal.Value);
			txtCommTotal.Value = this.CalcCommission(txtNetTotal.Value, txtGrossTotal.Value);

            // final price
            txtNetFinal.Value = txtNetTotal.Value;
		    txtSellFinal.Value = itinerarySet.GetGrossFinalPrice();

			// final percents
			txtMarkupFinal.Value = this.CalcMarkup(txtNetFinal.Value, txtSellFinal.Value);
			txtCommFinal.Value = this.CalcCommission(txtNetFinal.Value, txtSellFinal.Value);			
		}


		private void LoadServiceTypeValues(int typeID, ref Object oCount, ref Object oNet, ref Object oMarkup, ref Object oGross, ref Object oComm, ref Object oMargin, ref Object oSell)
		{
			decimal nett = 0; decimal markup = 0; decimal gross = 0;
			decimal comm = 0; decimal margin = 0; decimal sell = 0;

			// sum items
			ItinerarySet.PurchaseItemRow[] items = (ItinerarySet.PurchaseItemRow[])
				itinerarySet.PurchaseItem.Select("ServiceTypeID = " + typeID.ToString());
			foreach(ItinerarySet.PurchaseItemRow item in items)
			{
				if(item.RowState != System.Data.DataRowState.Deleted)
				{
					nett += item.NetTotalConverted;
				    gross += item.GrossTotalConverted;
				}
			}			
			// calcate item percentages
			markup = (nett  == 0) ? 0 : ((gross-nett)/nett)  * 100;
			comm   = (gross == 0) ? 0 : ((gross-nett)/gross) * 100;

			// adjustment final sell price
			ItinerarySet.ItineraryMarginOverrideRow r = itinerarySet.ItineraryMarginOverride.FindByItineraryIDServiceTypeID(itinerarySet.Itinerary[0].ItineraryID, typeID);
			if(r != null && !r.IsMarginNull() && r.Margin != 0)
			{
				margin = r.Margin;
				
				// calc adjusted sell
				string comOrMup = !itinerarySet.Itinerary[0].IsNetComOrMupNull() ? itinerarySet.Itinerary[0].NetComOrMup : "";
				if(comOrMup == "mup")
					sell = Common.CalcGrossByNetMarkup(nett, margin);
				else if(comOrMup == "com")
                    sell = Common.CalcGrossByNetCommission(nett, margin);
				else 
					sell = gross;

				oMargin = margin.ToString();
			}
			else 
				sell = gross;

			// fill return values
			if(items.Length > 0)
			{
				oCount  = items.Length;
				oNet   = nett.ToString();
				oMarkup = (markup > 0) ? markup.ToString() : "";
				oGross  = gross.ToString();
				oComm   = (comm > 0) ? comm.ToString() : "";
				oSell   = sell.ToString();
			}
		}

		private string CalcMarkup(object oNet, object oGross)
		{
			if(oNet.ToString() != "" && oGross.ToString() != "")
			{
				double nett = double.Parse(oNet.ToString());
				double gross = double.Parse(oGross.ToString());
				if(nett != 0)
				{
					double markup = (nett == 0) ? 0 : ((gross-nett)/nett)  * 100;
					return markup.ToString();
				}
			}
			return "";
		}

		private string CalcCommission(object oNet, object oGross)
		{
			if(oNet.ToString() != "" && oGross.ToString() != "")
			{
				double nett = double.Parse(oNet.ToString());
				double gross = double.Parse(oGross.ToString());
				if(gross != 0)
				{
					double comm = (gross == 0) ? 0 : ((gross-nett)/gross) * 100;
					return comm.ToString();
				}
			}
			return "";
		}
		
		private string GetArriveCity(ItinerarySet.ItineraryRow itin)
		{
			string s = "";
			if(itin.IsArriveCityIDNull())
				s += "(city not selected)";
			else
			{
				ToolSet.CityRow row = toolSet.City.FindByCityID(itin.ArriveCityID);
				s += (row != null) ? row.CityName : "(selected city not found)";
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
		private DataDynamics.ActiveReports.TextBox txtDays = null;
		private DataDynamics.ActiveReports.GroupHeader ghSummery = null;
		private DataDynamics.ActiveReports.Label Label30 = null;
		private DataDynamics.ActiveReports.Label lblMargin = null;
		private DataDynamics.ActiveReports.Label Label32 = null;
		private DataDynamics.ActiveReports.Label Label33 = null;
		private DataDynamics.ActiveReports.Label Label34 = null;
		private DataDynamics.ActiveReports.Label Label35 = null;
		private DataDynamics.ActiveReports.Label Label20 = null;
		private DataDynamics.ActiveReports.Label Label61 = null;
		private DataDynamics.ActiveReports.Line Line37 = null;
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.TextBox txtServiceTypeName = null;
		private DataDynamics.ActiveReports.TextBox txtNet = null;
		private DataDynamics.ActiveReports.TextBox txtMarkup = null;
		private DataDynamics.ActiveReports.TextBox txtGross = null;
		private DataDynamics.ActiveReports.TextBox txtComm = null;
		private DataDynamics.ActiveReports.TextBox txtMargin = null;
		private DataDynamics.ActiveReports.TextBox txtSell = null;
		private DataDynamics.ActiveReports.TextBox Sell1 = null;
		private DataDynamics.ActiveReports.Line Line35 = null;
		private DataDynamics.ActiveReports.GroupFooter gfSummery = null;
		private DataDynamics.ActiveReports.TextBox txtSellFinal = null;
		private DataDynamics.ActiveReports.TextBox txtNetTotal = null;
		private DataDynamics.ActiveReports.Label Label56 = null;
		private DataDynamics.ActiveReports.TextBox txtGrossTotal = null;
		private DataDynamics.ActiveReports.TextBox txtCommTotal = null;
		private DataDynamics.ActiveReports.TextBox txtMarkupTotal = null;
		private DataDynamics.ActiveReports.Label Label60 = null;
		private DataDynamics.ActiveReports.TextBox txtNetFinal = null;
		private DataDynamics.ActiveReports.TextBox txtCommFinal = null;
		private DataDynamics.ActiveReports.TextBox txtMarkupFinal = null;
		private DataDynamics.ActiveReports.TextBox txtCountTotal = null;
		private DataDynamics.ActiveReports.TextBox txtSellTotal = null;
		private DataDynamics.ActiveReports.Line Line32 = null;
		private DataDynamics.ActiveReports.Line Line34 = null;
		private DataDynamics.ActiveReports.TextBox txtMarginTotal = null;
		private DataDynamics.ActiveReports.Label Label62 = null;
		private DataDynamics.ActiveReports.Label Label63 = null;
		private DataDynamics.ActiveReports.Label Label64 = null;
		private DataDynamics.ActiveReports.Label Label65 = null;
		private DataDynamics.ActiveReports.Line Line33 = null;
		private DataDynamics.ActiveReports.Line Line36 = null;
		private DataDynamics.ActiveReports.ReportFooter ReportFooter = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.PricingSummary.rpx");
			this.ReportHeader = ((DataDynamics.ActiveReports.ReportHeader)(this.Sections["ReportHeader"]));
			this.ghSummery = ((DataDynamics.ActiveReports.GroupHeader)(this.Sections["ghSummery"]));
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.gfSummery = ((DataDynamics.ActiveReports.GroupFooter)(this.Sections["gfSummery"]));
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
			this.txtDays = ((DataDynamics.ActiveReports.TextBox)(this.ReportHeader.Controls[10]));
			this.Label30 = ((DataDynamics.ActiveReports.Label)(this.ghSummery.Controls[0]));
			this.lblMargin = ((DataDynamics.ActiveReports.Label)(this.ghSummery.Controls[1]));
			this.Label32 = ((DataDynamics.ActiveReports.Label)(this.ghSummery.Controls[2]));
			this.Label33 = ((DataDynamics.ActiveReports.Label)(this.ghSummery.Controls[3]));
			this.Label34 = ((DataDynamics.ActiveReports.Label)(this.ghSummery.Controls[4]));
			this.Label35 = ((DataDynamics.ActiveReports.Label)(this.ghSummery.Controls[5]));
			this.Label20 = ((DataDynamics.ActiveReports.Label)(this.ghSummery.Controls[6]));
			this.Label61 = ((DataDynamics.ActiveReports.Label)(this.ghSummery.Controls[7]));
			this.Line37 = ((DataDynamics.ActiveReports.Line)(this.ghSummery.Controls[8]));
			this.txtServiceTypeName = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[0]));
			this.txtNet = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[1]));
			this.txtMarkup = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.txtGross = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[3]));
			this.txtComm = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
			this.txtMargin = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[5]));
			this.txtSell = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[6]));
			this.Sell1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[7]));
			this.Line35 = ((DataDynamics.ActiveReports.Line)(this.Detail.Controls[8]));
			this.txtSellFinal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[0]));
			this.txtNetTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[1]));
			this.Label56 = ((DataDynamics.ActiveReports.Label)(this.gfSummery.Controls[2]));
			this.txtGrossTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[3]));
			this.txtCommTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[4]));
			this.txtMarkupTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[5]));
			this.Label60 = ((DataDynamics.ActiveReports.Label)(this.gfSummery.Controls[6]));
			this.txtNetFinal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[7]));
			this.txtCommFinal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[8]));
			this.txtMarkupFinal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[9]));
			this.txtCountTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[10]));
			this.txtSellTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[11]));
			this.Line32 = ((DataDynamics.ActiveReports.Line)(this.gfSummery.Controls[12]));
			this.Line34 = ((DataDynamics.ActiveReports.Line)(this.gfSummery.Controls[13]));
			this.txtMarginTotal = ((DataDynamics.ActiveReports.TextBox)(this.gfSummery.Controls[14]));
			this.Label62 = ((DataDynamics.ActiveReports.Label)(this.gfSummery.Controls[15]));
			this.Label63 = ((DataDynamics.ActiveReports.Label)(this.gfSummery.Controls[16]));
			this.Label64 = ((DataDynamics.ActiveReports.Label)(this.gfSummery.Controls[17]));
			this.Label65 = ((DataDynamics.ActiveReports.Label)(this.gfSummery.Controls[18]));
			this.Line33 = ((DataDynamics.ActiveReports.Line)(this.gfSummery.Controls[19]));
			this.Line36 = ((DataDynamics.ActiveReports.Line)(this.gfSummery.Controls[20]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.PricingSummary_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.PricingSummary_FetchData);
			this.gfSummery.BeforePrint += new System.EventHandler(this.gfSummery_BeforePrint);
			this.ReportStart += new System.EventHandler(this.PricingSummary_ReportStart);
		}

		#endregion
	}
}
