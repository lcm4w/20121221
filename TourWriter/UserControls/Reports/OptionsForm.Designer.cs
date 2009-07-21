namespace TourWriter.UserControls.Reports
{
    partial class OptionsForm
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
            this.txtTopMargin = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtLeftMargin = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtBottomMargin = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtRightMargin = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.txtSpacing = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSpacing2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblSpacing1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtTopMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLeftMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBottomMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRightMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpacing)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTopMargin
            // 
            this.txtTopMargin.FormatString = "###0.00 cm";
            this.txtTopMargin.Location = new System.Drawing.Point(91, 32);
            this.txtTopMargin.Name = "txtTopMargin";
            this.txtTopMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtTopMargin.Size = new System.Drawing.Size(54, 21);
            this.txtTopMargin.TabIndex = 0;
            // 
            // txtLeftMargin
            // 
            this.txtLeftMargin.FormatString = "###0.00 cm";
            this.txtLeftMargin.Location = new System.Drawing.Point(182, 30);
            this.txtLeftMargin.Name = "txtLeftMargin";
            this.txtLeftMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtLeftMargin.Size = new System.Drawing.Size(54, 21);
            this.txtLeftMargin.TabIndex = 1;
            // 
            // txtBottomMargin
            // 
            this.txtBottomMargin.FormatString = "###0.00 cm";
            this.txtBottomMargin.Location = new System.Drawing.Point(91, 54);
            this.txtBottomMargin.Name = "txtBottomMargin";
            this.txtBottomMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtBottomMargin.Size = new System.Drawing.Size(54, 21);
            this.txtBottomMargin.TabIndex = 2;
            // 
            // txtRightMargin
            // 
            this.txtRightMargin.FormatString = "###0.00 cm";
            this.txtRightMargin.Location = new System.Drawing.Point(182, 52);
            this.txtRightMargin.Name = "txtRightMargin";
            this.txtRightMargin.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtRightMargin.Size = new System.Drawing.Size(54, 21);
            this.txtRightMargin.TabIndex = 3;
            // 
            // txtSpacing
            // 
            this.txtSpacing.FormatString = "###0.00 cm";
            this.txtSpacing.Location = new System.Drawing.Point(290, 30);
            this.txtSpacing.Name = "txtSpacing";
            this.txtSpacing.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Double;
            this.txtSpacing.Size = new System.Drawing.Size(50, 21);
            this.txtSpacing.TabIndex = 4;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(518, 308);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "Run";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(599, 308);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Top";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Bottom";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(151, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Left";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(151, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Right";
            // 
            // lblSpacing2
            // 
            this.lblSpacing2.AutoSize = true;
            this.lblSpacing2.Enabled = false;
            this.lblSpacing2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblSpacing2.Location = new System.Drawing.Point(340, 35);
            this.lblSpacing2.Name = "lblSpacing2";
            this.lblSpacing2.Size = new System.Drawing.Size(184, 13);
            this.lblSpacing2.TabIndex = 11;
            this.lblSpacing2.Text = "(between reoccurring subreport items)";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(679, 302);
            this.splitContainer1.SplitterDistance = 210;
            this.splitContainer1.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.pnlLayout);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(679, 210);
            this.panel1.TabIndex = 15;
            // 
            // pnlLayout
            // 
            this.pnlLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlLayout.AutoScroll = true;
            this.pnlLayout.Location = new System.Drawing.Point(3, 21);
            this.pnlLayout.Name = "pnlLayout";
            this.pnlLayout.Size = new System.Drawing.Size(671, 187);
            this.pnlLayout.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 2);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 16);
            this.label8.TabIndex = 13;
            this.label8.Text = "Options";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label9.Location = new System.Drawing.Point(70, 4);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(395, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Use these options to select and filter the data.  These are not saved for future " +
                "use.";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.lblSpacing1);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.txtBottomMargin);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtSpacing);
            this.panel2.Controls.Add(this.txtTopMargin);
            this.panel2.Controls.Add(this.txtRightMargin);
            this.panel2.Controls.Add(this.lblSpacing2);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtLeftMargin);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(679, 88);
            this.panel2.TabIndex = 15;
            // 
            // lblSpacing1
            // 
            this.lblSpacing1.AutoSize = true;
            this.lblSpacing1.Location = new System.Drawing.Point(242, 35);
            this.lblSpacing1.Name = "lblSpacing1";
            this.lblSpacing1.Size = new System.Drawing.Size(46, 13);
            this.lblSpacing1.TabIndex = 16;
            this.lblSpacing1.Text = "Spacing";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "Layout";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Margins";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(70, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(379, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Use these settings to adjust the layout. These settings are saved for future use." +
                "";
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(677, 336);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report margins";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtTopMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLeftMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBottomMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRightMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpacing)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtTopMargin;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtLeftMargin;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtBottomMargin;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtRightMargin;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor txtSpacing;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSpacing2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.FlowLayoutPanel pnlLayout;
        private System.Windows.Forms.Label lblSpacing1;
    }
}