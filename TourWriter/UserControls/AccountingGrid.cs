using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Services;
using ButtonDisplayStyle=Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.UserControls
{
    public partial class AccountingGrid : DataExtractGrid
    {
        public bool EnableSetStatus { get; set; }

        public bool EnableSelectBox { get; set; }

        public AccountingGrid()
        {
            InitializeComponent();
        }

        public void SelectAllRows()
        {
            if (!EnableSelectBox)
                return;

            bool unselectAll = true;

            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.IsGroupByRow)
                    continue;

                if ((bool)row.Cells["IsSelected"].Value)
                    continue;

                row.Cells["IsSelected"].Value = true;
                unselectAll = false;
            }
            if (unselectAll)
            {
                // if all of the rows are already selected, then unselect all
                foreach (UltraGridRow row in grid.Rows)
                {
                    if (row.IsGroupByRow)
                        continue;

                    row.Cells["IsSelected"].Value = false;
                }
            }
        }

        public ToolStripButton AddToolStripButton(string text, string toolTipText, EventHandler onClick, bool insertSeperator)
        {
            return AddToolStripButton(text, null, toolTipText, onClick, insertSeperator);
        }

        public ToolStripButton AddToolStripButton(Image image, string toolTipText, EventHandler onClick, bool insertSeperator)
        {
            return AddToolStripButton(null, image, toolTipText, onClick, insertSeperator);
        }

        public ToolStripButton AddToolStripButton(string text, Image image, string toolTipText, EventHandler onClick, bool insertSeperator)
        {
            var button = new ToolStripButton(text, image, onClick);
            button.ImageScaling = ToolStripItemImageScaling.None;
            button.ToolTipText = toolTipText;

            if (insertSeperator)
                ToolStrip.Items.Insert(0, new ToolStripSeparator());

            ToolStrip.Items.Insert(0, button);

            return button;
        }

        private void InitializeGrid()
        {
            if (EnableSelectBox)
            {
                // add select all button to the grid toolstrip
                const string toolTipText = "Select/unselect all grid rows.";
                AddToolStripButton(Resources.CheckBox, toolTipText, delegate { SelectAllRows(); }, true);
            }
            if (EnableSetStatus)
            {
                BuildStatusList();

                // create status list combo box
                var cmbStatusList = new ToolStripComboBox();
                if (cmbStatusList.ComboBox == null)
                    return;

                cmbStatusList.Name = "cmbStatusList";
                cmbStatusList.ToolTipText = "Set status on all items";

                cmbStatusList.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbStatusList.ComboBox.ValueMember = "Value";
                cmbStatusList.ComboBox.DisplayMember = "Key";
                cmbStatusList.ComboBox.SelectedValueChanged += cmbStatusList_SelectedValueChanged;

                // populate combo box with items
                foreach (KeyValuePair<string, string> kvp in GetStatusList())
                {
                    cmbStatusList.ComboBox.Items.Add(kvp);
                }
                cmbStatusList.SelectedIndex = 0;

                // add status list combo box to a drop down menu
                var menuSetStatus = new ToolStripDropDownButton();
                menuSetStatus.Name = "menuSetStatus";
                menuSetStatus.Text = "Set status";
                menuSetStatus.DropDownItems.Add(cmbStatusList);
                ToolStrip.Items.Insert(0, new ToolStripSeparator());
                ToolStrip.Items.Insert(0, menuSetStatus);
            }
        }

        private void BuildStatusList()
        {
            if (!grid.DisplayLayout.ValueLists.Exists("StatusList"))
            {
                // create the value list if it doesn't exist
                grid.DisplayLayout.ValueLists.Add("StatusList");
                grid.DisplayLayout.ValueLists["StatusList"].SortStyle = ValueListSortStyle.Ascending;
            }

            // clear the value list
            grid.DisplayLayout.ValueLists["StatusList"].ValueListItems.Clear();

            // populate the value list
            foreach (KeyValuePair<string, string> kvp in GetStatusList())
            {
                grid.DisplayLayout.ValueLists["StatusList"].ValueListItems.Add(kvp.Value, kvp.Key);
            }
        }

        private static Dictionary<string, string> GetStatusList()
        {
            var statusList = new Dictionary<string, string>();
            statusList.Add("(none)", "");
            statusList.Add("Order", "O");
            statusList.Add("Quote", "Q");
            return statusList;
        }

        #region Events

        private void AccountingGrid_Load(object sender, EventArgs e)
        {
            InitializeGrid();
        }

        protected override void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.grid_InitializeLayout(sender, e);

            if (EnableSelectBox)
            {
                if (!e.Layout.Bands[0].Columns.Exists("IsSelected"))
                    e.Layout.Bands[0].Columns.Add("IsSelected");
            }
            if (EnableSetStatus)
            {
                if (!e.Layout.Bands[0].Columns.Exists("Status"))
                    e.Layout.Bands[0].Columns.Add("Status");
            }

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                GridHelper.SetDefaultCellAppearance(c);
                c.CellActivation = Activation.NoEdit;

                if (c.Key == "IsSelected")
                {
                    c.Header.Caption = "";
                    c.Header.VisiblePosition = 0;
                    c.DataType = typeof(bool);
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                }
                else if (c.Key == "Status")
                {
                    c.Header.Caption = "Status";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = e.Layout.Grid.DisplayLayout.ValueLists["StatusList"];
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
            }
        }

        private void grid_CellChange(object sender, CellEventArgs e)
        {
            // When the "IsSelected" checkbox is clicked, it needs to be forced out of edit mode
            // so that the row will immediately reflect the change
            if (e.Cell.Column.Key == "IsSelected")
            {
                e.Cell.EditorResolved.ExitEditMode(true, true);
            }
        }

        private void cmbStatusList_SelectedValueChanged(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.IsGroupByRow)
                    continue;

                var menuSetStatus = (ToolStripDropDownButton)ToolStrip.Items["menuSetStatus"];
                var cmbStatusList = (ToolStripComboBox)menuSetStatus.DropDownItems["cmbStatusList"];
                if (cmbStatusList.ComboBox != null)
                    row.Cells["Status"].Value = ((KeyValuePair<string, string>)cmbStatusList.ComboBox.SelectedItem).Value;
            }
        }

        #endregion
    }
}
