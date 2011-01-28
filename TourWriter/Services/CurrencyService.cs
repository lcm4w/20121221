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
        internal class Currency
        {
            public object Key { get; set; }
            public string FromCurrency { get; set; }
            public string ToCurrency { get; set; }
            public double Rate { get; set; }
            public string ErrorMessage { get; set; }
        }
        

        internal static string GetSystemCurrencyCode()
        {
            return !Cache.ToolSet.AppSettings[0].IsCurrencyCodeNull() ? 
                Cache.ToolSet.AppSettings[0].CurrencyCode :                         // db setting
                new RegionInfo(CultureInfo.CurrentCulture.LCID).ISOCurrencySymbol;  // user computer settings
        }
        
        internal static string GetItineraryCurrencyCode(ItinerarySet itinerarySet)
        {
            if (itinerarySet != null && itinerarySet.Itinerary.Count > 0 && !itinerarySet.Itinerary[0].IsCurrencyCodeNull())
                return itinerarySet.Itinerary[0].CurrencyCode;
            return GetSystemCurrencyCode();
        }

        internal static string GetBookingCurrencyCode(ItinerarySet.PurchaseItemRow purchaseItem)
        {
            if (purchaseItem != null && !purchaseItem.IsCurrencyCodeNull())
                return purchaseItem.CurrencyCode;
            return GetSystemCurrencyCode();
        }

        internal static string GetBookingCurrencyCode(UltraGridCell currencyCell)
        {
            if (currencyCell.Value != DBNull.Value && !string.IsNullOrEmpty(currencyCell.Value.ToString().Trim()))
                return currencyCell.Value.ToString();
            return GetSystemCurrencyCode();
        }

        internal static CultureInfo GetCultureInfoFromCultureCode(string cultureCode)
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(x => x.Name == cultureCode).FirstOrDefault();
        }

        internal static CultureInfo GetCultureInfoFromCurrencyCode(string currencyCode)
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).
                Where(cc => new RegionInfo(cc.LCID).ISOCurrencySymbol == currencyCode).FirstOrDefault();
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
                currency.Rate = GetRateHttp(currency.FromCurrency, currency.ToCurrency);
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
    }
}
