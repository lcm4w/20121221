using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Progress;
using TourWriter.Properties;
using TourWriter.Services;
using CellClickAction=Infragistics.Win.UltraWinGrid.CellClickAction;
using CellEventArgs=Infragistics.Win.UltraWinGrid.CellEventArgs;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
    /// <summary>
    /// Summary description for Publisher.
    /// </summary>
    public partial class PublisherMain : UserControl
    {
        private ItinerarySet itinerarySet;

        public ItinerarySet ItinerarySet
        {
            set
            {
                itinerarySet = value;
                DataBind();
            }
        }


        public PublisherMain()
        {
            InitializeComponent();

            txtSummaryTemplate.Text = Settings.Default.SummaryPublishingTemplate;
        }

        internal void DataBind()
        {
            gridAdvanced.SetDataBinding(itinerarySet, "Itinerary.ItineraryItineraryPubFile");
        }

        private void OpenLayoutForm(int publisherFileId)
        {
            LayoutForm form = new LayoutForm(itinerarySet, publisherFileId);
            form.ShowDialog();
            form.Dispose();
        }
             

        private void btnSummaryTemplate_Click(object sender, EventArgs e)
        {
            ChooseSummaryTemplate();
        }

        private void btnCreateSummary_Click(object sender, EventArgs e)
        {
            Exception ex = null;
            if (!WordHelper.IsWordInstalled(ref ex))
            {
                throw new Exception("Microsoft Word is not installed, or could not be started", ex);
            }
            if (!File.Exists(ExternalFilesHelper.ConvertToAbsolutePath(txtSummaryTemplate.Text)))
            {
                if (!ChooseSummaryTemplate())
                    return;
            }

        // Sync.
            //new PublisherSummary(itinerarySet, txtSummaryTemplate.Text, this).CreateDocument();

            // OR...
            
            // Async.
            ProgressWindow progress = new ProgressWindow();
            progress.Text = "Create summary publishing document";
            ThreadPool.QueueUserWorkItem(new WaitCallback(SummaryDocument_ThreadStart), progress);
            progress.ShowDialog();
        }

        private bool ChooseSummaryTemplate()
        {
            string fileFilter = "Word objects (*.doc;*.docx;*.rtf)|*.doc;*.docx;*.rtf|All files (*.*)|*.*";
            string file = App.SelectExternalFile(true, "Choose template file", fileFilter, 1);

            if (file != null && file != txtSummaryTemplate.Text)
            {
                txtSummaryTemplate.Text = file;
                Settings.Default.SummaryPublishingTemplate = file;
                Settings.Default.Save();
                return true;
            }
            return false;
        }
        

        private void gridAdvanced_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.Insert(0, "Browse");
            
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ItineraryPubFileName")
                {
                    c.Width = 100;
                    c.Header.Caption = "Description";
                    c.Header.ToolTipText = "Description of publishing file";
                    c.Band.SortedColumns.Add(c, false);
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "DayTemplateFile")
                {
                    c.Width = 150;
                    c.Header.Caption = "Template";
                    c.Header.ToolTipText = "Template file to use";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "Browse")
                {
                    c.Width = 35;
                    c.MinWidth = 35;
                    c.MaxWidth = 35;
                    c.Header.Caption = "";
                    c.Header.ToolTipText = "Browse for template";
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    c.CellButtonAppearance.Image = TourWriter.Properties.Resources.FolderExplore;
                    c.CellButtonAppearance.ImageHAlign = HAlign.Center;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                }
                else
                    c.Hidden = true;
            }
            int index = 0;
            e.Layout.Bands[0].Columns["ItineraryPubFileName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["DayTemplateFile"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Browse"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void gridAdvanced_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() != typeof(UltraGridEmptyRow))
                OpenLayoutForm((int)e.Row.Cells["ItineraryPubFileID"].Value);
        }

        private void gridAdvanced_ClickCellButton(object sender, CellEventArgs e)
        {
            string fileFilter = "Word objects (*.doc;*.docx;*.rtf)|*.doc;*.docx;*.rtf|All files (*.*)|*.*";
            string file = App.SelectExternalFile(true, "Choose template file", fileFilter, 1);

            if (file != null && file != e.Cell.Row.Cells["DayTemplateFile"].Value.ToString())
            {
                e.Cell.Row.Cells["DayTemplateFile"].Value = file;
                gridAdvanced.UpdateData();
            }
        }
        
        private void btnAdvancedEdit_Click(object sender, EventArgs e)
        {
            if (gridAdvanced.ActiveRow != null)
                OpenLayoutForm((int)gridAdvanced.ActiveRow.Cells["ItineraryPubFileID"].Value);
        }

        private void btnAdvancedAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                string fileName = itinerarySet.Itinerary[0].ItineraryName + " itinerary";
                ItinerarySet.ItineraryPubFileRow r = itinerarySet.ItineraryPubFile.NewItineraryPubFileRow();
                r.ItineraryID = itinerarySet.Itinerary[0].ItineraryID;
                r.ItineraryPubFileName =
                    App.CreateUniqueNameValue(gridAdvanced.Rows, "ItineraryPubFileName", fileName);
                r.AddedOn = DateTime.Now;
                r.AddedBy = Cache.User.UserID;

                itinerarySet.ItineraryPubFile.AddItineraryPubFileRow(r);

                GridHelper.SetActiveRow(
                    gridAdvanced, "ItineraryPubFileID", r.ItineraryPubFileID, "ItineraryPubFileName");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnAdvancedDelete_Click(object sender, EventArgs e)
        {
            UltraGridRow row;
            row = gridAdvanced.ActiveRow;
            if (row != null && App.AskDeleteRow())
            {
                // delete
                int i = row.Index;
                row.Delete(false);
                // select next row
                if ((i > gridAdvanced.Rows.Count - 1 ? --i : i) > -1)
                    // activate to load new layout
                    gridAdvanced.ActiveRow = gridAdvanced.Rows[i];
            }
        }

        private void btnAdvancedCopy_Click(object sender, EventArgs e)
        {
            if (gridAdvanced.ActiveRow == null)
                return;

            ItinerarySet.ItineraryPubFileRow copyRow = itinerarySet.ItineraryPubFile.FindByItineraryPubFileID(
                (int)gridAdvanced.ActiveRow.Cells["ItineraryPubFileID"].Value);

            if (App.AskYesNo("Copy booking item: " + copyRow.ItineraryPubFileName + "?"))
            {
                ItinerarySet.ItineraryPubFileRow newRow =
                    itinerarySet.CopyPubFile(
                        copyRow, "Copy of", TourWriter.Global.Cache.User.UserID);

                GridHelper.SetActiveRow(gridAdvanced, "ItineraryPubFileID", newRow.ItineraryPubFileID, "ItineraryPubFileName");
            }
        }
                
        private void btnCreateAdvanced_Click(object sender, EventArgs e)
        {
            // validate
            Exception ex = null;
            if (!WordHelper.IsWordInstalled(ref ex))
            {
                throw new Exception("Microsoft Word is not installed, or could not be started", ex);
            }
            if (gridAdvanced.ActiveRow == null)
            {
                App.ShowError("You have not yet added a publishing file.");
                return;
            }
            ItinerarySet.ItineraryPubFileRow pubFile =
                itinerarySet.ItineraryPubFile.FindByItineraryPubFileID(
                    (int)gridAdvanced.ActiveRow.Cells["ItineraryPubFileID"].Value);
            if (pubFile.IsLayoutNull())
            {
                App.ShowError("You have not yet edited the layout for the selected publishing file.");
                return;
            }
            if (!ChooseAdvancedTemplate(pubFile))
                return;

            // process
            bool useNewDocumentProcessing = chkUseNewDocCreator.Checked;
            if (useNewDocumentProcessing)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    CreateDocument_NEW();
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            else
            {
                CreateDocument();
            }
        }

        private void btnBuildAdvanced_Click(object sender, EventArgs e)
        {
            if (gridAdvanced.ActiveRow == null)
            {
                App.ShowError("You have not yet added a publishing file.");
                return;
            }
            OpenLayoutForm((int)gridAdvanced.ActiveRow.Cells["ItineraryPubFileID"].Value);
        }

        private static bool ChooseAdvancedTemplate(ItinerarySet.ItineraryPubFileRow pubFile)
        {
            bool fileNotFound =
                pubFile.IsDayTemplateFileNull()
                || pubFile.DayTemplateFile.Trim() == ""
                || !File.Exists(ExternalFilesHelper.ConvertToAbsolutePath(pubFile.DayTemplateFile));

            if (fileNotFound)
            {
                string fileFilter = "Word objects (*.doc;*.docx;*.rtf)|*.doc;*.docx;*.rtf|All files (*.*)|*.*";
                string file = App.SelectExternalFile(true, "Choose template file", fileFilter, 1);

                if (file != null)
                    pubFile.DayTemplateFile = file;
                else
                    return false;
            }
            return true;
        }

        #region Create doc

        private void CreateDocument()
        {
            ProgressWindow progress = new ProgressWindow();
            progress.Text = "Create advanced publishing document";
            ThreadPool.QueueUserWorkItem(new WaitCallback(AdvancedDocument_ThreadStart), progress);
            progress.ShowDialog();
        }

        #region Document thread properties
        private int progressMax;
        private int progressCount;
        private IProgressCallback callback;
        public delegate void DelegateThreadFinished();
        public delegate void DelegateThreadUpdate();
        public DelegateThreadFinished m_DelegateThreadFinished;
        public DelegateThreadUpdate m_DelegateThreadUpdate;
        #endregion

        #region Summary publishing document

        FileBuilderSimple publisherSummary = null;
        
        private void SummaryDocument_ThreadStart(object status)
        {
            callback = status as IProgressCallback;

            publisherSummary = new FileBuilderSimple(
                itinerarySet, ExternalFilesHelper.ConvertToAbsolutePath(txtSummaryTemplate.Text), this);

            m_DelegateThreadFinished = new DelegateThreadFinished(SummaryDocument_ThreadFinished);
            m_DelegateThreadUpdate = new DelegateThreadUpdate(SummaryDocument_ThreadUpdate);

            progressCount = 1;
            progressMax = publisherSummary.CountInsertFiles();
            callback.Begin(0, progressMax > 0 ? progressMax : 1);
            callback.SetText("Starting Word document...");

            publisherSummary.CreateDocument();
        }

        private void SummaryDocument_ThreadFinished()
        {
            if (callback != null)
                callback.End();

            Clipboard.Clear();
        }

        private void SummaryDocument_ThreadUpdate()
        {
            if (callback != null && !callback.IsAborting)
            {
                callback.SetText(String.Format("Inserting booking item {0} of {1}", progressCount++, progressMax));
                callback.Increment(1);
            }
            else
            {
                publisherSummary.CancelProcessing();
                callback.End();
            }
        }

        #endregion

        #region Advanced publishing document

        private FileBuilderAdvanced PublisherAdvanced = null;
        
        private void AdvancedDocument_ThreadStart(object status)
        {
            int publisherFileId = (int)gridAdvanced.ActiveRow.Cells["ItineraryPubFileId"].Value;
            
            ItinerarySet.ItineraryPubFileRow file =
                itinerarySet.ItineraryPubFile.FindByItineraryPubFileID(publisherFileId);
            UltraTree tree = new UltraTree();
            
            tree.LoadFromBinary(new MemoryStream(file.Layout));
            LayoutHelper.RefreshSort(tree);

            callback = status as IProgressCallback;

            string templateFile = ExternalFilesHelper.ConvertToAbsolutePath(
                gridAdvanced.ActiveRow.Cells["DayTemplateFile"].Value.ToString());
            PublisherAdvanced = new FileBuilderAdvanced(tree.Nodes[0], templateFile, this, chkSetFont.Checked);

            m_DelegateThreadFinished = new DelegateThreadFinished(AdvancedDocument_ThreadFinished);
            m_DelegateThreadUpdate = new DelegateThreadUpdate(AdvancedDocument_ThreadUpdate);

            progressCount = 1;
            progressMax = PublisherAdvanced.CountInsertFiles();
            callback.Begin(0, progressMax > 0 ? progressMax : 1);
            callback.SetText("Starting Word document...");

            PublisherAdvanced.CreateDocument();
        }

        private void AdvancedDocument_ThreadFinished()
        {
            if (callback != null)
                callback.End();

            Clipboard.Clear();
        }

        private void AdvancedDocument_ThreadUpdate()
        {
            if (callback != null && !callback.IsAborting)
            {
                callback.SetText(String.Format("Inserting Word file {0} of {1}", progressCount++, progressMax));
                callback.Increment(1);
            }
            else
            {
                PublisherAdvanced.CancelProcessing();
                callback.End();
            }
        }

        #endregion

        #endregion

        #region Create doc NEW - testing new processing for advanced doc creation

        private void CreateDocument_NEW()
        {
            string templateFile = ExternalFilesHelper.ConvertToAbsolutePath(
                gridAdvanced.ActiveRow.Cells["DayTemplateFile"].Value.ToString());

            int publisherFileId = (int)gridAdvanced.ActiveRow.Cells["ItineraryPubFileId"].Value;
            ItinerarySet.ItineraryPubFileRow file =
                itinerarySet.ItineraryPubFile.FindByItineraryPubFileID(publisherFileId);

            UltraTree tree = new UltraTree();
            tree.LoadFromBinary(new MemoryStream(file.Layout));
            LayoutHelper.RefreshSort(tree);

            DocumentWriter.CreateDoc(tree.Nodes[0].Nodes, templateFile);
            return;
        }

        private void chkUseNewDocCreator_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseNewDocCreator.Checked &&
                !App.AskYesNo(
                "Warning: new document creator functionality is still in BETA testing.\r\n\r\nUse anyway?"))
            {
                chkUseNewDocCreator.Checked = false;
            }
        }

        #endregion
    }
}