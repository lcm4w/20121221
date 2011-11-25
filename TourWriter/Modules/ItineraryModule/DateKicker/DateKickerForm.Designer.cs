namespace TourWriter.Modules.ItineraryModule.DateKicker
{
    partial class DateKickerForm
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtDayOffset = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.chkUpdateStartDate = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gridBookings = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpNewEndDate = new TourWriter.UserControls.NullableDateTimePicker();
            this.dtpOldEndDate = new TourWriter.UserControls.NullableDateTimePicker();
            this.dtpNewStartDate = new TourWriter.UserControls.NullableDateTimePicker();
            this.dtpOldStartDate = new TourWriter.UserControls.NullableDateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtDayOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBookings)).BeginInit();
            this.SuspendLayout();
            // 
            // txtDayOffset
            // 
            this.txtDayOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDayOffset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDayOffset.FormatString = "###0";
            this.txtDayOffset.Location = new System.Drawing.Point(171, 486);
            this.txtDayOffset.MaskInput = "-nnnn";
            this.txtDayOffset.Name = "txtDayOffset";
            this.txtDayOffset.PromptChar = ' ';
            this.txtDayOffset.Size = new System.Drawing.Size(40, 21);
            this.txtDayOffset.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtDayOffset, "Number of days to move to. Positive or negative or 0 (zero) to refresh current.");
            this.txtDayOffset.Visible = false;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStop.Location = new System.Drawing.Point(566, 487);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(156, 23);
            this.btnStartStop.TabIndex = 1;
            this.btnStartStop.Text = "Refresh Rates";
            this.toolTip1.SetToolTip(this.btnStartStop, "Refresh booking Rates for current or new dates");
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // chkUpdateStartDate
            // 
            this.chkUpdateStartDate.AutoSize = true;
            this.chkUpdateStartDate.Location = new System.Drawing.Point(345, 13);
            this.chkUpdateStartDate.Name = "chkUpdateStartDate";
            this.chkUpdateStartDate.Size = new System.Drawing.Size(169, 17);
            this.chkUpdateStartDate.TabIndex = 19;
            this.chkUpdateStartDate.Text = "(also move selected bookings)";
            this.toolTip1.SetToolTip(this.chkUpdateStartDate, "Moves the booking dates too, when you change the Itinerary Start date");
            this.chkUpdateStartDate.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(566, 530);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Keep";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(647, 530);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 492);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Move selected booking items by";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(213, 492);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "days";
            this.label2.Visible = false;
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
            this.gridBookings.Location = new System.Drawing.Point(12, 75);
            this.gridBookings.Name = "gridBookings";
            this.gridBookings.Size = new System.Drawing.Size(710, 406);
            this.gridBookings.TabIndex = 7;
            this.gridBookings.Text = "ultraGrid1";
            this.gridBookings.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.gridBookings.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grid_InitializeLayout);
            this.gridBookings.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grid_InitializeRow);
            this.gridBookings.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridBookings_MouseUp);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(12, 521);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(706, 2);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // dtpNewEndDate
            // 
            this.dtpNewEndDate.CustomFormat = "dd MMM yyyy,  HH:mm - dddd";
            this.dtpNewEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNewEndDate.Location = new System.Drawing.Point(230, 38);
            this.dtpNewEndDate.Name = "dtpNewEndDate";
            this.dtpNewEndDate.NullValue = "";
            this.dtpNewEndDate.Size = new System.Drawing.Size(109, 20);
            this.dtpNewEndDate.TabIndex = 17;
            this.dtpNewEndDate.Value = new System.DateTime(2007, 7, 26, 13, 48, 29, 843);
            // 
            // dtpOldEndDate
            // 
            this.dtpOldEndDate.CustomFormat = "dd MMM yyyy,  HH:mm - dddd";
            this.dtpOldEndDate.Enabled = false;
            this.dtpOldEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOldEndDate.Location = new System.Drawing.Point(109, 38);
            this.dtpOldEndDate.Name = "dtpOldEndDate";
            this.dtpOldEndDate.NullValue = "";
            this.dtpOldEndDate.Size = new System.Drawing.Size(109, 20);
            this.dtpOldEndDate.TabIndex = 17;
            this.dtpOldEndDate.Value = new System.DateTime(2007, 7, 26, 13, 48, 29, 843);
            // 
            // dtpNewStartDate
            // 
            this.dtpNewStartDate.CustomFormat = "";
            this.dtpNewStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNewStartDate.Location = new System.Drawing.Point(230, 11);
            this.dtpNewStartDate.Name = "dtpNewStartDate";
            this.dtpNewStartDate.NullValue = "";
            this.dtpNewStartDate.Size = new System.Drawing.Size(109, 20);
            this.dtpNewStartDate.TabIndex = 17;
            this.dtpNewStartDate.Value = new System.DateTime(2007, 7, 26, 13, 48, 29, 843);
            // 
            // dtpOldStartDate
            // 
            this.dtpOldStartDate.CustomFormat = "dd MMM yyyy,  HH:mm - dddd";
            this.dtpOldStartDate.Enabled = false;
            this.dtpOldStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOldStartDate.Location = new System.Drawing.Point(109, 11);
            this.dtpOldStartDate.Name = "dtpOldStartDate";
            this.dtpOldStartDate.NullValue = "";
            this.dtpOldStartDate.Size = new System.Drawing.Size(109, 20);
            this.dtpOldStartDate.TabIndex = 17;
            this.dtpOldStartDate.Value = new System.DateTime(2007, 7, 26, 13, 48, 29, 843);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Itinerary Start date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Itinerary End date";
            // 
            // DateKickerForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(734, 565);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkUpdateStartDate);
            this.Controls.Add(this.dtpNewEndDate);
            this.Controls.Add(this.dtpOldEndDate);
            this.Controls.Add(this.dtpNewStartDate);
            this.Controls.Add(this.dtpOldStartDate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gridBookings);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDayOffset);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DateKickerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TourWriter Refresh Rates";
            ((System.ComponentModel.ISupportInitialize)(this.txtDayOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBookings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStartStop;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtDayOffset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridBookings;
        private System.Windows.Forms.GroupBox groupBox1;
        private UserControls.NullableDateTimePicker dtpOldStartDate;
        private System.Windows.Forms.CheckBox chkUpdateStartDate;
        private UserControls.NullableDateTimePicker dtpNewStartDate;
        private UserControls.NullableDateTimePicker dtpOldEndDate;
        private UserControls.NullableDateTimePicker dtpNewEndDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
