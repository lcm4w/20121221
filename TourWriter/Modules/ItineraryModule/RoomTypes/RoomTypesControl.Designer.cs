namespace TourWriter.Modules.ItineraryModule.RoomTypes
{
    partial class RoomTypesControl
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
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            this.gridRoomTypes = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gridMembers = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblHeader1 = new System.Windows.Forms.Label();
            this.tsRoomTypes = new TourWriter.UserControls.MyToolStrip();
            this.btnAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAdd = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnDel = new System.Windows.Forms.ToolStripButton();
            this.lblHeader2 = new System.Windows.Forms.Label();
            this.tsMembers = new TourWriter.UserControls.MyToolStrip();
            this.btnMemberEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMemberAdd = new System.Windows.Forms.ToolStripButton();
            this.btnMemberDelete = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridRoomTypes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMembers)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tsRoomTypes.SuspendLayout();
            this.tsMembers.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridRoomTypes
            // 
            this.gridRoomTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridRoomTypes.Location = new System.Drawing.Point(3, 30);
            this.gridRoomTypes.Name = "gridRoomTypes";
            this.gridRoomTypes.Size = new System.Drawing.Size(250, 376);
            this.gridRoomTypes.TabIndex = 138;
            this.gridRoomTypes.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridRoomTypes_InitializeLayout);
            this.gridRoomTypes.AfterRowActivate += new System.EventHandler(this.gridRoomTypes_AfterRowActivate);
            // 
            // gridMembers
            // 
            this.gridMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridMembers.DisplayLayout.Appearance = appearance13;
            this.gridMembers.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.gridMembers.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance14.BorderColor = System.Drawing.SystemColors.Window;
            this.gridMembers.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridMembers.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
            this.gridMembers.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridMembers.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.gridMembers.DisplayLayout.MaxColScrollRegions = 1;
            this.gridMembers.DisplayLayout.MaxRowScrollRegions = 1;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            appearance17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gridMembers.DisplayLayout.Override.ActiveCellAppearance = appearance17;
            appearance18.BackColor = System.Drawing.SystemColors.Highlight;
            appearance18.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.gridMembers.DisplayLayout.Override.ActiveRowAppearance = appearance18;
            this.gridMembers.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.gridMembers.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            this.gridMembers.DisplayLayout.Override.CardAreaAppearance = appearance19;
            appearance20.BorderColor = System.Drawing.Color.Silver;
            appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.gridMembers.DisplayLayout.Override.CellAppearance = appearance20;
            this.gridMembers.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.gridMembers.DisplayLayout.Override.CellPadding = 0;
            appearance21.BackColor = System.Drawing.SystemColors.Control;
            appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance21.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance21.BorderColor = System.Drawing.SystemColors.Window;
            this.gridMembers.DisplayLayout.Override.GroupByRowAppearance = appearance21;
            appearance22.TextHAlignAsString = "Left";
            this.gridMembers.DisplayLayout.Override.HeaderAppearance = appearance22;
            this.gridMembers.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.gridMembers.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance23.BackColor = System.Drawing.SystemColors.Window;
            appearance23.BorderColor = System.Drawing.Color.Silver;
            this.gridMembers.DisplayLayout.Override.RowAppearance = appearance23;
            this.gridMembers.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance24.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gridMembers.DisplayLayout.Override.TemplateAddRowAppearance = appearance24;
            this.gridMembers.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.gridMembers.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.gridMembers.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.gridMembers.Location = new System.Drawing.Point(3, 28);
            this.gridMembers.Name = "gridMembers";
            this.gridMembers.Size = new System.Drawing.Size(1030, 376);
            this.gridMembers.TabIndex = 141;
            this.gridMembers.Text = "ultraGrid1";
            this.gridMembers.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridMembers_InitializeLayout);
            this.gridMembers.AfterExitEditMode += new System.EventHandler(this.gridMembers_AfterExitEditMode);
            this.gridMembers.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridMembers_CellChange);
            this.gridMembers.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridMembers_ClickCellButton);
            this.gridMembers.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.gridMembers_DoubleClickRow);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblHeader1);
            this.splitContainer1.Panel1.Controls.Add(this.gridRoomTypes);
            this.splitContainer1.Panel1.Controls.Add(this.tsRoomTypes);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblHeader2);
            this.splitContainer1.Panel2.Controls.Add(this.gridMembers);
            this.splitContainer1.Panel2.Controls.Add(this.tsMembers);
            this.splitContainer1.Size = new System.Drawing.Size(1297, 409);
            this.splitContainer1.SplitterDistance = 257;
            this.splitContainer1.TabIndex = 143;
            // 
            // lblHeader1
            // 
            this.lblHeader1.AutoSize = true;
            this.lblHeader1.Location = new System.Drawing.Point(3, 9);
            this.lblHeader1.Name = "lblHeader1";
            this.lblHeader1.Size = new System.Drawing.Size(63, 13);
            this.lblHeader1.TabIndex = 139;
            this.lblHeader1.Text = "Room types";
            // 
            // tsRoomTypes
            // 
            this.tsRoomTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tsRoomTypes.AutoSize = false;
            this.tsRoomTypes.BackColor = System.Drawing.Color.Transparent;
            this.tsRoomTypes.DisableAllMenuItems = true;
            this.tsRoomTypes.Dock = System.Windows.Forms.DockStyle.None;
            this.tsRoomTypes.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsRoomTypes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAll,
            this.toolStripSeparator2,
            this.btnAdd,
            this.btnDel});
            this.tsRoomTypes.Location = new System.Drawing.Point(114, 4);
            this.tsRoomTypes.Name = "tsRoomTypes";
            this.tsRoomTypes.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsRoomTypes.Size = new System.Drawing.Size(141, 25);
            this.tsRoomTypes.TabIndex = 137;
            this.tsRoomTypes.Text = "myToolStrip2";
            // 
            // btnAll
            // 
            this.btnAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(78, 22);
            this.btnAll.Text = "All Members";
            this.btnAll.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.btnAll.ToolTipText = "All Members";
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(29, 22);
            this.btnAdd.ToolTipText = "Add room type";
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
            // lblHeader2
            // 
            this.lblHeader2.AutoSize = true;
            this.lblHeader2.Location = new System.Drawing.Point(3, 9);
            this.lblHeader2.Name = "lblHeader2";
            this.lblHeader2.Size = new System.Drawing.Size(77, 13);
            this.lblHeader2.TabIndex = 143;
            this.lblHeader2.Text = "Itinerary clients";
            // 
            // tsMembers
            // 
            this.tsMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tsMembers.BackColor = System.Drawing.Color.Transparent;
            this.tsMembers.DisableAllMenuItems = true;
            this.tsMembers.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMembers.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMembers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnMemberEdit,
            this.toolStripSeparator1,
            this.btnMemberAdd,
            this.btnMemberDelete});
            this.tsMembers.Location = new System.Drawing.Point(961, 4);
            this.tsMembers.Name = "tsMembers";
            this.tsMembers.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsMembers.Size = new System.Drawing.Size(78, 25);
            this.tsMembers.TabIndex = 142;
            this.tsMembers.Text = "myToolStrip2";
            // 
            // btnMemberEdit
            // 
            this.btnMemberEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMemberEdit.Image = global::TourWriter.Properties.Resources.PageEdit;
            this.btnMemberEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMemberEdit.Name = "btnMemberEdit";
            this.btnMemberEdit.Size = new System.Drawing.Size(23, 22);
            this.btnMemberEdit.Text = "toolStripButton1";
            this.btnMemberEdit.ToolTipText = "Edit contact details for selected client";
            this.btnMemberEdit.Click += new System.EventHandler(this.btnMemberEdit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnMemberAdd
            // 
            this.btnMemberAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMemberAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnMemberAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMemberAdd.Name = "btnMemberAdd";
            this.btnMemberAdd.Size = new System.Drawing.Size(23, 22);
            this.btnMemberAdd.Text = "btnAdd";
            this.btnMemberAdd.ToolTipText = "Add new client";
            this.btnMemberAdd.Click += new System.EventHandler(this.btnAddMember_Click);
            // 
            // btnMemberDelete
            // 
            this.btnMemberDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMemberDelete.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnMemberDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMemberDelete.Name = "btnMemberDelete";
            this.btnMemberDelete.Size = new System.Drawing.Size(23, 22);
            this.btnMemberDelete.Text = "btnDelete";
            this.btnMemberDelete.ToolTipText = "Delete selected client";
            this.btnMemberDelete.Click += new System.EventHandler(this.btnMemberDelete_Click);
            // 
            // RoomTypesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "RoomTypesControl";
            this.Size = new System.Drawing.Size(1303, 413);
            ((System.ComponentModel.ISupportInitialize)(this.gridRoomTypes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMembers)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.tsRoomTypes.ResumeLayout(false);
            this.tsRoomTypes.PerformLayout();
            this.tsMembers.ResumeLayout(false);
            this.tsMembers.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.MyToolStrip tsRoomTypes;
        private System.Windows.Forms.ToolStripDropDownButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnDel;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridRoomTypes;
        private System.Windows.Forms.ToolStripButton btnAll;
        private UserControls.MyToolStrip tsMembers;
        private System.Windows.Forms.ToolStripButton btnMemberEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnMemberAdd;
        private System.Windows.Forms.ToolStripButton btnMemberDelete;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        internal Infragistics.Win.UltraWinGrid.UltraGrid gridMembers;
        private System.Windows.Forms.Label lblHeader1;
        private System.Windows.Forms.Label lblHeader2;
    }
}
