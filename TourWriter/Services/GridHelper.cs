using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;

namespace TourWriter.Services
{
    internal class GridHelper
    {
        /// <summary>
        /// Set the default configurations for data grid
        /// </summary>
        /// <param name="e"></param>
        /// <param name="autoFitColumns">Auto fit the columns to the grid width</param>
        /// <param name="colHeadersVisible">Show or hide the column headers</param>
        /// <param name="allowEditing">Allow column editing</param>
        internal static void Configure_OLD(InitializeLayoutEventArgs e,
                                       bool autoFitColumns, bool colHeadersVisible, bool allowEditing)
        {
            // default grid configuration

            // Set the ScrollBounds to ScrollToFill so that the UltraGrid stops scrolling
            // once the last visible row is fully visible. By default UltraGrid will
            // continue scrolling until only a single row is visible.
            e.Layout.Scrollbars = Scrollbars.Automatic;
            e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
            e.Layout.ScrollStyle = ScrollStyle.Immediate;

            foreach (UltraGridBand b in e.Layout.Bands)
                b.ColHeadersVisible = colHeadersVisible;

            e.Layout.AutoFitStyle = (autoFitColumns) ? AutoFitStyle.ResizeAllColumns : AutoFitStyle.None;
            e.Layout.CaptionAppearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.CaptionAppearance.ForeColor = SystemColors.ControlDarkDark;
            e.Layout.CaptionAppearance.TextHAlign = HAlign.Left;

            e.Layout.Appearance.BorderColor = SystemColors.InactiveCaption;
            e.Layout.Appearance.BackColor = SystemColors.Window;
            e.Layout.ViewStyle = ViewStyle.SingleBand;
            e.Layout.BorderStyle = UIElementBorderStyle.Solid;

            e.Layout.Override.BorderStyleCell = UIElementBorderStyle.None;
            e.Layout.Override.BorderStyleRow = UIElementBorderStyle.Dotted;
            e.Layout.Override.RowSelectors = DefaultableBoolean.False;
            e.Layout.Override.RowAppearance.BackColor = Color.White;
            e.Layout.Override.RowAppearance.BorderColor = Color.White;
            e.Layout.Override.RowSizing = RowSizing.Fixed;
            e.Layout.Override.BorderStyleRow = UIElementBorderStyle.None;

            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            //			e.Layout.Override.RowAlternateAppearance.BackColor = Color.LightYellow;
            //			e.Layout.Override.HeaderAppearance.ThemedElementAlpha = Alpha.Transparent;
            //			e.Layout.Override.HeaderAppearance.BackColor = Color.LightYellow;
            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            if (allowEditing)
            {
                // configure cell appearance
                e.Layout.Override.SelectedCellAppearance.BackColor = Color.White;
                e.Layout.Override.SelectedCellAppearance.ForeColor = Color.Black;
                e.Layout.Override.ActiveCellAppearance.BackColor = Color.LemonChiffon;
                e.Layout.Override.ActiveCellAppearance.ForeColor = Color.DarkBlue;
                e.Layout.Override.CellClickAction = CellClickAction.Edit;

                // to get the look of full row select		
                e.Layout.Override.ActiveRowAppearance.BackColor = Color.LemonChiffon;
                e.Layout.Override.ActiveRowAppearance.BorderColor = Color.Gray;
            }
            else
            {
                // configure row appearance
                e.Layout.Override.SelectedRowAppearance.BorderColor = Color.Gray;
                e.Layout.Override.SelectedRowAppearance.BackColor = Color.White;
                e.Layout.Override.SelectedRowAppearance.ForeColor = Color.Black;
                e.Layout.Override.ActiveRowAppearance.BackColor = Color.LemonChiffon;
                e.Layout.Override.ActiveRowAppearance.ForeColor = Color.DarkBlue;
                e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
            }
        }

        internal static void SetDefaultCellAppearance(UltraGridColumn column)
        {
            if (column.DataType == typeof(Int32) ||
                column.DataType == typeof(Decimal) ||
                column.DataType == typeof(DateTime))
            {
                column.CellAppearance.TextHAlign = HAlign.Right;
            }
        }

