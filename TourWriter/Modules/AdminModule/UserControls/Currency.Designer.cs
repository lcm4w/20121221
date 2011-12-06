namespace TourWriter.Modules.AdminModule.UserControls
{
    partial class Currency
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Currency));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDateTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbCurrencyTo = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.txtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.cmbCurrencyFrom = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.gridRate = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.btnCurrencyEdit = new System.Windows.Forms.LinkLabel();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbDatePoint = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gridCurrency = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbRateSource = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtCurrency = new System.Windows.Forms.TextBox();
            this.lnkDefaultCurrency = new System.Windows.Forms.LinkLabel();
            this.tsServiceTimes = new TourWriter.UserControls.MyToolStrip();
            this.toolstripRates = new TourWriter.UserControls.MyToolStrip();
            this.btnRateAdd = new System.Windows.Forms.ToolStripButton();
            this.btnRateDel = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRate)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrency)).BeginInit();
            this.toolstripRates.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.ItemSize = new System.Drawing.Size(62, 18);
            this.tabControl1.Location = new System.Drawing.Point(3, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(847, 553);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(770, 527);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Exchange Rates";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.gridRate);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tsServiceTimes);
            this.panel1.Controls.Add(this.toolstripRates);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(764, 521);
            this.panel1.TabIndex = 19;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.CausesValidation = false;
            this.label12.Location = new System.Drawing.Point(6, 10);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(184, 13);
            this.label12.TabIndex = 139;
            this.label12.Text = "Enter forward curreny exchange rates";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Red;
            this.panel2.Controls.Add(this.btnClear);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.txtDateTo);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.btnLoad);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.cmbCurrencyTo);
            this.panel2.Controls.Add(this.txtDateFrom);
            this.panel2.Controls.Add(this.cmbCurrencyFrom);
            this.panel2.Location = new System.Drawing.Point(326, 304);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(416, 67);
            this.panel2.TabIndex = 138;
            this.panel2.Visible = false;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(333, 38);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 17;
            this.btnClear.Text = "Clear";
            this.toolTip1.SetToolTip(this.btnClear, "Clear data view");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Currencies:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "from";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Dates:";
            // 
            // txtDateTo
            // 
            this.txtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtDateTo.Location = new System.Drawing.Point(227, 38);
            this.txtDateTo.Name = "txtDateTo";
            this.txtDateTo.Size = new System.Drawing.Size(93, 20);
            this.txtDateTo.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(205, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "to";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(67, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "from";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(333, 7);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 15;
            this.btnLoad.Text = "Search";
            this.toolTip1.SetToolTip(this.btnLoad, "Search for currencies");
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(205, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "to";
            // 
            // cmbCurrencyTo
            // 
            this.cmbCurrencyTo.CheckedListSettings.CheckStateMember = "";
            this.cmbCurrencyTo.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.cmbCurrencyTo.Location = new System.Drawing.Point(227, 8);
            this.cmbCurrencyTo.Name = "cmbCurrencyTo";
            this.cmbCurrencyTo.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.cmbCurrencyTo.Size = new System.Drawing.Size(93, 22);
            this.cmbCurrencyTo.TabIndex = 5;
            this.cmbCurrencyTo.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.cmbCurrencyTo_InitializeLayout);
            // 
            // txtDateFrom
            // 
            this.txtDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtDateFrom.Location = new System.Drawing.Point(100, 38);
            this.txtDateFrom.Name = "txtDateFrom";
            this.txtDateFrom.Size = new System.Drawing.Size(93, 20);
            this.txtDateFrom.TabIndex = 7;
            // 
            // cmbCurrencyFrom
            // 
            this.cmbCurrencyFrom.CheckedListSettings.CheckStateMember = "";
            this.cmbCurrencyFrom.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.cmbCurrencyFrom.Location = new System.Drawing.Point(100, 8);
            this.cmbCurrencyFrom.Name = "cmbCurrencyFrom";
            this.cmbCurrencyFrom.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.cmbCurrencyFrom.Size = new System.Drawing.Size(93, 22);
            this.cmbCurrencyFrom.TabIndex = 2;
            this.cmbCurrencyFrom.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.cmbCurrencyFrom_InitializeLayout);
            // 
            // gridRate
            // 
            this.gridRate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridRate.Location = new System.Drawing.Point(3, 26);
            this.gridRate.Name = "gridRate";
            this.gridRate.Size = new System.Drawing.Size(758, 492);
            this.gridRate.TabIndex = 1;
            this.gridRate.Text = "ultraGrid2";
            this.gridRate.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridRate_InitializeLayout);
            this.gridRate.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.gridRate_InitializeRow);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Rates";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.btnCurrencyEdit);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.cmbDatePoint);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.gridCurrency);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.cmbRateSource);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(839, 527);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Currencies Setup";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(139, 239);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(177, 20);
            this.textBox1.TabIndex = 147;
            this.textBox1.Text = "       (edit this in Agent labels)";
            // 
            // label16
            // 
            this.label16.CausesValidation = false;
            this.label16.Location = new System.Drawing.Point(14, 267);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(302, 52);
            this.label16.TabIndex = 146;
            this.label16.Text = "Only for Spot Rates, this sets a default margin to add to the \'now\' rate returned" +
    " from the Currency Exchange Webservice.\r\nSet this value in the Agent Labels setu" +
    "p area.";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.CausesValidation = false;
            this.label15.Location = new System.Drawing.Point(14, 242);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(116, 13);
            this.label15.TabIndex = 144;
            this.label15.Text = "Exchange Rate Margin";
            // 
            // btnCurrencyEdit
            // 
            this.btnCurrencyEdit.AutoSize = true;
            this.btnCurrencyEdit.Location = new System.Drawing.Point(750, 13);
            this.btnCurrencyEdit.Name = "btnCurrencyEdit";
            this.btnCurrencyEdit.Size = new System.Drawing.Size(87, 13);
            this.btnCurrencyEdit.TabIndex = 143;
            this.btnCurrencyEdit.TabStop = true;
            this.btnCurrencyEdit.Text = "(click to edit text)";
            this.btnCurrencyEdit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnCurrencyEdit_LinkClicked);
            // 
            // label13
            // 
            this.label13.CausesValidation = false;
            this.label13.Location = new System.Drawing.Point(14, 159);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(302, 58);
            this.label13.TabIndex = 140;
            this.label13.Text = "Only for Forward Rates, this sets the date that is used to lookup the predifined " +
    "exchange rate - either based on the start date of the specific booking, or the i" +
    "tinerary.";
            // 
            // cmbDatePoint
            // 
            this.cmbDatePoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDatePoint.FormattingEnabled = true;
            this.cmbDatePoint.Location = new System.Drawing.Point(139, 131);
            this.cmbDatePoint.Name = "cmbDatePoint";
            this.cmbDatePoint.Size = new System.Drawing.Size(177, 21);
            this.cmbDatePoint.TabIndex = 141;
            // 
            // label14
            // 
            this.label14.CausesValidation = false;
            this.label14.Location = new System.Drawing.Point(14, 56);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(319, 55);
            this.label14.TabIndex = 142;
            this.label14.Text = "This sets where to source the currency exchange rates from, either from a webserv" +
    "ice that gets the \'now\' Spot Rate, or from Forward Rates that you enter yourself" +
    " in advance.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.CausesValidation = false;
            this.label5.Location = new System.Drawing.Point(339, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(215, 13);
            this.label5.TabIndex = 128;
            this.label5.Text = "Default Currencies  -  select which to enable";
            // 
            // gridCurrency
            // 
            this.gridCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridCurrency.Location = new System.Drawing.Point(339, 29);
            this.gridCurrency.Name = "gridCurrency";
            this.gridCurrency.Size = new System.Drawing.Size(494, 495);
            this.gridCurrency.TabIndex = 0;
            this.gridCurrency.Text = "ultraGrid1";
            this.gridCurrency.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridCurrency_InitializeLayout);
            this.gridCurrency.Error += new Infragistics.Win.UltraWinGrid.ErrorEventHandler(this.gridCurrency_Error);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.CausesValidation = false;
            this.label4.Location = new System.Drawing.Point(14, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 13);
            this.label4.TabIndex = 136;
            this.label4.Text = "Conversion Date-point";
            // 
            // cmbRateSource
            // 
            this.cmbRateSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRateSource.FormattingEnabled = true;
            this.cmbRateSource.Location = new System.Drawing.Point(139, 29);
            this.cmbRateSource.Name = "cmbRateSource";
            this.cmbRateSource.Size = new System.Drawing.Size(177, 21);
            this.cmbRateSource.TabIndex = 138;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.CausesValidation = false;
            this.label10.Location = new System.Drawing.Point(14, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(123, 13);
            this.label10.TabIndex = 137;
            this.label10.Text = "Exchange Rates Source";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.CausesValidation = false;
            this.label11.Location = new System.Drawing.Point(10, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(113, 13);
            this.label11.TabIndex = 127;
            this.label11.Text = "Base System Currency";
            // 
            // txtCurrency
            // 
            this.txtCurrency.Location = new System.Drawing.Point(139, 8);
            this.txtCurrency.Name = "txtCurrency";
            this.txtCurrency.ReadOnly = true;
            this.txtCurrency.Size = new System.Drawing.Size(343, 20);
            this.txtCurrency.TabIndex = 139;
            // 
            // lnkDefaultCurrency
            // 
            this.lnkDefaultCurrency.AutoSize = true;
            this.lnkDefaultCurrency.Location = new System.Drawing.Point(488, 12);
            this.lnkDefaultCurrency.Name = "lnkDefaultCurrency";
            this.lnkDefaultCurrency.Size = new System.Drawing.Size(67, 13);
            this.lnkDefaultCurrency.TabIndex = 140;
            this.lnkDefaultCurrency.TabStop = true;
            this.lnkDefaultCurrency.Text = "(click to edit)";
            this.lnkDefaultCurrency.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDefaultCurrency_LinkClicked);
            // 
            // tsServiceTimes
            // 
            this.tsServiceTimes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tsServiceTimes.BackColor = System.Drawing.Color.Transparent;
            this.tsServiceTimes.DisableAllMenuItems = false;
            this.tsServiceTimes.Dock = System.Windows.Forms.DockStyle.None;
            this.tsServiceTimes.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsServiceTimes.Location = new System.Drawing.Point(659, 3);
            this.tsServiceTimes.Name = "tsServiceTimes";
            this.tsServiceTimes.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsServiceTimes.Size = new System.Drawing.Size(102, 25);
            this.tsServiceTimes.TabIndex = 121;
            this.tsServiceTimes.Text = "tsServiceTimes";
            this.tsServiceTimes.Visible = false;
            // 
            // toolstripRates
            // 
            this.toolstripRates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolstripRates.BackColor = System.Drawing.Color.Transparent;
            this.toolstripRates.DisableAllMenuItems = true;
            this.toolstripRates.Dock = System.Windows.Forms.DockStyle.None;
            this.toolstripRates.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolstripRates.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRateAdd,
            this.btnRateDel});
            this.toolstripRates.Location = new System.Drawing.Point(717, 3);
            this.toolstripRates.Name = "toolstripRates";
            this.toolstripRates.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolstripRates.Size = new System.Drawing.Size(49, 25);
            this.toolstripRates.TabIndex = 123;
            this.toolstripRates.Text = "myToolStrip2";
            // 
            // btnRateAdd
            // 
            this.btnRateAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRateAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnRateAdd.Image")));
            this.btnRateAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRateAdd.Name = "btnRateAdd";
            this.btnRateAdd.Size = new System.Drawing.Size(23, 22);
            this.btnRateAdd.ToolTipText = "Add new currency rate(s)";
            this.btnRateAdd.Click += new System.EventHandler(this.btnRateAdd_Click);
            // 
            // btnRateDel
            // 
            this.btnRateDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRateDel.Image = ((System.Drawing.Image)(resources.GetObject("btnRateDel.Image")));
            this.btnRateDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRateDel.Name = "btnRateDel";
            this.btnRateDel.Size = new System.Drawing.Size(23, 22);
            this.btnRateDel.Text = "btnCurrencyDelete";
            this.btnRateDel.ToolTipText = "Delete selected currency rate(s)";
            this.btnRateDel.Click += new System.EventHandler(this.btnRateDel_Click);
            // 
            // Currency
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.lnkDefaultCurrency);
            this.Controls.Add(this.txtCurrency);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label11);
            this.Name = "Currency";
            this.Size = new System.Drawing.Size(853, 590);
            this.Load += new System.EventHandler(this.Currency_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRate)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrency)).EndInit();
            this.toolstripRates.ResumeLayout(false);
            this.toolstripRates.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridCurrency;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridRate;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbCurrencyFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker txtDateTo;
        private System.Windows.Forms.DateTimePicker txtDateFrom;
        private System.Windows.Forms.Label label3;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbCurrencyTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private TourWriter.UserControls.MyToolStrip tsServiceTimes;
        private TourWriter.UserControls.MyToolStrip toolstripRates;
        private System.Windows.Forms.ToolStripButton btnRateAdd;
        private System.Windows.Forms.ToolStripButton btnRateDel;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbRateSource;
        private System.Windows.Forms.TextBox txtCurrency;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbDatePoint;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.LinkLabel btnCurrencyEdit;
        private System.Windows.Forms.LinkLabel lnkDefaultCurrency;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBox1;
    }
}
