using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;

namespace TourWriter.Modules.ItineraryModule.Publishing
{
    public partial class LayoutControl : UserControl
    {
        private int publisherFileId;

        private ItinerarySet itinerarySet;

        public UltraTree Tree
        {
            get { return treeLayout; }
        }


        public LayoutControl()
        {
            InitializeComponent();
        }

        public void LoadLayout(ItinerarySet itinerary, int pubFileId)
        {
            publisherFileId = pubFileId;
            itinerarySet = itinerary;

            LoadSavedLayout();
            
            RefreshLayoutTree();
        }
        
        private void LoadSavedLayout()
        {
            ItinerarySet.ItineraryPubFileRow file =
                itinerarySet.ItineraryPubFile.FindByItineraryPubFileID(publisherFileId);
            
            if (!file.IsLayoutNull())
                treeLayout.LoadFromBinary(new MemoryStream(file.Layout));
        }

        private void RefreshLayoutTree()
        {
            // Copy existing tree to temp tree
            UltraTree temp = new UltraTree();
            temp.Nodes = treeLayout.Nodes;

            // Notify display of refresh
            if (treeLayout.Nodes.Count > 0)
            {
                treeLayout.Nodes[0].Nodes.Clear();
                treeLayout.Nodes[0].Enabled = false;
                treeLayout.Nodes[0].Text = "Refreshing dates and locations...";
            }

            // Refresh temp tree on thread
            var layout = new LayoutHelper(itinerarySet, temp);
            var thread = new Thread(layout.RebuildLayoutTree) {Name = "PublisherLayoutBuilder", IsBackground = true};
            thread.Start();
            while (thread.IsAlive)
                Application.DoEvents();

            // Update display tree from temp
            treeLayout.SuspendLayout();
            treeLayout.Nodes.Clear();
            treeLayout.Nodes = temp.Nodes;
            temp.Dispose();
            treeLayout.ResumeLayout();

            // Update itinerary name
            if (treeLayout.Nodes.Count > 0)
            {
                ItinerarySet.ItineraryPubFileRow pubFile =
                    itinerarySet.ItineraryPubFile.FindByItineraryPubFileID(publisherFileId);

                treeLayout.Nodes[0].Text = (!pubFile.IsItineraryPubFileNameNull())
                                           ? pubFile.ItineraryPubFileName
                                           : String.Empty;
            }
            // Set display
            treeLayout.Nodes[0].CollapseAll();
            treeLayout.Nodes[0].Expanded = true;
            btnExpand.Text = "Expand";

            // Add custom sort class
            LayoutHelper.RefreshSort(treeLayout);
        }