        /// <summary>
        /// Sets the default visual appearance of a combo box.
        /// </summary>
        /// <param name="e">The combos InitializeLayoutEventArgs parameter.</param>
        /// <param name="colHeadersVisible">Weather to show column headers.</param>
        internal static void SetDefaultComboAppearance(InitializeLayoutEventArgs e, bool colHeadersVisible)
        {
            e.Layout.ViewStyle = ViewStyle.SingleBand;
            e.Layout.Bands[0].ColHeadersVisible = colHeadersVisible;
            e.Layout.AutoFitStyle = AutoFitStyle.None;
            e.Layout.ScrollStyle = ScrollStyle.Immediate;
            e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

            e.Layout.BorderStyle = UIElementBorderStyle.Solid;
            e.Layout.Appearance.BorderColor = SystemColors.InactiveCaption;

            e.Layout.Override.BorderStyleRow = UIElementBorderStyle.Dotted;
            e.Layout.Override.RowAppearance.BorderColor = Color.Transparent;
        }

        /// <summary>
        /// Sets the default visual appearance of a grid.
        /// </summary>
        /// <param name="e">The grids InitializeLayoutEventArgs parameter.</param>
        internal static void SetDefaultGridAppearance(InitializeLayoutEventArgs e)
        {
            #region Appearance
            e.Layout.Appearance.BackColor = SystemColors.Window;
            e.Layout.BorderStyle = UIElementBorderStyle.Solid;
            e.Layout.Appearance.BorderColor = SystemColors.InactiveCaption;
            e.Layout.Override.BorderStyleCell = UIElementBorderStyle.Dotted;
            e.Layout.Override.BorderStyleRow = UIElementBorderStyle.Dotted;
            e.Layout.Override.CellAppearance.BorderColor = Color.Silver;
            e.Layout.Override.CellAppearance.BackColor = SystemColors.Window;
            e.Layout.Override.RowAppearance.BorderColor = Color.Silver;
            e.Layout.Override.RowAppearance.BackColor = SystemColors.Window;
            e.Layout.CaptionVisible = DefaultableBoolean.False;
            #endregion

            #region Behaviour

            // Can't select GroupByRows.
            //e.Layout.Override.SelectTypeGroupByRow = SelectType.None;

            e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
            e.Layout.Override.SelectTypeRow = SelectType.None;
            e.Layout.ViewStyle = ViewStyle.SingleBand;
            e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            e.Layout.Override.RowSelectors = DefaultableBoolean.True;
            e.Layout.Override.RowSizing = RowSizing.AutoFree;
            e.Layout.EmptyRowSettings.ShowEmptyRows = true;
            e.Layout.GroupByBox.Hidden = true;

            #endregion

            #region Rows

            e.Layout.Override.RowSelectors = DefaultableBoolean.Default;
            e.Layout.Override.RowSizing = RowSizing.AutoFixed;

            e.Layout.Override.ActiveRowAppearance.BackColor = SystemColors.Highlight;
            e.Layout.Override.ActiveRowAppearance.ForeColor = SystemColors.HighlightText;

            e.Layout.Override.SelectedRowAppearance.BackColor = SystemColors.Highlight;
            e.Layout.Override.SelectedRowAppearance.ForeColor = SystemColors.HighlightText;

            e.Layout.Override.RowSpacingBefore = 1;

            #endregion

            #region Cells

            e.Layout.Override.CellAppearance.BorderColor = Color.Silver;
            e.Layout.Override.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;

            e.Layout.Override.ActiveCellAppearance.ForeColor = Color.Black;
            e.Layout.Override.ActiveCellAppearance.BackColor = Color.Azure;

            #endregion

            #region Caption

            // Grid caption
            e.Layout.CaptionVisible = DefaultableBoolean.False;
            //e.Layout.CaptionAppearance.TextHAlign = HAlign.Left;
            //e.Layout.CaptionAppearance.FontData.Bold = DefaultableBoolean.True;
            //e.Layout.CaptionAppearance.ForeColor = System.Drawing.SystemColors.GrayText;
            //e.Layout.CaptionAppearance.BackColor = System.Drawing.SystemColors.ControlLightLight;
            //e.Layout.CaptionAppearance.BackColor2 = System.Drawing.SystemColors.ActiveBorder;
            //e.Layout.CaptionAppearance.BackGradientStyle = GradientStyle.Vertical;
            //e.Layout.CaptionAppearance.ThemedElementAlpha = Alpha.Transparent;

            #endregion

            #region Colum headers

            e.Layout.Override.HeaderAppearance.TextHAlign = HAlign.Center;
            e.Layout.Override.HeaderAppearance.BackColor = SystemColors.ControlLightLight;
            e.Layout.Override.HeaderAppearance.BackColor2 = SystemColors.ActiveBorder;
            e.Layout.Override.HeaderAppearance.BackGradientStyle = GradientStyle.VerticalBump;
            e.Layout.Override.HeaderAppearance.ThemedElementAlpha = Alpha.Transparent;
            e.Layout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
            e.Layout.Override.HeaderStyle = HeaderStyle.WindowsXPCommand;
            e.Layout.Override.HeaderPlacement = HeaderPlacement.FixedOnTop;

            #endregion

            #region Add-row

            //// Add-Row Feature Related Settings
            //// --------------------------------------------------------------------------------
            //// To enable the add-row functionality set the AllowAddNew. If you set the property 
            //// to FixedAddRowOnTop or FixedAddRowOnBottom then the add-row will be fixed in the
            //// root band. It won't scroll out of view as rows are scrolled.
            ////
            //e.Layout.Override.AllowAddNew = AllowAddNew.FixedAddRowOnTop;
            //e.Layout.Override.AllowDelete = DefaultableBoolean.True;

            //// Set the appearance for template add-rows. Template add-rows are the 
            //// add-row templates that are displayed with each rows collection.
            ////
            //e.Layout.Override.TemplateAddRowAppearance.BackColor = Color.FromArgb(245, 250, 255);
            //e.Layout.Override.TemplateAddRowAppearance.ForeColor = SystemColors.Control;

            //// Once  the user modifies the contents of a template add-row, it becomes
            //// an add-row and the AddRowAppearance gets applied to such rows.
            ////
            //e.Layout.Override.AddRowAppearance.BackColor = Color.LightYellow;
            //e.Layout.Override.AddRowAppearance.ForeColor = Color.Blue;

            //// You can set the SpecialRowSeparator to a value with TemplateAddRow flag
            //// turned on to display a separator ui element after the add-row. By default
            //// UltraGrid displays a separator element if AllowAddNew is either
            //// FixedAddRowOnTop or FixedAddRowOnBottom. For scrolling add-rows you have to
            //// set the SpecialRowSeparator explicitly. You can also control the appearance
            //// of the separator using the SpecialRowSeparatorAppearance proeprty.
            ////
            //e.Layout.Override.SpecialRowSeparator = SpecialRowSeparator.TemplateAddRow;
            //e.Layout.Override.SpecialRowSeparatorAppearance.BackColor = SystemColors.Control;

            //// You can display a prompt in the add-row by setting the TemplateAddRowPrompt 
            //// proeprty. By default UltraGrid does not display any add-row prompt.
            ////
            //e.Layout.Override.TemplateAddRowPrompt = "Click here to add a new record...";

            //// You can control the appearance of the prompt using the Override's
            //// TemplateAddRowPromptAppearance property. By default the prompt is
            //// transparent. You can make it non-transparent by setting the appearance'
            //// BackColorAlpha property or by setting the BackColor to a desired value.
            ////
            //e.Layout.Override.TemplateAddRowPromptAppearance.ForeColor = SystemColors.GrayText;
            ////e.Layout.Override.TemplateAddRowPromptAppearance.FontData.Bold = DefaultableBoolean.True;

            //// By default the prompt is displayed across multiple cells. You can confine
            //// the prompt a particular cell by setting the SpecialRowPromptField property
            //// of the band to the key of the column that you want to display the prompt in.
            ////
            ////e.Layout.Bands[0].SpecialRowPromptField = e.Layout.Bands[0].Columns[0].Key;

            //// You can set the default value of an add-row field by using the DefaultCellValue 
            //// property of the column. Also you can initialize a template add-row to dynamic
            //// default values using the InitializeTemplateAddRow event of the UltraGrid.
            ////e.Layout.Bands[0].Columns[0].DefaultCellValue = "(DefaultValue)";
            //// --------------------------------------------------------------------------------

            //// Other miscellaneous settings
            //// --------------------------------------------------------------------------------
            //// Set the scroll style to immediate so the rows get scrolled immediately
            //// when the vertical scrollbar thumb is dragged.
            ////
            //e.Layout.ScrollStyle = ScrollStyle.Immediate;

            //// ScrollBounds of ScrollToFill will prevent the user from scrolling the
            //// grid further down once the last row becomes fully visible.
            ////
            //e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;

            #endregion
        }

