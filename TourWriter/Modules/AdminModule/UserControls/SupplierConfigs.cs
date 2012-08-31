using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.AdminModule.UserControls
{
    /// <summary>
    /// Summary description for SupplierConfigs.
    /// </summary>
    public class SupplierConfigs : UserControl
    {
        #region Designer

        private UltraGrid gridConfig;
        private Label lblHeading;
        private Label label4;
        private Label label1;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnConfigAdd;
        private ToolStripButton btnConfigDel;
        private Container components = null;

        public SupplierConfigs()
        {
            InitializeComponent();
        }


        protected override void Dispose(bool disposing)
        {
            EndEdits();

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
            this.gridConfig = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnConfigAdd = new System.Windows.Forms.ToolStripButton();
            this.btnConfigDel = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridConfig)).BeginInit();
            this.myToolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridConfig
            // 
            this.gridConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gridConfig.Location = new System.Drawing.Point(276, 110);
            this.gridConfig.Name = "gridConfig";
            this.gridConfig.Size = new System.Drawing.Size(220, 326);
            this.gridConfig.TabIndex = 19;
            this.gridConfig.Text = "Supplier Configurations";
            this.gridConfig.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridConfig_InitializeLayout);
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(496, 28);
            this.lblHeading.TabIndex = 28;
            this.lblHeading.Text = "Add supplier configurations";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(496, 32);
            this.label4.TabIndex = 27;
            this.label4.Text = "Supplier configurations help to describe a supplier. For each supplier, you can t" +
                "ick which configurations it matches. Some examples are: Swimming pool, pet frien" +
                "dly, bar service, etc).";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 56);
            this.label1.TabIndex = 29;
            this.label1.Text = "NOTE: these configurations do not apply to the services from a supplier. For serv" +
                "ice types and configurations, choose \'Service configs\' from the menu.";
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnConfigAdd,
            this.btnConfigDel});
            this.myToolStrip2.Location = new System.Drawing.Point(453, 87);
            this.myToolStrip2.Name = "myToolStrip2";
            this.myToolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.myToolStrip2.Size = new System.Drawing.Size(49, 25);
            this.myToolStrip2.TabIndex = 132;
            this.myToolStrip2.Text = "myToolStrip2";
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
            // SupplierConfigs
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridConfig);
            this.Controls.Add(this.myToolStrip2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label4);
            this.Name = "SupplierConfigs";
            this.Size = new System.Drawing.Size(520, 452);
            this.Load += new System.EventHandler(this.SupplierConfigs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridConfig)).EndInit();
            this.myToolStrip2.ResumeLayout(false);
            this.myToolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #endregion

        private ToolSet toolSet
        {
            get { return (Tag as AdminMain).ToolSet; }
        }


        private void SupplierConfigs_Load(object sender, EventArgs e)
        {
            BindControls();
        }

        private void BindControls()
        {
            gridConfig.SetDataBinding(toolSet, "SupplierConfigType");
        }

        private void EndEdits()
        {
            gridConfig.UpdateData();
        }

        #region Configurations

        private void gridConfig_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "SupplierConfigTypeName")
                {
                    c.CellActivation = Activation.AllowEdit;
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Supplier Configurations";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void btnConfigAdd_Click(object sender, EventArgs e)
        {
            // add new row
            ToolSet.SupplierConfigTypeRow r = toolSet.SupplierConfigType.NewSupplierConfigTypeRow();
            r.SupplierConfigTypeName =
                App.CreateUniqueNameValue(gridConfig.Rows, "SupplierConfigTypeName", "New Supplier Config");
            toolSet.SupplierConfigType.AddSupplierConfigTypeRow(r);

            // select new row
            gridConfig.Rows.Refresh(RefreshRow.ReloadData);
            gridConfig.ActiveRow = gridConfig.Rows[gridConfig.Rows.Count - 1];
            gridConfig.ActiveCell = gridConfig.ActiveRow.Cells["SupplierConfigTypeName"];
            gridConfig.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }

        private void btnConfigDel_Click(object sender, EventArgs e)
        {
            UltraGridRow row;
            row = gridConfig.ActiveRow;
            if (row != null && App.AskDeleteRow())
            {
                // delete
                int i = row.Index;
                row.Delete(false);
                // select next row
                if ((i > gridConfig.Rows.Count - 1 ? --i : i) > -1)
                    gridConfig.ActiveRow = gridConfig.Rows[i];
            }
        }

        #endregion

        protected override void OnValidating(CancelEventArgs e)
        {
            EndEdits();
            base.OnValidating(e);
        }
    }
}