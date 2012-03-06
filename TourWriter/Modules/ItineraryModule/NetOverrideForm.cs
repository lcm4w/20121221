using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinCalcManager;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;
using TourWriter.Info.Services;
using TourWriter.Services;
using System.Collections;

namespace TourWriter.Modules.ItineraryModule
{
    public partial class NetOverrideForm : Form
    {
        private readonly ItinerarySet itinerarySet;
        private string marginMinOrMax;        

        public NetOverrideForm(ItinerarySet itinerarySet)
        {
            InitializeComponent();
            Icon = Properties.Resources.TourWriter16;
            this.itinerarySet = itinerarySet;
            Load += NetOverrideForm_Load;
        }

        void NetOverrideForm_Load(object sender, EventArgs e)
        {
            var hasLocks = itinerarySet.PurchaseItem.Any(x => x.RowState != DataRowState.Deleted && !x.IsIsLockedAccountingNull() && x.IsLockedAccounting);
            if (hasLocks && !App.AskYesNo(
                "Warning: Itinerary contains locked bookings (exported to Accounting).\r\n\r\nAny changes you make to margins could change the pricing of the locked bookings.\r\n\r\nContinue?")) 
                Close();

            DataBind();

        }

        private void DataBind()
        {
            // populate temp binding table from service types and matching overrides
            DataTable serviceTypeCopy = Cache.ToolSet.ServiceType.Copy();
            
            // copy the row filter settings from the original table
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(serviceTypeCopy);
            dataSet.DefaultViewManager.DataViewSettings[dataSet.Tables[0].TableName].RowFilter =
                Cache.ToolSet.DefaultViewManager.DataViewSettings[Cache.ToolSet.ServiceType.TableName].RowFilter;

            serviceTypeCopy.Columns.Add("Override", typeof(decimal));            
            grid.DataSource = serviceTypeCopy;
            foreach (DataRow row in serviceTypeCopy.Rows)
            {
                // populate override column
                ItinerarySet.ItineraryMarginOverrideRow overrideRow = 
                    itinerarySet.ItineraryMarginOverride.FindByItineraryIDServiceTypeID(
                    itinerarySet.Itinerary[0].ItineraryID, (int)row["ServiceTypeID"]);

                if (overrideRow != null)
                    row["Override"] = overrideRow.Margin;
            }

            LoadMarginSelection();

            // check ComOrMup and MinOrMax value
            DisplayMarginSelection(itinerarySet.Itinerary[0].NetComOrMup, itinerarySet.Itinerary[0].NetMinOrMax);
             
            // check override type (service type, or master)
            if (!itinerarySet.Itinerary[0].IsNetMarginNull())
            {
                rdMasterOverride.Checked = true;
                txtMasterOverride.Value = itinerarySet.Itinerary[0].NetMargin;
            }
            else
            {
                rdServiceTypeOverride.Checked = true;
                txtMasterOverride.Value = null;
            }
        }        

        private void LoadMarginSelection()
        {
            cmbOverride.DataSource = new ArrayList {new { Text = "Minimum of", Value = "min" }, 
                                                 new { Text = "Maximum of", Value = "max" },
                                                 new { Text = "Exactly", Value = "exact" }};
            cmbOverride.SelectedIndex = -1;
            cmbOverride.SelectedIndexChanged += cmbOverride_SelectedIndexChanged;            

            cmbMargin.DataSource = new ArrayList {new { Text = "Markup", Value = "mup" }, 
                                                 new { Text = "Commission", Value = "com" },
                                                 new { Text = "Agent Commission", Value = "grs" }};
            cmbMargin.SelectedIndex = -1;
            cmbMargin.SelectedIndexChanged += cmbMargin_SelectedIndexChanged;            
        }

        private void DisplayMarginSelection(string margin, string @override)
        {
            switch (margin)
            {
                case "mup":
                    cmbMargin.SelectedIndex = 0;
                    break;
                case "com":
                    cmbMargin.SelectedIndex = 1;
                    break;
                case "grs":
                    cmbMargin.SelectedIndex = 2;
                    break;
                default:             
                    cmbMargin.SelectedIndex = -1;
                    break;
            }

            switch (@override)
            {
                case "min":
                    cmbOverride.SelectedIndex = 0;
                    break;
                case "max":
                    cmbOverride.SelectedIndex = 1;
                    break;
                case "exact":
                    cmbOverride.SelectedIndex = 2;
                    break;
                default:
                    cmbOverride.SelectedIndex = -1;
                    break;
            }            
        }  

