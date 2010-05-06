namespace TourWriter.UserControls.Reports
{
    partial class ExplorerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeReports = new System.Windows.Forms.TreeView();
            this.treeReportsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddReport = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRename = new System.Windows.Forms.ToolStripMenuItem();
            this.menuChangeReportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.treeReportsImageList = new System.Windows.Forms.ImageList(this.components);
            this.treeReportsContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeReports
            // 
            this.treeReports.AllowDrop = true;
            this.treeReports.ContextMenuStrip = this.treeReportsContextMenu;
            this.treeReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeReports.ImageIndex = 0;
            this.treeReports.ImageList = this.treeReportsImageList;
            this.treeReports.LabelEdit = true;
            this.treeReports.Location = new System.Drawing.Point(0, 0);
            this.treeReports.Name = "treeReports";
            this.treeReports.SelectedImageIndex = 0;
            this.treeReports.Size = new System.Drawing.Size(167, 338);
            this.treeReports.TabIndex = 129;
            this.treeReports.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeReports_BeforeLabelEdit);
            this.treeReports.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeReports_AfterLabelEdit);
            this.treeReports.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeReports_ItemDrag);
            this.treeReports.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeReports_NodeMouseClick);
            this.treeReports.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeReports_NodeMouseDoubleClick);
            this.treeReports.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeReports_DragDrop);
            this.treeReports.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeReports_DragEnter);
            this.treeReports.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeReports_MouseDown);
            // 
            // treeReportsContextMenu
            // 
            this.treeReportsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddReport,
            this.menuAddCategory,
            this.menuDelete,
            this.menuRename,
            this.menuChangeReportFile});
            this.treeReportsContextMenu.Name = "treeReportsMenu";
            this.treeReportsContextMenu.Size = new System.Drawing.Size(170, 136);
            this.treeReportsContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.treeReportsContextMenu_Opening);
            // 
            // menuAddReport
            // 
            this.menuAddReport.Name = "menuAddReport";
            this.menuAddReport.Size = new System.Drawing.Size(169, 22);
            this.menuAddReport.Text = "Add report";
            this.menuAddReport.Click += new System.EventHandler(this.menuAddReport_Click);
            // 
            // menuAddCategory
            // 
            this.menuAddCategory.Name = "menuAddCategory";
            this.menuAddCategory.Size = new System.Drawing.Size(169, 22);
            this.menuAddCategory.Text = "Add folder";
            this.menuAddCategory.Click += new System.EventHandler(this.menuAddCategory_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(169, 22);
            this.menuDelete.Text = "Remove";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // menuRename
            // 
            this.menuRename.Name = "menuRename";
            this.menuRename.Size = new System.Drawing.Size(169, 22);
            this.menuRename.Text = "Rename";
            this.menuRename.Click += new System.EventHandler(this.menuRename_Click);
            // 
            // menuChangeReportFile
            // 
            this.menuChangeReportFile.Name = "menuChangeReportFile";
            this.menuChangeReportFile.Size = new System.Drawing.Size(169, 22);
            this.menuChangeReportFile.Text = "Change report file";
            this.menuChangeReportFile.Click += new System.EventHandler(this.menuChangeReportFile_Click);
            // 
            // treeReportsImageList
            // 
            this.treeReportsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.treeReportsImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.treeReportsImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ExplorerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeReports);
            this.Name = "ExplorerControl";
            this.Size = new System.Drawing.Size(167, 338);
            this.treeReportsContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeReports;
        private System.Windows.Forms.ImageList treeReportsImageList;
        private System.Windows.Forms.ContextMenuStrip treeReportsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuAddReport;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripMenuItem menuRename;
        private System.Windows.Forms.ToolStripMenuItem menuChangeReportFile;
        private System.Windows.Forms.ToolStripMenuItem menuAddCategory;
    }
}