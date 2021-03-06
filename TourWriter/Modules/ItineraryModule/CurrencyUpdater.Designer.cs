﻿namespace TourWriter.Modules.ItineraryModule
{
    partial class CurrencyUpdater
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
            this.components = new System.ComponentModel.Container();
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
            this.gridBookings = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.txtRateAdjustment = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmbCurrency = new System.Windows.Forms.ComboBox();
            this.lnkEdit = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCcyService = new System.Windows.Forms.ComboBox();
            this.pnlMargin = new System.Windows.Forms.Panel();
            this.cmbCcyDatepoint = new System.Windows.Forms.ComboBox();
            this.lnkOptions = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.gridBookings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRateAdjustment)).BeginInit();
            this.pnlMargin.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridBookings
            // 
            this.gridBookings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridBookings.DisplayLayout.Appearance = appearance1;
            this.gridBookings.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.gridBookings.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.gridBookings.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridBookings.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.gridBookings.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.gridBookings.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.gridBookings.DisplayLayout.MaxColScrollRegions = 1;
            this.gridBookings.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.gridBookings.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.gridBookings.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.gridBookings.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.gridBookings.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.gridBookings.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.gridBookings.DisplayLayout.Override.CellAppearance = appearance8;
            this.gridBookings.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.gridBookings.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.gridBookings.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.gridBookings.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.gridBookings.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.gridBookings.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.gridBookings.DisplayLayout.Override.RowAppearance = appearance11;
            this.gridBookings.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gridBookings.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.gridBookings.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.gridBookings.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.gridBookings.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.gridBookings.Location = new System.Drawing.Point(12, 63);
            this.gridBookings.Name = "gridBookings";
            this.gridBookings.Size = new System.Drawing.Size(518, 366);
            this.gridBookings.TabIndex = 10;
            this.gridBookings.Text = "ultraGrid1";
            this.gridBookings.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.gridBookings.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridBookings_InitializeLayout);
            this.gridBookings.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.gridBookings_InitializeRow);
            this.gridBookings.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridBookings_MouseClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(455, 479);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(374, 479);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Keep";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.Location = new System.Drawing.Point(374, 435);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(156, 23);
            this.btnUpdate.TabIndex = 11;
            this.btnUpdate.Text = "Load Internet Rates";
            this.toolTip1.SetToolTip(this.btnUpdate, "Get latest exchange rates");
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // txtRateAdjustment
            // 
            this.txtRateAdjustment.FormatString = "##0\\.00%";
            this.txtRateAdjustment.Location = new System.Drawing.Point(63, 5);
            this.txtRateAdjustment.MaskInput = "-nnn.nn %";
            this.txtRateAdjustment.Name = "txtRateAdjustment";
            this.txtRateAdjustment.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtRateAdjustment.PromptChar = ' ';
            this.txtRateAdjustment.Size = new System.Drawing.Size(60, 21);
            this.txtRateAdjustment.TabIndex = 12;
            this.toolTip1.SetToolTip(this.txtRateAdjustment, "Example: enter 5 to add a 5% buffer to the default exchange rate");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Adjustment";
            this.toolTip1.SetToolTip(this.label1, "Example: enter 5 to add a 5% buffer to the default exchange rate");
            // 
            // cmbCurrency
            // 
            this.cmbCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurrency.FormattingEnabled = true;
            this.cmbCurrency.Location = new System.Drawing.Point(135, 25);
            this.cmbCurrency.Name = "cmbCurrency";
            this.cmbCurrency.Size = new System.Drawing.Size(395, 21);
            this.cmbCurrency.TabIndex = 17;
            this.toolTip1.SetToolTip(this.cmbCurrency, "Output currency for this Itinerary");
            // 
            // lnkEdit
            // 
            this.lnkEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkEdit.AutoSize = true;
            this.lnkEdit.Location = new System.Drawing.Point(437, 9);
            this.lnkEdit.Name = "lnkEdit";
            this.lnkEdit.Size = new System.Drawing.Size(93, 13);
            this.lnkEdit.TabIndex = 19;
            this.lnkEdit.TabStop = true;
            this.lnkEdit.Text = "(change currency)";
            this.toolTip1.SetToolTip(this.lnkEdit, "Click to enable currency change");
            this.lnkEdit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkEdit_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(15, 469);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(515, 2);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(123, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "(e.g. add margin to exchange rate)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 18;
            this.label3.Text = "Itinerary Currency";
            // 
            // cmbCcyService
            // 
            this.cmbCcyService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbCcyService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCcyService.FormattingEnabled = true;
            this.cmbCcyService.Location = new System.Drawing.Point(16, 481);
            this.cmbCcyService.Name = "cmbCcyService";
            this.cmbCcyService.Size = new System.Drawing.Size(150, 21);
            this.cmbCcyService.TabIndex = 21;
            this.cmbCcyService.Visible = false;
            this.cmbCcyService.SelectedIndexChanged += new System.EventHandler(this.cmbCcyService_SelectedIndexChanged);
            // 
            // pnlMargin
            // 
            this.pnlMargin.Controls.Add(this.label1);
            this.pnlMargin.Controls.Add(this.txtRateAdjustment);
            this.pnlMargin.Controls.Add(this.label2);
            this.pnlMargin.Location = new System.Drawing.Point(12, 431);
            this.pnlMargin.Name = "pnlMargin";
            this.pnlMargin.Size = new System.Drawing.Size(323, 32);
            this.pnlMargin.TabIndex = 22;
            // 
            // cmbCcyDatepoint
            // 
            this.cmbCcyDatepoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmbCcyDatepoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCcyDatepoint.FormattingEnabled = true;
            this.cmbCcyDatepoint.Location = new System.Drawing.Point(172, 481);
            this.cmbCcyDatepoint.Name = "cmbCcyDatepoint";
            this.cmbCcyDatepoint.Size = new System.Drawing.Size(150, 21);
            this.cmbCcyDatepoint.TabIndex = 23;
            this.cmbCcyDatepoint.Visible = false;
            // 
            // lnkOptions
            // 
            this.lnkOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkOptions.AutoSize = true;
            this.lnkOptions.Location = new System.Drawing.Point(15, 484);
            this.lnkOptions.Name = "lnkOptions";
            this.lnkOptions.Size = new System.Drawing.Size(47, 13);
            this.lnkOptions.TabIndex = 24;
            this.lnkOptions.TabStop = true;
            this.lnkOptions.Text = "(options)";
            this.toolTip1.SetToolTip(this.lnkOptions, "Click to enable currency change");
            this.lnkOptions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOptions_LinkClicked);
            // 
            // CurrencyUpdater
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(542, 514);
            this.Controls.Add(this.lnkOptions);
            this.Controls.Add(this.cmbCcyDatepoint);
            this.Controls.Add(this.pnlMargin);
            this.Controls.Add(this.cmbCcyService);
            this.Controls.Add(this.lnkEdit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbCurrency);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.gridBookings);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CurrencyUpdater";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TourWriter Update Currencies";
            ((System.ComponentModel.ISupportInitialize)(this.gridBookings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRateAdjustment)).EndInit();
            this.pnlMargin.ResumeLayout(false);
            this.pnlMargin.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid gridBookings;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnUpdate;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtRateAdjustment;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbCurrency;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel lnkEdit;
        private System.Windows.Forms.ComboBox cmbCcyService;
        private System.Windows.Forms.Panel pnlMargin;
        private System.Windows.Forms.ComboBox cmbCcyDatepoint;
        private System.Windows.Forms.LinkLabel lnkOptions;
    }
}