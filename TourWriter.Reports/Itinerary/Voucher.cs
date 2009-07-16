using System;
using System.Collections;
using TourWriter.Info;
using DataDynamics.ActiveReports;

namespace TourWriter.Reports.Itinerary
{
	public class Voucher : ActiveReport
	{	
		#region Defaults
		// Default layout position for vouchers on the page. 
		// Can be over-ridden in this reports language file.
		private const int DefaultPageTopMargin		  = 20;
		private const int DefaultPageLeftMargin		  = 22;
		private const int DefaultSpaceBetweenVouchers = 10;
		private const int DefaultVouchersPerPage      = 3;
		private const string DefaultDateFormat = "dd MMM yy"; // date part only
		private const int MeasurementConversionFactor = 25; // millimeters to inches
		#endregion

		#region Members
		private float topMargin;
		private float voucherY;
		private float voucherX;
		private float spaceBetweenVouchers;
		private int   vouchersPerPage;
		private string dateFormat; // allows user to set date format from template file
		private int voucherCount;
		private int rowCount;
		private string additionalText;
	    private UserSet.UserRow user;
		private AgentSet agentSet;
		private ToolSet toolSet;
		private ItinerarySet itinerarySet;
		private ItinerarySet.PurchaseLineRow[] purchaseLineArray;
		private ItinerarySet.PurchaseItemRow[] purchaseItemArray;
		#endregion
		
		#region Custom sort class
		/// <summary>
		/// Custom sort comparer for PurchaseLines array, so we can sort by earliest PurchaseItem
		/// </summary>
		public class PurchaseLineSortByDate : System.Collections.IComparer  
		{
			private ItinerarySet itinerarySet;
			public PurchaseLineSortByDate(ItinerarySet itinerarySet)
			{
				this.itinerarySet = itinerarySet;
			}


			int System.Collections.IComparer.Compare(Object x, Object y)  
			{
				string expression = "PurchaseLineID = {0}";
				
				ItinerarySet.PurchaseItemRow[] arrayX = (ItinerarySet.PurchaseItemRow[])itinerarySet.PurchaseItem.Select(
					String.Format(expression, (x as ItinerarySet.PurchaseLineRow).PurchaseLineID), "StartDate ASC");
				
				ItinerarySet.PurchaseItemRow[] arrayY = (ItinerarySet.PurchaseItemRow[])itinerarySet.PurchaseItem.Select(
					String.Format(expression, (y as ItinerarySet.PurchaseLineRow).PurchaseLineID), "StartDate ASC");
				
				if(arrayX.Length == 0 && arrayY.Length == 0)
					return 0;
				if(arrayX.Length == 0)
					return -1;
				if(arrayY.Length == 0)
					return 1;
				 
				ItinerarySet.PurchaseItemRow itemX = arrayX[0] as ItinerarySet.PurchaseItemRow;
				ItinerarySet.PurchaseItemRow itemY = arrayY[0] as ItinerarySet.PurchaseItemRow;

                // date sorting
				if(itemX.IsStartDateNull() && itemY.IsStartDateNull())
					return 0;
				if(itemX.IsStartDateNull())
					return 1;
				if(itemY.IsStartDateNull())
					return -1;

				if(itemX.StartDate > itemY.StartDate)
					return 1;
				if(itemX.StartDate < itemY.StartDate)
					return -1;

                // time sorting
                if (itemX.IsStartTimeNull() && itemY.IsStartTimeNull())
                    return 0;
                if (itemX.IsStartTimeNull())
                    return 1;
                if (itemY.IsStartTimeNull())
                    return -1;

                if (itemX.StartTime.TimeOfDay > itemY.StartTime.TimeOfDay)
                    return 1;
                if (itemX.StartTime.TimeOfDay < itemY.StartTime.TimeOfDay)
                    return -1;

				return 0;
			}
		}

        public class PurchaseItemSortByDate : System.Collections.IComparer
        {
            private ItinerarySet itinerarySet;
            public PurchaseItemSortByDate(ItinerarySet itinerarySet)
            {
                this.itinerarySet = itinerarySet;
            }


