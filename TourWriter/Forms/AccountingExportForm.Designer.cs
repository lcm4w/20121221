namespace TourWriter.Forms
{
    partial class AccountingExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountingExportForm));
            this.chkPurchases = new System.Windows.Forms.CheckBox();
            this.chkSuppliers = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtPurchases = new System.Windows.Forms.TextBox();
            this.txtSuppliers = new System.Windows.Forms.TextBox();
            this.txtClients = new System.Windows.Forms.TextBox();
            this.chkSales = new System.Windows.Forms.CheckBox();
            this.txtSales = new System.Windows.Forms.TextBox();
            this.chkClients = new System.Windows.Forms.CheckBox();
            this.chkPayments = new System.Windows.Forms.CheckBox();
            this.txtPayments = new System.Windows.Forms.TextBox();
            this.grpPurchases = new System.Windows.Forms.GroupBox();
            this.grpSales = new System.Windows.Forms.GroupBox();
            this.grpPurchases.SuspendLayout();
            this.grpSales.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkPurchases
            // 
            this.chkPurchases.AutoSize = true;
            this.chkPurchases.Location = new System.Drawing.Point(6, 19);
            this.chkPurchases.Name = "chkPurchases";
            this.chkPurchases.Size = new System.Drawing.Size(221, 17);
            this.chkPurchases.TabIndex = 0;
            this.chkPurchases.Text = "Purchases - exports all the purchase data";
            this.chkPurchases.UseVisualStyleBackColor = true;
            this.chkPurchases.CheckedChanged += new System.EventHandler(this.chkPurchases_CheckedChanged);
            // 
            // chkSuppliers
            // 
            this.chkSuppliers.AutoSize = true;
            this.chkSuppliers.Location = new System.Drawing.Point(6, 68);
            this.chkSuppliers.Name = "chkSuppliers";
            this.chkSuppliers.Size = new System.Drawing.Size(228, 17);
            this.chkSuppliers.TabIndex = 1;
            this.chkSuppliers.Text = "Suppliers - exports the related supplier data";
            this.chkSuppliers.UseVisualStyleBackColor = true;
            this.chkSuppliers.CheckedChanged += new System.EventHandler(this.chkSuppliers_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(254, 325);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(173, 325);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtPurchases
            // 
            this.txtPurchases.Enabled = false;
            this.txtPurchases.Location = new System.Drawing.Point(23, 42);
            this.txtPurchases.Name = "txtPurchases";
            this.txtPurchases.Size = new System.Drawing.Size(259, 20);
            this.txtPurchases.TabIndex = 7;
            // 
            // txtSuppliers
            // 
            this.txtSuppliers.Enabled = false;
            this.txtSuppliers.Location = new System.Drawing.Point(23, 91);
            this.txtSuppliers.Name = "txtSuppliers";
            this.txtSuppliers.Size = new System.Drawing.Size(259, 20);
            this.txtSuppliers.TabIndex = 8;
            // 
            // txtClients
            // 
            this.txtClients.Enabled = false;
            this.txtClients.Location = new System.Drawing.Point(23, 140);
            this.txtClients.Name = "txtClients";
            this.txtClients.Size = new System.Drawing.Size(259, 20);
            this.txtClients.TabIndex = 8;
            // 
            // chkSales
            // 
            this.chkSales.AutoSize = true;
            this.chkSales.Location = new System.Drawing.Point(6, 19);
            this.chkSales.Name = "chkSales";
            this.chkSales.Size = new System.Drawing.Size(172, 17);
            this.chkSales.TabIndex = 0;
            this.chkSales.Text = "Sales - exports all the sale data";
            this.chkSales.UseVisualStyleBackColor = true;
            this.chkSales.CheckedChanged += new System.EventHandler(this.chkSales_CheckedChanged);
            // 
            // txtSales
            // 
            this.txtSales.Enabled = false;
            this.txtSales.Location = new System.Drawing.Point(23, 42);
            this.txtSales.Name = "txtSales";
            this.txtSales.Size = new System.Drawing.Size(259, 20);
            this.txtSales.TabIndex = 7;
            // 
            // chkClients
            // 
            this.chkClients.AutoSize = true;
            this.chkClients.Location = new System.Drawing.Point(6, 117);
            this.chkClients.Name = "chkClients";
            this.chkClients.Size = new System.Drawing.Size(205, 17);
            this.chkClients.TabIndex = 1;
            this.chkClients.Text = "Clients - exports the related client data";
            this.chkClients.UseVisualStyleBackColor = true;
            this.chkClients.CheckedChanged += new System.EventHandler(this.chkClients_CheckedChanged);
            // 
            // chkPayments
            // 
            this.chkPayments.AutoSize = true;
            this.chkPayments.Location = new System.Drawing.Point(6, 68);
            this.chkPayments.Name = "chkPayments";
            this.chkPayments.Size = new System.Drawing.Size(262, 17);
            this.chkPayments.TabIndex = 0;
            this.chkPayments.Text = "Payments - exports all the received payments data";
            this.chkPayments.UseVisualStyleBackColor = true;
            this.chkPayments.CheckedChanged += new System.EventHandler(this.chkPayments_CheckedChanged);
            // 
            // txtPayments
            // 
            this.txtPayments.Enabled = false;
            this.txtPayments.Location = new System.Drawing.Point(23, 91);
            this.txtPayments.Name = "txtPayments";
            this.txtPayments.Size = new System.Drawing.Size(259, 20);
            this.txtPayments.TabIndex = 7;
            // 
            // grpPurchases
            // 
            this.grpPurchases.Controls.Add(this.txtSuppliers);
            this.grpPurchases.Controls.Add(this.chkSuppliers);
            this.grpPurchases.Controls.Add(this.txtPurchases);
            this.grpPurchases.Controls.Add(this.chkPurchases);
            this.grpPurchases.Location = new System.Drawing.Point(11, 12);
            this.grpPurchases.Name = "grpPurchases";
            this.grpPurchases.Size = new System.Drawing.Size(318, 122);
            this.grpPurchases.TabIndex = 13;
            this.grpPurchases.TabStop = false;
            this.grpPurchases.Text = "Purchases";
            // 
            // grpSales
            // 
            this.grpSales.Controls.Add(this.txtClients);
            this.grpSales.Controls.Add(this.chkClients);
            this.grpSales.Controls.Add(this.chkPayments);
            this.grpSales.Controls.Add(this.txtSales);
            this.grpSales.Controls.Add(this.chkSales);
            this.grpSales.Controls.Add(this.txtPayments);
            this.grpSales.Location = new System.Drawing.Point(11, 140);
            this.grpSales.Name = "grpSales";
            this.grpSales.Size = new System.Drawing.Size(318, 172);
            this.grpSales.TabIndex = 14;
            this.grpSales.TabStop = false;
            this.grpSales.Text = "Sales";
            // 
            // AccountingExportForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(341, 360);
            this.Controls.Add(this.grpSales);
            this.Controls.Add(this.grpPurchases);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountingExportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TourWriter Accounting Export";
            this.grpPurchases.ResumeLayout(false);
            this.grpPurchases.PerformLayout();
            this.grpSales.ResumeLayout(false);
            this.grpSales.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkPurchases;
        private System.Windows.Forms.CheckBox chkSuppliers;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtPurchases;
        private System.Windows.Forms.TextBox txtSuppliers;
        private System.Windows.Forms.TextBox txtClients;
        private System.Windows.Forms.CheckBox chkSales;
        private System.Windows.Forms.TextBox txtSales;
        private System.Windows.Forms.CheckBox chkClients;
        private System.Windows.Forms.CheckBox chkPayments;
        private System.Windows.Forms.TextBox txtPayments;
        private System.Windows.Forms.GroupBox grpPurchases;
        private System.Windows.Forms.GroupBox grpSales;
    }
}