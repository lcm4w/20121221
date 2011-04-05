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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtCurrency = new System.Windows.Forms.Label();
            this.btnList = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.btnDefault = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.gridCurrency = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.gridRate = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.tsServiceTimes = new TourWriter.UserControls.MyToolStrip();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnRateAdd = new System.Windows.Forms.ToolStripButton();
            this.btnRateDel = new System.Windows.Forms.ToolStripButton();
            this.ultraExpandableGroupBox1 = new Infragistics.Win.Misc.UltraExpandableGroupBox();
            this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
            this.btnClear = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.cmbCurrencyTo = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.cmbCurrencyFrom = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.txtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtDateTo = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrency)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRate)).BeginInit();
            this.myToolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).BeginInit();
            this.ultraExpandableGroupBox1.SuspendLayout();
            this.ultraExpandableGroupBoxPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyFrom)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 14);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(778, 573);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtCurrency);
            this.tabPage1.Controls.Add(this.btnList);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.btnDefault);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.gridCurrency);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(770, 547);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Currencies";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtCurrency
            // 
            this.txtCurrency.AutoSize = true;
            this.txtCurrency.CausesValidation = false;
            this.txtCurrency.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrency.ForeColor = System.Drawing.Color.Gray;
            this.txtCurrency.Location = new System.Drawing.Point(103, 17);
            this.txtCurrency.Name = "txtCurrency";
            this.txtCurrency.Size = new System.Drawing.Size(134, 20);
            this.txtCurrency.TabIndex = 131;
            this.txtCurrency.Text = "Your sell currency";
            // 
            // btnList
            // 
            this.btnList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnList.Location = new System.Drawing.Point(635, 61);
            this.btnList.Name = "btnList";
            this.btnList.Size = new System.Drawing.Size(129, 23);
            this.btnList.TabIndex = 130;
            this.btnList.Text = "Enable List Editing";
            this.btnList.UseVisualStyleBackColor = true;
            this.btnList.Click += new System.EventHandler(this.btnList_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.CausesValidation = false;
            this.label11.Location = new System.Drawing.Point(6, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 13);
            this.label11.TabIndex = 127;
            this.label11.Text = "Your sell currency";
            // 
            // btnDefault
            // 
            this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefault.Location = new System.Drawing.Point(635, 17);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(129, 23);
            this.btnDefault.TabIndex = 129;
            this.btnDefault.Text = "Edit Default Currency";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.CausesValidation = false;
            this.label5.Location = new System.Drawing.Point(6, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(246, 13);
            this.label5.TabIndex = 128;
            this.label5.Text = "Select the buy/sell currencies that you want to use";
            // 
            // gridCurrency
            // 
            this.gridCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCurrency.Location = new System.Drawing.Point(6, 85);
            this.gridCurrency.Name = "gridCurrency";
            this.gridCurrency.Size = new System.Drawing.Size(761, 459);
            this.gridCurrency.TabIndex = 0;
            this.gridCurrency.Text = "ultraGrid1";
            this.gridCurrency.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridCurrency_InitializeLayout);
            this.gridCurrency.Error += new Infragistics.Win.UltraWinGrid.ErrorEventHandler(this.gridCurrency_Error);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.ultraExpandableGroupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(770, 547);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Rates";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.gridRate);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tsServiceTimes);
            this.panel1.Controls.Add(this.myToolStrip2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 86);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(764, 458);
            this.panel1.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(9, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(583, 17);
            this.label4.TabIndex = 124;
            this.label4.Text = "Add/remove is disabled, please use new update functionality in Itinerary > Bookin" +
                "gs > Tools";
            // 
            // gridRate
            // 
            this.gridRate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridRate.Location = new System.Drawing.Point(3, 50);
            this.gridRate.Name = "gridRate";
            this.gridRate.Size = new System.Drawing.Size(758, 405);
            this.gridRate.TabIndex = 1;
            this.gridRate.Text = "ultraGrid2";
            this.gridRate.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridRate_InitializeLayout);
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
            // myToolStrip2
            // 
            this.myToolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRateAdd,
            this.btnRateDel});
            this.myToolStrip2.Location = new System.Drawing.Point(717, 27);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip2.TabIndex = 123;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnRateAdd
            // 
            this.btnRateAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRateAdd.Enabled = false;
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
            this.btnRateDel.Enabled = false;
            this.btnRateDel.Image = ((System.Drawing.Image)(resources.GetObject("btnRateDel.Image")));
            this.btnRateDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRateDel.Name = "btnRateDel";
            this.btnRateDel.Size = new System.Drawing.Size(23, 22);
            this.btnRateDel.Text = "btnCurrencyDelete";
            this.btnRateDel.ToolTipText = "Delete selected currency rate(s)";
            this.btnRateDel.Click += new System.EventHandler(this.btnRateDel_Click);
            // 
            // ultraExpandableGroupBox1
            // 
            this.ultraExpandableGroupBox1.Controls.Add(this.ultraExpandableGroupBoxPanel1);
            this.ultraExpandableGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraExpandableGroupBox1.ExpandedSize = new System.Drawing.Size(764, 83);
            this.ultraExpandableGroupBox1.Location = new System.Drawing.Point(3, 3);
            this.ultraExpandableGroupBox1.Name = "ultraExpandableGroupBox1";
            this.ultraExpandableGroupBox1.Size = new System.Drawing.Size(764, 83);
            this.ultraExpandableGroupBox1.TabIndex = 18;
            this.ultraExpandableGroupBox1.Text = "Search rates data";
            // 
            // ultraExpandableGroupBoxPanel1
            // 
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.btnClear);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label6);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label7);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label3);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.btnLoad);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cmbCurrencyTo);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cmbCurrencyFrom);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.txtDateFrom);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label8);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label9);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.txtDateTo);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label1);
            this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
            this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
            this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(758, 61);
            this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(385, 35);
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
            this.label6.Location = new System.Drawing.Point(3, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Currencies:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Dates:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(205, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "to";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(385, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 15;
            this.btnLoad.Text = "Search";
            this.toolTip1.SetToolTip(this.btnLoad, "Search for currencies");
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // cmbCurrencyTo
            // 
            this.cmbCurrencyTo.CheckedListSettings.CheckStateMember = "";
            this.cmbCurrencyTo.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.cmbCurrencyTo.Location = new System.Drawing.Point(227, 5);
            this.cmbCurrencyTo.Name = "cmbCurrencyTo";
            this.cmbCurrencyTo.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.cmbCurrencyTo.Size = new System.Drawing.Size(93, 22);
            this.cmbCurrencyTo.TabIndex = 5;
            this.cmbCurrencyTo.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.cmbCurrencyTo_InitializeLayout);
            // 
            // cmbCurrencyFrom
            // 
            this.cmbCurrencyFrom.CheckedListSettings.CheckStateMember = "";
            this.cmbCurrencyFrom.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.cmbCurrencyFrom.Location = new System.Drawing.Point(100, 5);
            this.cmbCurrencyFrom.Name = "cmbCurrencyFrom";
            this.cmbCurrencyFrom.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.cmbCurrencyFrom.Size = new System.Drawing.Size(93, 22);
            this.cmbCurrencyFrom.TabIndex = 2;
            this.cmbCurrencyFrom.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.cmbCurrencyFrom_InitializeLayout);
            // 
            // txtDateFrom
            // 
            this.txtDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtDateFrom.Location = new System.Drawing.Point(100, 35);
            this.txtDateFrom.Name = "txtDateFrom";
            this.txtDateFrom.Size = new System.Drawing.Size(93, 20);
            this.txtDateFrom.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(205, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "to";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(67, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "from";
            // 
            // txtDateTo
            // 
            this.txtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtDateTo.Location = new System.Drawing.Point(227, 35);
            this.txtDateTo.Name = "txtDateTo";
            this.txtDateTo.Size = new System.Drawing.Size(93, 20);
            this.txtDateTo.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "from";
            // 
            // Currency
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.tabControl1);
            this.Name = "Currency";
            this.Size = new System.Drawing.Size(784, 590);
            this.Load += new System.EventHandler(this.Currency_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCurrency)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRate)).EndInit();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).EndInit();
            this.ultraExpandableGroupBox1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyFrom)).EndInit();
            this.ResumeLayout(false);

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
        private Infragistics.Win.Misc.UltraExpandableGroupBox ultraExpandableGroupBox1;
        private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
        private TourWriter.UserControls.MyToolStrip tsServiceTimes;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private System.Windows.Forms.ToolStripButton btnRateAdd;
        private System.Windows.Forms.ToolStripButton btnRateDel;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnList;
        private System.Windows.Forms.Label txtCurrency;
    }
}
