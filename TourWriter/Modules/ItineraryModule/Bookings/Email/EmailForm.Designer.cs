namespace TourWriter.Modules.ItineraryModule.Bookings.Email
{
    partial class EmailForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.btnNext = new Infragistics.Win.Misc.UltraButton();
            this.btnBack = new Infragistics.Win.Misc.UltraButton();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProv = new System.Windows.Forms.ErrorProvider(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAttach = new System.Windows.Forms.TextBox();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.webBody = new System.Windows.Forms.WebBrowser();
            this.btnBookAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnBookThis = new System.Windows.Forms.ToolStripMenuItem();
            this.myToolStrip1 = new TourWriter.UserControls.MyToolStrip();
            this.btnAttach = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAttachThis = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAttachAll = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.errorProv)).BeginInit();
            this.myToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Subject";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "To";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPosition
            // 
            this.lblPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPosition.Location = new System.Drawing.Point(220, 8);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(309, 13);
            this.lblPosition.TabIndex = 16;
            this.lblPosition.Text = "Showing email 1 of 1";
            this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(559, 3);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(24, 20);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = ">";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Location = new System.Drawing.Point(535, 3);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(24, 20);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "<";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(245, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "This screen allows you to view and edit the emails.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // errorProv
            // 
            this.errorProv.ContainerControl = this;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 430);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Attachment";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 0;
            // 
            // txtAttach
            // 
            this.txtAttach.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAttach.Location = new System.Drawing.Point(68, 427);
            this.txtAttach.Name = "txtAttach";
            this.txtAttach.ReadOnly = true;
            this.txtAttach.Size = new System.Drawing.Size(443, 20);
            this.txtAttach.TabIndex = 26;
            // 
            // txtSubject
            // 
            this.txtSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubject.Location = new System.Drawing.Point(52, 64);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(531, 20);
            this.txtSubject.TabIndex = 2;
            // 
            // txtTo
            // 
            this.txtTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTo.Location = new System.Drawing.Point(52, 40);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(531, 20);
            this.txtTo.TabIndex = 1;
            // 
            // webBody
            // 
            this.webBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBody.Location = new System.Drawing.Point(3, 90);
            this.webBody.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBody.Name = "webBody";
            this.webBody.Size = new System.Drawing.Size(585, 331);
            this.webBody.TabIndex = 3;
            // 
            // btnBookAll
            // 
            this.btnBookAll.Name = "btnBookAll";
            this.btnBookAll.Size = new System.Drawing.Size(156, 22);
            this.btnBookAll.Text = "Book all...";
            this.btnBookAll.ToolTipText = "Send request for all bookings";
            // 
            // btnBookThis
            // 
            this.btnBookThis.Name = "btnBookThis";
            this.btnBookThis.Size = new System.Drawing.Size(156, 22);
            this.btnBookThis.Text = "Book selected...";
            this.btnBookThis.ToolTipText = "Send request for the selected booking(s)";
            // 
            // myToolStrip1
            // 
            this.myToolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip1.DisableAllMenuItems = true;
            this.myToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAttach});
            this.myToolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.myToolStrip1.Location = new System.Drawing.Point(514, 424);
            this.myToolStrip1.Name = "myToolStrip1";
            this.myToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip1.Size = new System.Drawing.Size(74, 25);
            this.myToolStrip1.TabIndex = 105;
            this.myToolStrip1.Text = "myToolStrip1";
            // 
            // btnAttach
            // 
            this.btnAttach.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAttachThis,
            this.btnAttachAll});
            this.btnAttach.Image = global::TourWriter.Properties.Resources.Attach;
            this.btnAttach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(71, 22);
            this.btnAttach.Text = "Attach";
            this.btnAttach.ToolTipText = "Add an attachment";
            // 
            // btnAttachThis
            // 
            this.btnAttachThis.Name = "btnAttachThis";
            this.btnAttachThis.Size = new System.Drawing.Size(152, 22);
            this.btnAttachThis.Text = "To this email...";
            this.btnAttachThis.Click += new System.EventHandler(this.btnAttachThis_Click);
            // 
            // btnAttachAll
            // 
            this.btnAttachAll.Name = "btnAttachAll";
            this.btnAttachAll.Size = new System.Drawing.Size(152, 22);
            this.btnAttachAll.Text = "To all emails...";
            this.btnAttachAll.Click += new System.EventHandler(this.btnAttachAll_Click);
            // 
            // EmailForm
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.myToolStrip1);
            this.Controls.Add(this.webBody);
            this.Controls.Add(this.txtTo);
            this.Controls.Add(this.txtSubject);
            this.Controls.Add(this.txtAttach);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPosition);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Name = "EmailForm";
            this.Size = new System.Drawing.Size(591, 453);
            this.VisibleChanged += new System.EventHandler(this.EmailForm_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.errorProv)).EndInit();
            this.myToolStrip1.ResumeLayout(false);
            this.myToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPosition;
        private Infragistics.Win.Misc.UltraButton btnNext;
        private Infragistics.Win.Misc.UltraButton btnBack;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ErrorProvider errorProv;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAttach;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.WebBrowser webBody;
        private System.Windows.Forms.ToolStripMenuItem btnBookAll;
        private System.Windows.Forms.ToolStripMenuItem btnBookThis;
        private TourWriter.UserControls.MyToolStrip myToolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton btnAttach;
        private System.Windows.Forms.ToolStripMenuItem btnAttachThis;
        private System.Windows.Forms.ToolStripMenuItem btnAttachAll;
    }
}
