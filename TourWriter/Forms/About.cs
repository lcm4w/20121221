using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TourWriter.Global;
using TourWriter.Properties;

namespace TourWriter.Forms
{
	/// <summary>
	/// Login and application start form for TourWriter application.
	/// </summary>
	public class About : Form
	{
		#region Designer

        private System.Windows.Forms.ErrorProvider error;
		internal System.Windows.Forms.Label lblCodebase;
		internal System.Windows.Forms.Label lblCopyright;
		internal System.Windows.Forms.Label lblDescription;
		internal System.Windows.Forms.Label lblAppVersion;
		internal System.Windows.Forms.Label lblTitle;
		internal System.Windows.Forms.Label label1;
		internal System.Windows.Forms.Label label2;
		internal System.Windows.Forms.Label label3;
		internal System.Windows.Forms.Label label4;
		internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        internal Label lblDbVersion;
        internal Label label6;
        internal Label label7;
        internal Label lblInstallId;
        private Button btnOK;
        private ToolTip toolTip1;
        internal Label lblOsVersion;
        internal Label label9;
        internal Label lblDotNetVersion;
        internal Label label11;
        private LinkLabel lnkEmailSupport;
        private LinkLabel lnkCopyInfo;
        private System.ComponentModel.IContainer components;
	
		public About()
		{
			InitializeComponent();
		}
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.error = new System.Windows.Forms.ErrorProvider(this.components);
            this.lblCodebase = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblAppVersion = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblDotNetVersion = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblOsVersion = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblInstallId = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblDbVersion = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lnkCopyInfo = new System.Windows.Forms.LinkLabel();
            this.lnkEmailSupport = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // error
            // 
            this.error.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.error.ContainerControl = this;
            resources.ApplyResources(this.error, "error");
            // 
            // lblCodebase
            // 
            resources.ApplyResources(this.lblCodebase, "lblCodebase");
            this.lblCodebase.Name = "lblCodebase";
            // 
            // lblCopyright
            // 
            resources.ApplyResources(this.lblCopyright, "lblCopyright");
            this.lblCopyright.Name = "lblCopyright";
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // lblAppVersion
            // 
            resources.ApplyResources(this.lblAppVersion, "lblAppVersion");
            this.lblAppVersion.Name = "lblAppVersion";
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.Name = "lblTitle";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblDotNetVersion);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.lblOsVersion);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblInstallId);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblDbVersion);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblCopyright);
            this.groupBox1.Controls.Add(this.lblCodebase);
            this.groupBox1.Controls.Add(this.lblTitle);
            this.groupBox1.Controls.Add(this.lblDescription);
            this.groupBox1.Controls.Add(this.lblAppVersion);
            this.groupBox1.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lblDotNetVersion
            // 
            resources.ApplyResources(this.lblDotNetVersion, "lblDotNetVersion");
            this.lblDotNetVersion.Name = "lblDotNetVersion";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // lblOsVersion
            // 
            resources.ApplyResources(this.lblOsVersion, "lblOsVersion");
            this.lblOsVersion.Name = "lblOsVersion";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // lblInstallId
            // 
            resources.ApplyResources(this.lblInstallId, "lblInstallId");
            this.lblInstallId.Name = "lblInstallId";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // lblDbVersion
            // 
            resources.ApplyResources(this.lblDbVersion, "lblDbVersion");
            this.lblDbVersion.Name = "lblDbVersion";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lnkCopyInfo
            // 
            resources.ApplyResources(this.lnkCopyInfo, "lnkCopyInfo");
            this.lnkCopyInfo.Name = "lnkCopyInfo";
            this.lnkCopyInfo.TabStop = true;
            this.lnkCopyInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCopyInfo_LinkClicked);
            // 
            // lnkEmailSupport
            // 
            resources.ApplyResources(this.lnkEmailSupport, "lnkEmailSupport");
            this.lnkEmailSupport.Name = "lnkEmailSupport";
            this.lnkEmailSupport.TabStop = true;
            this.lnkEmailSupport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkEmailSupport_LinkClicked);
            // 
            // About
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CancelButton = this.btnOK;
            this.Controls.Add(this.lnkEmailSupport);
            this.Controls.Add(this.lnkCopyInfo);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "About";
            this.Load += new System.EventHandler(this.About_Load);
            ((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion		
		#endregion			
		
		private void About_Load(object sender, EventArgs e)
		{
            Icon = Resources.TourWriter16;

			try
			{
				var ai = new AssemblyInfo();

				lblTitle.Text = ai.Title;
                lblAppVersion.Text = AssemblyInfo.InformationalVersion;
			    lblDbVersion.Text = Cache.ToolSet.AppSettings[0].VersionNumber;
                lblOsVersion.Text = Environment.OSVersion.VersionString;
			    lblDotNetVersion.Text = App.GetDotNetVersion();
                lblDbVersion.Text = Cache.ToolSet.AppSettings[0].VersionNumber;
				lblCopyright.Text = ai.Copyright;
				lblDescription.Text = ai.Description;
			    lblInstallId.Text = Cache.ToolSet.AppSettings[0].InstallID.ToString();
				lblCodebase.Text = ai.CodeBase;
			}
			catch(Exception ex)
			{
				App.Error(ex);
			}
		}
		
        private string GetInfo()
        {
            var info = new StringBuilder();
            info.AppendLine("Title: " + lblTitle.Text);
            info.AppendLine("Application Version: " + lblAppVersion.Text);
            info.AppendLine("Database Version: " + lblDbVersion.Text);
            info.AppendLine("OS Version: " + lblOsVersion.Text);
            info.AppendLine(".NET Version: " + lblDotNetVersion.Text);
            info.AppendLine("Copyright: " + lblCopyright.Text);
            info.AppendLine("Description: " + lblDescription.Text);
            info.AppendLine("Install ID: " + lblInstallId.Text);
            info.AppendLine("Codebase: " + lblCodebase.Text);

            return info.ToString();
        }

        private void CopyInfoToClipboard()
        {
            string info = GetInfo();

            try
            {
                Clipboard.SetText(info);
                App.ShowInfo("Info copied, now use right-click then \"Paste\" to copy it to another location.");
            }
            catch (ExternalException)
            {
                App.ShowError("Failed to copy.");
            }

        }
	
		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}

        private void lnkEmailSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var messageForm = new MessageForm(GetInfo());
            messageForm.ShowDialog();
        }

        private void lnkCopyInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CopyInfoToClipboard();
        }
	}
}
