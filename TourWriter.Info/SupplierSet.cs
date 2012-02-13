using System;

namespace TourWriter.Info {


    partial class SupplierSet
    {
        partial class ServiceWarningDataTable
        {
        }
    
        partial class SupplierDataTable
        {
        }
    
        partial class PaymentTermDataTable
        {
        }

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

        #region Legacy methods

        public ServiceRow AddService(string name, int userId, string currencyCode)
        {
            ServiceRow row = Service.NewServiceRow();

            row.SupplierID = Supplier[0].SupplierID;
            row.ServiceName = name;
            if (!Supplier[0].IsDefaultCheckinTimeNull())
                row.CheckinTime = Supplier[0].DefaultCheckinTime;
            if (!Supplier[0].IsDefaultCheckoutTimeNull())
                row.CheckoutTime = Supplier[0].DefaultCheckoutTime;
            if (currencyCode != "")
                row.CurrencyCode = currencyCode;
            row.IsRecordActive = true;
            row.AddedOn = DateTime.Now;
            row.AddedBy = userId;

            Service.AddServiceRow(row);
            return row;
        }

        /// <summary>
        /// Copy an existing service row.
        /// </summary>
        /// <param name="source">Service row to copy from</param>>
        /// <param name="prependText">String to prepend to row name</param>
        /// <param name="userId">Id of user adding the row</param>
        /// <param name="deepCopy">Copy child data also</param>
        /// <returns>The new service row</returns>
        public ServiceRow CopyServiceRow(ServiceRow source, string prependText, int userId, bool deepCopy)
        {
            ServiceRow newRow = Service.NewServiceRow();
            int id = newRow.ServiceID; // remember new id

            // Copy all values.
            //newRow.ItemArray = source.ItemArray; // this deletes child rows in SupplierConfigs table
            foreach (System.Data.DataColumn c in source.Table.Columns)
                if (c.ColumnName != "ServiceID") newRow[c] = source[c];

            // Set some new values.
            newRow.ServiceName = (!string.IsNullOrEmpty(prependText) ? prependText + " " : "") + newRow.ServiceName;
            newRow.ServiceID = id; // reset id to new id
            newRow.AddedBy = userId;
            newRow.AddedOn = DateTime.Now;

            Service.AddServiceRow(newRow);

            if (deepCopy) // Copy child data
            {
                foreach (RateRow rate in source.GetRateRows())
                {
                    CopyRate(rate, newRow.ServiceID, userId, deepCopy, false);
                }
                foreach (ServiceConfigRow config in source.GetServiceConfigRows())
                {
                    CopyServiceConfig(config, newRow.ServiceID, userId);
                }
            }
            return newRow;
        }

        /// <summary>
        /// Copy an existing rate row.
        /// </summary>
        /// <param name="source">Rate row to copy from</param>
        /// <param name="parentServiceId">Id of parent row</param>
        /// <param name="userId">Id of user adding the row</param>
        /// <param name="deepCopy">Copy child data also</param>
        /// <returns>The new rate row</returns>
        /// <param name="incrementDates">Increments the Rate dates</param>
        public RateRow CopyRate(RateRow source, int parentServiceId, int userId, bool deepCopy, bool incrementDates)
        {
            RateRow newRow = Rate.NewRateRow();
            int newId = newRow.RateID;
            newRow.ItemArray = source.ItemArray;

            // Set new values.
            newRow.RateID = newId;
            newRow.ServiceID = parentServiceId;
            newRow.AddedBy = userId;
            newRow.AddedOn = DateTime.Now;
            if (incrementDates)
            {
                newRow.ValidFrom = newRow.ValidTo.AddDays(1); // set to next season
                newRow.ValidTo = newRow.ValidFrom; // set to same as start
            }

            Rate.AddRateRow(newRow);

            if (deepCopy)
            {
                foreach (OptionRow option in source.GetOptionRows())
                {
                    CopyOption(option, newRow.RateID, null, userId);
                }
            }
            return newRow;
        }

        /// <summary>
        /// Copy an existing option row.
        /// </summary>
        /// <param name="source">Option row to copy from</param>
        /// <param name="parentRateId">Id of parent row</param>
        /// <param name="prependText">String to append to row name</param>
        /// <param name="userId">Id of user adding the row</param>
        /// <returns>The new rate row</returns>
        public OptionRow CopyOption(OptionRow source, int parentRateId, string prependText, int userId)
        {
            OptionRow newRow = Option.NewOptionRow();
            int newId = newRow.OptionID;
            newRow.ItemArray = source.ItemArray;

            // Set new values.
            newRow.OptionName = (!string.IsNullOrEmpty(prependText) ? prependText + " " : "") + newRow.OptionName;
            newRow.OptionID = newId;
            newRow.RateID = parentRateId;
            newRow.AddedBy = userId;
            newRow.AddedOn = DateTime.Now;

            Option.AddOptionRow(newRow);

            return newRow;
        }

        /// <summary>
        /// Copy an existing service configuration row.
        /// </summary>
        /// <param name="source">Service configuration row to copy from</param>
        /// <param name="parentServiceId">Id of parent row</param>
        /// <param name="userId">Id of user adding the row</param>
        /// <returns>The new service configuration row</returns>
        public ServiceConfigRow CopyServiceConfig(ServiceConfigRow source, int parentServiceId, int userId)
        {
            ServiceConfigRow newRow = ServiceConfig.NewServiceConfigRow();
            newRow.ItemArray = source.ItemArray;

            // Set new values.
            newRow.ServiceID = parentServiceId;
            newRow.AddedBy = userId;
            newRow.AddedOn = DateTime.Now;

            ServiceConfig.AddServiceConfigRow(newRow);
            return newRow;
        }
        
        #endregion
        
    }
}
