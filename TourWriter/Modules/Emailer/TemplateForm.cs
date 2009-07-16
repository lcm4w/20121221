using System;
using System.ComponentModel;
using TourWriter.Properties;
using TourWriter.UserControls;

namespace TourWriter.Modules.Emailer
{
    /// <summary>
    /// Summary description for Template.
    /// </summary>
    public partial class TemplateForm : WizardPage
    {
        private readonly bool showBookingEmailPricing;
        private readonly string defaultEmailTemplateFile = 
            Services.ExternalFilesHelper.GetTemplateFolder() + "\\TourWriter.Email.BookingRequest.txt";
        
        public TemplateForm()
        {
            InitializeComponent();
        }

        public TemplateForm(bool showBookingEmailPricing)
        {
            InitializeComponent();

            this.showBookingEmailPricing = showBookingEmailPricing;
        }

        
        private void TemplateForm_Load(object sender, EventArgs e)
        {
            LoadSettings();

            chkShowBookingPrice.Visible = showBookingEmailPricing;
        }

        private void TemplateForm_WizardNext(object sender, WizardPageEventArgs e)
        {
            if (!ValidateFields())
            {
                e.Cancel = true;
                return;
            }
            SaveSettings();
        }


        private void LoadSettings()
        {
            EmailBuilderBase parms = Wizard.Params as EmailBuilderBase;

            txtFrom.Text = parms.TemplateFrom;
            txtBody.Text = parms.TemplateBody;
            txtSubject.Text = parms.TemplateSubject;

            chkSaveCopy.Checked = Settings.Default.EmailerSaveWhenSent;
            chkBccSender.Checked = Settings.Default.EmailerBccSender;
            chkShowBookingPrice.Checked = Settings.Default.BookingEmailShowPrice;

            txtTemplate.Text = defaultEmailTemplateFile;
        }

        private void SaveSettings()
        {
            EmailBuilderBase builder = Wizard.Params as EmailBuilderBase;

            builder.TemplateFrom = txtFrom.Text;
            builder.TemplateBody = txtBody.Text;
            builder.TemplateSubject = txtSubject.Text;
            builder.TemplateBcc = chkBccSender.Checked ? txtFrom.Text : "";
            builder.SaveSendMessages = chkSaveCopy.Checked;
            builder.ShowBookingPrice = chkShowBookingPrice.Checked;

            Settings.Default.EmailerSaveWhenSent = chkSaveCopy.Checked;
            Settings.Default.EmailerBccSender = chkBccSender.Checked;
            Settings.Default.BookingEmailShowPrice = chkShowBookingPrice.Checked;
            Settings.Default.Save();
        }

        private static bool ValidateFields()
        {
            bool isValid = true;

            // TODO: validation

            return isValid;
        }

        private void TemplateForm_QueryCancel(object sender, CancelEventArgs e)
        {
            e.Cancel = !(Wizard.Params as EmailBuilderBase).QueryContinueToCancelEmails();
        }

        private void chkBccSender_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBccSender.Checked)
                txtBcc.Text = TourWriter.Global.Cache.User.Email;
            else
                txtBcc.Text = "";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string file = App.SelectExternalFile(
                false, "Select email template", "Text files (*.txt)|*.txt|All files (*.*)|*.*", 0);

            if (file == null)
                return;

            txtTemplate.Text = file;
            txtBody.Text = EmailBuilderBase.ReadTemplateText(txtTemplate.Text);
        }
    }
}