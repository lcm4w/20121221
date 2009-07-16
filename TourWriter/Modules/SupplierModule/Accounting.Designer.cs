namespace TourWriter.Modules.SupplierModule
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
            this.btnSaveLayout = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.gridPurchases = new TourWriter.UserControls.AccountingGrid();
            this.SuspendLayout();
            // 
            // btnSaveLayout
            // 
            this.btnSaveLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveLayout.Location = new System.Drawing.Point(390, 12);
            this.btnSaveLayout.Name = "btnSaveLayout";
            this.btnSaveLayout.Size = new System.Drawing.Size(75, 23);
            this.btnSaveLayout.TabIndex = 190;
            this.btnSaveLayout.Text = "Save Layout";
            this.btnSaveLayout.UseVisualStyleBackColor = true;
            this.btnSaveLayout.Click += new System.EventHandler(this.btnSaveLayout_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransfer.Location = new System.Drawing.Point(309, 12);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(75, 23);
            this.btnTransfer.TabIndex = 189;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // gridPurchases
            // 
            this.gridPurchases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPurchases.DataSource = null;
            this.gridPurchases.EnableSelectBox = true;
            this.gridPurchases.EnableSetStatus = true;
            this.gridPurchases.ExportFileName = "";
            this.gridPurchases.GridEnabled = true;
            this.gridPurchases.Location = new System.Drawing.Point(3, 41);
            this.gridPurchases.Name = "gridPurchases";
            this.gridPurchases.Size = new System.Drawing.Size(462, 219);
            this.gridPurchases.TabIndex = 191;
            // 
            // Accounting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPurchases);
            this.Controls.Add(this.btnSaveLayout);
            this.Controls.Add(this.btnTransfer);
            this.Name = "Accounting";
            this.Size = new System.Drawing.Size(468, 263);
            this.Load += new System.EventHandler(this.Accounting_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSaveLayout;
        private System.Windows.Forms.Button btnTransfer;
        private TourWriter.UserControls.AccountingGrid gridPurchases;
    }
}
