namespace TourWriter.Modules.SupplierModule
{
    partial class AllocationAgentsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance75 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance76 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance77 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AllocationAgentsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.gridAllocationAgent = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.tsAllocationAgent = new TourWriter.UserControls.MyToolStrip();
            this.btnAddAllocation = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteAllocation = new System.Windows.Forms.ToolStripButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAllocationAgent)).BeginInit();
            this.tsAllocationAgent.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.gridAllocationAgent);
            this.panel1.Controls.Add(this.tsAllocationAgent);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(307, 289);
            this.panel1.TabIndex = 0;
            // 
            // gridAllocationAgent
            // 
            this.gridAllocationAgent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance38.BackColor = System.Drawing.SystemColors.Window;
            appearance38.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridAllocationAgent.DisplayLayout.Appearance = appearance38;
            this.gridAllocationAgent.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance42.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance42.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance42.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance42.BorderColor = System.Drawing.SystemColors.Window;
            this.gridAllocationAgent.DisplayLayout.GroupByBox.Appearance = appearance42;
            appearance43.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridAllocationAgent.DisplayLayout.GroupByBox.BandLabelAppearance = appearance43;
            this.gridAllocationAgent.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.gridAllocationAgent.DisplayLayout.GroupByBox.Hidden = true;
            appearance44.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance44.BackColor2 = System.Drawing.SystemColors.Control;
            appearance44.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance44.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridAllocationAgent.DisplayLayout.GroupByBox.PromptAppearance = appearance44;
            this.gridAllocationAgent.DisplayLayout.MaxColScrollRegions = 1;
            this.gridAllocationAgent.DisplayLayout.MaxRowScrollRegions = 1;
            appearance45.BackColor = System.Drawing.SystemColors.Window;
            appearance45.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gridAllocationAgent.DisplayLayout.Override.ActiveCellAppearance = appearance45;
            appearance46.BackColor = System.Drawing.SystemColors.Highlight;
            appearance46.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.gridAllocationAgent.DisplayLayout.Override.ActiveRowAppearance = appearance46;
            this.gridAllocationAgent.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.gridAllocationAgent.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance47.BackColor = System.Drawing.SystemColors.Window;
            this.gridAllocationAgent.DisplayLayout.Override.CardAreaAppearance = appearance47;
            appearance48.BorderColor = System.Drawing.Color.Silver;
            appearance48.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.gridAllocationAgent.DisplayLayout.Override.CellAppearance = appearance48;
            this.gridAllocationAgent.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.gridAllocationAgent.DisplayLayout.Override.CellPadding = 0;
            appearance49.BackColor = System.Drawing.SystemColors.Control;
            appearance49.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance49.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance49.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance49.BorderColor = System.Drawing.SystemColors.Window;
            this.gridAllocationAgent.DisplayLayout.Override.GroupByRowAppearance = appearance49;
            appearance75.TextHAlignAsString = "Left";
            this.gridAllocationAgent.DisplayLayout.Override.HeaderAppearance = appearance75;
            this.gridAllocationAgent.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.gridAllocationAgent.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance76.BackColor = System.Drawing.SystemColors.Window;
            appearance76.BorderColor = System.Drawing.Color.Silver;
            this.gridAllocationAgent.DisplayLayout.Override.RowAppearance = appearance76;
            this.gridAllocationAgent.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance77.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gridAllocationAgent.DisplayLayout.Override.TemplateAddRowAppearance = appearance77;
            this.gridAllocationAgent.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.gridAllocationAgent.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.gridAllocationAgent.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.gridAllocationAgent.Location = new System.Drawing.Point(3, 36);
            this.gridAllocationAgent.Name = "gridAllocationAgent";
            this.gridAllocationAgent.Size = new System.Drawing.Size(304, 250);
            this.gridAllocationAgent.TabIndex = 130;
            this.gridAllocationAgent.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnUpdate;
            this.gridAllocationAgent.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridAllocationAgent_InitializeLayout);
            this.gridAllocationAgent.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridAllocationAgent_KeyUp);
            // 
            // tsAllocationAgent
            // 
            this.tsAllocationAgent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tsAllocationAgent.BackColor = System.Drawing.Color.Transparent;
            this.tsAllocationAgent.DisableAllMenuItems = true;
            this.tsAllocationAgent.Dock = System.Windows.Forms.DockStyle.None;
            this.tsAllocationAgent.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsAllocationAgent.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddAllocation,
            this.btnDeleteAllocation});
            this.tsAllocationAgent.Location = new System.Drawing.Point(252, 9);
            this.tsAllocationAgent.Name = "tsAllocationAgent";
            this.tsAllocationAgent.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsAllocationAgent.Size = new System.Drawing.Size(49, 25);
            this.tsAllocationAgent.TabIndex = 131;
            this.tsAllocationAgent.Text = "myToolStrip2";
            // 
            // btnAddAllocation
            // 
            this.btnAddAllocation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddAllocation.Image = ((System.Drawing.Image)(resources.GetObject("btnAddAllocation.Image")));
            this.btnAddAllocation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddAllocation.Name = "btnAddAllocation";
            this.btnAddAllocation.Size = new System.Drawing.Size(23, 22);
            this.btnAddAllocation.Text = "btnFocAdd";
            this.btnAddAllocation.ToolTipText = "Add new allocation";
            this.btnAddAllocation.Click += new System.EventHandler(this.btnAddAllocation_Click);
            // 
            // btnDeleteAllocation
            // 
            this.btnDeleteAllocation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDeleteAllocation.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteAllocation.Image")));
            this.btnDeleteAllocation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteAllocation.Name = "btnDeleteAllocation";
            this.btnDeleteAllocation.Size = new System.Drawing.Size(23, 22);
            this.btnDeleteAllocation.Text = "btnServiceDelete";
            this.btnDeleteAllocation.ToolTipText = "Delete allocation";
            this.btnDeleteAllocation.Click += new System.EventHandler(this.btnDeleteAllocation_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(229, 296);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(148, 296);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // AllocationAgentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 327);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AllocationAgentsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Agents";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAllocationAgent)).EndInit();
            this.tsAllocationAgent.ResumeLayout(false);
            this.tsAllocationAgent.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridAllocationAgent;
        private UserControls.MyToolStrip tsAllocationAgent;
        private System.Windows.Forms.ToolStripButton btnAddAllocation;
        private System.Windows.Forms.ToolStripButton btnDeleteAllocation;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
    }
}