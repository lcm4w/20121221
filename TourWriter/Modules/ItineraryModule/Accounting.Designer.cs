namespace TourWriter.Modules.ItineraryModule
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl23 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gridSales = new TourWriter.UserControls.AccountingGrid();
            this.gridAllocations = new TourWriter.UserControls.AccountingGrid();
            this.label4 = new System.Windows.Forms.Label();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gridPayments = new TourWriter.UserControls.AccountingGrid();
            this.label3 = new System.Windows.Forms.Label();
            this.ultraTabPageControl22 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gridPurchases = new TourWriter.UserControls.AccountingGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPaymentTerms = new System.Windows.Forms.LinkLabel();
            this.label15 = new System.Windows.Forms.Label();
            this.txtPaymentTerms = new System.Windows.Forms.TextBox();
            this.ultraTabSharedControlsPage4 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.tabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.cmbItineraryStatus = new System.Windows.Forms.ComboBox();
            this.label38 = new System.Windows.Forms.Label();
            this.itineraryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.itineraryStatusBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnSaveLayout = new System.Windows.Forms.Button();
            this.ultraTabPageControl23.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.ultraTabPageControl1.SuspendLayout();
            this.ultraTabPageControl22.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryStatusBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl23
            // 
            this.ultraTabPageControl23.Controls.Add(this.label2);
            this.ultraTabPageControl23.Controls.Add(this.splitContainer1);
            this.ultraTabPageControl23.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl23.Name = "ultraTabPageControl23";
            this.ultraTabPageControl23.Size = new System.Drawing.Size(466, 218);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sales for this itinerary";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gridSales);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gridAllocations);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Size = new System.Drawing.Size(466, 218);
            this.splitContainer1.SplitterDistance = 98;
            this.splitContainer1.TabIndex = 5;
            // 
            // gridSales
            // 
            this.gridSales.DataSource = null;
            this.gridSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSales.EnableSelectBox = true;
            this.gridSales.EnableSetStatus = true;
            this.gridSales.ExportFileName = "";
            this.gridSales.GridEnabled = true;
            this.gridSales.Location = new System.Drawing.Point(0, 0);
            this.gridSales.Name = "gridSales";
            this.gridSales.Size = new System.Drawing.Size(466, 98);
            this.gridSales.TabIndex = 0;
            // 
            // gridAllocations
            // 
            this.gridAllocations.DataSource = null;
            this.gridAllocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAllocations.EnableSelectBox = false;
            this.gridAllocations.EnableSetStatus = false;
            this.gridAllocations.ExportFileName = "";
            this.gridAllocations.GridEnabled = true;
            this.gridAllocations.Location = new System.Drawing.Point(0, 0);
            this.gridAllocations.Name = "gridAllocations";
            this.gridAllocations.Size = new System.Drawing.Size(466, 116);
            this.gridAllocations.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(165, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Sales allocations for this itinerary";
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.gridPayments);
            this.ultraTabPageControl1.Controls.Add(this.label3);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(466, 218);
            // 
            // gridPayments
            // 
            this.gridPayments.DataSource = null;
            this.gridPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPayments.EnableSelectBox = true;
            this.gridPayments.EnableSetStatus = false;
            this.gridPayments.ExportFileName = "";
            this.gridPayments.GridEnabled = true;
            this.gridPayments.Location = new System.Drawing.Point(0, 0);
            this.gridPayments.Name = "gridPayments";
            this.gridPayments.Size = new System.Drawing.Size(466, 218);
            this.gridPayments.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Received payments for this itinerary";
            // 
            // ultraTabPageControl22
            // 
            this.ultraTabPageControl22.Controls.Add(this.gridPurchases);
            this.ultraTabPageControl22.Controls.Add(this.label1);
            this.ultraTabPageControl22.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl22.Name = "ultraTabPageControl22";
            this.ultraTabPageControl22.Size = new System.Drawing.Size(466, 218);
            // 
            // gridPurchases
            // 
            this.gridPurchases.DataSource = null;
            this.gridPurchases.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPurchases.EnableSelectBox = true;
            this.gridPurchases.EnableSetStatus = true;
            this.gridPurchases.ExportFileName = "";
            this.gridPurchases.GridEnabled = true;
            this.gridPurchases.Location = new System.Drawing.Point(0, 0);
            this.gridPurchases.Name = "gridPurchases";
            this.gridPurchases.Size = new System.Drawing.Size(466, 218);
            this.gridPurchases.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Purchase payments due for this itinerary";
            // 
            // btnPaymentTerms
            // 
            this.btnPaymentTerms.AutoSize = true;
            this.btnPaymentTerms.BackColor = System.Drawing.Color.Red;
            this.btnPaymentTerms.Location = new System.Drawing.Point(242, 24);
            this.btnPaymentTerms.Name = "btnPaymentTerms";
            this.btnPaymentTerms.Size = new System.Drawing.Size(30, 13);
            this.btnPaymentTerms.TabIndex = 177;
            this.btnPaymentTerms.TabStop = true;
            this.btnPaymentTerms.Text = "(edit)";
            this.btnPaymentTerms.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPaymentTerms.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Red;
            this.label15.Location = new System.Drawing.Point(242, 10);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(99, 13);
            this.label15.TabIndex = 178;
            this.label15.Text = "Sale payment terms";
            this.label15.Visible = false;
            // 
            // txtPaymentTerms
            // 
            this.txtPaymentTerms.BackColor = System.Drawing.Color.Red;
            this.txtPaymentTerms.Location = new System.Drawing.Point(233, 8);
            this.txtPaymentTerms.Multiline = true;
            this.txtPaymentTerms.Name = "txtPaymentTerms";
            this.txtPaymentTerms.ReadOnly = true;
            this.txtPaymentTerms.Size = new System.Drawing.Size(108, 29);
            this.txtPaymentTerms.TabIndex = 176;
            this.txtPaymentTerms.Visible = false;
            // 
            // ultraTabSharedControlsPage4
            // 
            this.ultraTabSharedControlsPage4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage4.Name = "ultraTabSharedControlsPage4";
            this.ultraTabSharedControlsPage4.Size = new System.Drawing.Size(466, 218);
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
            this.tabControl.Controls.Add(this.ultraTabPageControl1);
            this.tabControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(8, 50);
            this.tabControl.MinTabWidth = 70;
            this.tabControl.Name = "tabControl";
            this.tabControl.SharedControlsPage = this.ultraTabSharedControlsPage4;
            this.tabControl.Size = new System.Drawing.Size(470, 244);
            this.tabControl.TabIndex = 6;
            ultraTab1.Key = "Sales";
            ultraTab1.TabPage = this.ultraTabPageControl23;
            ultraTab1.Text = "Sales";
            ultraTab2.TabPage = this.ultraTabPageControl1;
            ultraTab2.Key = "Receipts";
            ultraTab2.Tag = "Receipts";
            ultraTab2.Text = "Receipts";
            ultraTab3.Key = "Purchases";
            ultraTab3.TabPage = this.ultraTabPageControl22;
            ultraTab3.Text = "Purchases";
            this.tabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3});
            // 
            // btnTransfer
            // 
            this.btnTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransfer.Location = new System.Drawing.Point(319, 43);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(75, 23);
            this.btnTransfer.TabIndex = 184;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // cmbItineraryStatus
            // 
            this.cmbItineraryStatus.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbItineraryStatus.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbItineraryStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItineraryStatus.Enabled = false;
            this.cmbItineraryStatus.FormattingEnabled = true;
            this.cmbItineraryStatus.Location = new System.Drawing.Point(94, 13);
            this.cmbItineraryStatus.Name = "cmbItineraryStatus";
            this.cmbItineraryStatus.Size = new System.Drawing.Size(126, 21);
            this.cmbItineraryStatus.TabIndex = 187;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.BackColor = System.Drawing.Color.Transparent;
            this.label38.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(6, 16);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(82, 13);
            this.label38.TabIndex = 186;
            this.label38.Text = "Itinerary status";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // btnSaveLayout
            // 
            this.btnSaveLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveLayout.Location = new System.Drawing.Point(400, 43);
            this.btnSaveLayout.Name = "btnSaveLayout";
            this.btnSaveLayout.Size = new System.Drawing.Size(75, 23);
            this.btnSaveLayout.TabIndex = 188;
            this.btnSaveLayout.Text = "Save Layout";
            this.btnSaveLayout.UseVisualStyleBackColor = true;
            this.btnSaveLayout.Click += new System.EventHandler(this.btnSaveLayout_Click);
            // 
            // Accounting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSaveLayout);
            this.Controls.Add(this.btnPaymentTerms);
            this.Controls.Add(this.cmbItineraryStatus);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.txtPaymentTerms);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.tabControl);
            this.Name = "Accounting";
            this.Size = new System.Drawing.Size(484, 317);
            this.Load += new System.EventHandler(this.Accounting_Load);
            this.ultraTabPageControl23.ResumeLayout(false);
            this.ultraTabPageControl23.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            this.ultraTabPageControl22.ResumeLayout(false);
            this.ultraTabPageControl22.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.itineraryBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itineraryStatusBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl23;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl22;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage4;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabControl;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.LinkLabel btnPaymentTerms;
        private System.Windows.Forms.TextBox txtPaymentTerms;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.ComboBox cmbItineraryStatus;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.BindingSource itineraryBindingSource;
        private System.Windows.Forms.BindingSource itineraryStatusBindingSource;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSaveLayout;
        private TourWriter.UserControls.AccountingGrid gridSales;
        private TourWriter.UserControls.AccountingGrid gridAllocations;
        private TourWriter.UserControls.AccountingGrid gridPayments;
        private TourWriter.UserControls.AccountingGrid gridPurchases;

    }
}
