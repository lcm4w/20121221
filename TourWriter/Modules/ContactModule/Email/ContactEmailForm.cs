using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Properties;
using System.Data;

namespace TourWriter.Modules.ContactModule.Email
{
    public partial class ContactEmailForm : Form
    {
        private enum ActiveWindow
        {
            None,
            Template,
            Email,
            Send
        }

        private const string TemplateFormTitle = "Edit Template";
        private const string EmailFormTitle = "Edit Emails";
        private const string SendFormTitle = "Send Emails";

        private const string ButtonTextNext = "Next >";
        private const string ButtonTextSend = "Send";

        private readonly List<ContactEmailInfo> emailList;

        private TemplateForm _templateForm;
        private EmailForm _emailForm;
        private SendForm _sendForm;

        public ContactEmailForm(DataTable contactTable)
        {
            InitializeComponent();

            Size = Settings.Default.EmailEditorSize;
            Location = Settings.Default.EmailEditorLocation;
            if (!App.IsOnScreen(this)) Location = new System.Drawing.Point(10, 10);
            emailList = new List<ContactEmailInfo>();

            foreach (ContactSet.ContactRow row in contactTable.Rows)
            {
                emailList.Add(new ContactEmailInfo(row));
            }
        }

        private void ContactEmailForm_Load(object sender, EventArgs e)
        {
            SetActiveWindow(ActiveWindow.Template, ActiveWindow.None);
        }

        private TemplateForm templateForm
        {
            get
            {
                if (_templateForm == null)
                {
                    _templateForm = new TemplateForm();
                    AddControlToMainPanel(_templateForm);
                }
                return _templateForm;
            }
        }

        private EmailForm emailForm
        {
            get
            {
                if (_emailForm == null)
                {
                    _emailForm = new EmailForm(emailList);
                    AddControlToMainPanel(_emailForm);
                }
                return _emailForm;
            }
        }

        private SendForm sendForm
        {
            get
            {
                if (_sendForm == null)
                {
                    _sendForm = new SendForm(emailList);
                    _sendForm.SendCancelled += sendForm_SendCancelled;
                    AddControlToMainPanel(_sendForm);
                }
                return _sendForm;
            }
        }

        private ActiveWindow GetActiveWindow()
        {
            if (templateForm.Visible)
                return ActiveWindow.Template;

            else if (emailForm.Visible)
                return ActiveWindow.Email;

            else if (sendForm.Visible)
                return ActiveWindow.Send;

            return ActiveWindow.None;
        }

        private void SetActiveWindow(ActiveWindow newWindow, ActiveWindow prevWindow)
        {
            HideAllWindows();

            switch (newWindow)
            {
                case ActiveWindow.Template:
                    lblTitle.Text = TemplateFormTitle;
                    btnNextOrSend.Text = ButtonTextNext;
                    templateForm.Visible = true;
                    btnBack.Enabled = false;
                    btnNextOrSend.Enabled = true;
                    break;

                case ActiveWindow.Email:
                    if (prevWindow == ActiveWindow.Template)
                        BuildEmailList();
                    emailForm.ShowEmail(0);
                    lblTitle.Text = EmailFormTitle;
                    btnNextOrSend.Text = ButtonTextNext;
                    emailForm.Visible = true;
                    btnBack.Enabled = true;
                    btnNextOrSend.Enabled = true;
                    break;

                case ActiveWindow.Send:
                    lblTitle.Text = SendFormTitle;
                    btnNextOrSend.Text = ButtonTextSend;
                    sendForm.Visible = true;
                    btnBack.Enabled = true;
                    btnNextOrSend.Enabled = true;
                    break;

                default:
                    break;
            }
        }

        private void AddControlToMainPanel(Control control)
        {
            control.Dock = DockStyle.Fill;
            pnlMain.Controls.Add(control);
        }

        private void HideAllWindows()
        {
            templateForm.Visible = false;
            emailForm.Visible = false;
            sendForm.Visible = false;
        }

        private void SaveSettings()
        {
            TemplateSettings templateSettings = templateForm.GetTemplateSettings();
            Settings.Default.EmailerSaveWhenSent = templateSettings.SaveToFile;
            Settings.Default.EmailerBccSender = (templateSettings.Bcc.Length > 0);
            Settings.Default.BookingEmailShowPrice = templateSettings.ShowPrices;
            Settings.Default.EmailerReadReceipt = templateSettings.ReadReceipt;
            Settings.Default.Save();
        }

        private bool IsTemplateEmpty()
        {
            TemplateSettings templateSettings = templateForm.GetTemplateSettings();
            if (templateSettings.IsTemplateEmpty())
            {
                return !App.AskYesNo("No template selected. Do you still want to continue?");
            }
            return false;
        }

        public void BuildEmailList()
        {
            Cursor.Current = Cursors.WaitCursor;

            TemplateSettings templateSettings = templateForm.GetTemplateSettings();
            foreach (ContactEmailInfo emailInfo in emailList)
            {
                emailInfo.CreateEmailMessage(templateSettings);
            }

            Cursor.Current = Cursors.Default;
        }

        private void SendEmails()
        {
            btnBack.Enabled = false;
            btnNextOrSend.Enabled = false;

            sendForm.StartSendProcess();

            if (emailList.Count > 0)
            {
                btnBack.Enabled = true;
                btnNextOrSend.Enabled = true;
            }
            else
            {
                btnCancel.Text = "Close";
            }
        }

        private void sendForm_SendCancelled()
        {
            btnBack.Enabled = true;
            btnNextOrSend.Enabled = true;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            ActiveWindow currentWindow = GetActiveWindow();
            SetActiveWindow(currentWindow - 1, currentWindow);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (btnNextOrSend.Text != ButtonTextSend)
            {
                ActiveWindow currentWindow = GetActiveWindow();
                
                if (currentWindow != ActiveWindow.Template || !IsTemplateEmpty())
                    SetActiveWindow(currentWindow + 1, currentWindow);
            }
            else
            {
                SendEmails();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(emailList.Count == 0)
            {
                SaveSettings();
                Close();
            }
            else if (sendForm.IsSending)
            {
                sendForm.StopSendProcess();
            }
            else
            {
                if (App.AskYesNo("Click YES to cancel emails and close this window"))
                {
                    SaveSettings();
                    Close();
                }
            }
        }
        
        private void BookingEmailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.EmailEditorSize = 
                WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;
            Settings.Default.EmailEditorLocation = 
                WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;
            Settings.Default.Save();
            App.ClearBindings(this);
        }
    }
}
