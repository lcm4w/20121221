using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using TourWriter.Global;

namespace TourWriter.Services
{
    class CurrencyUpdateService
    {
        private const string updateUri =
            @"http://www.webservicex.net/CurrencyConvertor.asmx/ConversionRate?FromCurrency={0}&ToCurrency={1}";

        internal class Currency
        {
            public object Key { get; set; }
            public string FromCurrency { get; set; }
            public string ToCurrency { get; set; }
            public double Rate { get; set; }
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// Gets the currency code from the AppSettings if it exists, otherwise gets it from the region info.
        /// </summary>
        internal static string GetLocalCurrencyCode()
        {
            string currencyCode;
            if (!Cache.ToolSet.AppSettings[0].IsCurrencyCodeNull())
            {
                currencyCode = Cache.ToolSet.AppSettings[0].CurrencyCode;
            }
            else
            {
                RegionInfo regionInfo = new RegionInfo(CultureInfo.CurrentCulture.LCID);
                currencyCode = regionInfo.ISOCurrencySymbol;
            }
            return currencyCode;
        }

        /// <summary>
        /// Get the currency exchange rates for a list of currencies, from a currency webservice.
        /// Usage:
        ///    var currencies = new List<CurrencyUpdateService.Currency>();
        ///    currencies.Add(new CurrencyUpdateService.Currency { FromCurrency = "NZD", ToCurrency = "AUD" });
        ///    CurrencyUpdateService.GetRates(currencies);
        ///    foreach(var c in currencies)
        ///        Console.WriteLine(c.Rate);
        /// </summary>
        /// <param name="currencies">List of currency objects with from and to currency codes.</param>
        /// <returns>List of currency objects with updated exchange rates.</returns>
        internal static List<Currency> GetRates(List<Currency> currencies)
        {
            // Start threads
            var threads = new List<Thread>();
            for (int i = 0; i < currencies.Count; i++)
            {
                var currency = currencies[i];
                var thread = new Thread(() => GetRateHttp(currency)) {Name = string.Format("Currency_GetRateHttp{0}", i)};
                thread.Start();
                threads.Add(thread);
            }
            // Wait for threads to finish
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            threads.Clear();
            return currencies;
        }

        /// <summary>
        /// Get a currency exchange rate.
        /// </summary>
        /// <param name="fromCurrency">The currency to convert from.</param>
        /// <param name="toCurrency">The currency to convert to.</param>
        /// <returns>The currency rate.</returns>
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
            if (App.IsDebugMode ||
                Cache.ToolSet.AppSettings.Rows.Count > 0 && Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "13b8e136-405f-402f-a4bb-3913879be702".ToLower() || // dev
                Cache.ToolSet.AppSettings.Rows.Count > 0 && Cache.ToolSet.AppSettings[0].InstallID.ToString().ToLower() == "575E7900-BF13-42D1-A661-2242510C3359".ToLower()) // te
                return GetRateHttpTest(fromCurrency, toCurrency);

            string request = string.Format(updateUri, fromCurrency, toCurrency);
            string response = new WebClient().DownloadString(request);

            var reg = Regex.Match(response, @"[0-9]+\.[0-9]+(?=<\/double>)");

            double result;
            if (double.TryParse(reg.Value, NumberStyles.Any, CultureInfo.CreateSpecificCulture("en-US"), out result))
                return result;

            return 0;
        }

        private static Currency GetRateSoap(Currency currency)
        {
            try
            {
                currency.Rate = GetRateSoap(currency.FromCurrency, currency.ToCurrency);
            }
            catch (Exception ex)
            {
                // load error to the message field
                currency.ErrorMessage = ex.ToString();
            }
            return currency;
        }

        private static double GetRateSoap(string fromCurrency, string toCurrency)
        {
            var client = new CurrencyConvertorClient();
            return client.ConversionRate(fromCurrency, toCurrency);
        }

        [WebServiceBinding(Name = "CurrencyConvertorSoap", Namespace = "http://www.webserviceX.NET/")]
        private class CurrencyConvertorClient : SoapHttpClientProtocol
        {
            public CurrencyConvertorClient()
            {
                Url = "http://www.webservicex.net/CurrencyConvertor.asmx";
            }

            [SoapDocumentMethodAttribute("http://www.webserviceX.NET/ConversionRate", RequestNamespace = "http://www.webserviceX.NET/", ResponseNamespace = "http://www.webserviceX.NET/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
            public double ConversionRate(string FromCurrency, string ToCurrency)
            {
                object[] results = Invoke("ConversionRate", new object[] { FromCurrency, ToCurrency });
                return ((double)(results[0]));
            }
        }

        private static double GetRateHttpTest(string fromCurrency, string toCurrency)
        {
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
