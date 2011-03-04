using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using TourWriter.Services;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
    class DocumentWriter
    {
        #region Properties

        private static List<string> introFileList;
        private static List<string> endingFileList;
        private static List<PublisherDayInfo> daysList;

        // search and replace tags, these strings are searched for in the day template document
        private const string MergeFieldDayNumber = "[!DayNumber]";
        private const string MergeFieldDayName = "[!DayName]";
        //private const string MergeFieldDayDate = "[!DayDate]"; // see WordHelper.FindAndReplaceDate()
        private const string MergeFieldDayLocation = "[!DayLocation]";
        private const string MergeFieldDayText = "[!DayText]";
        private const string MergeFieldPageBreak = "[!PageBreak]";
        private const string MergeFieldColumnBreak = "[!ColumnBreak]";
        // node keys, each node has key set to one of these type indicators 
        private const string FrontSectionNodeKey = "Front";
        private const string DaysSectionNodeKey = "Day";
        private const string BackSectionNodeKey = "Back";
        private const string FileNodeTag = "File";

        #endregion

        public static void CreateDoc(TreeNodesCollection nodes, string dayLayoutFile)
        {
            ProcessFilePaths(nodes);

            CreateDocument(dayLayoutFile);
        }

        #region File path processing

        private static void ProcessFilePaths(TreeNodesCollection nodes)
        {
            // get intro files list
            introFileList = GetFileListFromNodeCollection(nodes[FrontSectionNodeKey].Nodes);

            // get ending files list
            endingFileList = GetFileListFromNodeCollection(nodes[BackSectionNodeKey].Nodes);

            // get list of days and files list for each day
            daysList = new List<PublisherDayInfo>();
            foreach (UltraTreeNode dayNode in nodes)
            {
                if (dayNode.Key.StartsWith(DaysSectionNodeKey) && dayNode.CheckedState == CheckState.Checked)
                {
                    DateTime date = DateTime.Parse(dayNode.Key.Split('~')[1].Trim());
                    string location = dayNode.Text.Split(':')[1].Trim();
                    List<string> filesList = GetFileListFromNodeCollection(dayNode.Nodes);

                    daysList.Add(new PublisherDayInfo(date, location, filesList));
                }
            }
        }

        private static List<string> GetFileListFromNodeCollection(TreeNodesCollection nodes)
        {
            List<string> list = new List<string>();

            foreach (UltraTreeNode node in nodes)
            {
                if (node.CheckedState == CheckState.Checked &&
                    node.Tag != null &&
                    node.Tag.ToString().StartsWith(FileNodeTag)) // is it a file node
                {
                    string file = node.Tag.ToString().Split('~')[1].Trim();
                    list.Add(ExternalFilesHelper.ConvertToAbsolutePath(file));
                }
            }
            return list;
        }

        #endregion

        #region Document creation

        private static void CreateDocument(string dayLayoutFile)
        {
            object no = false;
            object yes = true;
            object startOfDoc = "\\startofdoc";
            object endOfDoc = "\\endofdoc";
            object story = Word.WdUnits.wdStory;
            object pageBreak = Word.WdBreakType.wdPageBreak;
            object columnBreak = Word.WdBreakType.wdColumnBreak;
            object missing = System.Reflection.Missing.Value;
            object dayLayout = dayLayoutFile;

            // create new document
            var wordApp = new Word.Application();
            var document = wordApp.Documents.Add(ref dayLayout, ref missing, ref missing, ref yes);

            // write intro files
            var introFileListReverse = new List<string>(introFileList);
            introFileListReverse.Reverse();

            foreach (string introFile in introFileListReverse)
            {
                document.Bookmarks.Item(ref startOfDoc).Range.InsertBreak(ref pageBreak);
                document.Bookmarks.Item(ref startOfDoc).Range.InsertFile(introFile, ref missing, ref no, ref no, ref no);
            }

            // write days files
            foreach (var day in daysList)
            {
                //var inputCulture = Application.CurrentInputLanguage.Culture;

                // populate tags
                if (Find(document, MergeFieldDayNumber))
                    document.Application.Selection.Text = (daysList.IndexOf(day) + 1).ToString();
                if (Find(document, MergeFieldDayName))
                    document.Application.Selection.Text = day.Date.ToString("dddd", new System.Globalization.CultureInfo("en-GB"));//, inputCulture);

                WordHelper.FindAndReplaceDate(document, day.Date);
                //if (Find(document, MergeFieldDayDate))
                //    document.Application.Selection.Text = day.Date.ToString(dateFormat, new System.Globalization.CultureInfo("en-GB"));//, inputCulture);
                
                if (Find(document, MergeFieldDayLocation))
                    document.Application.Selection.Text = day.Location;
                if (Find(document, MergeFieldPageBreak))
                    document.Application.Selection.InsertBreak(ref pageBreak);
                if (Find(document, MergeFieldColumnBreak))
                    document.Application.Selection.InsertBreak(ref columnBreak);
                if (Find(document, MergeFieldDayText))
                {
                    string dayTextFontName = document.Application.Selection.Font.Name;

                    // to set cursor position for files
                    document.Application.Selection.Text = "";

                    object dayTextRangeStart = document.Application.Selection.Start;

                    // add day files
                    foreach (string filePath in day.FileList)
                    {
                        try
                        {
                            document.Application.Selection.InsertFile(filePath, ref missing, ref no, ref no, ref no);
                        }
                        catch (Exception ex)
                        {
                            if (ErrorHelper.IsFileAccessError(ex))
                            {
                                WordHelper.WriteErrorMessage(wordApp.Selection, String.Format(WordHelper.MsgFileNotFound, filePath));
                                App.ShowFileNotFound(filePath);
                            }
                            else throw;
                        }

                        if (day.FileList.IndexOf(filePath) < day.FileList.Count - 1)
                            document.Application.Selection.InsertParagraphAfter();
                    }

                    object dayTextRangeEnd = document.Application.Selection.End;
                    
                    // set day text font
                    document.Range(ref dayTextRangeStart, ref dayTextRangeEnd).Font.Name = dayTextFontName;
                }

                if (daysList.IndexOf(day) < daysList.Count - 1)
                {
                    document.Bookmarks.Item(ref endOfDoc).Range.InsertFile(dayLayoutFile, ref missing, ref no, ref no, ref no);
                }
            }

            // write ending files
            foreach (string filePath in endingFileList)
            {
                document.Bookmarks.Item(ref endOfDoc).Range.InsertFile(filePath, ref missing, ref no, ref no, ref no);
                document.Bookmarks.Item(ref endOfDoc).Range.InsertBreak(ref pageBreak);
            }

            // show document start page
            wordApp.Selection.HomeKey(ref story, ref missing);
            wordApp.Visible = true;
        }

        private static bool Find(Word._Document doc, string text)
        {
            object yes = true;
            object no = false;
            object find = text;
		    object missing = System.Reflection.Missing.Value;
            object searchAll = Word.WdFindWrap.wdFindContinue;
            object replaceNone = Word.WdReplace.wdReplaceNone;

            doc.Select();
            doc.Application.Selection.Range.Find.ClearFormatting();

            return doc.Application.Selection.Find.Execute(
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
        }

        #endregion

        /// <summary>
        /// Returns a list of all the file paths.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // write intro files
            foreach (string filePath in introFileList)
                sb.AppendLine(filePath);

            if (sb.Length > 0)
                sb.AppendLine();

            // write days files
            foreach (PublisherDayInfo day in daysList)
            {
                foreach (string filePath in day.FileList)
                    sb.AppendLine(filePath);
                if (sb.Length > 0)
                    sb.AppendLine();
            }

            // write ending files
            foreach (string filePath in endingFileList)
                sb.AppendLine(filePath);

            return sb.ToString();
        }
    }

    class PublisherDayInfo
    {
        public DateTime Date;
        public string Location;
        public List<string> FileList;

        public PublisherDayInfo(DateTime date, string location, List<string> fileList)
        {
            Date = date;
            Location = location;
            FileList = fileList;
        }
    }
}