        private void SaveChanges()
        {
            itinerarySet.Itinerary[0].NetComOrMup = cmbMargin.SelectedIndex >= 0 ? cmbMargin.SelectedValue.ToString() : null;
            itinerarySet.Itinerary[0].NetMinOrMax = cmbOverride.SelectedIndex >= 0 ? cmbOverride.SelectedValue.ToString() : null;

            if (rdMasterOverride.Checked)
            {
                // remove the service type overrides
                ClearOverrides();

                // set the net margin override
                if (txtMasterOverride.Value != DBNull.Value)
                    itinerarySet.Itinerary[0].NetMargin = Convert.ToDecimal(txtMasterOverride.Value);
                else
                    itinerarySet.Itinerary[0].SetNetMarginNull();
            }
            else if (rdServiceTypeOverride.Checked)
            {
                // remove the net margin override
                itinerarySet.Itinerary[0].SetNetMarginNull();
            }                                 

            // add/remove/update the service type overrides
            foreach (UltraGridRow row in grid.Rows)
            {
                int serviceTypeID = (int)row.Cells["ServiceTypeID"].Value;
                decimal? margin = row.Cells["Override"].Value != DBNull.Value ?
                    (decimal?)row.Cells["Override"].Value :
                    null;

                itinerarySet.ItineraryMarginOverride.AddInsertOrDelete(serviceTypeID, margin);
            }
        }

        private void Populate()
        {
            var agentRow = Cache.AgentSet.Agent.FindByAgentID(itinerarySet.Itinerary[0].AgentID);

            bool agentHasOverrides = ((from agent in Cache.AgentSet.AgentMargin where agent.AgentID == agentRow.AgentID select agent).Count() > 0
                                     || (!agentRow.IsParentAgentIDNull() && (from parentAgent in Cache.AgentSet.AgentMargin where parentAgent.AgentID == agentRow.AgentID select parentAgent).Count() > 0));

            if (!agentHasOverrides)
            {
                App.ShowInfo("No overrides found for current agent");
                return;
            }

            if (HasOverrides())
            {
                if (App.AskYesNo("Reset existing overrides?"))
                    ClearOverrides();
            }

            // add the new override rows
            foreach (ToolSet.ServiceTypeRow serviceTypeRow in Cache.ToolSet.ServiceType)
            {
                // get existing margin override row for this service type
                // if it's null, attempt to get the override from the parent agent
                var agentMarginRow =
                    Cache.AgentSet.AgentMargin.FindByAgentIDServiceTypeID(itinerarySet.Itinerary[0].AgentID, serviceTypeRow.ServiceTypeID)
                    ?? ((!agentRow.IsParentAgentIDNull()) ? Cache.AgentSet.AgentMargin.FindByAgentIDServiceTypeID(agentRow.ParentAgentID, serviceTypeRow.ServiceTypeID) : null);

                if (agentMarginRow != null)
                {
                    foreach (UltraGridRow row in grid.Rows)
                    {
                        if ((int)row.Cells["ServiceTypeID"].Value == serviceTypeRow.ServiceTypeID)
                        {
                            row.Cells["Override"].Value = agentMarginRow.Margin;
                            break;
                        }
                    }
                }
            }
        }

