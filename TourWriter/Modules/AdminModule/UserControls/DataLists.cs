using System.Data;
using System.ComponentModel;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.AdminModule.UserControls
{
	public class DataLists : System.Windows.Forms.UserControl
	{
		#region Designer

		private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label label4;
        private Infragistics.Win.UltraWinGrid.UltraGrid gridData;
		private Infragistics.Win.UltraWinGrid.UltraGrid gridTables;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnDel;
		private System.ComponentModel.Container components = null;

		public DataLists()
		{
			InitializeComponent();
		}
		
		protected override void Dispose( bool disposing )
		{
			EndAllEdits();

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
            this.lblHeading = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gridData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gridTables = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnDel = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTables)).BeginInit();
            this.myToolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 28;
            this.lblHeading.Text = "Edit the contents of the default data lists.";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(480, 28);
            this.label4.TabIndex = 27;
            this.label4.Text = "The data here is used to fill the drop-down lists etc throughout the application." +
                " On the left is the data lists, on the right the data can be entered to fill the" +
                " selected list.";
            // 
            // gridData
            // 
            this.gridData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridData.Location = new System.Drawing.Point(304, 116);
            this.gridData.Name = "gridData";
            this.gridData.Size = new System.Drawing.Size(196, 328);
            this.gridData.TabIndex = 36;
            this.gridData.Text = "Data in selected list";
            this.gridData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridData_InitializeLayout);
            // 
            // gridTables
            // 
            this.gridTables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridTables.Location = new System.Drawing.Point(16, 116);
            this.gridTables.Name = "gridTables";
            this.gridTables.Size = new System.Drawing.Size(272, 328);
            this.gridTables.TabIndex = 40;
            this.gridTables.Text = "Data lists";
            this.gridTables.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridTables_InitializeLayout);
            this.gridTables.AfterRowActivate += new System.EventHandler(this.gridTables_AfterRowActivate);
            this.gridTables.BeforeRowDeactivate += new System.ComponentModel.CancelEventHandler(this.gridTables_BeforeRowDeactivate);
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnDel});
            this.myToolStrip2.Location = new System.Drawing.Point(456, 93);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip2.TabIndex = 128;
            this.myToolStrip2.Text = "myToolStrip2";
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(23, 22);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDel
            // 
            this.btnDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(23, 22);
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // DataLists
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridData);
            this.Controls.Add(this.myToolStrip2);
            this.Controls.Add(this.gridTables);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label4);
            this.Name = "DataLists";
            this.Size = new System.Drawing.Size(520, 452);
            this.Load += new System.EventHandler(this.DataLists_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTables)).EndInit();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion					
		#endregion

		private DataTable dataListTables;
		private ToolSet toolSet
		{
			get
			{
				return (this.Tag as Modules.AdminModule.AdminMain).ToolSet;
			}
		}


		private void DataLists_Load(object sender, System.EventArgs e)
		{
			CreateTablesList();
			gridTables.DataSource = this.dataListTables;
		}

		private void CreateTablesList()
		{
            AddDataList("Service types (Accommodation, Activity,..)", "ServiceType",    "ServiceTypeName");
            AddDataList("Payment types (Cheque, Visa,..)",          "PaymentType",      "PaymentTypeName");
			AddDataList("Grade 1",                                  "Grade",			"GradeName"				);
			AddDataList("Grade 2",			                        "GradeExternal",	"GradeExternalName"		);
			AddDataList("Itinerary source (Repeat, New,..)",		"ItinerarySource",	"ItinerarySourceName"	);
			AddDataList("Itinerary status (Proposal, Final,..)",	"ItineraryStatus",	"ItineraryStatusName"	);
			AddDataList("Request status (Requested, Confirmed,..)",	"RequestStatus",	"RequestStatusName"		);
			AddDataList("Age groups (Adult, Child, Infant,..)",		"AgeGroup",			"AgeGroupName"			);
			AddDataList("Credit cards (Visa,..)",					"CreditCard",		"CreditCardName"		);			
			AddDataList("Office departments (FIT, Tours,..)",		"Department",		"DepartmentName"		);
			AddDataList("Office branches",							"Branch",			"BranchName"			);
            AddDataList("Contact categories",                       "ContactCategory",  "ContactCategoryName"   );
            AddDataList("Content types",                            "ContentType",      "ContentTypeName"       );
            AddDataList("Itinerary types",                          "ItineraryType",    "ItineraryTypeName"     );
		}
		
		private void EndAllEdits()
		{
			this.gridData.UpdateData();
			this.gridTables.UpdateData();
		}


		#region List tables
		private void gridTables_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
                if (c.Key == "DisplayText")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Lists";
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

			if(gridTables.Rows.Count > 0) 
				gridTables.ActiveRow = gridTables.Rows[0];
		}

		private void gridTables_BeforeRowDeactivate(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				// end binding edits child grid
				this.gridData.UpdateData();

				this.BindingContext[toolSet, GetCurrentDataTable("TableName")].EndCurrentEdit();
				this.gridData.DataBindings.Clear();
				this.gridData.DataSource = null;
			}
			catch(System.ArgumentException){ /* no such table, already handled in activate */ }
		}
		
		private void gridTables_AfterRowActivate(object sender, System.EventArgs e)
		{
			try
			{
				// bind child grid
				gridData.Text = "Data in  '" + GetCurrentDataTable("TableName") + "'  list";
				this.gridData.SetDataBinding(toolSet, GetCurrentDataTable("TableName"));
			}
			catch(System.ArgumentException)
			{
				App.ShowError("Couldn't find table '" + GetCurrentDataTable("TableName") + "'");
			}
		}

		#endregion

		#region List data
		private void gridData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
		{
			// show/hide columns 
			foreach(UltraGridColumn c in e.Layout.Bands[0].Columns)
			{
                if (c.Key == this.GetCurrentDataTable("ColumnName"))
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "List values";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
			}
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

			if(gridData.Rows.Count > 0) 
				gridData.ActiveRow = gridData.Rows[0];
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			DataRow dr = toolSet.Tables[GetCurrentDataTable("TableName")].NewRow();
			dr[GetCurrentDataTable("ColumnName")] = App.CreateUniqueNameValue(
				gridData.Rows, GetCurrentDataTable("ColumnName"), "New " + GetCurrentDataTable("TableName"));
			toolSet.Tables[GetCurrentDataTable("TableName")].Rows.Add(dr);

			// select new row
			gridData.Rows.Refresh(RefreshRow.ReloadData);
			gridData.ActiveCell = gridData.Rows[gridData.Rows.Count-1].Cells[GetCurrentDataTable("ColumnName")];
			gridData.PerformAction(UltraGridAction.EnterEditMode, false, false);
		}

		private void btnDel_Click(object sender, System.EventArgs e)
		{
			UltraGridRow row;
			row = gridData.ActiveRow;
			if(row != null && App.AskDeleteRow())
			{
				// delete
				int i = row.Index;
				row.Delete(false);
				// select next row
				if( ( i > gridData.Rows.Count-1 ? --i : i) > -1)
					gridData.ActiveRow = gridData.Rows[i];
			}
		}
		#endregion
		
		protected override void OnValidating(CancelEventArgs e)
		{
			EndAllEdits();
			base.OnValidating (e);
		}
		
		private string GetCurrentDataTable(string colName)
		{
			if(this.gridTables.ActiveRow != null)
				return this.gridTables.ActiveRow.Cells[colName].Value.ToString();
			return "";
		}

		private void AddDataList(string displayText, string tableName, string columnName)
		{
			if(dataListTables == null)
			{			
				// create data for the data lists
				dataListTables = new DataTable("DataLists");
				dataListTables.Columns.Add("DisplayText", typeof(System.String));
				dataListTables.Columns.Add("TableName",   typeof(System.String));
				dataListTables.Columns.Add("ColumnName",  typeof(System.String));
			}
			dataListTables.Rows.Add(new string[]{displayText, tableName, columnName});
		}
	}
}
