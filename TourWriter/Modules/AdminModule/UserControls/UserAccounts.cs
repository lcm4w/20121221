using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Utilities.Encryption;
using TourWriter.Services;
using CellClickAction=Infragistics.Win.UltraWinGrid.CellClickAction;

namespace TourWriter.Modules.AdminModule.UserControls
{
	/// <summary>
	/// Summary description for NewUser.
	/// </summary>
	public class UserAccounts : System.Windows.Forms.UserControl
	{
		#region Designer

        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblHeading;
		private System.Windows.Forms.Label label6;
		private Infragistics.Win.UltraWinTree.UltraTree treeRoles;
		private Infragistics.Win.UltraWinGrid.UltraGrid gridUsers;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel btnContact;
        private System.Windows.Forms.Label label7;
        private TextBox txtDisplayName;
        private Label label2;
        private TextBox txtEmail;
        private TextBox txtUsername;
        private Button btnChangePassword;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnUserAdd;
        private ToolStripButton btnUserDel;
        private GroupBox groupBox1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkDisable;
		private System.ComponentModel.Container components = null;

		public UserAccounts()
		{
			InitializeComponent();
		}
		
		protected override void Dispose(bool disposing)
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
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            this.gridUsers = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.treeRoles = new Infragistics.Win.UltraWinTree.UltraTree();
            this.label5 = new System.Windows.Forms.Label();
            this.btnContact = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnUserAdd = new System.Windows.Forms.ToolStripButton();
            this.btnUserDel = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkDisable = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeRoles)).BeginInit();
            this.myToolStrip2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkDisable)).BeginInit();
            this.SuspendLayout();
            // 
            // gridUsers
            // 
            this.gridUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridUsers.Location = new System.Drawing.Point(16, 105);
            this.gridUsers.Name = "gridUsers";
            this.gridUsers.Size = new System.Drawing.Size(200, 410);
            this.gridUsers.TabIndex = 14;
            this.gridUsers.Text = "Users";
            this.gridUsers.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridUsers_InitializeLayout);
            this.gridUsers.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.gridUsers_InitializeRow);
            this.gridUsers.AfterRowActivate += new System.EventHandler(this.gridUsers_AfterRowActivate);
            this.gridUsers.BeforeRowDeactivate += new System.ComponentModel.CancelEventHandler(this.gridUsers_BeforeRowDeactivate);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Username";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 243);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "User Roles";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 29;
            this.lblHeading.Text = "Manage user accounts";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(12, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(480, 28);
            this.label6.TabIndex = 28;
            this.label6.Text = "Manage application users and the secuity role(s) they belong to, which controls h" +
    "ow and where they can access the application.";
            // 
            // treeRoles
            // 
            this.treeRoles.FullRowSelect = true;
            this.treeRoles.Location = new System.Drawing.Point(100, 243);
            this.treeRoles.Name = "treeRoles";
            _override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Extended;
            this.treeRoles.Override = _override1;
            this.treeRoles.Size = new System.Drawing.Size(196, 161);
            this.treeRoles.TabIndex = 30;
            this.treeRoles.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.treeRoles_AfterCheck);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 177);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(177, 13);
            this.label5.TabIndex = 31;
            this.label5.Text = "View the Users contact details here:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnContact
            // 
            this.btnContact.AutoSize = true;
            this.btnContact.Location = new System.Drawing.Point(219, 177);
            this.btnContact.Name = "btnContact";
            this.btnContact.Size = new System.Drawing.Size(77, 13);
            this.btnContact.TabIndex = 32;
            this.btnContact.TabStop = true;
            this.btnContact.Text = "Contact details";
            this.btnContact.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnContact.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnContact_LinkClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "Email address";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(84, 58);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(212, 20);
            this.txtDisplayName.TabIndex = 58;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 57;
            this.label2.Text = "Display name";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(84, 84);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(212, 20);
            this.txtEmail.TabIndex = 59;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(84, 32);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(212, 20);
            this.txtUsername.TabIndex = 60;
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Location = new System.Drawing.Point(164, 120);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(132, 23);
            this.btnChangePassword.TabIndex = 61;
            this.btnChangePassword.Text = "Set Password";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUserAdd,
            this.btnUserDel});
            this.myToolStrip2.Location = new System.Drawing.Point(172, 82);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(80, 25);
            this.myToolStrip2.TabIndex = 134;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnUserAdd
            // 
            this.btnUserAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUserAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnUserAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserAdd.Name = "btnUserAdd";
            this.btnUserAdd.Size = new System.Drawing.Size(23, 22);
            this.btnUserAdd.Click += new System.EventHandler(this.btnUserAdd_Click);
            // 
            // btnUserDel
            // 
            this.btnUserDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUserDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnUserDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUserDel.Name = "btnUserDel";
            this.btnUserDel.Size = new System.Drawing.Size(23, 22);
            this.btnUserDel.Click += new System.EventHandler(this.btnUserDel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkDisable);
            this.groupBox1.Controls.Add(this.treeRoles);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnChangePassword);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.btnContact);
            this.groupBox1.Controls.Add(this.txtEmail);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtDisplayName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(242, 105);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 410);
            this.groupBox1.TabIndex = 135;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // chkDisable
            // 
            this.chkDisable.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkDisable.Location = new System.Drawing.Point(173, 217);
            this.chkDisable.Name = "chkDisable";
            this.chkDisable.Size = new System.Drawing.Size(123, 20);
            this.chkDisable.TabIndex = 63;
            this.chkDisable.Text = "Disable this account";
            this.chkDisable.BeforeCheckStateChanged += new Infragistics.Win.CheckEditor.BeforeCheckStateChangedHandler(this.chkDisable_BeforeCheckStateChanged);
            this.chkDisable.Click += new System.EventHandler(this.chkDisable_Click);
            // 
            // UserAccounts
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridUsers);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.myToolStrip2);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label6);
            this.Name = "UserAccounts";
            this.Size = new System.Drawing.Size(570, 543);
            this.Load += new System.EventHandler(this.UserAccounts_Load);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.UserAccounts_Validating);
            ((System.ComponentModel.ISupportInitialize)(this.gridUsers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeRoles)).EndInit();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkDisable)).EndInit();
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

		private void UserAccounts_Load(object sender, System.EventArgs e)
		{
			DataBind();
			
			if(gridUsers.Rows.Count > 0)
				gridUsers.ActiveRow = gridUsers.Rows[0];
		}

		private void DataBind()
		{
            userSet.User.DefaultView.RowFilter = App.IsDebugMode ? "" : string.Format("UserName <> '{0}'", App.AdminUserName);
		    gridUsers.DataSource = userSet.User.DefaultView;

            txtUsername.DataBindings.Add("Text", userSet.User.DefaultView, "Username");
            txtDisplayName.DataBindings.Add("Text", userSet.User.DefaultView, "DisplayName");
            txtEmail.DataBindings.Add("Text", userSet.User.DefaultView, "Email");

            chkDisable.DataBindings.Add("Checked", userSet.User.DefaultView, "IsRecordActive");
		}
		
		private void EndEdits()
		{		
			this.gridUsers.UpdateData();
		}


		#region User list
		private void gridUsers_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
                if (c.Key == "UserName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Users";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
		}

		private void gridUsers_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
		{
			e.Row.Appearance.ForeColor = ((bool)e.Row.Cells["IsRecordActive"].Value) ? 
				Color.Gray : Color.Black;
		
		}
		private void gridUsers_BeforeRowDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(this.Parent != null)
			{
			    bool isValid = this.ParentForm.Validate();
				e.Cancel = !isValid;
			}
		}
		
		private void gridUsers_AfterRowActivate(object sender, System.EventArgs e)
		{
			int id = (int)gridUsers.ActiveRow.Cells["UserID"].Value;
			treeRoles_LoadFromUser(id);
		}

		private void btnUserAdd_Click(object sender, System.EventArgs e)
		{
			if(this.ParentForm.Validate())
			{
				// initialise new row
				UserSet.UserRow r = userSet.User.NewUserRow();
				r.UserName = App.CreateUniqueNameValue(gridUsers.Rows, "UserName", "New User");
                r.DisplayName = r.UserName;

                // even blank passwords need to be encrypted
				r.Password = EncryptionHelper.EncryptString(String.Empty);
	
                r.IsRecordActive = false;
				r.AddedOn = DateTime.Now;
				r.AddedBy = TourWriter.Global.Cache.User.UserID;

				// add the row
				userSet.User.AddUserRow(r);

                // give them a role by default
                if (treeRoles.Nodes.Count > 0 && treeRoles.Nodes[0] != null)
                {
                    UserSet.RoleRow theRole = userSet.Role.FindByRoleID(int.Parse(treeRoles.Nodes[0].Key));
                    userSet.UserRole.AddUserRoleRow(
                        r, userSet.Role.FindByRoleID(theRole.RoleID), DateTime.Now, Cache.User.UserID, null);
                }

			    // select new row
				gridUsers.ActiveRow = gridUsers.Rows[gridUsers.Rows.Count-1];
				txtUsername.Select();
			}
		}

		private void btnUserDel_Click(object sender, System.EventArgs e)
		{
			CurrencyManager cm = this.BindingContext[userSet, "User"] as CurrencyManager;
			if(cm.Position > -1)
			{
				DataRowView drv = cm.Current as DataRowView;

				// can't delete last able account
				object o = userSet.User.Compute("COUNT(UserID)", "IsRecordActive = false");
				if((int)o == 1 && (bool)drv["IsRecordActive"] == false)
				{
					MessageBox.Show("Cannot delete last able user account");
				}
                else if (Cache.User.UserID == (int)gridUsers.ActiveRow.Cells["UserID"].Value)
                {
                    App.ShowError("You cannot delete yourself, your account will be disabled instead.");
                    chkDisable.Checked = true;
                }
				else
				{
					if(MessageBox.Show(App.GetResourceString("AskDeleteOrDisableRow"), App.MessageCaption,
						MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) 
						== DialogResult.Yes)
					{
						UltraGridRow row = gridUsers.ActiveRow;
						// delete
						int i = row.Index;
						row.Delete(false);
						// select next row
						if( ( i > gridUsers.Rows.Count-1 ? --i : i) > -1)
							gridUsers.ActiveRow = gridUsers.Rows[i];
					}
				}
			}
		}
								
		private void btnContact_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{	
			CurrencyManager cm = this.BindingContext[userSet, "User"] as CurrencyManager;
			if(cm.Position > -1)
			{
				DataRowView drv = cm.Current as DataRowView;

				if(drv["ContactID"].ToString() == "") // doesn't exist
				{
					if(App.AskCreateRow())
					{					
						Modules.ContactModule.ContactMain contact = new Modules.ContactModule.ContactMain();			
						if (contact.ShowDialog(this) == DialogResult.OK)
						{							
							// get new Contact row
							ContactSet.ContactRow c = (ContactSet.ContactRow)contact.ContactRow;

							// load contact row into this userSet
                            userSet.Contact.BeginLoadData();
                            userSet.Contact.LoadDataRow(c.ItemArray, true);
                            userSet.Contact.EndLoadData();

							// add FK value
							drv["ContactID"] = c.ContactID;
						}
					}
				}
				else // open existing contact record
				{
					int contactID = (int)drv["ContactID"];
					Modules.ContactModule.ContactMain contact = 
						new Modules.ContactModule.ContactMain(contactID);
				
					// reload contact to reflect changes
					if (contact.ShowDialog(this) == DialogResult.OK)
					{						
						userSet.Contact.BeginLoadData();
						userSet.Contact.LoadDataRow(contact.ContactRow.ItemArray, true);
						userSet.Contact.EndLoadData();
					}
					contact.Dispose();
				}
			}		
		}

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            PasswordForm passwordEditor = new PasswordForm();
            bool passwordChanged = passwordEditor.ShowDialog() == DialogResult.OK;

            if (passwordChanged)
            {
                var id = (int)gridUsers.ActiveRow.Cells["UserID"].Value;
                var user = userSet.User.FindByUserID(id);
                var password = EncryptionHelper.EncryptString(passwordEditor.Password);
                if (password != user.Password) user.Password = password;
            }
        }
		#endregion

		#region User detail
		private void treeRoles_LoadFromUser(int userID)
		{	
			treeRoles.Override.Sort = SortType.Default;	
			treeRoles.Nodes.Clear();

			foreach(UserSet.RoleRow row in userSet.Role)
			{
				UltraTreeNode node = new UltraTreeNode(row.RoleID.ToString(), row.RoleName);

				node.CheckedState = 
					userSet.UserRole.FindByUserIDRoleID(userID, row.RoleID) != null ?
					CheckState.Checked : CheckState.Unchecked;

				treeRoles.Nodes.Add(node);	
			}
			treeRoles.Override.Sort = SortType.Ascending;
		}

		private void treeRoles_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
		{		
			bool accept = (e.TreeNode.CheckedState == System.Windows.Forms.CheckState.Checked);
			UserSet.UserRow user = userSet.User.FindByUserID((int)gridUsers.ActiveRow.Cells["UserID"].Value);
			UserSet.RoleRow theRole = userSet.Role.FindByRoleID(int.Parse(e.TreeNode.Key));
				
			if(accept)
			{
				// import the Role to handle relationship constraints, Permissions don't matter at this mult-user level 
				if(userSet.Role.FindByRoleID(theRole.RoleID) == null)				
					userSet.Role.ImportRow(theRole); 

				// add the new UserRole
				userSet.UserRole.AddUserRoleRow(  
					user, userSet.Role.FindByRoleID(theRole.RoleID), DateTime.Now, TourWriter.Global.Cache.User.UserID, null);
			}
			else // unchecked so remove
			{
				// remove the UserRole, Permissions don't matter at this mult-user level
				userSet.UserRole.FindByUserIDRoleID(user.UserID, theRole.RoleID).Delete();
			}
		}
		
				
		private bool chkDisableFlag = false;
		private void chkDisable_Click(object sender, System.EventArgs e)
		{
			chkDisableFlag = true;
		}

		private void chkDisable_BeforeCheckStateChanged(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(chkDisableFlag)
			{
				// can't disable last able account
				CurrencyManager cm = this.BindingContext[userSet, "User"] as CurrencyManager;
				if(cm.Position > -1)
				{
					DataRowView drv = cm.Current as DataRowView;				

					if((bool)drv["IsRecordActive"] == false)
					{
						object o = userSet.User.Compute("COUNT(UserID)", "IsRecordActive = false");
						if((int)o == 1)
						{
							MessageBox.Show("Cannot disable last able user account");
							e.Cancel = true;
						}	
					}
				}
				chkDisableFlag = false;
			}
		}
		#endregion

        private void UserAccounts_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EndEdits();
        }
	}
}
