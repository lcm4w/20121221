using System.ComponentModel;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.AdminModule.UserControls
{
	public class Tasks : System.Windows.Forms.UserControl
	{
		#region Designer

		private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label label4;
        private UltraGrid gridTasks;
        private TourWriter.UserControls.MyToolStrip myToolStrip1;
        private System.Windows.Forms.ToolStripButton btnTaskAdd;
        private System.Windows.Forms.ToolStripButton btnTaskDel;
		private System.ComponentModel.Container components = null;

        public Tasks()
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
            this.gridTasks = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.myToolStrip1 = new TourWriter.UserControls.MyToolStrip();
            this.btnTaskAdd = new System.Windows.Forms.ToolStripButton();
            this.btnTaskDel = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).BeginInit();
            this.myToolStrip1.SuspendLayout();
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
            this.lblHeading.Text = "Manage Default Task items";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(480, 28);
            this.label4.TabIndex = 27;
            this.label4.Text = "Enter default tasks so that you can easily populate Itinerary Task lists with rep" +
    "eating tasks";
            // 
            // gridTasks
            // 
            this.gridTasks.Location = new System.Drawing.Point(16, 102);
            this.gridTasks.Name = "gridTasks";
            this.gridTasks.Size = new System.Drawing.Size(263, 328);
            this.gridTasks.TabIndex = 129;
            this.gridTasks.Text = "Default Tasks";
            this.gridTasks.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridTasks_InitializeLayout);
            // 
            // myToolStrip1
            // 
            this.myToolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip1.DisableAllMenuItems = true;
            this.myToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnTaskAdd,
            this.btnTaskDel});
            this.myToolStrip1.Location = new System.Drawing.Point(204, 76);
            this.myToolStrip1.Name = "myToolStrip1";
            this.myToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip1.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip1.TabIndex = 131;
            this.myToolStrip1.Text = "myToolStrip1";
            // 
            // btnTaskAdd
            // 
            this.btnTaskAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTaskAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnTaskAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTaskAdd.Name = "btnTaskAdd";
            this.btnTaskAdd.Size = new System.Drawing.Size(23, 22);
            this.btnTaskAdd.Click += new System.EventHandler(this.btnTaskAdd_Click);
            // 
            // btnTaskDel
            // 
            this.btnTaskDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTaskDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnTaskDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTaskDel.Name = "btnTaskDel";
            this.btnTaskDel.Size = new System.Drawing.Size(23, 22);
            this.btnTaskDel.Click += new System.EventHandler(this.btnTaskDel_Click);
            // 
            // Tasks
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.myToolStrip1);
            this.Controls.Add(this.gridTasks);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label4);
            this.Name = "Tasks";
            this.Size = new System.Drawing.Size(460, 435);
            this.Load += new System.EventHandler(this.Tasks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).EndInit();
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
				return (Tag as AdminMain).ToolSet;
			}
		}

        private void Tasks_Load(object sender, System.EventArgs e)
		{
		    gridTasks.DataSource = toolSet.Task;
		}
		
		private void EndAllEdits()
		{
			gridTasks.UpdateData();
		}

        private void gridTasks_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {           
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {               
                if (c.Key == "TaskName")
                {
                    c.Header.Caption = "Default Itinerary Task Names";
                    c.Header.VisiblePosition = 1;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.SortIndicator = SortIndicator.Ascending;
                }             
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void btnTaskAdd_Click(object sender, System.EventArgs e)
        {           
            // add new task row     
            var task = toolSet.Task.NewTaskRow(); 
            task.TaskName = "New default task...";
            toolSet.Task.AddTaskRow(task);

            var defaultTask = toolSet.DefaultTask.NewDefaultTaskRow();
            defaultTask.TaskID = task.TaskID;
            defaultTask.Type = "itinerary";
            toolSet.DefaultTask.AddDefaultTaskRow(defaultTask);
            GridHelper.SetActiveRow(gridTasks, "TaskID", task.TaskID, "TaskName");
        }

        private void btnTaskDel_Click(object sender, System.EventArgs e)
        {
            if (gridTasks.ActiveRow != null && App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridTasks, true);
        } 

        protected override void OnValidating(CancelEventArgs e)
		{
			EndAllEdits();
			base.OnValidating (e);
		}
	}
}
