using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using TourWriter.Services;
using TourWriter.UserControls;

namespace TourWriter.Modules.Emailer
{
    /// <summary>
    /// Summary description for Emailer.
    /// </summary>
    public partial class EmailForm : WizardPage
    {
        private int currentIndex
        {
            get { return (Wizard.Params as EmailBuilderBase).CurrentIndex; }
            set { (Wizard.Params as EmailBuilderBase).CurrentIndex = value; }
        }
        
        private IList emailList
        {
            get { return (Wizard.Params as EmailBuilderBase).EmailList; }
        }
        
        private const string PositionText = "Showing message {0} of {1}";
        

        public EmailForm()
        {
            InitializeComponent();
        }
        
        private void EmailForm_Load(object sender, EventArgs e) { }

        private void BuildAllEmails()
        {
            EmailBuilderBase builder = Wizard.Params as EmailBuilderBase;
            builder.BuildEmails();
        }

        private void LoadNewEmail(int index)
        {
            if (index < 0)
            {
                txtTo.Text = "";
                txtSubject.Text = "";
                txtBody.Text = "";
                txtAttach.Text = "";
            }
            else
            {
                // load texts
                EmailMessage email = emailList[index] as EmailMessage;
                txtTo.Text = email._To;
                txtSubject.Text = email.Subject;
                txtBody.Text = email.Body;
                txtAttach.Text = "";
                foreach (System.Net.Mail.Attachment att in email.Attachments)
                    txtAttach.Text += att.Name + ", ";
                txtAttach.Text = txtAttach.Text.Trim().TrimEnd(',');
            }

            // update position controls			
            currentIndex = index;
            btnBack.Enabled = (currentIndex > 0);
            btnNext.Enabled = (currentIndex < emailList.Count - 1);
            lblPosition.Text = String.Format(PositionText, currentIndex + 1, emailList.Count);

            txtTo.Focus();
            Validate();
        }

        private void SaveCurrentEmail()
        {
            if (currentIndex != -1 && currentIndex < emailList.Count)
            {
                EmailMessage email = emailList[currentIndex] as EmailMessage;
                email._To = txtTo.Text;
                email.Body = txtBody.Text;
                email.Subject = txtSubject.Text;
            }
        }

        private void SetError(object control, string msg)
        {
            Control c = control as Control;

            if (msg.Length > 0)
            {
                errorProv.SetError(c, msg);
                errorProv.SetIconAlignment(c, ErrorIconAlignment.TopRight);
                errorProv.SetIconPadding(c, -18);
            }
            else
                errorProv.SetError(c, "");
        }

        private void AddAttachment()
        {
            string[] files = App.SelectExternalFiles(false, "Choose attachement to add", "", 0);

            if (files != null && files.Length > 0)
            {
                EmailMessage email = emailList[currentIndex] as EmailMessage;
                if (email == null)
                    return;

                foreach (string filename in files)
                {
                    if (File.Exists(filename))
                    {
                        email.AddAttachment(filename);
                        txtAttach.Text += txtAttach.Text == "" ? filename : ", " + filename;
                    }
                    else
                        App.ShowFileNotFound(filename);
                }
            }
        }
        

        #region Events

        private void EmailForm_SetActive(object sender, CancelEventArgs e)
        {
            BuildAllEmails();
            LoadNewEmail(currentIndex);
        }

        private void EmailForm_QueryCancel(object sender, CancelEventArgs e)
        {
            e.Cancel = !(Wizard.Params as EmailBuilderBase).QueryContinueToCancelEmails();
        }

        private void EmailForm_WizardNext(object sender, WizardPageEventArgs e)
        {
            SaveCurrentEmail();
        }


        private void btnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentEmail();
            LoadNewEmail(++currentIndex);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SaveCurrentEmail();
            LoadNewEmail(--currentIndex);
        }

        private void txtTo_Validating(object sender, CancelEventArgs e)
        {
            SetError(sender,
                     !App.ValidateEmailAddresses(txtTo.Text) ? "Invalid email address" : "");
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            AddAttachment();
        }

        #endregion
    }
}