            int System.Collections.IComparer.Compare(Object x, Object y)
            {
                ItinerarySet.PurchaseItemRow itemX = (ItinerarySet.PurchaseItemRow)x;
                ItinerarySet.PurchaseItemRow itemY = (ItinerarySet.PurchaseItemRow)y;

                // date sorting
                if (itemX.IsStartDateNull() && itemY.IsStartDateNull())
                    return 0;
                if (itemX.IsStartDateNull())
                    return 1;
                if (itemY.IsStartDateNull())
                    return -1;

                if (itemX.StartDate > itemY.StartDate)
                    return 1;
                if (itemX.StartDate < itemY.StartDate)
                    return -1;

                // time sorting
                if (itemX.IsStartTimeNull() && itemY.IsStartTimeNull())
                    return 0;
                if (itemX.IsStartTimeNull())
                    return 1;
                if (itemY.IsStartTimeNull())
                    return -1;

                if (itemX.StartTime.TimeOfDay > itemY.StartTime.TimeOfDay)
                    return 1;
                if (itemX.StartTime.TimeOfDay < itemY.StartTime.TimeOfDay)
                    return -1;

                return 0;
            }
        }
		#endregion

		public Voucher(string purchaseLineCsList, ItinerarySet itinerarySet,
			UserSet.UserRow user, AgentSet agentSet, ToolSet toolSet, string additionalText)
        {
            SetLicense(Lookup.Lic);		
			InitializeReport();

			PrintWidth = PageSettings.PaperWidth;
			
			this.itinerarySet = itinerarySet;
			this.user = user;
			this.agentSet = agentSet;
			this.toolSet = toolSet;
			this.additionalText = additionalText;

			string expression = "PurchaseLineID IN (" + purchaseLineCsList + ")";
			purchaseLineArray = (ItinerarySet.PurchaseLineRow[])
				itinerarySet.PurchaseLine.Select(expression);

			IComparer comparer = new PurchaseLineSortByDate(itinerarySet);
			Array.Sort(purchaseLineArray, comparer);

		    Document.Name = String.Format("Itinerary Vouchers for {0}",
                itinerarySet.Itinerary[0].ItineraryName);
		}

		
		private void LoadUserLayoutVariables()
		{
			// Attempt to load user values from language file. 
			// Convert millimeters to inches.
 
			float voucherNoteHeight, voucherDetailHeight;

			// Voucher Detail height and adjust affected controls
			try   { voucherDetailHeight = float.Parse(VoucherDetailHeight.Text) / MeasurementConversionFactor; }
			catch { voucherDetailHeight = PurchaseItemsSubReport.Height; }
			
			// Voucher Note height and adjust affected controls
			try   { voucherNoteHeight = float.Parse(VoucherNoteHeight.Text) / MeasurementConversionFactor; }
			catch { voucherNoteHeight = VoucherNotes.Height; }
			
			// Top margin
			try   { voucherY = float.Parse(PageTopMargin.Text) / MeasurementConversionFactor; }
			catch { voucherY = (float)DefaultPageTopMargin / MeasurementConversionFactor; }											

			// Left margin
			try   { voucherX = float.Parse(PageLeftMargin.Text) / MeasurementConversionFactor; }
			catch { voucherX = (float)DefaultPageLeftMargin / MeasurementConversionFactor; }

			// Space between each voucher on a page
			try   { spaceBetweenVouchers = float.Parse(SpaceBetweenVouchers.Text) / MeasurementConversionFactor; }
			catch { spaceBetweenVouchers = (float)DefaultSpaceBetweenVouchers / MeasurementConversionFactor; }

			// Vouchers per page
			try   { vouchersPerPage = int.Parse(VouchersPerPage.Text); }
			catch { vouchersPerPage = DefaultVouchersPerPage; }
			
			// Date format (test valid format string in try/catch).
			try   { dateFormat = DateFormat.Text;  DateTime.Parse(DateTime.Now.ToString(dateFormat)); }
			catch { dateFormat = DefaultDateFormat; }

			// save for resetting on each new page
			topMargin = voucherY; 

			// make adjustments
			if(voucherDetailHeight != PurchaseItemsSubReport.Height)
				AdjustLayout(voucherDetailHeight - PurchaseItemsSubReport.Height, PurchaseItemsSubReport.Top);
			if(voucherNoteHeight != VoucherNotes.Height)
				AdjustLayout(voucherNoteHeight - VoucherNotes.Height, VoucherNotes.Top);
		}

