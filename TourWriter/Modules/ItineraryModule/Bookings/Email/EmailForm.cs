using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TourWriter.Services;

namespace TourWriter.Modules.ItineraryModule.Bookings.Email
{
    /// <summary>
    /// Summary description for Emailer.
    /// </summary>
    public partial class EmailForm : UserControl
    {
        private int currentIndex = 0;
        private readonly List<BookingEmailInfo> emailList;

        public EmailForm(List<BookingEmailInfo> emailList)
        {
            InitializeComponent();
            this.emailList = emailList;
        }

        public void ShowEmail(int index)
        {
            btnBack.Enabled = (index > 0);
            btnNext.Enabled = (index < emailList.Count - 1);
            lblPosition.Text = String.Format("Booking email {0} of {1}", index + 1, emailList.Count);

            EmailMessage email = emailList[index].EmailMessage;
            txtTo.Text = email._To;
            txtSubject.Text = email.Subject;
            webBody.DocumentText = email.Body;

            txtAttach.Text = String.Empty;
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
                email.Subject = txtSubject.Text;
                email.Body = TemplateSettings.ReplaceBodyHtml(webBody.DocumentText, webBody.Document.Body.OuterHtml);
            }
        }

        private void AddAttachment(bool addToAll)
        {
            if (emailList.Count == 0)
                return;

            string[] files = App.SelectExternalFiles(false, "Choose attachment to add", String.Empty, 0);

            if (files != null && files.Length > 0)
            {
                foreach (string filename in files)
                {
                    if (File.Exists(filename))
                    {
                        if (addToAll)
                        {
                            foreach (var email in emailList)
                                email.EmailMessage.AddAttachment(filename);
                        }
                        else
                            emailList[currentIndex].EmailMessage.AddAttachment(filename);

                        txtAttach.Text += ((txtAttach.Text == String.Empty) ? Path.GetFileName(filename) : "," + Path.GetFileName(filename));
                    }
                    else
                        App.ShowFileNotFound(filename);
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

        private void EmailForm_VisibleChanged(object sender, EventArgs e)
        {
            bool movingToNextForm = !Visible;

            if (movingToNextForm)
                SaveCurrentEmail();
        }

        private void btnAttachThis_Click(object sender, EventArgs e)
        {
            AddAttachment(false);
        }

        private void btnAttachAll_Click(object sender, EventArgs e)
        {
            AddAttachment(true);
        }
    }
}