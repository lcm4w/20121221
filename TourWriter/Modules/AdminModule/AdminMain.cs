using System;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using TourWriter.Info;
using TourWriter.BusinessLogic;
using TourWriter.Global;

namespace TourWriter.Modules.AdminModule
{
	/// <summary>
	/// Summary description for AdminMain.
	/// </summary>
	public class AdminMain : ModuleBase
	{
		#region Member vars

		internal ToolSet ToolSet
		{
			get{ return Cache.ToolSet; }
		}

        internal UserSet UserSet
		{
            get { return Cache.UserSet; }
		}

        internal AgentSet AgentSet
        {
            get { return Cache.AgentSet; }
		}
		
		#endregion

		#region Designer

        private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel pnlMain;
		internal Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar AdminMenu;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Splitter splitter1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem menuSave;
        private ToolStripMenuItem menuRefresh;
        private ToolStripMenuItem menuClose;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem menuHelp;
        private ToolStrip toolStrip1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolSave;
        private ToolStripButton toolRefresh;
        private ToolStripButton toolHelp;
		private System.ComponentModel.IContainer components = null;
		#endregion

		public AdminMain()
		{
			InitializeComponent();
			displayTypeName = "Setup";
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;
		}
		
		private void Admin_Load(object sender, System.EventArgs e)
        {
            // TODO: this ensures all users/roles/permissions are loaded, but not always required if:
            // - user does not navigate to those edit pages
            if(Cache.UserSet.User.Count == 1)
		        Cache.UserSet.LoadAll();

            Icon = System.Drawing.Icon.FromHandle(TourWriter.Properties.Resources.Wrench.GetHicon());
		    
			SetSecurityLevels();
			Cache.RefreshToolSet(true);

			foreach(DataTable dt in Cache.ToolSet.Tables)
			{
				dt.ColumnChanged += new DataColumnChangeEventHandler(DataSet_ColumnChanged);
				dt.RowDeleted    +=new DataRowChangeEventHandler(DataSet_RowDeleted);
			}
			foreach(DataTable dt in UserSet.Tables)
			{
				dt.ColumnChanged += new DataColumnChangeEventHandler(DataSet_ColumnChanged);
				dt.RowDeleted    +=new DataRowChangeEventHandler(DataSet_RowDeleted);
			}
			foreach(DataTable dt in AgentSet.Tables)
			{
				dt.ColumnChanged += new DataColumnChangeEventHandler(DataSet_ColumnChanged);
				dt.RowDeleted    +=new DataRowChangeEventHandler(DataSet_RowDeleted);
			}
		}
		
		
		protected override bool IsDataDirty()
		{
			CommitOpenEdits();
			return 
				(ToolSet  != null && ToolSet.HasChanges()) || 
				(UserSet  != null && UserSet.HasChanges()) || 
				(AgentSet != null && AgentSet.HasChanges());
		}

		protected override string GetDisplayName()
		{
			return "Setup"; // (AdminMenu.ActiveItem != null ? AdminMenu.ActiveItem.Text : "");
		}

		protected override void SaveDataChanges()
		{
			Cursor c = Cursor;
			Cursor = Cursors.WaitCursor;

			try
			{
				CommitOpenEdits();

				if(UserSet != null && UserSet.HasChanges())
					SaveUserSet();				
				if(AgentSet != null && AgentSet.HasChanges())
					SaveAgentSet();
				if(ToolSet != null && ToolSet.HasChanges())
					SaveToolSet(); // toolset last to get any changes to agentset above

				Text = Text.TrimStart('*');				
			}
			finally
			{				
				Cursor = c;
			}
		}

		protected override void CancelDataChanges()
		{
			Cache.RefreshToolSet(false);
            Cache.RefreshUserSet(false);
		    Cache.RefreshAgentSet(false);
		}

