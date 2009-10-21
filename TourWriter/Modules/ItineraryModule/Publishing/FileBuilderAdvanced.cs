using System;
using System.Collections;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using TourWriter.Services;
using Word;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
	public class FileBuilderAdvanced
    {
        #region Properties

        // search and replace tags, these strings are searched for in the day template document
		private const string MergeFieldDayNumber   = "[!DayNumber]";
		private const string MergeFieldDayName     = "[!DayName]";
		private const string MergeFieldDayDate     = "[!DayDate]";
		private const string MergeFieldDayLocation = "[!DayLocation]";
		private const string MergeFieldDayText     = "[!DayText]";
	    private const string MergeFieldLineBreak   = "[!LineBreak]";


		// node keys, each node has key set to one of these type indicators 
		public const string FrontSectionNodeKey = "Front";
		public const string DaysSectionNodeKey  = "Day";
		public const string BackSectionNodeKey  = "Back";
		public const string PurchaseItemNodeKey = "Item";
		public const string FileNodeTag         = "File";

		private Word.Application wordApp = null;
		private Document mainDoc = null;
		private TreeNodesCollection contentNodes;
		private object missing    = System.Reflection.Missing.Value;
		private bool cancelProcessing = false;
		private UltraTreeNode _parentNode;
		private PublisherMain controller;
	    private string dayTemplateFile;
	    private Document dayTemplateDoc;
	    private readonly bool _setFontToTemplate;

        #endregion

        private Document DayTemplateDocument
	    {
	        get
	        {
                if(dayTemplateDoc == null)
                {
                    dayTemplateDoc = WordHelper.LoadDocument(wordApp, dayTemplateFile);
                }
	            return dayTemplateDoc;
	        }
	    }

        public FileBuilderAdvanced(UltraTreeNode parentNode, string dayTemplateFile, PublisherMain controller, bool setFontToTemplate)
		{
			_parentNode = parentNode;
			this.controller = controller;
		    this.dayTemplateFile = dayTemplateFile;
		    _setFontToTemplate = setFontToTemplate;
		}
        

		public void CreateDocument()
        {
            ValidateBeforeStart();

			wordApp = new Word.Application();
			wordApp.Visible = false;
			Document frontDoc = null; 
			Document daysDoc = null; 
			Document backDoc = null;

			try
			{
                contentNodes = _parentNode.Nodes;

				// create the main document
				mainDoc = AddNewDocument(true);

				// create the section documents
				frontDoc = CreateDocumentSection(contentNodes[FrontSectionNodeKey], WordHelper.BreakTypes.PageBreak);
				daysDoc = CreateDocumentDaySection();
                backDoc = CreateDocumentSection(contentNodes[BackSectionNodeKey], WordHelper.BreakTypes.PageBreak);
				
				// add the section documents to the main document
				bool insertBreak = false;
				if(frontDoc != null)
				{
                    WordHelper.AppendDocument(wordApp, frontDoc, mainDoc, insertBreak);
                    WordHelper.CloseDocumentWithoutSaving(frontDoc);
					insertBreak = true;
				}
				if(daysDoc != null)
				{
					if(insertBreak)
                        WordHelper.InsertBreak(wordApp, mainDoc, WordHelper.BreakTypes.PageBreak);
                    WordHelper.AppendDocument(wordApp, daysDoc, mainDoc, insertBreak);
                    WordHelper.CloseDocumentWithoutSaving(daysDoc);
					insertBreak = true;
				}
				if(backDoc != null)
				{
					if(insertBreak)
                        WordHelper.InsertBreak(wordApp, mainDoc, WordHelper.BreakTypes.PageBreak);
                    WordHelper.AppendDocument(wordApp, backDoc, mainDoc, insertBreak);
                    WordHelper.CloseDocumentWithoutSaving(backDoc);
				}
                WordHelper.GoToBeginning(wordApp, mainDoc);
			}
			catch(Exception ex)
			{
                if (mainDoc != null) WordHelper.CloseDocumentWithoutSaving(mainDoc);
                if (frontDoc != null) WordHelper.CloseDocumentWithoutSaving(frontDoc);
                if (daysDoc != null) WordHelper.CloseDocumentWithoutSaving(daysDoc);
                if (backDoc != null) WordHelper.CloseDocumentWithoutSaving(backDoc);

				if(ex.Message.IndexOf(
					"method or property is not available because the license to use this application has expired..")
					> -1
					&& ex.Source == "Microsoft Word")
				{
					App.Error("Unable to use Microsoft Word because its license has expired", ex);
				}
				else
				{
                    ((Word._Application)wordApp).Quit(ref missing, ref missing, ref missing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp); 
					App.Error("Error creating Published Itinerary file", ex);
				}
				wordApp = null;				
			}
			finally
			{
				// notify control thread 
                controller.Invoke(controller.m_DelegateThreadFinished, null);
			}
			FinaliseProcessing();
		}
	    
		private Document CreateDocumentSection(UltraTreeNode parentNode, WordHelper.BreakTypes breakType)
		{			
			if(cancelProcessing) 
                return null;

			ArrayList files = GetFileNamesForSection(parentNode.Nodes);
			if(files.Count == 0)
				return null;

			// add files to temp document
			Document tempDoc = AddNewDocument(false);
            for(int i = 0; i < files.Count; i++)
            {
				if(cancelProcessing) 
                    return null;

                Document srcDoc = null;
                try
                {
                    string srcFileName = files[i].ToString();
                    
                    try
                    {
                        srcDoc = WordHelper.LoadDocument(wordApp, srcFileName);
                    }
                    catch (Exception ex)
                    {
                        if (ErrorHelper.IsFileAccessError(ex))
                        {
                            tempDoc.Bookmarks.Item(ref WordHelper.EndOfDoc).Range.Select();
                            WordHelper.WriteErrorMessage(
                                wordApp.Selection, String.Format(WordHelper.MsgFileNotFound, srcFileName));
                            App.ShowFileNotFound(srcFileName);
                        }
                        else throw;
                    }

                    WordHelper.AppendDocument(wordApp, srcDoc, tempDoc, false);
                    if (i < files.Count - 1)
                    {
                        WordHelper.InsertBreak(wordApp, tempDoc, breakType);
                    }
                }
                finally
                {
                    WordHelper.CloseDocumentWithoutSaving(srcDoc);
                }
				// notify control thread 
                controller.Invoke(controller.m_DelegateThreadUpdate, null);
			}
			return tempDoc;
		}

		private Document CreateDocumentDaySection()
		{
		    if(cancelProcessing) return null;

			// get list of day nodes for processing
			ArrayList dayNodes = new ArrayList();
			foreach(UltraTreeNode node in contentNodes)
			{
				if(cancelProcessing) return null;
				if(IsValidNode(node, DaysSectionNodeKey) && node.CheckedState == CheckState.Checked)
					dayNodes.Add(node);
			}
			if(dayNodes.Count == 0)	return null;
				
			Document daysSectionDoc = AddNewDocument(false);
			DateTime startDate = LayoutHelper.GetDaySectionDate(GetStartDayNode());
				
			// navigate each day node
			foreach(UltraTreeNode dayNode in dayNodes)
			{
				if(cancelProcessing) return null;
				
				// copy template into new doc
                Document dayDoc = AddNewDocument(false);
                DayTemplateDocument.Content.Copy();
				dayDoc.Content.Paste();

                // create content from files loaded for this day
                Document textDoc = CreateDocumentSection(dayNode, WordHelper.BreakTypes.ParagraphBreak);
                DateTime dayDate = LayoutHelper.GetDaySectionDate(dayNode);

                // apply template formatting to day content doc, this seems to get lost in prev copy/paste.
                if (textDoc != null) // if contains docs
                {
                    textDoc.Select();
                    Selection dayText = textDoc.Application.Selection;
                    Selection tmpText = WordHelper.GetSearchSelection(DayTemplateDocument, MergeFieldDayText, false);

                    dayText.Borders = tmpText.ParagraphFormat.Borders;
                    dayText.ParagraphFormat = tmpText.ParagraphFormat;
                }

			    // fill template placeholders
                WordHelper.FindAndReplace(dayDoc, MergeFieldDayNumber, ((dayDate - startDate).Days + 1).ToString());
                WordHelper.FindAndReplace(dayDoc, MergeFieldDayName, dayDate.ToString("dddd"));
                WordHelper.InsertDateText(dayDoc, dayDate);
                WordHelper.FindAndReplace(dayDoc, MergeFieldDayLocation, LayoutHelper.GetDaySectionLocation(dayNode));
                WordHelper.FindAndReplace(dayDoc, MergeFieldDayText, textDoc, _setFontToTemplate);

                if (WordHelper.FindAndReplace(dayDoc, MergeFieldLineBreak, ""))
                    WordHelper.InsertBreak(wordApp, dayDoc, WordHelper.BreakTypes.LineBreak);
                else
                    WordHelper.InsertBreak(wordApp, dayDoc, WordHelper.BreakTypes.PageBreak);
                
				// add day document to the days collection
			    daysSectionDoc.Content.Select();
                WordHelper.AppendDocument(wordApp, dayDoc, daysSectionDoc, false);

                // clean up
                WordHelper.CloseDocumentWithoutSaving(dayDoc);
                WordHelper.CloseDocumentWithoutSaving(textDoc);
			}
            // clean up the days collection
            daysSectionDoc.Bookmarks.Item(ref WordHelper.EndOfDoc).Range.Select();
			return daysSectionDoc;
		}
        

        public void CancelProcessing()
        {
            cancelProcessing = true;
        }

        private void FinaliseProcessing()
        {
            WordHelper.CloseDocumentWithoutSaving(dayTemplateDoc);

            if (cancelProcessing)
            {
                WordHelper.CloseDocumentWithoutSaving(mainDoc);
                ((Word._Application)wordApp).Quit(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            }
            else if (wordApp != null)
            {
                wordApp.Visible = true;
            }
        }

        private UltraTreeNode GetStartDayNode()
        {
            foreach (UltraTreeNode node in contentNodes)
                if (node.Key.StartsWith(DaysSectionNodeKey))
                    return node;

            return null;
        }

        private void ValidateBeforeStart()
        {
            if (_parentNode == null)
                throw new ArgumentException("Parent Node parameter is null.");

            if (_parentNode == null || dayTemplateFile == "")
                throw new ArgumentException("Template file parameter is null.");

            if (_parentNode.Nodes.Count == 0)
                throw new ArgumentException("The list of files to print is empty.");
        }
		
		private static ArrayList GetFileNamesForSection(TreeNodesCollection nodes)
		{
			ArrayList ar = new ArrayList();

			foreach(UltraTreeNode node in nodes)
				if(node.CheckedState == CheckState.Checked &&
					node.Tag != null && node.Tag.ToString().StartsWith(FileNodeTag))
					ar.Add(LayoutHelper.GetFileNodeFileName(node));

			return ar;
		}

        private Document AddNewDocument(bool visible)
        {
            return WordHelper.NewDocument(wordApp, visible, DayTemplateDocument.PageSetup.Orientation);
        }

		private static bool IsValidNode(UltraTreeNode node, string startsWithKey)
		{
			return 
				node.CheckedState == CheckState.Checked &&
				node.Key.StartsWith(startsWithKey);
		}

        public int CountInsertFiles()
        {
            int count = 0;
            if (_parentNode != null)
                AddInsertFileCount(_parentNode, ref count);
            return count;
        }

        private void AddInsertFileCount(UltraTreeNode parentNode, ref int count)
        {
            foreach (UltraTreeNode node in parentNode.Nodes)
            {
                if (node.CheckedState == CheckState.Checked && 
                    node.Tag != null && node.Tag.ToString().StartsWith(FileNodeTag))
                    count++;
                AddInsertFileCount(node, ref count);
            }
        }
	}
}
