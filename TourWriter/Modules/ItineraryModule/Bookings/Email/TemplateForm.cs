using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Properties;

namespace TourWriter.Modules.ItineraryModule.Bookings.Email
{
    /// <summary>
    /// Summary description for Template.
    /// </summary>
    public partial class TemplateForm : UserControl
    {
        private TemplateSettings templateSettings;
        private const string DefaultTemplateFile = "TourWriter.Email.BookingRequest.html";
        private const string DefaultSubject = "Booking request for {0}";
        private const string ErrorText =
            "<html><body>Default template text not found...</body></html>";
        
        public TemplateForm(string itineraryName)
        {
            InitializeComponent();

            webBody.Navigate("about:blank");
            webBody.Document.Write(LoadDefaultTemplate());
            webBody.Document.Body.SetAttribute("contenteditable", "true");

            txtFrom.Text = Cache.User.Email;
            txtSubject.Text = String.Format(DefaultSubject, itineraryName);
            txtTemplate.Text = DefaultTemplateFile;

            chkSaveCopy.Checked = Settings.Default.EmailerSaveWhenSent;
            chkBccSender.Checked = Settings.Default.EmailerBccSender;
            chkShowBookingPrice.Checked = Settings.Default.BookingEmailShowPrice;
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
            ;
            templateSettings.Subject = txtSubject.Text;
            templateSettings.SaveToFile = chkSaveCopy.Checked;
            templateSettings.ShowPrices = chkShowBookingPrice.Checked;
            templateSettings.ReadReceipt = chkReadReceipt.Checked;
            templateSettings.Body = TemplateSettings.ReplaceBodyHtml(
                webBody.DocumentText, webBody.Document.Body.OuterHtml);

            return templateSettings;
        }


        private static string LoadDefaultTemplate()
        {
            string fileName = Path.Combine(
                Services.ExternalFilesHelper.GetTemplateFolder(),
                DefaultTemplateFile);

            try
            {
                if (!File.Exists(fileName))
                {
                    string msg = "The default template file was not found.\r\nDo you want to create it?";
                    if (App.AskYesNo(msg))
                    {
                        string path = Path.GetDirectoryName(fileName);
                        try
                        {
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            TextWriter writer = new StreamWriter(fileName);
                            writer.Write(Resources.EmailHtmlTemplate);
                            writer.Close();
                            App.ShowInfo("File created to: " + fileName);
                        }
                        catch (DirectoryNotFoundException)
                        {
                            App.ShowError("Failed to connect to default template directory: " + path);
                            return ErrorText;
                        }
                    }
                }
                return ReadTemplateText(fileName);
            }
            catch (Exception ex)
            {
                App.Error(ex);
                return ErrorText;
            }
        }

        private static string ReadTemplateText(string templateFile)
        {
            if (!File.Exists(templateFile))
                return ErrorText;

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
