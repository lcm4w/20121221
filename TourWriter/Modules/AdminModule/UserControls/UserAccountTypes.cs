using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;
using TourWriter.Services;
using CellClickAction=Infragistics.Win.UltraWinGrid.CellClickAction;

namespace TourWriter.Modules.AdminModule.UserControls
{
	/// <summary>
	/// Summary description for NewUser.
	/// </summary>
	public class UserAccountTypes : System.Windows.Forms.UserControl
	{
		#region Designer

        private Infragistics.Win.UltraWinGrid.UltraGrid gridRoles;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblHeading;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnRoleAdd;
		private Infragistics.Win.UltraWinTree.UltraTree treePermissions;

		public UserAccountTypes()
		{
			InitializeComponent();
		}
		
		protected override void Dispose( bool disposing )
		{
			EndEdits();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserAccountTypes));
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            this.gridRoles = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.treePermissions = new Infragistics.Win.UltraWinTree.UltraTree();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnRoleAdd = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridRoles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treePermissions)).BeginInit();
            this.myToolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridRoles
            // 
            resources.ApplyResources(this.gridRoles, "gridRoles");
            this.gridRoles.Name = "gridRoles";
            this.gridRoles.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridRoles_InitializeLayout);
            this.gridRoles.AfterRowActivate += new System.EventHandler(this.gridRoles_AfterRowActivate);
            // 
            // treePermissions
            // 
            resources.ApplyResources(this.treePermissions, "treePermissions");
            this.treePermissions.FullRowSelect = true;
            this.treePermissions.Name = "treePermissions";
            _override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Extended;
            this.treePermissions.Override = _override1;
            this.treePermissions.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.treePermissions_AfterCheck);
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
            // lblHeading
            // 
            resources.ApplyResources(this.lblHeading, "lblHeading");
            this.lblHeading.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblHeading.Name = "lblHeading";
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            resources.ApplyResources(this.myToolStrip2, "myToolStrip2");
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRoleAdd});
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // btnRoleAdd
            // 
            this.btnRoleAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRoleAdd.Image = global::TourWriter.Properties.Resources.Plus;
            resources.ApplyResources(this.btnRoleAdd, "btnRoleAdd");
            this.btnRoleAdd.Name = "btnRoleAdd";
            this.btnRoleAdd.Click += new System.EventHandler(this.btnRoleAdd_Click);
            // 
            // UserAccountTypes
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridRoles);
            this.Controls.Add(this.myToolStrip2);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.treePermissions);
            this.Controls.Add(this.label4);
            this.Name = "UserAccountTypes";
            this.Load += new System.EventHandler(this.UserAccountTypes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridRoles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treePermissions)).EndInit();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#endregion

        private UserSet userSet
        {
            get
            {
                return (this.Tag as Modules.AdminModule.AdminMain).UserSet;
            }
        }

		private void UserAccountTypes_Load(object sender, System.EventArgs e)
		{
			DataBind();			
		}

		private void DataBind()
		{
			gridRoles.SetDataBinding(userSet, "Role");
		}
        		
		private void EndEdits()
		{
			this.gridRoles.UpdateData();
		}


		#region Role
		private void gridRoles_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
                if (c.Key == "RoleName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Roles";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
		}

		private void btnRoleAdd_Click(object sender, System.EventArgs e)
		{
			// add new row
			UserSet.RoleRow r = userSet.Role.NewRoleRow();
			r.RoleName = App.CreateUniqueNameValue(gridRoles.Rows, "RoleName", "New Role");
			userSet.Role.AddRoleRow(r);

			// select new row
			gridRoles.Rows.Refresh(RefreshRow.ReloadData);
			gridRoles.ActiveRow = gridRoles.Rows[gridRoles.Rows.Count-1];
		}

		private void gridRoles_AfterRowActivate(object sender, System.EventArgs e)
		{
			treePermissions_LoadFromRole((int)gridRoles.ActiveRow.Cells["RoleID"].Value);
		}
		#endregion

		#region Permission	
		private void treePermissions_LoadFromRole(int roleID)
		{	
			UltraGridRow activeRow = gridRoles.ActiveRow;
			treePermissions.Override.Sort = SortType.Default;			
			treePermissions.Nodes.Clear();

			foreach(UserSet.PermissionRow row in userSet.Permission)
			{
				UltraTreeNode node = new UltraTreeNode(row.PermissionID.ToString(), row.PermissionName);

				node.CheckedState = 
					userSet.RolePermission.FindByRoleIDPermissionID(roleID, row.PermissionID) != null ?
					CheckState.Checked : CheckState.Unchecked;

				treePermissions.Nodes.Add(node);
			}
			treePermissions.Override.Sort = SortType.Ascending;
			gridRoles.ActiveRow = activeRow;
		}

		private void treePermissions_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
		{		
			bool accept = (e.TreeNode.CheckedState == System.Windows.Forms.CheckState.Checked);

			UserSet.RoleRow role = userSet.Role.FindByRoleID((int)gridRoles.ActiveRow.Cells["RoleID"].Value);
			UserSet.PermissionRow permission = userSet.Permission.FindByPermissionID(int.Parse(e.TreeNode.Key));

			if(accept)
				userSet.RolePermission.AddRolePermissionRow(
					role, permission, DateTime.Now, TourWriter.Global.Cache.User.UserID, null);
			else
				userSet.RolePermission.FindByRoleIDPermissionID(role.RoleID, permission.PermissionID).Delete();			
		}
		#endregion
				
		protected override void OnValidating(CancelEventArgs e)
		{
			EndEdits();
			base.OnValidating (e);
		}

	}
}
