using System.Collections.Generic;
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
        public static string SystemCurrencyCode
        {
            get
            {
                var systemCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(x => x.Name == SystemLanguageCode).FirstOrDefault();
                var systemRegionInfo = new RegionInfo(systemCultureInfo.LCID);
                return systemRegionInfo.ISOCurrencySymbol;
            }
        }

        public static string SystemLanguageCode
        {
            get
            {
                var hasLanguageOverride = !Cache.ToolSet.AppSettings[0].IsLanguageCodeNull() &&
                                          !string.IsNullOrEmpty(Cache.ToolSet.AppSettings[0].LanguageCode.Trim());

                return hasLanguageOverride
                           ? Cache.ToolSet.AppSettings[0].LanguageCode.Trim()
                           : Thread.CurrentThread.CurrentCulture.Name;
            }
        }

        public static List<Language> Languages
        {
            get
            {
                return
                    new List<Language>(
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
                                   }).OrderBy(x => x.CountryName)
                        );
            }
        }

        internal static void SetSystemCurrencySymbol()
        {
            var ci = CultureInfo.GetCultureInfo(SystemLanguageCode);
            if (ci.Name == Thread.CurrentThread.CurrentCulture.Name) return; // nothing to do

            var clone = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            clone.NumberFormat.CurrencySymbol = ci.NumberFormat.CurrencySymbol;
            Thread.CurrentThread.CurrentCulture = clone;
        }
    }
}
