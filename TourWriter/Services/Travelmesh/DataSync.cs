using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TourWriter.Info;
using TourWriter.Services;

namespace Travelmesh
{
    class DataSync
    {
        internal static void SyncSupplier(SupplierSet.SupplierRow supplier)
        {
            var exported = !supplier.IsImportIDNull();
            var city = supplier.IsCityIDNull()
                           ? null
                           : Enumerable.Where(TourWriter.Global.Cache.ToolSet.City, x => x.RowState != DataRowState.Deleted && x.CityID == supplier.CityID).
                                 FirstOrDefault();
            var country = supplier.IsCountryIDNull()
                              ? null
                              : Enumerable.Where(TourWriter.Global.Cache.ToolSet.Country, x => x.RowState != DataRowState.Deleted && x.CountryID == supplier.CountryID).
                                    FirstOrDefault();

            var sup = new
            {
                Id = exported ? supplier.ImportID : 0,
                Name = supplier.SupplierName,
                Description =
                    !supplier.IsDescriptionNull() ? ConvertToHtml(supplier.Description) : null,
                Street = !supplier.IsStreetAddressNull() ? supplier.StreetAddress : null,
                City = city != null ? city.CityName : null,
                Country = country != null ? country.CountryName : null,
                Postcode = !supplier.IsPostcodeNull() ? supplier.Postcode : null,
                Contact = !supplier.IsHostNameNull() ? supplier.HostName : null,
                Phone = !supplier.IsPhoneNull() ? supplier.Phone : null,
                Email = !supplier.IsEmailNull() ? supplier.Email : null,
                Website = !supplier.IsWebsiteNull() ? supplier.Website : null,
            };
            supplier.ImportID = ApiWrapper.ApiRequest<int>("/supplier", exported ? "PUT" : "POST",
                                                           ApiWrapper.ConvertToJson(sup),
                                                           "application/json; charset=utf-8");
            if (!exported)
                TourWriter.Info.Services.DatabaseHelper.ExecuteScalar(string.Format(
                    "update [Supplier] set ImportID = {0} where SupplierID = {1}", supplier.ImportID,
                    supplier.SupplierID));
            supplier.AcceptChanges();
        }

        internal static void SyncService(SupplierSet.ServiceRow service)
        {
            var exported = !service.IsImportIDNull();
            var type = service.IsServiceTypeIDNull()
                           ? null
                           : Enumerable.Where(TourWriter.Global.Cache.ToolSet.ServiceType, x => x.RowState != DataRowState.Deleted && x.ServiceTypeID == service.ServiceTypeID).
                                 FirstOrDefault();

            var srv = new
            {
                Id = exported ? service.ImportID : 0,
                SupplierId = service.SupplierRow.ImportID,
                Name = service.ServiceName,
                Description = !service.IsDescriptionNull() ? ConvertToHtml(service.Description) : null,
                Type = type != null ? type.ServiceTypeName : null,
            };
            service.ImportID = ApiWrapper.ApiRequest<int>("/service", exported ? "PUT" : "POST",
                                                          ApiWrapper.ConvertToJson(srv),
                                                          "application/json; charset=utf-8");
            if (!exported)
                TourWriter.Info.Services.DatabaseHelper.ExecuteScalar(string.Format(
                    "update [Service] set ImportID = {0} where ServiceID = {1}", service.ImportID, service.ServiceID));
            service.AcceptChanges();
        }

        internal static void CreateServiceRates(SupplierSet.ServiceRow service)
        {
            CreateServiceRates(service, null);
        }

