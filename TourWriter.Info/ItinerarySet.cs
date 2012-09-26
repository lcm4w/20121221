using System;
using System.Collections.Generic;
using System.Data;
using TourWriter.Info.Services;
using System.Linq;

namespace TourWriter.Info
{
    public partial class ItinerarySet
    {
        partial class ItineraryPaxDataTable
        {
        }
        #region ItinerarySet

        public bool HasAccountingChanges()
        {
            ItinerarySet changes = (ItinerarySet)GetChanges();

            // only concerned with certain edits
            if (changes != null)
            {
                return
                    changes.PurchaseItem.Count > 0 || changes.PurchaseLine.Count > 0 ||
                    changes.ItineraryPayment.Count > 0 || changes.ItinerarySale.Count > 0 ||
                    changes.ItinerarySaleAllocation.Count > 0 || changes.ItineraryMarginOverride.Count > 0 ||
                    changes.Itinerary.Count > 0;
            }

            return false;
        }

        public ItinerarySet CopyWithEvents()
        {
            // Create new itinerarySet so events are registered.
            ItinerarySet newItinerarySet = new ItinerarySet();

            // Copy the data.
            newItinerarySet.Merge(Copy(), false);

            return newItinerarySet;
        }

        /// <summary>
        /// Number of days of this itinerary.
        /// </summary>
        /// <returns>The number of days.</returns>
        public int ItineraryLength()
        {
            if (!Itinerary[0].IsArriveDateNull() && !Itinerary[0].IsDepartDateNull())
            {
                DateTime arrive = Itinerary[0].ArriveDate.Date;
                DateTime depart = Itinerary[0].DepartDate.Date;

                return ((depart - arrive).Days + 1);
            }
            return 0;
        }

        /// <summary>
        /// Gets the sum of the booking net prices, excluding margin/final overrides.
        /// </summary>
        /// <returns></returns>
        public decimal GetNetBasePrice()
        {
            decimal total = 0;
            foreach (PurchaseItemRow item in PurchaseItem)
            {
                if (item.RowState != DataRowState.Deleted)
                    total += item.NetTotalConverted;
            }
            total = decimal.Round(total, 4, MidpointRounding.AwayFromZero); // to match database calcs
            return total;
        }

        /// <summary>
        /// Gets the sum of the booking gross prices, excluding margin/final overrides.
        /// </summary>
        /// <returns></returns>
        public decimal GetGrossBasePrice()
        {
            decimal total = 0;
            foreach (PurchaseItemRow item in PurchaseItem)
            {
                if (item.RowState != DataRowState.Deleted)
                    total += item.GrossTotalConverted;
            }
            total = decimal.Round(total, 4, MidpointRounding.AwayFromZero); // to match database calcs
            return total;
        }

        /// <summary>
        /// Gets the net markup, or an average of the service type markups
        /// </summary>
        public decimal GetMarginOverride()
        {
            var isOverrideAll = !Itinerary[0].IsNetMarginNull();

            // bulk override all PurchaseItems
            if (isOverrideAll)
            {
                if (!Itinerary[0].IsNetComOrMupNull() && Itinerary[0].NetComOrMup == "mup")
                {
                    return Itinerary[0].NetMargin;
                }
                else if (!Itinerary[0].IsNetComOrMupNull() && Itinerary[0].NetComOrMup == "com")
                {
                    // convert commission to markup
                    decimal comm = (Itinerary[0].NetMargin != 100) ? Itinerary[0].NetMargin : 99.99m;
                    return (((100 / (100 - comm)) - 1) * 100);
                }
                else // NetComOrMup == "grs"
                {
                    return Itinerary[0].NetMargin;
                }
            }
            // individual override per PurchaseItem
            else 
            {
                decimal totalGross = 0;

                foreach (PurchaseItemRow purchaseItem in PurchaseItem.Rows)
                {
                    if (purchaseItem.RowState == DataRowState.Deleted) continue;

                    var itinMargin = ItineraryMarginOverride.FindByItineraryIDServiceTypeID(
                        Itinerary[0].ItineraryID, purchaseItem.ServiceTypeID);

                    if (itinMargin != null)
                        totalGross += Itinerary[0].RecalculateGross(purchaseItem.NetTotalConverted, purchaseItem.GrossTotalConverted, itinMargin.Margin);
                    else
                        totalGross += purchaseItem.GrossTotalConverted;
                }
                var totalNet = GetNetBasePrice();
                var markup = GetMarkup(totalNet, totalGross);
                return markup;
            }
        }

