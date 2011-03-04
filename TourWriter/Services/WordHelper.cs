using System;
using System.Text.RegularExpressions;
using Word;
namespace TourWriter.Services
{
    /// <summary>
    /// Helper methods for working with MS Word.
    /// </summary>
    class WordHelper
    {
        internal static object StartOfDoc = "\\startofdoc";
        internal static object EndOfDoc = "\\endofdoc";
        internal static object missing = System.Reflection.Missing.Value;
        internal static string MsgFileNotFound = @"FAILED TO INSERT THE SPECIFIED FILE. The file ""{0}"" was not found - check that the file does exist and the file name is correct.";

        internal enum BreakTypes
        {
            PageBreak = 1,
            LineBreak = 2,
            ParagraphBreak = 3,
            None = 4
        }

        internal static Document NewDocument(Application app)
        {
            return app.Documents.Add(
                ref missing, ref missing, ref missing, ref missing);
        }

        internal static Document NewDocument(Application app, string templateFile)
        {
            object template = templateFile;
            return app.Documents.Add(
                ref template, ref missing, ref missing, ref missing);
        }

        internal static Document NewDocument(Application app, bool visible, WdOrientation pageOrientation)
        {
            object isVisible = visible ? true : false;
            Document doc = app.Documents.Add(ref missing, ref missing, ref missing, ref isVisible);
            doc.PageSetup.Orientation = pageOrientation;
            return doc;
        }

        internal static Document NewDocument(Application app, string templateFile, bool visible, WdOrientation pageOrientation)
        {
            object template = templateFile;
            object isVisible = visible ? true : false;
            Document doc = app.Documents.Add(ref template, ref missing, ref missing, ref isVisible);
            doc.PageSetup.Orientation = pageOrientation;
            return doc;
        }

        internal static Document LoadDocument(Application app, string documentPath)
        {
            object visible = false;
            object readOnly = true;
            object path = documentPath;

            return app.Documents.Open(
                ref path, ref missing, ref readOnly, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref visible);
        }

        internal static Document GetSearchSelection(Application app, Document searchDocument, string searchPattern)
        {
            Document document = app.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            Selection selection = GetSearchSelection(searchDocument, searchPattern, true);

            selection.Copy();
            document.Content.Paste();
            return document;
        }

        internal static Selection GetSearchSelection(Document searchDocument, string searchPattern, bool useWildcard)
        {
            object wildcard = useWildcard;
            object yes = true;
            object no = false;
            object pattern = searchPattern;
            object searchAll = WdFindWrap.wdFindContinue;
            object replaceNone = WdReplace.wdReplaceNone;

            searchDocument.Select();
            searchDocument.Application.Selection.Find.Execute(
                ref pattern,	// FindText 
                ref yes,		// MatchCase
                ref yes,		// MatchWholeWord
                ref wildcard,	// MatchWildcards
                ref no,			// MatchSoundsLike
                ref no,			// MatchAllWordForms
                ref yes,		// Forward	
                ref searchAll,	// Wrap			
                ref no,			// Format		
                ref missing,	// ReplaceWith
                ref replaceNone,// Replace 
                ref no,			// MatchKashida
                ref no, 		// MatchDiacritics
                ref no,			// MatchAlefHamza
                ref no			// MatchControl
                );

            return searchDocument.Application.Selection;
        }

        internal static void AppendCopiedContent(Document doc)
        {
            // Go to end of document.
            Range range = doc.Bookmarks.Item(ref EndOfDoc).Range;
            range.Select();

            // Paste pre-copied data.
            range.Paste();
        }

        internal static void AppendDocument(Document srcDoc, Document destDoc, bool disposeSrcDoc)
        {
            if (srcDoc != null)
            {
                srcDoc.Content.Copy();
                AppendCopiedContent(destDoc);

                if (disposeSrcDoc)
                    CloseDocumentWithoutSaving(srcDoc);
            }
        }

        internal static bool FindAndReplace(Document doc, string findText, string replacementText)
        {
            doc.Select();
            return FindAndReplace(doc.Application.Selection, findText, replacementText);
        }