        private bool HasOverrides()
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Cells["Override"].Value != DBNull.Value)
                    return true;
            }

            return false;
        }

        private void ClearOverrides()
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                row.Cells["Override"].Value = DBNull.Value;
            }
        }

        private void RecalculatePricing()
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                decimal net = 0;
                decimal gross = 0;
                decimal sell;
                int serviceTypeID = (int)row.Cells["ServiceTypeID"].Value;
                decimal? ovrride = row.Cells["Override"].Value != DBNull.Value ?
                    (decimal?)row.Cells["Override"].Value : null;

                // sum net/gross for this service type
                ItinerarySet.PurchaseItemRow[] rows =
                    (ItinerarySet.PurchaseItemRow[])
                    itinerarySet.PurchaseItem.Select("ServiceTypeID = " + serviceTypeID);
                foreach (ItinerarySet.PurchaseItemRow r in rows)
                {
                    if (r.RowState != DataRowState.Deleted)
                    {
                        net += r.NetTotalConverted;
                        gross += r.GrossTotalConverted;
                    }
                }

                // calculate sell from override
                if (ovrride != null)
                {
                    if (cmbMargin.SelectedValue == "com")
                        sell = Common.CalcGrossByNetCommission(net, (decimal)ovrride);
                    else if (cmbMargin.SelectedValue == "mup")
                        sell = Common.CalcGrossByNetMarkup(net, (decimal)ovrride);
                    else
                        sell = Common.CalcGrossByGrossCommission(gross, (decimal)ovrride);
                }
                else
                {
                    sell = gross;
                }

                // update unbound cells
                row.Cells["Net"].Value = net;
                row.Cells["Gross"].Value = gross;
                row.Cells["Markup"].Value = (net == 0) ? 0 : Common.CalcMarkupByNetGross(net, gross);
                row.Cells["Commission"].Value = (net == 0) ? 0 : Common.CalcCommissionByNetGross(net, gross);
                row.Cells["Sell"].Value = sell;
            }
        }

        private void grid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.Add("Count");
            e.Layout.Bands[0].Columns.Add("Net");
            e.Layout.Bands[0].Columns.Add("Gross");
            e.Layout.Bands[0].Columns.Add("Markup");
            e.Layout.Bands[0].Columns.Add("Commission");
            e.Layout.Bands[0].Columns.Add("Sell");

            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "ServiceTypeName")
                {
                    c.Width = 110;
                    c.Header.Caption = "Service type";
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "Net")
                {
                    c.Header.ToolTipText = "Total supplier net in Itinerary currency";
                    c.Band.SortedColumns.Add(c, true);
                    c.Width = 70;
                    c.MaxWidth = 70;
                    c.DataType = typeof(Decimal);
                    c.Format = "#0.00";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                }
                else if (c.Key == "Markup")
                {
                    c.Header.ToolTipText = "Average supplier markup";
                    c.Width = 80;
                    c.MaxWidth = 80;
                    c.DataType = typeof(Decimal);
                    c.Format = "##0.00\\%";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.Hidden = true;
                    c.TabStop = false;
                }
                else if (c.Key == "Override")
                {
                    c.Header.ToolTipText = "Your override";
                    c.Width = 70;
                    c.MaxWidth = 70;
                    c.Format = "#0.00\\%";
                    c.MaskInput = "nnn.nn% ";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }

                // -- hidden, but required for calcs --------------------------------------
                else if (c.Key == "Commission")
                {
                    c.Hidden = true; // <<<<
                    c.Width = 80;
                    c.MaxWidth = 80;
                    c.DataType = typeof(Decimal);
                    c.Format = "##0.00\\%";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                    c.TabStop = false;
                }
                else if (c.Key == "Gross")
                {
                    c.Hidden = !App.IsDebugMode; // <<<<
                    c.Width = 70;
                    c.MaxWidth = 70;
                    c.DataType = typeof(Decimal);
                    c.Format = "#0.00";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                }
                else if (c.Key == "Sell")
                {
                    c.Hidden = !App.IsDebugMode; // <<<<
                    c.Width = 70;
                    c.MaxWidth = 70;
                    c.DataType = typeof(Decimal);
                    c.Format = "c";
                    c.MaskInput = "$nnnnnnn.nn";
                    c.CellAppearance.TextHAlign = HAlign.Right;
                    c.CellActivation = Activation.NoEdit;
                }
                // -------------------------------------------------------------------------
                else
                {
                    c.Hidden = true;
                    c.TabStop = false;
                }
            }
            int index = 0;
            e.Layout.Bands[0].Columns["ServiceTypeName"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Net"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Markup"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Commission"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Gross"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Override"].Header.VisiblePosition = index++;
            e.Layout.Bands[0].Columns["Sell"].Header.VisiblePosition = index;

            GridHelper.SetDefaultGridAppearance(e);
            e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            grid_CreateSummeraries(e);
        }

        private void grid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            RecalculatePricing();
        }

        private static void grid_CreateSummeraries(InitializeLayoutEventArgs e)
        {
            // UltraCalcManager required for formula summaries
            e.Layout.Grid.CalcManager = new UltraCalcManager(e.Layout.Grid.Container);

            UltraGridBand band = e.Layout.Bands[0];
            band.Override.BorderStyleSummaryValue = UIElementBorderStyle.None;
            band.Override.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed;
            band.Override.SummaryFooterCaptionVisible = DefaultableBoolean.False;
            SummarySettings summary;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Net"]);
            summary.Key = "NetBaseTotal";
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "{0:###,###,###.00}";
            summary.Appearance.TextHAlign = HAlign.Right;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Gross"]);
            summary.Key = "GrossBaseTotal";
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "{0:###,###,###.00}";
            summary.Appearance.TextHAlign = HAlign.Right;

            summary = band.Summaries.Add(SummaryType.Sum, band.Columns["Sell"]);
            summary.Key = "SellTotal";
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "{0:###,###,###.00}";
            summary.Appearance.TextHAlign = HAlign.Right;

            summary = band.Summaries.Add(SummaryType.Formula, band.Columns["Markup"]);
            summary.Key = "MarkupAvg";
            summary.Formula = "(([GrossBaseTotal()] - [NetBaseTotal()])/[NetBaseTotal()]) * 100";
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "*{0:###.00}%";
            summary.Appearance.TextHAlign = HAlign.Right;

            summary = band.Summaries.Add(SummaryType.Formula, band.Columns["Commission"]);
            summary.Key = "CommissionAvg";
            summary.Formula = "(([GrossBaseTotal()] - [NetBaseTotal()])/[GrossBaseTotal()]) * 100";
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "*{0:###.00}%";
            summary.Appearance.TextHAlign = HAlign.Right;

            summary = band.Summaries.Add(SummaryType.Formula, band.Columns["Override"]);
            summary.Key = "OverrideAvg";
            summary.Formula = String.Empty;
            summary.SummaryPosition = SummaryPosition.UseSummaryPositionColumn;
            summary.DisplayFormat = "*{0:###.00}%";
            summary.Appearance.TextHAlign = HAlign.Right;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveChanges();
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnPopulate_Click(object sender, EventArgs e)
        {
            Populate();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (App.AskYesNo("Remove all overrides?"))
                ClearOverrides();
        }

        private void rdMasterOverride_CheckedChanged(object sender, EventArgs e)
        {
            txtMasterOverride.Enabled = rdMasterOverride.Checked;
        }

        private void rdServiceTypeOverride_CheckedChanged(object sender, EventArgs e)
        {
            grid.Enabled = rdServiceTypeOverride.Checked;
            toolStrip1.Enabled = rdServiceTypeOverride.Checked;
        }        

        private void cmbOverride_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbOverride.SelectedIndex >= 0)
            {
                RecalculatePricing();
            }
        }

        private void cmbMargin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMargin.SelectedIndex >= 0)
            {
                grid.DisplayLayout.Bands[0].Columns["Markup"].Hidden = !(cmbMargin.SelectedValue == "mup");
                grid.DisplayLayout.Bands[0].Columns["Commission"].Hidden = !(cmbMargin.SelectedValue == "com");
                grid.DisplayLayout.Bands[0].Columns["Gross"].Hidden = !(cmbMargin.SelectedValue == "grs");

                label14.Visible = true;
                label17.Visible = true;
                cmbOverride.Visible = true;

                switch (cmbMargin.SelectedValue.ToString())
                {
                    case "mup":
                        grid.DisplayLayout.Bands[0].Summaries["OverrideAvg"].Formula =
                            "(([SellTotal()] - [NetBaseTotal()])/[NetBaseTotal()]) * 100";
                        cmbOverride.SelectedValue = "exact";
                        break;
                    case "com":
                        grid.DisplayLayout.Bands[0].Summaries["OverrideAvg"].Formula =
                            "(([SellTotal()] - [NetBaseTotal()])/[SellTotal()]) * 100";
                        cmbOverride.SelectedValue = "exact";
                        break;
                    case "grs":
                        grid.DisplayLayout.Bands[0].Summaries["OverrideAvg"].Formula = 
                            "[SellTotal()]";
                        label14.Visible = false;
                        label17.Visible = false;
                        cmbOverride.Visible = false;
                        cmbOverride.SelectedIndex = -1;
                        break;
                    default:
                        grid.DisplayLayout.Bands[0].Summaries["OverrideAvg"].Formula = 
                            string.Empty;
                        break;
                }
               
                RecalculatePricing();
            }
        }
    }
}
