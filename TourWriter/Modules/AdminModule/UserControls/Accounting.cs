using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TourWriter.Forms;
using TourWriter.Global;
using TourWriter.Info;

namespace TourWriter.Modules.AdminModule.UserControls
{
    public partial class Accounting : UserControl
    {
        private const string PurchaseTermColumnName = "PurchaseTerm";
        private const string SaleTermColumnName = "SaleTerm";

        public Accounting()
        {
            InitializeComponent();
            
            InitializeData();
        }

        private void InitializeData()
        {
            agentDefaultBindingSource.DataSource = Cache.ToolSet.Agent;
            agentBindingSource.DataSource = Cache.ToolSet.Agent;
            paymentDueListBindingSource.DataSource = Cache.ToolSet.PaymentDue;
            paymentDueAgentBindingSource.DataSource = Cache.ToolSet.PaymentDue;
            taxTypesListBindingSource.DataSource = Cache.ToolSet.TaxType;
            taxTypesServiceBindingSource.DataSource = Cache.ToolSet.ServiceType;
            accountCategoryBindingSource.DataSource = Cache.ToolSet.AccountingCategory;
            requestStatusBindingSource.DataSource = Cache.ToolSet.RequestStatus;
            appSettingsBindingSource.DataSource = Cache.ToolSet.AppSettings;
            paymentsBindingSource.DataSource = Cache.ToolSet.PaymentType;

            // get the accounting template category by name
            var category = (Cache.ToolSet.TemplateCategory.Where(cat => cat.TemplateCategoryName == "Accounting")).First();
           
            // filter by template category ID
            var view = new DataView(Cache.ToolSet.Template);
            view.RowFilter = "ParentTemplateCategoryID = " + category.TemplateCategoryID;
            templateBindingSource.DataSource = view;

            // sort so visible agent is the default one
            // position does not change for this binding source
            agentDefaultBindingSource.Sort = "IsDefaultAgent DESC";

            // sort agent list
            agentBindingSource.Sort = "IsDefaultAgent DESC, AgentName ASC ";

            // accounting interface type
            var list = new Dictionary<string, string> { { "", "" }, { "Myob", "Myob" }, { "Xero", "Xero" } };
            cmbInterfaceType.DataSource = new BindingSource(list, null);
            cmbInterfaceType.ValueMember = "Value";
            cmbInterfaceType.DisplayMember = "Key";
            cmbInterfaceType.DataBindings.Add("SelectedValue", Cache.ToolSet.AppSettings, "AccountingSystem");
        }

        private int GetMarkupContainerComboBoxIndexByPaymentTypeID(int serviceTypeId)
        {
            for (int i = 0; i < cmbMarkupContainer.Items.Count; i++)
            {
                if (serviceTypeId == ((ToolSet.ServiceTypeRow)cmbMarkupContainer.Items[i]).ServiceTypeID)
                    return i;
            }
            return -1;
        }

        private void InitializeMarkupContainerComboBox()
        {
            cmbMarkupContainer.SelectedIndexChanged -= cmbMarkupContainer_SelectedIndexChanged;

            cmbMarkupContainer.Items.Clear();
            foreach (ToolSet.ServiceTypeRow row in Cache.ToolSet.ServiceType)
            {
                cmbMarkupContainer.Items.Add(row);
            }
            cmbMarkupContainer.ValueMember = "ServiceTypeID";
            cmbMarkupContainer.DisplayMember = "ServiceTypeName";

            if (Cache.ToolSet.ServiceType.MarkupContainerRow != null && Cache.ToolSet.ServiceType.MarkupContainerRow.RowState != DataRowState.Deleted)
            {
                cmbMarkupContainer.SelectedIndex = GetMarkupContainerComboBoxIndexByPaymentTypeID(Cache.ToolSet.ServiceType.MarkupContainerRow.ServiceTypeID);
            }
            else
            {
                cmbMarkupContainer.SelectedIndex = -1;
            }

            cmbMarkupContainer.SelectedIndexChanged += cmbMarkupContainer_SelectedIndexChanged;
        }

        private int GetPaymentTypeComboBoxIndexByPaymentTypeID(int paymentTypeId)
        {
            for (int i = 0; i < cmbDefaultPaymentType.Items.Count; i++)
            {
                if (paymentTypeId == ((ToolSet.PaymentTypeRow)cmbDefaultPaymentType.Items[i]).PaymentTypeID)
                    return i;
            }
            return -1;
        }