		private void AdjustLayout(float yCorrection, float sourceControlPosY)
		{
			foreach(ARControl c in Detail.Controls)
			{
				if(c.GetType() == typeof(Line))
				{
					Line l = c as DataDynamics.ActiveReports.Line;
					if(l.Y1 > sourceControlPosY) l.Y1 += yCorrection;
					if(l.Y2 > sourceControlPosY) l.Y2 += yCorrection;
				}
				else
					if(c.Top > sourceControlPosY) c.Top += yCorrection;
			}
		}


		private void Voucher_ReportStart(object sender, System.EventArgs eArgs)
		{	
			// reset rowcount each time report is run
			rowCount = 0;				
			voucherCount = 1;
			LoadUserLayoutVariables();
		}

		private void Voucher_DataInitialize(object sender, System.EventArgs eArgs)
		{
			Fields.Add("AgentLogo");
			Fields.Add("SupplierName");
			Fields.Add("SupplierAddress");
			Fields.Add("ItineraryName");

			Fields.Add("Note");
			Fields.Add("PurchaseLineID");
            Fields.Add("ItineraryID");
			Fields.Add("UserName");
			Fields.Add("Date");			
		}

		private void Voucher_FetchData(object sender, DataDynamics.ActiveReports.ActiveReport.FetchEventArgs eArgs)
		{
			try
			{
				if(rowCount < purchaseLineArray.Length)
				{
					ItinerarySet.PurchaseLineRow line = purchaseLineArray[rowCount++];
					ItinerarySet.SupplierLookupRow supplier = itinerarySet.SupplierLookup.FindBySupplierID(line.SupplierID);
					
					int purchaseLineID = line.PurchaseLineID;					
					purchaseItemArray = (ItinerarySet.PurchaseItemRow[])
						itinerarySet.PurchaseItem.Select("PurchaseLineID = " + purchaseLineID.ToString(), "StartDate");

				    IComparer comparer = new PurchaseItemSortByDate(itinerarySet);
				    Array.Sort(purchaseItemArray, comparer);

					Fields["SupplierName"	].Value = supplier.SupplierName;
					Fields["SupplierAddress"].Value = BuildSupplierAddress(supplier);
                    Fields["ItineraryName"].Value = itinerarySet.Itinerary[0].GetDisplayNameOrItineraryName();

                    Fields["Note"           ].Value = BuildVoucherNote(purchaseLineID);
					Fields["PurchaseLineID"	].Value = line.PurchaseLineID.ToString();
                    Fields["ItineraryID"    ].Value = itinerarySet.Itinerary[0].ItineraryID;
                    Fields["UserName"       ].Value = user.UserName;
					Fields["Date"			].Value = DateTime.Now.ToString("dd MMM yy");

					eArgs.EOF = false;
				}				
			}
			catch(Exception)
			{
				eArgs.EOF = true;
				throw;
			}		
		}

		private void Detail_Format(object sender, System.EventArgs eArgs)
		{
			// add the subreport
			PurchaseItemsSubReport.Report = new VoucherLines(itinerarySet, toolSet, purchaseItemArray, dateFormat);
			
			// add the logo
			string logo = GetAgentLogo();
			if(System.IO.File.Exists(logo))
			{
				try
				{
					Picture1.Image = System.Drawing.Image.FromFile(logo);
				}
				catch(System.OutOfMemoryException ex)
				{
					throw new System.IO.FileLoadException(
						"Invalid file format for file " + logo, ex);
				}
			}
			// set position
			PositionVoucherOnPage();	
		}


