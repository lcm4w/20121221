using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Properties;
using TourWriter.Services;

namespace TourWriter.UserControls.Reports
{
    public partial class ExplorerControl : UserControl
    {
        private const string DefaultReportsMessage = "Default reports cannot be changed.\r\n\r\nChanges are only allowed within the Custom folder.";
        private ReportCategory _reportCategory;
        public enum ReportCategory { General, Itinerary, Supplier }
        public event EventHandler<ReportDoubleClickEventArgs> ReportDoubleClick;

        public ExplorerControl()
        {
            InitializeComponent();
            if (!App.IsInDesignMode) 
                InitializeImageList();
        }
        
        public void PopulateReportTree(ReportCategory reportCategory)
        {
            if (App.IsInDesignMode) return;
            _reportCategory = reportCategory;
            InitializeReportTree();
        }

        public void RefreshReportTree()
        {
            InitializeReportTree();
        }

        private void InitializeReportTree()
        {
            treeReports.Nodes.Clear();

            // add default reports
            var node = new TreeNode { Text = "Standard", Tag = "Standard", ImageKey = "Folder", SelectedImageKey = "Folder", ContextMenu = null, ContextMenuStrip = null };
            treeReports.Nodes.Add(node);
            PopulateDefaultReportsList(node);

            // add custom reports  
            var parent = Cache.ToolSet.TemplateCategory.Where(
                    t => t.RowState != DataRowState.Deleted && t.TemplateCategoryName == _reportCategory.ToString()).FirstOrDefault();
            if (parent == null) return;
            node = new TreeNode { Text = "Custom", ImageKey = "Folder", SelectedImageKey = "Folder", Name = "ReportCategory." + parent.TemplateCategoryID };
            treeReports.Nodes.Add(node);
            PopulateCustomReportsList(parent.TemplateCategoryID, node);

            treeReports.TreeViewNodeSorter = new TreeNodeComparer();
            treeReports.ExpandAll();
        }

        private void PopulateDefaultReportsList(TreeNode node)
        {
            var path = Path.Combine(Path.Combine(App.Path_DefaultTemplatesFolder, "Reports"), _reportCategory.ToString());
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            foreach (var t in new DirectoryInfo(path).GetFiles().Where(f => f.Extension == ".rdl" || f.Extension == ".rdlc"))
            {
                var name = Regex.Replace(t.Name.Replace(t.Extension, ""), "([A-Z])", " $1", RegexOptions.Compiled).Trim();
                node.Nodes.Add(new TreeNode { Text = name, Name = t.FullName, Tag = "Standard", ImageKey = "Report", SelectedImageKey = "Report", ContextMenu = null, ContextMenuStrip = null });
            }
        }
        
        private void InitializeImageList()
        {
            treeReportsImageList.Images.Add("Folder", Resources.Folder);
            treeReportsImageList.Images.Add("Report", Resources.Report);
        }

        private void PopulateCustomReportsList(int categoryId, TreeNode parent)
        {
            // add the folders for this category
            foreach (var folder in Cache.ToolSet.TemplateCategory.Where(f => f.RowState != DataRowState.Deleted && 
                !f.IsParentTemplateCategoryIDNull() && f.ParentTemplateCategoryID == categoryId).OrderBy(f => f.TemplateCategoryID))
            {
                var folderId = folder.TemplateCategoryID;

                var child = new TreeNode
                               {
                                   Name = "ReportCategory." + folderId,
                                   Text = folder.TemplateCategoryName,
                                   ImageKey = "Folder",
                                   SelectedImageKey = "Folder",
                                   ContextMenuStrip = treeReportsContextMenu
                               };
                if (!folder.IsParentTemplateCategoryIDNull())
                    child.Tag = folder.ParentTemplateCategoryID;
                parent.Nodes.Add(child);

                PopulateCustomReportsList(folderId, child);
            }

            // add the files for this category
            var files = Cache.ToolSet.Template.Where(f => f.RowState != DataRowState.Deleted && f.ParentTemplateCategoryID == categoryId);
            foreach (var file in files) AddReportNode(parent, file);
        }

