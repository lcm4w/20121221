using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;
using TourWriter.Services;

namespace TourWriter.Modules.SupplierModule
{
    public partial class AllocationAgentsForm : Form
    {
        private SupplierSet supplierSet;
        public SupplierSet ClonedSupplierSet;
        private int AllocationID { get; set; }       
        private SupplierSet.AllocationAgentRow[] AllocationAgentRows { get; set; }
        public string AgentsAllocated { get; set; }
        internal SupplierSet SupplierSet
        {
            get { return supplierSet; }
            set
            {
                if (supplierSet == null)
                {
                    supplierSet = value;
                    DataBind();
                }
            }
        }

        private SupplierSet.AllocationRow CurrentAllocation { get; set; }
        private SupplierSet.AllocationRow ClonedAllocation { get; set; }

        private void DataBind()
        {
            //clone
            ClonedSupplierSet = (SupplierSet)supplierSet.Copy();


            gridAllocationAgent.DisplayLayout.ValueLists.Add("AgentList");
            UpdateAgentDDL();           
            gridAllocationAgent.DataSource = ClonedSupplierSet.Allocation.FindByAllocationID(CurrentAllocation.AllocationID).GetAllocationAgentRows();//CurrentAllocation.GetAllocationAgentRows();            
            gridAllocationAgent.CellDataError += gridAllocationAgent_CellDataError;
        }

        private void gridAllocationAgent_CellDataError(object sender, CellDataErrorEventArgs e)
        {
            e.RaiseErrorEvent = false;
            App.ShowError("The agent you selected is allocated already, please select a different agent");
        }

        private void UpdateAgentDDL()
        {
            gridAllocationAgent.DisplayLayout.ValueLists["AgentList"].ValueListItems.Clear();
            gridAllocationAgent.DisplayLayout.ValueLists["AgentList"].SortStyle = ValueListSortStyle.Ascending;
            foreach (ToolSet.AgentRow r in Cache.ToolSet.Agent.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                    continue;
                gridAllocationAgent.DisplayLayout.ValueLists["AgentList"].ValueListItems.Add(r.AgentID, r.AgentName);
            }

        }

        public AllocationAgentsForm(SupplierSet.AllocationRow allocation)
        {
            InitializeComponent();
            Icon = Properties.Resources.TourWriter16;
            //AllocationID = allocationID;
            CurrentAllocation = allocation;
            AllocationAgentRows = AllocationAgentRows;
        }

        private void gridAllocationAgent_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "AgentID")
                {
                    c.Header.Caption = "Agent";
                    c.Width = 192;
                    c.MinWidth = 192;
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = gridAllocationAgent.DisplayLayout.ValueLists["AgentList"];
                }
                else if (c.Key == "Quantity")
                {
                    c.Header.Caption = "Quantity";
                    c.Width = 80;
                    c.MinWidth = 80;
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                {
                    c.Hidden = true;
                }
            }
            GridHelper.SetDefaultGridAppearance(e);
        }

        private void btnAddAllocation_Click(object sender, EventArgs e)
        {
            //supplierSet.Supplier[0].SupplierName = supplierSet.Supplier[0].SupplierName;

            var availableAgentID = GetNextAvailableAgent();
            if (availableAgentID == 0)
            {
                App.ShowError("All agents are allocated");
                return;
            }
            var row = ClonedSupplierSet.AllocationAgent.NewAllocationAgentRow();//supplierSet.AllocationAgent.NewAllocationAgentRow();
            row.AllocationID = CurrentAllocation.AllocationID;
            row.AgentID = availableAgentID;
            row.Quantity = 0;
            ClonedSupplierSet.AllocationAgent.AddAllocationAgentRow(row);//supplierSet.AllocationAgent.AddAllocationAgentRow(row);
            gridAllocationAgent.DataSource = ClonedSupplierSet.Allocation.FindByAllocationID(CurrentAllocation.AllocationID).GetAllocationAgentRows();//gridAllocationAgent.DataSource = //CurrentAllocation.GetAllocationAgentRows();
            GridHelper.SetActiveRow(gridAllocationAgent, "AgentID", row.AgentID, "AgentID");
        }

        private void btnDeleteAllocation_Click(object sender, EventArgs e)
        {
            if (gridAllocationAgent.ActiveRow == null) return;

            if (App.AskDeleteRow())
            {
                //GridHelper.DeleteActiveRow(gridAllocationAgent, true);
                // supplierSet.AllocationAgent.FindByAllocationIDAgentID(CurrentAllocation.AllocationID, (int)gridAllocationAgent.ActiveRow.Cells["AgentID"].Value).Delete();
                ClonedSupplierSet.AllocationAgent.FindByAllocationIDAgentID(CurrentAllocation.AllocationID, (int)gridAllocationAgent.ActiveRow.Cells["AgentID"].Value).Delete();
                gridAllocationAgent.DataSource = ClonedSupplierSet.Allocation.FindByAllocationID(CurrentAllocation.AllocationID).GetAllocationAgentRows();//CurrentAllocation.GetAllocationAgentRows();
            }
        }

        private void gridAllocationAgent_KeyUp(object sender, KeyEventArgs e)
        {
            var grid = (UltraGrid)sender;
            if (grid.ActiveCell.Column.Key != "Quantity") return;
            if (e.KeyCode == Keys.Enter)
            {
                grid.PerformAction(UltraGridAction.ExitEditMode);
            }
        }

        private int GetNextAvailableAgent()
        {
            return (from ToolSet.AgentRow r in Cache.ToolSet.Agent.Rows
                    where r.RowState != DataRowState.Deleted
                    let agentUnallocated = gridAllocationAgent.Rows.Count(x => x.Cells["AgentID"].Value.ToString() == r.AgentID.ToString())
                    where agentUnallocated == 0
                    select r.AgentID).FirstOrDefault();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            foreach (var r in gridAllocationAgent.Rows)
            {
                var agent = Cache.ToolSet.Agent.FindByAgentID((int)r.Cells["AgentID"].Value);
                if (agent == null) continue;
                if (!string.IsNullOrEmpty(agent.AgentName))
                {
                    AgentsAllocated += (agent.AgentName.Substring(0, agent.AgentName.Length > 14 ? 15 : agent.AgentName.Length - 1) + "[" + ((int)r.Cells["Quantity"].Value) + "]" + ", ");
                }                
            }
            AgentsAllocated = string.IsNullOrEmpty(AgentsAllocated) ? "All" : AgentsAllocated.Remove(AgentsAllocated.LastIndexOf(","));
            ClonedSupplierSet.GetChanges(DataRowState.Modified | DataRowState.Added | DataRowState.Deleted);
            supplierSet.Merge(ClonedSupplierSet);         
        }
       
        private void btnCancel_Click(object sender, EventArgs e)
        {
            
        }
    }
}

