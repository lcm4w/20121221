using System;
using System.Collections.Generic;
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
                var existingChanges = GetChanges();
                
                var sql = string.Format(@"
select * from CurrencyRate 
where CodeFrom = '{0}' 
and CodeTo = '{1}' 
and ValidFrom <= '{3}' 
and ValidTo >= '{2}'
and (EnabledFrom is null or EnabledFrom <= '{4}')
and (EnabledTo is null or EnabledTo >= '{4}')",
                    fromCurrency, toCurrency, fromDate.ToString("yyyy.MM.dd"), toDate.ToString("yyyy.MM.dd"), DateTime.Now.Date.ToString("yyyy.MM.dd"));

                DataSetHelper.FillDataset(DataSet, sql);
                if (existingChanges != null) Merge(existingChanges);
            }

            /// <summary>
            /// Get a currency rate row
            /// </summary>
            /// <param name="date">The date for the exchange rate</param>
            /// <param name="from">The from currency code</param>
            /// <param name="to">The to currency code</param>
            /// <param name="loadIfNotCached">Loads from the database, if not already in the local dataset</param>
            /// <returns></returns>
            public List<CurrencyRateRow> GetCurrencyRate(DateTime date, string from, string to, bool loadIfNotCached)
            {
                var rates = GetCurrencyRate(date, from, to);

                if (rates.Count() == 0 && loadIfNotCached)
                {
                    // load from db, then try again
                    Load(from, to, date.Date, date.Date);
                    rates = GetCurrencyRate(date, from, to);
                }
                return rates.ToList();
            }

            private List<CurrencyRateRow> GetCurrencyRate(DateTime date, string from, string to)
            {
                var valid = this.Where(x => x.CodeFrom == from &&
                                            x.CodeTo == to &&
                                            x.ValidFrom.Date <= date.Date &&
                                            x.ValidTo.Date >= date.Date).ToList();

                var enabled = valid.Where(x => (x.IsEnabledFromNull() || x.EnabledFrom.Date <= DateTime.Now.Date) &&
                                               (x.IsEnabledToNull() || x.EnabledTo.Date >= DateTime.Now.Date)).ToList();

                return enabled;
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
