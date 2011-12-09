using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TourWriter.Info;

namespace Travelmesh
{
    class DataSync
    {
        private const bool SaveLocalLink = false;
        internal static void SyncSupplier(SupplierSet.SupplierRow supplier)
        {
            var exported = !supplier.IsImportIDNull();
            var city = supplier.IsCityIDNull() ? null
                           : Enumerable.Where(TourWriter.Global.Cache.ToolSet.City, x => x.RowState != DataRowState.Deleted && x.CityID == supplier.CityID).FirstOrDefault();
            var country = supplier.IsCountryIDNull() ? null
                              : Enumerable.Where(TourWriter.Global.Cache.ToolSet.Country, x => x.RowState != DataRowState.Deleted && x.CountryID == supplier.CountryID).FirstOrDefault();

            var sup = new
            {
                Id = exported ? supplier.ImportID : 0,
                Name = ConvertToHtml(supplier.SupplierName),
                Description = !supplier.IsDescriptionNull() ? ConvertToHtml(supplier.Description) : null,
                Street = !supplier.IsStreetAddressNull() ? supplier.StreetAddress : null,
                City = city != null ? city.CityName : null,
                Country = country != null ? country.CountryName : null,
                Postcode = !supplier.IsPostcodeNull() ? supplier.Postcode : null,
                Contact = !supplier.IsHostNameNull() ? supplier.HostName : null,
                Phone = !supplier.IsPhoneNull() ? supplier.Phone : null,
                Email = !supplier.IsEmailNull() ? supplier.Email : null,
                Website = !supplier.IsWebsiteNull() ? supplier.Website : null,
            };
            var response = ApiWrapper.ApiRequest("/suppliers", exported ? "PUT" : "POST", ApiWrapper.ConvertToJson(sup), "application/json; charset=utf-8").Response;
            supplier.ImportID = (int)response["id"];
            if (SaveLocalLink && !exported)
            {
                TourWriter.Info.Services.DatabaseHelper.ExecuteScalar(string.Format("update [Supplier] set ImportID = {0} where SupplierID = {1}", supplier.ImportID,supplier.SupplierID));
                supplier.AcceptChanges();
            }
        }

        internal static void SyncService(SupplierSet.ServiceRow service)
        {
            var exported = !service.IsImportIDNull();
            var type = service.IsServiceTypeIDNull() ? null
                           : Enumerable.Where(TourWriter.Global.Cache.ToolSet.ServiceType, x => x.RowState != DataRowState.Deleted && x.ServiceTypeID == service.ServiceTypeID).FirstOrDefault();

            var srv = new
            {
                Id = exported ? service.ImportID : 0,
                SupplierId = service.SupplierRow.ImportID,
                Name = ConvertToHtml(service.ServiceName),
                Description = !service.IsDescriptionNull() ? ConvertToHtml(service.Description) : null,
                Type = type != null ? type.ServiceTypeName : null,
            };
            var response = ApiWrapper.ApiRequest("/services", exported ? "PUT" : "POST", ApiWrapper.ConvertToJson(srv), "application/json; charset=utf-8").Response;
            service.ImportID = (int)response["id"];
            if (SaveLocalLink && !exported)
            {
                TourWriter.Info.Services.DatabaseHelper.ExecuteScalar(string.Format("update [Service] set ImportID = {0} where ServiceID = {1}", service.ImportID, service.ServiceID));
                service.AcceptChanges();
            }
        }

        internal static void SyncRatesForService(SupplierSet.ServiceRow service)
        {
            SyncRates(service, null);
        }
        
        internal static void SyncRatesForOption(int optionId, SupplierSet.ServiceRow service)
        {
            SyncRates(service, optionId);
        }


