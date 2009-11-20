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

namespace TourWriter.Modules.ItineraryModule
{
    public partial class ItineraryPaxOverride : Form
    {
        private int _purchaseItemId;
        private ItinerarySet _baseItinerarySet;
        private ItinerarySet.ItineraryPaxDataTable _paxTable;

        public ItineraryPaxOverride(ItinerarySet.PurchaseItemRow purchaseItem)
        {
            _purchaseItemId = purchaseItem.PurchaseItemID;
            _baseItinerarySet = purchaseItem.Table.DataSet as ItinerarySet;
            _paxTable = (ItinerarySet.ItineraryPaxDataTable)_baseItinerarySet.ItineraryPax.Copy();

            InitializeComponent();

            label1.Text = purchaseItem.PurchaseLineRow.PurchaseLineName + ", " + purchaseItem.PurchaseItemName;
            DataBind();
        }

        private void DataBind()
        {
            // bind to underlying pax data
            if (!grid.DisplayLayout.ValueLists.Exists("Actions"))
            {
                grid.DisplayLayout.ValueLists.Add("Actions");
                grid.DisplayLayout.ValueLists["Actions"].ValueListItems.Add("Default", "");
                grid.DisplayLayout.ValueLists["Actions"].ValueListItems.Add("Disable", "Disable");
                grid.DisplayLayout.ValueLists["Actions"].ValueListItems.Add("Override", "Override");
            }
            grid.DataSource = _paxTable;

            // add data
            foreach (var pax in _paxTable)
            {
                UltraGridRow row = null;
                foreach (var r in grid.Rows)
                {
                    if ((int)r.Cells["ItineraryPaxID"].Value == pax.ItineraryPaxID) { row = r; break; }
                }

                // default
                SetRowDisabled(row);
                row.Cells["Actions"].Value = "Default";


                // add overrides
                var ovrride = _baseItinerarySet.ItineraryPaxOverride.Where(o => o.RowState != DataRowState.Deleted &&
                    o.PurchaseItemID == _purchaseItemId && o.ItineraryPaxID == pax.ItineraryPaxID).FirstOrDefault();
                if (ovrride != null)
                {
                    pax.MemberCount = ovrride.MemberCount;
                    pax.MemberRooms = ovrride.MemberRooms;
                    pax.StaffCount = ovrride.StaffCount;
                    pax.StaffRooms = ovrride.StaffRooms;
                    var isDisabled = !pax.IsMemberCountNull() && pax.MemberCount == 0 && !pax.IsMemberRoomsNull() && pax.MemberRooms == 0 &&
                                     !pax.IsStaffCountNull() && pax.StaffCount == 0 && !pax.IsMemberCountNull() && pax.MemberCount == 0;

                    if (isDisabled)
                    {
                        row.Cells["Actions"].Value = "Disabled";
                    }
                    else
                    {
                        row.Cells["Actions"].Value = "Override";
                        SetRowEnabled(row);
                    }
                }
            }
        }


        private void grid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("Actions"))
                e.Layout.Bands[0].Columns.Add("Actions");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                c.CellClickAction = CellClickAction.CellSelect;

