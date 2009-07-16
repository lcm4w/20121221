using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Services;
using TourWriter.UserControls;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Modules.Emailer
{
    /// <summary>
    /// Control for sending emailList from an email list.
    /// Note: Thing to watch here is that the purchaseItemList (that email list is built from),
	/// and the email list, and the grid rows list all have to be kept synchronised.
	/// Eg. to delete an email, delete from purchaseItem list(index), email list (index), 
	/// and grid rows (where column EmailIndex == index).
    /// </summary>
    public partial class SendForm : WizardPage
    {
        private IList emailList
        {
            get { return (Wizard.Params as EmailBuilderBase).EmailList; }
        }

        private int currentIndex
        {
            get { return (Wizard.Params as EmailBuilderBase).CurrentIndex; }
        }

        private enum DisplayStatus
        {
            None,
            Sending,
            Sent,
            Stopped,
            Error
        }

        private bool cancelSendProcess = false;
        

        public SendForm()
        {
            InitializeComponent();

            BuildDataSource();
        }

        private void BuildDataSource()
        {
            gridSource.Band.Columns.Add("Status", typeof (String));
            gridSource.Band.Columns.Add("To", typeof (String));
            gridSource.Band.Columns.Add("Subject", typeof (String));
            gridSource.Band.Columns.Add("Error", typeof (String));
            gridSource.Band.Columns.Add("Email", typeof (EmailMessage));
        }

        private void LoadDisplayData()
        {
            gridSource.Rows.Clear();

            foreach (EmailMessage email in emailList)
                AddEmailToDisplayRow(email);

            grid.ActiveRow = null;
            foreach (UltraGridRow row in grid.Rows)
            {
                SetDisplayStatus(row, DisplayStatus.None);

                if (row.Cells["Email"].Value as EmailMessage ==
                    (Wizard.Params as EmailBuilderBase).CurrentEmail)
                {
                    row.Selected = true;
                    grid.ActiveRowScrollRegion.ScrollRowIntoView(row);
                }
            }
        }

        private void AddEmailToDisplayRow(EmailMessage email)
        {
            ResetEmailErrorMsg(email);

            UltraDataRow row = gridSource.Rows.Add(true);

            row["Status"] = "";
            row["To"] = email._To;
            row["Subject"] = email.Subject;
            row["Error"] = email._ErrorMsg;
            row["Email"] = email;
        }

        private void ResetEmailErrorMsg(EmailMessage email)
        {
            email._ErrorMsg = "";
            email._Status = EmailMessage.StatusType.None;
        }


        private void SendAll()
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                SendOne(row);
            }
        }

        private bool SendOne(UltraGridRow row)
        {
            EmailMessage email;
            row.Cells["Error"].Value = "";

            #region Sent

            email = row.Cells["Email"].Value as EmailMessage;
            if (email._Status == EmailMessage.StatusType.Sent)
            {
                SetDisplayStatus(row, DisplayStatus.Sent);
                return true;
            }

            #endregion

            #region Stopped

            if (cancelSendProcess)
            {
                SetDisplayStatus(row, DisplayStatus.Stopped);
                return false;
            }

            #endregion

            #region Validate

            grid.ActiveRowScrollRegion.ScrollRowIntoView(grid.Rows[row.Index]);
            if (!email.Validate())
            {
                SetDisplayStatus(row, DisplayStatus.Error);
                row.Cells["Error"].Value = email._ErrorMsg;
                return false;
            }

            #endregion

            #region Send

            SetDisplayStatus(row, DisplayStatus.Sending);

            var worker = new Sender_WorkerThread(
                (Wizard.Params as EmailBuilderBase), email);
            var thread = new Thread(worker.SendAndSave) {Name = "EmailSender_WorkerThread"};
            thread.Start();
            while (thread.IsAlive)
            {
                Application.DoEvents();
            }
            bool isSent = (email._Status == EmailMessage.StatusType.Sent);

            if (isSent)
            {
                SetDisplayStatus(row, DisplayStatus.Sent);
                return true;
            }
            else
            {
                SetDisplayStatus(row, DisplayStatus.Error);
                row.Cells["Error"].Value = email._ErrorMsg;
            }
            return false;

            #endregion
        }

        private void SetDisplayStatus(UltraGridRow row, DisplayStatus displayStatus)
        {
            string text = "";
            Image image = null;

            switch (displayStatus)
            {
                case DisplayStatus.None:
                    text = "";
                    image = Resources.Email;
                    break;
                case DisplayStatus.Sending:
                    text = "Sending...";
                    image = Resources.EmailGo;
                    break;
                case DisplayStatus.Sent:
                    text = "Sent";
                    image = Resources.Accept;
                    break;
                case DisplayStatus.Stopped:
                    text = "Stopped";
                    image = Resources.EmailError;
                    break;
                case DisplayStatus.Error:
                    text = "ERROR";
                    image = Resources.Cancel;
                    break;
            }
            SetDisplayStatus(row, text, image);
        }

        private void SetDisplayStatus(UltraGridRow row, string text, Image image)
        {
            grid.Rows[row.Index].Cells["Status"].Value = text;
            grid.Rows[row.Index].Cells["Status"].Appearance.Image = image;
            Application.DoEvents();
        }

        private void EnableAllButtons(bool isEnabled)
        {
            btnSendAll.Enabled = isEnabled;
            btnSendOne.Enabled = isEnabled;
            btnRemove.Enabled = isEnabled;
            Wizard.SetButtonsEnabled(isEnabled);
        }

        private void EnableSendButtons(bool isEnabled)
        {
            btnSendAll.Enabled = isEnabled;
            btnSendOne.Enabled = isEnabled;
        }

        private void RemoveEmail(UltraGridRow row)
        {
            (Wizard.Params as EmailBuilderBase).Remove(EmailFromGrid(row));
            row.Delete(false);
        }

        private void ClearDisplaySent()
        {
            for (int i = grid.Rows.Count - 1; i > -1; i--)
            {
                EmailMessage email = grid.Rows[i].Cells["Email"].Value as EmailMessage;

                if (email._Status == EmailMessage.StatusType.Sent)
                    RemoveEmail(grid.Rows[i]);
            }
        }

        private void ClearEmailListSent()
        {
            for (int i = emailList.Count - 1; i > -1; i--)
                if ((emailList[i] as EmailMessage)._Status == EmailMessage.StatusType.Sent)
                    (Wizard.Params as EmailBuilderBase).Remove(i);
        }

        private static EmailMessage EmailFromGrid(UltraGridRow row)
        {
            return row.Cells["Email"].Value as EmailMessage;
        }

        #region Events

        private void btnSendOne_Click(object sender, EventArgs e)
        {
            cancelSendProcess = false;
            try
            {
                EnableAllButtons(false);

                // send
                foreach (UltraGridRow row in grid.Selected.Rows)
                    SendOne(row);
                Thread.Sleep(700); // visual feedback
                ClearDisplaySent();

                // select next row
                if (currentIndex > -1)
                    grid.Rows[currentIndex].Selected = true;
            }
            finally
            {
                EnableAllButtons(true);
                EnableSendButtons(grid.Rows.Count > 0);
            }
        }

        private void btnSendAll_Click(object sender, EventArgs e)
        {
            cancelSendProcess = false;
            try
            {
                EnableAllButtons(false);
                btnSendAll.Visible = false;
                btnStop.Visible = true;

                SendAll();
                ClearDisplaySent();
            }
            finally
            {
                btnStop.Visible = false;
                btnSendAll.Visible = true;
                EnableAllButtons(true);
                EnableSendButtons(grid.Rows.Count > 0);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            cancelSendProcess = true;

            btnStop.Visible = false;
            btnSendAll.Visible = true;

            EnableSendButtons(emailList.Count > 0);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            grid.DeleteSelectedRows(true);

//			if(grid.ActiveRow == null)
//				return;
//
//			EmailMessage email = EmailFromGrid(grid.ActiveRow);
//
//			DialogResult result = MessageBox.Show(
//				String.Format("Remove the email to '{0}'?", email.To), 
//				App.MessageCaption,
//				MessageBoxButtons.OKCancel, 
//				MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
//			
//			if(result == DialogResult.OK)
//			{
//				this.RemoveEmail(grid.ActiveRow);
//			}			
        }


        private void SendForm_SetActive(object sender, CancelEventArgs e)
        {
            LoadDisplayData();
            EnableSendButtons(emailList.Count > 0);
        }

        private void SendForm_WizardFinish(object sender, CancelEventArgs e)
        {
            ClearEmailListSent();

            if ((Wizard.Params as EmailBuilderBase).QueryContinueToFinishEmails())
            {
                btnSendAll.PerformClick();
                e.Cancel = (emailList.Count != 0);
            }
            else
                e.Cancel = true;
        }

        private void SendForm_QueryCancel(object sender, CancelEventArgs e)
        {
            ClearEmailListSent();
            e.Cancel = !(Wizard.Params as EmailBuilderBase).QueryContinueToCancelEmails();
        }

        private void SendForm_WizardBack(object sender, WizardPageEventArgs e)
        {
            ClearEmailListSent();
        }


        private void grid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            //for(int i=e.Rows.Length-1; i>-1; i--)
            foreach (UltraGridRow row in e.Rows)
                (Wizard.Params as EmailBuilderBase).Remove(EmailFromGrid(row));
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
//			grid.DisplayLayout.Override.ActiveRowAppearance.BackColor = 
//				grid.DisplayLayout.Override.RowAppearance.BackColor; //System.Drawing.Color.AliceBlue;
//			grid.DisplayLayout.Override.ActiveRowAppearance.ForeColor =
//				System.Drawing.Color.Black; //grid.DisplayLayout.Override.RowAppearance.ForeColor;

            e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            e.Layout.Override.CellClickAction = CellClickAction.RowSelect;

            e.Layout.Bands[0].Columns["Email"].Hidden = true;

//			// groups and levels
//			e.Layout.Bands[0].Groups.Add();
//			grid.DisplayLayout.Bands[0].LevelCount = 2;
//
//			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
//			{
//				if(c.Key == "Status")
//				{
//					c.Group = e.Layout.Bands[0].Groups[0];
//					c.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
//				}
//				else if(c.Key == "To")
//				{
//					c.Group = e.Layout.Bands[0].Groups[0];
//					c.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
//				}	
//				else if(c.Key == "Subject")
//				{
//					c.Group = e.Layout.Bands[0].Groups[0];
//					c.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
//				}	
//				else if(c.Key == "Error")
//				{
//					c.Group = e.Layout.Bands[0].Groups[0];
//					c.Level = 1;
//					c.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
//				}
//				else
//				{
//					c.Hidden = true;
//				}
//			}
        }

        private void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() != typeof(UltraGridEmptyRow))
                Wizard.Back();
        }

        private void grid_AfterRowActivate(object sender, EventArgs e)
        {
            (Wizard.Params as EmailBuilderBase).CurrentEmail =
                grid.ActiveRow.Cells["Email"].Value as EmailMessage;
        }

        #endregion					

        public class Sender_WorkerThread
        {
            private EmailMessage email;
            private EmailBuilderBase emailBuilder;

            public Sender_WorkerThread(EmailBuilderBase emailBuilder, EmailMessage email)
            {
                this.email = email;
                this.emailBuilder = emailBuilder;
            }

            public void SendAndSave()
            {
                if (email._SaveWhenSent)
                {
                    if (email.Send())
                    {
                        string filename = email.SaveToDisk();
                        emailBuilder.RecordSavedEmail(email, filename);
                    }
                }
                else
                    email.Send();
            }
        }
    }
}