        private static void SyncRates(SupplierSet.ServiceRow service, int? forOptionId)
        {
            // current rates
            var allRates = service.GetRateRows().Where(x => x.RowState != DataRowState.Deleted && x.ValidTo > DateTime.Now.AddMonths(1) && x.IsIsDeletedNull()).ToList();

            // and their options
            var allOptions = allRates.SelectMany(x => x.GetOptionRows()).Where(x => x.RowState != DataRowState.Deleted && x.IsIsDeletedNull() && x.IsImportIDNull()).ToList();
            if (forOptionId.HasValue) allOptions = allOptions.Where(x => x.OptionID == forOptionId).ToList();

            // join rates and their child options
            var ratesOptions = from r in allRates
                               join o in allOptions on r.RateID equals o.RateID
                               where o.Net > 0 && o.Gross > 0
                               select new
                               {
                                   OptionName = ConvertToHtml(o.OptionName),
                                   o.OptionID,
                                   r.ValidFrom,
                                   r.ValidTo,
                                   o.Net,
                                   o.Gross,
                                   o.PricingOption,
                                   ServiceImportID = !service.IsImportIDNull() ? service.ImportID : (int?)null, OptionImportID = !o.IsImportIDNull() ? o.ImportID : (int?)null,
                               };

            // flip - unique options have child rates
            var group = ratesOptions.GroupBy(x => new { x.ServiceImportID, x.OptionName });

            // options and their rates
            var optionsRates = group.Select(x => new { ServiceId = x.Key.ServiceImportID, Name = x.Key.OptionName, Rates = x.ToList() }).ToList();

            // sync
            foreach (var option in optionsRates)
            {
                var response = ApiWrapper.ApiRequest("/options", "POST", ApiWrapper.ConvertToJson(option), "application/json; charset=utf-8").Response;
                forOptionId = (int)response["id"];
                foreach (var x in option.Rates)
                {
                    var exported = false;
                    /** x.OptionImportID.HasValue; >> only POST, no PUT as don't have online parent OptionId **/
                    var baseOption = allOptions.Where(t => t.OptionID == x.OptionID).FirstOrDefault();
                    var basePrice = GetRateBasePrice(x.Net, x.Gross, x.PricingOption);
                    var price = GetRatePrice(x.Net, x.Gross, x.PricingOption);
                    var margin = GetRateMargin(x.Net, x.Gross, x.PricingOption);

                    // use dict so we can skip properties with null values
                    var dict = new Dictionary<string, object>();
                    dict.Add("OptionId", forOptionId);
                    dict.Add("ValidFrom", x.ValidFrom);
                    dict.Add("ValidTo", x.ValidTo);
                    dict.Add("BasePrice", basePrice);
                    if (price.HasValue) dict.Add("Price", price); // add if not null
                    if (margin.HasValue) dict.Add("Margin", margin); // add if not null

                    response = ApiWrapper.ApiRequest("/rates", exported ? "PUT" : "POST", ApiWrapper.ConvertToJson(dict), "application/json; charset=utf-8").Response;
                    baseOption.ImportID = (int) response["id"];
                    if (SaveLocalLink && !exported)
                    {
                        TourWriter.Info.Services.DatabaseHelper.ExecuteScalar(string.Format("update [Option] set ImportID = {0} where OptionID = {1}", baseOption.ImportID, x.OptionID));
                        baseOption.AcceptChanges();
                    }
                }
            }
        }
        
        private static decimal? GetRateBasePrice(decimal net, decimal gross, string type)
        {
            decimal? d = null;

            if (type == "nm") d = net;
            if (type == "gc") d = gross;
            if (type == "ng") d = net;

            return d.HasValue ? Math.Round((decimal)d, 4) : 0;
        }

        private static decimal? GetRatePrice(decimal net, decimal gross, string type)
        {
            decimal? d = null;
            var margin = GetRateMargin(net, gross, type) ?? 1;

            if (type == "nm") d = net * (1 + margin);
            if (type == "gc") d = gross * (1 + margin);
            if (type == "ng") d = gross;

            return d.HasValue ? (decimal?)Math.Round((decimal)d, 4) : null;
        }

        private static decimal? GetRateMargin(decimal net, decimal gross, string type)
        {
            decimal? d = null;

            if (type == "nm") d = (gross - net) / net; // markup
            if (type == "gc") d = -((gross - net) / gross); // markdown (commission)
            if (type == "ng") d = null;

            return d.HasValue ? (decimal?)Math.Round((decimal)d, 4) : null;
        }

        private static string ConvertToHtml(string text)
        {
            return text.Trim().Replace("\r\n", "<br/>").Replace("<br/><br/>", "<br/>").Replace("<br/><br/>", "<br/>");
        }
    }
}