        internal static bool FindAndReplace(Selection selection, string findText, string replacementText)
        {
            object yes = true;
            object no = false;
            object find = findText;
            object searchAll = WdFindWrap.wdFindContinue;
            object replaceNone = WdReplace.wdReplaceNone;
            bool found = false;

            while (true)
            {
                // Select text here. Not using WdReplace.wdReplaceAll as has 255 char limit.
                selection.Find.Execute(
                    ref find, // FindText 
                    ref yes, // MatchCase
                    ref yes, // MatchWholeWord
                    ref no, // MatchWildcards
                    ref no, // MatchSoundsLike
                    ref no, // MatchAllWordForms
                    ref yes, // Forward	
                    ref searchAll, // Wrap			
                    ref no, // Format		
                    ref missing, // ReplaceWith
                    ref replaceNone, // Replace 
                    ref no, // MatchKashida
                    ref no, // MatchDiacritics
                    ref no, // MatchAlefHamza
                    ref no // MatchControl
                    );

                if (!selection.Find.Found)
                    break;

                // Replace selected text here.
                selection.Text = replacementText;
                found = true;
            }

            return found;
        }

        internal static bool FindAndReplace(Document doc, string findText, Document contentDoc, bool setFontToTemplate)
        {
            object yes = true;
            object no = false;
            object find = findText;
            object searchAll = WdFindWrap.wdFindContinue;
            object replaceNone = WdReplace.wdReplaceNone;

            // Get the text to replace
            doc.Select();
            doc.Application.Selection.Find.Execute(
                ref find,		// FindText 
                ref yes,		// MatchCase
                ref yes,		// MatchWholeWord
                ref no,			// MatchWildcards
                ref no,			// MatchSoundsLike
                ref no,			// MatchAllWordForms
                ref yes,		// Forward	
                ref searchAll,	// Wrap			
                ref no,			// Format		
                ref missing,	// ReplaceWith
                ref replaceNone,// Replace 
                ref no,			// MatchKashida
                ref no, 		// MatchDiacritics
                ref no,			// MatchAlefHamza
                ref no			// MatchControl
                );

            if (!doc.Application.Selection.Find.Found)
                return false;
            
            Selection dst = doc.Application.Selection;
            if (contentDoc != null)
            {
                // Get the copy content.
                contentDoc.Content.Select();
                Selection src = contentDoc.Application.Selection;

                if (setFontToTemplate)
                {
                    // Set   to same as placeholder
                    src.Font.Name = dst.Font.Name;
                    src.Font.Size = dst.Font.Size;
                }

                // Copy and paste
                src.Copy();
                dst.Paste();
            }
            else
            {
                // Clear the tag text if no content to replace with.
                dst.Text = "";
                dst.TypeBackspace();
            }
            return true;
        }

        internal static bool FindAndReplaceDate(Document doc, DateTime date)
        {
            // bit of a quick-and-dirty to enable custom formatting of date text

            doc.Select();
            var selection = doc.Application.Selection;

            selection.Find.ClearFormatting();
            selection.Find.MatchWildcards = true;
            selection.Find.Text = @"[\[][\!]DayDate(*)[\]]";

            var found = false;
            while (true)
            {
                object searchAll = WdFindWrap.wdFindContinue;
                object replaceNone = WdReplace.wdReplaceNone;
                selection.Find.Execute(
                    ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref searchAll,
                    ref missing, ref missing, ref replaceNone, ref missing, ref missing, ref missing, ref missing);

                if (!selection.Find.Found) break;

                // get date format string
                const string pattern = @"(?<=\[\!DayDate\((?:\s*)(?:\""|\'|“|‘)).*(?=(?:\""|\'|”|’)\)(?:\s*)\])";
                var format = Regex.Match(selection.Text, pattern).Value;
                if (string.IsNullOrEmpty(format))
                    format = "dd MMMM yyyy";

                selection.Text = date.ToString(format);
                found = true;
            }
            return found;
        }
        
        internal static void CloseDocumentWithoutSaving(Document doc)
        {
            if (doc != null)
            {
                object doSave = false;
                ((Word._Document)doc).Close(ref doSave, ref missing, ref missing);
                doc = null;
            }
        }

        internal static void InsertBreak(Application app, Document doc, BreakTypes breakType)
        {
            doc.Bookmarks.Item(ref EndOfDoc).Select();
            InsertBreak(app.Selection, breakType);
        }

