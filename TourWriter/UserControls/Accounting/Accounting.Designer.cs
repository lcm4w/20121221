namespace TourWriter.UserControls.Accounting
{
    partial class Accounting
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl22 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.label5 = new System.Windows.Forms.Label();
            this.ultraTabPageControl23 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.label2 = new System.Windows.Forms.Label();
            this.ultraTabSharedControlsPage4 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.tabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.lblHeading = new System.Windows.Forms.Label();
            this.txtTo = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.txtFrom = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.pnlDates = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.itineraryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.itineraryStatusBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.gridPurchases = new TourWriter.UserControls.DataExtractGrid();
            this.gridSales = new TourWriter.UserControls.DataExtractGrid();
            this.ultraTabPageControl22.SuspendLayout();
            this.ultraTabPageControl23.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrom)).BeginInit();
            this.pnlDates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryStatusBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl22
            // 
            this.ultraTabPageControl22.Controls.Add(this.label5);
            this.ultraTabPageControl22.Controls.Add(this.gridPurchases);
            this.ultraTabPageControl22.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl22.Name = "ultraTabPageControl22";
            this.ultraTabPageControl22.Size = new System.Drawing.Size(907, 464);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Purchases accounting data";
            // 
            // ultraTabPageControl23
            // 
            this.ultraTabPageControl23.Controls.Add(this.label2);
            this.ultraTabPageControl23.Controls.Add(this.gridSales);
            this.ultraTabPageControl23.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl23.Name = "ultraTabPageControl23";
            this.ultraTabPageControl23.Size = new System.Drawing.Size(907, 464);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sales accounting data";
            // 
            // ultraTabSharedControlsPage4
            // 
            this.ultraTabSharedControlsPage4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage4.Name = "ultraTabSharedControlsPage4";
            this.ultraTabSharedControlsPage4.Size = new System.Drawing.Size(907, 464);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.BackColorInternal = System.Drawing.Color.Transparent;
            this.tabControl.Controls.Add(this.ultraTabSharedControlsPage4);
            this.tabControl.Controls.Add(this.ultraTabPageControl22);
            this.tabControl.Controls.Add(this.ultraTabPageControl23);
            this.tabControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(8, 37);
            this.tabControl.MinTabWidth = 70;
            this.tabControl.Name = "tabControl";
            this.tabControl.SharedControlsPage = this.ultraTabSharedControlsPage4;
            this.tabControl.Size = new System.Drawing.Size(911, 490);
            this.tabControl.TabIndex = 6;
            ultraTab2.Key = "Purchases";
            ultraTab2.TabPage = this.ultraTabPageControl22;
            ultraTab2.Text = "Purchases";
            ultraTab4.Key = "Sales";
            ultraTab4.TabPage = this.ultraTabPageControl23;
            ultraTab4.Text = "Sales";
            this.tabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab2,
            ultraTab4});
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.BackColor = System.Drawing.Color.Transparent;
            this.lblHeading.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.Location = new System.Drawing.Point(7, 15);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(109, 13);
            this.lblHeading.TabIndex = 186;
            this.lblHeading.Text = "Accounting data for: ";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(274, 6);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(87, 21);
            this.txtTo.TabIndex = 191;
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(161, 6);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(87, 21);
            this.txtFrom.TabIndex = 190;
            // 
            // pnlDates
            // 
            this.pnlDates.Controls.Add(this.label4);
            this.pnlDates.Controls.Add(this.comboBox3);
            this.pnlDates.Controls.Add(this.label3);
            this.pnlDates.Controls.Add(this.comboBox2);
            this.pnlDates.Controls.Add(this.label1);
            this.pnlDates.Controls.Add(this.comboBox1);
            this.pnlDates.Controls.Add(this.txtTo);
            this.pnlDates.Controls.Add(this.label6);
            this.pnlDates.Controls.Add(this.txtFrom);
            this.pnlDates.Location = new System.Drawing.Point(107, 3);
            this.pnlDates.Name = "pnlDates";
            this.pnlDates.Size = new System.Drawing.Size(809, 34);
            this.pnlDates.TabIndex = 194;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(374, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 197;
            this.label3.Text = "with Booking";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(445, 7);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 196;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(133, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 195;
            this.label1.Text = "from";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(6, 7);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 194;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(257, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 13);
            this.label6.TabIndex = 193;
            this.label6.Text = "to";
            // 
            // itineraryBindingSource
            // 
            this.itineraryBindingSource.DataMember = "Itinerary";
            this.itineraryBindingSource.DataSource = typeof(TourWriter.Info.ItinerarySet);
            // 
            // itineraryStatusBindingSource
            // 
            this.itineraryStatusBindingSource.DataMember = "ItineraryStatus";
            this.itineraryStatusBindingSource.DataSource = typeof(TourWriter.Info.ToolSet);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(579, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 199;
            this.label4.Text = "with Itinerary";
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(650, 7);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(121, 21);
            this.comboBox3.TabIndex = 198;
            // 
            // gridPurchases
            // 
            this.gridPurchases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPurchases.DataSource = null;
            this.gridPurchases.ExportFileName = "";
            this.gridPurchases.GridEnabled = true;
            this.gridPurchases.Location = new System.Drawing.Point(0, 3);
            this.gridPurchases.Name = "gridPurchases";
            this.gridPurchases.Size = new System.Drawing.Size(907, 461);
            this.gridPurchases.TabIndex = 3;
            // 
            // gridSales
            // 
            this.gridSales.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridSales.DataSource = null;
            this.gridSales.ExportFileName = "";
            this.gridSales.GridEnabled = true;
            this.gridSales.Location = new System.Drawing.Point(0, 3);
            this.gridSales.Name = "gridSales";
            this.gridSales.Size = new System.Drawing.Size(907, 461);
            this.gridSales.TabIndex = 0;
            // 
            // Accounting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlDates);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.tabControl);
            this.Name = "Accounting";
            this.Size = new System.Drawing.Size(925, 530);
            this.ultraTabPageControl22.ResumeLayout(false);
            this.ultraTabPageControl22.PerformLayout();
            this.ultraTabPageControl23.ResumeLayout(false);
            this.ultraTabPageControl23.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFrom)).EndInit();
            this.pnlDates.ResumeLayout(false);
            this.pnlDates.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryStatusBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl23;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl22;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage4;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabControl;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.BindingSource itineraryBindingSource;
        private System.Windows.Forms.BindingSource itineraryStatusBindingSource;
        private TourWriter.UserControls.DataExtractGrid gridSales;
        private TourWriter.UserControls.DataExtractGrid gridPurchases;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtTo;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor txtFrom;
        private System.Windows.Forms.Panel pnlDates;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox3;

    }
}
