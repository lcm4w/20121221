using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;
using TourWriter.Utilities;
using ColumnStyle=Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class BookingFlagEditor : Form
    {
        private readonly ItinerarySet itinerarySetOrig;
        private readonly ItinerarySet itinerarySet;
        private readonly int purchaseItemId;

        public BookingFlagEditor(ItinerarySet itinerarySet, int purchaseItemId)
        {
            InitializeComponent();
            Icon = Properties.Resources.TourWriter16;

            itinerarySetOrig = itinerarySet;
            this.itinerarySet = (ItinerarySet)itinerarySet.Copy();
            this.purchaseItemId = purchaseItemId;
            DataBind();
        }

        private void DataBind()
        {
            // add the flag value list
            if (!gridNotes.DisplayLayout.ValueLists.Exists("FlagList"))
            {
                gridNotes.DisplayLayout.ValueLists.Add("FlagList");
                gridNotes.DisplayLayout.ValueLists["FlagList"].DisplayStyle = ValueListDisplayStyle.Picture;

                foreach (var r in Cache.ToolSet.Flag)
                {
                    var valueListItem = new ValueListItem(r.FlagID, String.Empty);
                    valueListItem.Appearance.Image = ImageHelper.ByteArrayToImage(r.FlagImage);
                    gridNotes.DisplayLayout.ValueLists["FlagList"].ValueListItems.Add(valueListItem);
                }
            }

            // bind the grid to a dataview showing only the notes for this purchase item
            var dataView = new DataView(itinerarySet.PurchaseItemNote);
            dataView.RowFilter = "PurchaseItemID = " + purchaseItemId;
            gridNotes.DataSource = dataView;
        }

        private void gridNotes_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "FlagID")
                {
                    c.Header.Caption = "Flag";
                    c.Style = ColumnStyle.DropDownList;
                    c.ValueList = e.Layout.ValueLists["FlagList"];
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.MaxWidth = c.MinWidth = 50;
                }
                else if (c.Key == "Note")
                {
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    c.Width = 100;
                }
                else
                {
                    c.Hidden = true;
                }
            }

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var note = itinerarySet.PurchaseItemNote.NewPurchaseItemNoteRow();
            note.PurchaseItemID = purchaseItemId;
            note.FlagID = Cache.ToolSet.Flag[0].FlagID;
            note.Note = App.CreateUniqueNameValue(gridNotes.Rows, "Note", "New note");
            itinerarySet.PurchaseItemNote.AddPurchaseItemNoteRow(note);

            GridHelper.SetActiveRow(gridNotes, "PurchaseItemNoteID", note.PurchaseItemNoteID, "Note");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            GridHelper.DeleteActiveRow(gridNotes, true);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // check for rows that have been deleted, but do not exist in the database
            for (int i = itinerarySetOrig.PurchaseItemNote.Count - 1; i >= 0; i--)
            {
                var row = itinerarySetOrig.PurchaseItemNote[i];
                if (row.RowState != DataRowState.Deleted && row.PurchaseItemNoteID < 0)
                {
                    if (itinerarySet.PurchaseItemNote.FindByPurchaseItemNoteID(row.PurchaseItemNoteID) == null)
                    {
                        itinerarySetOrig.PurchaseItemNote.RemovePurchaseItemNoteRow(row);
                    }
                }
            }

            var changes = itinerarySet.PurchaseItemNote.GetChanges();
            if (changes != null)
            {
                itinerarySetOrig.PurchaseItemNote.Merge(changes);
            }
        }
    }
}
