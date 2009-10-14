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
        private const string DefaultSubject = "Booking request for {0}";
        private const string ErrorText = "<html><body>Default template text not found...</body></html>";
        private readonly string _defaultTemplateFile = Path.Combine(
            Services.ExternalFilesHelper.GetTemplateFolder(), "TourWriter.Email.BookingRequest.html");
        private readonly string _templateFile;
        private TemplateSettings _templateSettings;


        public TemplateForm(string itineraryName) : this (itineraryName, "") { }

        public TemplateForm(string itineraryName, string templateFile)
        {
            InitializeComponent();
            chkSaveCopy.Checked = Settings.Default.EmailerSaveWhenSent;
            chkBccSender.Checked = Settings.Default.EmailerBccSender;
            chkShowBookingPrice.Checked = Settings.Default.BookingEmailShowPrice;
            chkReadReceipt.Checked = Settings.Default.EmailerReadReceipt;

            _templateFile = !string.IsNullOrEmpty(templateFile) ? 
                Services.ExternalFilesHelper.ConvertToAbsolutePath(templateFile) : EnsureDefaultTemplate();

            txtFrom.Text = Cache.User.Email;
            txtSubject.Text = String.Format(DefaultSubject, itineraryName);
            txtTemplate.Text = _templateFile;

            LoadTemplateFile(_templateFile);
        }

        private void LoadTemplateFile(string file)
        {
            webBody.Navigate("about:blank");

            if (webBody.Document != null)
            {
                webBody.Document.OpenNew(false);
                webBody.Document.Write(ReadTemplate(file));

                if (webBody.Document.Body != null)
                    webBody.Document.Body.SetAttribute("contenteditable", "true");
            }
        }

        private static string ReadTemplate(string templateFile)
        {
            if (!File.Exists(templateFile))
                return ErrorText;

            StreamReader sr = null;
            var sb = new StringBuilder();

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

        private string EnsureDefaultTemplate()
        {
            if (!File.Exists(_defaultTemplateFile) &&
                App.AskYesNo("The default template file was not found.\r\nDo you want to create it?"))
            {
                var path = Path.GetDirectoryName(_defaultTemplateFile);
                try
                {
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    TextWriter writer = new StreamWriter(_defaultTemplateFile);
                    writer.Write(Resources.EmailHtmlTemplate);
                    writer.Close();
                    App.ShowInfo("File created: " + _defaultTemplateFile);
                }
                catch (DirectoryNotFoundException)
                {
                    App.ShowError("Failed to connect to default template directory: " + path);
                    return ErrorText;
                }
                catch (Exception ex)
                {
                    App.Error(ex);
                    return ErrorText;
                }
            }
            return _defaultTemplateFile;
        }
        
        public TemplateSettings GetTemplateSettings()
        {
            if (_templateSettings == null) _templateSettings = new TemplateSettings();
            _templateSettings.From = txtFrom.Text;
            _templateSettings.Bcc = ((chkBccSender.Checked) ? txtBcc.Text : String.Empty);
            _templateSettings.Subject = txtSubject.Text;
            _templateSettings.SaveToFile = chkSaveCopy.Checked;
            _templateSettings.ShowPrices = chkShowBookingPrice.Checked;
            _templateSettings.ReadReceipt = chkReadReceipt.Checked;
            if (webBody.Document != null && webBody.Document.Body != null)
                _templateSettings.Body = TemplateSettings.ReplaceBodyHtml(webBody.DocumentText, webBody.Document.Body.OuterHtml);

            return _templateSettings;
        }

        #region Events

        private void chkBccSender_CheckedChanged(object sender, EventArgs e)
        {
            txtBcc.Text = chkBccSender.Checked ? Cache.User.Email : String.Empty;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var file = App.SelectExternalFile(
                false, "Select email template", "HTML files (*.html;*.htm)|*.html;*.htm|All files (*.*)|*.*", 0);
            if (file == null) return;

            txtTemplate.Text = file;
            LoadTemplateFile(file);
        }

        #endregion
    }
}