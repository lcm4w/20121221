using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Modules.SupplierModule.Content;
using TourWriter.Services;

namespace TourWriter.Modules.SupplierModule
{
    public partial class ContentControl : UserControl
    {
        private DataTable _table;
        private SupplierSet _supplierSet;
        internal SupplierSet SupplierSet
        {
            get { return _supplierSet; }
            set
            {
                _supplierSet = value;
                DataBind();
            }
        }
        internal SupplierMain MainForm;

        public ContentControl()
        {
            InitializeComponent();
        
            gridContents.AfterRowActivate += gridContents_AfterRowActivate;
            gridContents.AfterCellListCloseUp += gridContents_AfterCellListCloseUp;
            gridContents.DoubleClickRow += gridContents_DoubleClickRow;
        }
        
        private void DataBind()
        {
            _table = new DataTable();
            _table.Columns.Add("SupplierContentID", typeof(int));
            _table.Columns.Add("ServiceContentID", typeof(int));
            _table.Columns.Add("EntityName", typeof(string));
            _table.Columns.Add("ContentID", typeof(int));
            _table.Columns.Add("ContentName", typeof(string));
            _table.Columns.Add("TypeID", typeof(int));
            
            var supplier = _supplierSet.Supplier[0];

            // add supplier content rows
            foreach (var s in _supplierSet.SupplierContent.Where(x => x.RowState != DataRowState.Deleted))
                AddSupplierContentBindingRow(s);
            
            // add service content rows
            foreach (var s in _supplierSet.ServiceContent.Where(x => x.RowState != DataRowState.Deleted))
                AddServiceContentBindingRow(s);

            // init content types combo
            if (!gridContents.DisplayLayout.ValueLists.Exists("ContentTypeList"))
                gridContents.DisplayLayout.ValueLists.Add("ContentTypeList");
            else 
                gridContents.DisplayLayout.ValueLists["ContentTypeList"].ValueListItems.Clear();

            // add content types
            gridContents.DisplayLayout.ValueLists["ContentTypeList"].ValueListItems.Add(DBNull.Value, "");
            foreach (var r in Enumerable.Where(Cache.ToolSet.ContentType, x => x.RowState != DataRowState.Deleted))
                gridContents.DisplayLayout.ValueLists["ContentTypeList"].ValueListItems.Add(r.ContentTypeID, r.ContentTypeName);

            // load Add items.
            btnAdd.DropDownItems.Clear();
            btnAdd.DropDownItems.Add(new ToolStripMenuItem("Add for...") 
                                    { Enabled = false });
            btnAdd.DropDownItems.Add(new ToolStripSeparator());
            btnAdd.DropDownItems.Add(new ToolStripMenuItem("Supplier: " + supplier.SupplierName.Trim(), null, btnAddItem_Click)
                                    {Tag = "supplier:" + _supplierSet.Supplier[0].SupplierID});

            foreach (var service in _supplierSet.Service.Where(x => x.RowState != DataRowState.Deleted).OrderBy(x => x.ServiceName))
                btnAdd.DropDownItems.Add(new ToolStripMenuItem("Service: " + service.ServiceName.Trim(), null, btnAddItem_Click)
                                         {Tag = "service:" + service.ServiceID});

            gridContents.DataSource = _table;
        }
        
        private void gridContents_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("ContentName"))
                e.Layout.Bands[0].Columns.Add("ContentName");

