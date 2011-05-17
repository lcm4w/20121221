using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Properties;

namespace TourWriter.Modules.ItineraryModule.Bookings.Email
{
    public partial class BookingEmailForm : Form
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

        private readonly ItinerarySet itinerarySet;
        private readonly List<BookingEmailInfo> emailList;

        private IEnumerable<int> purchaseLineIdList;

        private string _templateFile;
        private TemplateForm _templateForm;
        private EmailForm _emailForm;
        private SendForm _sendForm;


        public BookingEmailForm(ItinerarySet itinerarySet, IEnumerable<int> purchaseLineIdList)
            : this (itinerarySet, purchaseLineIdList, "") { }

        public BookingEmailForm(ItinerarySet itinerarySet, IEnumerable<int> purchaseLineIdList, string defaultTemplate)
        {
            InitializeComponent();
            Size = Settings.Default.EmailEditorSize;
            Location = Settings.Default.EmailEditorLocation;
            if (!App.IsOnScreen(this)) Location = new System.Drawing.Point(10, 10);

            this.itinerarySet = itinerarySet;
            _templateFile = defaultTemplate;
            emailList = new List<BookingEmailInfo>();
            this.purchaseLineIdList = purchaseLineIdList;           
        }

        private void BookingEmailForm_Load(object sender, EventArgs e)
        {
            btnNextOrSend.Select();
            SetActiveWindow(ActiveWindow.Template, ActiveWindow.None);
            if (_templateForm.SkipTemplate) btnNextOrSend.PerformClick();
        }

        private TemplateForm templateForm
        {
            get
            {
                if (_templateForm == null)
                {
                    _templateForm = new TemplateForm(itinerarySet.Itinerary[0].ItineraryName, _templateFile);
                    _templateForm.SkipTemplate = Settings.Default.EmailEditorSkipTemplate;
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
                    _sendForm = new SendForm(itinerarySet, emailList);
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
                    if(prevWindow == ActiveWindow.Template)
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
            Settings.Default.EmailEditorSkipTemplate = _templateForm.SkipTemplate;
            Settings.Default.Save();
        }

        public void BuildEmailList()
        {
            Cursor.Current = Cursors.WaitCursor;
            emailList.Clear();            
            if (_templateForm.GroupBySupplierEmail)
            {
                var purchaseLineList = itinerarySet.PurchaseLine.Where(x => purchaseLineIdList.Contains(x.PurchaseLineID));

                var supplierList = itinerarySet.SupplierLookup.Where(x => !x.IsEmailNull()).ToList();

                var purchaseLineGroupByEmail = from p in purchaseLineList
                                               join sup in supplierList on p.SupplierID equals sup.SupplierID
                                               group p by sup.Email
                                                   into purchaseGroup
                                                   select purchaseGroup;

                foreach (var purchaseGroup in purchaseLineGroupByEmail)
                {
                    emailList.Add(new BookingEmailInfo(purchaseGroup));
                }
            }
            else
            {
                foreach (var row in purchaseLineIdList.Select(id => itinerarySet.PurchaseLine.FindByPurchaseLineID(id)))
                {
                    emailList.Add(new BookingEmailInfo(new List<ItinerarySet.PurchaseLineRow>() { row }));
                }
            }                              
            
            var templateSettings = templateForm.GetTemplateSettings();
           
            foreach (var emailInfo in emailList)
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
                SetActiveWindow(currentWindow + 1, currentWindow);
            }
            else
            {
                SendEmails();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (sendForm.IsSending)
            {
                sendForm.StopSendProcess();
            }
            else
            {
                SaveSettings();
                Close();
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
