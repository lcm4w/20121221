using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TourWriter.Info;
using TourWriter.Services;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Resources = TourWriter.Properties.Resources;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    public partial class InvoiceSelectorForm : Form
    {
        private readonly ItinerarySet _itinerarySet;

        /// <summary>
        /// Displays invoices for a purchase line.
        /// </summary>
        /// <param name="itinerarySet">The itinerary set to get data from.</param>
        /// <param name="purchaseLineId">The Id of the selected purchase line.</param>
        public InvoiceSelectorForm(ItinerarySet itinerarySet, int purchaseLineId)
        {
            InitializeComponent();
            this._itinerarySet = itinerarySet;

            var purchaseLine = _itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineId);
            gridInvoices.DataSource = purchaseLine.GetInvoiceRows();
        }

        private void gridInvoices_MouseUp(object sender, MouseEventArgs e)
        {
            UIElement clickedElement =
                gridInvoices.DisplayLayout.UIElement.ElementFromPoint(gridInvoices.PointToClient(MousePosition));

            if (clickedElement == null)
                return;

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

        /// <summary>
        /// Toggles the "IsSelected" column on all rows.
        /// </summary>
        private void SelectAll()
        {
            bool unselectAll = true;

            foreach (UltraGridRow row in gridInvoices.Rows)
            {

                if ((bool)row.Cells["IsSelected"].Value)
                    continue;

                row.Cells["IsSelected"].Value = true;
                unselectAll = false;
            }
            if (unselectAll)
            {
                // if all of the rows are already selected, then unselect all
                foreach (UltraGridRow row in gridInvoices.Rows)
                {
                    row.Cells["IsSelected"].Value = false;
                }
            }
        }

        private void gridInvoices_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.ReInitialize)
                return;

            int purchaseLineId = (int)e.Row.Cells["PurchaseLineID"].Value;
            ItinerarySet.PurchaseLineRow purchaseLine = _itinerarySet.PurchaseLine.FindByPurchaseLineID(purchaseLineId);

            e.Row.Cells["IsSelected"].Value = true;
        }

        private void gridInvoices_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("IsSelected"))
                e.Layout.Bands[0].Columns.Add("IsSelected");

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
                else if (c.Key == "Filename")
                {
                    c.Width = 150;
                    c.Header.Caption = "Filename";
                }
                else if (c.Key == "Amount")
                {
                    c.Width = 120;
                    c.Header.Caption = "Amount";
                    c.Format = "#0.00";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                }
                else if (c.Key == "CreatedOn")
                {
                    c.Width = 100;
                    c.Header.Caption = "Date";
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Date;
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                }
                else
                {
                    c.Hidden = true;
                }
            }

            // set the column order
            int index = 0;
            e.Layout.Bands[0].Columns["IsSelected"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Filename"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Amount"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["CreatedOn"].Header.VisiblePosition = index;

            // set defaults
            GridHelper.SetDefaultGridAppearance(e);

            // override defaults
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (!ValidateRows())
            {
                App.ShowError("Please select at least one invoice.");
                return;
            }
            else
            {
                const string invoiceWebLocation = @"http://tw.travelmesh.com/{0}/invoices/{1}";
                const string fileDescription = "Word and Excel";
                const string fileExtension = ".doc;*.docx;*.xls;*.xlsx;";
                string fileName;
                string invoiceFullPath;
                string saveFileName;

                foreach (UltraGridRow row in gridInvoices.Rows)
                {
                    if (!(bool)row.Cells["IsSelected"].Value)
                        continue;
                    fileName = row.Cells["Filename"].Value.ToString();
                    invoiceFullPath = string.Format(invoiceWebLocation, fileName.Substring(3, 8), fileName);
                    saveFileName = App.PromptSaveFile(System.IO.Path.GetFileName(invoiceFullPath), fileDescription, fileExtension, true);
                    if (saveFileName.Length > 0)
                        new System.Net.WebClient().DownloadFile(invoiceFullPath, saveFileName);
                }
            }
        }

        private bool ValidateRows()
        {
            foreach (var row in gridInvoices.Rows)
            {
                if ((bool)row.Cells["IsSelected"].Value)
                    return true;
            }
            return false;
        }
    }
}