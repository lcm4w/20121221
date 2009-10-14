using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Properties;

namespace TourWriter.UserControls.Reports
{
    public partial class ExplorerControl : UserControl
    {
        public event EventHandler<ReportDoubleClickEventArgs> ReportDoubleClick;

        public ExplorerControl()
        {
            InitializeComponent();

            if (!App.IsInDesignMode)
            {
                InitializeImageList();
                InitializeReportTree();
            }
        }

        public void FilterReportTree(string categoryName)
        {
            for (int i = treeReports.Nodes.Count - 1; i >= 0; i--)
            {
                var node = treeReports.Nodes[i];
                if (node.Text != categoryName)
                {
                    node.Remove();
                }
            }

            if (treeReports.Nodes.Count > 0)
                treeReports.Nodes[0].Expand();
        }

        public void RebuildReportTree()
        {
            InitializeReportTree();
        }

        private void InitializeReportTree()
        {
            treeReports.Nodes.Clear();
            LoadReportCategories(treeReports.Nodes);
            LoadReports(treeReports.Nodes);
            RepositionNodes(treeReports.Nodes);
            treeReports.TreeViewNodeSorter = new TreeNodeComparer();
        }

        private void InitializeImageList()
        {
            treeReportsImageList.Images.Add("Folder", Resources.Folder);
            treeReportsImageList.Images.Add("Report", Resources.Report);
        }

        private void LoadReportCategories(TreeNodeCollection nodes)
        {
            foreach (var row in Cache.ToolSet.TemplateCategory)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;

                var node = new TreeNode();
                node.Name = "ReportCategory." + row.TemplateCategoryID;
                node.Text = row.TemplateCategoryName;
                node.SelectedImageKey = node.ImageKey = "Folder";
                node.ContextMenuStrip = treeReportsContextMenu;
                if (!row.IsParentTemplateCategoryIDNull())
                    node.Tag = row.ParentTemplateCategoryID;

                nodes.Add(node);
            }
        }

