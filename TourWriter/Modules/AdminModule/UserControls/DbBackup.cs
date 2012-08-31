using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using TourWriter.Info;

namespace TourWriter.Modules.AdminModule.UserControls
{
	/// <summary>
	/// Summary description for NewUser.
	/// </summary>
	public class DbBackup : UserControl
	{
	    private const string RestoreWarningMessage =
            "WARNING - current data will be lost if it is not in the restore file!\r\n\r\n" +
            " * A backup should be carried out first, after all users have saved changes.\r\n" +
            " * Restore will remove all current and unsaved data for all users.\r\n" +
	        " * All data will be completely reverted to date of the chosen backup file.\r\n" +
            " * All users must restart TourWriter to refresh screen data";

	    private const string RemoteDatabaseMessage =
	        "Your database appears to be an Online Hosted Database, in which case all database maintenance is handled by the provider.\r\n\r\n" +
            "For further information, please contact the provider.";

		#region Designer
		private Label lblHeading;
		private Label label6;
        private Infragistics.Win.Misc.UltraButton btnBackup;
		private Label label4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLastBackupDate;
		internal ImageList imageList1;
		private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLastBackupFile;
        private Label label2;
		private Infragistics.Win.Misc.UltraButton btnRestore;
        private Label label5;
        private Label label7;
        private Label label8;
        private Infragistics.Win.Misc.UltraButton btnBackupFile;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBackupFile;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAddExtension;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
		private IContainer components;

