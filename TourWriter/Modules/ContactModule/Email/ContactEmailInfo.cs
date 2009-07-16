using System;
using System.Text;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.ContactModule.Email
{
    public class ContactEmailInfo
    {
        #region Private properties

        private readonly ContactSet.ContactRow contact;

        private EmailMessage emailMessage;

        #endregion

        public EmailMessage EmailMessage
        {
            get { return emailMessage; }
        }

        public ContactEmailInfo(ContactSet.ContactRow row)
        {
            contact = row;
        }

        public void CreateEmailMessage(TemplateSettings templateSettings)
        {
            // create email
            emailMessage = new EmailMessage();
            emailMessage._From = templateSettings.From;
            emailMessage.Subject = templateSettings.Subject;
            emailMessage.BodyEncoding = Encoding.UTF8;
            emailMessage.Body = BuildEmailBody(templateSettings.Body);
            emailMessage._Tag = this;
            emailMessage._SaveWhenSent = templateSettings.SaveToFile;
            emailMessage.IsBodyHtml = true;
            emailMessage._Bcc = templateSettings.Bcc;
            emailMessage._To = GetToAddress(contact.ContactName, contact.Email1);

            if (templateSettings.ReadReceipt)
            {
                // add read receipt
                emailMessage.Headers.Add("Disposition-Notification-To", templateSettings.From);
            }
        }
        
        #region Email body creation

        private string BuildEmailBody(string template)
        {
            template = ReplaceTag(template, TemplateSettings.TagDisplayName, contact.ContactName);
            template = ReplaceTag(template, TemplateSettings.TagTitle, contact.Title);
            template = ReplaceTag(template, TemplateSettings.TagFirstName, contact.FirstName);
            template = ReplaceTag(template, TemplateSettings.TagLastName, contact.LastName);
            template = ReplaceTag(template, TemplateSettings.TagUserDisplayName, Cache.User.DisplayName);
            template = ReplaceTag(template, TemplateSettings.TagUserEmail, Cache.User.Email);

            return template;
        }

        private static string GetToAddress(string name, string email)
        {
            // strip illegal characters from name
            name = name.
                Replace(",", "").
                Replace(";", "").
                Replace(":", "").
                Replace("(", "").
                Replace(")", "").
                Replace("[", "").
                Replace("]", "").
                Replace("<", "").
                Replace(">", "");

            // format RFC 2822 style address of "name <email>"
            return String.Format("{0} <{1}>", name, email);
        }

        private static string ReplaceTag(string template, string tag, string newValue)
        {
            return template.Replace(tag, newValue);
        }

        #endregion
    }
}
