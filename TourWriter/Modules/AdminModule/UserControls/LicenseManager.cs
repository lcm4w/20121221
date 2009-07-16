using System;
using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using TourWriter.Utilities.Xml;
using License=TourWriter.Info.License;

namespace TourWriter.Modules.AdminModule.UserControls
{
    /// <summary>
    /// Summary description for NewUser.
    /// </summary>
    public class LicenseManager : UserControl
    {
        #region Designer

        private Label lblHeading;
        private Label label6;
        internal ImageList imageList1;
        private LinkLabel lnkInfoUrl;
        private Label label2;
        private Label label3;
        private LinkLabel lnkPurchaseUrl;
        private UltraTextEditor txtLicense;
        private Button btnLoadNewLicense;
        private Label label1;
        private IContainer components;

        public LicenseManager()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseManager));
            this.txtLicense = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblHeading = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lnkInfoUrl = new System.Windows.Forms.LinkLabel();
            this.lnkPurchaseUrl = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLoadNewLicense = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicense)).BeginInit();
            this.SuspendLayout();
            // 
            // txtLicense
            // 
            this.txtLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicense.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLicense.Location = new System.Drawing.Point(15, 176);
            this.txtLicense.Multiline = true;
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.ReadOnly = true;
            this.txtLicense.Scrollbars = System.Windows.Forms.ScrollBars.Both;
            this.txtLicense.Size = new System.Drawing.Size(476, 217);
            this.txtLicense.TabIndex = 60;
            this.txtLicense.WordWrap = false;
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
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 42;
            this.lblHeading.Text = "TourWriter Licensing";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(480, 16);
            this.label6.TabIndex = 41;
            // 
            // lnkInfoUrl
            // 
            this.lnkInfoUrl.Location = new System.Drawing.Point(136, 52);
            this.lnkInfoUrl.Name = "lnkInfoUrl";
            this.lnkInfoUrl.Size = new System.Drawing.Size(372, 23);
            this.lnkInfoUrl.TabIndex = 56;
            this.lnkInfoUrl.TabStop = true;
            this.lnkInfoUrl.Text = "http://www.tourwriter.com";
            this.lnkInfoUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkInfoUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkInfoUrl_LinkClicked);
            // 
            // lnkPurchaseUrl
            // 
            this.lnkPurchaseUrl.Location = new System.Drawing.Point(136, 76);
            this.lnkPurchaseUrl.Name = "lnkPurchaseUrl";
            this.lnkPurchaseUrl.Size = new System.Drawing.Size(372, 23);
            this.lnkPurchaseUrl.TabIndex = 57;
            this.lnkPurchaseUrl.TabStop = true;
            this.lnkPurchaseUrl.Text = "http://www.tourwriter.com";
            this.lnkPurchaseUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkPurchaseUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPurchaseUrl_LinkClicked);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 23);
            this.label2.TabIndex = 58;
            this.label2.Text = "TourWriter information:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 23);
            this.label3.TabIndex = 59;
            this.label3.Text = "Purchase license:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLoadNewLicense
            // 
            this.btnLoadNewLicense.Location = new System.Drawing.Point(15, 117);
            this.btnLoadNewLicense.Name = "btnLoadNewLicense";
            this.btnLoadNewLicense.Size = new System.Drawing.Size(105, 23);
            this.btnLoadNewLicense.TabIndex = 61;
            this.btnLoadNewLicense.Text = "Load new license";
            this.btnLoadNewLicense.UseVisualStyleBackColor = true;
            this.btnLoadNewLicense.Click += new System.EventHandler(this.btnLoadNewLicense_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 62;
            this.label1.Text = "Current license file details";
            // 
            // LicenseManager
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLoadNewLicense);
            this.Controls.Add(this.txtLicense);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lnkPurchaseUrl);
            this.Controls.Add(this.lnkInfoUrl);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label6);
            this.Name = "LicenseManager";
            this.Size = new System.Drawing.Size(466, 404);
            this.Load += new System.EventHandler(this.LicenseManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtLicense)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #endregion


        private void LicenseManager_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        private void DataBind()
        {
            // show license file
            License license = new License();
            license.LoadFromDatabase();
            DisplayLicenseFile(license);
        }

        private void DisplayLicenseFile(License license)
        {
            lnkInfoUrl.Text = license.InfoURL;
            lnkPurchaseUrl.Text = license.PurchaseURL;
            txtLicense.Text = license.ToString();
        }

        private void btnLoadNewLicense_Click(object sender, EventArgs e)
        {
            string file = App.SelectExternalFile(
                   false, "Select license file", "TourWriterKey.txt", ".txt files (*.txt)|*.txt|All files (*.*)|*.*", 1);

            if (file != null)
            {

                Cursor = Cursors.WaitCursor;

                try
                {
                    if (XmlHelper.VerifySignedXmlFile(file))
                    {
                        License license = SaveNewLicenseFromFile(file);
                        DisplayLicenseFile(license);
                    }
                    else
                    {
                        App.ShowError("License file is not valid.");
                    }
                }
                catch (Exception)
                {
                    App.ShowError("License file is not valid.");
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private static License SaveNewLicenseFromFile(string fileName)
        {
            License license = new License();
            license.LoadFromFile(fileName);
            license.SaveToDatabase();
            return license;
        }

        private void lnkInfoUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            App.OpenUrl(lnkInfoUrl.Text);
        }

        private void lnkPurchaseUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            App.OpenUrl(lnkPurchaseUrl.Text);
        }
    }
}