        /// <summary>
        /// Gets the final gross price, including final markup or price overrides.
        /// </summary>
        /// <returns></returns>
        public decimal GetGrossFinalPrice()
        {
            decimal total;
            var hasFinalOverride = !Itinerary[0].IsGrossOverrideNull();

            if (hasFinalOverride)
            {
                total = Itinerary[0].GrossOverride;
            }
            else
            {
                // special case when override-all-servicetypes and 'grs' (discount)
                var hasDefaultServiceOverride = !Itinerary[0].IsNetMarginNull();
                if (hasDefaultServiceOverride && !Itinerary[0].IsNetComOrMupNull() && Itinerary[0].NetComOrMup == "grs")
                {
                    var gross = GetGrossBasePrice();
                    var margin = GetMarginOverride();
                    total = Common.CalcGrossByGrossCommission(gross, margin);
                }
                else
                {
                    var net = GetNetBasePrice();
                    var margin = GetMarginOverride();
                    total = (net + margin == 0)
                                ? GetGrossBasePrice()
                                : Common.CalcGrossByNetMarkup(net, margin);
                }
                
                // apply final itinerary overrde
                if (!Itinerary[0].IsGrossMarkupNull())
                    total *= (1 + Itinerary[0].GrossMarkup / 100);
            }
            total = decimal.Round(total, 4, MidpointRounding.AwayFromZero); // to match database calcs
            return total;
        }

        public decimal GetMarkup(decimal nett, decimal gross)
        {
            return Services.Common.CalcMarkupByNetGross(nett, gross);
        }

        public decimal GetCommission(decimal nett, decimal gross)
        {
            return Services.Common.CalcCommissionByNetGross(nett, gross);
        }

        public override void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler, params DataTable[] tables)
        {
            base.Load(reader, loadOption, errorHandler, tables);

            // Necessary so that OnTableNewRow is called!
            foreach (DataTable table in Tables)
                table.TableNewRow += delegate { };
        }

        #endregion

        #region Itinerary

        public partial class ItineraryDataTable
        {
            public override void BeginInit()
            {
                ColumnChanging += ValidateColumn;
            }

            void ValidateColumn(object sender, DataColumnChangeEventArgs e)
            {
                string errorMsg = "";

                if (e.Column == ItineraryNameColumn)
                {
                    if (string.IsNullOrEmpty(e.ProposedValue.ToString()))
                    {
                        errorMsg = "Itinerary name is required";
                    }
                }
                else if (e.Column == ArriveDateColumn)
                {
                    if (e.ProposedValue == DBNull.Value)
                    {
                        errorMsg = "Arrival date is required";
                    }
                    else if (
                        e.ProposedValue != DBNull.Value &&
                        e.Row[DepartDateColumn] != DBNull.Value &&
                        (DateTime)e.ProposedValue > (DateTime)e.Row[DepartDateColumn])
                    {
                        errorMsg = "Arrival date cannot be after departure date";
                    }
                }
                else if (e.Column == DepartDateColumn)
                {
                    if (e.ProposedValue != DBNull.Value &&
                        e.Row[ArriveDateColumn] != DBNull.Value &&
                        (DateTime)e.ProposedValue < (DateTime)e.Row[ArriveDateColumn])
                    {
                        errorMsg = "Departure date cannot be before arrival date";
                    }
                }

                // Set or reset error message
                e.Row.SetColumnError(e.Column, errorMsg);
            }
        }

        public partial class ItineraryRow
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public string GetDisplayNameOrItineraryName()
            {
                return (IsDisplayNameNull() || DisplayName == String.Empty) ? ItineraryName : DisplayName;
            }

            /// <summary>
            /// Tests if any itinerary price overrides exist.
            /// </summary>
            /// <returns></returns>
            public bool HasPriceOverrides
            {
                get
                {
                    return !(IsNetMarginNull()
                             && IsGrossMarkupNull()
                             && IsGrossOverrideNull());
                }
            }

