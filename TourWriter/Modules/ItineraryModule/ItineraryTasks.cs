using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.ItineraryModule
{
    public class ItineraryTasks : UserControl
    {
        private ItinerarySet _itinerarySet;
        internal ItinerarySet ItinerarySet
        {
            get { return _itinerarySet; }
            set
            {
                _itinerarySet = value;
                gridTasks.DataSource = _itinerarySet.Task;
            }
        }

        public ItineraryTasks()
        {
            InitializeComponent();
        }

        private int AddNewTask(string name)
        {
            //TODO: WTF HACK!!
            //Code below (add Task and Itinerary task rows) causes Constraint Exception on ItinerarySet save.
            //Workaround is to also edit any other table/row/field before saving, manualy or prog.
            //So lets first bullshit-edit the ItineryName...
            _itinerarySet.Itinerary[0].ItineraryName = _itinerarySet.Itinerary[0].ItineraryName;

            // add new task row and new itinerarytask row           
            var task = _itinerarySet.Task.NewTaskRow();
            task.TaskName = name;
            _itinerarySet.Task.AddTaskRow(task);
            //task.EndEdit();

            // add the itinerary task
            var i = _itinerarySet.ItineraryTask.NewItineraryTaskRow();
            i.TaskID = task.TaskID;
            i.ItineraryID = _itinerarySet.Itinerary[0].ItineraryID;
            _itinerarySet.ItineraryTask.AddItineraryTaskRow(i);

            return task.TaskID;
        }

        private void ItineraryTasks_Load(object sender, EventArgs e)
        {
        }

        private void gridType_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            //add custom tick box           
            if (!e.Layout.Bands[0].Columns.Exists("IsComplete"))
                e.Layout.Bands[0].Columns.Add("IsComplete");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "IsComplete")
                {
                    c.Header.Caption = "";
                    c.Header.VisiblePosition = 0;
                    c.DataType = typeof(bool);
                    //c.Style = ColumnStyle.CheckBox;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "TaskName")
                {
                    c.Header.Caption = "Name";
                    c.Header.VisiblePosition = 1;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.SortIndicator = SortIndicator.Ascending;
                }
                else if (c.Key == "DateDue")
                {
                    c.Header.Caption = "Date Due";
                    c.Header.VisiblePosition = 2;
                    c.CellActivation = Activation.AllowEdit;
                    //c.Style = ColumnStyle.Date;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                    c.Width = 80;
                }
                else if (c.Key == "DateCompleted")
                {
                    c.Header.Caption = "Date Completed";
                    c.Header.VisiblePosition = 3;
                    c.CellActivation = Activation.AllowEdit;
                    //c.Style = ColumnStyle.Date;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.OnRowActivate;
                    c.Width = 80;
                }
                else if (c.Key == "Note")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Note";
                    c.Header.VisiblePosition = 4;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
            
            e.Layout.Bands[0].Columns["IsComplete"].Width = 20;
            e.Layout.Bands[0].Columns["TaskName"].Width = 80;
            e.Layout.Bands[0].Columns["DateDue"].Width = 40;
            e.Layout.Bands[0].Columns["DateCompleted"].Width = 40;
            e.Layout.Bands[0].Columns["Note"].Width = 200;

            var index = 0;
            e.Layout.Bands[0].Columns["IsComplete"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["TaskName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["DateDue"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["DateCompleted"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Note"].Header.VisiblePosition = index++;
        }
        
        private void gridTasks_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["IsComplete"].Value = (e.Row.Cells["DateCompleted"].Value != DBNull.Value);
            e.Row.Cells["TaskName"].Appearance.FontData.Strikeout = 
                (bool)e.Row.Cells["IsComplete"].Value ? DefaultableBoolean.True : DefaultableBoolean.False;
        }

        private void gridTasks_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "IsComplete")
            {
                var ticked = (bool) e.Cell.EditorResolved.Value;
                if (ticked) e.Cell.Row.Cells["DateCompleted"].Value = DateTime.Now;
                else  e.Cell.Row.Cells["DateCompleted"].Value = DBNull.Value;
            }
        }

        private void btnTasksAdd_Click(object sender, EventArgs e)
        {
            var id = AddNewTask("New task...");
            GridHelper.SetActiveRow(gridTasks, "TaskID", id, "TaskName");
        }

        private void btnTasksDel_Click(object sender, EventArgs e)
        {     
            if (gridTasks.ActiveRow != null && App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridTasks, true);
        }
        
        private void btnPopulate_Click(object sender, EventArgs e)
        {
            if (!App.AskYesNo("Do you want to add the Default Tasks (only adds where missing)?")) return;

            var defaults = Global.Cache.ToolSet.Task.Where(x => x.RowState != DataRowState.Deleted).Select(x => x.TaskName);
            var existing = _itinerarySet.Task.Where(x => x.RowState != DataRowState.Deleted).Select(x => x.TaskName);

            foreach (var task in defaults.Except(existing))
                AddNewTask(task);
        }
        
        // ---------------------------------------------------------------------------
        #region Designer

        private Infragistics.Win.UltraWinGrid.UltraGrid gridTasks;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnTaskAdd;
        private ToolStripButton btnTaskDel;
        private ToolStripButton btnPopulate;
        private ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.IContainer components;
        
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItineraryTasks));
            this.gridTasks = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnTaskAdd = new System.Windows.Forms.ToolStripButton();
            this.btnTaskDel = new System.Windows.Forms.ToolStripButton();
            this.btnPopulate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).BeginInit();
            this.myToolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridTasks
            // 
            this.gridTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridTasks.Location = new System.Drawing.Point(0, 21);
            this.gridTasks.Name = "gridTasks";
            this.gridTasks.Size = new System.Drawing.Size(505, 300);
            this.gridTasks.TabIndex = 14;
            this.gridTasks.Text = "Tasks";
            this.gridTasks.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridType_InitializeLayout);
            this.gridTasks.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.gridTasks_InitializeRow);
            this.gridTasks.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.gridTasks_CellChange);
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPopulate,
            this.toolStripSeparator1,
            this.btnTaskAdd,
            this.btnTaskDel});
            this.myToolStrip2.Location = new System.Drawing.Point(363, -2);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(144, 25);
            this.myToolStrip2.TabIndex = 130;
            this.myToolStrip2.Text = "myToolStrip2";
            this.myToolStrip2.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.myToolStrip2_ItemClicked);
            // 
            // btnTaskAdd
            // 
            this.btnTaskAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTaskAdd.Image = global::TourWriter.Properties.Resources.Plus;
            this.btnTaskAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTaskAdd.Name = "btnTaskAdd";
            this.btnTaskAdd.Size = new System.Drawing.Size(23, 22);
            this.btnTaskAdd.ToolTipText = "Add new Task";
            this.btnTaskAdd.Click += new System.EventHandler(this.btnTasksAdd_Click);
            // 
            // btnTaskDel
            // 
            this.btnTaskDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTaskDel.Image = global::TourWriter.Properties.Resources.Remove;
            this.btnTaskDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTaskDel.Name = "btnTaskDel";
            this.btnTaskDel.Size = new System.Drawing.Size(23, 22);
            this.btnTaskDel.ToolTipText = "Delete selecte Task";
            this.btnTaskDel.Click += new System.EventHandler(this.btnTasksDel_Click);
            // 
            // btnPopulate
            // 
            this.btnPopulate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPopulate.Image = ((System.Drawing.Image)(resources.GetObject("btnPopulate.Image")));
            this.btnPopulate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPopulate.Name = "btnPopulate";
            this.btnPopulate.Size = new System.Drawing.Size(58, 22);
            this.btnPopulate.Text = "Populate";
            this.btnPopulate.ToolTipText = "Re/Populate from Default Tasks";
            this.btnPopulate.Click += new System.EventHandler(this.btnPopulate_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ItineraryTasks
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridTasks);
            this.Controls.Add(this.myToolStrip2);
            this.Name = "ItineraryTasks";
            this.Size = new System.Drawing.Size(505, 321);
            this.Load += new System.EventHandler(this.ItineraryTasks_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).EndInit();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void myToolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        #endregion
    }
}
