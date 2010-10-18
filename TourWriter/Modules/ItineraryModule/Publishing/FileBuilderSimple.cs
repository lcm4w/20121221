using System;
using System.Data;
using TourWriter.Info;
using TourWriter.Global;
using TourWriter.Services;
using Word;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
    /// <summary>
    /// Summary description for FileBuilderSimple.
    /// </summary>
    public class FileBuilderSimple
    {
        #region Template tags
        private const string tagItineraryName = "[!ItineraryName]";
        private const string tagArriveDay = "[!ArriveDay]";
        private const string tagArriveDate = "[!ArriveDate]";
        private const string tagArriveTime = "[!ArriveTime]";
        private const string tagArriveFlight = "[!ArriveFlight]";
        private const string tagArriveInformation = "[!ArriveInformation]";
        private const string tagDepartDay = "[!DepartDay]";
        private const string tagDepartDate = "[!DepartDate]";
        private const string tagDepartTime = "[!DepartTime]";
        private const string tagDepartFlight = "[!DepartFlight]";
        private const string tagDepartInformation = "[!DepartInformation]";

        //private const string tagBeginSectionDays = "[!BeginSectionDays]";
        //private const string tagEndSectionDays = "[!EndSectionDays]";
        //private const string tagItineraryDay = "[!ItineraryDay]";
        //private const string tagDayDate = "[!DayDate]";
        //private const string tagDayLocations = "[!DayLocations]";

        private const string tagBeginSectionItems = "[!BeginSectionItems]";
        private const string tagEndSectionItems = "[!EndSectionItems]";
        private const string tagLocation = "[!Location]";
        private const string tagDayNumber = "[!DayNumber]";
        private const string tagInDate = "[!InDate]";
        private const string tagInTime = "[!InTime]";
        private const string tagOutDate = "[!OutDate]";
        private const string tagOutTime = "[!OutTime]";
        private const string tagSupplier = "[!Supplier]";
        private const string tagStreetAddress = "[!StreetAddress]";
        private const string tagPhone = "[!Phone]";
        private const string tagFax = "[!Fax]";
        private const string tagEmail = "[!Email]";
        private const string tagDescription = "[!Description]";
        private const string tagBookingNote = "[!BookingNote]";

        #endregion

        private Application wordApp = null;
        private Document mainDoc = null;
        private Document templateDoc = null;
        private bool cancelProcessing = false;
        private string templateFile;
        private ItinerarySet itinerarySet;
        private PublisherMain controller;
        private object missing = System.Reflection.Missing.Value;

        public FileBuilderSimple(ItinerarySet itinerarySet, string templateFile, PublisherMain controller)
        {
            this.itinerarySet = itinerarySet;
            this.templateFile = templateFile;
            this.controller = controller;
        }

        public int CountInsertFiles()
        {
            return itinerarySet.PurchaseItem.Count;
        }

        public void CancelProcessing()
        {
            cancelProcessing = true;
        }

        public void CreateDocument()
        {
            try
            {
                wordApp = new Application();
                wordApp.Visible = false;
                mainDoc = WordHelper.NewDocument(wordApp, templateFile);
                ClearDocument(mainDoc);
                
                BuildMain();
            }
            finally
            {
                // notify control thread of finish
                controller.Invoke(controller.m_DelegateThreadFinished, null);
            }
            FinaliseProcessing();
        }

        private void ClearDocument(Document doc)
        {
            // delete all characters on the document
            object unitType = WdUnits.wdCharacter;
            object unitCount = doc.Content.Characters.Count;
            doc.Content.Delete(ref unitType, ref unitCount);
        }

        private void BuildMain()
        {
            // Add placeholders. 
            Template.Content.Copy();
            mainDoc.Content.Paste();

            // Fill placeholders.
            FillTagsMainSection();

            // Build items section.
            BuildDaysSection();
        }

        private void BuildDaysSection()
        {
            Document itemsDoc = WordHelper.NewDocument(wordApp, templateFile);
            ClearDocument(itemsDoc);

            itinerarySet.PurchaseItem.DefaultView.Sort = "StartDate";
            int dayNumber = 1;

            foreach (DataRowView view in itinerarySet.PurchaseItem.DefaultView)
            {
                App.Debug("Add item...");
                System.Windows.Forms.Application.DoEvents();
                
                if (cancelProcessing) return;

                Document temp = GetTemplateSection(
                    Template, tagBeginSectionItems, tagEndSectionItems);

                FillTagsItemsSection(temp, view.Row as ItinerarySet.PurchaseItemRow, dayNumber);
                temp.Content.Copy();
                WordHelper.AppendCopiedContent(itemsDoc);
                WordHelper.CloseDocumentWithoutSaving(temp);

                // notify control thread of update 
                controller.Invoke(controller.m_DelegateThreadUpdate, null);

                dayNumber++;
            }

            // Copy and paste content to selected template section.
            itemsDoc.Content.Copy();
            Selection s = SelectTemplateSection(mainDoc, tagBeginSectionItems, tagEndSectionItems, true);
            s.Paste();
            WordHelper.PerformBackspace(wordApp.Selection, 2);
            WordHelper.CloseDocumentWithoutSaving(itemsDoc);
        }

        private void FillTagsMainSection()
        {
            ItinerarySet.ItineraryRow itinerary = itinerarySet.Itinerary[0];
            WordHelper.FindAndReplace(mainDoc, tagItineraryName, itinerary.ItineraryName);
            WordHelper.FindAndReplace(mainDoc, tagArriveDay, !itinerary.IsArriveDateNull() ? itinerary.ArriveDate.ToString("dddd") : "");
            WordHelper.FindAndReplace(mainDoc, tagArriveDate, !itinerary.IsArriveDateNull() ? itinerary.ArriveDate.ToString("dd MMM yyyy") : "");
            WordHelper.FindAndReplace(mainDoc, tagArriveTime, !itinerary.IsArriveDateNull() ? itinerary.ArriveDate.ToShortTimeString() : "");
            WordHelper.FindAndReplace(mainDoc, tagArriveFlight, !itinerary.IsArriveFlightNull() ? itinerary.ArriveFlight : "");
            WordHelper.FindAndReplace(mainDoc, tagArriveInformation, !itinerary.IsArriveNoteNull() ? itinerary.ArriveNote : "");

            WordHelper.FindAndReplace(mainDoc, tagDepartDay, !itinerary.IsDepartDateNull() ? itinerary.DepartDate.ToString("dddd") : "");
            WordHelper.FindAndReplace(mainDoc, tagDepartDate, !itinerary.IsDepartDateNull() ? itinerary.DepartDate.ToString("dd MMM yyyy") : "");
            WordHelper.FindAndReplace(mainDoc, tagDepartTime, !itinerary.IsDepartDateNull() ? itinerary.DepartDate.ToShortTimeString() : "");
            WordHelper.FindAndReplace(mainDoc, tagDepartFlight, !itinerary.IsDepartFlightNull() ? itinerary.DepartFlight : "");
            WordHelper.FindAndReplace(mainDoc, tagDepartInformation, !itinerary.IsDepartNoteNull() ? itinerary.DepartNote : "");
        }

        private void FillTagsItemsSection(Document doc, ItinerarySet.PurchaseItemRow item, int dayNumber)
        {
            int numberOfDays;
            string location;
            ItinerarySet.SupplierLookupRow supplier;

            supplier = itinerarySet.SupplierLookup.FindBySupplierID(item.PurchaseLineRow.SupplierID);
            numberOfDays = !item.IsNumberOfDaysNull() ? (int)item.NumberOfDays : 1;

            ToolSet.CityRow city;
            location = (supplier != null && !supplier.IsCityIDNull()) ? (((city = Cache.ToolSet.City.FindByCityID(supplier.CityID)) != null) ? city.CityName.Trim() : "") : "";

            string startTime = (item.IsStartTimeNull()) ? String.Empty : item.StartTime.ToShortTimeString();
            string endTime = (item.IsEndTimeNull()) ? String.Empty : item.EndTime.ToShortTimeString();

            WordHelper.FindAndReplaceInParagraphs(doc, tagLocation, location);
            WordHelper.FindAndReplaceInParagraphs(doc, tagDayNumber, dayNumber.ToString());
            WordHelper.FindAndReplaceInParagraphs(doc, tagInDate, item.StartDate.ToString("dd MMM yyyy"));
            WordHelper.FindAndReplaceInParagraphs(doc, tagInTime, startTime);
            WordHelper.FindAndReplaceInParagraphs(doc, tagOutDate, item.StartDate.AddDays(numberOfDays).ToString("dd MMM yyyy"));
            WordHelper.FindAndReplaceInParagraphs(doc, tagOutTime, endTime);
            WordHelper.FindAndReplaceInParagraphs(doc, tagSupplier, supplier.SupplierName);
            WordHelper.FindAndReplaceInParagraphs(doc, tagDescription, item.PurchaseItemName);
            WordHelper.FindAndReplaceInParagraphs(doc, tagStreetAddress, !supplier.IsStreetAddressNull() ? supplier.StreetAddress : "");
            WordHelper.FindAndReplaceInParagraphs(doc, tagPhone, !supplier.IsPhoneNull() ? supplier.Phone : "");
            WordHelper.FindAndReplaceInParagraphs(doc, tagFax, !supplier.IsFaxNull() ? supplier.Fax : "");
            WordHelper.FindAndReplaceInParagraphs(doc, tagEmail, !supplier.IsEmailNull() ? supplier.Email : "");
            WordHelper.FindAndReplaceInParagraphs(doc, tagBookingNote, !item.PurchaseLineRow.IsNoteToClientNull() ? item.PurchaseLineRow.NoteToClient : "");
        }

        private Document Template
        {
            get
            {
                if (templateDoc != null)
                    return templateDoc;

                object template = templateFile;
                object notVisible = false;
                object isReadOnly = true;
                object noSave = false;

                // Open template doc
                templateDoc = WordHelper.NewDocument(wordApp, templateFile);

                return templateDoc;
            }
        }

        private Document GetTemplateSection(Document document, string searchStartTag, string searchEndTag)
        {
            // Get a section of the template.
            Document doc = WordHelper.GetSearchSelection(
                wordApp, document, GetWildcardSearchPattern(searchStartTag, searchEndTag));

            // Remove the sections start/end tags.
            Selection selection;
            selection = WordHelper.GetSearchSelection(doc, searchStartTag, false);
            WordHelper.PerformDelete(selection, 2);
            selection = WordHelper.GetSearchSelection(doc, searchEndTag, false);
            WordHelper.PerformBackspace(selection, 2);

            return doc;
        }

        private Selection SelectTemplateSection(Document document, string searchStartTag, string searchEndTag, bool includeSearchTagsInSelection)
        {
            // Select a section of the template.
            Selection selection = WordHelper.GetSearchSelection(
                document, GetWildcardSearchPattern(searchStartTag, searchEndTag), true);

            if (!includeSearchTagsInSelection)
            {
                selection.Start = selection.Start + searchStartTag.Length + 1; // +1 to get rid of newline
                selection.End = selection.End - searchEndTag.Length - 1;
            }
            return selection;
        }

        private string GetWildcardSearchPattern(string startTag, string endTag)
        {
            startTag = startTag.Replace("[!", "").Replace("]", "");
            endTag = endTag.Replace("[!", "").Replace("]", "");

            return String.Format(@"\[\!<{0}>\]*\[\!<{1}>\]", startTag, endTag);
        }

        private void FinaliseProcessing()
        {
            if (cancelProcessing)
            {
                foreach (Document doc in wordApp.Documents)
                    WordHelper.CloseDocumentWithoutSaving(doc);

                ((Word._Application)wordApp).Quit(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            }
            else if (wordApp != null)
            {
                wordApp.Options.SaveNormalPrompt = false;
                WordHelper.CloseDocumentWithoutSaving(templateDoc);
                WordHelper.GoToBeginning(wordApp, mainDoc);
                wordApp.Visible = true;
            }
            else
            {
                throw new Exception("Failed to create publishing document");
            }
        }
    }
}
