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

        public string FriendlyName { get { return string.Format("{0} ({1})", CountryName, LanguageName).Replace("()", ""); } }
        public string FriendlyCurrencyName { get { return string.Format("{0}, {1}, {2}", FriendlyName, CurrencyCode, CurrencySymbol).Replace(", ,", ""); } }

        public override string ToString() { return FriendlyName; }
    }
}