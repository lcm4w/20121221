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

namespace TourWriter.Modules.ItineraryModule
{
    public partial class AllocationAgentsForm : Form
    {
        public int TotalAllocations { get; set; }
        private ItinerarySet itinerarySet;
        private ItinerarySet ClonedItinerarySet { get; set; }

        internal ItinerarySet ItinerarySet   
        {
            get { return itinerarySet; }
            set
            {
                if (itinerarySet == null)
                {
                    itinerarySet = value;
                    DataBind();
                }
            }
        }

        private ItinerarySet.AllocationRow ClonedAllocation { get; set; }
        //private ItinerarySet.AllocationDataTable AllocationTable { get; set; }

        private void DataBind()
        {           
            //clone
            ClonedItinerarySet = (ItinerarySet)itinerarySet.Copy();          
            if (!ClonedItinerarySet.Itinerary[0].GetAllocationRows().Any())
            {
                var allocation = ClonedItinerarySet.Allocation.NewAllocationRow();
                allocation.ItineraryID = ClonedItinerarySet.Itinerary[0].ItineraryID;
                allocation.ValidFrom = DateTime.Now;
                allocation.ValidTo = DateTime.Now;
                allocation.Quantity = 0;
                ClonedItinerarySet.Allocation.AddAllocationRow(allocation);
                ClonedAllocation = allocation;                
               
            }
            else
            {
                ClonedAllocation = ClonedItinerarySet.Allocation.SingleOrDefault(x => x.ItineraryID == itinerarySet.Itinerary[0].ItineraryID);
            }
            gridAllocationAgent.DisplayLayout.ValueLists.Add("AgentList");
            UpdateAgentDDL();
            gridAllocationAgent.DataSource = ClonedItinerarySet.Allocation[0].GetAllocationAgentRows();//ClonedAllocation.GetAllocationAgentRows();
            gridAllocationAgent.CellDataError += gridAllocationAgent_CellDataError;                     
            txtAllocationTotal.Value = ClonedAllocation.Quantity;          
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

        public AllocationAgentsForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.TourWriter16;
            //CurrentAllocation = allocation;
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
            var row = ClonedItinerarySet.AllocationAgent.NewAllocationAgentRow();
            row.AllocationID = ClonedAllocation.AllocationID;
            row.AgentID = availableAgentID;
            row.Quantity = 0;
            ClonedItinerarySet.AllocationAgent.AddAllocationAgentRow(row);
            gridAllocationAgent.DataSource = ClonedItinerarySet.Allocation[0].GetAllocationAgentRows();//ClonedAllocation.GetAllocationAgentRows();
            GridHelper.SetActiveRow(gridAllocationAgent, "AgentID", row.AgentID, "AgentID");
        }

        private void btnDeleteAllocation_Click(object sender, EventArgs e)
        {
            if (gridAllocationAgent.ActiveRow == null) return;

            if (App.AskDeleteRow())
            {         
                //GridHelper.DeleteActiveRow(gridAllocationAgent, true);       
                var agent = ClonedItinerarySet.AllocationAgent.FindByAllocationIDAgentID(ClonedAllocation.AllocationID, int.Parse(gridAllocationAgent.ActiveRow.Cells["AgentID"].Value.ToString()));//Delete();
                if (agent != null)
                {
                    agent.Delete();
                    gridAllocationAgent.DataSource = ClonedItinerarySet.Allocation[0].GetAllocationAgentRows();//ClonedAllocation.GetAllocationAgentRows();
                }
            }
        }

        private void gridAllocationAgent_KeyUp(object sender, KeyEventArgs e)
        {
            var grid = (UltraGrid)sender;
            if (grid.ActiveCell == null) return;
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

        private void BtnOkClick(object sender, EventArgs e)
        {            
            var id = -1;
            foreach (var itin in ClonedItinerarySet.Itinerary.Rows.Cast<ItinerarySet.ItineraryRow>())
            {
                //itinerary allocations
                foreach (var alloc in itin.GetAllocationRows())
                {                    
                    if (alloc.RowState == DataRowState.Added)
                        SetInserted(alloc, "AllocationID", id--);

                    //alloc agents
                    foreach (var agent in alloc.GetAllocationAgentRows())
                    {
                        if (agent.RowState == DataRowState.Added)
                            SetInserted(agent);
                        if (agent.RowState == DataRowState.Deleted)
                            SetDeleted(agent);
                    }
                }
            }            

            TotalAllocations = (int)txtAllocationTotal.Value;
            ClonedItinerarySet.AllocationAgent.GetChanges(DataRowState.Modified | DataRowState.Added | DataRowState.Deleted);
            ClonedItinerarySet.GetChanges(DataRowState.Modified | DataRowState.Added | DataRowState.Deleted);  

            itinerarySet.AllocationAgent.Merge(ClonedItinerarySet.AllocationAgent);
            itinerarySet.Merge(ClonedItinerarySet, true);                     
        }

        private void gridAllocationAgent_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "Quantity")
            {
                if (GetTotal() > (int)txtAllocationTotal.Value)
                {
                    App.ShowWarning(string.Format("Allocations are not allowed to exceed {0}", (int)txtAllocationTotal.Value));
                    e.Cell.Value = e.Cell.OriginalValue;
                }
            }
        }

        private int GetTotal()
        {
            var total = 0;
            foreach(var r in gridAllocationAgent.Rows)
            {
                total += (int) r.Cells["Quantity"].Value;
            }
            return total;
        }

        private void gridAllocationAgent_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
          
        }

        private static void SetInserted(DataRow row)
        {
            row.AcceptChanges();
            row.SetAdded();
        }

        private static void SetDeleted(DataRow row)
        {
            row.AcceptChanges();
            row.Delete();
        }

        private static void SetInserted(DataRow row, string key, int value)
        {
            row.Table.Columns[key].ReadOnly = false;
            row[key] = value;
            row.AcceptChanges();
            row.SetAdded();
        }
    }
}
