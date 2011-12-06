namespace TourWriter.Forms
{
    partial class CurrencyRateEditor
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCurrencyTo = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.cmbCurrencyFrom = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtRate = new Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.txtDateTo = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyFrom)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(263, 199);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Currencies from";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(36, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Dates from";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(216, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "to";
            // 
            // cmbCurrencyTo
            // 
            this.cmbCurrencyTo.CheckedListSettings.CheckStateMember = "";
            this.cmbCurrencyTo.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.cmbCurrencyTo.Location = new System.Drawing.Point(233, 58);
            this.cmbCurrencyTo.Name = "cmbCurrencyTo";
            this.cmbCurrencyTo.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.cmbCurrencyTo.Size = new System.Drawing.Size(93, 22);
            this.cmbCurrencyTo.TabIndex = 2;
            this.cmbCurrencyTo.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.cmbCurrencyTo_InitializeLayout);
            // 
            // cmbCurrencyFrom
            // 
            this.cmbCurrencyFrom.CheckedListSettings.CheckStateMember = "";
            this.cmbCurrencyFrom.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            this.cmbCurrencyFrom.Location = new System.Drawing.Point(109, 58);
            this.cmbCurrencyFrom.Name = "cmbCurrencyFrom";
            this.cmbCurrencyFrom.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.cmbCurrencyFrom.Size = new System.Drawing.Size(93, 22);
            this.cmbCurrencyFrom.TabIndex = 1;
            this.cmbCurrencyFrom.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.cmbCurrencyFrom_InitializeLayout);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(216, 107);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "to";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(326, 28);
            this.label2.TabIndex = 26;
            this.label2.Text = "Enter forward currency rates, to allow forward bookings to be made in other curre" +
    "ncies";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(182, 199);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtRate
            // 
            appearance2.TextHAlignAsString = "Right";
            this.txtRate.Appearance = appearance2;
            this.txtRate.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.UseSpecifiedMask;
            this.txtRate.InputMask = "nnn.nnnn";
            this.txtRate.Location = new System.Drawing.Point(109, 145);
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(93, 20);
            this.txtRate.TabIndex = 5;
            this.txtRate.Text = "txtRate";
            this.toolTip1.SetToolTip(this.txtRate, "Forward rate");
            // 
            // txtDateFrom
            // 
            this.txtDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtDateFrom.Location = new System.Drawing.Point(109, 103);
            this.txtDateFrom.Name = "txtDateFrom";
            this.txtDateFrom.Size = new System.Drawing.Size(93, 20);
            this.txtDateFrom.TabIndex = 29;
            // 
            // txtDateTo
            // 
            this.txtDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.txtDateTo.Location = new System.Drawing.Point(233, 103);
            this.txtDateTo.Name = "txtDateTo";
            this.txtDateTo.Size = new System.Drawing.Size(93, 20);
            this.txtDateTo.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Exchange Rate";
            // 
            // CurrencyRateEditor
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(344, 232);
            this.Controls.Add(this.txtDateTo);
            this.Controls.Add(this.txtDateFrom);
            this.Controls.Add(this.txtRate);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbCurrencyTo);
            this.Controls.Add(this.cmbCurrencyFrom);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CurrencyRateEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TourWriter Currency Rates";
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrencyFrom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbCurrencyTo;
        private Infragistics.Win.UltraWinGrid.UltraCombo cmbCurrencyFrom;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOk;
        private Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit txtRate;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DateTimePicker txtDateFrom;
        private System.Windows.Forms.DateTimePicker txtDateTo;
        private System.Windows.Forms.Label label1;
    }
}