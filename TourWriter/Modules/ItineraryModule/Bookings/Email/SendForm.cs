using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Modules.ItineraryModule.Bookings.Email
{
    /// <summary>
    /// Control for sending emailList from an email list.
    /// </summary>
    public partial class SendForm : UserControl
    {
        public delegate void OnSendCancelled();
        public OnSendCancelled SendCancelled;

        private readonly ItinerarySet itinerarySet;
        private readonly List<BookingEmailInfo> emailList;
        private bool cancelSendProcess = false;
        bool isSending = false;

        public SendForm(ItinerarySet itinerarySet, List<BookingEmailInfo> emailList)
        {
            InitializeComponent();

            this.itinerarySet = itinerarySet;
            this.emailList = emailList;

            grid.DataSource = null;

            cmbBookingStatus_Initialize();
        }

        public bool IsSending
        {
            get { return isSending; }
        }

        public void StartSendProcess()
        {
            isSending = true;
            cancelSendProcess = false;

            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Cells["Selected"].Value != DBNull.Value && (bool)row.Cells["Selected"].Value)
                    SendOne(row);
            }
            Thread.Sleep(700); // to provide visual feedback
            ClearSendEmails();
            isSending = false;
        }

        public void StopSendProcess()
        {
            cancelSendProcess = true;
        }

        private void SendOne(UltraGridRow gridRow)
        {
            BookingEmailInfo emailInfo = 
                (BookingEmailInfo)gridRow.Cells["BookingEmailInfo"].Value;

            gridRow.Cells["Error"].Value = String.Empty;

            #region Sent

            if (emailInfo.EmailMessage._Status == EmailMessage.StatusType.Sent)
            {
                SetDisplayStatus(gridRow, emailInfo.EmailMessage._Status);
                return;
            }

            #endregion

            #region Stopped

            if (cancelSendProcess)
            {
                emailInfo.EmailMessage._Status = EmailMessage.StatusType.Stopped;
                SetDisplayStatus(gridRow, emailInfo.EmailMessage._Status);
                
                if (SendCancelled != null)
                    SendCancelled();
                
                return;
            }

            #endregion

            #region Validate

            grid.ActiveRowScrollRegion.ScrollRowIntoView(grid.Rows[gridRow.Index]);
            if (!emailInfo.EmailMessage.Validate())
            {
                emailInfo.EmailMessage._Status = EmailMessage.StatusType.Error;
                SetDisplayStatus(gridRow, emailInfo.EmailMessage._Status);
                gridRow.Cells["Error"].Value = emailInfo.EmailMessage._ErrorMsg;
                return;
            }

            #endregion

            #region Send

            emailInfo.EmailMessage._Status = EmailMessage.StatusType.Sending;
            SetDisplayStatus(gridRow, emailInfo.EmailMessage._Status);

            var worker = new Sender_WorkerThread(this, emailInfo.EmailMessage);
            var thread = new Thread(worker.SendAndSave) {Name = "EmailSender_WorkerThread"};
            thread.Start();
            while (thread.IsAlive)
            {
                Application.DoEvents();
            }
            bool isSent = (emailInfo.EmailMessage._Status == EmailMessage.StatusType.Sent);

            if (isSent)
            {
                if ((int)cmbBookingStatus.SelectedValue > -1)
                {
                    // set the booking status of the purchase items for this booking
                    foreach (ItinerarySet.PurchaseItemRow item in emailInfo.PurchaseLine.GetPurchaseItemRows())
                        item.RequestStatusID = (int) cmbBookingStatus.SelectedValue;
                }
                SetDisplayStatus(gridRow, emailInfo.EmailMessage._Status);
                return;
            }
            else
            {
                emailInfo.EmailMessage._Status = EmailMessage.StatusType.Error;
                SetDisplayStatus(gridRow, emailInfo.EmailMessage._Status);
                gridRow.Cells["Error"].Value = emailInfo.EmailMessage._ErrorMsg;
            }

            #endregion
        }

        private void FormShown()
        {
            if (grid.DataSource == null)
            {
                InitializeDataSource();
                SetSelectedRows(true);
            }
            else
            {
                grid.Rows.Refresh(RefreshRow.FireInitializeRow);
            }
        }

        private void InitializeDataSource()
        {
            gridSource.Band.Columns.Add("Selected", typeof (bool));
            gridSource.Band.Columns.Add("Status", typeof (string));
            gridSource.Band.Columns.Add("To", typeof (string));
            gridSource.Band.Columns.Add("Subject", typeof (string));
            gridSource.Band.Columns.Add("Attachment", typeof(Bitmap));
            gridSource.Band.Columns.Add("Error", typeof (string));
            gridSource.Band.Columns.Add("BookingEmailInfo", typeof (BookingEmailInfo));

            gridSource.Rows.Clear();
            gridSource.Rows.SetCount(emailList.Count);

            grid.DataSource = gridSource;
        }

        private void cmbBookingStatus_Initialize()
        {
            // Initialise combobox to include null value member.
            ToolSet.RequestStatusDataTable table = Cache.ToolSet.RequestStatus.Copy() as ToolSet.RequestStatusDataTable;
            if (table != null)
            {
                ToolSet.RequestStatusRow row = table.NewRequestStatusRow();
                row.RequestStatusName = "<none>";
                table.Rows.InsertAt(row, 0);
                cmbBookingStatus.DataSource = table;
                cmbBookingStatus.DisplayMember = table.RequestStatusNameColumn.ColumnName;
                cmbBookingStatus.ValueMember = table.RequestStatusIDColumn.ColumnName;
            }
        }

        private static void SetDisplayStatus(UltraGridRow row, EmailMessage.StatusType displayStatus)
        {
            string text = String.Empty;
            Image image = null;

            switch (displayStatus)
            {
                case EmailMessage.StatusType.None:
                    text = String.Empty;
                    image = Resources.Email;
                    break;
                case EmailMessage.StatusType.Sending:
                    text = "Sending...";
                    image = Resources.EmailGo;
                    break;
                case EmailMessage.StatusType.Sent:
                    text = "Sent";
                    image = Resources.Accept;
                    break;
                case EmailMessage.StatusType.Stopped:
                    text = "Stopped";
                    image = Resources.EmailError;
                    break;
                case EmailMessage.StatusType.Error:
                    text = "ERROR";
                    image = Resources.Cancel;
                    break;
            }
            SetDisplayStatus(row, text, image);
        }

        private static void SetDisplayStatus(UltraGridRow row, string text, Image image)
        {
            row.Cells["Status"].Value = text;
            row.Cells["Status"].Appearance.Image = image;
            Application.DoEvents();
        }
            
        private void RecordSavedEmail(EmailMessage email, string savedFilename)
        {
            savedFilename = ExternalFilesHelper.ConvertToRelativePath(savedFilename);

            // record the email in the message table
            ItinerarySet.MessageRow messageRow = itinerarySet.Message.NewMessageRow();
            messageRow.MessageType = "Email";
            messageRow.MessageTo = email._To;
            messageRow.MessageFrom = email._From;
            messageRow.MessageName =
                (email._Tag != null ? ((BookingEmailInfo)email._Tag).SupplierName + ": " : "") + email.Subject;
            messageRow.MessageFile = savedFilename;
            messageRow.AddedOn = DateTime.Now;
            messageRow.AddedBy = Cache.User.UserID;
            itinerarySet.Message.AddMessageRow(messageRow);

            // join the ItineraryMessage table
            itinerarySet.ItineraryMessage.AddItineraryMessageRow(
                itinerarySet.Itinerary[0], messageRow, DateTime.Now, Cache.User.UserID);

            // join the SupplierMessage table
            if (email._Tag != null)
            {
                int supplierId = ((BookingEmailInfo)email._Tag).SupplierId;
                itinerarySet.SupplierMessage.AddSupplierMessageRow(
                    messageRow, supplierId, DateTime.Now, Cache.User.UserID);
            }
        }

        private void SetSelectedRows(bool selectAllRows)
        {
            if (chkSelectAll.Checked != selectAllRows)
                chkSelectAll.Checked = selectAllRows;

            else
            {
                try
                {
                    foreach (UltraGridRow row in grid.Rows)
                        row.Cells["Selected"].Value = selectAllRows;
                }
                catch(InvalidOperationException) 
                { /* swallow Infragistics.Shared.SparseArray.CreateItemEnumerator.EnsureNotExhausted() exception,
                   * not sure where comming from, maybe all operations are on grid rows rather than datasource rows */ }
            }
        }

        private void ClearSendEmails()
        {
            for (var i = grid.Rows.Count - 1; i > -1; i--)
            {
                var o = grid.Rows[i].Cells["BookingEmailInfo"].Value;
                if (o == null || ((BookingEmailInfo)o).EmailMessage == null) continue;

                if (((BookingEmailInfo)o).EmailMessage._Status == EmailMessage.StatusType.Sent)
                {
                    grid.Rows[i].Delete(false);
                }
            }
        }

        private void chkSelectAll_Reposition()
        {
            int x = grid.Location.X + (20 - (chkSelectAll.Size.Width / 2));
            int y = grid.Location.Y + 6;
            chkSelectAll.Location = new Point(x, y);
        }



        #region Events

        private void grid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            foreach (UltraGridRow row in e.Rows)
                emailList.Remove((BookingEmailInfo)row.Cells["BookingEmailInfo"].Value);
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["BookingEmailInfo"].Hidden = true;

            e.Layout.Bands[0].Columns["Status"].CellAppearance.ImageHAlign = HAlign.Center;

            e.Layout.Bands[0].Columns["Attachment"].Header.Caption = "Attach";
            e.Layout.Bands[0].Columns["Attachment"].MinWidth = 50;
            e.Layout.Bands[0].Columns["Attachment"].MaxWidth = 50;
            e.Layout.Bands[0].Columns["Attachment"].CellAppearance.ImageHAlign = HAlign.Center;

            e.Layout.Bands[0].Columns["Selected"].Header.Caption = String.Empty;
            e.Layout.Bands[0].Columns["Selected"].MinWidth = 40;
            e.Layout.Bands[0].Columns["Selected"].MaxWidth = 40;
            e.Layout.Bands[0].Columns["Selected"].Header.ToolTipText =
                "Click the check box  to select/unselect all items"; // weird extra space required

            e.Layout.Bands[0].Columns["Status"].Width = 50;

            GridHelper.SetDefaultGridAppearance(e);

            // override default appearances
            e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
            e.Layout.Override.ActiveRowAppearance.BackColor = e.Layout.Override.RowAppearance.BackColor;
            e.Layout.Override.ActiveRowAppearance.ForeColor = e.Layout.Override.RowAppearance.ForeColor;
            e.Layout.Override.SelectedRowAppearance.BackColor = e.Layout.Override.RowAppearance.BackColor;
            e.Layout.Override.SelectedRowAppearance.ForeColor = e.Layout.Override.RowAppearance.ForeColor;
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

            chkSelectAll_Reposition();
        }

        private void grid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            EmailMessage email = emailList[e.Row.Index].EmailMessage;

            e.Row.Cells["To"].Value = email._To;
            e.Row.Cells["Subject"].Value = email.Subject;
            e.Row.Cells["Error"].Value = email._ErrorMsg;
            e.Row.Cells["BookingEmailInfo"].Value = emailList[e.Row.Index];

            if (email.Attachments.Count > 0)
            {
                string toolTip = String.Empty;
                foreach (System.Net.Mail.Attachment att in email.Attachments)
                    toolTip += att.Name + ",";
                toolTip = toolTip.TrimEnd(',');

                e.Row.Cells["Attachment"].ToolTipText = toolTip;
                e.Row.Cells["Attachment"].Value = Resources.Attach;
            }
            else
            {
                e.Row.Cells["Attachment"].ToolTipText = String.Empty;
                e.Row.Cells["Attachment"].Value = DBNull.Value;
            }

            e.Row.Selected = false;

            SetDisplayStatus(e.Row, email._Status);
        }

        private void grid_MouseClick(object sender, MouseEventArgs e)
        {
            UltraGridRow row = GridHelper.GetValidClickRow(grid);
            if (row != null)
            {
                row.Cells["Selected"].Value =
                    (row.Cells["Selected"].Value == DBNull.Value) ? true : !(bool)row.Cells["Selected"].Value;
            }
        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            SetSelectedRows(chkSelectAll.Checked);
        }

        private void SendForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                FormShown();
        }

        #endregion

        public class Sender_WorkerThread
        {
            private readonly SendForm sendForm;
            private readonly EmailMessage email;

            public Sender_WorkerThread(SendForm sendForm, EmailMessage email)
            {
                this.email = email;
                this.sendForm = sendForm;
            }

            public void SendAndSave()
            {
                if (email._SaveWhenSent)
                {
                    if (email.Send())
                    {
                        var emailStore = ExternalFilesHelper.GetEmailFolder();
                        try
                        {
                            System.IO.Directory.CreateDirectory(emailStore);
                            string filename = email.SaveToDisk(emailStore);
                            sendForm.RecordSavedEmail(email, filename);
                        } catch { }
                    }
                }
                else
                    email.Send();
            }
        }
    }
}