        private void InitializePaymentTypeComboBox()
        {
            cmbDefaultPaymentType.SelectedIndexChanged -= cmbDefaultPaymentType_SelectedIndexChanged;

            cmbDefaultPaymentType.Items.Clear();
            foreach (ToolSet.PaymentTypeRow row in Cache.ToolSet.PaymentType)
            {
                cmbDefaultPaymentType.Items.Add(row);
            }
            cmbDefaultPaymentType.ValueMember = "PaymentTypeID";
            cmbDefaultPaymentType.DisplayMember = "PaymentTypeName";

            if (Cache.ToolSet.PaymentType.DefaultPaymentTypeRow != null)
            {
                cmbDefaultPaymentType.SelectedIndex = GetPaymentTypeComboBoxIndexByPaymentTypeID(Cache.ToolSet.PaymentType.DefaultPaymentTypeRow.PaymentTypeID);
            }
            else
            {
                cmbDefaultPaymentType.SelectedIndex = -1;
            }

            cmbDefaultPaymentType.SelectedIndexChanged += cmbDefaultPaymentType_SelectedIndexChanged;
        }

        private void InitializeCustomPaymentTermCells()
        {
            foreach (DataGridViewRow row in gridAgentPaymentTerms.Rows)
            {
                UpdateCustomPaymentTermCell(PurchaseTermColumnName, row.Index);
                UpdateCustomPaymentTermCell(SaleTermColumnName, row.Index);
            }
        }

        private void UpdateCustomPaymentTermCell(string columnName, int rowIndex)
        {
            int agentId = (int)gridAgentPaymentTerms["AgentID", rowIndex].Value;
            int? paymentTermID = GetPaymentTermID(columnName, agentId);

            DataGridViewCellCollection cells = gridAgentPaymentTerms.Rows[rowIndex].Cells;

            if (paymentTermID.HasValue)
            {
                AgentSet.PaymentTermRow paymentTermRow =
                    Cache.AgentSet.PaymentTerm.FindByPaymentTermID((int)paymentTermID);

                // chop the \r\n off the end to prevent the cell from having an extra line
                string text = paymentTermRow.GetCustomText(Cache.ToolSet.PaymentDue);
                if (text.Substring(text.Length - 2) == "\r\n")
                    text = text.Remove(text.Length - 2);

                cells[columnName].Value = text;
            }
            else
            {
                cells[columnName].Value = String.Empty;
            }
        }

        /// <summary>
        /// Initializes the values in a PaymentTermsEditor.
        /// </summary>
        /// <param name="termsEditor">The PaymentTermsEditor to initialize.</param>
        /// <param name="columnName">Which terms to get (Purchase or Sale).</param>
        /// <param name="agentId">The ID of the relevant agent.</param>
        private static void InitializePaymentTermsEditor(PaymentTermsEditor termsEditor,
                                                         string columnName, int agentId)
        {
            int? paymentTermID = GetPaymentTermID(columnName, agentId);

            // if there's no existing terms then don't bother
            bool hasExistingTerms = (paymentTermID != null);
            if (hasExistingTerms)
            {
                AgentSet.PaymentTermRow paymentTermRow
                    = Cache.AgentSet.PaymentTerm.FindByPaymentTermID((int)paymentTermID);

                // fill in payment terms
                termsEditor.PaymentDueID = paymentTermRow.PaymentDueID;
                termsEditor.PaymentDuePeriod = (!paymentTermRow.IsPaymentDuePeriodNull())
                                               ? (int?)paymentTermRow.PaymentDuePeriod
                                               : null;

                if (!paymentTermRow.IsDepositAmountNull())
                {
                    // fill in deposit info
                    termsEditor.DepositRequired = true;
                    termsEditor.DepositAmount = paymentTermRow.DepositAmount;
                    termsEditor.DepositType = paymentTermRow.DepositType;
                    termsEditor.DepositDueID = paymentTermRow.DepositDueID;
                    termsEditor.DepositDuePeriod = (!paymentTermRow.IsDepositDuePeriodNull())
                                                   ? (int?)paymentTermRow.DepositDuePeriod
                                                   : null;
                }
            }
        }

