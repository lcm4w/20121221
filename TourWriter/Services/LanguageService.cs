using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TourWriter.Global;
using TourWriter.Info;

namespace TourWriter.Services
{
    public static class LanguageService
    {
        private static List<Language> _languages;
        public static List<Language> Languages
        {
            get { return _languages ?? (_languages = BuildLanguagesList()); }
        }

        public static BindingList<Language> GetBindingList()
        {
            var bl = new BindingList<Language>(Languages);
            bl.Insert(0, new Language());
            return bl;
        }

        public static Language GetSystemLanguage()
        {
            var hasSystemOverride = !Cache.ToolSet.AppSettings[0].IsLanguageCodeNull() &&
                                    !string.IsNullOrEmpty(Cache.ToolSet.AppSettings[0].LanguageCode.Trim());

            var languageCode = hasSystemOverride
                                   ? Cache.ToolSet.AppSettings[0].LanguageCode.Trim()
                                   : Thread.CurrentThread.CurrentCulture.Name;

            return Languages.Where(l => l.LanguageCode == languageCode).FirstOrDefault();
        }

        public static Language GetItineraryLanguage(ItinerarySet itinerarySet)
        {
            var hasOverride = itinerarySet != null &&
                              itinerarySet.Itinerary.Count > 0 &&
                              !itinerarySet.Itinerary[0].IsLanguageCodeNull() &&
                              !string.IsNullOrEmpty(itinerarySet.Itinerary[0].LanguageCode.Trim());

            return hasOverride ? GetLanguage(itinerarySet.Itinerary[0].LanguageCode.Trim()) : GetSystemLanguage();
        }

        internal static Language GetBookingLanguage(ItinerarySet.PurchaseItemRow purchaseItem)
        {
            var hasOverride = purchaseItem != null &&
                              !purchaseItem.IsLanguageCodeNull() &&
                              !string.IsNullOrEmpty(purchaseItem.LanguageCode.Trim());

            return hasOverride ? GetLanguage(purchaseItem.LanguageCode.Trim()) : GetSystemLanguage();
        }

        public static Language GetLanguage(string languageCode)
        {
            return Languages.Where(x => x.LanguageCode == languageCode).FirstOrDefault();
        }

        public static void SetSystemCurrencySymbol()
        {
            var ci = GetSystemLanguage().CultureInfo;
            if (ci.Name == Thread.CurrentThread.CurrentCulture.Name) return; // nothing to do

            var clone = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            clone.NumberFormat.CurrencySymbol = ci.NumberFormat.CurrencySymbol;
            Thread.CurrentThread.CurrentCulture = clone;
        }
        
        private static List<Language> BuildLanguagesList()
        {
            var list = new List<Language>(
                CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(
                    x => new { Culture = x, Region = new RegionInfo(x.LCID) }).
                    Select(x =>
                           new Language
                           {
                               CountryName = x.Region.EnglishName,
                               LanguageName = Regex.Match(x.Culture.EnglishName, @".*(?= \()").Value,
                               LanguageCode = x.Culture.Name,
                               CurrencyCode = x.Region.ISOCurrencySymbol,
                               CurrencySymbol = x.Region.CurrencySymbol,
                               CurrencyName = x.Region.CurrencyEnglishName,
                               CultureInfo = x.Culture,
                               RegionInfo = x.Region,
                           })
                );

            // add French Republic pacific countries
            var xpf = list.Where(x => x.LanguageCode == "fr-FR").FirstOrDefault();
            var xpfCi = (CultureInfo)xpf.CultureInfo.Clone();
            xpfCi.NumberFormat.CurrencySymbol = "F";

            list.Add(new Language
            {
                CountryName = "French Polynesia",
                LanguageName = "French",
                LanguageCode = "fr-PF",
                CurrencyName = "CFP franc",
                CurrencyCode = "XPF",
                CurrencySymbol = "F",
                CultureInfo = xpfCi,
                RegionInfo = xpf.RegionInfo,
            });
            list.Add(new Language
            {
                CountryName = "New Caledonia",
                LanguageName = "French",
                LanguageCode = "fr-NC",
                CurrencyName = "CFP franc",
                CurrencyCode = "XPF",
                CurrencySymbol = "F",
                CultureInfo = xpfCi,
                RegionInfo = xpf.RegionInfo,
            });

            return list.OrderBy(x => x.CountryName).ToList();
        }
    }
}