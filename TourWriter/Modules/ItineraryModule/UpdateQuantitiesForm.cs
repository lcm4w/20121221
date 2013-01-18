using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using TourWriter.Forms;
using TourWriter.Info;
using TourWriter.Services;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Resources = TourWriter.Properties.Resources;

namespace TourWriter.Modules.ItineraryModule
{
    public partial class UpdateQuantitiesForm : Form
    {
        private string _origButtonText;
        private readonly ItinerarySet itinerarySet;
        private readonly ItinerarySet.PurchaseItemDataTable purchaseItemTable;
        private readonly bool autoStart;
        bool stopUpdateProcess;

        /// <summary>
        /// Moves booking dates and attempts to update the quantities.
        /// </summary>
        /// <param name="itinerarySet">The itinerary set to get data from.</param>
        /// <param name="autoStart">Whether to start the update process as soon as the form is shown.</param>
        public UpdateQuantitiesForm(ItinerarySet itinerarySet, bool autoStart)
        {
            InitializeComponent();
            this.itinerarySet = itinerarySet;
            this.autoStart = autoStart;
            purchaseItemTable = (ItinerarySet.PurchaseItemDataTable)itinerarySet.PurchaseItem.Copy();
            var query = from pur in purchaseItemTable
                        where pur.ServiceTypeID == 1
                        select pur;
            gridBookings.DataSource = query.ToList();
            btnStartStop.Select();
            btnStartStop.Focus();
            _origButtonText = btnStartStop.Text;
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text == _origButtonText)
            {
                // this is the start button
                UpdateOptionsOnThread();
            }
            else if (btnStartStop.Text == "Stop")
            {
                // this is the stop button
                stopUpdateProcess = true;
            }
        }

        private void UpdateOptionsOnThread()
        {
            LockControls();

            Thread thread = new Thread(UpdateQuantities) { Name = "UpdateQuantities" };
            thread.Start();
        }

        /// <summary>
        /// Locks the controls, so they cannot be used while the update thread is running.
        /// </summary>
        private void LockControls()
        {
            btnStartStop.Text = "Stop";
            btnOk.Enabled = false;
            btnCancel.Enabled = false;
        }

        /// <summary>
        /// Unlocks the controls that were locked during the update process;
        /// </summary>
        private void UnlockControls()
        {
            btnStartStop.Text = _origButtonText;
            btnOk.Enabled = true;
            btnCancel.Enabled = true;
            stopUpdateProcess = false;
        }

        private void UpdateQuantities()
        {
            foreach (UltraGridRow row in gridBookings.Rows)
            {
                if (stopUpdateProcess)
                    break;

                // reset result column
                Invoke(new MethodInvoker(
                delegate
                {
                    row.Cells["Result"].Value = null;
                    row.Cells["Result"].Appearance.Image = null;
                    row.Cells["Result"].ToolTipText = string.Empty;
                }));

                // is it selected/valid
                if (!((bool)row.Cells["IsSelected"].Value)) continue;

                int purchaseItemId = (int)row.Cells["PurchaseItemID"].Value;
                int purchaseLineId = (int)row.Cells["PurchaseLineID"].Value;
                ItinerarySet.PurchaseItemRow purchaseItem =
                    itinerarySet.PurchaseItem.FindByPurchaseItemID(purchaseItemId);
                ItinerarySet.PurchaseLineRow purchaseLine =
                    itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineId);
                ItinerarySet.OptionLookupRow opt =
                    itinerarySet.OptionLookup.FindByOptionID(purchaseItem.OptionID);
                ItinerarySet.RoomTypeRow rt =
                    (ItinerarySet.RoomTypeRow)itinerarySet.RoomType.Select("OptionTypeID = " + opt.OptionTypeID.ToString() + " AND ItineraryID = " + purchaseLine.ItineraryID.ToString()).FirstOrDefault();
                if (rt != null)
                {
                    row.Cells["Quantity"].Value = rt.GetActualQuantity();
                    row.Cells["Result"].Value = "OK";
                }
            }