        /// <summary>
        /// Opens a PaymentTermsEditor and if OK is clicked, it will either add
        /// a new PaymentTermRow to the table, or update the existing one.
        /// </summary>
        /// <param name="termsEditor">The PaymentTermsEditor to open.</param>
        /// <param name="columnName">Which terms are being edited (Purchase or Sale).</param>
        /// <param name="agentId">The ID of the relevant agent.</param>
        private static void OpenPaymentTermsEditor(PaymentTermsEditor termsEditor,
                                                   string columnName, int agentId)
        {
            AgentSet.PaymentTermRow paymentTermRow;
            int? paymentTermID = GetPaymentTermID(columnName, agentId);

            // create a new row if there's no existing terms, otherwise use the existing row
            bool hasExistingTerms = (paymentTermID != null);
            if (hasExistingTerms)
            {
                paymentTermRow = Cache.AgentSet.PaymentTerm.FindByPaymentTermID((int)paymentTermID);
            }
            else
            {
                paymentTermRow = Cache.AgentSet.PaymentTerm.NewPaymentTermRow();
            }

            DialogResult result = termsEditor.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (termsEditor.DeleteTerms)
                {
                    paymentTermRow.Delete();
                }
                else
                {
                    // dump all the values from the editor into the row
                    paymentTermRow.PaymentDueID = (int)termsEditor.PaymentDueID;

                    if (termsEditor.PaymentDuePeriod.HasValue)
                        paymentTermRow.PaymentDuePeriod = (int)termsEditor.PaymentDuePeriod;
                    else
                        paymentTermRow.SetPaymentDuePeriodNull();

                    if (termsEditor.DepositRequired)
                    {
                        // fill in deposit fields
                        paymentTermRow.DepositAmount = (decimal)termsEditor.DepositAmount;
                        paymentTermRow.DepositType = (char)termsEditor.DepositType;
                        paymentTermRow.DepositDueID = (int)termsEditor.DepositDueID;

                        if (termsEditor.DepositDuePeriod.HasValue)
                            paymentTermRow.DepositDuePeriod = (int)termsEditor.DepositDuePeriod;
                        else
                            paymentTermRow.SetDepositDuePeriodNull();
                    }
                    else
                    {
                        // deposit not required, null all deposit related fields
                        paymentTermRow.SetDepositAmountNull();
                        paymentTermRow.SetDepositTypeNull();
                        paymentTermRow.SetDepositDueIDNull();
                        paymentTermRow.SetDepositDuePeriodNull();
                    }

                    if (!hasExistingTerms)
                    {
                        AgentSet.AgentRow agentRow =
                            Cache.AgentSet.Agent.FindByAgentID(agentId);

                        // add the row to the table
                        Cache.AgentSet.PaymentTerm.AddPaymentTermRow(paymentTermRow);
                        
                        if (columnName == PurchaseTermColumnName)
                            agentRow.PurchasePaymentTermID = paymentTermRow.PaymentTermID;

                        else if (columnName == SaleTermColumnName)
                            agentRow.SalePaymentTermID = paymentTermRow.PaymentTermID;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the relevant payment term ID.
        /// </summary>
        /// <param name="columnName">Which payment term to get (Sale or Purchase)</param>
        /// <param name="agentId">The agent to get the payment terms from.</param>
        /// <returns>The relevant PaymentTermID, or null.</returns>
        private static int? GetPaymentTermID(string columnName, int agentId)
        {
            AgentSet.AgentRow agentRow = Cache.AgentSet.Agent.FindByAgentID(agentId);
            if (agentRow == null)
                return null;

            int? paymentTermID = null;

            if (columnName == PurchaseTermColumnName)
            {
                if (!agentRow.IsPurchasePaymentTermIDNull())
                    paymentTermID = agentRow.PurchasePaymentTermID;
            }
            else if (columnName == SaleTermColumnName)
            {
                if (!agentRow.IsSalePaymentTermIDNull())
                    paymentTermID = agentRow.SalePaymentTermID;
            }

            return paymentTermID;
        }

        #region Events
        void grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Whenever a grid is saved, the custom payment term cells get reset.
            // Every grid must be hooked up to this event.

            // sometimes the PaymentDue table is empty when this event fires
            if (Cache.ToolSet.PaymentDue.Rows.Count == 0)
                return;

            if (e.ListChangedType == ListChangedType.Reset)
            {
                InitializeCustomPaymentTermCells();
                InitializeMarkupContainerComboBox();
                InitializePaymentTypeComboBox();
            }
        }

        private void gridAgentPaymentTerms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            int agentId = (int)gridAgentPaymentTerms["AgentID", e.RowIndex].Value;

            if (gridAgentPaymentTerms.Columns[e.ColumnIndex].Name == "PurchaseTermButton")
            {
                PaymentTermsEditor termsEditor = new PaymentTermsEditor();
                InitializePaymentTermsEditor(termsEditor, PurchaseTermColumnName, agentId);
                OpenPaymentTermsEditor(termsEditor, PurchaseTermColumnName, agentId);
                UpdateCustomPaymentTermCell(PurchaseTermColumnName, e.RowIndex);
            }

            else if (gridAgentPaymentTerms.Columns[e.ColumnIndex].Name == "SaleTermButton")
            {
                PaymentTermsEditor termsEditor = new PaymentTermsEditor();
                InitializePaymentTermsEditor(termsEditor, SaleTermColumnName, agentId);
                OpenPaymentTermsEditor(termsEditor, SaleTermColumnName, agentId);
                UpdateCustomPaymentTermCell(SaleTermColumnName, e.RowIndex);
            }
        }

        private void gridAgentPaymentTerms_KeyUp(object sender, KeyEventArgs e)
        {
            // allow user to clear the selected item in the combobox, by using the
            // delete key or backspace key.
            if((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back ) &&
                gridAgentPaymentTerms.CurrentCell is DataGridViewComboBoxCell)
            {
                // set cell value to null
                gridAgentPaymentTerms.CurrentCell.Value = DBNull.Value;
            }
        }

        private void gridServiceCategoryCodes_KeyUp(object sender, KeyEventArgs e)
        {
            // allow user to clear the selected item in the combobox, by using the
            // delete key or backspace key.
            if ((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) &&
                gridServiceCategoryCodes.CurrentCell is DataGridViewComboBoxCell)
            {
                // set cell value to null
                gridServiceCategoryCodes.CurrentCell.Value = DBNull.Value;
            }
        }

        private void gridServiceTaxTypes_KeyUp(object sender, KeyEventArgs e)
        {
            // allow user to clear the selected item in the combobox, by using the
            // delete key or backspace key.
            if ((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back) &&
                 gridServiceTaxTypes.CurrentCell is DataGridViewComboBoxCell)
            {
                // set cell value to null
                gridServiceTaxTypes.CurrentCell.Value = DBNull.Value;
            }
        }

        private void gridAgentPaymentTerms_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Don't allow new selection in combobox if any of the combobox items 
            // are not saved. New items will have negative id number (-1) that will
            // change after save, making forign-key invalid.
            bool lookupTableIsDirty = (Cache.ToolSet.PaymentDue.GetChanges() != null);
            if (lookupTableIsDirty)
            {
                e.Cancel = true;
                App.ShowInfo("Please save changes before taking this action");
            }
        }