        internal static void InsertBreak(Selection selection, BreakTypes breakType)
        {
            switch (breakType)
            {
                case BreakTypes.PageBreak:
                    {
                        object brk = (int) WdBreakType.wdPageBreak;
                        selection.InsertBreak(ref brk);
                        break;
                    }
                case BreakTypes.LineBreak:
                    {
                        object brk = (int) WdBreakType.wdLineBreak;
                        selection.InsertBreak(ref brk);
                        break;
                    }
                case BreakTypes.ParagraphBreak:
                    {
                        selection.InsertParagraphAfter();
                        break;
                    }
                case BreakTypes.None:
                    {
                        break;
                    }
            }
        }

        internal static void GoToBeginning(Application app, Document doc)
        {
            //doc.Bookmarks.Item(ref StartOfDoc).Select();

            object unit;
            unit = WdUnits.wdStory;
            app.Selection.HomeKey(ref unit, ref missing);
        }

        internal static void GoToEnd(Application app, Document doc)
        {
            //doc.Bookmarks.Item(ref EndOfDoc).Select();

            object unit;
            unit = WdUnits.wdStory;
            app.Selection.EndKey(ref unit, ref missing);
        }

        internal static void PerformDelete(Selection selection, int count)
        {
            object unit = WdUnits.wdCharacter;
            object charCount = count;
            selection.Delete(ref unit, ref charCount);
        }

        internal static void PerformBackspace(Selection selection, int count)
        {
            for (int i = 0; i < count; i++)
                selection.TypeBackspace();

        }

        internal static void FindAndReplaceInParagraphs(Document doc, string findText, string replacementText)
        {
            for (int i = doc.Paragraphs.Count; i > 0; i--)
            {
                Paragraph paragraph = doc.Paragraphs.Item(i);
                bool containsSearchTags = paragraph.Range.Words.Count > 1;

                if (paragraph.Range.Words.Count > 1) // if more than just a paragraph break
                    FindAndReplace(paragraph, findText, replacementText);
            }


            //System.Diagnostics.Debug.WriteLine("===================================");
            //			for(int j=1 ; j<doc.Paragraphs.Count+1; j++)
            //				System.Diagnostics.Debug.WriteLine("Check Para("+j.ToString()+") \"" + doc.Paragraphs.Item(j).Range.Text +"\"");
            //			System.Diagnostics.Debug.WriteLine("Para("+i.ToString()+"): after F&P = \"" + paragraph.Range.Text +"\"");
            //			System.Diagnostics.Debug.WriteLine("Para("+i.ToString()+"): before F&P = \"" + paragraph.Range.Text +"\"");
            //			System.Diagnostics.Debug.WriteLine(
            //				String.Format("{0}->{1} \t (\"{2}\"->\"{3}\")",  
            //				              countBefore, countAfter, findText, replacementText));
            //			System.Diagnostics.Debug.WriteLine("F->R = \"" + findText + "\" -> \"" + replacementText + "\"");
            //			System.Diagnostics.Debug.WriteLine("------------------");


            //			foreach(Paragraph paragraph in doc.Paragraphs)
            //			{
            //				bool isParagraphBreak = paragraph.Range.Words.Count == 1;
            //				
            //				FindAndReplace(paragraph, findText, replacementText);
            //				
            //				// Delete paragraph if findText was only text in paragaph.
            //				if(!isParagraphBreak && paragraph.Range.Words.Count == 1)
            //					paragraph.Range.Delete(ref missing, ref missing);
            //			}
        }

        internal static bool FindAndReplace(Paragraph paragraph, string findText, string replacementText)
        {
            // Warning: paragraph.Range seems to extend the paragraph.Range.Find.Execute 
            // method further than just this paragraph!

            object yes = true;
            object no = false;
            object find = findText;
            object searchAll = WdFindWrap.wdFindContinue;
            object replaceNone = WdReplace.wdReplaceNone;

            // Select text here. Not using WdReplace.wdReplaceAll as has 255 char limit.
            
            paragraph.Range.Select();
            paragraph.Range.Application.Selection.Find.Execute(
                ref find,		// FindText 
                ref yes,		// MatchCase
                ref yes,		// MatchWholeWord
                ref no,			// MatchWildcards
                ref no,			// MatchSoundsLike
                ref no,			// MatchAllWordForms
                ref yes,		// Forward	
                ref searchAll,	// Wrap			
                ref no,			// Format		
                ref missing,	// ReplaceWith
                ref replaceNone,// Replace 
                ref no,			// MatchKashida
                ref no, 		// MatchDiacritics
                ref no,			// MatchAlefHamza
                ref no			// MatchControl
                );

            // Replace selected text here.
            if (paragraph.Range.Application.Selection.Find.Found)
            {
                paragraph.Range.Application.Selection.Text = replacementText;
                return true;
            }
            return false;
        }
        
