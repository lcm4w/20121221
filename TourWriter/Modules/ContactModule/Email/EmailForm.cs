using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TourWriter.Services;

namespace TourWriter.Modules.ContactModule.Email
{
    /// <summary>
    /// Summary description for Emailer.
    /// </summary>
    public partial class EmailForm : UserControl
    {
        private int currentIndex = 0;
        private readonly List<ContactEmailInfo> emailList;

        public EmailForm(List<ContactEmailInfo> emailList)
        {
            InitializeComponent();
            this.emailList = emailList;
        }

        public void ShowEmail(int index)
        {
            btnBack.Enabled = (index > 0);
            btnNext.Enabled = (index < emailList.Count - 1);
            lblPosition.Text = String.Format("Email {0} of {1}", index + 1, emailList.Count);

            EmailMessage email = emailList[index].EmailMessage;
            txtTo.Text = email._To;
            txtBcc.Text = email._Bcc;
            txtSubject.Text = email.Subject;
            webBody.DocumentText = email.Body;
            foreach (System.Net.Mail.Attachment att in email.Attachments)
                txtAttach.Text += att.Name + ",";
            txtAttach.Text = txtAttach.Text.TrimEnd(',');

            currentIndex = index;
        }

        private void SaveCurrentEmail()
        {
            EmailMessage email = emailList[currentIndex].EmailMessage;
            if (email != null)
            {
                email._To = txtTo.Text;
                email._Bcc = txtBcc.Text;
                email.Subject = txtSubject.Text;
                email.Body = TemplateSettings.ReplaceBodyHtml(webBody.DocumentText, webBody.Document.Body.OuterHtml);
            }
        }

        private void AddAttachment()
        {
            if (emailList.Count == 0)
                return;

            string[] files = App.SelectExternalFiles(false, "Choose attachment to add", String.Empty, 0);

            if (files != null && files.Length > 0)
            {
                EmailMessage email = emailList[currentIndex].EmailMessage;

                foreach (string filename in files)
                {
                    email.AddAttachment(filename);
                    txtAttach.Text += ((txtAttach.Text == String.Empty) ? filename : ", " + filename);
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentEmail();
            ShowEmail(currentIndex + 1);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SaveCurrentEmail();
            ShowEmail(currentIndex - 1);
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            AddAttachment();
        }

        private void EmailForm_VisibleChanged(object sender, EventArgs e)
        {
            bool movingToNextForm = !Visible;

            if (movingToNextForm)
                SaveCurrentEmail();
        }
    }
}