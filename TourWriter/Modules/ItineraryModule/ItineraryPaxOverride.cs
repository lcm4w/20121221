using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TourWriter.Info;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;

namespace TourWriter.Modules.ItineraryModule
{
    public partial class ItineraryPaxOverride : Form
    {
        private int _purchaseItemId;
        private ItinerarySet _baseItinerarySet;
        private ItinerarySet.ItineraryPaxOverrideDataTable _paxOverrideTable;

        public ItineraryPaxOverride(ItinerarySet.PurchaseItemRow purchaseItem)
        {
            _purchaseItemId = purchaseItem.PurchaseItemID;
            _baseItinerarySet = purchaseItem.Table.DataSet as ItinerarySet;
            _paxOverrideTable = (ItinerarySet.ItineraryPaxOverrideDataTable)_baseItinerarySet.ItineraryPaxOverride.Copy();

            InitializeComponent();
            DataBind();
        }

        private void DataBind()
        {
            RebuildPaxList();
            _paxOverrideTable.DefaultView.RowFilter = "PurchaseItemID = " + _purchaseItemId;
            gridPax.DataSource = _paxOverrideTable.DefaultView;
            gridPax.AfterCellUpdate += gridPax_AfterCellUpdate;
        }

        private void gridPax_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ItineraryPaxName")
                {
                    c.Header.Caption = "Pax name";
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                    c.ValueList = gridPax.DisplayLayout.ValueLists["PaxList"];
                    c.ButtonDisplayStyle = ButtonDisplayStyle.OnRowActivate;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "MemberCount")
                {
                    c.Header.Caption = "Member count";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MemberRooms")
                {
                    c.Header.Caption = "Member rooms";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "StaffCount")
                {
                    c.Header.Caption = "Staff count";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "StaffRooms")
                {
                    c.Header.Caption = "Staff rooms";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else
                    c.Hidden = true;
            }
            TourWriter.Services.GridHelper.SetDefaultGridAppearance(e);
        }

        private void gridPax_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "ItineraryPaxID")
            {
                RebuildPaxList();
            }
        }

        private void RebuildPaxList()
        {
            if (!gridPax.DisplayLayout.ValueLists.Exists("PaxList"))
                gridPax.DisplayLayout.ValueLists.Add("PaxList");
            else
                gridPax.DisplayLayout.ValueLists["PaxList"].ValueListItems.Clear();

            var currentRows = _paxOverrideTable.Where(p => p.PurchaseItemID == _purchaseItemId).Select(p => p.ItineraryPaxID);
            var availableRows = _baseItinerarySet.ItineraryPax.Where(p => p.RowState != DataRowState.Deleted && !currentRows.Contains(p.ItineraryPaxID));

            foreach (var row in availableRows)      
                gridPax.DisplayLayout.ValueLists["PaxList"].ValueListItems.Add(row.ItineraryPaxID, row.ItineraryPaxName);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var row = _paxOverrideTable.NewItineraryPaxOverrideRow();
             row.PurchaseItemID = _purchaseItemId;

             if (gridPax.DisplayLayout.ValueLists["PaxList"].ValueListItems.Count > 0)
             {
                 row.ItineraryPaxID = Convert.ToInt32(gridPax.DisplayLayout.ValueLists["PaxList"].ValueListItems[0].DataValue);
                 _paxOverrideTable.AddItineraryPaxOverrideRow(row);
                 RebuildPaxList();
             }
             else
             {
                 App.ShowInfo("No pax ranges to override. Pax ranges can be added in the Clients tab.");
             }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (gridPax.ActiveRow == null) return;
            gridPax.ActiveRow.Delete();
            RebuildPaxList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _baseItinerarySet.ItineraryPaxOverride.Merge(_paxOverrideTable, false);
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
