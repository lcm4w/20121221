using System.Globalization;

namespace TourWriter.Info
{
    public class Language
    {
        public string CountryName { get; set; }
        public string LanguageName { get; set; }
        public string LanguageCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public RegionInfo RegionInfo { get; set; }

        public override string ToString() { return string.Format("{0} ({1})", CountryName, LanguageName); }
    }
}