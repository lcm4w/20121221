using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using Infragistics.Win.UltraWinGrid;
using TourWriter.Global;
using TourWriter.Info;

namespace TourWriter.Services
{
    class CurrencyService
    {
        public static ToolSet.CurrencyRow GetCurrency(string currencyCode)
        {
            return Cache.ToolSet.Currency.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
        }

        internal static string GetComputerCurrencyCode()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).
                Where(x => x.Name == Thread.CurrentThread.CurrentCulture.Name).
                Select(x => new RegionInfo(x.LCID).ISOCurrencySymbol).
                FirstOrDefault();
        }

        internal static string GetApplicationCurrencyCode()
        {
            var entity = Cache.ToolSet.AppSettings[0];

            return entity != null && !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ? 
                entity.CurrencyCode : null;
        }

        internal static string GetApplicationCurrencyCodeOrDefault()
        {
            return GetApplicationCurrencyCode() ?? GetComputerCurrencyCode();
        }

        internal static string GetServiceCurrencyCode(SupplierSet.ServiceRow entity)
        {
            return entity != null && !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ? 
                entity.CurrencyCode : null;
        }

        internal static string GetItineraryCurrencyCode(ItinerarySet.ItineraryRow entity)
        {
            return entity != null && !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ? 
                entity.CurrencyCode : null;
        }

        internal static string GetItineraryCurrencyCodeOrDefault(ItinerarySet.ItineraryRow entity)
        {
            return GetItineraryCurrencyCode(entity) ?? GetApplicationCurrencyCodeOrDefault();
        }

        internal static string GetPurchaseItemCurrencyCode(ItinerarySet.PurchaseItemRow entity)
        {
            return entity != null && !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ? 
                entity.CurrencyCode : null;
        }

        internal static string GetPurchaseItemCurrencyCodeOrDefault(ItinerarySet.PurchaseItemRow entity)
        {
            return GetPurchaseItemCurrencyCode(entity) ?? GetApplicationCurrencyCodeOrDefault();
        }
        
        internal static string GetPurchaseItemCurrencyCodeOrDefault(UltraGridCell currencyCodeCell)
        {
            var ccyCode = currencyCodeCell.Value != DBNull.Value && !string.IsNullOrEmpty(currencyCodeCell.Value.ToString().Trim()) ? 
                currencyCodeCell.Value.ToString() : null;

            return ccyCode ?? GetApplicationCurrencyCodeOrDefault();
        }
        
        internal static string GetServiceCurrencyCodeOrDefault(SupplierSet.ServiceRow entity)
        {
            return GetServiceCurrencyCode(entity) ?? GetApplicationCurrencyCodeOrDefault();
        }

        internal static void SetUiCultureInfo()
        {
            if (GetApplicationCurrencyCode() == null) return; // nothing to do

            var siteCurrencyCode = GetApplicationCurrencyCode();
            var siteCurrencyOverride = CultureInfo.CreateSpecificCulture(siteCurrencyCode);
            if (Thread.CurrentThread.CurrentCulture.Name == siteCurrencyOverride.Name) return; // nothing to do

            var ciSystem = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            var nfSystem = ciSystem.NumberFormat;
            var nfOverride = siteCurrencyOverride.NumberFormat;

            // set the currency properties
            nfSystem.CurrencyDecimalDigits = nfOverride.CurrencyDecimalDigits;
            nfSystem.CurrencyDecimalSeparator = nfOverride.CurrencyDecimalSeparator;
            nfSystem.CurrencyGroupSeparator = nfOverride.CurrencyGroupSeparator;
            nfSystem.CurrencyGroupSizes = nfOverride.CurrencyGroupSizes;
            nfSystem.CurrencyNegativePattern = nfOverride.CurrencyNegativePattern;
            nfSystem.CurrencyPositivePattern = nfOverride.CurrencyPositivePattern;
            nfSystem.CurrencySymbol = nfOverride.CurrencySymbol;

            Thread.CurrentThread.CurrentCulture = ciSystem;
        }
        

        #region Ccy update service
        
        internal class Currency
        {
            public object Key { get; set; }
            public string FromCurrency { get; set; }
            public string ToCurrency { get; set; }
            public double Rate { get; set; }
            public string ErrorMessage { get; set; }
        }

        internal static List<Currency> GetRates(List<Currency> currencies)
        {
            // Start threads
            var threads = new List<Thread>();
            for (var i = 0; i < currencies.Count; i++) // GetRateHttp(currencies[i]);
            {
                var currency = currencies[i];
                var thread = new Thread(() => GetRateHttp(currency)) { Name = string.Format("Currency_GetRateHttp{0}", i) };
                thread.Start();
                threads.Add(thread);
            }
            // Wait for threads to finish
            foreach (var thread in threads)
            {
                thread.Join();
            }
            threads.Clear();
            return currencies;
        }

        internal static double? GetRate(string fromCurrency, string toCurrency)
        {
            if (fromCurrency == toCurrency || string.IsNullOrEmpty(fromCurrency.Trim()) || string.IsNullOrEmpty(toCurrency.Trim()))
                return null;
            return GetRateHttp(fromCurrency, toCurrency);
        }

        private static Currency GetRateHttp(Currency currency)
        {
            try
            {
                var rate = GetRateHttp(currency.FromCurrency, currency.ToCurrency);
                if (rate > 100000) 
                    currency.ErrorMessage = "Rate " + rate + " does not appear to be correct.";
                else 
                    currency.Rate = rate;
            }
            catch (Exception ex)
            {
                // load error to the message field
                currency.ErrorMessage = ex.Message;
                ErrorHelper.SendEmail(ex, true);
            }
            return currency;
        }

        private static double GetRateHttp(string fromCurrency, string toCurrency)
        {
            if (fromCurrency.ToLower().Trim() == toCurrency.ToLower().Trim()) return 1;

            const string url = "http://finance.yahoo.com/d/quotes.csv?s={0}{1}=X&f=l1&e=.csv";

            double result;
            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; .NET CLR 2.0.50727;)");
            var response = client.DownloadString(string.Format(url, fromCurrency, toCurrency));

            App.Debug(string.Format("Called currency service: {0} -> {1} = {2}",fromCurrency,toCurrency,response));
            return double.TryParse(response, out result) ? result : 0;
        }

        #endregion
    }
}
