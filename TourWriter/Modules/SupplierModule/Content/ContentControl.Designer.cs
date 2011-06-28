namespace TourWriter.Modules.SupplierModule
{
    partial class ContentControl
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
            this.gridContents = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.srcContents = new System.Windows.Forms.BindingSource(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnDel = new System.Windows.Forms.ToolStripButton();
            this.contentEditor1 = new TourWriter.Modules.SupplierModule.Content.ContentEditor();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.srcContentTypes = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridContents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcContents)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.myToolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srcContentTypes)).BeginInit();
            this.SuspendLayout();
            // 
            // gridContents
            // 
            this.gridContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridContents.Location = new System.Drawing.Point(0, 23);
            this.gridContents.Name = "gridContents";
            this.gridContents.Size = new System.Drawing.Size(189, 353);
            this.gridContents.TabIndex = 135;
            this.gridContents.Text = "Available content";
            this.gridContents.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridContents_InitializeLayout);
            // 
            // srcContents
            // 
            this.srcContents.DataMember = "Content";
            this.srcContents.DataSource = typeof(TourWriter.Info.SupplierSet);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(2, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gridContents);
            this.splitContainer1.Panel1.Controls.Add(this.myToolStrip2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.contentEditor1);
            this.splitContainer1.Size = new System.Drawing.Size(577, 376);
            this.splitContainer1.SplitterDistance = 190;
            this.splitContainer1.TabIndex = 142;
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnDel});
            this.myToolStrip2.Location = new System.Drawing.Point(137, 0);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(55, 25);
            this.myToolStrip2.TabIndex = 136;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(29, 22);
            this.btnAdd.ToolTipText = "Add content for...";
            // 
            // btnDel
            // 
            this.btnDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(23, 22);
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // contentEditor1
            // 
            this.contentEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentEditor1.Content = null;
            this.contentEditor1.Location = new System.Drawing.Point(3, 23);
            this.contentEditor1.Name = "contentEditor1";
            this.contentEditor1.Size = new System.Drawing.Size(377, 350);
            this.contentEditor1.TabIndex = 147;
            // 
            // srcContentTypes
            // 
            this.srcContentTypes.DataMember = "ContentType";
            this.srcContentTypes.DataSource = typeof(TourWriter.Info.ToolSet);
            this.srcContentTypes.Sort = "ContentTypeName";
            // 
            // ContentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ContentControl";
            this.Size = new System.Drawing.Size(582, 383);
            ((System.ComponentModel.ISupportInitialize)(this.gridContents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcContents)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srcContentTypes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid gridContents;
        private UserControls.MyToolStrip myToolStrip2;
        private System.Windows.Forms.ToolStripButton btnDel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.BindingSource srcContents;
        private System.Windows.Forms.BindingSource srcContentTypes;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripDropDownButton btnAdd;
        private System.Windows.Forms.ToolTip toolTip2;
        private Content.ContentEditor contentEditor1;
    }
}