            if (InvokeRequired)
                Invoke(new MethodInvoker(UnlockControls));
            else
                UnlockControls();
        }

        private void gridBookings_MouseUp(object sender, MouseEventArgs e)
        {
            UIElement clickedElement =
                gridBookings.DisplayLayout.UIElement.ElementFromPoint(gridBookings.PointToClient(MousePosition));

            if (clickedElement == null)
                return;

            // if a cell is clicked, check if it is the "Choose..." link cell
            CellUIElement cellElement = (CellUIElement)clickedElement.GetAncestor(typeof(CellUIElement));
            if (cellElement != null)
            {
                UltraGridCell cell = (UltraGridCell)cellElement.GetContext(typeof(UltraGridCell));

                if (cell == null)
                    return;

                if (cell.Column.Key == "Result" && !String.IsNullOrEmpty((string)cell.Value))
                {
                    ChooseOption(cell.Row);
                }
            }

            // if a header is clicked, check if it is the "IsSelected" column
            HeaderUIElement headerElement = (HeaderUIElement)clickedElement.GetAncestor(typeof(HeaderUIElement));
            if (headerElement != null)
            {
                UltraGridColumn column = (UltraGridColumn)headerElement.GetContext(typeof(UltraGridColumn));
                if (column.Key == "IsSelected")
                {
                    SelectAll();
                }
            }
        }

        private void ChooseOption(UltraGridRow row)
        {
            int purchaseLineId = (int)row.Cells["PurchaseLineID"].Value;
            ItinerarySet.PurchaseLineRow purchaseLine =
                itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineId);
        }

        /// <summary>
        /// Toggles the "IsSelected" column on all rows.
        /// </summary>
        private void SelectAll()
        {
            bool unselectAll = true;

            foreach (UltraGridRow row in gridBookings.Rows)
            {

                if ((bool)row.Cells["IsSelected"].Value)
                    continue;

                row.Cells["IsSelected"].Value = true;
                unselectAll = false;
            }
            if (unselectAll)
            {
                // if all of the rows are already selected, then unselect all
                foreach (UltraGridRow row in gridBookings.Rows)
                {
                    row.Cells["IsSelected"].Value = false;
                }
            }
        }

        private void gridBookings_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.ReInitialize)
                return;

            int purchaseLineId = (int)e.Row.Cells["PurchaseLineID"].Value;
            ItinerarySet.PurchaseLineRow purchaseLine =
                itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineId);

            e.Row.Cells["IsSelected"].Value = true;
            e.Row.Cells["Result"].ToolTipText = string.Empty;
            e.Row.Cells["PurchaseLineName"].Value = purchaseLine.PurchaseLineName;
            e.Row.Activation = Activation.AllowEdit;
        }

        private void gridBookings_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("IsSelected"))
                e.Layout.Bands[0].Columns.Add("IsSelected");
            if (!e.Layout.Bands[0].Columns.Exists("Result"))
                e.Layout.Bands[0].Columns.Add("Result");
            if (!e.Layout.Bands[0].Columns.Exists("PurchaseLineName"))
                e.Layout.Bands[0].Columns.Add("PurchaseLineName");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "IsSelected")
                {
                    c.Header.Caption = String.Empty;
                    c.DataType = typeof(bool);
                    c.CellClickAction = CellClickAction.Edit;
                    c.Header.Appearance.Image = Resources.CheckBox;
                    c.Header.Appearance.ImageHAlign = HAlign.Center;
                    c.Header.Appearance.ImageVAlign = VAlign.Middle;
                    c.Width = 30;
                    c.MaxWidth = 30;
                    c.SortIndicator = SortIndicator.Disabled;
                }
                else if (c.Key == "Result")
                {
                    c.Width = 80;
                    c.MaxWidth = 80;
                    c.CellActivation = Activation.ActivateOnly;
                    c.CellAppearance.ForeColor = Color.Blue;
                    c.CellAppearance.FontData.Underline = DefaultableBoolean.True;
                    c.CellAppearance.ImageHAlign = HAlign.Left;
                    c.CellAppearance.ImageVAlign = VAlign.Middle;
                }
                else if (c.Key == "PurchaseLineName")
                {
                    c.Width = 80;
                    c.Header.Caption = "Booking name";
                }
                else if (c.Key == "PurchaseItemName")
                {
                    c.Width = 120;
                    c.Header.Caption = "Item description";
                }
                else if (c.Key == "Quantity")
                {
                    c.Header.Caption = "Quantity";
                }
                else
                {
                    c.Hidden = true;
                }
            }

            // set the column order
            int index = 0;
            e.Layout.Bands[0].Columns["IsSelected"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseLineName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["PurchaseItemName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Quantity"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Result"].Header.VisiblePosition = index;

            // set defaults
            GridHelper.SetDefaultGridAppearance(e);

            // override defaults
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ValidateRows()) return;

            itinerarySet.PurchaseItem.Merge(purchaseItemTable, false);
            DialogResult = DialogResult.OK;
        }

        private bool ValidateRows()
        {
            var i = 0;
            foreach (var row in gridBookings.Rows)
            {
                i++;
                if (!(bool)row.Cells["IsSelected"].Value) continue;
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // do nothing
            return;
        }
    }
}