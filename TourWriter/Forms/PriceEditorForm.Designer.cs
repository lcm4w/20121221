namespace TourWriter.Dialogs
{
    partial class PriceEditorForm
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
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            this.txtMarkup = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtNet = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtGross = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtCommission = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label46 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pnlRounding = new System.Windows.Forms.Panel();
            this.chkRoundFive = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkRoundOne = new System.Windows.Forms.CheckBox();
            this.chkRoundTen = new System.Windows.Forms.CheckBox();
            this.lblCurrencyInfo = new System.Windows.Forms.Label();
            this.grpOption = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtMarkup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGross)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCommission)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.pnlRounding.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpOption)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMarkup
            // 
            this.txtMarkup.Enabled = false;
            this.txtMarkup.Location = new System.Drawing.Point(82, 56);
            this.txtMarkup.Name = "txtMarkup";
            this.txtMarkup.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtMarkup.PromptChar = ' ';
            this.txtMarkup.Size = new System.Drawing.Size(92, 21);
            this.txtMarkup.TabIndex = 3;
            this.txtMarkup.AfterExitEditMode += new System.EventHandler(this.editor_AfterExitEditMode);
            this.txtMarkup.Enter += new System.EventHandler(this.editor_Enter);
            // 
            // txtNet
            // 
            this.txtNet.Enabled = false;
            this.txtNet.Location = new System.Drawing.Point(82, 32);
            this.txtNet.Name = "txtNet";
            this.txtNet.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtNet.PromptChar = ' ';
            this.txtNet.Size = new System.Drawing.Size(92, 21);
            this.txtNet.TabIndex = 2;
            this.txtNet.AfterExitEditMode += new System.EventHandler(this.editor_AfterExitEditMode);
            this.txtNet.Enter += new System.EventHandler(this.editor_Enter);
            // 
            // txtGross
            // 
            this.txtGross.Enabled = false;
            this.txtGross.Location = new System.Drawing.Point(82, 80);
            this.txtGross.Name = "txtGross";
            this.txtGross.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtGross.PromptChar = ' ';
            this.txtGross.Size = new System.Drawing.Size(92, 21);
            this.txtGross.TabIndex = 4;
            this.txtGross.AfterExitEditMode += new System.EventHandler(this.editor_AfterExitEditMode);
            this.txtGross.Enter += new System.EventHandler(this.editor_Enter);
            // 
            // txtCommission
            // 
            this.txtCommission.Enabled = false;
            this.txtCommission.Location = new System.Drawing.Point(82, 104);
            this.txtCommission.Name = "txtCommission";
            this.txtCommission.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtCommission.PromptChar = ' ';
            this.txtCommission.Size = new System.Drawing.Size(92, 21);
            this.txtCommission.TabIndex = 5;
            this.txtCommission.AfterExitEditMode += new System.EventHandler(this.editor_AfterExitEditMode);
            this.txtCommission.Enter += new System.EventHandler(this.editor_Enter);
            // 
            // label46
            // 
            this.label46.Location = new System.Drawing.Point(14, 56);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(68, 23);
            this.label46.TabIndex = 127;
            this.label46.Text = "Markup";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label43
            // 
            this.label43.Location = new System.Drawing.Point(14, 104);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(68, 23);
            this.label43.TabIndex = 125;
            this.label43.Text = "Commission";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label44
            // 
            this.label44.Location = new System.Drawing.Point(14, 80);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(68, 23);
            this.label44.TabIndex = 126;
            this.label44.Text = "Gross";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(14, 32);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(68, 23);
            this.label21.TabIndex = 124;
            this.label21.Text = "Net";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(255, 192);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(336, 192);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.label3);
            this.ultraGroupBox1.Controls.Add(this.label2);
            this.ultraGroupBox1.Controls.Add(this.pnlRounding);
            this.ultraGroupBox1.Controls.Add(this.lblCurrencyInfo);
            this.ultraGroupBox1.Controls.Add(this.txtMarkup);
            this.ultraGroupBox1.Controls.Add(this.txtNet);
            this.ultraGroupBox1.Controls.Add(this.txtGross);
            this.ultraGroupBox1.Controls.Add(this.txtCommission);
            this.ultraGroupBox1.Controls.Add(this.label46);
            this.ultraGroupBox1.Controls.Add(this.label43);
            this.ultraGroupBox1.Controls.Add(this.label44);
            this.ultraGroupBox1.Controls.Add(this.label21);
            this.ultraGroupBox1.Location = new System.Drawing.Point(211, 14);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(200, 163);
            this.ultraGroupBox1.TabIndex = 2;
            this.ultraGroupBox1.Text = "Edit price";
            // 
            // pnlRounding
            // 
            this.pnlRounding.Controls.Add(this.chkRoundFive);
            this.pnlRounding.Controls.Add(this.label1);
            this.pnlRounding.Controls.Add(this.chkRoundOne);
            this.pnlRounding.Controls.Add(this.chkRoundTen);
            this.pnlRounding.Enabled = false;
            this.pnlRounding.Location = new System.Drawing.Point(6, 130);
            this.pnlRounding.Name = "pnlRounding";
            this.pnlRounding.Size = new System.Drawing.Size(188, 31);
            this.pnlRounding.TabIndex = 129;
            // 
            // chkRoundFive
            // 
            this.chkRoundFive.AutoSize = true;
            this.chkRoundFive.Location = new System.Drawing.Point(107, 8);
            this.chkRoundFive.Name = "chkRoundFive";
            this.chkRoundFive.Size = new System.Drawing.Size(32, 17);
            this.chkRoundFive.TabIndex = 1;
            this.chkRoundFive.Text = "5";
            this.toolTip1.SetToolTip(this.chkRoundFive, "Round gross price to nearest round figure");
            this.chkRoundFive.UseVisualStyleBackColor = true;
            this.chkRoundFive.CheckedChanged += new System.EventHandler(this.chkRoundFive_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 17);
            this.label1.TabIndex = 126;
            this.label1.Text = "Rounding";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.label1, "Round gross price to nearest round figure");
            // 
            // chkRoundOne
            // 
            this.chkRoundOne.AutoSize = true;
            this.chkRoundOne.Location = new System.Drawing.Point(67, 8);
            this.chkRoundOne.Name = "chkRoundOne";
            this.chkRoundOne.Size = new System.Drawing.Size(32, 17);
            this.chkRoundOne.TabIndex = 0;
            this.chkRoundOne.Text = "1";
            this.toolTip1.SetToolTip(this.chkRoundOne, "Round gross price to nearest round figure");
            this.chkRoundOne.UseVisualStyleBackColor = true;
            this.chkRoundOne.CheckedChanged += new System.EventHandler(this.chkRoundOne_CheckedChanged);
            // 
            // chkRoundTen
            // 
            this.chkRoundTen.AutoSize = true;
            this.chkRoundTen.Location = new System.Drawing.Point(147, 8);
            this.chkRoundTen.Name = "chkRoundTen";
            this.chkRoundTen.Size = new System.Drawing.Size(38, 17);
            this.chkRoundTen.TabIndex = 2;
            this.chkRoundTen.Text = "10";
            this.toolTip1.SetToolTip(this.chkRoundTen, "Round gross price to nearest round figure");
            this.chkRoundTen.UseVisualStyleBackColor = true;
            this.chkRoundTen.CheckedChanged += new System.EventHandler(this.chkRoundTen_CheckedChanged);
            // 
            // lblCurrencyInfo
            // 
            this.lblCurrencyInfo.AutoSize = true;
            this.lblCurrencyInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrencyInfo.ForeColor = System.Drawing.Color.Blue;
            this.lblCurrencyInfo.Location = new System.Drawing.Point(99, 14);
            this.lblCurrencyInfo.Name = "lblCurrencyInfo";
            this.lblCurrencyInfo.Size = new System.Drawing.Size(78, 13);
            this.lblCurrencyInfo.TabIndex = 128;
            this.lblCurrencyInfo.Text = "Currency: NZD";
            this.lblCurrencyInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpOption
            // 
            this.grpOption.BackColor = System.Drawing.Color.Transparent;
            this.grpOption.BackColorInternal = System.Drawing.Color.Transparent;
            this.grpOption.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            valueListItem1.DataValue = "";
            valueListItem1.DisplayText = "<Net and Markup>";
            valueListItem2.DataValue = "";
            valueListItem2.DisplayText = "<Net and Gross>";
            valueListItem3.DataValue = "";
            valueListItem3.DisplayText = "<Gross and Commission>";
            this.grpOption.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            this.grpOption.ItemSpacingVertical = 10;
            this.grpOption.Location = new System.Drawing.Point(16, 32);
            this.grpOption.Name = "grpOption";
            this.grpOption.Size = new System.Drawing.Size(162, 94);
            this.grpOption.TabIndex = 1;
            this.grpOption.ValueChanged += new System.EventHandler(this.grpOption_ValueChanged);
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.grpOption);
            this.ultraGroupBox2.Location = new System.Drawing.Point(15, 14);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(190, 163);
            this.ultraGroupBox2.TabIndex = 1;
            this.ultraGroupBox2.Text = "Choose editing option";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Controls.Add(this.ultraGroupBox1);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.ultraGroupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(420, 223);
            this.panel2.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 130;
            this.label2.Text = "%";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(176, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 13);
            this.label3.TabIndex = 131;
            this.label3.Text = "%";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PriceEditorForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(420, 223);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PriceEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TourWriter Price Editor";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.CostingDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtMarkup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGross)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCommission)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            this.pnlRounding.ResumeLayout(false);
            this.pnlRounding.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpOption)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtMarkup;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtGross;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtCommission;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label21;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtNet;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinEditors.UltraOptionSet grpOption;
        private System.Windows.Forms.Label lblCurrencyInfo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnlRounding;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkRoundOne;
        private System.Windows.Forms.CheckBox chkRoundTen;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkRoundFive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}
