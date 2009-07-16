using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Properties;

namespace TourWriter.Modules.ContactModule.Email
{
    /// <summary>
    /// Summary description for Template.
    /// </summary>
    public partial class TemplateForm : UserControl
    {
        private TemplateSettings templateSettings;
        private const string DefaultSubject = "Subject...";
        
        public TemplateForm()
        {
            InitializeComponent();

            webBody.Navigate("about:blank");
            webBody.Document.Write("<html><body></body></html>");
            webBody.Document.Body.SetAttribute("contenteditable", "true");

            txtFrom.Text = Cache.User.Email;
            txtSubject.Text = DefaultSubject;

            chkBccSender.Checked = Settings.Default.EmailerBccSender;
            chkReadReceipt.Checked = Settings.Default.EmailerReadReceipt;
        }

        public TemplateSettings GetTemplateSettings()
        {
            if (templateSettings == null)
            {
                templateSettings = new TemplateSettings();
            }
            templateSettings.From = txtFrom.Text;
            templateSettings.Bcc = ((chkBccSender.Checked) ? txtBcc.Text : String.Empty);
            templateSettings.Subject = txtSubject.Text;
            templateSettings.ReadReceipt = chkReadReceipt.Checked;
            templateSettings.Body = TemplateSettings.ReplaceBodyHtml(
                webBody.DocumentText, webBody.Document.Body.OuterHtml);

            return templateSettings;
        }

        private static string ReadTemplateText(string templateFile)
        {
            StreamReader sr = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                string input;
                sr = File.OpenText(templateFile);

                while ((input = sr.ReadLine()) != null)
                    sb.AppendLine(input);

                if (!sb.ToString().ToLower().Contains("<body") ||
                    !sb.ToString().ToLower().Contains("</body>"))
                {
                    App.ShowError("Invalid email template, 'body' opening or closing tag not found");
                }
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            return sb.ToString();
        }

        #region Events

        private void chkBccSender_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBccSender.Checked)
                txtBcc.Text = Cache.User.Email;

            else
                txtBcc.Text = String.Empty;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string file = App.SelectExternalFile(
                false, "Select email template", "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*", 0);

            if (file == null)
                return;

            txtTemplate.Text = file;

            webBody.Document.OpenNew(false);
            webBody.Document.Write(ReadTemplateText(txtTemplate.Text));
            webBody.Document.Body.SetAttribute("contenteditable", "true");
        }

        #endregion
    }
}
