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
        public bool SkipTemplate
        {
            get { return chkSkip.Checked; }
            set { chkSkip.Checked = value; }
        }

        public bool GroupBySupplierEmail
        {
            get { return checkGroupByEmail.Checked; }
            set { checkGroupByEmail.Checked = value; }
        }

        private const string DefaultSubject = "Booking request for {0}";
        private const string ErrorText = "<html><body>Default template text not found...</body></html>";
        private readonly string _templateFile;
        private TemplateSettings _templateSettings;
        private string _defaultTemplate;
        private string DefaultTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultTemplate))
                {
                    // try TourWriterData folder
                    var template = Path.Combine(Services.ExternalFilesHelper.GetTemplateFolder(), "BookingRequest.html");
                    if (!File.Exists(template))
                        template = Path.Combine(Services.ExternalFilesHelper.GetTemplateFolder(), "TourWriter.Email.BookingRequest.html");

                    // use default 
                    if (!File.Exists(template))
                        template = Path.Combine(App.Path_DefaultTemplatesFolder, "Email\\BookingRequest.html");
                    _defaultTemplate = template;
                }
                return _defaultTemplate;
            }
        }


        public TemplateForm(string itineraryName) : this (itineraryName, "") { }

        public TemplateForm(string itineraryName, string templateFile)
        {
            templateFile = Services.ExternalFilesHelper.ConvertToAbsolutePath(templateFile);
            _templateFile = File.Exists(templateFile) ? templateFile : DefaultTemplate;

            InitializeComponent();
            chkSaveCopy.Checked = Settings.Default.EmailerSaveWhenSent;
            chkBccSender.Checked = Settings.Default.EmailerBccSender;
            chkShowBookingPrice.Checked = Settings.Default.BookingEmailShowPrice;
            chkReadReceipt.Checked = Settings.Default.EmailerReadReceipt;
            checkGroupByEmail.Checked = Settings.Default.EmailerGroupBookingsSupplier;
            
            txtFrom.Text = Cache.User.Email;
            txtSubject.Text = String.Format(DefaultSubject, itineraryName);
            txtTemplate.Text = _templateFile;

            webBody.Navigate("about:blank");
            LoadTemplateFile(_templateFile);
        }

        private void LoadTemplateFile(string file)
        {
            if (webBody.Document != null)
            {
                var template = ReadTemplate(file);
                ProcessTemplateSubject(ref template);
                webBody.Document.OpenNew(false);
                webBody.Document.Write(template);

                if (webBody.Document.Body != null)
                    webBody.Document.Body.SetAttribute("contenteditable", "true");
            }
        }

        private void ProcessTemplateSubject(ref string template)
        {
            try
            {
                if (template.Contains(BookingEmailInfo.SubjectStartTag))
                {
                    var i = template.IndexOf(BookingEmailInfo.SubjectStartTag);
                    var j = template.IndexOf(BookingEmailInfo.SubjectEndTag) + BookingEmailInfo.SubjectEndTag.Length;
                    var subject = template.Substring(i, j - i);
                    var body = template.Remove(i, j - i);

                    txtSubject.Text = subject.Replace(BookingEmailInfo.SubjectStartTag, "").Replace(BookingEmailInfo.SubjectEndTag, "");
                    template = body;
                }
            }
            catch (Exception ex)
            {
                App.Error("Failed to parse Subject text, check that Subject is correct in the template (use tags " + 
                    BookingEmailInfo.SubjectStartTag + " and " + BookingEmailInfo.SubjectEndTag, ex, false);
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