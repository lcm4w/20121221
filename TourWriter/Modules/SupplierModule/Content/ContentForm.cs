using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Services;

namespace TourWriter.Modules.SupplierModule.Content
{
    public partial class ContentForm : Form
    {
        public int? SupplierOrServiceId { get; set; }
        public int? ContentId { get; set; }
        public string DisplayName { get; set; }
        private readonly SupplierMain _mainForm;

        public ContentForm(SupplierMain mainForm, int? currentContentId)
        {
            ContentId = currentContentId;
            _mainForm = mainForm;

            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            // load content data
            if (_mainForm.ContentTableCache == null || _mainForm.ContentTableCacheIsDirty)
            {
                Cursor = Cursors.WaitCursor;
                Thread.Sleep(200); // show cursor...
                try
                {
                    const string sql = "select ContentID, SupplierID, ContentName, Heading, Body, ImagePath from Content";
                    var reader = Info.Services.DatabaseHelper.ExecuteReader(sql);
                    _mainForm.ContentTableCache = new Info.SupplierSet.ContentDataTable();
                    _mainForm.ContentTableCache.Load(reader);
                    _mainForm.ContentTableCacheIsDirty = false;
                }
                finally { Cursor = Cursors.Default; }
            }
            grid.DataSource = null;
            grid.DataSource = _mainForm.ContentTableCache;

            // highlight row
            if (ContentId.HasValue)
            {
                foreach (var row in grid.Rows)
                    if ((int)row.Cells["ContentID"].Value == ContentId)
                    {
                        grid.ActiveRow = row;
                        row.Selected = true;
                        break;
                    }
            } 
        }

        private void SetContentCachesDirty()
        {
            foreach (var supplierForm in _mainForm.MdiParent.MdiChildren.Where(x => x.GetType() == typeof(SupplierMain)))
                ((SupplierMain)supplierForm).ContentTableCacheIsDirty = true;
        }

        private void ContentForm_Load(object sender, EventArgs e)
        {
            txtEntityName.Text = DisplayName;
        }

        private void gridContents_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ContentName") { }
                else if (c.Key == "Heading") { }
                else if (c.Key == "Body") { }
                else if (c.Key == "ImagePath") { c.Header.Caption = "Image"; }
                else c.Hidden = true;
            }
            var index = 0;
            e.Layout.Bands[0].Columns["ContentName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Heading"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Body"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ImagePath"].Header.VisiblePosition = index++;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (grid.ActiveRow != null)
                ContentId = ((int)grid.ActiveRow.Cells["ContentID"].Value);
            
            DialogResult = DialogResult.OK;
        }

        private void grid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.GetType() == typeof(UltraGridEmptyRow)) return;
            ContentId = ((int)grid.ActiveRow.Cells["ContentID"].Value);

            DialogResult = DialogResult.OK;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var content = _mainForm.ContentTableCache.NewContentRow();
            content.ContentName = "New Content";
            content.SupplierID = 1;
            var o = Info.Services.DatabaseHelper.ExecuteScalar(string.Format(
                "insert into content (supplierid,contentname) values (1,'{0}'); select SCOPE_IDENTITY(); ", content.ContentName));

            int id;
            if (o != null && int.TryParse(o.ToString(), out id))
            {
                content.ContentID = id;
                _mainForm.ContentTableCache.AddContentRow(content);
                grid.ActiveRow = grid.Rows[grid.Rows.Count - 1];
                Thread.Sleep(250);
                ContentId = id;
                DialogResult = DialogResult.OK;
            }

            SetContentCachesDirty();
        }
        
        private void btnDel_Click(object sender, EventArgs e)
        {
            var row = grid.ActiveRow;
            if (row == null || !App.AskDeleteRow()) return;
            var i = row.Index;

            // delete
            var id = (int)row.Cells["ContentID"].Value;
            Info.Services.DatabaseHelper.ExecuteScalar(string.Format("delete content where contentid = {0}", id));

            // remove from cache
            var c = _mainForm.ContentTableCache.Where(x => x.RowState != DataRowState.Deleted && x.ContentID == id).FirstOrDefault();
            if (c != null)
            {
                c.Delete();
                c.AcceptChanges();
            }
            
            // remove from supplierSet
            c = _mainForm.supplierSet.Content.Where(x => x.RowState != DataRowState.Deleted && x.ContentID == id).FirstOrDefault();
            if (c != null)
            {
                // supplier content
                foreach (var sup in _mainForm.supplierSet.SupplierContent.Where(x => x.RowState != DataRowState.Deleted && x.ContentID == id))
                    sup.Delete();

                // service content
                foreach (var srv in _mainForm.supplierSet.SupplierContent.Where(x => x.RowState != DataRowState.Deleted && x.ContentID == id))
                    srv.Delete();

                // content
                c.Delete();
                c.AcceptChanges();
            }

            SetContentCachesDirty();

            if ((i > grid.Rows.Count - 1 ? --i : i) > -1)
                grid.ActiveRow = grid.Rows[i];
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _mainForm.ContentTableCache = null;
            LoadData();
        }
    }
}