namespace TourWriter.Modules.ItineraryModule.Bookings.Email
{
    partial class TemplateForm
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
            this.errorProv = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkSaveCopy = new System.Windows.Forms.CheckBox();
            this.chkBccSender = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBcc = new System.Windows.Forms.TextBox();
            this.chkShowBookingPrice = new System.Windows.Forms.CheckBox();
            this.txtTemplate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.webBody = new System.Windows.Forms.WebBrowser();
            this.chkReadReceipt = new System.Windows.Forms.CheckBox();
            this.chkSkip = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkGroupByEmail = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProv)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProv
            // 
            this.errorProv.ContainerControl = this;
            // 
            // txtFrom
            // 
            this.txtFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFrom.Location = new System.Drawing.Point(52, 40);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(344, 20);
            this.txtFrom.TabIndex = 1;
            // 
            // txtSubject
            // 
            this.txtSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubject.Location = new System.Drawing.Point(52, 86);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(344, 20);
            this.txtSubject.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(376, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "These details will be used as the template to build the booking email messages";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkSaveCopy
            // 
            this.chkSaveCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSaveCopy.AutoSize = true;
            this.chkSaveCopy.Location = new System.Drawing.Point(419, 29);
            this.chkSaveCopy.Name = "chkSaveCopy";
            this.chkSaveCopy.Size = new System.Drawing.Size(166, 17);
            this.chkSaveCopy.TabIndex = 4;
            this.chkSaveCopy.Text = "Save a copy of the sent email";
            this.chkSaveCopy.UseVisualStyleBackColor = true;
            // 
            // chkBccSender
            // 
            this.chkBccSender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBccSender.AutoSize = true;
            this.chkBccSender.Location = new System.Drawing.Point(419, 49);
            this.chkBccSender.Name = "chkBccSender";
            this.chkBccSender.Size = new System.Drawing.Size(164, 17);
            this.chkBccSender.TabIndex = 5;
            this.chkBccSender.Text = "Bcc to senders email address";
            this.chkBccSender.UseVisualStyleBackColor = true;
            this.chkBccSender.CheckedChanged += new System.EventHandler(this.chkBccSender_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "From";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Bcc";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Subject";
            // 
            // txtBcc
            // 
            this.txtBcc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBcc.Location = new System.Drawing.Point(52, 63);
            this.txtBcc.Name = "txtBcc";
            this.txtBcc.Size = new System.Drawing.Size(344, 20);
            this.txtBcc.TabIndex = 2;
            // 
            // chkShowBookingPrice
            // 
            this.chkShowBookingPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowBookingPrice.AutoSize = true;
            this.chkShowBookingPrice.Location = new System.Drawing.Point(419, 70);
            this.chkShowBookingPrice.Name = "chkShowBookingPrice";
            this.chkShowBookingPrice.Size = new System.Drawing.Size(168, 17);
            this.chkShowBookingPrice.TabIndex = 6;
            this.chkShowBookingPrice.Text = "Show price details for booking";
            this.chkShowBookingPrice.UseVisualStyleBackColor = true;
            // 
            // txtTemplate
            // 
            this.txtTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTemplate.Location = new System.Drawing.Point(68, 427);
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.Size = new System.Drawing.Size(498, 20);
            this.txtTemplate.TabIndex = 38;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 430);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "Template";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Image = global::TourWriter.Properties.Resources.FolderExplore;
            this.btnBrowse.Location = new System.Drawing.Point(568, 426);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(23, 22);
            this.btnBrowse.TabIndex = 40;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // webBody
            // 
            this.webBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBody.Location = new System.Drawing.Point(3, 136);
            this.webBody.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBody.Name = "webBody";
            this.webBody.Size = new System.Drawing.Size(591, 285);
            this.webBody.TabIndex = 8;
            // 
            // chkReadReceipt
            // 
            this.chkReadReceipt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkReadReceipt.AutoSize = true;
            this.chkReadReceipt.Location = new System.Drawing.Point(419, 90);
            this.chkReadReceipt.Name = "chkReadReceipt";
            this.chkReadReceipt.Size = new System.Drawing.Size(129, 17);
            this.chkReadReceipt.TabIndex = 7;
            this.chkReadReceipt.Text = "Include a read receipt";
            this.chkReadReceipt.UseVisualStyleBackColor = true;
            // 
            // chkSkip
            // 
            this.chkSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSkip.AutoSize = true;
            this.chkSkip.Location = new System.Drawing.Point(419, 8);
            this.chkSkip.Name = "chkSkip";
            this.chkSkip.Size = new System.Drawing.Size(93, 17);
            this.chkSkip.TabIndex = 41;
            this.chkSkip.Text = "Skip this page";
            this.toolTip1.SetToolTip(this.chkSkip, "Go straight to the next page when window opens");
            this.chkSkip.UseVisualStyleBackColor = true;
            // 
            // checkGroupByEmail
            // 
            this.checkGroupByEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkGroupByEmail.AutoSize = true;
            this.checkGroupByEmail.Location = new System.Drawing.Point(419, 110);
            this.checkGroupByEmail.Name = "checkGroupByEmail";
            this.checkGroupByEmail.Size = new System.Drawing.Size(183, 17);
            this.checkGroupByEmail.TabIndex = 42;
            this.checkGroupByEmail.Text = "Group bookings by Supplier email";
            this.checkGroupByEmail.UseVisualStyleBackColor = true;
            // 
            // TemplateForm
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.checkGroupByEmail);
            this.Controls.Add(this.chkSkip);
            this.Controls.Add(this.chkReadReceipt);
            this.Controls.Add(this.webBody);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtTemplate);
            this.Controls.Add(this.chkShowBookingPrice);
            this.Controls.Add(this.txtBcc);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkBccSender);
            this.Controls.Add(this.chkSaveCopy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSubject);
            this.Controls.Add(this.txtFrom);
            this.Name = "TemplateForm";
            this.Size = new System.Drawing.Size(597, 453);
            ((System.ComponentModel.ISupportInitialize)(this.errorProv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ErrorProvider errorProv;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkBccSender;
        private System.Windows.Forms.CheckBox chkSaveCopy;
        private System.Windows.Forms.TextBox txtBcc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkShowBookingPrice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTemplate;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.WebBrowser webBody;
        private System.Windows.Forms.CheckBox chkReadReceipt;
        private System.Windows.Forms.CheckBox chkSkip;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkGroupByEmail;
    }
}
