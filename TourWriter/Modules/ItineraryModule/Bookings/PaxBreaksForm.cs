using System;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class PaxBreaksForm : Form
    {
        private ItinerarySet _itinerarySet;

        public PaxBreaksForm(ItinerarySet itinerarySet)
        {
            _itinerarySet = itinerarySet;

            InitializeComponent();
            gridPax.DataSource = itinerarySet.ItineraryPax;
        }

        internal void AddPax()
        {
            ItinerarySet.ItineraryPaxRow pax = _itinerarySet.ItineraryPax.NewItineraryPaxRow();
            pax.ItineraryID = _itinerarySet.Itinerary[0].ItineraryID;
            pax.ItineraryPaxName = App.CreateUniqueNameValue(gridPax.Rows, "ItineraryPaxName", "New pax");
            _itinerarySet.ItineraryPax.AddItineraryPaxRow(pax);

            GridHelper.SetActiveRow(gridPax, "ItineraryPaxID", pax.ItineraryPaxID, "ItineraryPaxName");
        }

        private void gridPax_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ItineraryPaxName")
                {
                    c.Width = 80;
                    c.Header.Caption = "Pax name";
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MemberCount")
                {
                    c.Width = 30;
                    c.Header.Caption = "Member count";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "MemberRooms")
                {
                    c.Width = 30;
                    c.Header.Caption = "Member rooms";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "StaffCount")
                {
                    c.Width = 30;
                    c.Header.Caption = "Staff count";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else if (c.Key == "StaffRooms")
                {
                    c.Width = 30;
                    c.Header.Caption = "Staff rooms";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellClickAction = CellClickAction.Edit;
                    c.CellActivation = Activation.AllowEdit;
                }
                else
                    c.Hidden = true;
            }

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void btnPaxAdd_Click(object sender, EventArgs e)
        {
            AddPax();
        }

        private void btnPaxDel_Click(object sender, EventArgs e)
        {
            if (gridPax.ActiveRow != null && App.AskDeleteRow())
                GridHelper.DeleteActiveRow(gridPax, true);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
