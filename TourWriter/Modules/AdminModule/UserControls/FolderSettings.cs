using System;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.Misc;
using TourWriter.Info;

namespace TourWriter.Modules.AdminModule.UserControls
{
    /// <summary>
    /// Summary description for NewUser.
    /// </summary>
    public class FolderSettings : UserControl
    {
        #region Designer

        private Label lblHeading;
        private Label label6;
        internal ImageList imageList1;
        private GroupBox groupBox2;
        private Label label3;
        private Label label5;
        private TextBox txtExternalFilesPath;
        private UltraButton btnExternalFilesPath;
        private DataGridView gridTemplates;
        private Label label1;
        private DataGridViewTextBoxColumn FileName;
        private DataGridViewTextBoxColumn TemplateName;
        private DataGridViewButtonColumn Edit;
        private Label lblErrorMsg;
        private IContainer components;

        public FolderSettings()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            EndAllEdits();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderSettings));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExternalFilesPath = new Infragistics.Win.Misc.UltraButton();
            this.label5 = new System.Windows.Forms.Label();
            this.txtExternalFilesPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gridTemplates = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TemplateName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.lblErrorMsg = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTemplates)).BeginInit();
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
            this.lblHeading.Text = "Folder Settings";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(480, 16);
            this.label6.TabIndex = 41;
            this.label6.Text = "Configure folders used by the TourWriter application.";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnExternalFilesPath);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtExternalFilesPath);
            this.groupBox2.Location = new System.Drawing.Point(3, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(500, 81);
            this.groupBox2.TabIndex = 63;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Default file location folders";
            // 
            // btnExternalFilesPath
            // 
            appearance2.Image = 16;
            this.btnExternalFilesPath.Appearance = appearance2;
            this.btnExternalFilesPath.ImageList = this.imageList1;
            this.btnExternalFilesPath.Location = new System.Drawing.Point(463, 45);
            this.btnExternalFilesPath.Name = "btnExternalFilesPath";
            this.btnExternalFilesPath.Size = new System.Drawing.Size(26, 23);
            this.btnExternalFilesPath.TabIndex = 69;
            this.btnExternalFilesPath.Click += new System.EventHandler(this.btnExternalFilesPath_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(374, 13);
            this.label5.TabIndex = 67;
            this.label5.Text = "External files, base location for all content/files outside of TourWriter databas" +
                "e";
            // 
            // txtExternalFilesPath
            // 
            this.txtExternalFilesPath.Location = new System.Drawing.Point(24, 46);
            this.txtExternalFilesPath.Name = "txtExternalFilesPath";
            this.txtExternalFilesPath.Size = new System.Drawing.Size(439, 20);
            this.txtExternalFilesPath.TabIndex = 66;
            this.txtExternalFilesPath.Validated += new System.EventHandler(this.txtExternalFilesPath_Validated);
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label3.Location = new System.Drawing.Point(12, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(480, 16);
            this.label3.TabIndex = 64;
            this.label3.Text = "NOTE: Network folder paths should be used if TourWriter is running in a network e" +
                "nvironment.";
            // 
            // gridTemplates
            // 
            this.gridTemplates.AllowUserToAddRows = false;
            this.gridTemplates.AllowUserToDeleteRows = false;
            this.gridTemplates.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridTemplates.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gridTemplates.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridTemplates.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridTemplates.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName,
            this.TemplateName,
            this.Edit});
            this.gridTemplates.Location = new System.Drawing.Point(3, 193);
            this.gridTemplates.Name = "gridTemplates";
            this.gridTemplates.Size = new System.Drawing.Size(500, 140);
            this.gridTemplates.TabIndex = 65;
            this.gridTemplates.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridTemplates_CellDoubleClick);
            this.gridTemplates.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridTemplates_CellClick);
            // 
            // FileName
            // 
            this.FileName.HeaderText = "FileName";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Visible = false;
            // 
            // TemplateName
            // 
            this.TemplateName.FillWeight = 184.7716F;
            this.TemplateName.HeaderText = "Template name";
            this.TemplateName.Name = "TemplateName";
            this.TemplateName.ReadOnly = true;
            // 
            // Edit
            // 
            this.Edit.FillWeight = 15.22843F;
            this.Edit.HeaderText = "Edit";
            this.Edit.Name = "Edit";
            this.Edit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Edit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Edit.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 66;
            this.label1.Text = "Templates";
            // 
            // lblErrorMsg
            // 
            this.lblErrorMsg.BackColor = System.Drawing.SystemColors.Window;
            this.lblErrorMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorMsg.ForeColor = System.Drawing.Color.Red;
            this.lblErrorMsg.Location = new System.Drawing.Point(103, 250);
            this.lblErrorMsg.Name = "lblErrorMsg";
            this.lblErrorMsg.Size = new System.Drawing.Size(286, 41);
            this.lblErrorMsg.TabIndex = 67;
            this.lblErrorMsg.Text = "Templates folder not found, please ensure the default folder above is set up corr" +
                "ectly.\r\n";
            this.lblErrorMsg.Visible = false;
            // 
            // FolderSettings
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.lblErrorMsg);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gridTemplates);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label6);
            this.Name = "FolderSettings";
            this.Size = new System.Drawing.Size(520, 363);
            this.Load += new System.EventHandler(this.FolderSettings_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTemplates)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #endregion

        private ToolSet toolSet
        {
            get { return (Tag as AdminMain).ToolSet; }
        }


        private void FolderSettings_Load(object sender, EventArgs e)
        {
            txtExternalFilesPath.DataBindings.Add("Text", toolSet, "AppSettings.ExternalFilesPath");

            InitializeTemplatesGrid();
        }

        private void InitializeTemplatesGrid()
        {
            gridTemplates.Rows.Clear();

            DirectoryInfo templateDir = new DirectoryInfo(Services.ExternalFilesHelper.GetTemplateFolder());
            FileInfo[] fileList;

            try
            {
                fileList = templateDir.GetFiles();
                lblErrorMsg.Visible = false;
            }
            catch (DirectoryNotFoundException)
            {
                lblErrorMsg.Visible = true;
                return;
            }

            foreach (FileInfo file in fileList)
            {
                string templateName;

                // convert the filename into a pretty name
                switch (file.Name)
                {
                    case "TourWriter.Email.BookingRequest.html":
                        templateName = "Email - Booking Request";
                        break;
                    case "TourWriter.Reports.Itinerary.ClientFinalPricing.txt":
                        templateName = "Reports - Client Final Pricing";
                        break;
                    case "TourWriter.Reports.Itinerary.ClientFullPricing.txt":
                        templateName = "Reports - Client Full Pricing";
                        break;
                    case "TourWriter.Reports.Itinerary.Voucher.txt":
                        templateName = "Reports - Voucher";
                        break;
                    default:
                        templateName = String.Empty;
                        break;
                }

                // ignore the files that aren't listed above
                if (templateName.Length == 0)
                    continue;

                // add the row to the grid
                gridTemplates.Rows.Add(new object[] { file.FullName, templateName, "..." });
            }
        }

        private static void EditTemplate(string fileName)
        {
            Process.Start("notepad.exe", fileName);
        }

        private void btnExternalFilesPath_Click(object sender, EventArgs e)
        {
            string path = App.PromptChooseDir(txtExternalFilesPath.Text);
            if (path.Length > 0 && path != txtExternalFilesPath.Text)
            {
                txtExternalFilesPath.Text = path;
                toolSet.AppSettings[0].ExternalFilesPath = path;
                InitializeTemplatesGrid();
            }
        }

        private void gridTemplates_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            DataGridViewRow row = gridTemplates.Rows[e.RowIndex];

            if (gridTemplates.Columns[e.ColumnIndex].Name == "Edit")
            {
                EditTemplate(row.Cells["FileName"].Value.ToString());
            }
        }

        private void gridTemplates_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            DataGridViewRow row = gridTemplates.Rows[e.RowIndex];

            if (gridTemplates.Columns[e.ColumnIndex].Name == "TemplateName")
            {
                EditTemplate(row.Cells["FileName"].Value.ToString());
            }
        }

        private void EndAllEdits()
        {
            Validate();
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            EndAllEdits();
            base.OnValidating(e);
        }

        private void txtExternalFilesPath_Validated(object sender, EventArgs e)
        {
            InitializeTemplatesGrid();
        }
    }
}