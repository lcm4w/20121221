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
    public class OptionConfigs : System.Windows.Forms.UserControl
    {
        #region Designer

        private Infragistics.Win.UltraWinGrid.UltraGrid gridType;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label label4;
        private TourWriter.UserControls.MyToolStrip myToolStrip2;
        private ToolStripButton btnTypeAdd;
        private ToolStripButton btnTypeDel;
        private System.ComponentModel.Container components = null;

        public OptionConfigs()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            this.EndEdits();

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
            this.gridType = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.myToolStrip2 = new TourWriter.UserControls.MyToolStrip();
            this.btnTypeAdd = new System.Windows.Forms.ToolStripButton();
            this.btnTypeDel = new System.Windows.Forms.ToolStripButton();
            this.myToolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridType
            // 
            this.gridType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridType.Location = new System.Drawing.Point(16, 107);
            this.gridType.Name = "gridType";
            this.gridType.Size = new System.Drawing.Size(476, 213);
            this.gridType.TabIndex = 14;
            this.gridType.Text = "Option Types";
            this.gridType.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.gridType_InitializeLayout);
            // 
            // lblHeading
            // 
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblHeading.Location = new System.Drawing.Point(12, 4);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(480, 28);
            this.lblHeading.TabIndex = 28;
            this.lblHeading.Text = "Add Option types and their configurations";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(480, 32);
            this.label4.TabIndex = 27;
            this.label4.Text = "Option types help define the type of each service option (eg. Double, Trip" +
                "le). The Divisor column specifies how the cost of an option will be divided amonst the pax (eg. set Single to divide the cost by 1, Double by 2, Triple by 3, etc).";
            // 
            // myToolStrip2
            // 
            this.myToolStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myToolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.myToolStrip2.DisableAllMenuItems = true;
            this.myToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.myToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.myToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnTypeAdd,
            this.btnTypeDel});
            this.myToolStrip2.Location = new System.Drawing.Point(446, 84);
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
            // OptionConfigs
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.gridType);
            this.Controls.Add(this.myToolStrip2);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label4);
            this.Name = "OptionConfigs";
            this.Size = new System.Drawing.Size(505, 321);
            this.Load += new System.EventHandler(this.OptionConfigs_Load);
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


        private void OptionConfigs_Load(object sender, System.EventArgs e)
        {
            BindControls();
        }

        private void BindControls()
        {
            gridType.SetDataBinding(toolSet, "OptionType");
        }

        private void EndEdits()
        {
            this.gridType.UpdateData();
        }

        private void gridType_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        { 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "OptionTypeName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Option Types";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "Divisor")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Divisor";
                    c.CellClickAction = CellClickAction.Edit;
                    c.Header.ToolTipText = "How the cost will be divided out - eg divide 'Double' by 2 pax";
                }
                else
                    c.Hidden = true;
            }
            // configure
            GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void btnTypeAdd_Click(object sender, System.EventArgs e)
        {
            ToolSet.OptionTypeRow r = toolSet.OptionType.NewOptionTypeRow();
            r.OptionTypeName = App.CreateUniqueNameValue(gridType.Rows, "OptionTypeName", "New Option Type");
            toolSet.OptionType.AddOptionTypeRow(r);
            GridHelper.SetActiveRow(gridType, "OptionTypeID", r.OptionTypeID, "OptionTypeName");
        }

        private void btnTypeDel_Click(object sender, System.EventArgs e)
        {     
            if (gridType.ActiveRow != null && App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridType, true);
        }
        
        protected override void OnValidating(CancelEventArgs e)
        {
            EndEdits();
            base.OnValidating(e);
        }
    }
}
