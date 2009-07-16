using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TourWriter.Services;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Forms
{
    /// <summary>
    /// Form to show errors to user.
    /// </summary>
    public class MessageForm : Form
    {
        #region Designer

        private Button btnClose;
        private LinkLabel btnDetails;
        private CheckBox chkEmail;
        private readonly Container components = null;
        private Label label1;
        private CheckBox chkScreenshot;
        private Button btnCopy;
        private TextBox txtMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageForm));
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDetails = new System.Windows.Forms.LinkLabel();
            this.chkEmail = new System.Windows.Forms.CheckBox();
            this.chkScreenshot = new System.Windows.Forms.CheckBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDetails
            // 
            resources.ApplyResources(this.btnDetails, "btnDetails");
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.TabStop = true;
            this.btnDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnDetails_LinkClicked);
            // 
            // chkEmail
            // 
            resources.ApplyResources(this.chkEmail, "chkEmail");
            this.chkEmail.Checked = true;
            this.chkEmail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEmail.Name = "chkEmail";
            this.chkEmail.UseVisualStyleBackColor = true;
            // 
            // chkScreenshot
            // 
            resources.ApplyResources(this.chkScreenshot, "chkScreenshot");
            this.chkScreenshot.Name = "chkScreenshot";
            this.chkScreenshot.UseVisualStyleBackColor = true;
            // 
            // txtMessage
            // 
            resources.ApplyResources(this.txtMessage, "txtMessage");
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.TextChanged += new System.EventHandler(this.txtMessage_TextChanged);
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // MessageForm
            // 
            this.AcceptButton = this.btnClose;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.chkScreenshot);
            this.Controls.Add(this.chkEmail);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MessageForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Load += new System.EventHandler(this.MessageForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MessageForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #endregion

        private readonly string msgDetail = "";
        private string msgMessage = "";

        // defaults
        private const string defaultFileName = "screenshot.png";
        private static readonly ImageFormat defaultFileType = ImageFormat.Png;

        public bool AllowEdit
        {
            set { txtMessage.ReadOnly = !value; }
        }

        public bool SendSupportEmail
        {
            set { chkEmail.Checked = value; }
        }

        public bool IncludeScreenshot
        {
            set { chkScreenshot.Checked = value; }
        }

        private MessageForm()
        {
            InitializeComponent();
            Icon = Resources.TourWriter16;
            TopMost = true;
        }

        public MessageForm(string message) : this()
        {
            msgMessage = message;
            btnDetails.Visible = false;
            btnCopy.Visible = false;
        }

        public MessageForm(Exception exception) : this()
        {
            msgMessage = exception.Message;
            msgDetail = exception.ToString();
        }

        public MessageForm(string message, Exception exception) : this()
        {
            msgMessage = message;
            msgDetail = exception.ToString();
        }

        public MessageForm(string message, string detail) : this()
        {
            msgMessage = message;
            msgDetail = message + "\r\n" + detail;
        }


        private void MessageForm_Load(object sender, EventArgs e)
        {
            txtMessage.Text = !string.IsNullOrEmpty(msgMessage) ? msgMessage : "(no message)";
        }

        private void SendEmail()
        {
            const bool suppressErrors = false;

            if (chkScreenshot.Checked)
            {
                ErrorHelper.SendEmail(
                    msgMessage, msgDetail, suppressErrors, GetScreenshotAttachment());
            }
            else
            {
                ErrorHelper.SendEmail(msgMessage, msgDetail, suppressErrors);
            }
        }

        private static Attachment GetScreenshotAttachment()
        {
            MemoryStream ms = ScreenshotService.CaptureScreenToMemoryStream(defaultFileType);
            return new Attachment(ms, defaultFileName);
        }

        private void btnDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // toggle message/detail
            txtMessage.Text = txtMessage.Text == msgMessage ? msgDetail : msgMessage;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MessageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (InvokeRequired) { Invoke(new MethodInvoker(Hide)); }
            else { Hide(); }
            App.MainForm.Invoke((MethodInvoker) (() => App.MainForm.BringToFront()));
            Application.DoEvents();

            if (chkEmail.Checked) SendEmail();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                txtMessage.SelectAll();
                Clipboard.SetText(msgDetail);
                App.ShowInfo("Details copied to clipboard, ready to paste.");
            }
            catch (ExternalException)
            {
                App.ShowInfo("Failed to copy details to clipboard.");
            }
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            // if the message box is not readonly then this is a user message
            if (!txtMessage.ReadOnly)
                msgMessage = txtMessage.Text;
        }
    }
}
