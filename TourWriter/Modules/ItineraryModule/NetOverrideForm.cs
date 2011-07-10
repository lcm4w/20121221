﻿using System;
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

            // check ComOrMup value
            if (itinerarySet.Itinerary[0].NetComOrMup == "com")
            {
                rdCommission.Checked = true;                
            }
            else
            {
                rdMarkup.Checked = true;                
            }

            LoadMarginSelection();
            DisplayMarginSelection(itinerarySet.Itinerary[0].NetMinOrMax);

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
            cbMarginForMarkup.SelectedIndexChanged -= cbMarginForMarkup_SelectedIndexChanged;
            cbMarginForMarkup.DataSource = new ArrayList {new { Text = "Set minimum", Value = "min" }, 
                                                          new { Text = "Set maximum", Value = "max" },
                                                          new { Text = "Set exact", Value = "exact" }};            
            cbMarginForMarkup.SelectedIndexChanged += cbMarginForMarkup_SelectedIndexChanged;

            cbMarginForCommission.SelectedIndexChanged -= cbMarginForCommission_SelectedIndexChanged;
            cbMarginForCommission.DataSource = new ArrayList {new { Text = "Set minimum", Value = "min" }, 
                                                              new { Text = "Set maximum", Value = "max" },
                                                              new { Text = "Set exact", Value = "exact" }};            
            cbMarginForCommission.SelectedIndexChanged += cbMarginForCommission_SelectedIndexChanged;
        }

        private void DisplayMarginSelection(string marginOverride)
        {            
            cbMarginForMarkup.Enabled = rdMarkup.Checked;
            cbMarginForCommission.Enabled = rdCommission.Checked;
            cbMarginForMarkup.SelectedValue = marginOverride ?? "exact";
            cbMarginForCommission.SelectedValue = marginOverride ?? "exact";            
        }

        private void UpdateMarginOverrideSelectionMessage()
        {
            if (rdMarkup.Checked && cbMarginForMarkup.SelectedIndex >= 0)
                label3.Text = App.GetMarginOverrideSelectionMessage("mup", (string)cbMarginForMarkup.SelectedValue);
            else if (rdCommission.Checked && cbMarginForCommission.SelectedIndex >= 0)
                label3.Text = App.GetMarginOverrideSelectionMessage("com", (string)cbMarginForCommission.SelectedValue);
            else
                label3.Text = string.Empty;
        }

        private void SaveChanges()
        {
            itinerarySet.Itinerary[0].NetComOrMup = (rdMarkup.Checked) ? "mup" : "com";
            itinerarySet.Itinerary[0].NetMinOrMax = marginMinOrMax;

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
                    (decimal?)row.Cells["Override"].Value : null;

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
                if (App.AskYesNo("Remove existing overrides?"))
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
                    if (rdCommission.Checked)
                        sell = Common.CalcGrossByNetCommission(net, (decimal)ovrride);
                    else
                        sell = Common.CalcGrossByNetMarkup(net, (decimal)ovrride);
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

        private void rdMarkup_CheckedChanged(object sender, EventArgs e)
        {
            grid.DisplayLayout.Bands[0].Columns["Markup"].Hidden = !rdMarkup.Checked;

            // change formula to calculate average markup
            RecalculatePricing();
            grid.DisplayLayout.Bands[0].Summaries["OverrideAvg"].Formula =
                (rdMarkup.Checked) ? "(([SellTotal()] - [NetBaseTotal()])/[NetBaseTotal()]) * 100" : String.Empty;

            DisplayMarginSelection(null);
            UpdateMarginOverrideSelectionMessage();
        }

        private void rdCommission_CheckedChanged(object sender, EventArgs e)
        {
            grid.DisplayLayout.Bands[0].Columns["Commission"].Hidden = !rdCommission.Checked;

            // change formula to calculate average commission
            RecalculatePricing();
            grid.DisplayLayout.Bands[0].Summaries["OverrideAvg"].Formula =
                (rdCommission.Checked) ? "(([SellTotal()] - [NetBaseTotal()])/[SellTotal()]) * 100" : String.Empty;

            DisplayMarginSelection(null);
            UpdateMarginOverrideSelectionMessage();
        }

        private void cbMarginForMarkup_SelectedIndexChanged(object sender, EventArgs e)
        {
            marginMinOrMax = (string)cbMarginForMarkup.SelectedValue;
            UpdateMarginOverrideSelectionMessage();
        }

        private void cbMarginForCommission_SelectedIndexChanged(object sender, EventArgs e)
        {
            marginMinOrMax = (string)cbMarginForCommission.SelectedValue;
            UpdateMarginOverrideSelectionMessage();
        }
    }
}
