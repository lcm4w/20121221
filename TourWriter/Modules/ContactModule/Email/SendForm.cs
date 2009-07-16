using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Services;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Modules.ContactModule.Email
{
    /// <summary>
    /// Control for sending emailList from an email list.
    /// </summary>
    public partial class SendForm : UserControl
    {
        public delegate void OnSendCancelled();
        public OnSendCancelled SendCancelled;

        private readonly List<ContactEmailInfo> emailList;
        private bool cancelSendProcess = false;
        bool isSending = false;

        public SendForm(List<ContactEmailInfo> emailList)
        {
            InitializeComponent();

            this.emailList = emailList;

            grid.DataSource = null;
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
            ContactEmailInfo emailInfo = 
                (ContactEmailInfo)gridRow.Cells["BookingEmailInfo"].Value;

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

            var worker = new Sender_WorkerThread(emailInfo.EmailMessage);
            var thread = new Thread(worker.SendAndSave) {Name = "EmailSender_WorkerThread"};
            thread.Start();
            while (thread.IsAlive)
            {
                Application.DoEvents();
            }
            bool isSent = (emailInfo.EmailMessage._Status == EmailMessage.StatusType.Sent);

            if (isSent)
            {
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
            gridSource.Band.Columns.Add("Error", typeof (string));
            gridSource.Band.Columns.Add("BookingEmailInfo", typeof (ContactEmailInfo));

            gridSource.Rows.Clear();
            gridSource.Rows.SetCount(emailList.Count);

            grid.DataSource = gridSource;
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

        private void SetSelectedRows(bool selectAllRows)
        {
            if (chkSelectAll.Checked != selectAllRows)
                chkSelectAll.Checked = selectAllRows;

            else
            {
                foreach (UltraGridRow row in grid.Rows)
                    row.Cells["Selected"].Value = selectAllRows;
            }
        }

        private void ClearSendEmails()
        {
            for (int i = grid.Rows.Count - 1; i > -1; i--)
            {
                EmailMessage email =
                    ((ContactEmailInfo)grid.Rows[i].Cells["BookingEmailInfo"].Value).EmailMessage;

                if (email._Status == EmailMessage.StatusType.Sent)
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
                emailList.Remove((ContactEmailInfo)row.Cells["BookingEmailInfo"].Value);
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["BookingEmailInfo"].Hidden = true;

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
            private readonly EmailMessage email;

            public Sender_WorkerThread(EmailMessage email)
            {
                this.email = email;
            }

            public void SendAndSave()
            {
                email.Send();
            }
        }
    }
}