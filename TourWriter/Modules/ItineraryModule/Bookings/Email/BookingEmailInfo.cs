using System;
using System.Collections;
using System.Data;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.ItineraryModule.Bookings.Email
{
    public class BookingEmailInfo
    {
        #region Private properties

        // email body table and row settings
        private readonly string htmlTableTemplate = "<table cellspacing=\"0\" cellpadding=\"0\">";
        private readonly string htmlRowTemplateBookings = "<tr valign=top><td width=20></td><td>{0}:&nbsp;&nbsp;</td><td>{1}</td></tr>";
        private readonly string htmlRowTemplateNotes = "<tr valign=top><td width=20></td><td>{0}</td></tr>";
        
        // email body search-and-replace tags
        private readonly string tagHostName = "[!HostName]";
        private readonly string tagSupplierName = "[!SupplierName]";
        private readonly string tagItineraryID = "[!ItineraryID]";
        private readonly string tagBookingID = "[!BookingID]";
        private readonly string tagBookingDetails = "[!BookingDetails]";
        private readonly string tagBookingNotes = "[!BookingNotes]";
        private readonly string tagClientNotes = "[!ClientNotes]";
        private readonly string tagUserName = "[!UserName]";
        private readonly string tagUserEmail = "[!UserEmail]";

        private bool showPrices;
        private int supplierId;
        private string supplierName;
        private EmailMessage emailMessage;
        private readonly ItinerarySet.PurchaseLineRow purchaseLine;

        #endregion

        public int SupplierId
        {
            get { return supplierId; }
        }

        public string SupplierName
        {
            get { return supplierName; }
        }

        public EmailMessage EmailMessage
        {
            get { return emailMessage; }
        }

        public ItinerarySet.PurchaseLineRow PurchaseLine
        {
            get { return purchaseLine; }
        }


        public BookingEmailInfo(ItinerarySet.PurchaseLineRow purchaseLine)
        {
            this.purchaseLine = purchaseLine;
        }

        public void CreateEmailMessage(TemplateSettings templateSettings)
        {
            showPrices = templateSettings.ShowPrices;
            ItinerarySet.SupplierLookupRow supplier =
                GetBookingItinerarySet().SupplierLookup.FindBySupplierID(PurchaseLine.SupplierID);

            // create email
            emailMessage = new EmailMessage();
            emailMessage._To = GetToAddress(supplier.SupplierName, !supplier.IsEmailNull() ? supplier.Email : "");
            emailMessage._From = templateSettings.From;
            emailMessage._Bcc = templateSettings.Bcc;
            emailMessage.Subject = templateSettings.Subject;
            emailMessage.IsBodyHtml = true;
            emailMessage.Body = BuildEmailBody(templateSettings.Body);
            emailMessage._Tag = this;
            emailMessage._SaveWhenSent = templateSettings.SaveToFile;

            // add alternate text view
            var textPart = AlternateView.CreateAlternateViewFromString(
                HtmlToPlainText(emailMessage.Body), Encoding.GetEncoding("iso-8859-1"), "text/plain");
            textPart.TransferEncoding = TransferEncoding.SevenBit;
            emailMessage.AlternateViews.Add(textPart);
            
            // add read receipt
            if (templateSettings.ReadReceipt)
                emailMessage.Headers.Add("Disposition-Notification-To", templateSettings.From);

            supplierId = supplier.SupplierID;
            supplierName = supplier.SupplierName;
        }

        /// <summary>
        /// Strip all html tags and insert new lines where necessary (line break tags, etc)
        /// </summary>
        private static string HtmlToPlainText(string html)
        {
            const string bodyRegex = @"(?<=(<body[^>]*>)).*(?=</body\s*>)";
            const string breakRegex = @"<br[^>]*>";
            const string paragraphRegex = @"<p[^>]*>";
            const string tableRegex = @"</table\s*>";
            const string tableRowRegex = @"<tr[^>]*>";
            const string tableCellRegex = @"</td\s*>";
            const string htmlTagRegex = @"<[^>]*>";

            var match = Regex.Match(html, bodyRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!match.Success)
                return String.Empty;

            string body = match.Value;
            body = body.Replace(Environment.NewLine, String.Empty);
            body = body.Replace("&nbsp;", " ");
            body = Regex.Replace(body, breakRegex, Environment.NewLine, RegexOptions.IgnoreCase);
            body = Regex.Replace(body, paragraphRegex, Environment.NewLine, RegexOptions.IgnoreCase);
            body = Regex.Replace(body, tableRegex, Environment.NewLine, RegexOptions.IgnoreCase);
            body = Regex.Replace(body, tableRowRegex, Environment.NewLine, RegexOptions.IgnoreCase);
            body = Regex.Replace(body, tableCellRegex, "\t", RegexOptions.IgnoreCase);
            body = Regex.Replace(body, htmlTagRegex, String.Empty);

            return body;
        }

        #region Email body creation

        private string BuildEmailBody(string template)
        {
            ItinerarySet.SupplierLookupRow supplier =
                GetBookingItinerarySet().SupplierLookup.FindBySupplierID(PurchaseLine.SupplierID);

            template = ReplaceTag(template, tagHostName,
                (!supplier.IsHostNameNull() && supplier.HostName != "" ? supplier.HostName : "Reservations").Trim());
            template = ReplaceTag(template, tagSupplierName, supplier.SupplierName);
            template = ReplaceTag(template, tagItineraryID, purchaseLine.ItineraryID.ToString());
            template = ReplaceTag(template, tagBookingID, purchaseLine.PurchaseLineID.ToString());
            template = ReplaceTag(template, tagBookingDetails, BuildBookingDetails(PurchaseLine));
            template = ReplaceTag(template, tagBookingNotes, BuildBookingNotes(PurchaseLine));
            template = ReplaceTag(template, tagClientNotes, BuildClientNotes(GetBookingItinerarySet().ItineraryGroup));
            template = ReplaceTag(template, tagUserName, Cache.User.DisplayName);
            template = ReplaceTag(template, tagUserEmail, Cache.User.Email);

            return template;
        }

        private string BuildBookingDetails(ItinerarySet.PurchaseLineRow line)
        {
            StringBuilder sbDetails = new StringBuilder();

            var itinerarySet = (ItinerarySet)line.Table.DataSet;
            var dataView = new DataView(itinerarySet.PurchaseItem);
            dataView.RowFilter = "PurchaseLineID = " + line.PurchaseLineID;

            var rowList = new ArrayList();
            rowList.AddRange(itinerarySet.PurchaseItem.Select("PurchaseLineID = " + line.PurchaseLineID));
            rowList.Sort(new DateTimeSortComparer());

            int index = 1;
            foreach (var row in rowList)
            {
                var item = (ItinerarySet.PurchaseItemRow)row;

                StringBuilder sb = new StringBuilder();
                string heading = "Booking" + ((dataView.Count > 1) ? " " + index++ : "");
                string comm = "", price = "", total = "";

                bool showEndDate = (!item.IsStartDateNull() && !item.IsNumberOfDaysNull() && item.NumberOfDays >= 1);
                if (showPrices)
                    GetBookingPrices(item, ref price, ref comm, ref total);

                // create booking details
                sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Name", GetBookingItinerarySet().Itinerary[0].GetDisplayNameOrItineraryName()));
                sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Description", item.PurchaseItemName));
                sb.AppendLine(NewTableRow(htmlRowTemplateBookings,
                    GetBookingStartName(item.PurchaseItemID), 
                    GetBookingItinerarySet().GetPurchaseItemStartDateTimeString(item.PurchaseItemID, "d MMMM yyyy")));
                if (showEndDate)
                    sb.AppendLine(NewTableRow(htmlRowTemplateBookings,
                        GetBookingEndName(item.PurchaseItemID), 
                        GetBookingItinerarySet().GetPurchaseItemClientCheckoutDateTimeString(item.PurchaseItemID, "d MMMM yyyy", null)));
                if (showEndDate)
                    sb.AppendLine(NewTableRow(htmlRowTemplateBookings, GetNumberOfDaysName(item.ServiceTypeID), item.NumberOfDays));
                if (!item.IsQuantityNull())
                    sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Quantity", item.Quantity));
                if (!item.IsBookingReferenceNull())
                    sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Supplier ref", item.BookingReference));
                if (showPrices && price != "0")
                    sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Price", price));
                if (showPrices && comm != String.Empty)
                    sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Commission", comm));
                if (showPrices && total != String.Empty)
                    sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Total Net", total));

                sbDetails.AppendLine(NewTable(heading, htmlTableTemplate, sb.ToString()));
            }
            return sbDetails.ToString();
        }

        private string BuildBookingNotes(ItinerarySet.PurchaseLineRow line)
        {
            StringBuilder sb = new StringBuilder();
            if (!line.IsNoteToSupplierNull() && line.NoteToSupplier.Trim() != "")
            {
                string notes = line.NoteToSupplier;
                notes = notes.Replace("\r\n", "<br>\r\n");
                sb.AppendLine(NewTable(
                    "Booking Note:", htmlTableTemplate, NewTableRow(htmlRowTemplateNotes, notes)));
            }
            return sb.ToString();
        }

        private string BuildClientNotes(ItinerarySet.ItineraryGroupDataTable groups)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ItinerarySet.ItineraryGroupRow group in groups)
            {
                if (group.RowState != DataRowState.Deleted && !group.IsNoteToSupplierNull())
                    sb.AppendLine(NewTableRow(htmlRowTemplateNotes, group.NoteToSupplier));
            }

            if (sb.Length > 0)
            {
                string notes = sb.ToString();
                notes = notes.Replace("\r\n", "<br>\r\n");
                sb.Remove(0, sb.Length);
                sb.AppendLine(NewTable("Client Note:", htmlTableTemplate, notes));
            }
            return sb.ToString();
        }

        private static string GetNumberOfDaysName(int serviceTypeId)
        {
            var serviceType = Cache.ToolSet.ServiceType.FindByServiceTypeID(serviceTypeId);

            if (!serviceType.IsNumberOfDaysNameNull())
                return serviceType.NumberOfDaysName;

            return "Nights";
        }

        private string GetBookingStartName(int itemId)
        {
            int serviceTypeId = GetBookingItinerarySet().GetPurchaseItemServiceTypeId(itemId);
            ToolSet.ServiceTypeRow serviceTypeRow =
                Cache.ToolSet.ServiceType.FindByServiceTypeID(serviceTypeId);

            if (serviceTypeRow == null)
                return "Arrive"; // if the ServiceType has been deleted, just use this

            return serviceTypeRow.BookingStartName;
        }

        private string GetBookingEndName(int itemId)
        {
            int serviceTypeId = GetBookingItinerarySet().GetPurchaseItemServiceTypeId(itemId);
            ToolSet.ServiceTypeRow serviceTypeRow =
                Cache.ToolSet.ServiceType.FindByServiceTypeID(serviceTypeId);

            if (serviceTypeRow == null)
                return "Depart"; // if the ServiceType has been deleted, just use this

            return serviceTypeRow.BookingEndName;
        }

        private static string GetToAddress(string name, string email)
        {
            // strip illegal characters from name
            name = name.
                Replace(",", "").
                Replace(";", "").
                Replace(":", "").
                Replace("(", "").
                Replace(")", "").
                Replace("[", "").
                Replace("]", "").
                Replace("<", "").
                Replace(">", "");

            // format RFC 2822 style address of "name <email>"
            return String.Format("{0} <{1}>", name, email);
        }


        private static string NewTable(string heading, string tableTemplate, string tableRowHtml)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<p>");
            sb.AppendLine(heading);
            sb.AppendLine(tableTemplate);
            sb.AppendLine(tableRowHtml);
            sb.AppendLine("</table>");
            sb.AppendLine("</p>");

            return sb.ToString();
        }

        private static string NewTableRow(string template, string text)
        {
            return String.Format(template, text);
        }

        private static string NewTableRow(string template, string description, object value)
        {
            return String.Format(template, description, value);
        }

        private static string ReplaceTag(string template, string tag, string newValue)
        {
            return template.Replace(tag, newValue);
        }

        private ItinerarySet GetBookingItinerarySet()
        {
            return purchaseLine.Table.DataSet as ItinerarySet;
        }

        private void GetBookingPrices(
            ItinerarySet.PurchaseItemRow item, ref string price, ref string commission, ref string total)
        {
            ItinerarySet.OptionLookupRow lookup = GetBookingItinerarySet().OptionLookup.FindByOptionID(item.OptionID);
            if (lookup == null)
                return;

            decimal net = !item.IsNetNull() ? item.Net : 0;

            if (lookup.PricingOption == "gc") // Gross plus Commission
            {
                // Earning commission on gross, so show commission rate.
                decimal gross = !item.IsGrossNull() ? item.Gross : 0;
                decimal comm = GetBookingItinerarySet().GetCommission(net, gross);

                commission = (comm / 100).ToString("p");
                price = ((item.CurrencyCode != "") ? ("(" + item.CurrencyCode + ") ") : ("")) +
                        gross.ToString(App.GetCurrencyFormat(item.CurrencyCode));
            }
            else // Net, so no commission.
            {
                commission = "";
                price = ((item.CurrencyCode != "") ? ("(" + item.CurrencyCode + ") ") : ("")) +
                        net.ToString(App.GetCurrencyFormat(item.CurrencyCode));
            }
            total = ((item.CurrencyCode != "") ? ("(" + item.CurrencyCode + ") ") : ("")) +
                    item.NetTotal.ToString(App.GetCurrencyFormat(item.CurrencyCode));
        }

        #endregion
    }

    public class DateTimeSortComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            var row1 = (DataRow)x;
            var row2 = (DataRow)y;

            var oDate1 = row1["StartDate"];
            var oDate2 = row2["StartDate"];

            // handle nulls
            if (oDate1 == DBNull.Value && oDate2 == DBNull.Value)
                return 0;
            if (oDate1 != DBNull.Value && oDate2 == DBNull.Value)
                return 1;
            if (oDate1 == DBNull.Value && oDate2 != DBNull.Value)
                return -1;

            var d1 = (DateTime)oDate1;
            var d2 = (DateTime)oDate2;

            // compare dates
            if (d1.Date > d2.Date)
                return 1;
            if (d1.Date < d2.Date)
                return -1;

            var oTime1 = row1["StartTime"];
            var oTime2 = row2["StartTime"];

            // handle nulls
            if (oTime1 == DBNull.Value && oTime2 == DBNull.Value)
                return 0;
            if (oTime1 != DBNull.Value && oTime2 == DBNull.Value)
                return 1;
            if (oTime1 == DBNull.Value && oTime2 != DBNull.Value)
                return -1;

            var t1 = (DateTime)oTime1;
            var t2 = (DateTime)oTime2;

            // compare times
            if (t1.TimeOfDay > t2.TimeOfDay)
                return 1;
            if (t1.TimeOfDay < t2.TimeOfDay)
                return -1;
            return 0;
        }
    }
}
