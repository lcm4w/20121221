using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TourWriter.Info.Services;

namespace TourWriter.Info 
{
    
    partial class ToolSet
    {
        partial class ServiceTypeDataTable
        {
            public ServiceTypeRow MarkupContainerRow
            {
                get
                {
                    foreach (ServiceTypeRow row in Rows)
                    {
                        if (!row.IsIsAdditionalMarkupContainerNull() && row.IsAdditionalMarkupContainer)
                            return row;
                    }
                    return null;
                }
                set
                {
                    foreach (ServiceTypeRow row in Rows)
                    {
                        row.IsAdditionalMarkupContainer = (value != null && value.ServiceTypeID == row.ServiceTypeID);
                    }
                }
            }
        }

        partial class PaymentTypeDataTable
        {
            public PaymentTypeRow DefaultPaymentTypeRow
            {
                get
                {
                    foreach (PaymentTypeRow row in Rows)
                    {
                        if (!row.IsIsDefaultNull() && row.IsDefault)
                            return row;
                    }
                    return null;
                }
                set
                {
                    foreach (PaymentTypeRow row in Rows)
                    {
                        row.IsDefault = (value != null && value.PaymentTypeID == row.PaymentTypeID);
                    }
                }
            }
        }

        public override void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler, params DataTable[] tables)
        {
            base.Load(reader, loadOption, errorHandler, tables);

            // Necessary so that OnTableNewRow is called!
            foreach(DataTable table in Tables)
                table.TableNewRow += delegate { };
        }
        
        partial class CurrencyRateDataTable
        {
            public void Load(string fromCurrency, string toCurrency, DateTime fromDate, DateTime toDate)
            {
                // Retain changes to merge into new data.
                DataTable changes = GetChanges();

                SqlParameter param1 = new SqlParameter("@CurrencyCodeFrom", fromCurrency);
                SqlParameter param2 = new SqlParameter("@CurrencyCodeTo", toCurrency);
                SqlParameter param3 = new SqlParameter("@DateFrom", fromDate.Date);
                SqlParameter param4 = new SqlParameter("@DateTo", toDate.Date);

                DataSetHelper.FillDataSet(
                    ConnectionString.GetConnectionString(), DataSet, "_ToolSet_Load_CurrencyRate",
                    new string[1] { TableName }, param1, param2, param3, param4);

                if (changes != null)
                    Merge(changes);
            }

            /// <summary>
            /// Get a currency rate row
            /// </summary>
            /// <param name="date">The date for the exchange rate</param>
            /// <param name="from">The from currency code</param>
            /// <param name="to">The to currency code</param>
            /// <param name="loadIfNotCached">Loads from the database, if not already in the local dataset</param>
            /// <returns></returns>
            public CurrencyRateRow GetCurrencyRate(DateTime date, string from, string to, bool loadIfNotCached)
            {
                // set exch rates here after user has finished editing dates
                var currencyRate = this.Where(x => x.ForecastDate == date && x.CurrencyCodeFrom == from && x.CurrencyCodeTo == to).FirstOrDefault();

                if (currencyRate == null && loadIfNotCached)
                {
                    // load from database and try again
                    Load(from, to, date.Date, date.Date);
                    currencyRate = this.Where(x => x.ForecastDate == date && x.CurrencyCodeFrom == from && x.CurrencyCodeTo == to).FirstOrDefault();
                }
                return currencyRate;
            }
        }

        partial class AgentDataTable
        {
            public void PopulateAgentNameParentName()
            {
                var rows = this.Where(r => r.RowState != DataRowState.Deleted);
                foreach (var row in rows)
                {
                    var name = row.AgentName;
                    if (!row.IsParentAgentIDNull())
                    {
                        int id = row.ParentAgentID;

                        var parent = rows.Where(r => r.AgentID == id);
                        if (parent.Count() > 0)
                            name = row.AgentName + string.Format(" ({0})", parent.First().AgentName);
                    }
                    if (row.IsAgentNameParentNameNull() || row.AgentNameParentName != name) 
                        row.AgentNameParentName = name;
                }
            }
        }
    }
}