                if (c.Key == "Actions")
                {
                    c.DefaultCellValue = "Default";
                    c.ValueList = grid.DisplayLayout.ValueLists["Actions"];
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                    c.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "ItineraryPaxName")
                {
                    c.Header.Caption = "Pax name";
                }
                else if (c.Key == "MemberCount")
                {
                    c.Header.Caption = "Member count";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "MemberRooms")
                {
                    c.Header.Caption = "Member rooms";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    
                }
                else if (c.Key == "StaffCount")
                {
                    c.Header.Caption = "Staff count";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "StaffRooms")
                {
                    c.Header.Caption = "Staff rooms";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
            }
            int index = 0;
            e.Layout.Bands[0].Columns["ItineraryPaxName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Actions"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["MemberCount"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["MemberRooms"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["StaffCount"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["StaffRooms"].Header.VisiblePosition = index;

            TourWriter.Services.GridHelper.SetDefaultGridAppearance(e);

            e.Layout.Override.ActiveRowAppearance.BackColor = e.Layout.Override.RowAppearance.BackColor;
            e.Layout.Override.ActiveRowAppearance.ForeColor = e.Layout.Override.RowAppearance.ForeColor;
            e.Layout.Override.SelectedRowAppearance.BackColor = e.Layout.Override.RowAppearance.BackColor;
            e.Layout.Override.SelectedRowAppearance.ForeColor = e.Layout.Override.RowAppearance.ForeColor;
        }

        private void grid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (var row in _paxTable)
            {
                var baseRow = _baseItinerarySet.ItineraryPax.Where(p => p.ItineraryPaxID == row.ItineraryPaxID).FirstOrDefault();
                var ovrRow = _baseItinerarySet.ItineraryPaxOverride.Where(p => p.RowState != DataRowState.Deleted &&
                    p.PurchaseItemID == _purchaseItemId && p.ItineraryPaxID == row.ItineraryPaxID).FirstOrDefault();

                if (baseRow.MemberCount != row.MemberCount ||
                    baseRow.MemberRooms != baseRow.MemberRooms ||
                    baseRow.StaffCount != row.StaffCount ||
                    baseRow.StaffRooms != row.StaffRooms)
                {
                    if (ovrRow == null)
                    {
                        ovrRow = _baseItinerarySet.ItineraryPaxOverride.NewItineraryPaxOverrideRow();
                        ovrRow.ItineraryPaxID = row.ItineraryPaxID;
                        ovrRow.PurchaseItemID = _purchaseItemId;
                        _baseItinerarySet.ItineraryPaxOverride.AddItineraryPaxOverrideRow(ovrRow);
                    }
                    ovrRow.MemberCount = row.MemberCount;
                    ovrRow.MemberRooms = row.MemberRooms;
                    ovrRow.StaffCount = row.StaffCount;
                    ovrRow.StaffRooms = row.StaffRooms;
                }
                else if (ovrRow != null)
                    ovrRow.Delete();
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void grid_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key != "Actions") return;

            e.Cell.Row.Update();
            switch (e.Cell.Value.ToString())
            {
                case "Default":
                    ResetPax(e.Cell.Row);
                    break;
                case "Disable":
                    DisablePax(e.Cell.Row);
                    break;
                case "Override":
                    SetRowEnabled(e.Cell.Row);
                    break;
            }
        }
        

        private void ResetPax(UltraGridRow row)
        {
            var paxId = Convert.ToInt32(row.Cells["ItineraryPaxID"].Value);
            var baseRow = _baseItinerarySet.ItineraryPax.Where(p => p.ItineraryPaxID == paxId).FirstOrDefault();

            row.Cells["MemberCount"].Value = baseRow.MemberCount;
            row.Cells["MemberRooms"].Value = baseRow.MemberRooms;
            row.Cells["StaffCount"].Value = baseRow.StaffCount;
            row.Cells["StaffRooms"].Value = baseRow.StaffRooms;

            SetRowDisabled(row);
        }

        private void DisablePax(UltraGridRow row)
        {
            row.Cells["MemberCount"].Value = 0;
            row.Cells["MemberRooms"].Value = 0;
            row.Cells["StaffCount"].Value = 0;
            row.Cells["StaffRooms"].Value = 0;

            SetRowDisabled(row);
        }

        private void SetRowEnabled(UltraGridRow row)
        {
            row.Cells["MemberCount"].Activation = Activation.AllowEdit;
            row.Cells["MemberRooms"].Activation = Activation.AllowEdit;
            row.Cells["StaffCount"].Activation = Activation.AllowEdit;
            row.Cells["StaffRooms"].Activation = Activation.AllowEdit;
        }

        private void SetRowDisabled(UltraGridRow row)
        {
            row.Cells["MemberCount"].Activation = Activation.NoEdit;
            row.Cells["MemberRooms"].Activation = Activation.NoEdit;
            row.Cells["StaffCount"].Activation = Activation.NoEdit;
            row.Cells["StaffRooms"].Activation = Activation.NoEdit;
        }
    }
}
