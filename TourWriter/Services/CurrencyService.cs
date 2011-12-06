using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
        
        internal static string GetServiceCurrencyCodeOrDefault(SupplierSet.ServiceRow entity)
        {
            return GetServiceCurrencyCode(entity) ?? GetApplicationCurrencyCodeOrDefault();
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
        
        internal static string GetItineraryPaymentCurrencyCodeOrDefault(ItinerarySet.ItineraryPaymentRow entity)
        {
            if (entity == null) return null;
            return !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ?
                entity.CurrencyCode : GetItineraryCurrencyCodeOrDefault(entity.ItineraryMemberRow.ItineraryGroupRow.ItineraryRow);
        }

        internal static void SetUiCultureInfo()
        {
            var hasOverride = !Cache.ToolSet.AppSettings[0].IsLanguageCodeNull() && !string.IsNullOrEmpty(Cache.ToolSet.AppSettings[0].LanguageCode.Trim());
            var siteLanguageCode = hasOverride ? Cache.ToolSet.AppSettings[0].LanguageCode.Trim() : Thread.CurrentThread.CurrentCulture.Name;
            var ciOverride = CultureInfo.CreateSpecificCulture(siteLanguageCode);
            var nfOverride = ciOverride.NumberFormat;
            
            if (nfOverride == CultureInfo.CurrentCulture.NumberFormat) 
                return; // nothing to do

            var ciClone = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            var nfClone = ciClone.NumberFormat;

            // set the currency properties
            nfClone.CurrencyDecimalDigits = nfOverride.CurrencyDecimalDigits;
            nfClone.CurrencyDecimalSeparator = nfOverride.CurrencyDecimalSeparator;
            nfClone.CurrencyGroupSeparator = nfOverride.CurrencyGroupSeparator;
            nfClone.CurrencyGroupSizes = nfOverride.CurrencyGroupSizes;
            nfClone.CurrencyNegativePattern = nfOverride.CurrencyNegativePattern;
            nfClone.CurrencyPositivePattern = nfOverride.CurrencyPositivePattern;
            nfClone.CurrencySymbol = nfOverride.CurrencySymbol;

            Thread.CurrentThread.CurrentCulture = ciClone;
        }
        

        #region Ccy update service

        internal class Currency
        {
            public object Key { get; set; }
            public string FromCurrency { get; set; }
            public string ToCurrency { get; set; }
            public decimal Rate { get; set; }
            public string ErrorMessage { get; set; }
        }

        internal static List<Currency> GetRates(List<Currency> currencies, string ccyService)
        {
            // Start threads
            var threads = new List<Thread>();
            for (var i = 0; i < currencies.Count; i++) // GetRateHttp(currencies[i]);
            {
                var currency = currencies[i];
                var thread = new Thread(() => GetRateHttp(currency, ccyService)) { Name = string.Format("Currency_GetRateHttp{0}", i) };
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
        
        //internal static decimal? GetRate(string fromCurrency, string toCurrency)
        //{
        //    return GetRate(fromCurrency, toCurrency, App.UseGoogleCcyService ? ServiceTypes.Google : ServiceTypes.Yahoo);
        //}

        internal static decimal? GetRate(string fromCurrency, string toCurrency, string ccyService)
        {
            if (fromCurrency == toCurrency || string.IsNullOrEmpty(fromCurrency.Trim()) || string.IsNullOrEmpty(toCurrency.Trim()))
                return null;
            return GetRateHttp(fromCurrency, toCurrency, ccyService);
        }

        private static Currency GetRateHttp(Currency currency, string ccyService)
        {
            try
            {
                var rate = GetRateHttp(currency.FromCurrency, currency.ToCurrency, ccyService);
                if (rate > 100000) 
                    currency.ErrorMessage = "Rate " + rate + " does not appear to be correct.";
                else 
                    currency.Rate = rate;
            }
            catch (Exception ex)
            {
                // load error to the message field
                currency.ErrorMessage = "No available: " + ex.Message;
                ErrorHelper.SendEmail(ex, true);
            }
            return currency;
        }

        private static decimal GetRateHttp(string fromCurrency, string toCurrency, string ccyService)
        {
            if (fromCurrency.ToLower().Trim() == toCurrency.ToLower().Trim()) 
                return 1;

            var d = ccyService == "google" ? 
                GetCcyGoogle(fromCurrency, toCurrency) : 
                GetCcyYahoo(fromCurrency, toCurrency);

            return (d.HasValue) ? (decimal)d : 0;
        }

        private static decimal? GetCcyGoogle(string from, string to)
        {
            // alternative? but returns html page: http://www.google.com/finance/converter?a=379.00&from=USD&to=NZD
            
            const string url = "http://www.google.com/ig/calculator?hl=en&q=1{0}=?{1}";
            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");

            var s = string.Format(url, from, to);
            var response = client.DownloadString(s).Trim();

            WriteDebug("CCY Reqeust: " + s);
            WriteDebug("CCY Response: " + response);

            /***************************************************************************************************************************************
             * call: http://www.google.com/ig/calculator?hl=en&q=1VND=?GBP
             * response: {lhs: \"1 Vietnamese dong\",rhs: \"3.03030303 \x26#215; 10\x3csup\x3e-5\x3c/sup\x3e British pounds\",error: \"\",icc: true}
             * where: 3.03030303 \x26#215; 10\x3csup\x3e-5\x3c/sup\x3e
             * equals: 3.03030303 x 10^-5
             * eg: \x26#215;    = X
             *     \x3csup\x3e  = ^
             *     \x3c/sup\x3e = blank (maybe end of value minus word value at the end) 
             ***************************************************************************************************************************************/

            var regex = new Regex("^{lhs:.\"([^\"]*)\",rhs:.\"([^\"]*)\".*}$", RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            var results = regex.Split(response);
            var lhs = results[1];
            var rhs = results[2];

            regex = new Regex("^(\\d+\\.?\\d*)\\s(\\\\x26#215;\\s10\\\\x3csup\\\\x3e(.*)\\\\x3c/sup\\\\x3e)?.*$", RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            // value on left side
            //results = regex.Split(lhs);
            //var d1 = Decimal.Parse(results[1]);

            // value on right side
            results = regex.Split(rhs);
            decimal result;
            if (results.Count() > 3)
            {   // handle exponent
                var d = Decimal.Parse(results[1]);
                var exp = int.Parse(results[3]);
                var pow = Math.Pow(10, exp);
                result = d * Convert.ToDecimal(pow);
            }
            else result = Decimal.Parse(results[1]);

            WriteDebug(string.Format("CCY Result: {0} > {1} = {2}", from, to, result));
            return result;
        }

        private static decimal? GetCcyYahoo(string from, string to)
        {
            const string url = "http://finance.yahoo.com/d/quotes.csv?s={0}{1}=X&f=l1&e=.csv";
            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");

            var s = string.Format(url, from, to);
            var response = client.DownloadString(s).Trim();

            WriteDebug("CCY Reqeust: " + s);
            WriteDebug("CCY Response: " + response);

            decimal result;
            decimal.TryParse(response, out result);
            WriteDebug(string.Format("CCY Result: {0} > {1} = {2}", from, to, result));
            return result;
        }

        public static void WriteDebug(string text)
        {
            //if (!App.IsDebugMode) return;
            //var file = Path.Combine(App.TempFolder, "ccy_test.txt");
            //using (var sw = new StreamWriter(file, true)) sw.WriteLine(text);
        }
        #endregion
    }
}