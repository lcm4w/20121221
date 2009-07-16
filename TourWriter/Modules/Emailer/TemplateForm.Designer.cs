namespace TourWriter.Modules.Emailer
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
            this.txtBody = new System.Windows.Forms.TextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.errorProv)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProv
            // 
            this.errorProv.ContainerControl = this;
            // 
            // txtBody
            // 
            this.txtBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBody.Location = new System.Drawing.Point(8, 131);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBody.Size = new System.Drawing.Size(575, 309);
            this.txtBody.TabIndex = 27;
            // 
            // txtFrom
            // 
            this.txtFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFrom.Location = new System.Drawing.Point(52, 60);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(338, 20);
            this.txtFrom.TabIndex = 28;
            // 
            // txtSubject
            // 
            this.txtSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubject.Location = new System.Drawing.Point(52, 106);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(338, 20);
            this.txtSubject.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "The details entered here will be used to build all the emails from.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkSaveCopy
            // 
            this.chkSaveCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSaveCopy.AutoSize = true;
            this.chkSaveCopy.Location = new System.Drawing.Point(419, 62);
            this.chkSaveCopy.Name = "chkSaveCopy";
            this.chkSaveCopy.Size = new System.Drawing.Size(166, 17);
            this.chkSaveCopy.TabIndex = 31;
            this.chkSaveCopy.Text = "Save a copy of the sent email";
            this.chkSaveCopy.UseVisualStyleBackColor = true;
            // 
            // chkBccSender
            // 
            this.chkBccSender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBccSender.AutoSize = true;
            this.chkBccSender.Location = new System.Drawing.Point(419, 85);
            this.chkBccSender.Name = "chkBccSender";
            this.chkBccSender.Size = new System.Drawing.Size(164, 17);
            this.chkBccSender.TabIndex = 32;
            this.chkBccSender.Text = "Bcc to senders email address";
            this.chkBccSender.UseVisualStyleBackColor = true;
            this.chkBccSender.CheckedChanged += new System.EventHandler(this.chkBccSender_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "From";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Bcc";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Subject";
            // 
            // txtBcc
            // 
            this.txtBcc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBcc.Location = new System.Drawing.Point(52, 83);
            this.txtBcc.Name = "txtBcc";
            this.txtBcc.Size = new System.Drawing.Size(338, 20);
            this.txtBcc.TabIndex = 36;
            // 
            // chkShowBookingPrice
            // 
            this.chkShowBookingPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowBookingPrice.AutoSize = true;
            this.chkShowBookingPrice.Location = new System.Drawing.Point(419, 108);
            this.chkShowBookingPrice.Name = "chkShowBookingPrice";
            this.chkShowBookingPrice.Size = new System.Drawing.Size(168, 17);
            this.chkShowBookingPrice.TabIndex = 37;
            this.chkShowBookingPrice.Text = "Show price details for booking";
            this.chkShowBookingPrice.UseVisualStyleBackColor = true;
            this.chkShowBookingPrice.Visible = false;
            // 
            // txtTemplate
            // 
            this.txtTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTemplate.Location = new System.Drawing.Point(68, 447);
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.Size = new System.Drawing.Size(492, 20);
            this.txtTemplate.TabIndex = 38;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 450);
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
            this.btnBrowse.Location = new System.Drawing.Point(562, 446);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(23, 22);
            this.btnBrowse.TabIndex = 40;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // TemplateForm
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
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
            this.Controls.Add(this.txtBody);
            this.Name = "TemplateForm";
            this.PageText = "Edit template";
            this.Size = new System.Drawing.Size(591, 473);
            this.QueryCancel += new System.ComponentModel.CancelEventHandler(this.TemplateForm_QueryCancel);
            this.Load += new System.EventHandler(this.TemplateForm_Load);
            this.WizardNext += new TourWriter.UserControls.WizardPageEventHandler(this.TemplateForm_WizardNext);
            this.Controls.SetChildIndex(this.txtBody, 0);
            this.Controls.SetChildIndex(this.txtFrom, 0);
            this.Controls.SetChildIndex(this.txtSubject, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.chkSaveCopy, 0);
            this.Controls.SetChildIndex(this.chkBccSender, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.txtBcc, 0);
            this.Controls.SetChildIndex(this.chkShowBookingPrice, 0);
            this.Controls.SetChildIndex(this.txtTemplate, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.btnBrowse, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorProv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ErrorProvider errorProv;
        private System.Windows.Forms.TextBox txtBody;
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
    }
}