        /// <summary>
        /// Sets the default group appearance of a grid.
        /// </summary>
        /// <param name="e">The grids InitializeLayoutEventArgs parameter.</param>
        internal static void SetDefaultGroupByAppearance(InitializeLayoutEventArgs e)
        {
            e.Layout.Override.GroupByRowAppearance.ForeColor = Color.DimGray;
            e.Layout.Override.GroupByRowAppearance.BackColor = SystemColors.Control;
            e.Layout.Override.GroupByRowAppearance.BackColor2 = SystemColors.Control;

            //e.Layout.Override.GroupByRowAppearance.ForeColor = Color.DimGray;
            //e.Layout.Override.GroupByRowAppearance.FontData.Bold = DefaultableBoolean.True;
            //e.Layout.Override.GroupByRowAppearance.FontData.SizeInPoints = 10;
            //e.Layout.Override.GroupByRowAppearance.BackColor = Color.Gainsboro;
            //e.Layout.Override.GroupByRowAppearance.BackColor2 = Color.White;
            //e.Layout.Override.HeaderAppearance.BackGradientStyle = GradientStyle.Vertical;
            //e.Layout.Override.GroupByRowAppearance.ThemedElementAlpha = Alpha.Transparent;
        }

        /// <summary>
        /// Sets the default summary appearance of a grid.
        /// </summary>
        /// <param name="e">The grids InitializeLayoutEventArgs parameter.</param>
        internal static void SetDefaultSummaryAppearance(InitializeLayoutEventArgs e)
        {
            //e.Layout.Override.SummaryValueAppearance.FontData.SizeInPoints = 9;
            //e.Layout.Override.SummaryValueAppearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Override.SummaryValueAppearance.TextHAlign = HAlign.Right;
            e.Layout.Override.SummaryValueAppearance.ForeColor = Color.DimGray;

            e.Layout.Override.SummaryValueAppearance.BackColor = SystemColors.Info;
            e.Layout.Override.SummaryFooterAppearance.BackColor = SystemColors.Info;
        }