        private TreeNode AddReportNode(TreeNode parent, ToolSet.TemplateRow report)
        {
            var node = new ReportNode
                           {
                               Name = "Report." + report.TemplateID,
                               Text = report.TemplateName,
                               Tag = report.ParentTemplateCategoryID,
                               FilePath = report.FilePath,
                               ImageKey = "Report",
                               SelectedImageKey = "Report",
                               ContextMenuStrip = treeReportsContextMenu
                           };

            parent.Nodes.Add(node);
            return node;
        }

        private TreeNode AddCategoryNode(TreeNodeCollection nodes, ToolSet.TemplateCategoryRow category)
        {
            var node = new TreeNode();
            node.Name = "ReportCategory." + category.TemplateCategoryID;
            node.Text = category.TemplateCategoryName;
            node.Tag = category.ParentTemplateCategoryID;
            node.SelectedImageKey = node.ImageKey = "Folder";
            node.ContextMenuStrip = treeReportsContextMenu;

            nodes.Add(node);
            return node;
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

        private static bool IsDefaultReport(TreeNode node)
        {
            return node.Tag != null && node.Tag.ToString().ToLower().StartsWith("Standard".ToLower());
        }

        private static bool IsReportNode(TreeNode node)
        {
            return (node.ImageKey == "Report");
        }

        private TreeNode AddReport(string reportFile, TreeNode parentCategoryNode)
        {
            int categoryId = GetNodeID(parentCategoryNode);

            var newReport = Cache.ToolSet.Template.NewTemplateRow();
            newReport.ParentTemplateCategoryID = categoryId;
            newReport.FilePath = reportFile;
            newReport.TemplateName = Path.GetFileNameWithoutExtension(reportFile);
            Cache.ToolSet.Template.AddTemplateRow(newReport);

            // add the new report to the tree
            var newNode = AddReportNode(parentCategoryNode, newReport);
            parentCategoryNode.Expand();
            return newNode;
        }

        private void AddCustomCategory()
        {
            var node = treeReports.SelectedNode;
            if (node == null || IsDefaultReport(node))
                foreach (var n in treeReports.Nodes.Cast<TreeNode>().Where(n => n.Text == "Custom")) node = n;
            else
                node = GetReportCategoryNode(node);

            var categoryId = GetNodeID(node);

            var newCategory = Cache.ToolSet.TemplateCategory.NewTemplateCategoryRow();
            newCategory.ParentTemplateCategoryID = categoryId;
            newCategory.TemplateCategoryName = "New Category";
            Cache.ToolSet.TemplateCategory.AddTemplateCategoryRow(newCategory);

            // add the new report to the tree
            var newNode = AddCategoryNode(node.Nodes, newCategory);
            node.Expand();
            treeReports.SelectedNode = newNode;
            newNode.BeginEdit();
        }

        private void AddCustomReport()
        {
            var node = treeReports.SelectedNode;
            if (treeReports.SelectedNode == null || IsDefaultReport(treeReports.SelectedNode))
                foreach (var n in treeReports.Nodes.Cast<TreeNode>().Where(n => n.Text == "Custom")) node = n;

            var categoryNode = GetReportCategoryNode(node);

            string[] reportFiles = App.SelectExternalFiles(true, "Select report file(s)", "Reports (*.rdlc)|*.rdlc|All files (*.*)|*.*", 0);
            if (reportFiles == null)
                return;

            TreeNode newNode = null;
            foreach (string reportFile in reportFiles)
            {
                newNode = AddReport(reportFile, categoryNode);
            }
            if (newNode != null) treeReports.SelectedNode = newNode;
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
            string reportFile = ((ReportNode)treeReports.SelectedNode).FilePath;
            reportFile = App.SelectExternalFile(true, "Select report file(s)", reportFile, "Reports (*.rdlc)|*.rdlc|All files (*.*)|*.*", 0);
            if (String.IsNullOrEmpty(reportFile))
                return;

            var report = GetReportRow(treeReports.SelectedNode);
            report.FilePath = reportFile;
        }

        private void OpenSelectedReportFile()
        {
            if (!(treeReports.SelectedNode is ReportNode)) return;
            var file = ExternalFilesHelper.ConvertToAbsolutePath(((ReportNode) treeReports.SelectedNode).FilePath);
            if (!File.Exists(file))
            {
                App.ShowWarning("File does not exist: " + file);
                return;
            }
            App.SelectExternalFileInFolder(file);
        }

        #region Events

        private void menuAddReport_Click(object sender, EventArgs e)
        {
            AddCustomReport();
        }

        private void menuAddCategory_Click(object sender, EventArgs e)
        {
            AddCustomCategory();
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

        private void menuOpenFileLocation_Click(object sender, EventArgs e)
        {
            OpenSelectedReportFile();
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

            if (node == destCategoryNode) return;

            if (destNode.Text == "Standard" || destCategoryNode.Text == "Standard" || IsDefaultReport(node))
            {
                //App.ShowInfo(DefaultReportsMessage);
                return;
            }

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

            if (IsDefaultReport(e.Node))
            {
                ReportDoubleClick(this, new ReportDoubleClickEventArgs(e.Node.Text, e.Node.Name));
            }
            else
            {
                var report = GetReportRow(e.Node);
                string reportName = report.TemplateName;
                string reportPath = Services.ExternalFilesHelper.ConvertToAbsolutePath(report.FilePath);// Cache.ToolSet.AppSettings[0].ExternalFilesPath + report.FilePath;
                ReportDoubleClick(this, new ReportDoubleClickEventArgs(reportName, reportPath));
            }
        }

        private void treeReports_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeReports.SelectedNode = e.Node;
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

        private void treeReportsContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var node = treeReports.SelectedNode;

            // not on a node
            if (node == null)
            {
                treeReports.ContextMenuStrip.Items["menuChangeReportFile"].Visible = false;
                treeReports.ContextMenuStrip.Items["menuDelete"].Visible = false;
                treeReports.ContextMenuStrip.Items["menuRename"].Visible = false;
                treeReports.ContextMenuStrip.Items["menuOpenFileLocation"].Visible = false;
                return;
            }

            if (node.ContextMenuStrip == null)
                node.ContextMenuStrip = treeReportsContextMenu;

            // is default/standard folder
            var isDefaultReports = node.Text == "Standard" || IsDefaultReport(node);
            if (isDefaultReports)
            {
                node.ContextMenuStrip.Items["menuChangeReportFile"].Visible = false;
                node.ContextMenuStrip.Items["menuDelete"].Visible = false;
                node.ContextMenuStrip.Items["menuRename"].Visible = false;
                node.ContextMenuStrip.Items["menuOpenFileLocation"].Visible = false;
                //App.ShowInfo(DefaultReportsMessage);
                return;
            }

            // is the custom folder
            var isCustomReportParent = node.Text == "Custom";
            if (isCustomReportParent)
            {
                node.ContextMenuStrip.Items["menuChangeReportFile"].Visible = false;
                node.ContextMenuStrip.Items["menuDelete"].Visible = false;
                node.ContextMenuStrip.Items["menuRename"].Visible = false;
                node.ContextMenuStrip.Items["menuOpenFileLocation"].Visible = false;
                return;
            }

            // is a report-file node
            var isReport = IsReportNode(node) || node.Name.StartsWith("Report");
            if (isReport)
            {
                node.ContextMenuStrip.Items["menuChangeReportFile"].Visible = IsReportNode(node);
                node.ContextMenuStrip.Items["menuDelete"].Visible = true;
                node.ContextMenuStrip.Items["menuRename"].Visible = true;
                node.ContextMenuStrip.Items["menuOpenFileLocation"].Visible = true;
            }
        }

        private void treeReports_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeReports.SelectedNode = treeReports.GetNodeAt(e.X, e.Y);
            } 
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

    class ReportNode : TreeNode
    {
        public string FilePath { get; set; }
    }
}