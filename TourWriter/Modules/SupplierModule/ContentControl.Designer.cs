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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Content", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ContentId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SupplierId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ContentTypeId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ContentName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Heading");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Body");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ImagePath");
            this.gridContents = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.srcContents = new System.Windows.Forms.BindingSource(this.components);
            this.txtNames = new System.Windows.Forms.TextBox();
            this.cmbTypes = new System.Windows.Forms.ComboBox();
            this.srcContentTypes = new System.Windows.Forms.BindingSource(this.components);
            this.txtHeading = new System.Windows.Forms.TextBox();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnDel = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtImage = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridContents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcContents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcContentTypes)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.myToolStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridContents
            // 
            this.gridContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridContents.DataSource = this.srcContents;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn7.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            this.gridContents.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.gridContents.Location = new System.Drawing.Point(0, 23);
            this.gridContents.Name = "gridContents";
            this.gridContents.Size = new System.Drawing.Size(271, 578);
            this.gridContents.TabIndex = 135;
            this.gridContents.Text = "Publishing content";
            this.gridContents.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridContents_InitializeLayout);
            // 
            // srcContents
            // 
            this.srcContents.DataMember = "Content";
            this.srcContents.DataSource = typeof(TourWriter.Info.SupplierSet);
            // 
            // txtNames
            // 
            this.txtNames.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcContents, "ContentName", true));
            this.txtNames.Location = new System.Drawing.Point(63, 25);
            this.txtNames.Name = "txtNames";
            this.txtNames.Size = new System.Drawing.Size(244, 20);
            this.txtNames.TabIndex = 138;
            // 
            // cmbTypes
            // 
            this.cmbTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTypes.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.srcContents, "ContentTypeId", true));
            this.cmbTypes.DataSource = this.srcContentTypes;
            this.cmbTypes.DisplayMember = "ContentTypeName";
            this.cmbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypes.FormattingEnabled = true;
            this.cmbTypes.Location = new System.Drawing.Point(353, 25);
            this.cmbTypes.MinimumSize = new System.Drawing.Size(150, 0);
            this.cmbTypes.Name = "cmbTypes";
            this.cmbTypes.Size = new System.Drawing.Size(174, 21);
            this.cmbTypes.TabIndex = 139;
            this.toolTip1.SetToolTip(this.cmbTypes, "User display type (not used in reports)");
            this.cmbTypes.ValueMember = "ContentTypeId";
            this.cmbTypes.SelectedIndexChanged += new System.EventHandler(this.cmbTypes_SelectedIndexChanged);
            // 
            // srcContentTypes
            // 
            this.srcContentTypes.DataMember = "ContentType";
            this.srcContentTypes.DataSource = typeof(TourWriter.Info.ToolSet);
            this.srcContentTypes.Sort = "ContentTypeName";
            // 
            // txtHeading
            // 
            this.txtHeading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeading.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcContents, "Heading", true));
            this.txtHeading.Location = new System.Drawing.Point(80, 30);
            this.txtHeading.Name = "txtHeading";
            this.txtHeading.Size = new System.Drawing.Size(436, 20);
            this.txtHeading.TabIndex = 0;
            // 
            // txtBody
            // 
            this.txtBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBody.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcContents, "Body", true));
            this.txtBody.Location = new System.Drawing.Point(15, 112);
            this.txtBody.MinimumSize = new System.Drawing.Size(400, 150);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(501, 412);
            this.txtBody.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name: ";
            this.toolTip1.SetToolTip(this.label1, "Content name (not used in reports)");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(319, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 140;
            this.label2.Text = "Type: ";
            this.toolTip1.SetToolTip(this.label2, "Content type (not used in report)");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 141;
            this.label3.Text = "Heading: ";
            this.toolTip1.SetToolTip(this.label3, "Heading text (optional)");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 142;
            this.label4.Text = "Body HTML";
            this.toolTip1.SetToolTip(this.label4, "Main body text, in HTML format (optional)");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 143;
            this.label5.Text = "Image path: ";
            this.toolTip1.SetToolTip(this.label5, "Path to image file (optional)");
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
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.cmbTypes);
            this.splitContainer1.Panel2.Controls.Add(this.txtNames);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(817, 601);
            this.splitContainer1.SplitterDistance = 272;
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
            this.myToolStrip2.Location = new System.Drawing.Point(225, 0);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip2.TabIndex = 136;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(23, 22);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtHeading);
            this.groupBox1.Controls.Add(this.txtImage);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtBody);
            this.groupBox1.Location = new System.Drawing.Point(16, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(522, 530);
            this.groupBox1.TabIndex = 147;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Content for publishing";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Image = global::TourWriter.Properties.Resources.FolderExplore;
            this.btnBrowse.Location = new System.Drawing.Point(495, 59);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(23, 22);
            this.btnBrowse.TabIndex = 146;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtImage
            // 
            this.txtImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtImage.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcContents, "ImagePath", true));
            this.txtImage.Location = new System.Drawing.Point(80, 60);
            this.txtImage.Name = "txtImage";
            this.txtImage.Size = new System.Drawing.Size(413, 20);
            this.txtImage.TabIndex = 145;
            // 
            // ContentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ContentControl";
            this.Size = new System.Drawing.Size(822, 608);
            this.Load += new System.EventHandler(this.ContentControl_Load);
            this.VisibleChanged += new System.EventHandler(this.ContentControl_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.gridContents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcContents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.srcContentTypes)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid gridContents;
        private UserControls.MyToolStrip myToolStrip2;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnDel;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtHeading;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNames;
        private System.Windows.Forms.ComboBox cmbTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.BindingSource srcContents;
        private System.Windows.Forms.BindingSource srcContentTypes;
        private System.Windows.Forms.TextBox txtImage;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