		protected override void CommitOpenEdits()
		{
			Validate();

		    App.CommitGridEdits(this);

			if(ToolSet != null)	
			{
				foreach (DataTable dt in ToolSet.Tables)
					foreach (DataRow dr in dt.Rows)
						dr.EndEdit();
			}
			if(UserSet != null)
			{
				foreach (DataTable dt in UserSet.Tables)
					foreach (DataRow dr in dt.Rows)
						dr.EndEdit();
			}
			if(AgentSet != null)
			{
				foreach (DataTable dt in AgentSet.Tables)
					foreach (DataRow dr in dt.Rows)
						dr.EndEdit();
			}
		}

		
		private void SetSecurityLevels()
		{
			// check user permissions and show only what the user is allowed to see

			// menu items
			AdminMenu.Groups["Users"].Items["UserDetail"].Visible		= true;
		    AdminMenu.Groups["Users"].Items["UserAccounts"].Visible     = Services.AppPermissions.UserHasPermission(Services.AppPermissions.Permissions.UserAccounts);
		    AdminMenu.Groups["Users"].Items["UserAccountTypes"].Visible = Services.AppPermissions.UserHasPermission(Services.AppPermissions.Permissions.UserAccounts);

			// menu categories
			AdminMenu.Groups["DataSetup"].Enabled	   = Services.AppPermissions.UserHasPermission(Services.AppPermissions.Permissions.DataLists);
			AdminMenu.Groups["Administration"].Enabled = Services.AppPermissions.UserHasPermission(Services.AppPermissions.Permissions.Administration);
		}

		internal void LoadNewControl(string controlToLoad)
		{	
			// load new control into pnlMain form
			if(CurrentControlIsValid() && controlToLoad.Trim() != "")
			{
				Cursor = Cursors.WaitCursor;
				
				// clear current bindings
				App.ClearBindings(this.pnlMain);

				// clear current control
				this.pnlMain.Controls.Clear();

				// load the controls dynamically based on what was clicked, check the Key property of the node
				try
				{
					// since the form is not in the current MdiChildren collection, load it up using reflection
					Assembly asm = Assembly.LoadFrom(Assembly.GetExecutingAssembly().CodeBase);
					Type t = asm.GetType("TourWriter.Modules.AdminModule.UserControls." + controlToLoad);
					UserControl uc = (UserControl)Activator.CreateInstance(t);
					uc.Tag = this;
					uc.Dock = DockStyle.Fill;
					pnlMain.Controls.Add(uc);
					this.MenuNode.Text = "Options/" + (AdminMenu.ActiveItem != null ? AdminMenu.ActiveItem.Text : "");
				}
				catch(Exception ex)
				{
					App.Error(ex);
				}
				finally
				{
					this.Cursor = Cursors.Default;
				}
			}
		}
		
		
		private bool CurrentControlIsValid()
		{
			// validate fires the currently loaded usercontrols onvalidate delegate
			return this.Validate();
		}


        private static void SaveToolSet()
		{
			Cache.SaveToolSet();
		}

        private void SaveUserSet()
        {
            // save current selected user
            int index = BindingContext[UserSet, "User"].Position;

            Cache.SaveUserSet();

            // reselect user 
            BindingContext[UserSet, "User"].Position = index;
		}

		private static void SaveAgentSet()
		{
		    Cache.SaveAgentSet();
		}
		
		private void AdminMenu_ItemClick(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
		{
			// pass the selected item's Key property to the generic load event
			LoadNewControl(e.Item.Key.ToString());

			SetFormActiveText();
		}

		
		private void DataSet_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			base.SetDataDirtyName();
		}
		