            public decimal RecalculateGross(decimal netFinal, decimal grossFinal, decimal margin)
            {
                decimal totalGross = 0M;

                // commission
                if (!IsNetComOrMupNull() && NetComOrMup == "com")
                {                    
                    var commission = ((ItinerarySet)this.tableItinerary.DataSet).GetCommission(netFinal, grossFinal);
                    if (NetMinOrMax == "min") // ensure minimum, so give us the biggest value
                        margin = Math.Max(commission, margin);
                    else if (NetMinOrMax == "max") // ensure maximum, so give us the smallest value
                        margin = Math.Min(commission, margin);

                    totalGross = Common.CalcGrossByNetCommission(netFinal, margin);
                }
                // markup
                else if (!IsNetComOrMupNull() && NetComOrMup == "mup")
                {
                    var markup = ((ItinerarySet)this.tableItinerary.DataSet).GetMarkup(netFinal, grossFinal);
                    if (NetMinOrMax == "min") // ensure minimum, so give us the biggest value
                        margin = Math.Max(markup, margin);
                    else if (NetMinOrMax == "max") // ensure maximum, so give us the smallest value
                        margin = Math.Min(markup, margin);

                    totalGross = Common.CalcGrossByNetMarkup(netFinal, margin);
                }
                // discount on gross
                else // if (!IsNetComOrMupNull() && NetComOrMup == "grs")
                {
                    totalGross = Common.CalcGrossByGrossCommission(grossFinal, margin);
                }

                return totalGross;
            }
        }

        #endregion

        #region PurchaseLine

        partial class PurchaseLineDataTable
        {
            public PurchaseLineRow Add(int itineraryId, int supplierId, string purchaseLineName, int userId)
            {
                PurchaseLineRow line;
                line = NewPurchaseLineRow();
                line.ItineraryID = itineraryId;
                line.SupplierID = supplierId;
                line.PurchaseLineName = purchaseLineName;
                line.AddedBy = userId;
                line.AddedOn = DateTime.Now;

                AddPurchaseLineRow(line);
                return line;
            }
        }

        partial class PurchaseLineRow
        {
            public string SupplierName
            {
                get
                {
                    var l = ((ItinerarySet)Table.DataSet).SupplierLookup.FindBySupplierID(SupplierID);
                    return (l != null) ? l.SupplierName : "";
                }
            }
        }

        #endregion

        #region PurchaseItem

        partial class PurchaseItemDataTable
        {
            public override void EndInit()
            {
                base.EndInit();
                RegisterEvents();
            }

            private void RegisterEvents()
            {
                RowChanged += RowChangedEvent;
                ColumnChanged += ColumnChangedEvent;
            }

            private void RemoveEvents()
            {
                RowChanged -= RowChangedEvent;
                ColumnChanged -= ColumnChangedEvent;

            }

            private static void RowChangedEvent(object sender, DataRowChangeEventArgs e)
            {
                if (e.Action == DataRowAction.Add && e.Row.RowState == DataRowState.Added)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format("Called:  PurchaseItemDataTable.RowChangedEvent(), {0}, {1}, call RecalculateTotals()...",
                        e.Action, e.Row.RowState));

                    PurchaseItemRow purchaseItem = e.Row as PurchaseItemRow;
                    if (purchaseItem != null)
                        purchaseItem.RecalculateTotals();
                }
            }

            private void ColumnChangedEvent(object sender, DataColumnChangeEventArgs e)
            {
                if (e.Column == NetUnitColumn ||
                    e.Column == GrossUnitColumn ||
                    e.Column == QuantityColumn ||
                    e.Column == NumberOfDaysColumn ||
                    e.Column == DiscountUnitsColumn)
                {
                    PurchaseItemRow purchaseItem = e.Row as PurchaseItemRow;
                    if (purchaseItem != null)
                        purchaseItem.RecalculateTotals();
                }
            }

            public PurchaseItemRow ChangePurchaseItemOption(PurchaseItemRow item, SupplierSet.ServiceRow service, SupplierSet.OptionRow option, ToolSet toolSet)
            {
                // update PurchaseItem data columns
                UpdateReadOnlyColumn(item.Table.Columns["OptionID"], item, option.OptionID);
                item.PurchaseItemName = service.ServiceName + ((option.OptionName != "") ? (", " + option.OptionName) : "");
                item.Net = option.Net;
                item.Gross = option.Gross;

                // update PurchaseItem lookup columns
                UpdateReadOnlyColumn(item.Table.Columns["RateID"], item, option.RateID);
                UpdateReadOnlyColumn(item.Table.Columns["ServiceID"], item, service.ServiceID);
                if (!option.IsOptionTypeIDNull())
                    UpdateReadOnlyColumn(item.Table.Columns["OptionTypeID"], item, option.OptionTypeID);
                else
                    UpdateReadOnlyColumn(item.Table.Columns["OptionTypeID"], item, DBNull.Value);
                UpdateReadOnlyColumn(item.Table.Columns["ServiceTypeID"], item, service.ServiceTypeID);
                UpdateReadOnlyColumn(item.Table.Columns["OptionName"], item, option.OptionName);
                UpdateReadOnlyColumn(item.Table.Columns["ServiceName"], item, service.ServiceName);
                var stype = toolSet.ServiceType.FindByServiceTypeID(service.ServiceTypeID);
                UpdateReadOnlyColumn(item.Table.Columns["ServiceTypeName"], item,
                    stype != null ? stype.ServiceTypeName : "");

                if (!option.IsOptionTypeIDNull())
                {
                    var otype = toolSet.OptionType.FindByOptionTypeID(option.OptionTypeID);
                    UpdateReadOnlyColumn(item.Table.Columns["OptionTypeName"], item,
                                         otype != null ? otype.OptionTypeName : "");
                } else UpdateReadOnlyColumn(item.Table.Columns["OptionTypeName"], item, "");

                item.ChargeType = !service.IsChargeTypeNull() ? service.ChargeType : "";
                item.IsDefaultOptionType = !option.IsIsDefaultNull() ? option.IsDefault : false;
                item.CurrencyCode = !service.IsCurrencyCodeNull() ? service.CurrencyCode : "";

                // TODO: do we need these lookup tables, once old reports are gone?
                // update ItinerarySet lookup tables
                var itin = item.Table.DataSet as ItinerarySet;
                if (itin != null) itin.AddLookupRows(service.Table.DataSet as SupplierSet, option.OptionID);

                return item;
            }

            private static void UpdateReadOnlyColumn(DataColumn col, DataRow row, object val)
            {
                var isReadOnly = col.ReadOnly;
                if (isReadOnly) col.ReadOnly = false;
                row[col] = val;
                if (isReadOnly) col.ReadOnly = true;
            }

            public PurchaseItemRow Add(
                SupplierSet lookupSupplierSet, int purchaseLineId, string purchaseItemName, double? qty, double? days, 
                decimal net, decimal gross, DateTime startDate, DateTime? startTime, DateTime? endTime, int userId, 
                int serviceTypeId, int optionId, int rateId, int serviceId, string serviceTypeName, int? optionTypeId,
                string serviceName, string optionName, string optionTypeName, string chargeType, bool isDefault, 
                string currencyCode)
            {
                // Add lookups first.
                ItinerarySet itinerarySet = DataSet as ItinerarySet;
                if (itinerarySet != null)
                    itinerarySet.AddLookupRows(lookupSupplierSet, optionId);
                
                // Add new row.
                PurchaseItemRow item = NewPurchaseItemRow();
                RemoveEvents();

                item.PurchaseLineID = purchaseLineId;
                item.OptionID = optionId;
                item.PurchaseItemName = purchaseItemName;
                item.StartDate = startDate;
                if (startTime.HasValue) item.StartTime = (DateTime)startTime;
                if (endTime.HasValue) item.EndTime = (DateTime)endTime;
                item.Net = decimal.Round(net, 2, MidpointRounding.AwayFromZero);
                item.Gross = decimal.Round(gross, 2, MidpointRounding.AwayFromZero);
                if (qty.HasValue) item.Quantity = (double)qty;
                if (days.HasValue) item.NumberOfDays = (double)days;
                item.AddedOn = DateTime.Now;
                item.AddedBy = userId;

                // add lookup cols
                item.RateID = rateId;
                item.ServiceID = serviceId;
                if (optionTypeId.HasValue) item.OptionTypeID = (int)optionTypeId;
                item.ServiceTypeID = serviceTypeId;
                item.OptionName = optionName;
                item.ServiceName = serviceName;
                item.ServiceTypeName = serviceTypeName;
                item.OptionTypeName = optionTypeName;
                item.ChargeType = chargeType;
                item.IsDefaultOptionType = isDefault;
                item.CurrencyCode = currencyCode;
                // discounts
                // NO: not here, user has not yet entered qty and/or nights

                RegisterEvents();
                AddPurchaseItemRow(item);
                return item;
            }
        }

        partial class PurchaseItemRow
        {
            private ItinerarySet ds
            {
                get { return (ItinerarySet)Table.DataSet; }
            }

            public int GetServiceTypeId()
            {
                return ds.OptionLookup.FindByOptionID(OptionID).ServiceTypeID;
            }

            /// <summary>
            /// Gets the Supplier ID of the purchase item
            /// </summary>
            /// <returns>The Supplier ID</returns>
            public int GetSupplierId()
            {
                return PurchaseLineRow.SupplierID;
            }

            /// <summary>
            /// Get the start date and time, and if specified include the Service early checkin time.
            /// </summary>
            public string GetPurchaseItemStartDateTimeString(string dateFormat, string timeFormat, bool includeEarlyCheckin)
            {
                if (IsStartDateNull()) return "";

                // date only
                if (IsStartTimeNull()) return string.Format("{0:" + dateFormat + "}", StartDate);

                // date and time
                var datetime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, StartTime.Second);

                // early checkin
                if (includeEarlyCheckin)
                {
                    var option = ((ItinerarySet)Table.DataSet).OptionLookup.FindByOptionID(OptionID);
                    if (!option.IsCheckinMinutesEarlyNull()) datetime.AddMinutes(option.CheckinMinutesEarly);
                }
                return string.Format("{0:" + dateFormat + " " + timeFormat + "}", datetime);
            }

            /// <summary>
            /// Get the end date and time for this Service, from end-date override or else default.
            /// </summary>
            public string GetPurchaseItemEndDateTimeString(string dateFormat, string timeFormat)
            {
                var endDate = !IsEndDateNull() ?
                    EndDate : // end date override
                    StartDate.Date.AddDays(!IsNumberOfDaysNull() ? NumberOfDays : 0); // calc from start date

                // date only
                if (IsEndTimeNull()) return string.Format("{0:" + dateFormat + "}", endDate);

                // date and time
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, EndTime.Hour, EndTime.Minute, EndTime.Second);
                return string.Format("{0:" + dateFormat + " " + timeFormat + "}", endDate);
            }
            
            /// <summary>
            /// Forces recalculatation of calculated columns (total converted net and gross).
            /// </summary>
            public void RecalculateTotals()
            {
                System.Diagnostics.Debug.WriteLine("Called: PurchaseItemDataTable.PurchaseItemRow.RecalculateTotals(), custom columns recalc");

                NetBaseTotal = calcNetBaseTotal();
                NetTotalConverted = calcTotalNetConverted();
                GrossTotalConverted = calcTotalGrossConverted();
            }

            public decimal NetTotal
            {
                get { return (!IsNetNull() ? Net : 0) * getUnitMultiplier(); }
            }

            public decimal GrossTotal
            {
                get { return (!IsGrossNull() ? Gross : 0) * getUnitMultiplier(); }
            }


            private decimal calcNetBaseTotal()
            {
                return (!IsNetNull() ? Net : 0) * getUnitMultiplier();
            }

            private decimal calcTotalNetConverted()
            {
                return NetTotal * (!IsCurrencyRateNull() ? CurrencyRate : 1);
            }

            private decimal calcTotalGrossConverted()
            {
                return GrossTotal * (!IsCurrencyRateNull() ? CurrencyRate : 1);
            }

            private decimal getUnitMultiplier()
            {
                decimal quantity = !IsQuantityNull() ? (decimal)Quantity : 1;
                decimal numberOfDays = !IsNumberOfDaysNull() ? (decimal)NumberOfDays : 1;
                decimal discount = numberOfDays * (!IsDiscountUnitsNull() ? (decimal)DiscountUnits : 0);
                return (quantity * numberOfDays) - discount;
            }

            public IEnumerable<DiscountRow> GetDiscountRows()
            {
                return ((ItinerarySet) Table.DataSet).Discount.Where(x => x.RowState != DataRowState.Deleted && x.ServiceID == ServiceID);
            }
        }

        #endregion

        #region PaymentTerms

        partial class PaymentTermRow
        {
            public string GetCustomText(ToolSet.PaymentDueDataTable paymentDueTable)
            {
                return Services.Common.GetPaymentTermsFullText(
                    (!IsPaymentDueIDNull()) ? (int?)PaymentDueID : null,
                    (!IsPaymentDuePeriodNull()) ? (int?)PaymentDuePeriod : null,
                    (!IsDepositAmountNull()) ? (decimal?)DepositAmount : null,
                    (!IsDepositTypeNull()) ? (char?)DepositType : null,
                    (!IsDepositDueIDNull()) ? (int?)DepositDueID : null,
                    (!IsDepositDuePeriodNull()) ? (int?)DepositDuePeriod : null,
                    paymentDueTable);
            }
        }

        #endregion

        #region ItinerarySale
        public partial class ItinerarySaleRow
        {
            public decimal Amount
            {
                get
                {
                    var itinerarySet = (ItinerarySet)Table.DataSet;
                    var saleTotals = itinerarySet.ItinerarySaleAllocation.Where(
                            a => a.RowState != DataRowState.Deleted && a.ItinerarySaleID == ItinerarySaleID);

                    decimal totalSales = 0;
                    foreach (var row in saleTotals)
                        totalSales += row["Amount"] != DBNull.Value ? (decimal)row["Amount"] : 0;

                    return totalSales;
                }
            }
        }
        #endregion

        #region ItinerarySaleAllocation

        public partial class ItinerarySaleAllocationDataTable
        {
            /// <summary>
            /// Adds or updates or deletes a row, depending on row existing and amount param having a value.
            /// </summary>
            /// <param name="saleId">The sale id.</param>
            /// <param name="serviceTypeId">The service type id.</param>
            /// <param name="amount">The amount, which if null will cause row to be deleted.</param>
            public void AddInsertOrDelete(int saleId, int serviceTypeId, decimal? amount)
            {
                ItinerarySet ds = DataSet as ItinerarySet;
                if (ds == null)
                    return;

                ItinerarySaleAllocationRow existingRow = FindByItinerarySaleIDServiceTypeID(saleId, serviceTypeId);

                if (existingRow == null && amount.HasValue)
                {
                    // add new row
                    ItinerarySaleAllocationRow r = ds.ItinerarySaleAllocation.NewItinerarySaleAllocationRow();
                    r.ItinerarySaleID = saleId;
                    r.ServiceTypeID = serviceTypeId;
                    r.Amount = (decimal)amount;
                    ds.ItinerarySaleAllocation.AddItinerarySaleAllocationRow(r);
                }
                else if (existingRow != null && amount.HasValue)
                {
                    // update existing row
                    if (existingRow.Amount != (decimal)amount)
                        existingRow.Amount = (decimal)amount;
                }
                else if (existingRow != null)
                {
                    // delete existing row
                    existingRow.Delete();
                }
            }
        }

        #endregion

        #region ItineraryMarginOverride

        partial class ItineraryMarginOverrideDataTable
        {
            /// <summary>
            /// Adds or updates or deletes a row, depending on row existing and margin param having a value.
            /// </summary>
            /// <param name="serviceTypeId">The service type id.</param>
            /// <param name="margin">The margin, which if null will cause row to be deleted.</param>
            public void AddInsertOrDelete(int serviceTypeId, decimal? margin)
            {
                ItinerarySet ds = DataSet as ItinerarySet;
                if (ds == null)
                    return;

                ItineraryMarginOverrideRow existingRow = FindByItineraryIDServiceTypeID(
                    ds.Itinerary[0].ItineraryID, serviceTypeId);

                if (existingRow == null && margin.HasValue)
                {
                    // add new row
                    ItineraryMarginOverrideRow r = ds.ItineraryMarginOverride.NewItineraryMarginOverrideRow();
                    r.ItineraryID = ds.Itinerary[0].ItineraryID;
                    r.ServiceTypeID = serviceTypeId;
                    r.Margin = (decimal)margin;
                    ds.ItineraryMarginOverride.AddItineraryMarginOverrideRow(r);
                }
                else if (existingRow != null && margin.HasValue)
                {
                    // update existing row
                    if (existingRow.Margin != (decimal)margin)
                        existingRow.Margin = (decimal)margin;
                }
                else if (existingRow != null)
                {
                    // delete existing row
                    existingRow.Delete();
                }
            }
        }

        #endregion

        #region Legacy methods - needs cleanup and move to above partial classes!

        /// <summary>
        /// Copy an existing PurchaseLine row.
        /// </summary>
        /// <param name="source">PurchaseLine row to copy from</param>
        /// <param name="prependText">String to append to row name</param>
        /// <param name="userId">Id of user adding the row</param>
        /// <param name="deepCopy">Wether to copy child relations also</param>
        /// <returns>The new PurchaseLine row</returns>
        public PurchaseLineRow CopyPurchaseLine(PurchaseLineRow source, string prependText, int userId, bool deepCopy)
        {
            PurchaseLineRow newRow = PurchaseLine.NewPurchaseLineRow();
            int newId = newRow.PurchaseLineID;
            newRow.ItemArray = source.ItemArray;

            // Set new values.
            newRow.PurchaseLineName = (!string.IsNullOrEmpty(prependText) ? prependText + " " : "") + newRow.PurchaseLineName;
            newRow.PurchaseLineID = newId;
            newRow.AddedBy = userId;
            newRow.AddedOn = DateTime.Now;

            PurchaseLine.AddPurchaseLineRow(newRow);

            if (deepCopy)
            {
                foreach (PurchaseItemRow item in source.GetPurchaseItemRows())
                    if (item.RowState != DataRowState.Deleted)
                        CopyPurchaseItem(item, newRow.PurchaseLineID, null, userId);
            }
            return newRow;
        }

        /// <summary>
        /// Copy an existing PurchaseItem row.
        /// </summary>
        /// <param name="source">PurchaseItem row to copy from</param>
        /// <param name="parentPurchaseLineID">Id of parent row</param>
        /// <param name="prependText">String to append to row name</param>
        /// <param name="userId">Id of user adding the row</param>
        /// <returns>The new PurchaseItem row</returns>
        public PurchaseItemRow CopyPurchaseItem(PurchaseItemRow source, int parentPurchaseLineID, string prependText, int userId)
        {
            PurchaseItemRow newRow = PurchaseItem.NewPurchaseItemRow();
            int newId = newRow.PurchaseItemID;
            newRow.ItemArray = source.ItemArray;

            // Set new values.
            newRow.PurchaseItemName = (!string.IsNullOrEmpty(prependText) ? prependText + " " : "") + newRow.PurchaseItemName;
            newRow.PurchaseItemID = newId;
            newRow.PurchaseLineID = parentPurchaseLineID;
            newRow.AddedBy = userId;
            newRow.AddedOn = DateTime.Now;

            PurchaseItem.AddPurchaseItemRow(newRow);

            return newRow;
        }

        /// <summary>
        /// Copy an existing PubFile row.
        /// </summary>
        /// <param name="source">PubFile row to copy from</param>
        /// <param name="prependText">String to append to row name</param>
        /// <param name="userId">Id of user adding the row</param>
        /// <returns>The new PubFile row</returns>
        public ItineraryPubFileRow CopyPubFile(ItineraryPubFileRow source, string prependText, int userId)
        {
            ItineraryPubFileRow newRow = ItineraryPubFile.NewItineraryPubFileRow();
            int newId = newRow.ItineraryPubFileID;
            newRow.ItemArray = source.ItemArray;

            // Set new values.
            newRow.ItineraryPubFileName = (!string.IsNullOrEmpty(prependText) ? prependText + " " : "") + newRow.ItineraryPubFileName;
            newRow.ItineraryPubFileID = newId;
            newRow.AddedBy = userId;
            newRow.AddedOn = DateTime.Now;

            ItineraryPubFile.AddItineraryPubFileRow(newRow);

            return newRow;
        }


        #region Public accessor methods

        public string GetPurchaseItemFullName(PurchaseLineRow line,
                                              PurchaseItemRow item)
        {
            string s = "";
            s += !line.IsPurchaseLineNameNull() ? (line.PurchaseLineName + ", ") : "";
            s += !item.IsPurchaseItemNameNull() ? (item.PurchaseItemName + ", ") : "";

            return s.Replace(", ,", ",").Replace(",,", ",").Trim().TrimEnd(',');
        }

        public int? GetPurchaseLineCityId(int purchaseLineId)
        {
            SupplierLookupRow supplier =
                SupplierLookup.FindBySupplierID(
                    PurchaseLine.FindByPurchaseLineID(purchaseLineId).SupplierID);

            if (!supplier.IsCityIDNull())
                return supplier.CityID;
            return null;
        }

        public int? GetPurchaseItemCityId(int purchaseItemId)
        {
            SupplierLookupRow supplier =
                SupplierLookup.FindBySupplierID(
                    PurchaseItem.FindByPurchaseItemID(purchaseItemId).PurchaseLineRow.SupplierID);

            if (supplier != null && !supplier.IsCityIDNull())
                return supplier.CityID;
            return null;
        }

        public int? GetPurchaseItemRegionId(int purchaseItemId)
        {
            SupplierLookupRow supplier =
                SupplierLookup.FindBySupplierID(
                    PurchaseItem.FindByPurchaseItemID(purchaseItemId).PurchaseLineRow.SupplierID);

            if (supplier != null && !supplier.IsRegionIDNull())
                return supplier.RegionID;
            return null;
        }

        public int GetPurchaseItemServiceTypeId(int purchaseItemId)
        {
            PurchaseItemRow purchaseItem = null;
            OptionLookupRow option = null;

            try
            {
                purchaseItem = PurchaseItem.FindByPurchaseItemID(purchaseItemId);
                option = OptionLookup.FindByOptionID(purchaseItem.OptionID);

                return option.ServiceTypeID;
            }
            catch (NullReferenceException ex)
            {
                string msg = String.Empty;

                msg += "\r\nPurchase Item: " +
                    (purchaseItem != null ? purchaseItem.PurchaseItemID.ToString() : "null");

                msg += "\r\nPurchase Item OptionID: " +
                    (purchaseItem != null ? purchaseItem.OptionID.ToString() : "null");

                msg += "\r\nOption: " +
                    (option != null ? option.OptionID.ToString() : "null");



                Exception newEx = new Exception(ex.Message + msg);
                throw newEx;
            }
        }

        public int? GetPurchaseItemGradeId(int purchaseItemId)
        {
            SupplierLookupRow supplier =
                SupplierLookup.FindBySupplierID(
                    PurchaseItem.FindByPurchaseItemID(purchaseItemId).PurchaseLineRow.SupplierID);

            if (!supplier.IsGradeIDNull())
                return supplier.GradeID;
            return null;
        }

        public int? GetPurchaseItemGradeExternalId(int purchaseItemId)
        {
            SupplierLookupRow supplier =
                SupplierLookup.FindBySupplierID(
                    PurchaseItem.FindByPurchaseItemID(purchaseItemId).PurchaseLineRow.SupplierID);

            if (!supplier.IsGradeExternalIDNull())
                return supplier.GradeExternalID;
            return null;
        }

        #endregion

        #region Calculations

        #endregion

        public void AddLookupRows(SupplierSet supplierSet, int optionId)
        {
            OptionLookupRow optionLookup = OptionLookup.FindByOptionID(optionId);

            if (optionLookup == null)
            {
                SupplierSet.OptionRow option = supplierSet.Option.FindByOptionID(optionId);
                SupplierSet.RateRow rate = option.RateRow;
                SupplierSet.ServiceRow service = rate.ServiceRow;
                SupplierSet.SupplierRow supplier = service.SupplierRow;

                optionLookup = OptionLookup.NewOptionLookupRow();
                optionLookup.OptionID = option.OptionID;
                optionLookup.OptionName = option.OptionName;
                optionLookup.ServiceName = service.ServiceName;
                optionLookup.SupplierID = supplier.SupplierID;
                if (!service.IsServiceTypeIDNull())
                    optionLookup.ServiceTypeID = service.ServiceTypeID;
                if (!service.IsCheckinTimeNull())
                    optionLookup.CheckinTime = service.CheckinTime;
                if (!option.IsNetNull())
                    optionLookup.Net = option.Net;
                if (!option.IsGrossNull())
                    optionLookup.Gross = option.Gross;
                if (!option.IsPricingOptionNull())
                    optionLookup.PricingOption = option.PricingOption;
                if (!service.IsCheckinMinutesEarlyNull())
                    optionLookup.CheckinMinutesEarly = service.CheckinMinutesEarly;
                optionLookup.ValidFrom = rate.ValidFrom;
                optionLookup.ValidTo = rate.ValidTo;

                OptionLookup.AddOptionLookupRow(optionLookup);


                SupplierLookupRow supplierLookup = SupplierLookup.FindBySupplierID(supplier.SupplierID);

                if (supplierLookup == null)
                {
                    supplierLookup = SupplierLookup.NewSupplierLookupRow();
                    supplierLookup.SupplierID = supplier.SupplierID;
                    supplierLookup.SupplierName = supplier.SupplierName;
                    if (!supplier.IsHostNameNull())
                        supplierLookup.HostName = supplier.HostName;
                    if (!supplier.IsPhoneNull())
                        supplierLookup.Phone = supplier.Phone;
                    if (!supplier.IsFreePhoneNull())
                        supplierLookup.FreePhone = supplier.FreePhone;
                    if (!supplier.IsFaxNull())
                        supplierLookup.Fax = supplier.Fax;
                    if (!supplier.IsEmailNull())
                        supplierLookup.Email = supplier.Email;
                    if (!supplier.IsStreetAddressNull())
                        supplierLookup.StreetAddress = supplier.StreetAddress;
                    if (!supplier.IsCityIDNull())
                        supplierLookup.CityID = supplier.CityID;
                    if (!supplier.IsRegionIDNull())
                        supplierLookup.RegionID = supplier.RegionID;
                    if (!supplier.IsStateIDNull())
                        supplierLookup.StateID = supplier.StateID;
                    if (!supplier.IsCountryIDNull())
                        supplierLookup.CountryID = supplier.CountryID;

                    SupplierLookup.AddSupplierLookupRow(supplierLookup);
                }
            }

            // add service FOCs
            var serviceId = supplierSet.Option.FindByOptionID(optionId).RateRow.ServiceID;
            foreach (var row in Enumerable.Where(supplierSet.Discount.Where(x => x.ServiceID == serviceId), row => Discount.FindByDiscountID(row.DiscountID) == null))
            {
                Discount.Rows.Add(row.DiscountID, row.ServiceID, row.UnitsUsed, row.UnitsFree);
                Discount.Rows[Discount.Rows.Count - 1].AcceptChanges();
            }
        }

        #endregion        

        // Returns existing bookings for this supplier
        public IEnumerable<PurchaseLineRow> GetSupplierBookings(int supplierId)
        {
             return from line in PurchaseLine
                    where (line.RowState != DataRowState.Deleted && line.SupplierID == supplierId)
                    select line;
        }
    }    
}
