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
        private readonly string htmlRowTemplateClients = "<tr valign=top><td width=20></td><td>{0}:&nbsp;&nbsp;</td><td>{1}</td></tr>";
        
        // email body search-and-replace tags
        private const string HostNameTag = "[!HostName]";
        private const string SupplierNameTag = "[!SupplierName]";
        private const string PurchaseLineNameTag = "[!BookingName]";
        private const string ItineraryNameTag = "[!ItineraryName]";
        private const string DisplayNameTag = "[!DisplayName]";
        private const string AgentNameTag = "[!AgentName]";
        private const string ItineraryIdTag = "[!ItineraryID]";
        private const string CustomCodeTag = "[!CustomID]";
        private const string CountryOrOriginTag = "[!CountryOfOrigin]";
        private const string PaxCountTag = "[!PaxCount]";
        private const string BookingIdTag = "[!BookingID]";
        private const string BookingDetailsTag = "[!BookingDetails]";
        private const string ClientDetailsTag = "[!ClientDetails]";
        private const string BookingNotesTag = "[!BookingNotes]";
        private const string ClientNotesTag = "[!ClientNotes]";
        private const string UserNameTag = "[!UserName]";
        private const string UserEmailTag = "[!UserEmail]";
        private const string ItineraryStartDateTag = "[!ItineraryStartDate]";
        
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
        private const string ItemFocTag = "[!ItemFoc]";
        private const string ItemPriceEndTag = "[!ItemPriceEnd]";
        private const string BookingDetailEndTag = "[!BookingDetailEnd]";

        private const string ClientDetailStartTag = "[!ClientDetailStart]";
        private const string ClientCountTag = "[!ClientCount]";
        private const string ClientTitleStartTag = "[!ClientTitleStart]";
        private const string ClientTitleTag = "[!ClientTitle]";
        private const string ClientTitleEndTag = "[!ClientTitleEnd]";
        private const string ClientNameTag = "[!ClientName]";
        private const string ClientCommentsStartTag = "[!ClientCommentsStart]";
        private const string ClientCommentsTag = "[!ClientComments]";
        private const string ClientCommentsEndTag = "[!ClientCommentsEnd]";
        private const string ClientAgeGroupStartTag = "[!ClientAgeGroupStart]";
        private const string ClientAgeGroupTag = "[!ClientAgeGroup]";
        private const string ClientAgeGroupEndTag = "[!ClientAgeGroupEnd]";
        private const string ClientAgeStartTag = "[!ClientAgeStart]";
        private const string ClientAgeTag = "[!ClientAge]";
        private const string ClientAgeEndTag = "[!ClientAgeEnd]";
        private const string ClientDetailEndTag = "[!ClientDetailEnd]";

        internal const string SubjectStartTag = "[!SubjectStart]";
        internal const string SubjectEndTag = "[!SubjectEnd]";

        private bool showPrices;
        private int supplierId;
        private string supplierName;
        private EmailMessage emailMessage;     

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
        
        internal IEnumerable<ItinerarySet.PurchaseLineRow> PurchaseLines { get; set; }

        public BookingEmailInfo(IEnumerable<ItinerarySet.PurchaseLineRow> purchaseLines)
        {     
            PurchaseLines = purchaseLines;
        }

        public void CreateEmailMessage(TemplateSettings templateSettings)
        {
            // default to first PurchaseLine for header info
            var firstPpurchaseLine = PurchaseLines.First();

            showPrices = templateSettings.ShowPrices;
            var supplier = GetBookingItinerarySet().SupplierLookup.FindBySupplierID(firstPpurchaseLine.SupplierID);

            // create email
            emailMessage = new EmailMessage();
            emailMessage._To = GetToAddress(supplier);
            emailMessage._From = templateSettings.From;
            emailMessage._Bcc = templateSettings.Bcc;
            emailMessage.Subject = BuildEmailText(templateSettings.Subject, firstPpurchaseLine);
            emailMessage.IsBodyHtml = true;

            if (PurchaseLines.Count() > 0)
            {
                foreach (var purchaseLineRow in PurchaseLines)
                    emailMessage.Body += BuildEmailText(templateSettings.Body, purchaseLineRow);
            }
            else
            {
                emailMessage.Body = BuildEmailText(templateSettings.Body, firstPpurchaseLine);
            }

            emailMessage._Tag = this;
            emailMessage._SaveWhenSent = templateSettings.SaveToFile;

            // add alternate text view
            var textPart = AlternateView.CreateAlternateViewFromString(
                HtmlToPlainText(emailMessage.Body), Encoding.GetEncoding("UTF-8"), "text/plain");
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

        public string BuildEmailText(string template, ItinerarySet.PurchaseLineRow purchaseLine)
        {
            var supplier = GetBookingItinerarySet().SupplierLookup.FindBySupplierID(purchaseLine.SupplierID);
            var origin = !purchaseLine.ItineraryRow.IsCountryIDNull() ? Cache.ToolSet.Country.FindByCountryID(purchaseLine.ItineraryRow.CountryID) : null;
            var pax = !purchaseLine.ItineraryRow.IsPaxOverrideNull() ? purchaseLine.ItineraryRow.PaxOverride : 
                purchaseLine.ItineraryRow.GetItineraryGroupRows().Sum(g => g.GetItineraryMemberRows().Count());

            template = ReplaceTag(template, HostNameTag,
                (!supplier.IsHostNameNull() && supplier.HostName != "" ? supplier.HostName : "Reservations").Trim());

            template = ReplaceTag(template, SupplierNameTag, supplier.SupplierName);
            template = ReplaceTag(template, PurchaseLineNameTag, purchaseLine.PurchaseLineName);
            template = ReplaceTag(template, ItineraryNameTag, purchaseLine.ItineraryRow.ItineraryName);
            template = ReplaceTag(template, DisplayNameTag, !purchaseLine.ItineraryRow.IsDisplayNameNull() ? purchaseLine.ItineraryRow.DisplayName : "");
            template = ReplaceTag(template, AgentNameTag, !purchaseLine.ItineraryRow.IsAgentIDNull() ? Cache.ToolSet.Agent.FindByAgentID(purchaseLine.ItineraryRow.AgentID).AgentName : "");
            template = ReplaceTag(template, ItineraryIdTag, purchaseLine.ItineraryID.ToString());
            template = ReplaceTag(template, CustomCodeTag, !purchaseLine.ItineraryRow.IsCustomCodeNull() ? purchaseLine.ItineraryRow.CustomCode : "");
            template = ReplaceTag(template, CountryOrOriginTag, origin != null ? origin.CountryName : "");
            template = ReplaceTag(template, PaxCountTag, pax.ToString());
            template = ReplaceTag(template, BookingIdTag, purchaseLine.PurchaseLineID.ToString());
            template = ReplaceBookingDetailsTag(template, purchaseLine);
            template = ReplaceClientDetailsTag(template, purchaseLine);
            template = ReplaceTag(template, BookingNotesTag, BuildBookingNotes(purchaseLine));
            template = ReplaceTag(template, ClientNotesTag, BuildClientNotes(GetBookingItinerarySet().ItineraryGroup));
            template = ReplaceTag(template, UserNameTag, Cache.User.DisplayName);
            template = ReplaceTag(template, UserEmailTag, Cache.User.Email);
            template = ReplaceTag(template, ItineraryStartDateTag, purchaseLine.ItineraryRow.ArriveDate.ToShortDateString());

            return template;
        }

        private string ReplaceClientDetailsTag(string template, ItinerarySet.PurchaseLineRow purchaseLine)
        {
            if (template.Contains(ClientDetailStartTag))
            {
                // new email template, with custom client details
                return InsertCustomClientDetails(template, purchaseLine);
            }
            // handle old email templates (which don't allow custom client details)
            return ReplaceTag(template, ClientDetailsTag, BuildClientDetails(purchaseLine));
        }

        private string BuildClientDetails(ItinerarySet.PurchaseLineRow line)
        {
            StringBuilder sbDetails = new StringBuilder();

            var itinerarySet = (ItinerarySet)line.Table.DataSet;

            int index = 1;
            foreach (ItinerarySet.ItineraryMemberRow item in itinerarySet.ItineraryMember)
            {
                var sb = new StringBuilder();
                string heading = "Client" + ((itinerarySet.ItineraryMember.Count > 1) ? " " + index++ : "");

                // create client details
                if (item.IsTitleNull())
                    sb.AppendLine(NewTableRow(htmlRowTemplateClients, "Name", item.ItineraryMemberName));
                else
                    sb.AppendLine(NewTableRow(htmlRowTemplateClients, "Name", item.Title + " " + item.ItineraryMemberName));
                if (!item.IsCommentsNull())
                    sb.AppendLine(NewTableRow(htmlRowTemplateClients, "Comments", item.Comments));
                if (!item.IsAgeGroupIDNull())
                    sb.AppendLine(NewTableRow(htmlRowTemplateClients, "Age Group", GetAgeGroup(item.AgeGroupID)));
                if (!item.IsAgeNull())
                    sb.AppendLine(NewTableRow(htmlRowTemplateClients, "Age", item.Age));
                else if (!item.IsContactIDNull() && GetContactAge(item.ContactID) != "")
                    sb.AppendLine(NewTableRow(htmlRowTemplateClients, "Age", GetContactAge(item.ContactID)));

                sbDetails.AppendLine(NewTable(heading, htmlTableTemplate, sb.ToString()));
            }
            return sbDetails.ToString();
        }

        private string InsertCustomClientDetails(string template, ItinerarySet.PurchaseLineRow line)
        {
            var detailStart = template.IndexOf(ClientDetailStartTag);
            var detailEnd = template.IndexOf(ClientDetailEndTag) + ClientDetailEndTag.Length;
            var detailTemplate = template.Substring(detailStart, detailEnd - detailStart);

            var titleStart = template.IndexOf(ClientTitleStartTag);
            var titleEnd = template.IndexOf(ClientTitleEndTag) + ClientTitleEndTag.Length;
            var titleTemplate = template.Substring(titleStart, titleEnd - titleStart);

            var commentsStart = template.IndexOf(ClientCommentsStartTag);
            var commentsEnd = template.IndexOf(ClientCommentsEndTag) + ClientCommentsEndTag.Length;
            var commentsTemplate = template.Substring(commentsStart, commentsEnd - commentsStart);

            var ageGroupStart = template.IndexOf(ClientAgeGroupStartTag);
            var ageGroupEnd = template.IndexOf(ClientAgeGroupEndTag) + ClientAgeGroupEndTag.Length;
            var ageGroupTemplate = template.Substring(ageGroupStart, ageGroupEnd - ageGroupStart);

            var ageStart = detailTemplate.IndexOf(ClientAgeStartTag);
            var ageEnd = detailTemplate.IndexOf(ClientAgeEndTag) + ClientAgeEndTag.Length;
            var ageTemplate = detailTemplate.Substring(ageStart, ageEnd - ageStart);

            var i = 1;
            string clientDetail = "", detailText;
            var itinerarySet = (ItinerarySet)line.Table.DataSet;
            foreach (ItinerarySet.ItineraryMemberRow member in itinerarySet.ItineraryMember)
            {
                detailText = detailTemplate.Replace(ClientDetailStartTag, "").Replace(ClientDetailEndTag, "");

                detailText = ReplaceTag(detailText, ClientCountTag, i++.ToString());

                var titleText = "";
                if (!member.IsTitleNull())
                {
                    titleText = titleTemplate.Replace(ClientTitleStartTag, "").Replace(ClientTitleEndTag, "");
                    titleText = ReplaceTag(titleText, ClientTitleTag, member.Title);
                }
                detailText = detailText.Replace(titleTemplate, titleText);

                var commentsText = "";
                if (!member.IsCommentsNull())
                {
                    commentsText = commentsTemplate.Replace(ClientCommentsStartTag, "").Replace(ClientCommentsEndTag, "");
                    commentsText = ReplaceTag(commentsText, ClientCommentsTag, member.Comments);
                }
                detailText = detailText.Replace(commentsTemplate, commentsText);

                var ageGroupText = "";
                if (!member.IsAgeGroupIDNull())
                {
                    ageGroupText = ageGroupTemplate.Replace(ClientAgeGroupStartTag, "").Replace(ClientAgeGroupEndTag, "");
                    ageGroupText = ReplaceTag(ageGroupText, ClientAgeGroupTag, GetAgeGroup(member.AgeGroupID));
                }
                detailText = detailText.Replace(ageGroupTemplate, ageGroupText);

                var ageText = "";
                if (!member.IsAgeNull())
                {
                    ageText = ageTemplate.Replace(ClientAgeStartTag, "").Replace(ClientAgeEndTag, "");
                    ageText = ReplaceTag(ageText, ClientAgeTag, member.Age.ToString());
                }
                else if (!member.IsContactIDNull() && GetContactAge(member.ContactID) != "")
                {
                    ageText = ageTemplate.Replace(ClientAgeStartTag, "").Replace(ClientAgeEndTag, "");
                    ageText = ReplaceTag(ageText, ClientAgeTag, GetContactAge(member.ContactID));
                }
                detailText = detailText.Replace(ageTemplate, ageText);

                detailText = ReplaceTag(detailText, ClientNameTag, member.ItineraryMemberName);

                clientDetail += detailText;
            }
            template = template.Replace(detailTemplate, clientDetail);
            return template;
        }

        private string  ReplaceBookingDetailsTag(string template, ItinerarySet.PurchaseLineRow purchaseLine)
        {
            if (template.Contains(BookingDetailStartTag))
            {
                // new email template, with custom booking details
                return InsertCustomBookingDetails(template, purchaseLine);
            }
            // handle old email templates (which don't allow custom booking details)
            return ReplaceTag(template, BookingDetailsTag, BuildBookingDetails(purchaseLine));
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
                detailText = ReplaceTag(detailText, ItemFocTag, !item.IsDiscountUnitsNull() ? ((int)item.DiscountUnits).ToString() : "");

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

        private static string GetToAddress(ItinerarySet.SupplierLookupRow supplier) //string name, string email)
        {
            if (supplier == null) return string.Empty;
            var name = !supplier.IsSupplierNameNull() ? supplier.SupplierName : "";
            var email = !supplier.IsEmailNull() ? supplier.Email : "";
            
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

        private static string GetAgeGroup(int ageGroupId)
        {
            var ageGroup = Cache.ToolSet.AgeGroup.FindByAgeGroupID(ageGroupId);

            if (!ageGroup.IsAgeGroupNameNull())
                return ageGroup.AgeGroupName;

            return "Adult";
        }

        private static string GetContactAge(int contactId)
        {
            TourWriter.BusinessLogic.Contact c = new TourWriter.BusinessLogic.Contact();
            TourWriter.Info.ContactSet contactSet  = c.GetContactSet(contactId);

            if (!contactSet.Contact[0].IsBirthDateNull())
                return CalculateAge(contactSet.Contact[0].BirthDate);

            return "";
        }

        private static string CalculateAge(DateTime bd)
        {
            DateTime birthDate = DateTime.Parse(bd.ToString());
            DateTime now = DateTime.Today;

            int years = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                --years;

            return years.ToString();
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
            return PurchaseLines.FirstOrDefault().Table.DataSet as ItinerarySet;
        }

        private void GetBookingPrices(ItinerarySet.PurchaseItemRow item, ref string price, ref string commission, ref string total)
        {
            var lookup = GetBookingItinerarySet().OptionLookup.FindByOptionID(item.OptionID);
            if (lookup == null) return;

            var currencyCode = CurrencyService.GetPurchaseItemCurrencyCode(item);
            var format = currencyCode != null ?
                "{0:" + string.Format("{0} ({1})", CurrencyService.GetCurrency(currencyCode).DisplayFormat, currencyCode) + "}" :
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