		private void DebugVoucherOnPage()
		{			
			System.Diagnostics.Debug.WriteLine(
				String.Format(
				"Page = {0}, Voucher = {1}, X,Y = {2},{3}",				
				Document.Pages.Count.ToString(),
				voucherCount.ToString(),
				voucherX.ToString(),
				voucherY.ToString()));
		}

		private void PositionVoucherOnPage()
		{
			// Position voucher on the page, first correcting the initial design-time postion,
			// then according to the user assigned page margins/spacers, then shrink its height.

			// Make correction for design-time layout
			float xCorrection, yCorrection;
			xCorrection = LineLeft.X1;
			yCorrection = LineTop.Y1;

			// position to user settings
			foreach(ARControl c in Detail.Controls)
			{
				if(c.GetType() == typeof(Line))
				{
					Line l = c as DataDynamics.ActiveReports.Line;
					l.X1 = l.X1 + voucherX - xCorrection;
					l.X2 = l.X2 + voucherX - xCorrection;
					l.Y1 = l.Y1 + voucherY - yCorrection;
					l.Y2 = l.Y2 + voucherY - yCorrection;
				}
				else
				{
					c.Left = c.Left + voucherX - xCorrection;
					c.Top  = c.Top + voucherY - yCorrection;
				}
			}
			// shrink height
			Detail.Height = this.LineBottom.Y1 + 0.01f;

			// set for next voucher
			if(voucherCount++ % vouchersPerPage == 0)
			{
				Detail.NewPage = NewPage.After;
				voucherY = topMargin;
			}			
			else 
			{
				Detail.NewPage = NewPage.None;				
				voucherY = spaceBetweenVouchers;
			}
		}

		private string GetAgentLogo()
		{
            if (agentSet != null && agentSet.Agent.Count > 0 && !agentSet.Agent[0].IsVoucherLogoFileNull()) 
				return Lookup.ConvertToFullPath(
                    agentSet.Agent[0].VoucherLogoFile, toolSet.AppSettings[0].ExternalFilesPath);

			return "";
		}

		private string BuildSupplierAddress(ItinerarySet.SupplierLookupRow supplier)
        {
            string street = "";
            string city = "";
            string region = "";
            string state = "";
            string phone1 = "";
            string phone2 = "";

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
            if (!supplier.IsFreePhoneNull())
            {
                phone1 = supplier.FreePhone.Trim();
            }
            if (!supplier.IsPhoneNull())
            {
                phone2 = supplier.Phone.Trim();
            }

			// remove any repeated info
			if(city   == region) city   = "";
			if(region == state ) region = "";
			if(phone1 == phone2) phone2 = "";
										
			return
				(street != "" ? street + Environment.NewLine : "") +
				(city   != "" ? city   + Environment.NewLine : "") +
				(region != "" ? region + ", " + state : state) + Environment.NewLine +
				(phone1 != "" || phone2 != "" ? "Tel: " +
				(phone1 != "" ? phone1 + (phone2 != "" ? ", or " + phone2 : "") : phone2) : "");
		}

        private string BuildVoucherNote(int purchaseLineID)
        {
            // additional note
            string s = additionalText;
            
            // booking note
            s += (s.Length > 0 ? Environment.NewLine : "") +
                 itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineID).NoteToVoucher;

			// agent note
            if (agentSet != null && agentSet.Agent.Count > 0 && 
                !agentSet.Agent[0].IsVoucherFooterNull() && agentSet.Agent[0].VoucherFooter != "")
			{
				if(s.Trim().Length > 0 && !s.EndsWith(Environment.NewLine))
					s += Environment.NewLine;
				s += agentSet.Agent[0].VoucherFooter;
			}
			return s;
		}


