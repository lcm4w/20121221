using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Services;

namespace TourWriter.Modules.SupplierModule
{
    public partial class ContentControl : UserControl
    {
        private bool _isDataLoaded;
        internal SupplierSet SupplierSet { get; set; }

        public ContentControl()
        {
            InitializeComponent();
        }

        private void ContentControl_Load(object sender, EventArgs e)
        {
        }

        private void DataBind()
        {
            BuildContentTypesList();
            srcContents.DataSource = SupplierSet.Content;
            srcContentTypes.DataSource = Cache.ToolSet;
        }

        private void BuildContentTypesList()
        {
            if (!gridContents.DisplayLayout.ValueLists.Contains("ContentTypeList"))
            {
                gridContents.DisplayLayout.ValueLists.Add("ContentTypeList");
                gridContents.DisplayLayout.ValueLists["ContentTypeList"].SortStyle = ValueListSortStyle.Ascending;
            }
            else
            {
                gridContents.DisplayLayout.ValueLists["ContentTypeList"].ValueListItems.Clear();
            }
            foreach (var r in Enumerable.Where(Cache.ToolSet.ContentType, r => r.RowState != DataRowState.Deleted))
            {
                gridContents.DisplayLayout.ValueLists["ContentTypeList"].ValueListItems.Add(
                    r.ContentTypeId, r.ContentTypeName);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var row = SupplierSet.Content.NewContentRow();
            row.ContentName = App.CreateUniqueNameValue(gridContents.Rows, "ContentName", "New Content");
            row.SupplierId = SupplierSet.Supplier[0].SupplierID;
            SupplierSet.Content.AddContentRow(row);

            gridContents.ActiveRow = gridContents.Rows[gridContents.Rows.Count - 1];
            txtNames.Select();
                
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            var row = gridContents.ActiveRow;
            if (row == null || !App.AskDeleteRow()) return;
            var i = row.Index;
            row.Delete(false);
            if ((i > gridContents.Rows.Count - 1 ? --i : i) > -1)
                gridContents.ActiveRow = gridContents.Rows[i];
        }
        
        private void gridContents_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            // show/hide columns 
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ContentName")
                {
                    c.Band.SortedColumns.Add(c, false);
                    c.Header.Caption = "Content";
                    c.CellClickAction = CellClickAction.Edit;
                }
                else if (c.Key == "ServiceTypeID")
                {
                    c.Width = 60;
                    c.Header.Caption = "Type";
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                    c.ValueList = gridContents.DisplayLayout.ValueLists["ServiceTypeList"];
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
            }
            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;

        }

        private void ContentControl_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && !_isDataLoaded)
            {
                Cursor.Current = Cursors.WaitCursor;
                
                DataBind();
                _isDataLoaded = true;

                Cursor.Current = Cursors.Default;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            const string fileFilter = "Word objects (*.jpg;*.jpeg;*.gif;*.png;*.bmp)|" + 
                                      "*.jpg;*.jpeg;*.gif;*.png;*.bmp|All files (*.*)|*.*";
            var file = App.SelectExternalFile(true, "Choose image", fileFilter, 1);
            txtImage.Text = file;
            srcContents.EndEdit();
        }

        private void cmbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            srcContentTypes.EndEdit();

            // TODO: for TravelEssence, set name to type
            if (Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "575E7900-BF13-42D1-A661-2242510C3359".ToLower())
            {
                txtNames.Text = cmbTypes.Text;
                srcContents.EndEdit();
            }
        }
    }
}