        internal static void CreateServiceRates(SupplierSet.ServiceRow service, int? optionId)
        {
            // current rates
            var rateRows = service.GetRateRows().Where(x => x.RowState != DataRowState.Deleted &&
                                                            x.ValidTo > DateTime.Now.AddMonths(1) &&
                                                            x.IsIsDeletedNull()).ToList();

            // 'new' options (not yet created)
            var optionRows =
                rateRows.SelectMany(x => x.GetOptionRows()).Where(x => x.RowState != DataRowState.Deleted &&
                                                                       x.IsIsDeletedNull() && x.IsImportIDNull()).
                    ToList();

            if (optionId.HasValue) optionRows = optionRows.Where(x => x.OptionID == optionId).ToList();

            // join rates and their child options
            var ratesOptions = from r in rateRows
                               join o in optionRows on r.RateID equals o.RateID
                               select new
                               {
                                   o.OptionName,
                                   o.OptionID,
                                   r.ValidFrom,
                                   r.ValidTo,
                                   o.Net,
                                   o.Gross,
                                   o.PricingOption,
                                   ServiceImportID =
                        !service.IsImportIDNull() ? service.ImportID : (int?)null,
                                   OptionImportID = !o.IsImportIDNull() ? o.ImportID : (int?)null,
                               };

            // flip - options have child rates
            var group = ratesOptions.GroupBy(x => new { x.ServiceImportID, x.OptionName });

            var optionsRates =
                group.Select(
                    x => new { ServiceId = x.Key.ServiceImportID, Name = x.Key.OptionName, Rates = x.ToList() }).
                    ToList();
            foreach (var option in optionsRates)
            {
                optionId = ApiWrapper.ApiRequest<int>("/option", "POST", ApiWrapper.ConvertToJson(option),
                                                      "application/json; charset=utf-8");

                foreach (var x in option.Rates)
                {
                    var exported = false;
                    /** x.OptionImportID.HasValue; >> only POST, no PUT as don't have online parent OptionId **/
                    var baseOption = optionRows.Where(t => t.OptionID == x.OptionID).FirstOrDefault();
                    var basePrice = GetRateBasePrice(x.Net, x.Gross, x.PricingOption);
                    var overridePrice = GetRateOverridePrice(x.Gross, x.PricingOption);
                    var overridePercent = GetRateOverridePercent(x.Net, x.Gross, x.PricingOption);

                    // use dict so we can skip properties with null values
                    var dict = new Dictionary<string, object>();
                    dict.Add("OptionId", optionId);
                    dict.Add("ValidFrom", x.ValidFrom);
                    dict.Add("ValidTo", x.ValidTo);
                    dict.Add("BasePrice", basePrice);
                    if (overridePrice.HasValue) dict.Add("OverridePrice", optionId); // skip null
                    if (overridePercent.HasValue) dict.Add("OverridePercent", optionId);

                    baseOption.ImportID = ApiWrapper.ApiRequest<int>("/rate", exported ? "PUT" : "POST",
                                                                     ApiWrapper.ConvertToJson(dict),
                                                                     "application/json; charset=utf-8");
                    if (!exported)
                        TourWriter.Info.Services.DatabaseHelper.ExecuteScalar(string.Format(
                            "update [Option] set ImportID = {0} where OptionID = {1}", baseOption.ImportID,
                            x.OptionID));
                    baseOption.AcceptChanges();
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

        private static decimal? GetRateOverridePrice(decimal gross, string type)
        {
            decimal? d = null;

            if (type == "nm") d = null;
            if (type == "gc") d = null;
            if (type == "ng") d = gross;

            return d.HasValue ? (decimal?)Math.Round((decimal)d, 4) : null;
        }

        private static decimal? GetRateOverridePercent(decimal net, decimal gross, string type)
        {
            decimal? d = null;

            if (type == "nm") d = (gross - net) / net; // markup
            if (type == "gc") d = -((gross - net) / gross); // markdown (commission)
            if (type == "ng") d = null;

            return d.HasValue ? (decimal?)Math.Round((decimal)d, 4) : null;
        }

        private static string ConvertToHtml(string text)
        {
            return text.Replace("\r\n", "<br/>".Replace("<br/><br/>", "<br/>").Trim());
        }
    }
}
