namespace TourWriter.Forms
{
    partial class PaymentTermsEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPaymentDuePeriod = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPaymentDue = new TourWriter.UserControls.NullableComboBox();
            this.paymentDueBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtDepositDuePeriod = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtDepositAmount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.cmbDepositType = new TourWriter.UserControls.NullableComboBox();
            this.chkDepositRequired = new System.Windows.Forms.CheckBox();
            this.lblTerms = new System.Windows.Forms.Label();
            this.lblAmount = new System.Windows.Forms.Label();
            this.cmbDepositDue = new TourWriter.UserControls.NullableComboBox();
            this.paymentDueBindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkRemoveTerms = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPaymentDuePeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paymentDueBindingSource)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepositDuePeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepositAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paymentDueBindingSource2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPaymentDuePeriod);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbPaymentDue);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Payment Terms";
            // 
            // txtPaymentDuePeriod
            // 
            this.txtPaymentDuePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPaymentDuePeriod.FormatString = "";
            this.txtPaymentDuePeriod.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtPaymentDuePeriod.Location = new System.Drawing.Point(63, 24);
            this.txtPaymentDuePeriod.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.txtPaymentDuePeriod.MaskInput = "nnnnnn";
            this.txtPaymentDuePeriod.Name = "txtPaymentDuePeriod";
            this.txtPaymentDuePeriod.Nullable = true;
            this.txtPaymentDuePeriod.Size = new System.Drawing.Size(71, 21);
            this.txtPaymentDuePeriod.TabIndex = 131;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Terms";
            // 
            // cmbPaymentDue
            // 
            this.cmbPaymentDue.DataSource = this.paymentDueBindingSource;
            this.cmbPaymentDue.DisplayMember = "PaymentDueName";
            this.cmbPaymentDue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentDue.FormattingEnabled = true;
            this.cmbPaymentDue.Location = new System.Drawing.Point(140, 25);
            this.cmbPaymentDue.Name = "cmbPaymentDue";
            this.cmbPaymentDue.Size = new System.Drawing.Size(150, 21);
            this.cmbPaymentDue.TabIndex = 1;
            this.cmbPaymentDue.ValueMember = "PaymentDueID";
            this.cmbPaymentDue.SelectedIndexChanged += new System.EventHandler(this.cmbPaymentDue_SelectedIndexChanged);
            // 
            // paymentDueBindingSource
            // 
            this.paymentDueBindingSource.DataMember = "PaymentDue";
            this.paymentDueBindingSource.DataSource = typeof(TourWriter.Info.ToolSet);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtDepositDuePeriod);
            this.groupBox2.Controls.Add(this.txtDepositAmount);
            this.groupBox2.Controls.Add(this.cmbDepositType);
            this.groupBox2.Controls.Add(this.chkDepositRequired);
            this.groupBox2.Controls.Add(this.lblTerms);
            this.groupBox2.Controls.Add(this.lblAmount);
            this.groupBox2.Controls.Add(this.cmbDepositDue);
            this.groupBox2.Location = new System.Drawing.Point(12, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(296, 111);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Deposit";
            // 
            // txtDepositDuePeriod
            // 
            this.txtDepositDuePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDepositDuePeriod.FormatString = "";
            this.txtDepositDuePeriod.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtDepositDuePeriod.Location = new System.Drawing.Point(63, 75);
            this.txtDepositDuePeriod.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.txtDepositDuePeriod.MaskInput = "nnnnnn";
            this.txtDepositDuePeriod.Name = "txtDepositDuePeriod";
            this.txtDepositDuePeriod.Nullable = true;
            this.txtDepositDuePeriod.Size = new System.Drawing.Size(71, 21);
            this.txtDepositDuePeriod.TabIndex = 130;
            // 
            // txtDepositAmount
            // 
            this.txtDepositAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDepositAmount.FormatString = "";
            this.txtDepositAmount.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtDepositAmount.Location = new System.Drawing.Point(63, 48);
            this.txtDepositAmount.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            this.txtDepositAmount.MaskInput = "nnnnnn.nn";
            this.txtDepositAmount.Name = "txtDepositAmount";
            this.txtDepositAmount.Nullable = true;
            this.txtDepositAmount.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtDepositAmount.Size = new System.Drawing.Size(71, 21);
            this.txtDepositAmount.TabIndex = 129;
            // 
            // cmbDepositType
            // 
            this.cmbDepositType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepositType.FormattingEnabled = true;
            this.cmbDepositType.Location = new System.Drawing.Point(140, 49);
            this.cmbDepositType.Name = "cmbDepositType";
            this.cmbDepositType.Size = new System.Drawing.Size(40, 21);
            this.cmbDepositType.TabIndex = 10;
            // 
            // chkDepositRequired
            // 
            this.chkDepositRequired.AutoSize = true;
            this.chkDepositRequired.Location = new System.Drawing.Point(17, 23);
            this.chkDepositRequired.Name = "chkDepositRequired";
            this.chkDepositRequired.Size = new System.Drawing.Size(127, 17);
            this.chkDepositRequired.TabIndex = 9;
            this.chkDepositRequired.Text = "Is a deposit required?";
            this.chkDepositRequired.UseVisualStyleBackColor = true;
            this.chkDepositRequired.CheckedChanged += new System.EventHandler(this.chkDepositRequired_CheckedChanged);
            // 
            // lblTerms
            // 
            this.lblTerms.AutoSize = true;
            this.lblTerms.Location = new System.Drawing.Point(14, 79);
            this.lblTerms.Name = "lblTerms";
            this.lblTerms.Size = new System.Drawing.Size(36, 13);
            this.lblTerms.TabIndex = 8;
            this.lblTerms.Text = "Terms";
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(14, 52);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(43, 13);
            this.lblAmount.TabIndex = 7;
            this.lblAmount.Text = "Amount";
            // 
            // cmbDepositDue
            // 
            this.cmbDepositDue.DataSource = this.paymentDueBindingSource2;
            this.cmbDepositDue.DisplayMember = "PaymentDueName";
            this.cmbDepositDue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDepositDue.FormattingEnabled = true;
            this.cmbDepositDue.Location = new System.Drawing.Point(140, 76);
            this.cmbDepositDue.Name = "cmbDepositDue";
            this.cmbDepositDue.Size = new System.Drawing.Size(150, 21);
            this.cmbDepositDue.TabIndex = 3;
            this.cmbDepositDue.ValueMember = "PaymentDueID";
            this.cmbDepositDue.SelectedIndexChanged += new System.EventHandler(this.cmbDepositDue_SelectedIndexChanged);
            // 
            // paymentDueBindingSource2
            // 
            this.paymentDueBindingSource2.DataMember = "PaymentDue";
            this.paymentDueBindingSource2.DataSource = typeof(TourWriter.Info.ToolSet);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(152, 224);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(233, 224);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkRemoveTerms
            // 
            this.chkRemoveTerms.AutoSize = true;
            this.chkRemoveTerms.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRemoveTerms.Location = new System.Drawing.Point(171, 194);
            this.chkRemoveTerms.Name = "chkRemoveTerms";
            this.chkRemoveTerms.Size = new System.Drawing.Size(137, 17);
            this.chkRemoveTerms.TabIndex = 10;
            this.chkRemoveTerms.Text = "Remove payment terms";
            this.chkRemoveTerms.UseVisualStyleBackColor = true;
            this.chkRemoveTerms.CheckedChanged += new System.EventHandler(this.chkRemoveTerms_CheckedChanged);
            // 
            // PaymentTermsEditor
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(320, 259);
            this.Controls.Add(this.chkRemoveTerms);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PaymentTermsEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Payment Terms Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPaymentDuePeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paymentDueBindingSource)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepositDuePeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepositAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paymentDueBindingSource2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private TourWriter.UserControls.NullableComboBox cmbPaymentDue;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private TourWriter.UserControls.NullableComboBox cmbDepositDue;
        private System.Windows.Forms.CheckBox chkDepositRequired;
        private System.Windows.Forms.Label lblTerms;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.Label label1;
        private TourWriter.UserControls.NullableComboBox cmbDepositType;
        private System.Windows.Forms.BindingSource paymentDueBindingSource;
        private System.Windows.Forms.BindingSource paymentDueBindingSource2;
        private System.Windows.Forms.CheckBox chkRemoveTerms;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtPaymentDuePeriod;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtDepositDuePeriod;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtDepositAmount;
    }
}