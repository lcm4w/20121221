using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
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

        /// <summary>
        /// Gets the base Itinerary, TourWriter, or user currency
        /// </summary>
        internal static string GetBaseCurrencyCode(ItinerarySet.ItineraryRow itinerary = null)
        {
            // itinerary base currency
            if (itinerary != null && !itinerary.IsBaseCurrencyNull()) return itinerary.BaseCurrency;

            // TourWriter base currency
            if (!Cache.ToolSet.AppSettings[0].IsCurrencyCodeNull()) return Cache.ToolSet.AppSettings[0].CurrencyCode;
            
            // computer base currency
            return new RegionInfo(CultureInfo.CurrentCulture.LCID).ISOCurrencySymbol;
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
