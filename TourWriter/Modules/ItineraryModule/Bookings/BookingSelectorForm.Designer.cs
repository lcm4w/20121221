namespace TourWriter.Modules.ItineraryModule.Bookings
{
    partial class BookingSelectorForm
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
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.tabSearch = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.supplierSearch1 = new TourWriter.Modules.SupplierModule.SupplierSearch();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.serviceEditor1 = new TourWriter.Modules.SupplierModule.ServiceEditor();
            this.txtSupplierName = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkCurrency = new System.Windows.Forms.CheckBox();
            this.chkIncrementDates = new System.Windows.Forms.CheckBox();
            this.lblBooking = new System.Windows.Forms.Label();
            this.cmbBookings = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.gridBookings = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.myToolStrip3 = new TourWriter.UserControls.MyToolStrip();
            this.btnOptionDel = new System.Windows.Forms.ToolStripButton();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage7 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.popupControler = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tabSearch.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbBookings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBookings)).BeginInit();
            this.myToolStrip3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabSearch
            // 
            this.tabSearch.Controls.Add(this.supplierSearch1);
            this.tabSearch.Location = new System.Drawing.Point(-10000, -10000);
            this.tabSearch.Name = "tabSearch";
            this.tabSearch.Size = new System.Drawing.Size(642, 449);
            // 
            // supplierSearch1
            // 
            this.supplierSearch1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.supplierSearch1.BackColor = System.Drawing.Color.Transparent;
            this.supplierSearch1.Location = new System.Drawing.Point(0, 3);
            this.supplierSearch1.Name = "supplierSearch1";
            this.supplierSearch1.Size = new System.Drawing.Size(642, 424);
            this.supplierSearch1.TabIndex = 0;
            this.supplierSearch1.OnSupplierPicked += new TourWriter.Modules.SupplierModule.OnSupplierPickedHandler(this.supplierSearch1_OnSupplierPicked);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.panel2);
            this.ultraTabPageControl1.Controls.Add(this.splitter1);
            this.ultraTabPageControl1.Controls.Add(this.panel1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(642, 449);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.serviceEditor1);
            this.panel2.Controls.Add(this.txtSupplierName);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(642, 329);
            this.panel2.TabIndex = 147;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Supplier";
            // 
            // serviceEditor1
            // 
            this.serviceEditor1.AllowEditing = false;
            this.serviceEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serviceEditor1.AutoScroll = true;
            this.serviceEditor1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.serviceEditor1.Location = new System.Drawing.Point(2, 26);
            this.serviceEditor1.Name = "serviceEditor1";
            this.serviceEditor1.Size = new System.Drawing.Size(640, 297);
            this.serviceEditor1.TabIndex = 0;
            this.serviceEditor1.OnSupplierSelected += new TourWriter.Modules.SupplierModule.OnSupplierSelectedHandler(this.serviceEditor1_OnSupplierSelected);
            this.serviceEditor1.OnRateSelected += new TourWriter.Modules.SupplierModule.OnRateSelectedHandler(this.serviceEditor1_OnRateSelected);
            this.serviceEditor1.OnOptionSelected += new TourWriter.Modules.SupplierModule.OnOptionSelectedHandler(this.serviceEditor1_OnOptionSelected);
            this.serviceEditor1.OnOptionCheckedChanged += new TourWriter.Modules.SupplierModule.OnOptionCheckedChangedHandler(this.serviceEditor1_OnOptionCheckedChanged);
            // 
            // txtSupplierName
            // 
            this.txtSupplierName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSupplierName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSupplierName.Location = new System.Drawing.Point(56, 7);
            this.txtSupplierName.Name = "txtSupplierName";
            this.txtSupplierName.ReadOnly = true;
            this.txtSupplierName.Size = new System.Drawing.Size(580, 20);
            this.txtSupplierName.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtSupplierName, "Name of the supplier that is displayed below");
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.Silver;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 329);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(642, 3);
            this.splitter1.TabIndex = 146;
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkCurrency);
            this.panel1.Controls.Add(this.chkIncrementDates);
            this.panel1.Controls.Add(this.lblBooking);
            this.panel1.Controls.Add(this.cmbBookings);
            this.panel1.Controls.Add(this.gridBookings);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.myToolStrip3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 332);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(642, 117);
            this.panel1.TabIndex = 145;
            // 
            // chkCurrency
            // 
            this.chkCurrency.AutoSize = true;
            this.chkCurrency.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCurrency.Location = new System.Drawing.Point(433, 27);
            this.chkCurrency.Name = "chkCurrency";
            this.chkCurrency.Size = new System.Drawing.Size(105, 17);
            this.chkCurrency.TabIndex = 149;
            this.chkCurrency.Text = "Update currency";
            this.toolTip1.SetToolTip(this.chkCurrency, "Update the currency exchange rate on load");
            this.chkCurrency.UseVisualStyleBackColor = true;
            this.chkCurrency.Visible = false;
            // 
            // chkIncrementDates
            // 
            this.chkIncrementDates.AutoSize = true;
            this.chkIncrementDates.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkIncrementDates.Location = new System.Drawing.Point(8, 27);
            this.chkIncrementDates.Name = "chkIncrementDates";
            this.chkIncrementDates.Size = new System.Drawing.Size(102, 17);
            this.chkIncrementDates.TabIndex = 148;
            this.chkIncrementDates.Text = "Increment dates";
            this.toolTip1.SetToolTip(this.chkIncrementDates, "Increment (rather than copy) the date of bookings as they are added");
            this.chkIncrementDates.UseVisualStyleBackColor = true;
            // 
            // lblBooking
            // 
            this.lblBooking.AutoSize = true;
            this.lblBooking.Location = new System.Drawing.Point(131, 28);
            this.lblBooking.Name = "lblBooking";
            this.lblBooking.Size = new System.Drawing.Size(68, 13);
            this.lblBooking.TabIndex = 147;
            this.lblBooking.Text = "Add items to:";
            // 
            // cmbBookings
            // 
            this.cmbBookings.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cmbBookings.Location = new System.Drawing.Point(201, 24);
            this.cmbBookings.Name = "cmbBookings";
            this.cmbBookings.Size = new System.Drawing.Size(209, 21);
            this.cmbBookings.TabIndex = 146;
            this.toolTip1.SetToolTip(this.cmbBookings, "Choose where to add this item (eg add to existing booking)");
            this.cmbBookings.ValueChanged += new System.EventHandler(this.cmbBookings_ValueChanged);
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
            this.gridBookings.Location = new System.Drawing.Point(3, 47);
            this.gridBookings.Name = "gridBookings";
            this.gridBookings.Size = new System.Drawing.Size(636, 70);
            this.gridBookings.TabIndex = 144;
            this.gridBookings.Text = "ultraGrid1";
            this.gridBookings.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.gridBookings.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridBookings_AfterCellUpdate);
            this.gridBookings.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridBookings_InitializeLayout);
            this.gridBookings.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridBookings_CellChange);
            this.gridBookings.AfterCellListCloseUp += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridBookings_AfterCellListCloseUp);
            this.gridBookings.CellDataError += new Infragistics.Win.UltraWinGrid.CellDataErrorEventHandler(this.gridBookings_CellDataError);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(303, 13);
            this.label2.TabIndex = 145;
            this.label2.Text = "Selected bookings (tick options above right to select bookings)";
            // 
            // myToolStrip3
            // 
            this.myToolStrip3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip3.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip3.DisableAllMenuItems = true;
            this.myToolStrip3.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOptionDel});
            this.myToolStrip3.Location = new System.Drawing.Point(616, 24);
            this.myToolStrip3.Name = "myToolStrip3";
            this.myToolStrip3.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.myToolStrip3.Size = new System.Drawing.Size(26, 25);
            this.myToolStrip3.TabIndex = 143;
            this.myToolStrip3.Text = "myToolStrip3";
            // 
            // btnOptionDel
            // 
            this.btnOptionDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOptionDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnOptionDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOptionDel.Name = "btnOptionDel";
            this.btnOptionDel.Size = new System.Drawing.Size(23, 22);
            this.btnOptionDel.ToolTipText = "Remove selected booking";
            this.btnOptionDel.Click += new System.EventHandler(this.btnOptionDel_Click);
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(725, 567);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(491, 486);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 22;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(572, 486);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.ultraTabSharedControlsPage7);
            this.tabControl.Controls.Add(this.tabSearch);
            this.tabControl.Controls.Add(this.ultraTabPageControl1);
            this.tabControl.Location = new System.Drawing.Point(4, 5);
            this.tabControl.MinTabWidth = 60;
            this.tabControl.Name = "tabControl";
            appearance13.FontData.BoldAsString = "True";
            this.tabControl.SelectedTabAppearance = appearance13;
            this.tabControl.SharedControlsPage = this.ultraTabSharedControlsPage7;
            this.tabControl.Size = new System.Drawing.Size(646, 475);
            this.tabControl.TabIndex = 50;
            ultraTab1.Key = "Search";
            ultraTab1.TabPage = this.tabSearch;
            ultraTab1.Text = "Search";
            ultraTab2.Key = "Select";
            ultraTab2.TabPage = this.ultraTabPageControl1;
            ultraTab2.Text = "Select";
            this.tabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.tabControl.SelectedTabChanged += new Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventHandler(this.tabControl_SelectedTabChanged);
            // 
            // ultraTabSharedControlsPage7
            // 
            this.ultraTabSharedControlsPage7.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage7.Name = "ultraTabSharedControlsPage7";
            this.ultraTabSharedControlsPage7.Size = new System.Drawing.Size(642, 449);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(491, 486);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 51;
            this.btnNext.Text = "Select >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(400, 486);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 52;
            this.btnBack.Text = "< Search";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // popupControler
            // 
            this.popupControler.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // BookingSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(654, 514);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnNext);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(670, 530);
            this.Name = "BookingSelectorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TourWriter - Add new booking";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BookingSelectorForm_FormClosing);
            this.Load += new System.EventHandler(this.BookingSelectorForm_Load);
            this.tabSearch.ResumeLayout(false);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbBookings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBookings)).EndInit();
            this.myToolStrip3.ResumeLayout(false);
            this.myToolStrip3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage7;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl tabSearch;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private TourWriter.Modules.SupplierModule.ServiceEditor serviceEditor1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSupplierName;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private TourWriter.Modules.SupplierModule.SupplierSearch supplierSearch1;
        private Infragistics.Win.Misc.UltraPopupControlContainer popupControler;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private TourWriter.UserControls.MyToolStrip myToolStrip3;
        private System.Windows.Forms.ToolStripButton btnOptionDel;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridBookings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cmbBookings;
        private System.Windows.Forms.Label lblBooking;
        private System.Windows.Forms.CheckBox chkIncrementDates;
        private System.Windows.Forms.CheckBox chkCurrency;
    }
}
