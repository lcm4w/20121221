using System;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using TourWriter.Info;

namespace TourWriter.Modules.AdminModule.UserControls
{
    /// <summary>
    /// Summary description for NewUser.
    /// </summary>
    public class EmailSettings : UserControl
    {
        #region Designer

        private Label lblHeading;
        private Label label6;
        internal ImageList imageList1;
        private GroupBox groupBox1;
        private UltraTextEditor txtEmailServer;
        private UltraNumericEditor txtEmailPort;
        private TextBox txtEmailPass;
        private Label label7;
        private TextBox txtEmailUser;
        private Label label4;
        private Label label1;
        private Label label2;
        private Button btnTestEmail;
        private CheckBox chkEnableSsl;
        private IContainer components;

        public EmailSettings()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            EndAllEdits();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmailSettings));
            this.lblHeading = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtEmailServer = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtEmailPort = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.chkEnableSsl = new System.Windows.Forms.CheckBox();
            this.btnTestEmail = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEmailPass = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtEmailUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmailServer)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmailPort)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 42;
            this.lblHeading.Text = "Email Settings";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(480, 16);
            this.label6.TabIndex = 41;
            this.label6.Text = "Configure email settings used by the TourWriter application.";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Silver;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            this.imageList1.Images.SetKeyName(20, "");
            this.imageList1.Images.SetKeyName(21, "");
            this.imageList1.Images.SetKeyName(22, "");
            this.imageList1.Images.SetKeyName(23, "");
            this.imageList1.Images.SetKeyName(24, "");
            this.imageList1.Images.SetKeyName(25, "");
            this.imageList1.Images.SetKeyName(26, "");
            this.imageList1.Images.SetKeyName(27, "");
            this.imageList1.Images.SetKeyName(28, "");
            this.imageList1.Images.SetKeyName(29, "");
            this.imageList1.Images.SetKeyName(30, "");
            this.imageList1.Images.SetKeyName(31, "");
            this.imageList1.Images.SetKeyName(32, "");
            this.imageList1.Images.SetKeyName(33, "");
            this.imageList1.Images.SetKeyName(34, "");
            this.imageList1.Images.SetKeyName(35, "");
            this.imageList1.Images.SetKeyName(36, "");
            this.imageList1.Images.SetKeyName(37, "");
            this.imageList1.Images.SetKeyName(38, "");
            this.imageList1.Images.SetKeyName(39, "");
            this.imageList1.Images.SetKeyName(40, "");
            this.imageList1.Images.SetKeyName(41, "");
            // 
            // txtEmailServer
            // 
            this.txtEmailServer.Location = new System.Drawing.Point(126, 18);
            this.txtEmailServer.Name = "txtEmailServer";
            this.txtEmailServer.Size = new System.Drawing.Size(188, 21);
            this.txtEmailServer.TabIndex = 60;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtEmailPort);
            this.groupBox1.Controls.Add(this.chkEnableSsl);
            this.groupBox1.Controls.Add(this.btnTestEmail);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtEmailPass);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtEmailUser);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtEmailServer);
            this.groupBox1.Location = new System.Drawing.Point(8, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(500, 171);
            this.groupBox1.TabIndex = 62;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Email server information";
            // 
            // txtEmailPort
            // 
            this.txtEmailPort.FormatString = "g";
            this.txtEmailPort.Location = new System.Drawing.Point(126, 44);
            this.txtEmailPort.Name = "txtEmailPort";
            this.txtEmailPort.Nullable = true;
            this.txtEmailPort.NullText = "25";
            this.txtEmailPort.Size = new System.Drawing.Size(40, 21);
            this.txtEmailPort.TabIndex = 111;
            // 
            // chkEnableSsl
            // 
            this.chkEnableSsl.AutoSize = true;
            this.chkEnableSsl.Location = new System.Drawing.Point(183, 48);
            this.chkEnableSsl.Name = "chkEnableSsl";
            this.chkEnableSsl.Size = new System.Drawing.Size(180, 17);
            this.chkEnableSsl.TabIndex = 120;
            this.chkEnableSsl.Text = "Use encrypted connection (SSL)";
            this.chkEnableSsl.UseVisualStyleBackColor = true;
            // 
            // btnTestEmail
            // 
            this.btnTestEmail.Location = new System.Drawing.Point(414, 110);
            this.btnTestEmail.Name = "btnTestEmail";
            this.btnTestEmail.Size = new System.Drawing.Size(75, 23);
            this.btnTestEmail.TabIndex = 119;
            this.btnTestEmail.Text = "Test";
            this.btnTestEmail.UseVisualStyleBackColor = true;
            this.btnTestEmail.Click += new System.EventHandler(this.btnTestEmail_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 118;
            this.label2.Text = "Outgoing port number";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 117;
            this.label1.Text = "Outgoing email server";
            // 
            // txtEmailPass
            // 
            this.txtEmailPass.Location = new System.Drawing.Point(126, 110);
            this.txtEmailPass.Name = "txtEmailPass";
            this.txtEmailPass.PasswordChar = '*';
            this.txtEmailPass.Size = new System.Drawing.Size(188, 20);
            this.txtEmailPass.TabIndex = 116;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 115;
            this.label7.Text = "Email password";
            // 
            // txtEmailUser
            // 
            this.txtEmailUser.Location = new System.Drawing.Point(126, 85);
            this.txtEmailUser.Name = "txtEmailUser";
            this.txtEmailUser.Size = new System.Drawing.Size(188, 20);
            this.txtEmailUser.TabIndex = 114;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 113;
            this.label4.Text = "Email username";
            // 
            // EmailSettings
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label6);
            this.Name = "EmailSettings";
            this.Size = new System.Drawing.Size(520, 293);
            this.Load += new System.EventHandler(this.EmailSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtEmailServer)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmailPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        private ToolSet toolSet
        {
            get { return (Tag as AdminMain).ToolSet; }
        }


        private void EmailSettings_Load(object sender, EventArgs e)
        {
            if (toolSet.AppSettings[0].IsSmtpServerEnableSslNull())
                toolSet.AppSettings[0].SmtpServerEnableSsl = false;

            // bindings			
            txtEmailServer.DataBindings.Add("Text", toolSet, "AppSettings.SmtpServerName");
            txtEmailPort.DataBindings.Add("Text", toolSet, "AppSettings.SmtpServerPort");
            chkEnableSsl.DataBindings.Add("Checked", toolSet, "AppSettings.SmtpServerEnableSsl");
            txtEmailUser.DataBindings.Add("Text", toolSet, "AppSettings.SmtpServerUsername");
            txtEmailPass.DataBindings.Add("Text", toolSet, "AppSettings.SmtpServerPassword");
        }

        private void EndAllEdits()
        {
            Validate();
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            EndAllEdits();
            base.OnValidating(e);
        }

        private void btnTestEmail_Click(object sender, EventArgs e)
        {
            if (Global.Cache.User.IsEmailNull() || Global.Cache.User.Email == "")
            {
                App.ShowError("You have not entered your send-to address, please do this in Tools-Options-User");
                return;
            }

            var email = new Services.EmailMessage();
            email._To = Global.Cache.User.Email;
            email._From = Global.Cache.User.Email;
            email.Subject = "TourWriter test email";
            email.Body = "This is a test email, initiated by a TourWriter user to test their email settings.";

            bool isSent;
            try
            {
                Cursor = Cursors.WaitCursor;
                isSent = email.Send(
                    txtEmailServer.Text,
                    txtEmailPort.Value != null ? (int) txtEmailPort.Value : 25,
                    chkEnableSsl.Checked, txtEmailUser.Text,
                    txtEmailPass.Text);
            }
            finally { Cursor = Cursors.Default; }

            if (isSent)
                App.ShowInfo("Email sent successfully.");
            else
                App.ShowError("Failed to send email: " + email._ErrorMsg);
        }
    }
}