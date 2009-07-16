using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.Search
{
    /// <summary>
    /// Search for items in the MainForm menus.
    /// </summary>
    public partial class SearchMain : ModuleBase
    {
        #region Member vars

        private enum SearchType
        {
            StartsWith,
            Contains,
            ByID
        }

        private DataSet ds;

        public NavigationTreeItemInfo.ItemTypes MenuType
        {
            get { return (NavigationTreeItemInfo.ItemTypes) cmbSearch.SelectedItem.DataValue; }
            set
            {
                int index = -1;
                for (int i = 0; i < cmbSearch.Items.Count; i++)
                {
                    if ((NavigationTreeItemInfo.ItemTypes) cmbSearch.Items[i].DataValue == value)
                    {
                        index = i;
                        break;
                    }
                }
                cmbSearch.SelectedIndex = index;
            }
        }

        #endregion

        public SearchMain()
        {
            Icon = Icon.FromHandle(TourWriter.Properties.Resources.Magnifier.GetHicon()); 
            
            InitializeComponent();
            displayTypeName = "Search";
            LoadComboItems();

            // Merged with parent.
            menuStrip1.Visible = false;
            toolStrip1.Visible = false;
        }

        private void SearchMain_Load(object sender, EventArgs e)
        {
        }

        #region Override methods

        protected override bool IsDataDirty()
        {
            return false;
        }

        protected override string GetDisplayName()
        {
            return string.Format("Search {0} '{1}'", cmbSearch.Text, txtSearch.Text); 
        }

        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                PerformSearch();

                if (ds != null)
                    DisplayResults();

                SetFormActiveText();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void cmbSearch_ValueChanged(object sender, EventArgs e)
        {
            grid.DataSource = null;
            SetSearchTypes();
        }

        private void SetSearchTypes()
        {
            if (NavigationTreeItemInfo.ItemTypes.PurchaseLine ==
                (NavigationTreeItemInfo.ItemTypes)cmbSearch.SelectedItem.DataValue)
            {
                cmbType.Items.Clear();
                cmbType.Items.Add(SearchType.ByID, "ID equals");
            }
            else
            {
                cmbType.Items.Clear();
                cmbType.Items.Add(SearchType.Contains, "Name contains");
                cmbType.Items.Add(SearchType.StartsWith, "Name starts with");
                cmbType.Items.Add(SearchType.ByID, "ID equals");
            }

            cmbType.SelectedIndex = 0;
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Hidden = true;

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "Name")
                {
                    c.Width = 100;
                    c.Header.Caption = "Name";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.Band.SortedColumns.Add(c, false);
                }
                    // only for booking search
                else if (c.Key == "BookingName")
                {
                    c.Width = 100;
                    c.Header.Caption = "Booking name";
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.Band.SortedColumns.Add(c, false);
                }
                else if (c.Key == "AddedOn")
                {
                    c.Width = 30;
                    c.Header.Caption = "Added on";
                }
                else if (c.Key == "AddedBy")
                {
                    c.Width = 30;
                    c.Header.Caption = "Added by";
                }
                else
                    c.Hidden = true;
            }

            GridHelper.SetDefaultGridAppearance(e);
        }

        private void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow))
                return;

            NavigationTreeItemInfo info = new NavigationTreeItemInfo(
                (int)e.Row.Cells["ID"].Value,
                (string)e.Row.Cells["Name"].Value,
                MenuType,
                (int)e.Row.Cells["ParentFolderID"].Value,
                (bool)e.Row.Cells["IsActive"].Value);

            UltraTreeNode node = App.MainForm.BuildMenuNode(info);

            if (MenuType == NavigationTreeItemInfo.ItemTypes.Itinerary)
                App.MainForm.Load_ItineraryForm(node);
            else if (MenuType == NavigationTreeItemInfo.ItemTypes.Supplier)
                App.MainForm.Load_SupplierForm(node);
            else if (MenuType == NavigationTreeItemInfo.ItemTypes.Contact)
                App.MainForm.Load_ContactForm(node);
            else if (MenuType == NavigationTreeItemInfo.ItemTypes.PurchaseLine)
                App.MainForm.Load_ItineraryForm(node);
        }


        private void LoadComboItems()
        {
            cmbSearch.Items.Add(NavigationTreeItemInfo.ItemTypes.Itinerary, "Itineraries");
            cmbSearch.Items.Add(NavigationTreeItemInfo.ItemTypes.Supplier, "Suppliers");
            cmbSearch.Items.Add(NavigationTreeItemInfo.ItemTypes.Contact, "Contacts");
            cmbSearch.Items.Add(NavigationTreeItemInfo.ItemTypes.PurchaseLine, "Bookings");
            cmbSearch.SelectedIndex = 0;

            cmbType.SelectedIndex = 0;
        }

        private void PerformSearch()
        {
            NavigationTreeItemInfo m = new NavigationTreeItemInfo();

            string phrase = txtSearch.Text;
            NavigationTreeItemInfo.ItemTypes itemType = (NavigationTreeItemInfo.ItemTypes) cmbSearch.SelectedItem.DataValue;
            SearchType searchType = (SearchType)cmbType.SelectedItem.DataValue;

            try
            {
                Cursor = Cursors.WaitCursor;
                
                Thread.Sleep(500);  // show cursor...

                // validate
                phrase = phrase.Replace("\'", "\'\'"); // escape sql quotes
                phrase = phrase.ToLower().Replace(" delete ", "");
                phrase = phrase.ToLower().Replace(" drop ", "");
                
                if (searchType == SearchType.ByID)
                {
                    int id;
                    bool isInteger = int.TryParse(phrase, out id);
                    ds = m.SearchByID((isInteger) ? id : -1, itemType);
                }
                else
                {
                    if (searchType == SearchType.Contains)
                    {
                        phrase = "%" + phrase;
                    }
                    ds = m.Search(phrase, itemType);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void DisplayResults()
        {
            grid.DataSource = ds;
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            App.ShowHelp("SearchMain");
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}