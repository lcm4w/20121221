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

        public QuoteTable(ItinerarySet itinerarySet, IEnumerable<ToolSet.OptionTypeRow> optionTypes)
        {
            _itinerarySet = itinerarySet;

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
                                     new DataColumn("Gross", typeof (Decimal))
                                 });

            // pax columns
            foreach (ItinerarySet.ItineraryPaxRow p in pax.Rows)
            {
                Columns.Add(new PaxColumn
                {
                    ColumnName = p.ItineraryPaxName,
                    Key = p,
                    DataType = typeof(Decimal)
                });
            }

            // suppliments
            foreach (ToolSet.OptionTypeRow o in options)
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
                bool addSupplimentItem = !addPaxItem && !item.IsOptionTypeIDNull() && costType == CostTypes.Client;
                bool isValid = addPaxItem || addSupplimentItem || costType != CostTypes.Client;

                // populate text details columns
                if (isNewRow)
                {
                    RowPopulateDetails(row, item, costType);
                }

                // populate money values columns
                if (addPaxItem)
                {
                    var totalGross = item.Gross*(decimal) item.NumberOfDays;
                    row["Gross"] = totalGross + (row["Gross"] != DBNull.Value ? (decimal)row["Gross"] : 0);
                    RowPopulatePaxColumns(row, item, costType);
                }
                if (addSupplimentItem)
                {
                    RowPopulateSupplimentColumns(row, item);
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
                                    item.PurchaseLineRow.SupplierName,
                                    item.ServiceName,
                                    item.ServiceTypeName,
                                    !item.IsStartDateNull() ? (object) item.StartDate : DBNull.Value,
                                    costType.ToString(),
                                    item.ChargeType
                                };
        }

        private void RowPopulatePaxColumns(DataRow row, ItinerarySet.PurchaseItemRow item, CostTypes costType)
        {
            foreach (ItinerarySet.ItineraryPaxRow pax in _itinerarySet.ItineraryPax.Rows)
            {
                var paxCol = GetPaxColumn(pax);
                if (paxCol == null) continue;

                var paxMultiplier = RowGetPaxChargeMultiplier(item, pax, costType);
                var newVal = (double)item.Gross * item.NumberOfDays * paxMultiplier;
                var oldVal = row[paxCol] != DBNull.Value ? (double)row[paxCol] : 0;
                row[paxCol] = oldVal + newVal;
            }
        }

        private void RowPopulateSupplimentColumns(DataRow row, ItinerarySet.PurchaseItemRow item)
        {
            var supCol = GetSuppliementsColumn(item.OptionTypeID);
            if (supCol == null) return;

            var newVal = item.Gross / supCol.Divisor;
            if (row["Gross"] != DBNull.Value)
            {
                var gross = (decimal) row["Gross"];
                if (item.ChargeType == "ROOM")
                    gross = gross / 2;
                newVal = newVal - gross;
            }
            var oldVal = row[supCol] != DBNull.Value ? (decimal) row[supCol] : 0;
            row[supCol] = oldVal + newVal;
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
                            1;
                    }
                case CostTypes.Staff:
                    {
                       // return memberCount != 0 ? staffRooms / memberCount : 0;

                        return
                            (item.ChargeType == "ROOM") ? (memberCount != 0 ? staffRooms / memberCount : 0) :
                            (item.ChargeType == "PAX") ? (memberCount != 0 ? staffCount / memberCount : 0) :
                            0;
                    }
                case CostTypes.Foc:
                    {
                        var foc = RowGetPaxFocs(item, pax);
                        return memberCount != 0 ? -(foc/memberCount) : 0;
                    }
                default: return 1;
            }
        }

        private double RowGetPaxFocs(ItinerarySet.PurchaseItemRow item, ItinerarySet.ItineraryPaxRow pax)
        {
            double qty = (item.ChargeType == "ROOM")
                              ? pax.MemberRooms + pax.StaffRooms // total rooms
                              : pax.MemberCount + pax.StaffCount;// total pax

            var q =
                _itinerarySet.ServiceFoc.Where(foc =>
                    foc.RowState != DataRowState.Deleted && foc.ServiceID == item.ServiceID && qty >= foc.UnitsUsed).
                    Select(foc => foc.UnitsFree);

            return q.Count() > 0 ? q.Max() : 0;
        }

        #endregion

        #region Columns

        class PaxColumn : DataColumn
        {
            public ItinerarySet.ItineraryPaxRow Key { get; set; }
        }

        class SuppliementsColumn : DataColumn
        {
            public int Key { get; set; }
            public int Divisor { get; set; }
        }

        #endregion
    }
}
