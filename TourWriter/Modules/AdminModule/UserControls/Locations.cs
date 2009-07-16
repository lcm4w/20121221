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
	/// Summary description for Locations.
	/// </summary>
	public class Locations : System.Windows.Forms.UserControl
	{
		#region Designer

		private System.Windows.Forms.Label lblHeading;
		private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridCountry;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridState;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridRegion;
		private Infragistics.Win.UltraWinGrid.UltraGrid gridCity;
		private System.Windows.Forms.Label label5;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnCountryAdd;
        private ToolStripButton btnCountryDel;
        private TourWriter.UserControls.MyToolStrip myToolStrip1;
        private ToolStripButton btnStateAdd;
        private ToolStripButton btnStateDel;
        private TourWriter.UserControls.MyToolStrip myToolStrip3;
        private ToolStripButton btnRegionAdd;
        private ToolStripButton btnRegionDel;
        private TourWriter.UserControls.MyToolStrip myToolStrip4;
        private ToolStripButton btnCityAdd;
        private ToolStripButton btnCityDel;
		private System.ComponentModel.Container components = null;

		public Locations()
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
            this.gridCountry = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gridState = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gridRegion = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gridCity = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.myToolStrip4 = new TourWriter.UserControls.MyToolStrip();
            this.btnCityAdd = new System.Windows.Forms.ToolStripButton();
            this.btnCityDel = new System.Windows.Forms.ToolStripButton();
            this.myToolStrip3 = new TourWriter.UserControls.MyToolStrip();
            this.btnRegionAdd = new System.Windows.Forms.ToolStripButton();
            this.btnRegionDel = new System.Windows.Forms.ToolStripButton();
            this.myToolStrip1 = new TourWriter.UserControls.MyToolStrip();
            this.btnStateAdd = new System.Windows.Forms.ToolStripButton();
            this.btnStateDel = new System.Windows.Forms.ToolStripButton();
            this.label5 = new System.Windows.Forms.Label();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnCountryAdd = new System.Windows.Forms.ToolStripButton();
            this.btnCountryDel = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridCountry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRegion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCity)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.myToolStrip4.SuspendLayout();
            this.myToolStrip3.SuspendLayout();
            this.myToolStrip1.SuspendLayout();
            this.myToolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridCountry
            // 
            this.gridCountry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridCountry.Location = new System.Drawing.Point(8, 118);
            this.gridCountry.Name = "gridCountry";
            this.gridCountry.Size = new System.Drawing.Size(385, 370);
            this.gridCountry.TabIndex = 14;
            this.gridCountry.Text = "Countries";
            this.gridCountry.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridCountry_InitializeLayout);
            // 
            // gridState
            // 
            this.gridState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridState.Location = new System.Drawing.Point(4, 44);
            this.gridState.Name = "gridState";
            this.gridState.Size = new System.Drawing.Size(130, 320);
            this.gridState.TabIndex = 19;
            this.gridState.Text = "States";
            this.gridState.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridState_InitializeLayout);
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(492, 28);
            this.lblHeading.TabIndex = 28;
            this.lblHeading.Text = "Locations data";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(492, 28);
            this.label4.TabIndex = 27;
            this.label4.Text = "The locations data is inter-connected, so start by entering a country, then the s" +
                "tates of that country, then the regions of that state, and finally the cities of" +
                " that region.";
            // 
            // gridRegion
            // 
            this.gridRegion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridRegion.Location = new System.Drawing.Point(140, 44);
            this.gridRegion.Name = "gridRegion";
            this.gridRegion.Size = new System.Drawing.Size(130, 320);
            this.gridRegion.TabIndex = 31;
            this.gridRegion.Text = "Regions";
            this.gridRegion.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridRegion_InitializeLayout);
            // 
            // gridCity
            // 
            this.gridCity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridCity.Location = new System.Drawing.Point(276, 44);
            this.gridCity.Name = "gridCity";
            this.gridCity.Size = new System.Drawing.Size(130, 320);
            this.gridCity.TabIndex = 34;
            this.gridCity.Text = "Cities";
            this.gridCity.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridCity_InitializeLayout);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.gridCity);
            this.groupBox1.Controls.Add(this.gridState);
            this.groupBox1.Controls.Add(this.gridRegion);
            this.groupBox1.Controls.Add(this.myToolStrip3);
            this.groupBox1.Controls.Add(this.myToolStrip1);
            this.groupBox1.Controls.Add(this.myToolStrip4);
            this.groupBox1.Location = new System.Drawing.Point(408, 118);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 370);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Country detail";
            // 
            // myToolStrip4
            // 
            this.myToolStrip4.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip4.DisableAllMenuItems = true;
            this.myToolStrip4.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip4.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCityAdd,
            this.btnCityDel});
            this.myToolStrip4.Location = new System.Drawing.Point(362, 21);
            this.myToolStrip4.Name = "myToolStrip4";
            this.myToolStrip4.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip4.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip4.TabIndex = 134;
            this.myToolStrip4.Text = "myToolStrip4";
            // 
            // btnCityAdd
            // 
            this.btnCityAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCityAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnCityAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCityAdd.Name = "btnCityAdd";
            this.btnCityAdd.Size = new System.Drawing.Size(23, 22);
            this.btnCityAdd.Click += new System.EventHandler(this.btnCityAdd_Click);
            // 
            // btnCityDel
            // 
            this.btnCityDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCityDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnCityDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCityDel.Name = "btnCityDel";
            this.btnCityDel.Size = new System.Drawing.Size(23, 22);
            this.btnCityDel.Click += new System.EventHandler(this.btnCityDel_Click);
            // 
            // myToolStrip3
            // 
            this.myToolStrip3.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip3.DisableAllMenuItems = true;
            this.myToolStrip3.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRegionAdd,
            this.btnRegionDel});
            this.myToolStrip3.Location = new System.Drawing.Point(227, 21);
            this.myToolStrip3.Name = "myToolStrip3";
            this.myToolStrip3.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip3.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip3.TabIndex = 132;
            this.myToolStrip3.Text = "myToolStrip3";
            // 
            // btnRegionAdd
            // 
            this.btnRegionAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRegionAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnRegionAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRegionAdd.Name = "btnRegionAdd";
            this.btnRegionAdd.Size = new System.Drawing.Size(23, 22);
            this.btnRegionAdd.Click += new System.EventHandler(this.btnRegionAdd_Click);
            // 
            // btnRegionDel
            // 
            this.btnRegionDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRegionDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnRegionDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRegionDel.Name = "btnRegionDel";
            this.btnRegionDel.Size = new System.Drawing.Size(23, 22);
            this.btnRegionDel.Click += new System.EventHandler(this.btnRegionDel_Click);
            // 
            // myToolStrip1
            // 
            this.myToolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip1.DisableAllMenuItems = true;
            this.myToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnStateAdd,
            this.btnStateDel});
            this.myToolStrip1.Location = new System.Drawing.Point(91, 21);
            this.myToolStrip1.Name = "myToolStrip1";
            this.myToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip1.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip1.TabIndex = 130;
            this.myToolStrip1.Text = "myToolStrip1";
            // 
            // btnStateAdd
            // 
            this.btnStateAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStateAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnStateAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStateAdd.Name = "btnStateAdd";
            this.btnStateAdd.Size = new System.Drawing.Size(23, 22);
            this.btnStateAdd.Click += new System.EventHandler(this.btnStateAdd_Click);
            // 
            // btnStateDel
            // 
            this.btnStateDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStateDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnStateDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStateDel.Name = "btnStateDel";
            this.btnStateDel.Size = new System.Drawing.Size(23, 22);
            this.btnStateDel.Click += new System.EventHandler(this.btnStateDel_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(492, 16);
            this.label5.TabIndex = 46;
            this.label5.Text = "NOTE: only enter the data you need, so that the dropdown boxs and data transfers " +
                "work efficiently.";
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCountryAdd,
            this.btnCountryDel});
            this.myToolStrip2.Location = new System.Drawing.Point(349, 95);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip2.TabIndex = 128;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnCountryAdd
            // 
            this.btnCountryAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCountryAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnCountryAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCountryAdd.Name = "btnCountryAdd";
            this.btnCountryAdd.Size = new System.Drawing.Size(23, 22);
            this.btnCountryAdd.Click += new System.EventHandler(this.btnCountryAdd_Click);
            // 
            // btnCountryDel
            // 
            this.btnCountryDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCountryDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnCountryDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCountryDel.Name = "btnCountryDel";
            this.btnCountryDel.Size = new System.Drawing.Size(23, 22);
            this.btnCountryDel.Click += new System.EventHandler(this.btnCountryDel_Click);
            // 
            // Locations
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridCountry);
            this.Controls.Add(this.myToolStrip2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label4);
            this.Name = "Locations";
            this.Size = new System.Drawing.Size(869, 529);
            this.Load += new System.EventHandler(this.Locations_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridCountry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRegion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCity)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.myToolStrip4.ResumeLayout(false);
            this.myToolStrip4.PerformLayout();
            this.myToolStrip3.ResumeLayout(false);
            this.myToolStrip3.PerformLayout();
            this.myToolStrip1.ResumeLayout(false);
            this.myToolStrip1.PerformLayout();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
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


		private void Locations_Load(object sender, System.EventArgs e)
		{
			BindControls();
		}

		private void BindControls()
		{		
			// bind types
			gridCountry.SetDataBinding(toolSet, "Country");
			
			// bind configs
			gridState.SetDataBinding(toolSet, "Country.CountryState");
			gridRegion.SetDataBinding(toolSet, "Country.CountryState.StateRegion");
			gridCity.SetDataBinding(toolSet, "Country.CountryState.StateRegion.RegionCity");		
		}
		
		private void EndEdits()
		{
			this.gridCountry.UpdateData();
			this.gridState.UpdateData();
			this.gridRegion.UpdateData();
			this.gridCity.UpdateData();
		}
		

		#region Country
		private void gridCountry_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{		
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)			
			{
                if (c.Key == "CountryName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Country";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "CountryCode")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Country Code";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "PhoneCode")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Phone Code";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

		    e.Layout.Bands[0].Columns["CountryCode"].Width = 60;
            e.Layout.Bands[0].Columns["PhoneCode"].Width = 60;
		}

		private void btnCountryAdd_Click(object sender, System.EventArgs e)
		{
			// add new row
			ToolSet.CountryRow r = toolSet.Country.NewCountryRow();
			r.CountryName = App.CreateUniqueNameValue(gridCountry.Rows, "CountryName", "New Country");
			toolSet.Country.AddCountryRow(r);

			// select new row
			gridCountry.Rows.Refresh(RefreshRow.ReloadData);
			gridCountry.ActiveRow = gridCountry.Rows[gridCountry.Rows.Count-1];
		}

		private void btnCountryDel_Click(object sender, System.EventArgs e)
		{
			UltraGridRow row;
			row = gridCountry.ActiveRow;
			if(row != null && App.AskDeleteRow())
			{
				// delete
				int i = row.Index;
				row.Delete(false);
				// select next row
				if( ( i > gridCountry.Rows.Count-1 ? --i : i) > -1)
					gridCountry.ActiveRow = gridCountry.Rows[i];
			}
		}
		#endregion

		#region State
		private void gridState_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{		
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
                if (c.Key == "StateName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "States";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
		}

		private void btnStateAdd_Click(object sender, System.EventArgs e)
		{
			if(gridCountry.ActiveRow != null)
			{
				ToolSet.StateRow r = toolSet.State.NewStateRow();
				r.StateName = App.CreateUniqueNameValue( gridState.Rows, "StateName", "New State");
				r.CountryID = (int)gridCountry.ActiveRow.Cells["CountryID"].Value;
				toolSet.State.AddStateRow(r);

				// select new row
				gridState.Rows.Refresh(RefreshRow.ReloadData);
				gridState.ActiveCell = gridState.Rows[gridState.Rows.Count-1].Cells["StateName"];
				gridState.PerformAction(UltraGridAction.EnterEditMode, false, false);
			}
			else
				MessageBox.Show(App.GetResourceString("ShowRowNotSelected"));
		}

		private void btnStateDel_Click(object sender, System.EventArgs e)
		{
			UltraGridRow row;
			row = gridState.ActiveRow;
			if(row != null && App.AskDeleteRow())
			{
				// delete
				int i = row.Index;
				row.Delete(false);
				// select next row
				if( ( i > gridState.Rows.Count-1 ? --i : i) > -1)
					gridState.ActiveRow = gridState.Rows[i];
			}	
		}
		#endregion			
			
		#region Region
		private void gridRegion_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{		
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)			
			{
                if (c.Key == "RegionName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Regions";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
		}

		private void btnRegionAdd_Click(object sender, System.EventArgs e)
		{
			if(gridState.ActiveRow != null)
			{
				ToolSet.RegionRow r = toolSet.Region.NewRegionRow();
				r.RegionName = App.CreateUniqueNameValue( gridRegion.Rows, "RegionName", "New Region");
				r.StateID = (int)gridState.ActiveRow.Cells["StateID"].Value;
				toolSet.Region.AddRegionRow(r);

				// select new row
				gridRegion.Rows.Refresh(RefreshRow.ReloadData);
				gridRegion.ActiveCell = gridRegion.Rows[gridRegion.Rows.Count-1].Cells["RegionName"];
				gridRegion.PerformAction(UltraGridAction.EnterEditMode, false, false);
			}
			else
				MessageBox.Show(App.GetResourceString("ShowRowNotSelected"));
		}

		private void btnRegionDel_Click(object sender, System.EventArgs e)
		{
			UltraGridRow row;
			row = gridRegion.ActiveRow;
			if(row != null && App.AskDeleteRow())
			{
				// delete
				int i = row.Index;
				row.Delete(false);
				// select next row
				if( ( i > gridRegion.Rows.Count-1 ? --i : i) > -1)
					gridRegion.ActiveRow = gridRegion.Rows[i];
			}	
		}
		#endregion

		#region City
		private void gridCity_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{			
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)			
			{
                if (c.Key == "CityName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Cities";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
		}	

		private void btnCityAdd_Click(object sender, System.EventArgs e)
		{
			if(gridRegion.ActiveRow != null)
			{
				ToolSet.CityRow r = toolSet.City.NewCityRow();
				r.CityName = App.CreateUniqueNameValue( gridCity.Rows, "CityName", "New City");
				r.RegionID = (int)gridRegion.ActiveRow.Cells["RegionID"].Value;
				toolSet.City.AddCityRow(r);

				// select new row
				gridCity.Rows.Refresh(RefreshRow.ReloadData);
				gridCity.ActiveCell = gridCity.Rows[gridCity.Rows.Count-1].Cells["CityName"];
				gridCity.PerformAction(UltraGridAction.EnterEditMode, false, false);
			}
			else
				MessageBox.Show(App.GetResourceString("ShowRowNotSelected"));
		}

		private void btnCityDel_Click(object sender, System.EventArgs e)
		{
			UltraGridRow row;
			row = gridCity.ActiveRow;
			if(row != null && App.AskDeleteRow())
			{
				// delete
				int i = row.Index;
				row.Delete(false);
				// select next row
				if( ( i > gridCity.Rows.Count-1 ? --i : i) > -1)
					gridCity.ActiveRow = gridCity.Rows[i];
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