		#region ActiveReports Designer generated code
		private DataDynamics.ActiveReports.Detail Detail = null;
		private DataDynamics.ActiveReports.Picture Picture1 = null;
		private DataDynamics.ActiveReports.Label Label3 = null;
		private DataDynamics.ActiveReports.TextBox VoucherNotes = null;
		private DataDynamics.ActiveReports.Label Label5 = null;
		private DataDynamics.ActiveReports.TextBox txtUsername = null;
		private DataDynamics.ActiveReports.TextBox TextBox = null;
		private DataDynamics.ActiveReports.Label Label6 = null;
		private DataDynamics.ActiveReports.TextBox TextBox1 = null;
		private DataDynamics.ActiveReports.Label Label4 = null;
		private DataDynamics.ActiveReports.Label Label1 = null;
		private DataDynamics.ActiveReports.Label Title1 = null;
		private DataDynamics.ActiveReports.Label Label2 = null;
		private DataDynamics.ActiveReports.TextBox txtBookingName = null;
		private DataDynamics.ActiveReports.TextBox AgentInfo1 = null;
		private DataDynamics.ActiveReports.SubReport PurchaseItemsSubReport = null;
		private DataDynamics.ActiveReports.Line Line = null;
		private DataDynamics.ActiveReports.Line LineTop = null;
		private DataDynamics.ActiveReports.Line LineBottom = null;
		private DataDynamics.ActiveReports.Line LineLeft = null;
		private DataDynamics.ActiveReports.Line LineRight = null;
		private DataDynamics.ActiveReports.Label PageTopMargin = null;
		private DataDynamics.ActiveReports.Label PageLeftMargin = null;
		private DataDynamics.ActiveReports.Label SpaceBetweenVouchers = null;
		private DataDynamics.ActiveReports.Label VouchersPerPage = null;
		private DataDynamics.ActiveReports.Label DateFormat = null;
		private DataDynamics.ActiveReports.Label VoucherDetailHeight = null;
		private DataDynamics.ActiveReports.Label VoucherNoteHeight = null;
		public void InitializeReport()
		{
			this.LoadLayout(this.GetType(), "TourWriter.Reports.Itinerary.Voucher.rpx");
			this.Detail = ((DataDynamics.ActiveReports.Detail)(this.Sections["Detail"]));
			this.Picture1 = ((DataDynamics.ActiveReports.Picture)(this.Detail.Controls[0]));
			this.Label3 = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[1]));
			this.VoucherNotes = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[2]));
			this.Label5 = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[3]));
			this.txtUsername = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[4]));
			this.TextBox = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[5]));
			this.Label6 = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[6]));
			this.TextBox1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[7]));
			this.Label4 = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[8]));
			this.Label1 = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[9]));
			this.Title1 = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[10]));
			this.Label2 = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[11]));
			this.txtBookingName = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[12]));
			this.AgentInfo1 = ((DataDynamics.ActiveReports.TextBox)(this.Detail.Controls[13]));
			this.PurchaseItemsSubReport = ((DataDynamics.ActiveReports.SubReport)(this.Detail.Controls[14]));
			this.Line = ((DataDynamics.ActiveReports.Line)(this.Detail.Controls[15]));
			this.LineTop = ((DataDynamics.ActiveReports.Line)(this.Detail.Controls[16]));
			this.LineBottom = ((DataDynamics.ActiveReports.Line)(this.Detail.Controls[17]));
			this.LineLeft = ((DataDynamics.ActiveReports.Line)(this.Detail.Controls[18]));
			this.LineRight = ((DataDynamics.ActiveReports.Line)(this.Detail.Controls[19]));
			this.PageTopMargin = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[20]));
			this.PageLeftMargin = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[21]));
			this.SpaceBetweenVouchers = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[22]));
			this.VouchersPerPage = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[23]));
			this.DateFormat = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[24]));
			this.VoucherDetailHeight = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[25]));
			this.VoucherNoteHeight = ((DataDynamics.ActiveReports.Label)(this.Detail.Controls[26]));
			// Attach Report Events
			this.DataInitialize += new System.EventHandler(this.Voucher_DataInitialize);
			this.FetchData += new DataDynamics.ActiveReports.ActiveReport.FetchEventHandler(this.Voucher_FetchData);
			this.ReportStart += new System.EventHandler(this.Voucher_ReportStart);
			this.Detail.Format += new System.EventHandler(this.Detail_Format);
		}

		#endregion
	}	

}