        private void LoadReports(TreeNodeCollection nodes)
        {
            foreach (var row in Cache.ToolSet.Template)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;

                AddReportNode(nodes, row);
            }
        }

        private void AddReportNode(TreeNodeCollection nodes, ToolSet.TemplateRow report)
        {
            var node = new TreeNode();
            node.Name = "Report." + report.TemplateID;
            node.Text = report.TemplateName;
            node.Tag = report.ParentTemplateCategoryID;
            node.SelectedImageKey = node.ImageKey = "Report";
            node.ContextMenuStrip = treeReportsContextMenu;

            nodes.Add(node);
        }

        private void AddCategoryNode(TreeNodeCollection nodes, ToolSet.TemplateCategoryRow category)
        {
            var node = new TreeNode();
            node.Name = "ReportCategory." + category.TemplateCategoryID;
            node.Text = category.TemplateCategoryName;
            node.Tag = category.ParentTemplateCategoryID;
            node.SelectedImageKey = node.ImageKey = "Folder";
            node.ContextMenuStrip = treeReportsContextMenu;

            nodes.Add(node);
        }

        private static void RepositionNodes(TreeNodeCollection nodes)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                var node = nodes[i];
                if (node.Tag == null)
                    continue;

                var categoryId = (int)node.Tag;
                string key = "ReportCategory." + categoryId;
                var nodesFound = nodes.Find(key, true);
                if (nodesFound.Length == 0)
                    continue;

                var parentNode = nodesFound[0];

                // move the node
                node.Remove();
                parentNode.Nodes.Add(node);
            }
            if (nodes.Count > 0) nodes[0].Expand();
        }

        private static TreeNode GetReportCategoryNode(TreeNode node)
        {
            if (!IsReportNode(node))
                return node;

            return node.Parent;
        }

        private static int GetNodeID(TreeNode node)
        {
            string prefix = (IsReportNode(node)) ? "Report." : "ReportCategory.";
            return Convert.ToInt32(node.Name.Replace(prefix, String.Empty));
        }

        private static ToolSet.TemplateRow GetReportRow(TreeNode node)
        {
            if (!IsReportNode(node))
                return null;

            int reportId = GetNodeID(node);
            var report = Cache.ToolSet.Template.FindByTemplateID(reportId);
            return report;
        }

        private static ToolSet.TemplateCategoryRow GetReportCategoryRow(TreeNode node)
        {
            if (IsReportNode(node))
                return null;

            int categoryId = GetNodeID(node);
            var category = Cache.ToolSet.TemplateCategory.FindByTemplateCategoryID(categoryId);
            return category;
        }

        private static bool IsReportNode(TreeNode node)
        {
            return (node.ImageKey == "Report");
        }

        private void AddReport(string reportFile, TreeNode parentCategoryNode)
        {
            int categoryId = GetNodeID(parentCategoryNode);

            var newReport = Cache.ToolSet.Template.NewTemplateRow();
            newReport.ParentTemplateCategoryID = categoryId;
            newReport.FilePath = reportFile;
            newReport.TemplateName = Path.GetFileNameWithoutExtension(reportFile);
            Cache.ToolSet.Template.AddTemplateRow(newReport);

            // add the new report to the tree
            AddReportNode(parentCategoryNode.Nodes, newReport);
            parentCategoryNode.Expand();
        }

        private void AddCategory(TreeNode parentCategoryNode)
        {
            int categoryId = GetNodeID(parentCategoryNode);

            var newCategory = Cache.ToolSet.TemplateCategory.NewTemplateCategoryRow();
            newCategory.ParentTemplateCategoryID = categoryId;
            newCategory.TemplateCategoryName = "New Category";
            Cache.ToolSet.TemplateCategory.AddTemplateCategoryRow(newCategory);

            // add the new report to the tree
            AddCategoryNode(parentCategoryNode.Nodes, newCategory);
            parentCategoryNode.Expand();
        }

        private void AddReportsToSelectedCategory()
        {
            var node = treeReports.SelectedNode ?? treeReports.Nodes[0];
            var categoryNode = GetReportCategoryNode(node);

            string[] reportFiles = App.SelectExternalFiles(true, "Select report file(s)", "Reports (*.rdlc)|*.rdlc|All files (*.*)|*.*", 0);
            if (reportFiles == null)
                return;

            foreach (string reportFile in reportFiles)
            {
                AddReport(reportFile, categoryNode);
            }
        }

        private void DeleteSelectedNode()
        {
            var node = treeReports.SelectedNode;
            if (node == null)
            {
                App.ShowError("Please select a node to delete.");
                return;
            }
            if (node == treeReports.Nodes[0])
            {
                App.ShowError("You cannot delete the root node.");
                return;
            }

            if (!App.AskDeleteRow())
                return;

            if (node.Nodes.Count > 0)
            {
                App.ShowError("Cannot delete folder as it is not empty, delete the folder contents first.");
                return;
            }

            // remove from tree
            node.Remove();

            // remove from dataset
            if (IsReportNode(node))
            {
                int reportId = Convert.ToInt32(node.Name.Replace("Report.", String.Empty));
                var report = Cache.ToolSet.Template.FindByTemplateID(reportId);
                report.Delete();
            }
            else
            {
                int categoryId = Convert.ToInt32(node.Name.Replace("ReportCategory.", String.Empty));
                var category = Cache.ToolSet.TemplateCategory.FindByTemplateCategoryID(categoryId);
                if (category != null) category.Delete();
            }
        }

        private void ChangeSelectedReportFile()
        {
            string reportFile = App.SelectExternalFile(true, "Select report file(s)", "Reports (*.rdlc)|*.rdlc|All files (*.*)|*.*", 0);
            if (String.IsNullOrEmpty(reportFile))
                return;

            var report = GetReportRow(treeReports.SelectedNode);
            report.FilePath = reportFile;
        }

        #region Events

        private void menuAddReport_Click(object sender, EventArgs e)
        {
            AddReportsToSelectedCategory();
        }

        private void menuAddCategory_Click(object sender, EventArgs e)
        {
            var node = treeReports.SelectedNode ?? treeReports.Nodes[0];
            var categoryNode = GetReportCategoryNode(node);
            AddCategory(categoryNode);
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            DeleteSelectedNode();
        }

        private void menuRename_Click(object sender, EventArgs e)
        {
            treeReports.SelectedNode.BeginEdit();
        }

        private void menuChangeReportFile_Click(object sender, EventArgs e)
        {
            ChangeSelectedReportFile();
        }

        private void treeReports_ItemDrag(object sender, ItemDragEventArgs e)
        {
            treeReports.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeReports_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeReports_DragDrop(object sender, DragEventArgs e)
        {
            var location = treeReports.PointToClient(new Point(e.X, e.Y));
            var node = (TreeNode)e.Data.GetData(typeof(TreeNode));
            var destNode = treeReports.GetNodeAt(location);
            var destCategoryNode = GetReportCategoryNode(destNode);

            // don't move the node if the destination node is a descendant of the node being moved
            var nodesFound = node.Nodes.Find(destCategoryNode.Name, true);
            if (nodesFound.Length > 0)
                return;

            // move the node
            node.Remove();
            destCategoryNode.Nodes.Add(node);
            destCategoryNode.Expand();

            // update the dataset to reflect the node's new position
            int destCategoryId = GetNodeID(destCategoryNode);
            if (IsReportNode(node))
            {
                int reportId = GetNodeID(node);
                var report = Cache.ToolSet.Template.FindByTemplateID(reportId);
                report.ParentTemplateCategoryID = destCategoryId;
            }
            else
            {
                int categoryId = GetNodeID(node);
                var category = Cache.ToolSet.TemplateCategory.FindByTemplateCategoryID(categoryId);
                category.ParentTemplateCategoryID = destCategoryId;
            }
        }

        private void treeReports_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!IsReportNode(e.Node))
                return;

            var report = GetReportRow(e.Node);
            string reportName = report.TemplateName;
            string reportPath = Cache.ToolSet.AppSettings[0].ExternalFilesPath + report.FilePath;
            ReportDoubleClick(this, new ReportDoubleClickEventArgs(reportName, reportPath));
        }

        private void treeReports_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            treeReports.SelectedNode = e.Node;

            e.Node.ContextMenuStrip.Items["menuChangeReportFile"].Visible = IsReportNode(e.Node);
            e.Node.ContextMenuStrip.Items["menuDelete"].Visible = e.Node != treeReports.Nodes[0];
            e.Node.ContextMenuStrip.Items["menuRename"].Visible = e.Node != treeReports.Nodes[0];
        }

        private void treeReports_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null)
                return;

            if (IsReportNode(e.Node))
            {
                var report = GetReportRow(e.Node);
                report.TemplateName = e.Label;
            }
            else
            {
                var category = GetReportCategoryRow(e.Node);
                category.TemplateCategoryName = e.Label;
            }
        }

        private void treeReports_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node == treeReports.Nodes[0])
                e.CancelEdit = true;
        }

        #endregion

        private class TreeNodeComparer : IComparer
        {
            public int Compare(object obj1, object obj2)
            {
                var node1 = (TreeNode)obj1;
                var node2 = (TreeNode)obj2;

                if (node1.ImageKey != node2.ImageKey)
                {
                    return (!IsReportNode(node1)) ? -1 : 1;
                }

                return String.Compare(node1.Text, node2.Text);
            }
        }
    }

    public class ReportDoubleClickEventArgs : EventArgs
    {
        public string ReportName { get; private set; }
        public string ReportPath { get; private set; }

        public ReportDoubleClickEventArgs(string reportName, string reportPath)
        {
            ReportName = reportName;
            ReportPath = reportPath;
        }
    }
}