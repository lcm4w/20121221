using System;
using System.Windows.Forms;
using System.Reflection;

namespace TourWriter.Modules.ReportsModule.old
{
	/// <summary>
	/// Summary description for AdminMain.
	/// </summary>
	public class GeneralReportsMain : ModuleBase
	{
		#region Member vars
		#endregion

		#region Designer

        private Infragistics.Win.UltraWinEditors.UltraOptionSet optReports;
		private System.Windows.Forms.Label label47;
		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.GroupBox grpReport;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem menuHelp;
        private ToolStrip toolStrip1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolHelp;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem menuClose;
        private ToolStripSeparator toolStripSeparator1;
        private Label label11;
		private System.ComponentModel.IContainer components = null;
		#endregion

		public GeneralReportsMain()
		{
			InitializeComponent();
            displayTypeName = "Reports";
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;
		}
		
		private void ReportsMain_Load(object sender, System.EventArgs e)
        {
            Icon = System.Drawing.Icon.FromHandle(TourWriter.Properties.Resources.Report.GetHicon());
		    
			SetSecurityLevels();

			// load default user control
			optReports.CheckedIndex = 0;
		}
				
		
		#region Override methods
		protected override bool IsDataDirty()
		{
			return false;
		}

		protected override string GetDisplayName()
		{
			return "General Reports";//optReports.Text;
		}
		
		#endregion

		private void SetSecurityLevels()
		{
			// check user permissions and show only what the user is allowed to see
			if(!Services.AppPermissions.UserHasPermission(Services.AppPermissions.Permissions.ReportClientLocations))
			{
			    int index = optReports.Items.ValueList.FindStringExact("Client Locations");
                if (index >= 0)
			        optReports.Items.RemoveAt(index);
			}
			if(!Services.AppPermissions.UserHasPermission(Services.AppPermissions.Permissions.ReportSuppierPurchases))
			{
                int index = optReports.Items.ValueList.FindStringExact("Supplier Purchases");
                if (index >= 0)
                    optReports.Items.RemoveAt(index);
			}
			if(!Services.AppPermissions.UserHasPermission(Services.AppPermissions.Permissions.ReportItineraryYield))
			{
                int index = optReports.Items.ValueList.FindStringExact("Itinerary Yield");
                if (index >= 0)
                    optReports.Items.RemoveAt(index);
			}
		}
		
		internal void LoadReportControl(string controlToLoad)
		{	
			// load new control into pnlMain form
			if(controlToLoad.Trim() != "")
			{
				Cursor = Cursors.WaitCursor;
				
				// clear current bindings
				App.ClearBindings(pnlMain);

				// clear current control
				this.pnlMain.Controls.Clear();

				// load the controls dynamically based on what was clicked, check the Key property of the node
				try
				{
					// since the form is not in the current MdiChildren collection, load it up using reflection
					Assembly asm = Assembly.LoadFrom(Assembly.GetExecutingAssembly().CodeBase);
					Type t = asm.GetType("TourWriter.Modules.ReportsModule.old.UserControls." + controlToLoad);
					UserControl uc = (UserControl)Activator.CreateInstance(t);
					uc.Tag = this;
					uc.Dock = DockStyle.Fill;
					pnlMain.Controls.Add(uc);
				}
				catch(Exception ex)
				{
					App.Error(ex);
				}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}
		
					
		private void optReports_ValueChanged(object sender, System.EventArgs e)
		{
			grpReport.Text = optReports.Text;
			LoadReportControl(optReports.Value.ToString());	

			SetFormActiveText();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralReportsMain));
            this.optReports = new Infragistics.Win.UltraWinEditors.UltraOptionSet();
            this.label47 = new System.Windows.Forms.Label();
            this.grpReport = new System.Windows.Forms.GroupBox();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolHelp = new System.Windows.Forms.ToolStripButton();
            this.label11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.optReports)).BeginInit();
            this.grpReport.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // optReports
            // 
            this.optReports.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.optReports.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            valueListItem1.DataValue = "ClientLocations";
            valueListItem1.DisplayText = "Client Locations";
            valueListItem2.DataValue = "SupplierPurchases";
            valueListItem2.DisplayText = "Supplier Purchases";
            valueListItem3.DataValue = "ItineraryYield";
            valueListItem3.DisplayText = "Itinerary Yield";
            this.optReports.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            this.optReports.ItemSpacingVertical = 10;
            this.optReports.Location = new System.Drawing.Point(16, 44);
            this.optReports.Name = "optReports";
            this.optReports.Size = new System.Drawing.Size(147, 265);
            this.optReports.TabIndex = 59;
            this.optReports.TextIndentation = 4;
            this.optReports.ValueChanged += new System.EventHandler(this.optReports_ValueChanged);
            // 
            // label47
            // 
            this.label47.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.Location = new System.Drawing.Point(13, 17);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(204, 24);
            this.label47.TabIndex = 60;
            this.label47.Text = "General Reports";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpReport
            // 
            this.grpReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpReport.Controls.Add(this.pnlMain);
            this.grpReport.Location = new System.Drawing.Point(169, 22);
            this.grpReport.Name = "grpReport";
            this.grpReport.Size = new System.Drawing.Size(455, 287);
            this.grpReport.TabIndex = 6;
            this.grpReport.TabStop = false;
            this.grpReport.Text = "Report name";
            // 
            // pnlMain
            // 
            this.pnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMain.AutoScroll = true;
            this.pnlMain.Location = new System.Drawing.Point(8, 22);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(439, 257);
            this.pnlMain.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 89);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(648, 333);
            this.panel2.TabIndex = 61;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.AutoScroll = true;
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.grpReport);
            this.panel3.Controls.Add(this.label47);
            this.panel3.Controls.Add(this.optReports);
            this.panel3.Location = new System.Drawing.Point(8, 8);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(632, 317);
            this.panel3.TabIndex = 62;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 40);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(648, 24);
            this.menuStrip1.TabIndex = 62;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.menuClose});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator1.MergeIndex = 1;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(97, 6);
            // 
            // menuClose
            // 
            this.menuClose.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuClose.MergeIndex = 2;
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(100, 22);
            this.menuClose.Text = "Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelp});
            this.helpToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // menuHelp
            // 
            this.menuHelp.Image = ((System.Drawing.Image)(resources.GetObject("menuHelp.Image")));
            this.menuHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuHelp.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuHelp.MergeIndex = 1;
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(167, 22);
            this.menuHelp.Text = "Help for this screen";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.toolHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 64);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(648, 25);
            this.toolStrip1.TabIndex = 63;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolHelp
            // 
            this.toolHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolHelp.Image")));
            this.toolHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolHelp.Name = "toolHelp";
            this.toolHelp.Size = new System.Drawing.Size(48, 22);
            this.toolHelp.Text = "Help";
            this.toolHelp.ToolTipText = "Help for this screen";
            this.toolHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(16, 4);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(608, 13);
            this.label11.TabIndex = 155;
            this.label11.Text = "Please use new reports, this page will be removed when new reports complete";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GeneralReportsMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(648, 422);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.HeaderVisible = true;
            this.MainToolStrip = this.toolStrip1;
            this.Name = "GeneralReportsMain";
            this.Text = "General Reports";
            this.Load += new System.EventHandler(this.ReportsMain_Load);
            this.Controls.SetChildIndex(this.menuStrip1, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.optReports)).EndInit();
            this.grpReport.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private void menuHelp_Click(object sender, EventArgs e)
        {
            App.ShowHelp("GeneralReports." + optReports.Value);
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }
	}
}
