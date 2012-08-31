using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.BusinessLogic;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.AdminModule.UserControls
{
	/// <summary>
	/// Summary description for NewUser.
	/// </summary>
	public class ServiceConfigs : System.Windows.Forms.UserControl
	{
		#region Designer

        private Infragistics.Win.UltraWinGrid.UltraGrid gridType;
		private Infragistics.Win.UltraWinGrid.UltraGrid gridConfig;
		private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label label4;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnTypeAdd;
        private ToolStripButton btnTypeDel;
        private TourWriter.UserControls.MyToolStrip myToolStrip1;
        private ToolStripButton btnConfigAdd;
        private ToolStripButton btnConfigDel;
		private System.ComponentModel.Container components = null;

		public ServiceConfigs()
		{
			InitializeComponent();
		}
		
		protected override void Dispose( bool disposing )
		{
			this.EndEdits();

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
            this.gridType = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gridConfig = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnTypeAdd = new System.Windows.Forms.ToolStripButton();
            this.btnTypeDel = new System.Windows.Forms.ToolStripButton();
            this.myToolStrip1 = new TourWriter.UserControls.MyToolStrip();
            this.btnConfigAdd = new System.Windows.Forms.ToolStripButton();
            this.btnConfigDel = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridConfig)).BeginInit();
            this.myToolStrip2.SuspendLayout();
            this.myToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridType
            // 
            this.gridType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridType.Location = new System.Drawing.Point(16, 107);
            this.gridType.Name = "gridType";
            this.gridType.Size = new System.Drawing.Size(455, 394);
            this.gridType.TabIndex = 14;
            this.gridType.Text = "Service Types";
            this.gridType.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridType_InitializeLayout);
            // 
            // gridConfig
            // 
            this.gridConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridConfig.Location = new System.Drawing.Point(489, 107);
            this.gridConfig.Name = "gridConfig";
            this.gridConfig.Size = new System.Drawing.Size(236, 394);
            this.gridConfig.TabIndex = 19;
            this.gridConfig.Text = "Service Type Configurations";
            this.gridConfig.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridConfig_InitializeLayout);
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 28;
            this.lblHeading.Text = "Add service types and their configurations";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(480, 32);
            this.label4.TabIndex = 27;
            this.label4.Text = "Service types help define the type of each service from a supplier (eg. Accommoda" +
                "tion). Configurations are added to help describe each service type.";
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnTypeAdd,
            this.btnTypeDel});
            this.myToolStrip2.Location = new System.Drawing.Point(428, 84);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip2.TabIndex = 130;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnTypeAdd
            // 
            this.btnTypeAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTypeAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnTypeAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTypeAdd.Name = "btnTypeAdd";
            this.btnTypeAdd.Size = new System.Drawing.Size(23, 22);
            this.btnTypeAdd.Click += new System.EventHandler(this.btnTypeAdd_Click);
            // 
            // btnTypeDel
            // 
            this.btnTypeDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTypeDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnTypeDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTypeDel.Name = "btnTypeDel";
            this.btnTypeDel.Size = new System.Drawing.Size(23, 22);
            this.btnTypeDel.Click += new System.EventHandler(this.btnTypeDel_Click);
            // 
            // myToolStrip1
            // 
            this.myToolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip1.DisableAllMenuItems = true;
            this.myToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnConfigAdd,
            this.btnConfigDel});
            this.myToolStrip1.Location = new System.Drawing.Point(682, 84);
            this.myToolStrip1.Name = "myToolStrip1";
            this.myToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip1.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip1.TabIndex = 132;
            this.myToolStrip1.Text = "myToolStrip1";
            // 
            // btnConfigAdd
            // 
            this.btnConfigAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConfigAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnConfigAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConfigAdd.Name = "btnConfigAdd";
            this.btnConfigAdd.Size = new System.Drawing.Size(23, 22);
            this.btnConfigAdd.Click += new System.EventHandler(this.btnConfigAdd_Click);
            // 
            // btnConfigDel
            // 
            this.btnConfigDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConfigDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnConfigDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConfigDel.Name = "btnConfigDel";
            this.btnConfigDel.Size = new System.Drawing.Size(23, 22);
            this.btnConfigDel.Click += new System.EventHandler(this.btnConfigDel_Click);
            // 
            // ServiceConfigs
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridConfig);
            this.Controls.Add(this.gridType);
            this.Controls.Add(this.myToolStrip1);
            this.Controls.Add(this.myToolStrip2);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label4);
            this.Name = "ServiceConfigs";
            this.Size = new System.Drawing.Size(774, 545);
            this.Load += new System.EventHandler(this.ServiceConfigs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridConfig)).EndInit();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.myToolStrip1.ResumeLayout(false);
            this.myToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion		
		#endregion

		private ToolSet toolSet
		{
			get
			{
				return (this.Tag as Modules.AdminModule.AdminMain).ToolSet;
			}
		}


		private void ServiceConfigs_Load(object sender, System.EventArgs e)
		{
			BindControls();
		}

		private void BindControls()
		{
			gridType.SetDataBinding(toolSet, "ServiceType");
			gridConfig.SetDataBinding(toolSet, "ServiceType.ServiceTypeServiceConfigType");
		}
		
		private void EndEdits()
		{
			this.gridType.UpdateData();
			this.gridConfig.UpdateData();
		}
		

		#region Types
		private void gridType_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
                if (c.Key == "ServiceTypeName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Service Types";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "BookingStartName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Start Text";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "BookingEndName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "End Text";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "NumberOfDaysName")
                {
                    c.Hidden = true; // TODO WIP hide stype function
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Days Text";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

		    e.Layout.Bands[0].Columns["BookingStartName"].Width = 60;
            e.Layout.Bands[0].Columns["BookingEndName"].Width = 60;
		}

		private void btnTypeAdd_Click(object sender, System.EventArgs e)
		{
			// add new row
			ToolSet.ServiceTypeRow r = toolSet.ServiceType.NewServiceTypeRow();
			r.ServiceTypeName = App.CreateUniqueNameValue(gridType.Rows, "ServiceTypeName", "New Service Type");
			toolSet.ServiceType.AddServiceTypeRow(r);

			// select new row
			gridType.Rows.Refresh(RefreshRow.ReloadData);
			gridType.ActiveRow = gridType.Rows[gridType.Rows.Count-1];
			gridType.ActiveCell = gridType.ActiveRow.Cells["ServiceTypeName"];
			gridType.PerformAction(UltraGridAction.EnterEditMode, false, false);
		}

		private void btnTypeDel_Click(object sender, System.EventArgs e)
		{
			UltraGridRow row;
			row = gridType.ActiveRow;
            
            if (row != null && App.AskDeleteRow())
            {
                row.Cells["IsDeleted"].Value = true;
                gridType.DataBind();
            }
		}
		#endregion

		#region Configurations
		private void gridConfig_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
				if(c.Key == "ServiceConfigTypeName")
				{
					c.CellActivation = Activation.AllowEdit;
                    c.Header.Caption = "Service Type Configurations";
                    c.CellClickAction = CellClickAction.Edit;
					c.Band.SortedColumns.Add(c, false);
				}
				else
					c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
		}

		private void btnConfigAdd_Click(object sender, System.EventArgs e)
		{
			// add new row
			ToolSet.ServiceConfigTypeRow r = toolSet.ServiceConfigType.NewServiceConfigTypeRow();
			r.ServiceTypeID = (int)gridType.ActiveRow.Cells[0].Value;
			r.ServiceConfigTypeName = App.CreateUniqueNameValue(gridConfig.Rows, "ServiceConfigTypeName", "New Service Config");
			toolSet.ServiceConfigType.AddServiceConfigTypeRow(r);

			// select new row
			gridConfig.Rows.Refresh(RefreshRow.ReloadData);
			gridConfig.ActiveRow = gridConfig.Rows[gridConfig.Rows.Count-1];
			gridConfig.ActiveCell = gridConfig.ActiveRow.Cells["ServiceConfigTypeName"];
			gridConfig.PerformAction(UltraGridAction.EnterEditMode, false, false);
		}

		private void btnConfigDel_Click(object sender, System.EventArgs e)
		{
			UltraGridRow row;
			row = gridConfig.ActiveRow;
			if(row != null && App.AskDeleteRow())
			{
				// delete
				int i = row.Index;
				row.Delete(false);
				// select next row
				if( ( i > gridConfig.Rows.Count-1 ? --i : i) > -1)
					gridConfig.ActiveRow = gridConfig.Rows[i];
			}
		}		
		#endregion		
		
		protected override void OnValidating(CancelEventArgs e)
		{
			EndEdits();
			base.OnValidating (e);
		}

	}
}