		private void DataSet_RowDeleted(object sender, DataRowChangeEventArgs e)
		{
			base.SetDataDirtyName();
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
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem7 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem8 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem9 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem10 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem11 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem12 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem13 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem14 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminMain));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.AdminMenu = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSave = new System.Windows.Forms.ToolStripButton();
            this.toolRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolHelp = new System.Windows.Forms.ToolStripButton();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AdminMenu)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.AutoScroll = true;
            this.pnlMain.BackColor = System.Drawing.SystemColors.Control;
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(180, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(440, 444);
            this.pnlMain.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 89);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 460);
            this.panel2.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.AutoScroll = true;
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.splitter1);
            this.panel3.Controls.Add(this.pnlMain);
            this.panel3.Controls.Add(this.AdminMenu);
            this.panel3.Location = new System.Drawing.Point(8, 8);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(620, 444);
            this.panel3.TabIndex = 6;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(180, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 444);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // AdminMenu
            // 
            this.AdminMenu.AnimationSpeed = Infragistics.Win.UltraWinExplorerBar.AnimationSpeed.Fast;
            this.AdminMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.AdminMenu.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            ultraExplorerBarItem1.Key = "UserDetail";
            ultraExplorerBarItem1.Text = "My account details";
            ultraExplorerBarItem1.ToolTipText = "Edit you login and contact details.";
            ultraExplorerBarItem2.Key = "UserAccounts";
            ultraExplorerBarItem2.Text = "User accounts";
            ultraExplorerBarItem2.ToolTipText = "Manage the users and their security access levels";
            ultraExplorerBarItem3.Key = "UserAccountTypes";
            ultraExplorerBarItem3.Text = "Account types";
            ultraExplorerBarItem3.ToolTipText = "Control security by setting the types of accounts that users can belong to.";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1,
            ultraExplorerBarItem2,
            ultraExplorerBarItem3});
            ultraExplorerBarGroup1.Key = "Users";
            ultraExplorerBarGroup1.Text = "User accounts";
            ultraExplorerBarItem4.Key = "DataLists";
            ultraExplorerBarItem4.Text = "General lists";
            ultraExplorerBarItem5.Key = "SupplierConfigs";
            ultraExplorerBarItem5.Text = "Supplier configs";
            ultraExplorerBarItem6.Key = "ServiceConfigs";
            ultraExplorerBarItem6.Text = "Service configs";
            ultraExplorerBarItem6.ToolTipText = "Setup the various service types and their configurations";
            ultraExplorerBarItem7.Key = "Locations";
            ultraExplorerBarItem7.Text = "Locations";
            ultraExplorerBarItem8.Key = "AgentLabels";
            ultraExplorerBarItem8.Text = "Agent labels";
            ultraExplorerBarItem9.Key = "Currency";
            ultraExplorerBarItem9.Text = "Currencies";
            ultraExplorerBarItem10.Key = "Accounting";
            ultraExplorerBarItem10.Text = "Accounting setup";
            ultraExplorerBarGroup2.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem4,
            ultraExplorerBarItem5,
            ultraExplorerBarItem6,
            ultraExplorerBarItem7,
            ultraExplorerBarItem8,
            ultraExplorerBarItem9,
            ultraExplorerBarItem10});
            ultraExplorerBarGroup2.Key = "DataSetup";
            ultraExplorerBarGroup2.Text = "Data setup";
            ultraExplorerBarItem11.Key = "EmailSettings";
            ultraExplorerBarItem11.Text = "Email settings";
            ultraExplorerBarItem11.ToolTipText = "Configure email related settings";
            ultraExplorerBarItem12.Key = "FolderSettings";
            ultraExplorerBarItem12.Text = "Folder  settings";
            ultraExplorerBarItem12.ToolTipText = "Set default folder paths etc...";
            ultraExplorerBarItem13.Key = "DbBackup";
            ultraExplorerBarItem13.Text = "Database";
            ultraExplorerBarItem13.ToolTipText = "Perform backup action on the database";
            ultraExplorerBarItem14.Key = "LicenseManager";
            ultraExplorerBarItem14.Text = "Licensing";
            ultraExplorerBarItem14.ToolTipText = "TourWriter subscription license";
            ultraExplorerBarGroup3.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem11,
            ultraExplorerBarItem12,
            ultraExplorerBarItem13,
            ultraExplorerBarItem14});
            ultraExplorerBarGroup3.Key = "Administration";
            ultraExplorerBarGroup3.Text = "Administration";
            this.AdminMenu.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1,
            ultraExplorerBarGroup2,
            ultraExplorerBarGroup3});
            this.AdminMenu.ItemSettings.MaxLines = 2;
            this.AdminMenu.Location = new System.Drawing.Point(0, 0);
            this.AdminMenu.Name = "AdminMenu";
            this.AdminMenu.Size = new System.Drawing.Size(180, 444);
            this.AdminMenu.TabIndex = 2;
            this.AdminMenu.ItemClick += new Infragistics.Win.UltraWinExplorerBar.ItemClickEventHandler(this.AdminMenu_ItemClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 40);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(636, 24);
            this.menuStrip1.TabIndex = 23;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.menuSave,
            this.menuRefresh,
            this.menuClose});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripSeparator2.MergeIndex = 1;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(135, 6);
            // 
            // menuSave
            // 
            this.menuSave.Image = ((System.Drawing.Image)(resources.GetObject("menuSave.Image")));
            this.menuSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuSave.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuSave.MergeIndex = 2;
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSave.Size = new System.Drawing.Size(138, 22);
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuRefresh
            // 
            this.menuRefresh.Image = global::TourWriter.Properties.Resources.Refresh;
            this.menuRefresh.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuRefresh.MergeIndex = 3;
            this.menuRefresh.Name = "menuRefresh";
            this.menuRefresh.Size = new System.Drawing.Size(138, 22);
            this.menuRefresh.Text = "&Refresh";
            this.menuRefresh.Click += new System.EventHandler(this.menuRefresh_Click);
            // 
            // menuClose
            // 
            this.menuClose.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuClose.MergeIndex = 4;
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(138, 22);
            this.menuClose.Text = "Close";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelp});
            this.helpToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // menuHelp
            // 
            this.menuHelp.Image = ((System.Drawing.Image)(resources.GetObject("menuHelp.Image")));
            this.menuHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.menuHelp.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuHelp.MergeIndex = 1;
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(176, 22);
            this.menuHelp.Text = "Help for this screen";
            this.menuHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.toolSave,
            this.toolRefresh,
            this.toolHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 64);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(636, 25);
            this.toolStrip1.TabIndex = 24;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolSave
            // 
            this.toolSave.Image = ((System.Drawing.Image)(resources.GetObject("toolSave.Image")));
            this.toolSave.Name = "toolSave";
            this.toolSave.Size = new System.Drawing.Size(51, 22);
            this.toolSave.Text = "&Save";
            this.toolSave.ToolTipText = "Save changes";
            this.toolSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // toolRefresh
            // 
            this.toolRefresh.Image = global::TourWriter.Properties.Resources.Refresh;
            this.toolRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRefresh.Name = "toolRefresh";
            this.toolRefresh.Size = new System.Drawing.Size(66, 22);
            this.toolRefresh.Text = "&Refresh";
            this.toolRefresh.ToolTipText = "Refresh data";
            this.toolRefresh.Click += new System.EventHandler(this.menuRefresh_Click);
            // 
            // toolHelp
            // 
            this.toolHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolHelp.Image")));
            this.toolHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolHelp.Name = "toolHelp";
            this.toolHelp.Size = new System.Drawing.Size(52, 22);
            this.toolHelp.Text = "Help";
            this.toolHelp.ToolTipText = "Help for this screen";
            this.toolHelp.Click += new System.EventHandler(this.menuHelp_Click);
            // 
            // AdminMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(636, 549);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.HeaderVisible = true;
            this.MainToolStrip = this.toolStrip1;
            this.Name = "AdminMain";
            this.Text = "Setup";
            this.Load += new System.EventHandler(this.Admin_Load);
            this.Controls.SetChildIndex(this.menuStrip1, 0);
            this.Controls.SetChildIndex(this.toolStrip1, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AdminMenu)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private void menuSave_Click(object sender, EventArgs e)
        {
            if (CurrentControlIsValid())
                SaveDataChanges();
        }

        private void menuRefresh_Click(object sender, EventArgs e)
        {
            bool hasChanges = Cache.ToolSet.HasChanges() || Cache.AgentSet.HasChanges() || Cache.UserSet.HasChanges();
            if (hasChanges)
            {
                hasChanges = App.AskYesNo(App.GetResourceString("AskDoSave"));
            }
            Cache.RefreshToolSet(hasChanges);
            Cache.RefreshAgentSet(hasChanges);
            Cache.RefreshUserSet(hasChanges);
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            if (pnlMain.Controls.Count > 0)
            {
                App.ShowHelp("Setup." + pnlMain.Controls[0].Name);
            }
            else
            {
                App.ShowHelp();
            }
        }
	}
}