        private void gridServiceCategoryCodes_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Don't allow new selection in combobox if any of the combobox items 
            // are not saved. New items will have negative id number (-1) that will
            // change after save, making forign-key invalid.
            bool lookupTableIsDirty = (Cache.ToolSet.AccountingCategory.GetChanges() != null);
            if (lookupTableIsDirty)
            {
                e.Cancel = true;
                App.ShowInfo("Please save changes before taking this action");
            }
        }

        private void gridServiceTypes_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Don't allow new selection in combobox if any of the combobox items 
            // are not saved. New items will have negative id number (-1) that will
            // change after save, making forign-key invalid.
            bool lookupTableIsDirty = (Cache.ToolSet.TaxType.GetChanges() != null);
            if (lookupTableIsDirty)
            {
                e.Cancel = true;
                App.ShowInfo("Please save changes before taking this action");
            }
        }

        private void gridAgentPaymentTerms_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // these can be caused by invalid forign-key, after the related row
            // has been deleted from lookup table, item in combo no longer exists.
            gridAgentPaymentTerms[e.ColumnIndex, e.RowIndex].Value = DBNull.Value;
        }

        private void gridServiceCategoryCodes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // these can be caused by invalid forign-key, after the related row
            // has been deleted from lookup table, item in combo no longer exists.
            gridServiceCategoryCodes[e.ColumnIndex, e.RowIndex].Value = DBNull.Value;
        }

        private void gridServiceTaxTypes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // these can be caused by invalid forign-key, after the related row
            // has been deleted from lookup table, item in combo no longer exists.
            gridServiceTaxTypes[e.ColumnIndex, e.RowIndex].Value = DBNull.Value;
        }

        private void gridTaxTypes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (App.AskYesNo("Amount is invalid, would you like to set it to 0.00?"))
            {
                gridTaxTypes[e.ColumnIndex, e.RowIndex].Value = 0.00m;
                gridTaxTypes.RefreshEdit();
            }
        }

        private void btnCategoryAdd_Click(object sender, EventArgs e)
        {
            ToolSet.AccountingCategoryRow r = Cache.ToolSet.AccountingCategory.NewAccountingCategoryRow();
            r.AccountingCategoryCode = "New Category";

            Cache.ToolSet.AccountingCategory.AddAccountingCategoryRow(r);
            gridAccountingCategories.CurrentCell
                = gridAccountingCategories[0, gridAccountingCategories.Rows.Count - 2];
            gridAccountingCategories.CurrentRow.Selected = true;
        }

        private void btnCategoryDel_Click(object sender, EventArgs e)
        {
            if (gridAccountingCategories.CurrentRow != null &&
                !gridAccountingCategories.CurrentRow.IsNewRow &&
                App.AskDeleteRow())
            {
                gridAccountingCategories.Rows.Remove(gridAccountingCategories.CurrentRow);
            }
        }

        private void btnTaxTypeAdd_Click(object sender, EventArgs e)
        {
            ToolSet.TaxTypeRow r = Cache.ToolSet.TaxType.NewTaxTypeRow();
            r.TaxTypeName = "New Tax Type";
            Cache.ToolSet.TaxType.AddTaxTypeRow(r);
            gridTaxTypes.CurrentCell
                = gridTaxTypes[0, gridTaxTypes.Rows.Count - 2];
            gridTaxTypes.CurrentRow.Selected = true;
        }

        private void btnTaxTypeDel_Click(object sender, EventArgs e)
        {
            if (gridTaxTypes.CurrentRow != null &&
                !gridTaxTypes.CurrentRow.IsNewRow &&
                App.AskDeleteRow())
            {
                gridTaxTypes.Rows.Remove(gridTaxTypes.CurrentRow);
            }
        }

        private void cmbMarkupContainer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMarkupContainer.SelectedIndex != -1)
            {
                int id = ((ToolSet.ServiceTypeRow)cmbMarkupContainer.Items[cmbMarkupContainer.SelectedIndex]).ServiceTypeID;
                ToolSet.ServiceTypeRow row = Cache.ToolSet.ServiceType.FindByServiceTypeID(id);
                Cache.ToolSet.ServiceType.MarkupContainerRow = row;
            }
            else
            {
                Cache.ToolSet.ServiceType.MarkupContainerRow = null;
            }
        }

        private void cmbDefaultPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDefaultPaymentType.SelectedIndex != -1)
            {
                int id = ((ToolSet.ServiceTypeRow)cmbMarkupContainer.Items[cmbDefaultPaymentType.SelectedIndex]).ServiceTypeID;
                ToolSet.PaymentTypeRow row = Cache.ToolSet.PaymentType.FindByPaymentTypeID(id);
                Cache.ToolSet.PaymentType.DefaultPaymentTypeRow = row;
            }
            else
            {
                Cache.ToolSet.PaymentType.DefaultPaymentTypeRow = null;
            }
        }

        private void gridTemplates_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridTemplates.Columns[e.ColumnIndex] == browseColumn)
            {
                string fileName = App.SelectExternalFile(true, "Choose template", "Accounting export template (*.txt;*.csv;*.tab)|*.txt*.csv;*.tab|All files (*.*)|*.*", 0);

                if (!String.IsNullOrEmpty(fileName))
                    gridTemplates[filePathColumn.Name, e.RowIndex].Value = fileName;
            }
        }
        #endregion
    }
}
