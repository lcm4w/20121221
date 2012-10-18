using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.SupplierModule
{
    public partial class AllocationOptionsForm : Form
    {
        public string OptionsAllocated { get; set; }

        private SupplierSet supplierSet;
        private SupplierSet ClonedSupplierSet { get; set; }
        private SupplierSet.AllocationRow ClonedAllocation { get; set; }

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
                {
                    // Merge to retain bindings.
                    //supplierSet.Clear();
                    //supplierSet.Merge(value);
                }
            }
        }
        private SupplierSet.AllocationRow CurrentAllocation { get; set; }    

        public AllocationOptionsForm(SupplierSet.AllocationRow allocation)
        {
            InitializeComponent();
            Icon = Properties.Resources.TourWriter16;
            CurrentAllocation = allocation;
        }

        private void DataBind()
        {
            ClonedSupplierSet = (SupplierSet)supplierSet.Copy();
            ClonedAllocation = ClonedSupplierSet.Allocation.FindByAllocationID(CurrentAllocation.AllocationID);
            gridOptionTypes.DataSource = Global.Cache.ToolSet.OptionType;
        }

        private void gridOptionTypes_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            //add custom tick box           
            if (!e.Layout.Bands[0].Columns.Exists("IsAllocated"))
                e.Layout.Bands[0].Columns.Add("IsAllocated");

            // show/hide columns 
            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "IsAllocated")
                {
                    c.Header.Caption = "";
                    c.Header.VisiblePosition = 0;
                    c.Width = 30;
                    c.MinWidth = 30;
                    c.DataType = typeof(bool);                   
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "OptionTypeName")
                {
                    c.Header.Caption = "Room";
                    c.Header.VisiblePosition = 1;
                    c.Width = 225;
                    c.MinWidth = 225;
                }
                else
                {
                    c.Hidden = true;
                }
            }
            GridHelper.SetDefaultGridAppearance(e);
        }

        private void gridOptionTypes_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "IsAllocated")
            {
                var ticked = (bool)e.Cell.EditorResolved.Value;
                if (ticked)
                {
                    var row = ClonedSupplierSet.AllocationOption.NewAllocationOptionRow();
                    row.AddedOn = DateTime.Now;
                    row.OptionTypeID = int.Parse(gridOptionTypes.ActiveRow.Cells["OptionTypeID"].Value.ToString());
                    row.AllocationID = ClonedAllocation.AllocationID;                    
                    ClonedSupplierSet.AllocationOption.AddAllocationOptionRow(row);
                }
                else
                {                 
                    ClonedSupplierSet.AllocationOption.FindByAllocationIDOptionTypeID(ClonedAllocation.AllocationID,int.Parse(e.Cell.Row.Cells["OptionTypeID"].Value.ToString())).Delete();
                }
            }
        }

        private void gridOptionTypes_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var allocationOption = ClonedSupplierSet.AllocationOption.FindByAllocationIDOptionTypeID(ClonedAllocation.AllocationID, (int.Parse(e.Row.Cells["OptionTypeID"].Value.ToString()))); //supplierSet.Allocation[0].AllocationID
            e.Row.Cells["IsAllocated"].Value = allocationOption != null;            
        }

         //var allocation = supplierSet.Allocation.FindByAllocationID((int)allocationRow.Cells["AllocationID"].Value);            
         //   var optionsAllocated = "";
         //   foreach (var r in allocation.GetAllocationOptionRows())
         //   {
         //       var option = Global.Cache.ToolSet.OptionType.FindByOptionTypeID(r.OptionTypeID);
         //       if (option == null) continue;
         //       if (!string.IsNullOrEmpty(option.OptionTypeName))
         //       {
         //           optionsAllocated += (option.OptionTypeName + ", ");
         //       }
         //   }
         //   allocationRow.Cells["Options"].Value = string.IsNullOrEmpty(optionsAllocated) ? "All" : optionsAllocated.Remove(optionsAllocated.LastIndexOf(","));

        private void btnOk_Click(object sender, EventArgs e)
        {
            foreach (var r in gridOptionTypes.Rows)
            {
                if (!((bool) r.Cells["IsAllocated"].Value)) continue;
                var option = Global.Cache.ToolSet.OptionType.FindByOptionTypeID((int) r.Cells["OptionTypeID"].Value);
                if (option == null) continue;
                if (!string.IsNullOrEmpty(option.OptionTypeName))
                {
                    OptionsAllocated += (option.OptionTypeName + ", ");
                }
            }
            OptionsAllocated = string.IsNullOrEmpty(OptionsAllocated) ? "All" : OptionsAllocated.Remove(OptionsAllocated.LastIndexOf(","));
            ClonedSupplierSet.GetChanges(DataRowState.Modified | DataRowState.Added | DataRowState.Deleted);
            supplierSet.Merge(ClonedSupplierSet, false);
        }     
    }
}