        internal static void AppendDocument(Application app, Document srcDoc, Document destDoc, bool addSectionBreakBetween)
        {
            if (srcDoc != null)
            {
                Range range = destDoc.Bookmarks.Item(ref EndOfDoc).Range;

                if (addSectionBreakBetween)
                {
                    // this break ensures seperation of formatting between the two contents
                    object brk = WdBreakType.wdSectionBreakContinuous;
                    range.InsertBreak(ref brk);
                }
                srcDoc.Content.Copy();
                range.Paste();
            }
        }

        internal static Object GetEndOfRange(Document doc, Range dataRange)
        {
            Object start = dataRange.End - 1;
            Object end = dataRange.End - 1;
            Range rng = doc.Range(ref start, ref end);
            Object range = rng;
            return range;
        }

        internal static void CreateBookmark(Document doc, string bookmarkName, object bookmarkRange)
        {
            doc.Bookmarks.Add(bookmarkName, ref bookmarkRange);
        }
        
        internal static void SetBookmark(Document doc, object bookmarkName, string text)
        {
            if (doc.Bookmarks.Exists(bookmarkName.ToString()))
                doc.Bookmarks.Item(ref bookmarkName).Range.Text = text;
        }

        internal static void SetBookmark(Application app, Document doc, object bookmarkName, Document contentDoc)
        {
            if (contentDoc != null)
            {
                if (doc.Bookmarks.Exists(bookmarkName.ToString()))
                {
                    contentDoc.Content.Copy();

                    Range range = doc.Bookmarks.Item(ref bookmarkName).Range;
                    range.Select();

                    range.Paste();

                    object collapseDir = WdCollapseDirection.wdCollapseStart;
                    app.Selection.Collapse(ref collapseDir);
                }
            }
        }

        internal static void RemoveTempDocs()
        {
            //			foreach(Document temp in wordApp.Documents)
            //				if(temp != mainDoc)
            //					this.CloseDocumentWithoutSaving(temp);
        }

        internal static void WriteErrorMessage(Selection selection, string message)
        {
            int bold = selection.Font.Bold;
            WdColor color = selection.Font.Color;

            // set error format
            selection.Font.Color = WdColor.wdColorRed;
            selection.Font.Bold = 1; //1 = on, 0 = off

            selection.TypeText(message);

            // reset to original format
            selection.Font.Bold = bold;
            selection.Font.Color = color;

            InsertBreak(selection, BreakTypes.ParagraphBreak);
        }

        internal static void InsertAnyFile(Application app, Selection selection, string filename)
        {
            try
            {
                if (
                    filename.ToLower().EndsWith(".doc") ||
                    filename.ToLower().EndsWith(".rtf"))
                {
                    InsertDocument(selection, filename);
                }
                else if (
                    filename.ToLower().EndsWith(".jpg") ||
                    filename.ToLower().EndsWith(".jpeg") ||
                    filename.ToLower().EndsWith(".gif") ||
                    filename.ToLower().EndsWith(".png") ||
                    filename.ToLower().EndsWith(".bmp"))
                {
                    InsertImage(selection, filename);
                    InsertBreak(selection, BreakTypes.LineBreak);
                }
                else
                {
                    WriteErrorMessage(app.Selection, "File type not supported: " + filename);
                }
            }
            catch (Exception ex)
            {
                if (ErrorHelper.IsFileAccessError(ex))
                {
                    WriteErrorMessage(app.Selection, String.Format(MsgFileNotFound, filename));
                    App.ShowFileNotFound(filename);
                }
                else throw;
            }
        }

        internal static void InsertDocument(Selection selection, string documentFilename)
        {
            selection.InsertFile(documentFilename,
                ref missing, ref missing, ref missing, ref missing);
        }

        internal static void InsertImage(Selection selection, string imageFilename)
        {
            object linkToFile = false;
            object saveWithDoc = true;

            selection.InlineShapes.AddPicture(
                imageFilename, ref linkToFile, ref saveWithDoc, ref missing);
        }

        internal static bool IsWordInstalled(ref Exception ex)
        {
            try
            {
                Application a = new Application();
                return true;
            }
            catch (System.Exception e)
            {
                ex = e;
                return false;
            }
        }
    }
}
