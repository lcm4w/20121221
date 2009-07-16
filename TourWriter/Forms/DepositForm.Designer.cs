namespace TourWriter.Forms
{
    partial class DepositForm
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
            this.txtPaymentTermDays = new System.Windows.Forms.TextBox();
            this.cmbPaymentTerms = new TourWriter.UserControls.NullableComboBox();
            this.paymentTermsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbDepositType = new TourWriter.UserControls.NullableComboBox();
            this.txtDepositAmount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.paymentTermsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepositAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // txtPaymentTermDays
            // 
            this.txtPaymentTermDays.Location = new System.Drawing.Point(111, 104);
            this.txtPaymentTermDays.Name = "txtPaymentTermDays";
            this.txtPaymentTermDays.Size = new System.Drawing.Size(82, 20);
            this.txtPaymentTermDays.TabIndex = 0;
            // 
            // cmbPaymentTerms
            // 
            this.cmbPaymentTerms.DataSource = this.paymentTermsBindingSource;
            this.cmbPaymentTerms.DisplayMember = "PaymentTermName";
            this.cmbPaymentTerms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentTerms.FormattingEnabled = true;
            this.cmbPaymentTerms.Location = new System.Drawing.Point(255, 104);
            this.cmbPaymentTerms.Name = "cmbPaymentTerms";
            this.cmbPaymentTerms.Size = new System.Drawing.Size(140, 21);
            this.cmbPaymentTerms.TabIndex = 1;
            this.cmbPaymentTerms.ValueMember = "PaymentTermID";
            this.cmbPaymentTerms.SelectedIndexChanged += new System.EventHandler(this.cmbPaymentTerms_SelectedIndexChanged);
            // 
            // paymentTermsBindingSource
            // 
            this.paymentTermsBindingSource.DataMember = "PaymentTerm";
            this.paymentTermsBindingSource.DataSource = typeof(TourWriter.Info.ToolSet);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Days";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Terms";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(247, 155);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(328, 155);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbDepositType
            // 
            this.cmbDepositType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepositType.FormattingEnabled = true;
            this.cmbDepositType.Location = new System.Drawing.Point(255, 37);
            this.cmbDepositType.Name = "cmbDepositType";
            this.cmbDepositType.Size = new System.Drawing.Size(140, 21);
            this.cmbDepositType.TabIndex = 134;
            this.cmbDepositType.SelectedIndexChanged += new System.EventHandler(this.cmbDepositType_SelectedIndexChanged);
            // 
            // txtDepositAmount
            // 
            this.txtDepositAmount.FormatString = "";
            this.txtDepositAmount.Location = new System.Drawing.Point(111, 37);
            this.txtDepositAmount.Name = "txtDepositAmount";
            this.txtDepositAmount.Nullable = true;
            this.txtDepositAmount.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtDepositAmount.Size = new System.Drawing.Size(82, 21);
            this.txtDepositAmount.TabIndex = 133;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(218, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 132;
            this.label4.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 131;
            this.label3.Text = "Amount";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Location = new System.Drawing.Point(12, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 135;
            this.label5.Text = "Deposit";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(12, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 136;
            this.label6.Text = "Payment terms";
            // 
            // DepositForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(415, 190);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbDepositType);
            this.Controls.Add(this.txtDepositAmount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbPaymentTerms);
            this.Controls.Add(this.txtPaymentTermDays);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DepositForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter deposit information";
            ((System.ComponentModel.ISupportInitialize)(this.paymentTermsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepositAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPaymentTermDays;
        private TourWriter.UserControls.NullableComboBox cmbPaymentTerms;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.BindingSource paymentTermsBindingSource;
        private TourWriter.UserControls.NullableComboBox cmbDepositType;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtDepositAmount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}