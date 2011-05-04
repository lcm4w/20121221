using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Info;
using TourWriter.Modules.Emailer;
using TourWriter.Modules.ItineraryModule.Bookings.Email;
using TourWriter.Properties;
using TourWriter.UserControls;
using Resources=TourWriter.Properties.Resources;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class BookingEditorForm : Form
    {
        private ItinerarySet itinerarySet;
        private ItineraryMain itineraryMain;

        public BookingEditorForm(ItinerarySet itinerarySet, ItineraryMain itineraryMain)
        {
            this.itinerarySet = itinerarySet;
            this.itineraryMain = itineraryMain;

            InitializeComponent();

            Icon = Resources.TourWriter16;

            bookingsBindingSource.DataSource = itinerarySet;
            bookingsBindingSource.DataMember = "PurchaseLine";
            bookingsNavigator.BindingSource = bookingsBindingSource;
            
            bookingEditor11.BindingSource = bookingsBindingSource;
            bookingEditor11.OnOpenSupplier += bookingEditor11_OnOpenSupplier;
            bookingEditor11.ItineraryMain = this.itineraryMain;

            cmbBookings.ComboBox.DataSource = bookingsBindingSource;
            cmbBookings.ComboBox.ValueMember = "PurchaseLineID";
            cmbBookings.ComboBox.DisplayMember = "PurchaseLineName";
        }

        private void BookingEditorForm_Load(object sender, EventArgs e)
        {
            Size = Settings.Default.BookingEditorSize;
            Location = Settings.Default.BookingEditorLocation;
            if (!App.IsOnScreen(this)) Location = new System.Drawing.Point(10, 10);
            Text = Text.Trim() + ": " + itinerarySet.Itinerary[0].ItineraryName;
            
            bookingEditor11.Select(); // ensure 'cancel' button not selected at load.
        }

        public void SetActiveRows(int? purchaseLineId, int? purchaseItemId)
        {
            if (purchaseLineId.HasValue)
            {
                int i = 0;
                foreach (DataRowView row in bookingsBindingSource.List)
                {
                    if ((row.Row as ItinerarySet.PurchaseLineRow).PurchaseLineID == purchaseLineId)
                    {
                        bookingsBindingSource.Position = i;
                        break;
                    }
                    i++;
                }
            }

            bookingEditor11.SetActiveRow(purchaseItemId);
        }

        private void OpenSupplier(int supplierId, int? serviceId, int? optionId)
        {
            BookingSelectorForm bookingSelectorForm = new BookingSelectorForm(itinerarySet, itineraryMain);
            bookingSelectorForm.LoadSupplier(supplierId, serviceId, optionId);
            bookingSelectorForm.SetPage(BookingSelectorForm.PageType.Select);
            bookingSelectorForm.Size = Size;
            bookingSelectorForm.ShowDialog();
            bookingSelectorForm.Dispose();
        }

        private void SendBookingRequest(List<int> idList)
        {
            BookingEmailForm bookingEmailer = new BookingEmailForm(itinerarySet, idList);
            bookingEmailer.ShowDialog();
        }

        internal void SetBindingContext(BindingContext bindingContext)
        {
            BindingContext = bindingContext;
            bookingEditor11.SetBindingContext(bindingContext);
        }
        
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAddLine_Click(object sender, EventArgs e)
        {
            BookingSelectorForm bookingSelector = new BookingSelectorForm(itinerarySet, itineraryMain);
            bookingSelector.Size = Size;
            bookingSelector.OnPurchaseItemAdded +=
                new OnPurchaseItemAddedHandler(bookingSelector_OnPurchaseItemAdded);

            bookingSelector.ShowDialog();
            bookingSelector.Dispose();
        }

        private void btnDeleteLine_Click(object sender, EventArgs e)
        {
            if(bookingsBindingSource.Count == 0)
                return;

            if (App.AskYesNo("Are you sure you want to delete this row and its related data?"))
                bookingsBindingSource.RemoveCurrent();
        }

        private void btnSendBookings_ButtonClick(object sender, EventArgs e)
        {
            btnSendBookings.ShowDropDown();
        }

        private void btnBookAll_Click(object sender, EventArgs e)
        {
            btnBookThis_Click(sender, e);
        }

        private void btnBookThis_Click(object sender, EventArgs e)
        {
            DataRowView dv = bookingsBindingSource.Current as DataRowView;
            ItinerarySet.PurchaseLineRow row = dv.Row as ItinerarySet.PurchaseLineRow;

            List<int> idList = new List<int>();
            idList.Add(row.PurchaseLineID);

            SendBookingRequest(idList);
        }

        private void bookingEditor11_OnOpenSupplier(BookingEditorOpenSupplierEventArgs e)
        {
            OpenSupplier(e.supplierId, null, e.optionId);
        }

        private void BookingsManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.BookingEditorSize =
                WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;
            Settings.Default.BookingEditorLocation =
                WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;
            Settings.Default.Save();
            App.ClearBindings(this);
        }
                
        private void bookingSelector_OnPurchaseItemAdded(PurchaseItemAddedEventArgs e)
        {
            SetActiveRows(e.PurchaseLineId, e.PurchaseItemId);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            ItinerarySet.PurchaseLineRow copyRow = 
                (bookingsBindingSource.Current as DataRowView).Row as ItinerarySet.PurchaseLineRow;

            if (App.AskYesNo("Copy booking: " + copyRow.PurchaseLineName + "?"))
            {
                ItinerarySet.PurchaseLineRow newRow =
                    itinerarySet.CopyPurchaseLine(
                        copyRow, "Copy of", TourWriter.Global.Cache.User.UserID, true);

                SetActiveRows(newRow.PurchaseLineID, null);
            }
        }
    }
}