            // show/hide columns 
            foreach (var c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "EntityName")
                {
                    c.Header.Caption = "For";
                    c.Header.ToolTipText = "Name of connected Supplier or Service";
                }
                else if (c.Key == "ContentName")
                {
                    c.Header.Caption = "Content name";
                    c.Header.ToolTipText = "Name of connected Content record";
                }
                else if (c.Key == "TypeID")
                {
                    c.Header.Caption = "Type";
                    c.Header.ToolTipText = "The Type (or category)";
                    c.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                    c.ValueList = gridContents.DisplayLayout.ValueLists["ContentTypeList"];
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }
                else
                    c.Hidden = true;
            }
            var index = 0;
            e.Layout.Bands[0].Columns["EntityName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["ContentName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["TypeID"].Header.VisiblePosition = index++;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
        }

        void gridContents_AfterRowActivate(object sender, EventArgs e)
        {
            var id = (int)gridContents.ActiveRow.Cells["ContentID"].Value;
            var content = _supplierSet.Content.Where(x => x.RowState != DataRowState.Deleted && x.ContentID == id).FirstOrDefault();
            if (content != null) contentEditor1.Content = content;
        }
        
        void gridContents_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            //var form = new ContentForm
            //{
            //    SupplierOrServiceContentId =
            //        (int)(e.Row.Cells["SuplierContentID"].Value != DBNull.Value ? e.Row.Cells["SuplierContentID"].Value : e.Row.Cells["ServiceContentID"].Value),
            //    EntityType = e.Row.Cells["SuplierContentID"].Value != DBNull.Value ? "Supplier" : "Service",
            //    EntityName = e.Row.Cells["EntityName"].Value.ToString()
            //};
            //if (form.ShowDialog() == DialogResult.OK)
            //{
            //    //ContentChooserComplete(form, row);
            //}
        }

        private void gridContents_AfterCellListCloseUp(object sender, CellEventArgs e)
        {
            e.Cell.Row.Cells["TypeID"].EditorResolved.ExitEditMode(true, true);
            var typeId = e.Cell.Row.Cells["TypeID"].Value != DBNull.Value ? (int?)e.Cell.Row.Cells["TypeID"].Value : null;

            if (e.Cell.Row.Cells["SupplierContentID"].Value != DBNull.Value)
            {
                // update supplier content
                var id = (int)e.Cell.Row.Cells["SupplierContentID"].Value;
                var sup = _supplierSet.SupplierContent.Where(x => x.RowState != DataRowState.Deleted && x.SupplierContentID == id).FirstOrDefault();
                if (sup != null)
                {
                    if (typeId.HasValue) 
                        sup.ContentTypeID = (int)typeId;
                    else sup.SetContentTypeIDNull();
                }
            }
            else if (e.Cell.Row.Cells["ServiceContentID"].Value != DBNull.Value)
            {
                // update service content
                var id = (int)e.Cell.Row.Cells["ServiceContentID"].Value;
                var srv = _supplierSet.ServiceContent.Where(x => x.RowState != DataRowState.Deleted && x.ServiceContentID == id).FirstOrDefault();
                if (srv != null)
                {
                    if (typeId.HasValue)
                        srv.ContentTypeID = (int)typeId;
                    else srv.SetContentTypeIDNull();
                }
            }

            // TODO: setting contenttype of back-compat for TE
            var contentId = (int)e.Cell.Row.Cells["ContentID"].Value;
            var content = _supplierSet.Content.Where(x => x.RowState != DataRowState.Deleted && x.ContentID == contentId).FirstOrDefault();
            if (content != null)
            {
                if (typeId.HasValue)
                {
                    content.ContentTypeID = (int) typeId;

                    // TODO: for TravelEssence, set name to type
                    if (Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "575E7900-BF13-42D1-A661-2242510C3359".ToLower())
                    {
                        e.Cell.Row.Cells["ContentName"].Value = e.Cell.Text;
                        content.ContentName = e.Cell.Text;
                        contentEditor1.RefreshName();
                    }
                }
                else content.SetContentTypeIDNull();
            }
        }
        
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripDropDownItem;
            if (item == null) return;
   
            // run content form
            var form = new ContentForm(MainForm, null) { DisplayName = item.Text };
            if (form.ShowDialog() != DialogResult.OK) return;

            // get selected content row
            var content = MainForm.ContentTableCache.Where(x => x.ContentID == form.ContentId).FirstOrDefault();
            if (content == null) return;

            // import it into supplier set
            if (_supplierSet.Content.Where(x => x.ContentID == content.ContentID).Count() == 0)
                _supplierSet.Content.ImportRow(content);
            
            // create new SupplierContent
            if (item.Tag.ToString().StartsWith("supplier"))
            {
                var supplierContent = _supplierSet.SupplierContent.NewSupplierContentRow();
                supplierContent.SupplierID = int.Parse(item.Tag.ToString().Split(':')[1]);
                supplierContent.ContentID = content.ContentID;
                content.SupplierID = supplierContent.SupplierID; // TODO: hack, temp-back-compat for TE
                _supplierSet.SupplierContent.AddSupplierContentRow(supplierContent);
                AddSupplierContentBindingRow(supplierContent);
            }

            // or new ServiceContent
            else if (item.Tag.ToString().StartsWith("service"))
            {
                var serviceContent = _supplierSet.ServiceContent.NewServiceContentRow();
                serviceContent.ServiceID = int.Parse(item.Tag.ToString().Split(':')[1]);
                serviceContent.ContentID = content.ContentID;
                _supplierSet.ServiceContent.AddServiceContentRow(serviceContent);
                AddServiceContentBindingRow(serviceContent);
            }
            gridContents.ActiveRow = gridContents.Rows[gridContents.Rows.Count - 1];
        }

        private void AddSupplierContentBindingRow(SupplierSet.SupplierContentRow row)
        {
            var content = _supplierSet.Content.Where(x => x.RowState != DataRowState.Deleted && x.ContentID == row.ContentID).FirstOrDefault();
            if (content == null) { row.Delete(); return; } // cleanup

            var supplier = _supplierSet.Supplier[0];
            _table.Rows.Add(row.SupplierContentID, null, supplier.SupplierName, row.ContentID, content.ContentName,
                !row.IsContentTypeIDNull() ? row.ContentTypeID : (int?)null);
        }

        private void AddServiceContentBindingRow(SupplierSet.ServiceContentRow row)
        {
            var content = _supplierSet.Content.Where(x => x.RowState != DataRowState.Deleted && x.ContentID == row.ContentID).FirstOrDefault();
            if (content == null) { row.Delete(); return; } // cleanup

            var service = _supplierSet.Service.Where(x => x.RowState != DataRowState.Deleted && x.ServiceID == row.ServiceID).FirstOrDefault();
            if (service == null) { row.Delete(); return; } // cleanup

            _table.Rows.Add(null, row.ServiceContentID, service.ServiceName, row.ContentID, content.ContentName,
                !row.IsContentTypeIDNull() ? row.ContentTypeID : (int?)null);
        }
        
        private void btnDel_Click(object sender, EventArgs e)
        {
            var row = gridContents.ActiveRow;
            if (row == null || !App.AskDeleteRow()) return;
            var i = row.Index;

            if (row.Cells["SupplierContentID"].Value != DBNull.Value)
            {
                var id = (int)row.Cells["SupplierContentID"].Value;
                var sup = _supplierSet.SupplierContent.Where(x => x.RowState != DataRowState.Deleted && x.SupplierContentID == id).FirstOrDefault();
                if (sup != null)
                {
                    sup.Delete();
                    row.Delete(false);
                }
            }
            else if (row.Cells["ServiceContentID"].Value != DBNull.Value)
            {
                var id = (int)row.Cells["ServiceContentID"].Value;
                var srv = _supplierSet.ServiceContent.Where(x => x.RowState != DataRowState.Deleted && x.ServiceContentID == id).FirstOrDefault();
                if (srv != null)
                {
                    srv.Delete();
                    row.Delete(false);
                }
            }

            if ((i > gridContents.Rows.Count - 1 ? --i : i) > -1)
                gridContents.ActiveRow = gridContents.Rows[i];
        }
    }
}
