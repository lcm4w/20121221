using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TourWriter.Info;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    class QuoteTable : DataTable
    {
        private readonly ItinerarySet _itinerarySet;
        private enum CostTypes { Client, Staff, Foc };
        internal enum PriceTypes { Net, Gross };
        private static PriceTypes _priceType;

        public QuoteTable() { } // parameterless constructor required for base.Copy() etc
 
        public QuoteTable(ItinerarySet itinerarySet, IEnumerable<ToolSet.OptionTypeRow> optionTypes, PriceTypes priceType)
        {
            _itinerarySet = itinerarySet;
            _priceType = priceType;

            if (Columns.Count == 0)
                BuildTableSchema(itinerarySet.ItineraryPax, optionTypes);
        }

        private static IEnumerable<List<ItinerarySet.PurchaseItemRow>> GetPurchaseItemGroups(DataTable items)
        {
            // NOTE: based grouping on common PurchaseLineId then RateId, eg all options with common RateId.

            var group = new List<ItinerarySet.PurchaseItemRow>();
            var groups = new List<List<ItinerarySet.PurchaseItemRow>>();
            var dv = new DataView { Table = items, Sort = "PurchaseLineID, RateID" };

            var lineId = int.MinValue;
            var rateId = int.MinValue;
            for (var i = 0; i < dv.Count; i++)
            {
                // get valid item
                var item = (ItinerarySet.PurchaseItemRow)dv[i].Row;
                if (item.RowState == DataRowState.Deleted)
                    continue;

                // start new group if item is not related to previous
                if (item.PurchaseLineID != lineId || item.RateID != rateId)
                {
                    lineId = item.PurchaseLineID;
                    rateId = item.RateID;
                    AddGroupToList(groups, group);
                    group = new List<ItinerarySet.PurchaseItemRow>();
                }
                group.Add(item);
            }
            AddGroupToList(groups, group);
            return groups;
        }

        private static void AddGroupToList(ICollection<List<ItinerarySet.PurchaseItemRow>> groups, List<ItinerarySet.PurchaseItemRow> group)
        {
            if (group.Count == 0) return;

            group.Sort((x, y) => y.IsDefaultOptionType.CompareTo(x.IsDefaultOptionType));
            groups.Add(group);
        }

        private void BuildTableSchema(DataTable pax, IEnumerable<ToolSet.OptionTypeRow> options)
        {
            TableName = "QuoteTable";

            // service info
            Columns.AddRange(new[]
                                 {
                                     new DataColumn("PurchaseItemID", typeof (string)),
                                     new DataColumn("SupplierName", typeof (String)),
                                     new DataColumn("ServiceName", typeof (String)),
                                     new DataColumn("ServiceType", typeof (String)),
                                     new DataColumn("BookingDate", typeof (DateTime)),
                                     new DataColumn("Category", typeof (String)),
                                     new DataColumn("ChargeType", typeof (String)),
                                     new DataColumn("Price", typeof (Decimal))
                                 });

            // pax columns
            foreach (ItinerarySet.ItineraryPaxRow p in pax.Rows)
            {
                if (p.RowState == DataRowState.Deleted) continue;
                Columns.Add(new PaxColumn
                {
                    ColumnName = p.ItineraryPaxName,
                    Key = p,
                    DataType = typeof(Decimal)
                });
            }

            // suppliments
            foreach (ToolSet.OptionTypeRow o in options.Where(x => x.RowState != DataRowState.Deleted))
            {
                //if (o.OptionTypeID == 2) continue; // don't add default double here, its used for the default Pax columns
                Columns.Add(new SuppliementsColumn
                {
                    ColumnName = o.OptionTypeName,
                    Key = o.OptionTypeID,
                    Divisor = o.Divisor,
                    DataType = typeof(Decimal)
                });
            }
        }

        #region Table

        public void TablePopulate(ItinerarySet.PurchaseItemDataTable items)
        {
            var groups = GetPurchaseItemGroups(items);
            foreach (List<ItinerarySet.PurchaseItemRow> group in groups)
                TableAddItemGroup(group);

            TablePrintSums();
        }

        private void TablePrintSums()
        {
            foreach (DataColumn c in Columns)
            {
                if (c is PaxColumn || c is SuppliementsColumn)
                {
                    string colName = c.ColumnName;
                    var q = from t in this.AsEnumerable()
                            select t.Field<decimal?>(colName);

                    App.Debug(string.Format("> Itinerary quote total - {0}: \t ${1}", colName, decimal.Round((decimal)q.Sum(), 2)));
                }
            }
        }
        
        private void TableAddItemGroup(IEnumerable<ItinerarySet.PurchaseItemRow> group)
        {
            DataRow clientRow = NewRow(), staffRow = NewRow(), focRow = NewRow();

            RowPopulate(clientRow, group, CostTypes.Client);
            Rows.Add(clientRow);

            RowPopulate(staffRow, group, CostTypes.Staff);
            if (RowHasValues(staffRow))
                Rows.Add(staffRow);

            RowPopulate(focRow, group, CostTypes.Foc);
            if (RowHasValues(focRow))
                Rows.Add(focRow);
        }

        private DataColumn GetPaxColumn(ItinerarySet.ItineraryPaxRow key)
        {            
            return Columns.Cast<DataColumn>().FirstOrDefault(c => c is PaxColumn && ((PaxColumn) c).Key == key);
        }

        private SuppliementsColumn GetSuppliementsColumn(int key)
        {
            foreach (DataColumn c in Columns)
                if (c is SuppliementsColumn && ((SuppliementsColumn)c).Key == key)
                    return c as SuppliementsColumn;
            return null;
        }

        #endregion

        #region Rows

        private void RowPopulate(DataRow row, IEnumerable<ItinerarySet.PurchaseItemRow> group, CostTypes costType)
        {
            foreach (ItinerarySet.PurchaseItemRow item in group)
            {
                bool isNewRow = row.ItemArray[0] == DBNull.Value;
                bool addPaxItem = item.IsDefaultOptionType;
                bool addSupplimentItem = /*!addPaxItem &&*/ !item.IsOptionTypeIDNull() && costType == CostTypes.Client;
                bool isValid = addPaxItem || addSupplimentItem || costType != CostTypes.Client;

                // populate text details columns
                if (isNewRow)
                {
                    RowPopulateDetails(row, item, costType);
                }

                // populate money values columns
                if (addPaxItem)
                {
                    var price = GetItemPrice(item);//.Gross*(decimal) item.NumberOfDays;
                    var currentTotal = row["Price"] != DBNull.Value ? (decimal) row["Price"] : 0;
                    row["Price"] = currentTotal + price;
                    RowPopulatePaxColumns(row, item, costType);
                }
                if (addSupplimentItem)
                {
                    RowPopulateSupplimentColumn(row, item, addPaxItem);
                }

                // validate
                if (!isValid) row.RowError = "Column value ommited, item was not marked as pax price or suppliment price";
            }
        }

        private static void RowPopulateDetails(DataRow row, ItinerarySet.PurchaseItemRow item, CostTypes costType)
        {
            row.ItemArray = new[]
                                {
                                    item.PurchaseItemID,
                                    item.PurchaseLineRow.PurchaseLineName,
                                    item.PurchaseItemName,
                                    item.ServiceTypeName,
                                    !item.IsStartDateNull() ? (object) item.StartDate : DBNull.Value,
                                    costType.ToString(),
                                    item.ChargeType
                                };
        }

        private void RowPopulatePaxColumns(DataRow row, ItinerarySet.PurchaseItemRow item, CostTypes costType)
        {
            // if item is 'double' (default), then use it to populate all teh pax columns

            foreach (ItinerarySet.ItineraryPaxRow pax in _itinerarySet.ItineraryPax.Rows)
            {
                var paxCol = GetPaxColumn(pax);
                if (paxCol == null) continue;

                var paxMultiplier = RowGetPaxChargeMultiplier(item, pax, costType);
                var price = (double)GetItemPrice(item) * paxMultiplier; //.Gross * item.NumberOfDays * paxMultiplier;
                try
                {
                    var currentTotal = row[paxCol] != DBNull.Value && !string.IsNullOrEmpty(row[paxCol].ToString().Trim()) ? Convert.ToDouble(row[paxCol]) : 0;
                    row[paxCol] = currentTotal + price;
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidCastException("Failed to cast value to Double: " + row[paxCol], ex);
                }
            }
        }

        private void RowPopulateSupplimentColumn(DataRow row, ItinerarySet.PurchaseItemRow item, bool isPaxItem)
        {
            //  each item represents one surcharge (suppliement, reduction, etc - eg on item per surcharge).

            var supCol = GetSuppliementsColumn(item.OptionTypeID);
            if (supCol == null) return;

            var price = GetItemPrice(item) / supCol.Divisor; //.Gross / supCol.Divisor;
            if (!isPaxItem && row["Price"] != DBNull.Value)
            {
                var basePrice = (decimal) row["Price"];
                if (item.ChargeType == "ROOM")
                    basePrice = basePrice / 2;
                price = price - basePrice;
            }
            var currentTotal = row[supCol] != DBNull.Value ? (decimal)row[supCol] : 0;
            row[supCol] = currentTotal + price;
        }

        private static bool RowHasValues(DataRow row)
        {
            foreach (DataColumn col in row.Table.Columns)
            {
                var isValueCol = col is PaxColumn || col is SuppliementsColumn;
                if (isValueCol && row[col] != DBNull.Value)
                {
                    return true;
                }
            }
            return false;
        }

        private double RowGetPaxChargeMultiplier(ItinerarySet.PurchaseItemRow item, ItinerarySet.ItineraryPaxRow pax, CostTypes costType)
        {   
            var ovrride = item.GetItineraryPaxOverrideRows().
                Where(o => o.ItineraryPaxID == pax.ItineraryPaxID && o.PurchaseItemID == item.PurchaseItemID).FirstOrDefault();

            double memberRooms = ovrride != null && !ovrride.IsMemberRoomsNull() ? ovrride.MemberRooms : pax.MemberRooms;
            double memberCount = ovrride != null && !ovrride.IsMemberCountNull() ? ovrride.MemberCount : pax.MemberCount;
            double staffRooms = ovrride != null && !ovrride.IsStaffRoomsNull() ? ovrride.StaffRooms : pax.StaffRooms;
            double staffCount = ovrride != null && !ovrride.IsStaffCountNull() ? ovrride.StaffCount : pax.StaffCount;

            switch (costType)
            {
                case CostTypes.Client:
                    {
                        return
                            (item.ChargeType == "ROOM" ) ? (memberCount != 0 ? memberRooms / memberCount : 0) :
                            (item.ChargeType == "GROUP") ? (memberCount != 0 ? 1 / memberCount : 0) :
                            (item.ChargeType == "PAX"  ) ? (memberCount != 0 ? 1 : memberCount) :
                            1;
                    }
                case CostTypes.Staff:
                    {
                       // return memberCount != 0 ? staffRooms / memberCount : 0;

                        double memCount = pax.MemberCount;

                        return
                            (item.ChargeType == "ROOM") ? (memCount != 0 ? staffRooms / memCount : 0) :
                            (item.ChargeType == "PAX") ? (memCount != 0 ? staffCount / memCount : 0) :
                            0;
                    }
                case CostTypes.Foc:
                    {
                        var qty = (item.ChargeType == "ROOM")
                                      ? memberRooms + staffRooms
                                      : memberCount + staffCount;

                        var foc = Services.Discounts.CalcDiscount((decimal)qty, item.GetDiscountRows());
                        return memberCount != 0 ? -((double)foc/memberCount) : 0;
                    }
                default: return 1;
            }
        }

        private static decimal GetItemPrice(ItinerarySet.PurchaseItemRow item)
        {
            return _priceType == PriceTypes.Gross ? item.GrossTotalConverted : item.NetTotalConverted;
        }

        #endregion

        #region Columns

        internal class PaxColumn : DataColumn
        {
            public ItinerarySet.ItineraryPaxRow Key { get; set; }
        }

        internal class SuppliementsColumn : DataColumn
        {
            public int Key { get; set; }
            public int Divisor { get; set; }
        }

        #endregion
    }
}
