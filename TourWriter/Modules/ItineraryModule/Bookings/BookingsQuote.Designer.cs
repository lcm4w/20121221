﻿namespace TourWriter.Modules.ItineraryModule.Bookings
{
    partial class BookingsQuote
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            this.grid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnEditOverrides = new System.Windows.Forms.Button();
            this.myToolStrip1 = new TourWriter.UserControls.MyToolStrip();
            this.cmbPriceType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExpand = new System.Windows.Forms.ToolStripButton();
            this.btnFilter = new System.Windows.Forms.ToolStripButton();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.btnExcel = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.myToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.grid.DisplayLayout.Appearance = appearance1;
            this.grid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.grid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.grid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.grid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.grid.DisplayLayout.MaxColScrollRegions = 1;
            this.grid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.grid.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.grid.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.grid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.grid.DisplayLayout.Override.CellAppearance = appearance8;
            this.grid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grid.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.grid.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.grid.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.grid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.grid.DisplayLayout.Override.RowAppearance = appearance11;
            this.grid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.grid.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.grid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.grid.Location = new System.Drawing.Point(3, 28);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(890, 433);
            this.grid.TabIndex = 1;
            this.grid.Text = "ultraGrid1";
            this.grid.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.grid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grid_InitializeLayout);
            // 
            // btnEditOverrides
            // 
            this.btnEditOverrides.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditOverrides.Location = new System.Drawing.Point(3, 467);
            this.btnEditOverrides.Name = "btnEditOverrides";
            this.btnEditOverrides.Size = new System.Drawing.Size(125, 23);
            this.btnEditOverrides.TabIndex = 76;
            this.btnEditOverrides.Text = "Edit Overrides";
            this.btnEditOverrides.UseVisualStyleBackColor = true;
            this.btnEditOverrides.Click += new System.EventHandler(this.btnEditOverrides_Click);
            // 
            // myToolStrip1
            // 
            this.myToolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip1.DisableAllMenuItems = true;
            this.myToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbPriceType,
            this.toolStripSeparator1,
            this.btnExpand,
            this.btnFilter,
            this.btnPrint,
            this.btnExcel});
            this.myToolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.myToolStrip1.Location = new System.Drawing.Point(638, 5);
            this.myToolStrip1.Name = "myToolStrip1";
            this.myToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip1.Size = new System.Drawing.Size(255, 25);
            this.myToolStrip1.TabIndex = 75;
            this.myToolStrip1.Text = "myToolStrip1";
            // 
            // cmbPriceType
            // 
            this.cmbPriceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPriceType.Items.AddRange(new object[] {
            "Net prices",
            "Gross prices"});
            this.cmbPriceType.Name = "cmbPriceType";
            this.cmbPriceType.Size = new System.Drawing.Size(121, 25);
            this.cmbPriceType.ToolTipText = "Use net or gross Supplier prices";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnExpand
            // 
            this.btnExpand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExpand.Image = global::TourWriter.Properties.Resources.Expand;
            this.btnExpand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(23, 22);
            this.btnExpand.Text = "Expand";
            this.btnExpand.ToolTipText = "Expand/collapse the group-by views";
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFilter.Image = global::TourWriter.Properties.Resources.Filter;
            this.btnFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(23, 22);
            this.btnFilter.Text = "toolStripButton1";
            this.btnFilter.ToolTipText = "Enable column data filters";
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrint.Image = global::TourWriter.Properties.Resources.Printer;
            this.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(23, 22);
            this.btnPrint.Text = "toolStripButton1";
            this.btnPrint.ToolTipText = "Print current data view";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnExcel
            // 
            this.btnExcel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExcel.Image = global::TourWriter.Properties.Resources.PageExcel;
            this.btnExcel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(23, 22);
            this.btnExcel.Text = "toolStripButton1";
            this.btnExcel.ToolTipText = "Export current view to Excel";
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // BookingsQuote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.btnEditOverrides);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.myToolStrip1);
            this.Name = "BookingsQuote";
            this.Size = new System.Drawing.Size(896, 493);
            this.VisibleChanged += new System.EventHandler(this.BookingsQuote_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.myToolStrip1.ResumeLayout(false);
            this.myToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grid;
        private TourWriter.UserControls.MyToolStrip myToolStrip1;
        private System.Windows.Forms.ToolStripButton btnExpand;
        private System.Windows.Forms.ToolStripButton btnFilter;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripButton btnExcel;
        private System.Windows.Forms.Button btnEditOverrides;
        private System.Windows.Forms.ToolStripComboBox cmbPriceType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
