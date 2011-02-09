using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        
        // email body search-and-replace tags
        private readonly string tagHostName = "[!HostName]";
        private readonly string tagSupplierName = "[!SupplierName]";
        private readonly string tagItineraryName = "[!ItineraryName]";
        private readonly string tagDisplayName = "[!DisplayName]";
        private readonly string tagAgentName = "[!AgentName]";
        private readonly string tagItineraryID = "[!ItineraryID]";
        private readonly string tagCustomCode = "[!CustomID]";
        private readonly string tagBookingID = "[!BookingID]";
        private readonly string tagBookingDetails = "[!BookingDetails]";
        private readonly string tagBookingNotes = "[!BookingNotes]";
        private readonly string tagClientNotes = "[!ClientNotes]";
        private readonly string tagUserName = "[!UserName]";
        private readonly string tagUserEmail = "[!UserEmail]";
        
        private const string BookingDetailStartTag = "[!BookingDetailStart]";
        private const string ItemCountTag = "[!ItemCount]";
        private const string ItemNameTag = "[!ItemName]";
        private const string ItemDescTag = "[!ItemDesc]";
        private const string ItemStartTextTag = "[!ItemStartText]";
        private const string ItemStartDateTag = "[!ItemStartDate]";
        private const string ItemEndDateStartTag = "[!ItemEndDateStart]";
        private const string ItemEndTextTag = "[!ItemEndText]";
        private const string ItemEndDateTag = "[!ItemEndDate]";
        private const string ItemLengthTextTag = "[!ItemLengthText]";
        private const string ItemLengthTag = "[!ItemLength]";
        private const string ItemEndDateEndTag = "[!ItemEndDateEnd]";
        private const string ItemQuantityTag = "[!ItemQuantity]";
        private const string ItemReferenceStartTag = "[!ItemReferenceStart]";
        private const string ItemReferenceTag = "[!ItemReference]";
        private const string ItemReferenceEndTag = "[!ItemReferenceEnd]";
        private const string ItemPriceStartTag = "[!ItemPriceStart]";
        private const string ItemPriceTag = "[!ItemPrice]";
        private const string ItemCommissionTag = "[!ItemCommission]";
        private const string ItemTotalTag = "[!ItemTotal]";
        private const string ItemPriceEndTag = "[!ItemPriceEnd]";
        private const string BookingDetailEndTag = "[!BookingDetailEnd]";

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
            template = ReplaceTag(template, tagItineraryName, purchaseLine.ItineraryRow.ItineraryName);
            template = ReplaceTag(template, tagDisplayName, !purchaseLine.ItineraryRow.IsDisplayNameNull() ? purchaseLine.ItineraryRow.DisplayName : "");
            template = ReplaceTag(template, tagAgentName, !purchaseLine.ItineraryRow.IsAgentIDNull() ? Cache.ToolSet.Agent.FindByAgentID(purchaseLine.ItineraryRow.AgentID).AgentName : "");
            template = ReplaceTag(template, tagItineraryID, purchaseLine.ItineraryID.ToString());
            template = ReplaceTag(template, tagCustomCode, !purchaseLine.ItineraryRow.IsCustomCodeNull() ? purchaseLine.ItineraryRow.CustomCode : "");
            template = ReplaceTag(template, tagBookingID, purchaseLine.PurchaseLineID.ToString());
            template = ReplaceBookingDetailsTag(template);
            template = ReplaceTag(template, tagBookingNotes, BuildBookingNotes(PurchaseLine));
            template = ReplaceTag(template, tagClientNotes, BuildClientNotes(GetBookingItinerarySet().ItineraryGroup));
            template = ReplaceTag(template, tagUserName, Cache.User.DisplayName);
            template = ReplaceTag(template, tagUserEmail, Cache.User.Email);

            return template;
        }

        private string  ReplaceBookingDetailsTag(string template)
        {
            if (template.Contains(BookingDetailStartTag))
            {
                // new email template, with custom booking details
                return InsertCustomBookingDetails(template, purchaseLine);
            }
            // handle old email templates (which don't allow custom booking details)
            return ReplaceTag(template, tagBookingDetails, BuildBookingDetails(PurchaseLine));
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

            const string dateFormat = "dddd, dd MMMM yyyy";
            const string timeFormat = "HH:mm";

            int index = 1;
            foreach (var row in rowList)
            {
                var item = (ItinerarySet.PurchaseItemRow)row;

                var sb = new StringBuilder();
                string heading = "Booking" + ((dataView.Count > 1) ? " " + index++ : "");
                string comm = "", price = "", total = "";

                bool showEndDate = (!item.IsStartDateNull() && !item.IsNumberOfDaysNull() && item.NumberOfDays >= 1);
                if (showPrices)
                    GetBookingPrices(item, ref price, ref comm, ref total);

                // create booking details
                sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Name", GetBookingItinerarySet().Itinerary[0].GetDisplayNameOrItineraryName()));
                sb.AppendLine(NewTableRow(htmlRowTemplateBookings, "Description", item.PurchaseItemName));
                sb.AppendLine(NewTableRow(htmlRowTemplateBookings, GetBookingStartName(item.PurchaseItemID), item.GetPurchaseItemStartDateTimeString(dateFormat, timeFormat, false)));
                if (showEndDate)
                    sb.AppendLine(NewTableRow(htmlRowTemplateBookings, GetBookingEndName(item.PurchaseItemID), item.GetPurchaseItemEndDateTimeString(dateFormat, timeFormat)));
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

        private string InsertCustomBookingDetails(string template, ItinerarySet.PurchaseLineRow line)
        {
            var detailStart = template.IndexOf(BookingDetailStartTag);
            var detailEnd = template.IndexOf(BookingDetailEndTag) + BookingDetailEndTag.Length;
            var detailTemplate = template.Substring(detailStart, detailEnd - detailStart);

            var endDateStart = template.IndexOf(ItemEndDateStartTag);
            var endDateEnd = template.IndexOf(ItemEndDateEndTag) + ItemEndDateEndTag.Length;
            var endDateTemplate = template.Substring(endDateStart, endDateEnd - endDateStart);
            
            var refStart = template.IndexOf(ItemReferenceStartTag);
            var refEnd = template.IndexOf(ItemReferenceEndTag) + ItemReferenceEndTag.Length;
            var refTemplate = template.Substring(refStart, refEnd - refStart);

            var priceStart = detailTemplate.IndexOf(ItemPriceStartTag);
            var priceEnd = detailTemplate.IndexOf(ItemPriceEndTag) + ItemPriceEndTag.Length;
            var priceTemplate = detailTemplate.Substring(priceStart, priceEnd - priceStart);

            const string dateFormat = "dddd, dd MMMM yyyy";
            const string timeFormat = "HH:mm";
            
            var i = 1;
            string bookingDetail = "", detailText;
            var items = new ArrayList(line.GetPurchaseItemRows());
            items.Sort(new DateTimeSortComparer());
            foreach (var item in items.OfType<ItinerarySet.PurchaseItemRow>())
            {
                detailText = detailTemplate.Replace(BookingDetailStartTag, "").Replace(BookingDetailEndTag, "");
                var showEndDate = (!item.IsStartDateNull() && !item.IsNumberOfDaysNull() && item.NumberOfDays >= 1);
                
                var enEnCulture = CultureInfo.CreateSpecificCulture("en-UK");

                detailText = ReplaceTag(detailText, ItemCountTag, i++.ToString());
                detailText = ReplaceTag(detailText, ItemNameTag, GetBookingItinerarySet().Itinerary[0].GetDisplayNameOrItineraryName());
                detailText = ReplaceTag(detailText, ItemDescTag, item.PurchaseItemName);
                detailText = ReplaceTag(detailText, ItemStartTextTag, GetBookingStartName(item.PurchaseItemID));
                detailText = ReplaceTag(detailText, ItemStartDateTag, item.GetPurchaseItemStartDateTimeString(dateFormat, timeFormat, false));

                var endDateText = "";
                if (showEndDate)
                {
                    endDateText = endDateTemplate.Replace(ItemEndDateStartTag, "").Replace(ItemEndDateEndTag, "");
                    endDateText = ReplaceTag(endDateText, ItemEndTextTag, GetBookingEndName(item.PurchaseItemID));
                    endDateText = ReplaceTag(endDateText, ItemEndDateTag, item.GetPurchaseItemEndDateTimeString(dateFormat, timeFormat));
                    endDateText = ReplaceTag(endDateText, ItemLengthTextTag, GetNumberOfDaysName(item.ServiceTypeID));
                    endDateText = ReplaceTag(endDateText, ItemLengthTag, item.NumberOfDays.ToString());
                }
                detailText = detailText.Replace(endDateTemplate, endDateText);

                detailText = ReplaceTag(detailText, ItemQuantityTag, !item.IsQuantityNull() ? item.Quantity.ToString() : "");

                var refText = "";
                if (!item.IsBookingReferenceNull())
                {
                    refText = refTemplate.Replace(ItemReferenceStartTag, "").Replace(ItemReferenceEndTag, "");
                    refText = ReplaceTag(refText, ItemReferenceTag, item.BookingReference);
                }
                detailText = detailText.Replace(refTemplate, refText);

                var priceText = "";
                if (showPrices)
                {
                    string comm = "", price = "", total = ""; 
                    GetBookingPrices(item, ref price, ref comm, ref total);

                    priceText = priceTemplate.Replace(ItemPriceStartTag, "").Replace(ItemPriceEndTag, "");
                    priceText = ReplaceTag(priceText, ItemPriceTag, price != "0" ? price : "");
                    priceText = ReplaceTag(priceText, ItemCommissionTag, !string.IsNullOrEmpty(comm) ? comm : "");
                    priceText = ReplaceTag(priceText, ItemTotalTag, !string.IsNullOrEmpty(total) ? total : "");
                }
                detailText = detailText.Replace(priceTemplate, priceText);

                bookingDetail += detailText;
            }
            template = template.Replace(detailTemplate, bookingDetail);
            return template;
        }

        private static string BuildBookingNotes(ItinerarySet.PurchaseLineRow line)
        {
            return line.NoteToSupplier.Replace("\r\n", "<br />");
        }

        private static string BuildClientNotes(IEnumerable<ItinerarySet.ItineraryGroupRow> groups)
        {
            var note = "";
            foreach (var group in groups)
            {
                if (group.RowState != DataRowState.Deleted && !group.IsNoteToSupplierNull())
                    note += group.NoteToSupplier + "<br />";
            }
            return note.Replace("\r\n", "<br />");
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

        private void GetBookingPrices(ItinerarySet.PurchaseItemRow item, ref string price, ref string commission, ref string total)
        {
            var lookup = GetBookingItinerarySet().OptionLookup.FindByOptionID(item.OptionID);
            if (lookup == null) return;

            var currencyCode = Currencies.GetPurchaseItemCurrencyCode(item);
            var format = currencyCode != null ?
                "{0:" + string.Format("{0} ({1})", Currencies.Single(currencyCode).Format, currencyCode) + "}" :
                "{0:c}";
            
            var net = !item.IsNetNull() ? item.Net : 0;

            if (lookup.PricingOption == "gc") // Gross plus Commission
            {
                // Earning commission on gross, so show commission rate.
                var gross = !item.IsGrossNull() ? item.Gross : 0;
                var comm = GetBookingItinerarySet().GetCommission(net, gross);

                commission = string.Format("{0:p}", comm/100);
                price = string.Format(format, gross);
            }
            else // Net, so no commission.
            {
                commission = "";
                price = string.Format(format, net);
            }
            total = string.Format(format, item.NetTotal);
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