		public DbBackup()
		{
			InitializeComponent();
		}
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DbBackup));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnBackup = new Infragistics.Win.Misc.UltraButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLastBackupDate = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtLastBackupFile = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRestore = new Infragistics.Win.Misc.UltraButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnBackupFile = new Infragistics.Win.Misc.UltraButton();
            this.txtBackupFile = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkAddExtension = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastBackupDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastBackupFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackupFile)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 42;
            this.lblHeading.Text = "Database Management";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(480, 28);
            this.label6.TabIndex = 41;
            this.label6.Text = "Perform database backups, or restore a database from a backup.";
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(434, 97);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(68, 23);
            this.btnBackup.TabIndex = 38;
            this.btnBackup.Text = "Backup";
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 50;
            this.label4.Text = "Last backup date:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtLastBackupDate
            // 
            this.txtLastBackupDate.Location = new System.Drawing.Point(100, 100);
            this.txtLastBackupDate.Name = "txtLastBackupDate";
            this.txtLastBackupDate.ReadOnly = true;
            this.txtLastBackupDate.Size = new System.Drawing.Size(218, 21);
            this.txtLastBackupDate.TabIndex = 52;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Silver;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            this.imageList1.Images.SetKeyName(20, "");
            this.imageList1.Images.SetKeyName(21, "");
            this.imageList1.Images.SetKeyName(22, "");
            this.imageList1.Images.SetKeyName(23, "");
            this.imageList1.Images.SetKeyName(24, "");
            this.imageList1.Images.SetKeyName(25, "");
            this.imageList1.Images.SetKeyName(26, "");
            this.imageList1.Images.SetKeyName(27, "");
            this.imageList1.Images.SetKeyName(28, "");
            this.imageList1.Images.SetKeyName(29, "");
            this.imageList1.Images.SetKeyName(30, "");
            this.imageList1.Images.SetKeyName(31, "");
            this.imageList1.Images.SetKeyName(32, "");
            this.imageList1.Images.SetKeyName(33, "");
            this.imageList1.Images.SetKeyName(34, "");
            this.imageList1.Images.SetKeyName(35, "");
            this.imageList1.Images.SetKeyName(36, "");
            this.imageList1.Images.SetKeyName(37, "");
            this.imageList1.Images.SetKeyName(38, "");
            this.imageList1.Images.SetKeyName(39, "");
            this.imageList1.Images.SetKeyName(40, "");
            this.imageList1.Images.SetKeyName(41, "");
            // 
            // txtLastBackupFile
            // 
            this.txtLastBackupFile.Location = new System.Drawing.Point(100, 76);
            this.txtLastBackupFile.Name = "txtLastBackupFile";
            this.txtLastBackupFile.ReadOnly = true;
            this.txtLastBackupFile.Size = new System.Drawing.Size(392, 21);
            this.txtLastBackupFile.TabIndex = 56;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 55;
            this.label2.Text = "Last backup file:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(432, 130);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(68, 23);
            this.btnRestore.TabIndex = 58;
            this.btnRestore.Text = "Restore";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(18, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(416, 100);
            this.label5.TabIndex = 58;
            this.label5.Text = "(warning message set in Load() and shared with message box)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(181, 13);
            this.label7.TabIndex = 58;
            this.label7.Text = "Restore database from a backup file.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(146, 13);
            this.label8.TabIndex = 59;
            this.label8.Text = "Backup the database to a file";
            // 
            // btnBackupFile
            // 
            appearance1.Image = 16;
            this.btnBackupFile.Appearance = appearance1;
            this.btnBackupFile.ImageList = this.imageList1;
            this.btnBackupFile.Location = new System.Drawing.Point(476, 55);
            this.btnBackupFile.Name = "btnBackupFile";
            this.btnBackupFile.Size = new System.Drawing.Size(26, 23);
            this.btnBackupFile.TabIndex = 54;
            this.btnBackupFile.Click += new System.EventHandler(this.btnBackupFile_Click);
            // 
            // txtBackupFile
            // 
            this.txtBackupFile.Location = new System.Drawing.Point(34, 55);
            this.txtBackupFile.Name = "txtBackupFile";
            this.txtBackupFile.Size = new System.Drawing.Size(436, 21);
            this.txtBackupFile.TabIndex = 43;
            // 
            // chkAddExtension
            // 
            this.chkAddExtension.Checked = true;
            this.chkAddExtension.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddExtension.Location = new System.Drawing.Point(34, 82);
            this.chkAddExtension.Name = "chkAddExtension";
            this.chkAddExtension.Size = new System.Drawing.Size(156, 20);
            this.chkAddExtension.TabIndex = 49;
            this.chkAddExtension.Text = "Add date-time to filename";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.btnBackupFile);
            this.groupBox1.Controls.Add(this.chkAddExtension);
            this.groupBox1.Controls.Add(this.btnBackup);
            this.groupBox1.Controls.Add(this.txtBackupFile);
            this.groupBox1.Location = new System.Drawing.Point(11, 143);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 126);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database Backup";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRestore);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(11, 290);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(506, 159);
            this.groupBox2.TabIndex = 61;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Restore";
            // 
            // DbBackup
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtLastBackupFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLastBackupDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label6);
            this.Name = "DbBackup";
            this.Size = new System.Drawing.Size(520, 452);
            this.Load += new System.EventHandler(this.DbBackup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtLastBackupDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLastBackupFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackupFile)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		#endregion

		private ToolSet toolSet
		{
			get
			{
			    return ((AdminMain)Tag).ToolSet;
			}
		}

		private void DbBackup_Load(object sender, EventArgs e)
		{
		    label5.Text = RestoreWarningMessage;

			// prepare defaults
			if(toolSet.AppSettings.Rows.Count == 0)
				toolSet.AppSettings.AddAppSettingsRow(toolSet.AppSettings.NewAppSettingsRow());
			if(toolSet.AppSettings[0].IsLastDbBackupNameNull() || toolSet.AppSettings[0].LastDbBackupName == "")
                toolSet.AppSettings[0].LastDbBackupName = Path.Combine(Info.Services.DatabaseHelper.GetDefaultBackupDir(), "TourWriter.bak");	
	
			// set binding
			txtBackupFile.DataBindings.Add("Text", toolSet, "AppSettings.LastDbBackupName");	
			txtLastBackupFile.DataBindings.Add("Text", toolSet, "AppSettings.LastDbBackupFile");
			txtLastBackupDate.DataBindings.Add("Text", toolSet, "AppSettings.LastDbBackupDate");		

		}

        private void DoBackup(string backupfile)
        {
            Cursor = Cursors.WaitCursor;

            var processDate = DateTime.Now;
            if (chkAddExtension.Checked)
            {
                string dateInsert = processDate.ToString("_yyyy-MM-dd_H-mm-ss");

                if (backupfile.LastIndexOf(".") > -1)
                    backupfile = backupfile.Insert(backupfile.LastIndexOf("."), dateInsert);
                else
                    backupfile = backupfile + dateInsert;
            }

            Application.DoEvents();
            Info.Services.DatabaseHelper.Backup(backupfile);

            // save backup details
            txtLastBackupFile.Value = backupfile;
            txtLastBackupDate.Value = processDate;
            toolSet.AppSettings[0].LastDbBackupDate = processDate;
            toolSet.AppSettings[0].LastDbBackupFile = backupfile;
            toolSet.AppSettings[0].LastDbBackupName = txtBackupFile.Text;
            App.ShowInfo(App.GetResourceString("ShowDatabaseBackupSuccess"));
            Cursor = Cursors.Default;
        }
		
		private void btnBackupFile_Click(object sender, EventArgs e)
		{
			string saveToFileName = App.PromptSaveFile(txtBackupFile.Text, "Database Backup files", ".bak"); 
			if(saveToFileName != "")
				txtBackupFile.Text = saveToFileName;
		}		
		
		private void btnBackup_Click(object sender, EventArgs e)
        {
            if (App.DatabaseConnectionType == "remote") { App.ShowInfo(RemoteDatabaseMessage); return; }
		
			if(!App.IsValidFileName(txtBackupFile.Text))
			{
				App.ShowError("File name is not valid");
				return;
			}
			try
            {
                DoBackup(txtBackupFile.Text);
			}
			catch(Exception ex)
            {
                Cursor = Cursors.Default;

                App.Error(App.GetResourceString("ShowDatabaseBackupFailed"), ex);
                txtBackupFile.Text = Path.Combine(Info.Services.DatabaseHelper.GetDefaultBackupDir(), Path.GetFileName(txtBackupFile.Text));
                var msg = "Would you like to try to backup to the default backup location on the server?\r\n" + txtBackupFile.Text +
                             ".\r\n\r\nNOTE: this is the location on the database server, not your computer (unless it is the server).";
                if (App.AskYesNo(msg)) DoBackup(txtBackupFile.Text);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void btnRestore_Click(object sender, EventArgs e)
        {
            if (App.DatabaseConnectionType == "remote") { App.ShowInfo(RemoteDatabaseMessage); return; }

            string restorefile = App.SelectExternalFile(
                false, "Select backup to restore from", ".bak files (*.bak)|*.bak|All files (*.*)|*.*", 2);

            if (string.IsNullOrEmpty(restorefile))
                return;

		    bool doRestore =
		        MessageBox.Show(
                    RestoreWarningMessage + "\r\n\r\n" + "Are you sure you want to proceed?",
		            "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
		            MessageBoxDefaultButton.Button2)
		        == DialogResult.Yes;

            if (doRestore)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    Application.DoEvents();
                    Info.Services.DatabaseHelper.Restore(restorefile);

                    App.ShowInfo(
                        "Restore SUCCEEDED.\r\n\r\n" +
                        "Please ensure ALL USERS RESTART TourWriter to reset their application data.\r\n\r\n" +
                        "TourWriter will now close - you can restart immediatly");

                    // force immediate exit, don't allow user to save changes to new database
                    Application.Exit();
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    App.ShowError("Restore FAILED.\r\n\r\nClick OK to view error message");
                    App.Error(App.GetResourceString("ShowDatabaseRestoreFailed"), ex);
                    if (App.AskYesNo(
                        "It is recommended that you restart TourWriter to refresh the application data.\r\n\r\n" +
                        "Click YES to close TourWriter now."))
                    {
                        Application.Exit();
                        Environment.Exit(0);
                    }
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
		}
	}
}