        /// <summary>
        /// Sets the active row in a grid, and puts a cell into edit mode.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="searchIdColumnName">The name of the column to search, for the index Id.</param>
        /// <param name="searchId">The index Id of the row that you want to activate.</param>
        /// <param name="editCellColumnName">The name of the column that you want to put into edit mode. Use an empty string for none.</param>
        internal static void SetActiveRow(UltraGrid grid, string searchIdColumnName,
                                          object searchId, string editCellColumnName)
        {
            if (searchId == null)
                return;

            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Cells[searchIdColumnName].Value.ToString() == searchId.ToString())
                {
                    grid.ActiveRow = row;
                    if (grid.ActiveRow != null && editCellColumnName != "")
                    {
                        grid.ActiveCell = grid.ActiveRow.Cells[editCellColumnName];
                        grid.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    }
                    break;
                }
            }
        }

        internal static string PrintColLayout(UltraGrid grid)
        {
            string s = "";
            foreach (UltraGridBand band in grid.DisplayLayout.Bands)
                s += PrintColLayout(grid, band);
            return s;
        }

        internal static string PrintColLayout(UltraGrid grid, UltraGridBand band)
        {
            string s = "Band, Column, VisiblePosition, Key, Caption \n";
            foreach (UltraGridColumn col in band.Columns)
                s +=
                    col.Band.Index + ", " +
                    col.Index + ", " +
                    col.Header.VisiblePosition + ", " +
                    col.Key + ", " +
                    col.Header.Caption + "\n";

            return s;
        }

        internal static void LayoutCol(UltraGrid grid, string col, int pos, int size)
        {
            grid.DisplayLayout.Bands[0].Columns[col].Header.VisiblePosition = pos;
            grid.DisplayLayout.Bands[0].Columns[col].Width = size;
        }

        internal static UltraGridRow GetValidClickRow(UltraGrid grid)
        {
            //Get the last element that the mouse entered
            UIElement lastElementEntered = grid.DisplayLayout.UIElement.LastElementEntered;
            if (lastElementEntered == null)
                return null;

            //See if there's a RowUIElement in the chain.
            RowUIElement rowElement;
            if (lastElementEntered is RowUIElement)
                rowElement = (RowUIElement)lastElementEntered;
            else
            {
                rowElement = (RowUIElement)lastElementEntered.GetAncestor(typeof(RowUIElement));
            }

            if (rowElement == null)
                return null;

            //Try to get a row from the element
            UltraGridRow row = (UltraGridRow)rowElement.GetContext(typeof(UltraGridRow));

            //If no row was returned, then the mouse is not over a row. 
            if (row == null)
                return null;

            //The mouse is over a row. 

            //This part is optional, but if the user double-clicks the line 
            //between Row selectors, the row is AutoSized by 
            //default. In that case, we probably don't want to do 
            //the double-click code.

            //Get the current mouse pointer location and convert it
            //to grid coords. 
            //System.Drawing.Point MousePosition = grid.PointToClient(Control.MousePosition);
            //See if the Point is on an AdjustableElement - meaning that
            //the user is clicking the line on the row selector
            if (lastElementEntered.AdjustableElementFromPoint(Control.MousePosition) != null)
                return null;

            //Everthing looks good, so display a message based on the row.
            //MessageBox.Show(this, "The key of the row is:" + row.Cells[0].Value.ToString()); 

            return row;
        }

        internal static UltraGridCell GetValidClickCell(UltraGrid grid)
        {
            //Get the last element that the mouse entered
            UIElement lastElementEntered = grid.DisplayLayout.UIElement.LastElementEntered;
            if (lastElementEntered == null)
                return null;

            //See if there's a CellUIElement in the chain.
            CellUIElement cellElement;
            if (lastElementEntered is CellUIElement)
                cellElement = (CellUIElement)lastElementEntered;
            else
            {
                cellElement = (CellUIElement)lastElementEntered.GetAncestor(typeof(CellUIElement));
            }

            if (cellElement == null)
                return null;

            //Try to get a row from the element
            UltraGridCell cell = (UltraGridCell)cellElement.GetContext(typeof(UltraGridCell));

            //If no row was returned, then the mouse is not over a row. 
            if (cell == null)
                return null;

            //The mouse is over a cell. 

            //This part is optional, but if the user double-clicks the line 
            //between Row selectors, the row is AutoSized by 
            //default. In that case, we probably don't want to do 
            //the double-click code.

            //Get the current mouse pointer location and convert it
            //to grid coords. 
            //System.Drawing.Point MousePosition = grid.PointToClient(Control.MousePosition);
            //See if the Point is on an AdjustableElement - meaning that
            //the user is clicking the line on the row selector
            if (lastElementEntered.AdjustableElementFromPoint(Control.MousePosition) != null)
                return null;

            //Everthing looks good, so display a message based on the row.
            //MessageBox.Show(this, "The key of the row is:" + row.Cells[0].Value.ToString()); 

            return cell;
        }

        internal static void DeleteActiveRow(UltraGrid grid, bool selectNextRow)
        {
            if (grid.ActiveRow == null)
                return;

            // Index of delete row.
            int index = grid.ActiveRow.Index;

            // Get the parent group by row.
            UltraGridGroupByRow parentGroupByRow = null;
            if (grid.ActiveRow.ParentRow != null && grid.ActiveRow.ParentRow.IsGroupByRow)
                parentGroupByRow = grid.ActiveRow.ParentRow as UltraGridGroupByRow;

            // Delete it.
            grid.ActiveRow.Delete(false);

            if (selectNextRow)
            {
                if (parentGroupByRow == null)
                {
                    // Set new index in ungrouped rows.
                    if ((index > grid.Rows.Count - 1 ? --index : index) > -1)
                        grid.ActiveRow = grid.Rows[index];
                }
                else
                {
                    // Set new index in grouped rows.
                    if ((index > parentGroupByRow.Rows.Count - 1 ? --index : index) > -1)
                        grid.ActiveRow = parentGroupByRow.Rows[index];
                }
            }
        }

        internal static void DeleteSelectedRows(UltraGrid grid, bool displayPrompt)
        {
            grid.DeleteSelectedRows(displayPrompt);
        }

        internal static bool HandleInvalidGridEdits(UltraGrid grid, bool forceExit)
        { // Handle an individual grid
            bool isValid = (grid.ActiveCell == null ||
                            !grid.ActiveCell.IsInEditMode ||
                            grid.ActiveCell.EditorResolved.IsValid);

            if (!isValid)
            {
                if (forceExit)
                {
                    grid.ActiveCell.CancelUpdate(); // reset
                    return true;
                }
                else
                {
                    grid.ActiveCell.EditorResolved.ExitEditMode(false, true); // fires warning messagebox
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates and populates a currency data dropdown that will map to a grid column. 
        /// Usage: 
        /// c.ValueList = GridHelper.GetEmbeddableCurrencyDropDown(sender as UltraGrid, c);
        /// </summary>
        /// <returns>Currency combo box to map to column.</returns>
        internal static UltraDropDown GetEmbeddableCurrencyDropDown(UltraGrid grid, UltraGridColumn column)
        {
            UltraDropDown dropDown = new UltraDropDown();
            column.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            #region Event handling

            dropDown.InitializeLayout +=
                delegate(object sender, InitializeLayoutEventArgs e)
                    {
                        SetCurrencyComboAppearance(e);

                        // Add a summary row as a way of showing an extra row 
                        // that can pose as a null or <default> value row.
                        SummarySettings summary;
                        UltraGridBand band = e.Layout.Bands[0];

                        summary = band.Summaries.Add(
                            "", SummaryPosition.UseSummaryPositionColumn, band.Columns["CurrencyCode"]);
                        summary.DisplayFormat = " ";

                        summary = band.Summaries.Add(
                            "", SummaryPosition.UseSummaryPositionColumn, band.Columns["CurrencyName"]);
                        summary.DisplayFormat = "<default>";

                        e.Layout.Override.SummaryFooterCaptionVisible =
                            DefaultableBoolean.False;
                        e.Layout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;
                        e.Layout.Override.SummaryValueAppearance.BackColor =
                            SystemColors.Window;
                        e.Layout.Override.SummaryValueAppearance.BorderColor = Color.Silver;
                    };

            dropDown.BeforeDropDown +=
                delegate
                    {
                        // Check if opening value is null.
                        if (dropDown.SelectedRow == null)
                        {
                            // Make default row appear selected.
                            dropDown.DisplayLayout.Override.SummaryValueAppearance.ForeColor = 
                                SystemColors.HighlightText;
                            dropDown.DisplayLayout.Override.SummaryValueAppearance.BackColor =
                                SystemColors.Highlight;
                        }
                    };

            dropDown.RowSelected +=
                delegate
                    {
                        // Make default row appear unselected.
                        dropDown.DisplayLayout.Override.SummaryValueAppearance.ForeColor =
                            SystemColors.WindowText;
                        dropDown.DisplayLayout.Override.SummaryValueAppearance.BackColor =
                            SystemColors.Window;
                    };

            dropDown.MouseDown +=
                delegate
                    {
                        // Check if clicked default row (which is actually the summary area).
                        UIElement el = dropDown.DisplayLayout.UIElement.LastElementEntered;
                        if (el != null && el.GetAncestor(typeof (SummaryValueUIElement)) != null)
                        {
                            // Make default row appear selected.
                            dropDown.SelectedRow = null;
                            dropDown.DisplayLayout.Override.SummaryValueAppearance.ForeColor =
                                SystemColors.HighlightText;
                            dropDown.DisplayLayout.Override.SummaryValueAppearance.BackColor =
                                SystemColors.Highlight;
                        }
                    };

            dropDown.MouseUp +=
                delegate
                    {
                        // Need to manually close dropdown after default row was selected.
                        UIElement el = dropDown.DisplayLayout.UIElement.LastElementEntered;
                        if (el != null && el.GetAncestor(typeof (SummaryValueUIElement)) != null)
                        {
                            grid.ActiveCell.DroppedDown = false;
                        }
                    };

            #endregion

            dropDown.ValueMember = "CurrencyCode";
            dropDown.DisplayMember = "CurrencyCode";
            dropDown.DataSource = Cache.ToolSet.Currency;
            return dropDown;
        }
        
        internal static void SetCurrencyComboAppearance(InitializeLayoutEventArgs e)
        {
            foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
            {
                if (c.Key == "CurrencyCode")
                {
                    c.Header.Caption = "Code";
                    c.Width = 40;
                    c.SortIndicator = SortIndicator.Ascending;
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                }
                else if (c.Key == "CurrencyName")
                {
                    c.Header.Caption = "Name";
                    c.Width = 150;
                    c.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
                }
                else c.Hidden = true;
            }
            SetDefaultComboAppearance(e, true);
        }


        /// <summary>
        /// Gets the data rows from a grid, excluding group-by rows
        /// </summary>
        /// <param name="grid">The grid</param>
        /// <param name="keepDataTypes">Whether or not to keep the column data types</param>
        /// <returns>Table of data from grid rows</returns>
        internal static DataTable GetDataRowsTable(UltraGrid grid, bool keepDataTypes)
        {
            return ConvertToDataTable(GetDataRowsList(grid), keepDataTypes);
        }

        internal static DataTable GetDataRowsTable(UltraGrid grid)
        {
            return ConvertToDataTable(GetDataRowsList(grid));
        }

        /// <summary>
        /// Gets the data rows from a grid, excluding group-by rows
        /// </summary>
        /// <param name="grid">The grid</param>
        /// <returns>List of grid rows</returns>
        internal static List<UltraGridRow> GetDataRowsList(UltraGrid grid)
        {
            List<UltraGridRow> list = new List<UltraGridRow>();
            AddDataRowsToList(grid.Rows, list);
            return list;
        }

        /// <summary>
        /// Gets the data rows from a row collection, excluding group-by rows
        /// </summary>
        /// <param name="rows">The rows collection</param>
        /// <param name="list">The list to add rows to</param>
        private static void AddDataRowsToList(RowsCollection rows, List<UltraGridRow> list)
        {
            if (list == null)
                list = new List<UltraGridRow>();

            foreach(UltraGridRow row in rows)
            {
                if (row.IsGroupByRow)
                {
                    AddDataRowsToList(row.ChildBands[0].Rows, list);
                }
                else
                {
                    list.Add(row);
                }
            }
        }

        /// <summary>
        /// Converts a list of UltraGridRows to a DataTable
        /// </summary>
        /// <param name="gridRows">List of UltraGridRows</param>
        /// <param name="keepDataTypes">Whether or not to keep the column data types</param>
        /// <returns>A DataTable</returns>
        private static DataTable ConvertToDataTable(IList<UltraGridRow> gridRows, bool keepDataTypes)
        {
            DataTable table = new DataTable();
            if(gridRows.Count == 0)
                return table;
            
            // add table columns
            foreach(UltraGridCell cell in gridRows[0].Cells)
            {
                if (keepDataTypes)
                    table.Columns.Add(cell.Column.Key, cell.Column.DataType);
                else
                    table.Columns.Add(cell.Column.Key);
            }

            // add table rows
            foreach (UltraGridRow gridRow in gridRows)
            {
                DataRow row = table.NewRow();
                foreach (UltraGridCell cell in gridRow.Cells)
                {
                    row[cell.Column.Key] = cell.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private static DataTable ConvertToDataTable(IList<UltraGridRow> gridRows)
        {
            return ConvertToDataTable(gridRows, false);
        }
    }
}