        internal byte[] GetLayoutBytes()
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                treeLayout.SaveAsBinary(ms);
                return ms.ToArray();
            }
            finally
            {
                ms.Close();
            }
        }

        internal void SaveLayoutFile()
        {
            itinerarySet.ItineraryPubFile.FindByItineraryPubFileID(publisherFileId).
                Layout = GetLayoutBytes();
        }

        private void DeleteFiles(SelectedNodesCollection nodes)
        {
            if (nodes.Count > 0)
                nodes[0].BringIntoView(true);

            if (nodes.Count == 0 || !App.AskDeleteRow())
                return;

            foreach (UltraTreeNode node in nodes)
            {
                if (node.Tag != null && node.Tag.ToString().StartsWith(FileBuilderAdvanced.FileNodeTag))
                {
                    node.Remove();
                    SaveLayoutFile();
                }
            }
        }

        private void OpenFile(UltraTreeNode node)
        {
            node.BringIntoView(true);

            // Open file in default application.
            if (node.Key == "" && node.Tag.ToString().StartsWith(FileBuilderAdvanced.FileNodeTag))
            {
                string fileName = Services.ExternalFilesHelper.ConvertToAbsolutePath(LayoutHelper.GetFileNodeFileName(node));
                if (File.Exists(fileName))
                    Process.Start(fileName);
                else
                    App.ShowFileNotFound(fileName);
            }
        }


        private void treeLayout_AfterCheck(object sender, NodeEventArgs e)
        {
            // Check/uncheck related nodes
            treeLayout.EventManager.AllEventsEnabled = false;
            App.Tree_SynchroniseChildNodes(e.TreeNode);
            App.Tree_SynchroniseParentNodes(e.TreeNode);
            treeLayout.EventManager.AllEventsEnabled = true;
        }

        private void treeLayout_DoubleClick(object sender, EventArgs e)
        {
            if (treeLayout.SelectedNodes.Count > 0)
                OpenFile(treeLayout.SelectedNodes[0]);
        }

        private void treeLayout_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                DeleteFiles(treeLayout.SelectedNodes);
        }


        private void btnFileDelete_Click(object sender, EventArgs e)
        {
            DeleteFiles(treeLayout.SelectedNodes);
        }

        private void btnFileAdd_Click(object sender, EventArgs e)
        {
            ArrayList sectionList = new ArrayList(treeLayout.Nodes[0].Nodes.Count);
            foreach (UltraTreeNode node in treeLayout.Nodes[0].Nodes)
                sectionList.Add(node);

            PublisherFileChooser fc;
            if (LayoutHelper.IsAnySectionNode(treeLayout.ActiveNode))
                fc = new PublisherFileChooser(itinerarySet, sectionList, treeLayout.ActiveNode.Key);
            else if (LayoutHelper.IsFileNode(treeLayout.ActiveNode))
                fc = new PublisherFileChooser(itinerarySet, sectionList, treeLayout.ActiveNode.Parent.Key);
            else
                fc = new PublisherFileChooser(itinerarySet, sectionList);

            if (fc.ShowDialog() == DialogResult.OK)
            {
                UltraTreeNode parentNode = treeLayout.Nodes[0].Nodes[fc.GetSelectedSectionKey()];
                if (parentNode != null)
                {
                    treeLayout.SelectedNodes.Clear();
                    treeLayout.Override.SelectionType = SelectType.Extended;

                    // add the new nodes
                    parentNode.Expanded = true;
                    UltraTreeNode addNode;
                    foreach (UltraTreeNode node in fc.GetSelectedFiles())
                    {
                        addNode = (UltraTreeNode) node.Clone();
                        parentNode.Nodes.Add(addNode);
                        addNode.CheckedState = CheckState.Unchecked;
                        addNode.Selected = true;
                        addNode.BringIntoView();
                    }

                    // provide visual feedback by selecting the added nodes for 1 second
                    Application.DoEvents();
                    Thread.Sleep(1000); // for visual feedback
                    treeLayout.SelectedNodes.Clear();
                    treeLayout.Override.SelectionType = SelectType.Single;

                    SaveLayoutFile();
                }
            }
        }

        private void btnFileEdit_Click(object sender, EventArgs e)
        {
            if (treeLayout.SelectedNodes.Count > 0)
                OpenFile(treeLayout.SelectedNodes[0]);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (treeLayout.ActiveNode == null || !LayoutHelper.IsFileNode(treeLayout.ActiveNode))
                return;

            UltraTreeNode node = treeLayout.ActiveNode;
            node.BringIntoView(true);

            if (node.Index > 0)
            {
                node.Reposition(node.Parent.Nodes, node.Index - 1);
            }
            else if (node.Parent.Index > 0)
            {
                TreeNodesCollection collection =
                    node.Parent.Parent.Nodes[node.Parent.Index - 1].Nodes;
                node.Reposition(collection, collection.Count > 0 ? collection.Count : 0);
                collection.ParentNode.Expanded = true;
            }
            treeLayout.ActiveNode = node;
            node.Selected = true;
            node.BringIntoView();
            SaveLayoutFile();
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (treeLayout.ActiveNode == null || !LayoutHelper.IsFileNode(treeLayout.ActiveNode))
                return;

            UltraTreeNode node = treeLayout.ActiveNode;
            node.BringIntoView(true);

            if (node.Index < node.Parent.Nodes.Count - 1)
            {
                node.Reposition(node.Parent.Nodes, node.Index + 1);
            }
            else if (node.Parent.Index < node.Parent.Parent.Nodes.Count - 1)
            {
                TreeNodesCollection collection =
                    node.Parent.Parent.Nodes[node.Parent.Index + 1].Nodes;
                node.Reposition(collection, 0);
                collection.ParentNode.Expanded = true;
            }
            treeLayout.ActiveNode = node;
            node.Selected = true;
            node.BringIntoView();
            SaveLayoutFile();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (treeLayout.Nodes.Count == 0)
                return;

            if (btnExpand.Text == "Expand")
            {
                btnExpand.Text = "Collapse";
                treeLayout.ExpandAll();

                //if (treeLayout.Nodes.Count > 0)
                //    treeLayout.Nodes[0].ExpandAll(ExpandAllType.OnlyNodesWithChildren);
            }
            else if (btnExpand.Text == "Collapse")
            {
                btnExpand.Text = "Expand";
                //treeLayout.Nodes[0].CollapseAll();

                //treeLayout.CollapseAll();
                //if (treeLayout.Nodes.Count > 0)
                foreach (UltraTreeNode node in treeLayout.Nodes[0].Nodes)
                    node.CollapseAll();
            }
            treeLayout.Refresh();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshLayoutTree();
        }
    }
}