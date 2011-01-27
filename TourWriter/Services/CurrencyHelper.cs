using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TourWriter.Global;
using TourWriter.Services;

namespace TourWriter.Services
{
    public partial class Currencies
    {
        internal static string GetComputerCurrencyCode()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).
                Where(x => x.Name == Thread.CurrentThread.CurrentCulture.Name).
                Select(x => new RegionInfo(x.LCID).ISOCurrencySymbol).
                FirstOrDefault();
        }


        internal static string GetSiteCurrencyCode()
        {
            var entity = Cache.ToolSet.AppSettings[0];

            return !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ? entity.CurrencyCode : null;
        }

        internal static string GetServiceCurrencyCode(Info.SupplierSet.ServiceRow entity)
        {
            return !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ? entity.CurrencyCode : null;
        }
        
        internal static string GetItineraryCurrencyCode(Info.ItinerarySet.ItineraryRow entity)
        {
            return !entity.IsBaseCurrencyNull() && !string.IsNullOrEmpty(entity.BaseCurrency.Trim()) ? entity.BaseCurrency : null;
        }
        
        internal static string GetPurchaseItemCurrencyCode(Info.ItinerarySet.PurchaseItemRow entity)
        {
            return !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ? entity.CurrencyCode : null;
        }
        

        internal static string GetSiteCurrencyCodeOrDefault()
        {
            return GetSiteCurrencyCode() ?? GetComputerCurrencyCode();
        }

        internal static string GetServiceCurrencyCodeOrDefault(Info.SupplierSet.ServiceRow entity)
        {
            return GetServiceCurrencyCode(entity) ?? GetSiteCurrencyCodeOrDefault();
        }

        internal static string GetItineraryCurrencyCodeOrDefault(Info.ItinerarySet.ItineraryRow entity)
        {
            return GetItineraryCurrencyCode(entity) ?? GetSiteCurrencyCodeOrDefault();
        }

        internal static string GetPurchaseItemCurrencyCodeOrDefault(Info.ItinerarySet.PurchaseItemRow entity)
        {
            return GetPurchaseItemCurrencyCode(entity) ?? GetSiteCurrencyCodeOrDefault();
        }


        internal static void SetApplicationBaseCurrency()
        {
            if (GetSiteCurrencyCode() == null) return; // nothing to do

            var siteCurrencyCode = GetSiteCurrencyCode();
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
    }
}

namespace System
{
    public static class DecimalCurrencyHelper
    {
        public static string ToLocalCurrencyString(this decimal value, string currencyCode)
        {
            return Currencies.Single(currencyCode).FormatLocalPrice(value);
        }

        public static string ToPortableCurrencyString(this decimal value, string currencyCode)
        {
            return Currencies.Single(currencyCode).FormatPortablePrice(value);
        }

        public static string ToGlobalCurrencyString(this decimal value, string currencyCode)
        {
            return Currencies.Single(currencyCode).FormatGlobalPrice(value);
        }
    }
}

#region Initial source code import
namespace TourWriter.Services
{
    public partial class Currencies
    {
        public static Currency Single(string currencyCode)
        {
            return CurrencyInfo.Where(x => x.CurrencyCode.ToUpper() == currencyCode.ToUpper()).FirstOrDefault();
        }

        /// <summary>
        /// Get all currencies (tier1 and tier2)
        /// </summary>
        public static List<Currency> All()
        {
            return CurrencyInfo.ToList();
        }

        /// <summary>
        /// Get the tier1 currencies
        /// </summary>
        public static List<Currency> AllTier1()
        {
            return CurrencyInfo.Where(x => x.IsTier1Currency).ToList();
        }

        #region Currency class

        public class Currency
        {
            private const int PrecisionMask = 0x07;
            private const int PositionFlag = 0x08;
            private const int SpaceFlag = 0x20;
            private readonly int _patternNumber;

            public Currency(string currencyCode, string currencyName, int patternNumber, string localSign, string portableSign, bool isTier1Currency)
            {
                CurrencyCode = currencyCode;
                CurrencyName = currencyName;
                _patternNumber = patternNumber;
                LocalCurrencySign = localSign;
                PortableCurrencySign = portableSign;
                IsTier1Currency = isTier1Currency;
            }

            public string CurrencyCode
            { get; private set; }

            public string CurrencyName
            { get; private set; }

            public string LocalCurrencySign
            { get; private set; }

            public string LocalCurrencyPattern
            {
                get { return GetCurrencyPattern(_patternNumber, LocalCurrencySign); }
            }

            public string PortableCurrencySign
            { get; private set; }

            public string PortableCurrencyPattern
            {
                get { return GetCurrencyPattern(_patternNumber, PortableCurrencySign); }
            }

            public string GlobalCurrencySign
            {
                get { return CurrencyCode == LocalCurrencySign ? CurrencyCode : CurrencyCode + " " + LocalCurrencySign; }
            }

            public string GlobalCurrencyPattern
            {
                get
                {
                    if (CurrencyCode == LocalCurrencySign)
                    {
                        var patternNum = _patternNumber;
                        if ((patternNum & PositionFlag) == 0)
                            patternNum = patternNum | SpaceFlag;

                        return GetCurrencyPattern(patternNum, LocalCurrencySign);
                    }
                    return CurrencyCode + " " + GetCurrencyPattern(_patternNumber, LocalCurrencySign);
                }
            }

            public string FormatLocalPrice(decimal? price)
            {
                return string.Format("{0:" + LocalCurrencyPattern + "}", price);
            }

            public string FormatPortablePrice(decimal? price)
            {
                return string.Format("{0:" + PortableCurrencyPattern + "}", price);
            }

            public string FormatGlobalPrice(decimal? price)
            {
                return string.Format("{0:" + GlobalCurrencyPattern + "}", price);
            }

            public bool IsPrefixSignPosition
            {
                get { return (_patternNumber & PositionFlag) == 0; }
            }

            public bool IsTier1Currency
            { get; private set; }

            public override string ToString()
            {
                return string.Format("{0}, {1}", CurrencyCode, CurrencyName);
            }

            private static string GetCurrencyPattern(int patternNum, string sign)
            {
                var strParts = new List<string> { "#,##0" };
                var precision = patternNum & PrecisionMask;
                if (precision > 0)
                {
                    strParts.Add(".");
                    for (var i = 0; i < precision; i++)
                    {
                        strParts.Add("0");
                    }
                }
                if ((patternNum & PositionFlag) == 0)
                {
                    strParts.Insert(0, (patternNum & SpaceFlag) == 1 ? "' " : "'");
                    strParts.Insert(0, sign);
                    strParts.Insert(0, "'");
                }
                else
                {
                    strParts.AddRange(new[] { (patternNum & SpaceFlag) == 1 ? " '" : "'", sign, "'" });
                }
                return string.Join("", strParts.ToArray());
            }
        }
        #endregion

        #region Currencies data

        private static readonly List<Currency> CurrencyInfo =
            new List<Currency>
                {
// tier 1
new Currency("AED", "UAE Dirham", 2, "\u062F\u002e\u0625", "DH", true),
new Currency("ARS", "Argentine Peso", 2, "$", "AR$", true),
new Currency("AUD", "Australian Dollar", 2, "$", "AU$", true),
new Currency("BDT", "Taka", 2, "\u09F3", "Tk", true),
new Currency("BRL", "Brazilian Real", 2, "R$", "R$", true),
new Currency("CAD", "Canadian Dollar", 2, "$", "C$", true),
new Currency("CHF", "Swiss Franc", 2, "Fr.", "CHF", true),
new Currency("CLP", "Chilean Peso", 0, "$", "CL$", true),
new Currency("CNY", "Yuan Renminbi", 2, "¥", "RMB¥", true),
new Currency("COP", "Colombian Peso", 2, "$", "COL$", true),
new Currency("CRC", "Costa Rican Colon", 2, "\u20a1", "CR₡", true),
new Currency("CUP", "Cuban Peso", 2, "$", "$MN", true),
new Currency("CZK", "Czech Koruna", 10, "Kč", "Kč", true),
new Currency("DKK", "Danish Krone", 26, "kr", "kr", true),
new Currency("DOP", "Dominican Peso", 2, "$", "RD$", true),
new Currency("EGP", "Egyptian Pound", 2, "£", "LE", true),
new Currency("EUR", "Euro", 26, "€", "€", true),
new Currency("GBP", "Pound Sterling", 2, "£", "GB£", true),
new Currency("HKD", "Hong Kong Dollar", 2, "$", "HK$", true),
new Currency("ILS", "New Israeli Sheqel", 10, "\u20AA", "IL₪", true),
new Currency("INR", "Indian Rupee", 2, "Rs", "Rs", true),
new Currency("ISK", "Iceland Krona", 10, "kr", "kr", true),
new Currency("JMD", "Jamaican Dollar", 2, "$", "JA$", true),
new Currency("JPY", "Yen", 0, "¥", "JP¥", true),
new Currency("KRW", "Won", 0, "\u20A9", "KR₩", true),
new Currency("LKR", "Sri Lanka Rupee", 2, "Rs", "SLRs", true),
new Currency("MNT", "Tugrik", 2, "\u20AE", "MN₮", true),
new Currency("MXN", "Mexican Peso", 2, "$", "Mex$", true),
new Currency("MYR", "Malaysian Ringgit", 2, "RM", "RM", true),
new Currency("NOK", "Norwegian Krone", 26, "kr", "NOkr", true),
new Currency("PAB", "Balboa", 2, "B/.", "B/.", true),
new Currency("PEN", "Nuevo Sol", 2, "S/.", "S/.", true),
new Currency("PHP", "Philippine Peso", 2, "P", "PHP", true),
new Currency("PKR", "Pakistan Rupee", 2, "Rs.", "PKRs.", true),
new Currency("RUB", "Russian Ruble", 10, "руб", "руб", true),
new Currency("SAR", "Saudi Riyal", 2, "\u0633\u002E\u0631", "SR", true),
new Currency("SEK", "Swedish Krona", 10, "kr", "kr", true),
new Currency("SGD", "Singapore Dollar", 2, "$", "S$", true),
new Currency("THB", "Baht", 2, "\u0e3f", "THB", true),
new Currency("TRY", "new Turkish Lira", 2, "YTL", "YTL", true),
new Currency("TWD", "New Taiwan Dollar", 2, "NT$", "NT$", true),
new Currency("USD", "US Dollar", 2, "$", "US$", true),
new Currency("UYU", "Peso Uruguayo", 2, "$", "UY$", true),
new Currency("VND", "Dong", 10, "\u20AB", "VN₫", true),
new Currency("YER", "Yemeni Rial", 2, "YER", "YER", true),
new Currency("ZAR", "Rand", 2, "R", "ZAR", true),

//tier 2
new Currency("AFN", "Afghani", 18, "\u060b", "AFN", false),
new Currency("ALL", "Lek", 2, "Lek", "Lek", false),
new Currency("AMD", "Armenian Dram", 10, "\u0564\u0580\u002e", "dram", false),
new Currency("ANG", "Netherlands Antillian Guikder", 2, "\u0083", "NAƒ", false),
new Currency("AOA", "Kwanza", 2, "Kz", "Kz", false),
new Currency("AWG", "Aruban Guilder", 2, "ƒ", "Afl.", false),
new Currency("AZN", "Azerbaijanian Manat", 2, "m", "man", false),
new Currency("BAM", "Convertible Marks", 18, "КМ", "KM", false),
new Currency("BBD", "Barbados Dollar", 2, "$", "Bds$", false),
new Currency("BGN", "Bulgarian Lev", 10, "\u043b\u0432", "лв", false),
new Currency("BHD", "Bahraini Dinar", 3, "\u0628\u002e\u062f\u002e", "BD", false),
new Currency("BIF", "Burundi Franc", 0, "FBu", "FBu", false),
new Currency("BMD", "Bermudian Dollar (customarily known as Bermuda Dollar)", 2, "$", "BD$", false),
new Currency("BND", "Brunei Dollar", 2, "$", "B$", false),
new Currency("BOB", "Boliviano", 2, "B$", "B$", false),
new Currency("BSD", "Bahamian Dollar", 2, "$", "B$", false),
new Currency("BTN", "Ngultrum", 2, "Nu.", "Nu.", false),
new Currency("BWP", "Pula", 2, "P", "pula", false),
new Currency("BYR", "Belarussian Ruble", 0, "Br", "Br", false),
new Currency("BZD", "Belize Dollar", 2, "$", "BZ$", false),
new Currency("CDF", "Franc Congolais", 2, "F", "CDF", false),
new Currency("CVE", "Cape Verde Escudo", 2, "$", "Esc", false),
new Currency("DJF", "Djibouti Franc", 0, "Fdj", "Fdj", false),
new Currency("DZD", "Algerian Dinar", 2, "\u062f\u062C", "DA", false),
new Currency("EEK", "Kroon", 10, "EEK", "EEK", false),
new Currency("ERN", "Nakfa", 2, "Nfk", "Nfk", false),
new Currency("ETB", "Ethiopian Birr", 2, "Br", "Br", false),
new Currency("FJD", "Fiji Dollar", 2, "$", "FJ$", false),
new Currency("FKP", "Falkland Islands Pound", 2, "£", "FK£", false),
new Currency("GEL", "Lari", 2, "GEL", "GEL", false),
new Currency("GHS", "Cedi", 2, "\u20B5", "GHS¢", false),
new Currency("GIP", "Gibraltar Pound", 2, "£", "GI£", false),
new Currency("GMD", "Dalasi", 2, "D", "GMD", false),
new Currency("GNF", "Guinea Franc", 0, "FG", "FG", false),
new Currency("GTQ", "Quetzal", 2, "Q", "GTQ", false),
new Currency("GYD", "Guyana Dollar", 2, "$", "GY$", false),
new Currency("HNL", "Lempira", 2, "L", "HNL", false),
new Currency("HRK", "Croatian Kuna", 2, "kn", "kn", false),
new Currency("HTG", "Gourde", 2, "G", "HTG", false),
new Currency("HUF", "Forint", 10, "Ft", "Ft", false),
new Currency("IDR", "Rupiah", 2, "Rp", "Rp", false),
new Currency("IQD", "Iraqi Dinar", 3, "\u0639\u062F", "IQD", false),
new Currency("IRR", "Iranian Rial", 2, "\ufdfc", "IRR", false),
new Currency("JOD", "Jordanian Dinar", 3, "JOD", "JOD", false),
new Currency("KES", "Kenyan Shilling", 2, "KSh", "KSh", false),
new Currency("KGS", "Som", 2, "som", "som", false),
new Currency("KHR", "Riel", 10, "\u17DB", "KHR", false),
new Currency("KMF", "Comoro Franc", 0, "KMF", "KMF", false),
new Currency("KPW", "North Korean Won", 2, "\u20A9", "KPW", false),
new Currency("KWD", "Kuwaiti Dinar", 3, "\u062F\u002e\u0643", "KWD", false),
new Currency("KYD", "Cayman Islands Dollar", 2, "$", "CI$", false),
new Currency("KZT", "Tenge", 10, "KZT", "KZT", false),
new Currency("LAK", "Kip", 2, "\u20AD", "LA₭", false),
new Currency("LBP", "Lebanese Pound", 2, "\u0644\u002e\u0644", "LBP", false),
new Currency("LRD", "Liberian Dollar", 2, "$", "L$", false),
new Currency("LSL", "Loti", 2, "L", "LSL", false),
new Currency("LTL", "Lithuanian Litas", 10, "Lt", "Lt", false),
new Currency("LVL", "Latvian Lats", 10, "Ls", "Ls", false),
new Currency("LYD", "Libyan Dinar", 3, "\u0644\u002e\u062F", "LD", false),
new Currency("MAD", "Moroccan Dirham", 2, "\u0645\u002E\u062F\u002E", "MAD", false),
new Currency("MDL", "Moldovan Leu", 2, "MDL", "MDL", false),
new Currency("MGA", "Malagasy Ariary", 1, "MGA", "MGA", false),
new Currency("MKD", "Denar", 2, "MKD", "MKD", false),
new Currency("MMK", "Kyat", 2, "K", "MMK", false),
new Currency("MOP", "Pataca", 2, "MOP$", "MOP$", false),
new Currency("MRO", "Ouguiya", 1, "UM", "UM", false),
new Currency("MUR", "Mauritius Rupee", 2, "Rs", "MURs", false),
new Currency("MVR", "Rufiyaa", 2, "Rf", "MRF", false),
new Currency("MWK", "Kwacha", 2, "MK", "MK", false),
new Currency("MZN", "Merical", 2, "MTn", "MTn", false),
new Currency("NAD", "Namibian Dollar", 2, "$", "N$", false),
new Currency("NGN", "Naira", 2, "\u20A6", "NG₦", false),
new Currency("NIO", "Cordoba Oro", 2, "C$", "C$", false),
new Currency("NPR", "Nepalese Rupee", 2, "Rs", "NPRs", false),
new Currency("NZD", "New Zealand Dollar", 2, "$", "NZ$", false),
new Currency("OMR", "Rial Omani", 3, "\u0639\u002E\u062F\u002E", "OMR", false),
new Currency("PGK", "Kina", 2, "K", "PGK", false),
new Currency("PLN", "Zloty", 10, "zł", "zł", false),
new Currency("PYG", "Guarani", 0, "\u20b2", "PYG", false),
new Currency("QAR", "Qatari Rial", 2, "\u0642\u002E\u0631", "QR", false),
new Currency("RON", "Leu", 2, "L", "RON", false),
new Currency("RSD", "Serbian Dinar", 2, "РС\u0414", "RSD", false),
new Currency("RWF", "Rwanda Franc", 0, "RF", "RF", false),
new Currency("SBD", "Solomon Islands Dollar", 2, "$", "SI$", false),
new Currency("SCR", "Seychelles Rupee", 2, "SR", "SCR", false),
new Currency("SDG", "Sudanese Pound", 2, "SDG", "SDG", false),
new Currency("SHP", "Saint Helena Pound", 2, "£", "SH£", false),
new Currency("SKK", "Slovak Koruna", 10, "Sk", "Sk", false),
new Currency("SLL", "Leone", 2, "Le", "Le", false),
new Currency("SOS", "Somali Shilling", 2, "So. Sh.", "So. Sh.", false),
new Currency("SRD", "Surinam Dollar", 2, "$", "SR$", false),
new Currency("STD", "Dobra", 2, "Db", "Db", false),
new Currency("SYP", "Syrian Pound", 18, "SYP", "SYP", false),
new Currency("SZL", "Lilangeni", 2, "L", "SZL", false),
new Currency("TJS", "Somoni", 2, "TJS", "TJS", false),
new Currency("TMM", "Manat", 2, "m", "TMM", false),
new Currency("TND", "Tunisian Dinar", 3, "\u062F\u002e\u062A ", "DT", false),
new Currency("TOP", "Tongan Pa'anga", 2, "T$", "T$", false),
new Currency("TTD", "Trinidad and Tobago Dollar", 2, "$", "TT$", false),
new Currency("TZS", "Tanzanian Shilling", 10, "TZS", "TZS", false),
new Currency("UAH", "Hryvnia", 10, "\u20B4", "грн", false),
new Currency("UGX", "Uganda Shilling", 2, "USh", "USh", false),
new Currency("UZS", "Uzbekistan Sum", 2, "UZS", "UZS", false),
new Currency("VEF", "Bolivar Fuerte", 2, "Bs.F", "Bs.F", false),
new Currency("VUV", "Vatu", 0, "Vt", "Vt", false),
new Currency("WST", "Tala", 2, "WS$", "WS$", false),
new Currency("XAF", "CFA Franc", 0, "FCFA", "FCFA", false),
new Currency("XCD", "East Caribbean Dollar", 2, "$", "EC$", false),
new Currency("XOF", "CFA Franc BCEAO †", 0, "CFA", "CFA", false),
new Currency("XPF", "CFP Franc", 0, "F", "XPF", false),
new Currency("ZMK", "Kwacha", 2, "ZK", "ZK", false),
new Currency("ZWL", "Zimbabwe Dollar", 2, "$", "ZW$", false)
                };

        #endregion
    }
}

namespace TourWriter.Services
{
    #region Currency Data Import

    /// <summary>
    /// Import currency data from 3rd party sources (google and unicode.org)
    /// </summary>
    class CurrencyImport
    {
        class Row { public string Code; public string Name; public int Pattern; public string Local; public string Portable; public bool IsTier1; }

        private static void ImportData()
        {
            var tier1 = ImportData(Tier1CurrencyData, true);
            var tier2 = ImportData(Tier2CurrencyData, false);
            string breakPoint;
        }

        private static string ImportData(string data, bool isTier1)
        {
            var list = new List<Row>();

            // insert currencies
            foreach (Match m in Regex.Matches(data, @"^\s*'([A-Z]+)': \[(\d+), '(.+)', '(.+)'],?", RegexOptions.Multiline))
            {
                var code = m.Groups[1].Value;
                const string name = "";
                var patt = int.Parse(m.Groups[2].Value);
                var sign = m.Groups[3].Value;
                var port = m.Groups[4].Value;

                // add to list
                list.Add(new Row { Code = code, Name = name, Pattern = patt, Local = sign, Portable = port, IsTier1 = isTier1});
            }

            // update currency names
            var count = 0;
            var warnings = "";
            foreach (Match m in Regex.Matches(CurrencyNames, @"^\s*currency\t\|\t([A-Z]+)\t\|\t(.+?)\t\|", RegexOptions.Multiline))
            {
                var code = m.Groups[1].Value;
                var name = m.Groups[2].Value;

                if (code == "XAF") name = "CFA Franc";
                if (code == "TOP") name = "Tongan Pa'anga";

                // get existing currency object
                var c = list.Where(x => x.Code == code).FirstOrDefault();

                if (c == null) continue;
                if (name == c.Name) continue;
                if (!string.IsNullOrEmpty(c.Name)) warnings += string.Format("'{0}' name changed '{1}' -> '{2}'\r\n", c.Code, c.Name, name);

                // update currency name
                c.Name = name;
                count++;
            }

            // validate
            if (count < list.Count()) throw new Exception("Not all currency codes had names found for them.");
            if (warnings.Count() > 0) throw new Exception("Some currency codes had multiple names found for them (see 'warnings' property).");
            /*
'XAF' name changed 'CFA Franc BEAC ‡' -> 'CFP Franc'
'XAF' name changed 'CFP Franc' -> 'CFA Franc BEAC ‡'
'TOP' name changed 'CFA Franc BCEAO †' -> 'Pa'anga'
             */

            // write output
            var sb = new StringBuilder();
            foreach (var c in list)
                sb.AppendLine(string.Format(@"new Currency(""{0}"", ""{1}"", {2}, ""{3}"", ""{4}"", {5}),",
                                            c.Code, c.Name, c.Pattern, c.Local, c.Portable, c.IsTier1.ToString().ToLower()));
            return sb.ToString();
        }

        #region Data: Tier1 Currencies

        /// <summary>
        /// Source data for tier 2 currencies
        /// file name:      Currency.js
        /// file location:  http://code.google.com/p/closure-library/source/browse/trunk/closure/goog/i18n/currency.js
        /// website:        Google Closure Library - http://code.google.com/p/closure-library/source/browse/#svn%2Ftrunk%2Fclosure%2Fgoog%2Fi18n%253Fstate%253Dclosed
        /// </summary>
        private const string Tier1CurrencyData = @"
'AED': [2, '\u062F\u002e\u0625', 'DH'],
'ARS': [2, '$', 'AR$'],
'AUD': [2, '$', 'AU$'],
'BDT': [2, '\u09F3', 'Tk'],
'BRL': [2, 'R$', 'R$'],
'CAD': [2, '$', 'C$'],
'CHF': [2, 'Fr.', 'CHF'],
'CLP': [0, '$', 'CL$'],
'CNY': [2, '¥', 'RMB¥'],
'COP': [2, '$', 'COL$'],
'CRC': [2, '\u20a1', 'CR₡'],
'CUP': [2, '$', '$MN'],
'CZK': [10, 'Kč', 'Kč'],
'DKK': [26, 'kr', 'kr'],
'DOP': [2, '$', 'RD$'],
'EGP': [2, '£', 'LE'],
'EUR': [26, '€', '€'],
'GBP': [2, '£', 'GB£'],
'HKD': [2, '$', 'HK$'],
'ILS': [10, '\u20AA', 'IL₪'],
'INR': [2, 'Rs', 'Rs'],
'ISK': [10, 'kr', 'kr'],
'JMD': [2, '$', 'JA$'],
'JPY': [0, '¥', 'JP¥'],
'KRW': [0, '\u20A9', 'KR₩'],
'LKR': [2, 'Rs', 'SLRs'],
'MNT': [2, '\u20AE', 'MN₮'],
'MXN': [2, '$', 'Mex$'],
'MYR': [2, 'RM', 'RM'],
'NOK': [26, 'kr', 'NOkr'],
'PAB': [2, 'B/.', 'B/.'],
'PEN': [2, 'S/.', 'S/.'],
'PHP': [2, 'P', 'PHP'],
'PKR': [2, 'Rs.', 'PKRs.'],
'RUB': [10, 'руб', 'руб'],
'SAR': [2, '\u0633\u002E\u0631', 'SR'],
'SEK': [10, 'kr', 'kr'],
'SGD': [2, '$', 'S$'],
'THB': [2, '\u0e3f', 'THB'],
'TRY': [2, 'YTL', 'YTL'],
'TWD': [2, 'NT$', 'NT$'],
'USD': [2, '$', 'US$'],
'UYU': [2, '$', 'UY$'],
'VND': [10, '\u20AB', 'VN₫'],
'YER': [2, 'YER', 'YER'],
'ZAR': [2, 'R', 'ZAR']
";

        #endregion

        #region  Data: Tier2 Currencies

        /// <summary>
        /// Source data for tier 2 currencies
        /// file name:      Currency.js
        /// file location:  http://code.google.com/p/closure-library/source/browse/trunk/closure/goog/i18n/currency.js
        /// website:        Google Closure Library - http://code.google.com/p/closure-library/source/browse/#svn%2Ftrunk%2Fclosure%2Fgoog%2Fi18n%253Fstate%253Dclosed
        /// </summary>
        private const string Tier2CurrencyData = @"
'AFN': [18, '\u060b', 'AFN'],
'ALL': [2, 'Lek', 'Lek'],
'AMD': [10, '\u0564\u0580\u002e', 'dram'],
'ANG': [2, '\u0083', 'NAƒ'],
'AOA': [2, 'Kz', 'Kz'],
'AWG': [2, 'ƒ', 'Afl.'],
'AZN': [2, 'm', 'man'],
'BAM': [18, 'КМ', 'KM'],
'BBD': [2, '$', 'Bds$'],
'BGN': [10, '\u043b\u0432', 'лв'],
'BHD': [3, '\u0628\u002e\u062f\u002e', 'BD'],
'BIF': [0, 'FBu', 'FBu'],
'BMD': [2, '$', 'BD$'],
'BND': [2, '$', 'B$'],
'BOB': [2, 'B$', 'B$'],
'BSD': [2, '$', 'B$'],
'BTN': [2, 'Nu.', 'Nu.'],
'BWP': [2, 'P', 'pula'],
'BYR': [0, 'Br', 'Br'],
'BZD': [2, '$', 'BZ$'],
'CDF': [2, 'F', 'CDF'],
'CVE': [2, '$', 'Esc'],
'DJF': [0, 'Fdj', 'Fdj'],
'DZD': [2, '\u062f\u062C', 'DA'],
'EEK': [10, 'EEK', 'EEK'],
'ERN': [2, 'Nfk', 'Nfk'],
'ETB': [2, 'Br', 'Br'],
'FJD': [2, '$', 'FJ$'],
'FKP': [2, '£', 'FK£'],
'GEL': [2, 'GEL', 'GEL'],
'GHS': [2, '\u20B5', 'GHS¢'],
'GIP': [2, '£', 'GI£'],
'GMD': [2, 'D', 'GMD'],
'GNF': [0, 'FG', 'FG'],
'GTQ': [2, 'Q', 'GTQ'],
'GYD': [2, '$', 'GY$'],
'HNL': [2, 'L', 'HNL'],
'HRK': [2, 'kn', 'kn'],
'HTG': [2, 'G', 'HTG'],
'HUF': [10, 'Ft', 'Ft'],
'IDR': [2, 'Rp', 'Rp'],
'IQD': [3, '\u0639\u062F', 'IQD'],
'IRR': [2, '\ufdfc', 'IRR'],
'JOD': [3, 'JOD', 'JOD'],
'KES': [2, 'KSh', 'KSh'],
'KGS': [2, 'som', 'som'],
'KHR': [10, '\u17DB', 'KHR'],
'KMF': [0, 'KMF', 'KMF'],
'KPW': [2, '\u20A9', 'KPW'],
'KWD': [3, '\u062F\u002e\u0643', 'KWD'],
'KYD': [2, '$', 'CI$'],
'KZT': [10, 'KZT', 'KZT'],
'LAK': [2, '\u20AD', 'LA₭'],
'LBP': [2, '\u0644\u002e\u0644', 'LBP'],
'LRD': [2, '$', 'L$'],
'LSL': [2, 'L', 'LSL'],
'LTL': [10, 'Lt', 'Lt'],
'LVL': [10, 'Ls', 'Ls'],
'LYD': [3, '\u0644\u002e\u062F', 'LD'],
'MAD': [2, '\u0645\u002E\u062F\u002E', 'MAD'],
'MDL': [2, 'MDL', 'MDL'],
'MGA': [1, 'MGA', 'MGA'],
'MKD': [2, 'MKD', 'MKD'],
'MMK': [2, 'K', 'MMK'],
'MOP': [2, 'MOP$', 'MOP$'],
'MRO': [1, 'UM', 'UM'],
'MUR': [2, 'Rs', 'MURs'],
'MVR': [2, 'Rf', 'MRF'],
'MWK': [2, 'MK', 'MK'],
'MZN': [2, 'MTn', 'MTn'],
'NAD': [2, '$', 'N$'],
'NGN': [2, '\u20A6', 'NG₦'],
'NIO': [2, 'C$', 'C$'],
'NPR': [2, 'Rs', 'NPRs'],
'NZD': [2, '$', 'NZ$'],
'OMR': [3, '\u0639\u002E\u062F\u002E', 'OMR'],
'PGK': [2, 'K', 'PGK'],
'PLN': [10, 'zł', 'zł'],
'PYG': [0, '\u20b2', 'PYG'],
'QAR': [2, '\u0642\u002E\u0631', 'QR'],
'RON': [2, 'L', 'RON'],
'RSD': [2, 'РС\u0414', 'RSD'],
'RWF': [0, 'RF', 'RF'],
'SBD': [2, '$', 'SI$'],
'SCR': [2, 'SR', 'SCR'],
'SDG': [2, 'SDG', 'SDG'],
'SHP': [2, '£', 'SH£'],
'SKK': [10, 'Sk', 'Sk'],
'SLL': [2, 'Le', 'Le'],
'SOS': [2, 'So. Sh.', 'So. Sh.'],
'SRD': [2, '$', 'SR$'],
'STD': [2, 'Db', 'Db'],
'SYP': [18, 'SYP', 'SYP'],
'SZL': [2, 'L', 'SZL'],
'TJS': [2, 'TJS', 'TJS'],
'TMM': [2, 'm', 'TMM'],
'TND': [3, '\u062F\u002e\u062A ', 'DT'],
'TOP': [2, 'T$', 'T$'],
'TTD': [2, '$', 'TT$'],
'TZS': [10, 'TZS', 'TZS'],
'UAH': [10, '\u20B4', 'грн'],
'UGX': [2, 'USh', 'USh'],
'UZS': [2, 'UZS', 'UZS'],
'VEF': [2, 'Bs.F', 'Bs.F'],
'VUV': [0, 'Vt', 'Vt'],
'WST': [2, 'WS$', 'WS$'],
'XAF': [0, 'FCFA', 'FCFA'],
'XCD': [2, '$', 'EC$'],
'XOF': [0, 'CFA', 'CFA'],
'XPF': [0, 'F', 'XPF'],
'ZMK': [2, 'ZK', 'ZK'],
'ZWL': [2, '$', 'ZW$']
";
        #endregion
            
        #region  Data: Currency Names

        /// <summary>
        /// Source data for tier 2 currencies
        /// file name:      ISO4217.txt
        /// file location:  http://unicode.org/cldr/trac/browser/tags/release-1-9/tools/java/org/unicode/cldr/util/data/ISO4217.txt
        /// website:        CLDR - Unicode Common Locale Data Repository - http://cldr.unicode.org/index/downloads
        /// </summary>
        private const string CurrencyNames =
            @"
currency	|	XAG	|	Silver	|	ZZ	|	(no country)	|	C
currency	|	XAU	|	Gold	|	ZZ	|	(no country)	|	C
currency	|	XBA	|	Bond Markets Units European Composite Unit (EURCO)	|	ZZ	|	(no country)	|	C
currency	|	XBB	|	European Monetary Unit (E.M.U.-6) 	|	ZZ	|	(no country)	|	C
currency	|	XBC	|	European Unit of Account 9(E.U.A.-9)	|	ZZ	|	(no country)	|	C
currency	|	XBD	|	European Unit of Account 17(E.U.A.-17)	|	ZZ	|	(no country)	|	C
currency	|	XFO	|	Gold-Franc	|	ZZ	|	(no country)	|	C
currency	|	XFU	|	UIC-Franc	|	ZZ	|	(no country)	|	C
currency	|	XPD	|	Palladium	|	ZZ	|	(no country)	|	C
currency	|	XPT	|	Platinum	|	ZZ	|	(no country)	|	C
currency	|	XTS	|	Codes specifically reserved for testing purposes	|	ZZ	|	(no country)	|	C
currency	|	XXX	|	The codes assigned for transactions where no currency is involved	|	ZZ	|	(no country)	|	C
currency	|	AFN	|	Afghani	|	AF	|	AFGHANISTAN	|	C
currency	|	ALL	|	Lek	|	AL	|	ALBANIA	|	C
currency	|	DZD	|	Algerian Dinar	|	DZ	|	ALGERIA	|	C
currency	|	USD	|	US Dollar	|	AS	|	AMERICAN SAMOA	|	C
currency	|	EUR	|	Euro	|	AD	|	ANDORRA	|	C
currency	|	AOA	|	Kwanza	|	AO	|	ANGOLA	|	C
currency	|	XCD	|	East Caribbean Dollar	|	AI	|	ANGUILLA	|	C
currency	|	XXX	|	No universal currency	|	AQ	|	ANTARCTICA	|	C
currency	|	XCD	|	East Caribbean Dollar	|	AG	|	ANTIGUA AND BARBUDA	|	C
currency	|	ARS	|	Argentine Peso	|	AR	|	ARGENTINA	|	C
currency	|	AMD	|	Armenian Dram	|	AM	|	ARMENIA	|	C
currency	|	AWG	|	Aruban Guilder	|	AW	|	ARUBA	|	C
currency	|	AUD	|	Australian Dollar	|	AU	|	AUSTRALIA	|	C
currency	|	EUR	|	Euro	|	AT	|	AUSTRIA	|	C
currency	|	AZM	|	Azerbaijanian Manat	 (1993-2006) 	|	AZ	|	AZERBAIJAN	|	C
currency	|	AZN	|	Azerbaijanian Manat	|	AZ	|	AZERBAIJAN	|	C
currency	|	BSD	|	Bahamian Dollar	|	BS	|	BAHAMAS	|	C
currency	|	BHD	|	Bahraini Dinar	|	BH	|	BAHRAIN	|	C
currency	|	BDT	|	Taka	|	BD	|	BANGLADESH	|	C
currency	|	BBD	|	Barbados Dollar	|	BB	|	BARBADOS	|	C
currency	|	BYR	|	Belarussian Ruble	|	BY	|	BELARUS	|	C
currency	|	EUR	|	Euro	|	BE	|	BELGIUM	|	C
currency	|	BZD	|	Belize Dollar	|	BZ	|	BELIZE	|	C
currency	|	XOF	|	CFA Franc BCEAO †	|	BJ	|	BENIN	|	C
currency	|	BMD	|	Bermudian Dollar (customarily known as Bermuda Dollar)	|	BM	|	BERMUDA	|	C
currency	|	BTN	|	Ngultrum	|	BT	|	BHUTAN	|	C
currency	|	INR	|	Indian Rupee	|	BT	|	BHUTAN	|	C
currency	|	BOB	|	Boliviano	|	BO	|	BOLIVIA	|	C
currency	|	BOV	|	Mvdol	|	BO	|	BOLIVIA	|	F
currency	|	BAM	|	Convertible Marks	|	BA	|	BOSNIA & HERZEGOVINA	|	C
currency	|	BWP	|	Pula	|	BW	|	BOTSWANA	|	C
currency	|	NOK	|	Norwegian Krone	|	BV	|	BOUVET ISLAND	|	C
currency	|	BRL	|	Brazilian Real	|	BR	|	BRAZIL	|	C
currency	|	USD	|	US Dollar	|	IO	|	BRITISH INDIAN OCEAN TERRITORY	|	C
currency	|	BND	|	Brunei Dollar	|	BN	|	BRUNEI DARUSSALAM	|	C
currency	|	BGN	|	Bulgarian Lev	|	BG	|	BULGARIA	|	C
currency	|	XOF	|	CFA Franc BCEAO †	|	BF	|	BURKINA FASO	|	C
currency	|	BIF	|	Burundi Franc	|	BI	|	BURUNDI	|	C
currency	|	KHR	|	Riel	|	KH	|	CAMBODIA	|	C
currency	|	XAF	|	CFA Franc BEAC ‡	|	CM	|	CAMEROON	|	C
currency	|	CAD	|	Canadian Dollar	|	CA	|	CANADA	|	C
currency	|	CVE	|	Cape Verde Escudo	|	CV	|	CAPE VERDE	|	C
currency	|	KYD	|	Cayman Islands Dollar	|	KY	|	CAYMAN ISLANDS	|	C
currency	|	XAF	|	CFA Franc BEAC ‡	|	CF	|	CENTRAL AFRICAN REPUBLIC	|	C
currency	|	XAF	|	CFA Franc BEAC ‡	|	TD	|	CHAD	|	C
currency	|	CLP	|	Chilean Peso	|	CL	|	CHILE	|	C
currency	|	CNY	|	Yuan Renminbi	|	CN	|	CHINA	|	C
currency	|	AUD	|	Australian Dollar	|	CX	|	CHRISTMAS ISLAND	|	C
currency	|	AUD	|	Australian Dollar	|	CC	|	COCOS (KEELING) ISLANDS	|	C
currency	|	COP	|	Colombian Peso	|	CO	|	COLOMBIA	|	C
currency	|	KMF	|	Comoro Franc	|	KM	|	COMOROS	|	C
currency	|	XAF	|	CFA Franc BEAC ‡	|	CG	|	CONGO	|	C
currency	|	CDF	|	Franc Congolais	|	CD	|	CONGO, THE DEMOCRATIC REPUBLIC OF	|	C
currency	|	NZD	|	New Zealand Dollar	|	CK	|	COOK ISLANDS	|	C
currency	|	CRC	|	Costa Rican Colon	|	CR	|	COSTA RICA	|	C
currency	|	XOF	|	CFA Franc BCEAO †	|	CI	|	CÔTE D'IVOIRE	|	C
currency	|	HRK	|	Croatian Kuna	|	HR	|	CROATIA	|	C
currency	|	CUP	|	Cuban Peso	|	CU	|	CUBA	|	C
currency	|	CYP	|	Cyprus Pound	|	CY	|	CYPRUS	|	O
currency	|	CZK	|	Czech Koruna	|	CZ	|	CZECH REPUBLIC	|	C
currency	|	DKK	|	Danish Krone	|	DK	|	DENMARK	|	C
currency	|	DJF	|	Djibouti Franc	|	DJ	|	DJIBOUTI	|	C
currency	|	XCD	|	East Caribbean Dollar	|	DM	|	DOMINICA	|	C
currency	|	DOP	|	Dominican Peso	|	DO	|	DOMINICAN REPUBLIC	|	C
currency	|	USD	|	US Dollar	|	EC	|	ECUADOR	|	C
currency	|	EGP	|	Egyptian Pound	|	EG	|	EGYPT	|	C
currency	|	SVC	|	El Salvador Colon	|	SV	|	EL SALVADOR	|	C
currency	|	USD	|	US Dollar	|	SV	|	EL SALVADOR	|	C
currency	|	XAF	|	CFA Franc BEAC ‡	|	GQ	|	EQUATORIAL GUINEA	|	C
currency	|	ERN	|	Nakfa	|	ER	|	ERITREA	|	C
currency	|	EEK	|	Kroon	|	EE	|	ESTONIA	|	C
currency	|	ETB	|	Ethiopian Birr	|	ET	|	ETHIOPIA	|	C
currency	|	FKP	|	Falkland Islands Pound	|	FK	|	FALKLAND ISLANDS (MALVINAS)	|	C
currency	|	DKK	|	Danish Krone	|	FO	|	FAROE ISLANDS	|	C
currency	|	FJD	|	Fiji Dollar	|	FJ	|	FIJI	|	C
currency	|	EUR	|	Euro	|	FI	|	FINLAND	|	C
currency	|	EUR	|	Euro	|	FR	|	FRANCE	|	C
currency	|	EUR	|	Euro	|	GF	|	FRENCH GUIANA	|	C
currency	|	XAF	|	CFP Franc	|	PF	|	FRENCH POLYNESIA	|	C
currency	|	EUR	|	Euro	|	TF	|	FRENCH SOUTHERN TERRITORIES	|	C
currency	|	XAF	|	CFA Franc BEAC ‡	|	GA	|	GABON	|	C
currency	|	GMD	|	Dalasi	|	GM	|	GAMBIA	|	C
currency	|	GEL	|	Lari	|	GE	|	GEORGIA	|	C
currency	|	EUR	|	Euro	|	DE	|	GERMANY	|	C
currency	|	GHC	|	Cedi	|	GH	|	GHANA	|	O
currency	|	GHS	|	Cedi	|	GH	|	GHANA	|	C
currency	|	GIP	|	Gibraltar Pound	|	GI	|	GIBRALTAR	|	C
currency	|	EUR	|	Euro	|	GR	|	GREECE	|	C
currency	|	DKK	|	Danish Krone	|	GL	|	GREENLAND	|	C
currency	|	XCD	|	East Caribbean Dollar	|	GD	|	GRENADA	|	C
currency	|	EUR	|	Euro	|	GP	|	GUADELOUPE	|	C
currency	|	USD	|	US Dollar	|	GU	|	GUAM	|	C
currency	|	GTQ	|	Quetzal	|	GT	|	GUATEMALA	|	C
currency	|	GNF	|	Guinea Franc	|	GN	|	GUINEA	|	C
currency	|	GWP	|	Guinea-Bissau Peso	|	GW	|	GUINEA-BISSAU	|	O
currency	|	XOF	|	CFA Franc BCEAO †	|	GW	|	GUINEA-BISSAU	|	C
currency	|	GYD	|	Guyana Dollar	|	GY	|	GUYANA	|	C
currency	|	HTG	|	Gourde	|	HT	|	HAITI	|	C
currency	|	USD	|	US Dollar	|	HT	|	HAITI	|	C
currency	|	AUD	|	Australian Dollar	|	HM	|	HEARD ISLAND AND McDONALD ISLANDS	|	C
currency	|	EUR	|	Euro	|	VA	|	HOLY SEE (VATICAN CITY STATE)	|	C
currency	|	HNL	|	Lempira	|	HN	|	HONDURAS	|	C
currency	|	HKD	|	Hong Kong Dollar	|	HK	|	HONG KONG	|	C
currency	|	HUF	|	Forint	|	HU	|	HUNGARY	|	C
currency	|	ISK	|	Iceland Krona	|	IS	|	ICELAND	|	C
currency	|	INR	|	Indian Rupee	|	IN	|	INDIA	|	C
currency	|	IDR	|	Rupiah	|	ID	|	INDONESIA	|	C
currency	|	XDR	|	SDR	|	ZZ	|	INTERNATIONAL MONETARY FUND (I.M.F) 	|	C
currency	|	IRR	|	Iranian Rial	|	IR	|	IRAN (ISLAMIC REPUBLIC OF)	|	C
currency	|	IQD	|	Iraqi Dinar	|	IQ	|	IRAQ	|	C
currency	|	EUR	|	Euro	|	IE	|	IRELAND	|	C
currency	|	ILS	|	New Israeli Sheqel	|	IL	|	ISRAEL	|	C
currency	|	EUR	|	Euro	|	IT	|	ITALY	|	C
currency	|	JMD	|	Jamaican Dollar	|	JM	|	JAMAICA	|	C
currency	|	JPY	|	Yen	|	JP	|	JAPAN	|	C
currency	|	JOD	|	Jordanian Dinar	|	JO	|	JORDAN	|	C
currency	|	KZT	|	Tenge	|	KZ	|	KAZAKHSTAN	|	C
currency	|	KES	|	Kenyan Shilling	|	KE	|	KENYA	|	C
currency	|	AUD	|	Australian Dollar	|	KI	|	KIRIBATI	|	C
currency	|	KPW	|	North Korean Won	|	KP	|	KOREA, DEMOCRATIC PEOPLE'S REPUBLIC OF	|	C
currency	|	KRW	|	Won	|	KR	|	KOREA, REPUBLIC OF	|	C
currency	|	KWD	|	Kuwaiti Dinar	|	KW	|	KUWAIT	|	C
currency	|	KGS	|	Som	|	KG	|	KYRGYZSTAN	|	C
currency	|	LAK	|	Kip	|	LA	|	LAO PEOPLE'S DEMOCRATIC REPUBLIC	|	C
currency	|	LVL	|	Latvian Lats	|	LV	|	LATVIA	|	C
currency	|	LBP	|	Lebanese Pound	|	LB	|	LEBANON	|	C
currency	|	LSL	|	Loti	|	LS	|	LESOTHO	|	C
currency	|	ZAR	|	Rand	|	LS	|	LESOTHO	|	C
currency	|	LRD	|	Liberian Dollar	|	LR	|	LIBERIA	|	C
currency	|	LYD	|	Libyan Dinar	|	LY	|	LIBYAN ARAB JAMAHIRIYA	|	C
currency	|	CHF	|	Swiss Franc	|	LI	|	LIECHTENSTEIN	|	C
currency	|	LTL	|	Lithuanian Litas	|	LT	|	LITHUANIA	|	C
currency	|	EUR	|	Euro	|	LU	|	LUXEMBOURG	|	C
currency	|	MOP	|	Pataca	|	MO	|	MACAO	|	C
currency	|	MKD	|	Denar	|	MK	|	MACEDONIA, THE FORMER YUGOSLAV REPUBLIC OF	|	C
currency	|	MGA	|	Malagasy Ariary	|	MG	|	MADAGASCAR	|	C
currency	|	MGF	|	Malagasy Franc	|	MG	|	MADAGASCAR	|	O
currency	|	MWK	|	Kwacha	|	MW	|	MALAWI	|	C
currency	|	MYR	|	Malaysian Ringgit	|	MY	|	MALAYSIA	|	C
currency	|	MVR	|	Rufiyaa	|	MV	|	MALDIVES	|	C
currency	|	XOF	|	CFA Franc BCEAO †	|	ML	|	MALI	|	C
currency	|	MTL	|	Maltese Lira	|	MT	|	MALTA	|	O
currency	|	USD	|	US Dollar	|	MH	|	MARSHALL ISLANDS	|	C
currency	|	EUR	|	Euro	|	MQ	|	MARTINIQUE	|	C
currency	|	MRO	|	Ouguiya	|	MR	|	MAURITANIA	|	C
currency	|	MUR	|	Mauritius Rupee	|	MU	|	MAURITIUS	|	C
currency	|	EUR	|	Euro	|	YT	|	MAYOTTE	|	C
currency	|	MXN	|	Mexican Peso	|	MX	|	MEXICO	|	C
currency	|	USD	|	US Dollar	|	FM	|	MICRONESIA (FEDERATED STATES OF)	|	C
currency	|	MDL	|	Moldovan Leu	|	MD	|	MOLDOVA, REPUBLIC OF	|	C
currency	|	EUR	|	Euro	|	MC	|	MONACO	|	C
currency	|	MNT	|	Tugrik	|	MN	|	MONGOLIA	|	C
currency	|	XCD	|	East Caribbean Dollar	|	MS	|	MONTSERRAT	|	C
currency	|	MAD	|	Moroccan Dirham	|	MA	|	MOROCCO	|	C
currency	|	MZM	|	Merical	|	MZ	|	MOZAMBIQUE	|	C
currency	|	MZN	|	Merical	|	MTn	|	MOZAMBIQUE	|	C
currency	|	MMK	|	Kyat	|	MM	|	MYANMAR	|	C
currency	|	NAD	|	Namibian Dollar	|	NA	|	NAMIBIA	|	C
currency	|	ZAR	|	Rand	|	NA	|	NAMIBIA	|	C
currency	|	AUD	|	Australian Dollar	|	NR	|	NAURU	|	C
currency	|	NPR	|	Nepalese Rupee	|	NP	|	NEPAL	|	C
currency	|	EUR	|	Euro	|	NL	|	NETHERLANDS	|	C
currency	|	ANG	|	Netherlands Antillian Guikder	|	AN	|	NETHERLANDS ANTILLES	|	C
currency	|	XPF	|	CFP Franc	|	NC	|	NEW CALEDONIA	|	C
currency	|	NZD	|	New Zealand Dollar	|	NZ	|	NEW ZEALAND	|	C
currency	|	NIO	|	Cordoba Oro	|	NI	|	NICARAGUA	|	C
currency	|	XOF	|	CFA Franc BCEAO †	|	NE	|	NIGER	|	C
currency	|	NGN	|	Naira	|	NG	|	NIGERIA	|	C
currency	|	NZD	|	New Zealand Dollar	|	NU	|	NIUE	|	C
currency	|	AUD	|	Australian Dollar	|	NF	|	NORFOLK ISLAND	|	C
currency	|	USD	|	US Dollar	|	MP	|	NORTHERN MARIANA ISLANDS	|	C
currency	|	NOK	|	Norwegian Krone	|	NO	|	NORWAY	|	C
currency	|	OMR	|	Rial Omani	|	OM	|	OMAN	|	C
currency	|	PKR	|	Pakistan Rupee	|	PK	|	PAKISTAN	|	C
currency	|	USD	|	US Dollar	|	PW	|	PALAU	|	C
currency	|	PAB	|	Balboa	|	PA	|	PANAMA	|	C
currency	|	USD	|	US Dollar	|	PA	|	PANAMA	|	C
currency	|	PGK	|	Kina	|	PG	|	PAPUA NEW GUINEA	|	C
currency	|	PYG	|	Guarani	|	PY	|	PARAGUAY	|	C
currency	|	PEN	|	Nuevo Sol	|	PE	|	PERU	|	C
currency	|	PHP	|	Philippine Peso	|	PH	|	PHILIPPINES	|	C
currency	|	NZD	|	New Zealand Dollar	|	PN	|	PITCAIRN	|	C
currency	|	PLN	|	Zloty	|	PL	|	POLAND	|	C
currency	|	EUR	|	Euro	|	PT	|	PORTUGAL	|	C
currency	|	USD	|	US Dollar	|	PR	|	PUERTO RICO	|	C
currency	|	QAR	|	Qatari Rial	|	QA	|	QATAR	|	C
currency	|	EUR	|	Euro	|	RE	|	RÉUNION	|	C
currency	|	ROL	|	Old Leu	|	RO	|	ROMANIA	|	C
currency	|	RON	|	Leu	|	RO	|	ROMANIA	|	I
currency	|	RUB	|	Russian Ruble	|	RU	|	RUSSIAN FEDERATION	|	C
currency	|	RWF	|	Rwanda Franc	|	RW	|	RWANDA	|	C
currency	|	SHP	|	Saint Helena Pound	|	SH	|	SAINT HELENA	|	C
currency	|	XCD	|	East Caribbean Dollar	|	KN	|	SAINT KITTS AND NEVIS	|	C
currency	|	XCD	|	East Caribbean Dollar	|	LC	|	SAINT LUCIA	|	C
currency	|	EUR	|	Euro	|	PM	|	SAINT PIERRE AND MIQUELON	|	C
currency	|	XCD	|	East Caribbean Dollar	|	VC	|	SAINT VINCENT AND THE GRENADINES	|	C
currency	|	WST	|	Tala	|	WS	|	SAMOA	|	C
currency	|	EUR	|	Euro	|	SM	|	SAN MARINO	|	C
currency	|	STD	|	Dobra	|	ST	|	SÃO TOME AND PRINCIPE	|	C
currency	|	SAR	|	Saudi Riyal	|	SA	|	SAUDI ARABIA	|	C
currency	|	XOF	|	CFA Franc BCEAO †	|	SN	|	SENEGAL	|	C
currency	|	CSD	|	Serbian Dinar	|	CS	|	SERBIA AND MONTENEGRO	|	C
currency	|	EUR	|	Euro	|	CS	|	SERBIA AND MONTENEGRO	|	C
currency	|	SCR	|	Seychelles Rupee	|	SC	|	SEYCHELLES	|	C
currency	|	SLL	|	Leone	|	SL	|	SIERRA LEONE	|	C
currency	|	SGD	|	Singapore Dollar	|	SG	|	SINGAPORE	|	C
currency	|	SKK	|	Slovak Koruna	|	SK	|	SLOVAKIA	|	C
currency	|	SIT	|	Tolar	|	SI	|	SLOVENIA	|	O
currency	|	EUR	|	Euro	|	SI	|	SLOVENIA	|	C
currency	|	SBD	|	Solomon Islands Dollar	|	SB	|	SOLOMON ISLANDS	|	C
currency	|	SOS	|	Somali Shilling	|	SO	|	SOMALIA	|	C
currency	|	ZAR	|	Rand	|	ZA	|	SOUTH AFRICA	|	C
currency	|	EUR	|	Euro	|	ES	|	SPAIN	|	C
currency	|	LKR	|	Sri Lanka Rupee	|	LK	|	SRI LANKA	|	C
currency	|	SDG	|	Sudanese Pound	|	SD	|	SUDAN	|	C
currency	|	SDD	|	Sudanese Dinar	|	SD	|	SUDAN	|	O
currency	|	SRD	|	Surinam Dollar	|	SR	|	SURINAME	|	C
currency	|	NOK	|	Norwegian Krone	|	SJ	|	SVALBARD AND JAN MAYEN	|	C
currency	|	SZL	|	Lilangeni	|	SZ	|	SWAZILAND	|	C
currency	|	SEK	|	Swedish Krona	|	SE	|	SWEDEN	|	C
currency	|	CHF	|	Swiss Franc	|	CH	|	SWITZERLAND	|	C
currency	|	SYP	|	Syrian Pound	|	SY	|	SYRIAN ARAB REPUBLIC	|	C
currency	|	TWD	|	New Taiwan Dollar	|	TW	|	TAIWAN, PROVINCE OF CHINA	|	C
currency	|	TJS	|	Somoni	|	TJ	|	TAJIKISTAN	|	C
currency	|	TZS	|	Tanzanian Shilling	|	TZ	|	TANZANIA, UNITED REPUBLIC OF	|	C
currency	|	THB	|	Baht	|	TH	|	THAILAND	|	C
currency	|	USD	|	US Dollar	|	TL	|	TIMOR-LESTE	|	C
currency	|	TOP	|	CFA Franc BCEAO †	|	TG	|	TOGO	|	C
currency	|	NZD	|	New Zealand Dollar	|	TK	|	TOKELAU	|	C
currency	|	TOP	|	Pa'anga	|	TO	|	TONGA	|	C
currency	|	TTD	|	Trinidad and Tobago Dollar	|	TT	|	TRINIDAD AND TOBAGO	|	C
currency	|	TND	|	Tunisian Dinar	|	TN	|	TUNISIA	|	C
currency	|	TRY	|	new Turkish Lira	|	TR	|	TURKEY	|	C
currency	|	TMM	|	Manat	|	TM	|	TURKMENISTAN	|	C
currency	|	USD	|	US Dollar	|	TC	|	TURKS AND CAICOS ISLANDS	|	C
currency	|	AUD	|	Australian Dollar	|	TV	|	TUVALU	|	C
currency	|	UGX	|	Uganda Shilling	|	UG	|	UGANDA	|	C
currency	|	UAH	|	Hryvnia	|	UA	|	UKRAINE	|	C
currency	|	AED	|	UAE Dirham	|	AE	|	UNITED ARAB EMIRATES	|	C
currency	|	GBP	|	Pound Sterling	|	GB	|	UNITED KINGDOM	|	C
currency	|	USD	|	US Dollar	|	US	|	UNITED STATES	|	C
currency	|	USD	|	US Dollar	|	UM	|	UNITED STATES MINOR OUTLYING ISLANDS	|	C
currency	|	UYU	|	Peso Uruguayo	|	UY	|	URUGUAY	|	C
currency	|	UZS	|	Uzbekistan Sum	|	UZ	|	UZBEKISTAN	|	C
currency	|	VUV	|	Vatu	|	VU	|	VANUATU	|	C
currency	|	VEB	|	Bolivar	|	VE	|	VENEZUELA	|	O
currency	|	VEF	|	Bolivar Fuerte	|	VE	|	VENEZUELA	|	C
currency	|	VND	|	Dong	|	VN	|	VIET NAM	|	C
currency	|	USD	|	US Dollar	|	VG	|	VIRGIN ISLANDS (BRITISH)	|	C
currency	|	USD	|	US Dollar	|	VI	|	VIRGIN ISLANDS (US)	|	C
currency	|	XPF	|	CFP Franc	|	WF	|	WALLIS AND FUTUNA	|	C
currency	|	MAD	|	Moroccan Dirham	|	EH	|	WESTERN SAHARA	|	C
currency	|	YER	|	Yemeni Rial	|	YE	|	YEMEN	|	C
currency	|	ZMK	|	Kwacha	|	ZM	|	ZAMBIA	|	C
currency	|	ZWD	|	Zimbabwe Dollar	|	ZW	|	ZIMBABWE	|	C
currency	|	CLF	|	Unidades de formento	|	CL	|	CHILE	|	F
currency	|	COU	|	Unidad de Valor Real	|	CO	|	COLOMBIA	|	F
currency	|	MXV	|	Mexican Unidad de Inversion (UID)	|	MX	|	MEXICO	|	F
currency	|	CHE	|	WIR Euro	|	CH	|	SWITZERLAND	|	F
currency	|	CHW	|	WIR Franc	|	CH	|	SWITZERLAND	|	F
currency	|	TRL	|	old Turkish Lira	|	TR	|	TURKEY	|	O
currency	|	USN	|	(Next day)	|	US	|	UNITED STATES	|	F
currency	|	USS	|	(Same day)	|	US	|	UNITED STATES	|	F
currency	|	BYB	|	Belarus Ruble	|	BY	|	BELARUS	|	I
currency	|	ECS	|	Sucre	|	EC	|	ECUADOR	|	I
currency	|	MAF	|	Mali Franc	|	ML	|	MALI	|	I
currency	|	TJR	|	Tajik Ruble	|	TJ	|	TAJIKISTAN	|	I
currency	|	XRE	|	RINET Funds Code	|	ZZ	|	(no country)	|	O
currency	|	AFA	|	Afghani	|	AF	|	AFGHANISTAN	|	O
currency	|	ADP	|	Andorran Peseta	|	AD	|	ANDORRA	|	O
currency	|	AOK	|	Kwanza	|	AO	|	ANGOLA	|	O
currency	|	AON	|	New Kwanza	|	AO	|	ANGOLA	|	O
currency	|	AOR	|	Kwanza Reajustado	|	AO	|	ANGOLA	|	O
currency	|	ARA	|	Austral	|	AR	|	ARGENTINA	|	O
currency	|	ARP	|	Peso Argentino	|	AR	|	ARGENTINA	|	O
currency	|	ATS	|	Schilling	|	AT	|	AUSTRIA	|	O
currency	|	BEC	|	Convertible Franc	|	BE	|	BELGIUM	|	O
currency	|	BEF	|	Belgian Franc	|	BE	|	BELGIUM	|	O
currency	|	BEL	|	Financial Franc	|	BE	|	BELGIUM	|	O
currency	|	BOP	|	Peso	|	BO	|	BOLIVIA	|	O
currency	|	BAD	|	Dinar	|	BA	|	BOSNIA AND HERZEGOVINA	|	O
currency	|	BRB	|	Cruzeiro	|	BR	|	BRAZIL	|	O
currency	|	BRC	|	Cruzado	|	BR	|	BRAZIL	|	O
currency	|	BRE	|	Cruzeiro	|	BR	|	BRAZIL	|	O
currency	|	BRN	|	New Cruzado	|	BR	|	BRAZIL	|	O
currency	|	BRR	|	Cruzeiro Real	|	BR	|	BRAZIL	|	O
currency	|	BGL	|	Lev	|	BG	|	BULGARIA	|	O
currency	|	BUK	|	N.A.	|	BU	|	BURMA	|	O
currency	|	HRD	|	Dinar	|	HR	|	CROATIA	|	O
currency	|	CSK	|	Koruna	|	CSHH	|	CZECHOSLOVAKIA	|	O
currency	|	TPE	|	Timor Escudo	|	TP	|	EAST TIMOR	|	O
currency	|	ECV	|	Ecuadoran Constant Value Unit	|	EC	|	ECUADOR	|	O
currency	|	GQE	|	Ekwele	|	GQ	|	EQUATORIAL GUINEA	|	O
currency	|	XEU	|	European Currency Unit (E.C.U)	|	ZZ	|	EUROPEAN MONETARY CO-OPERATION FUND (EMCF)	|	O
currency	|	FIM	|	Markka	|	FI	|	FINLAND	|	O
currency	|	FRF	|	French Franc	|	FR	|	FRANCE	|	O
currency	|	GEK	|	Georgian Coupon	|	GE	|	GEORGIA	|	O
currency	|	DDM	|	Mark der DDR	|	DD	|	GERMAN DEMOCRATIC REPUBLIC	|	O
currency	|	DEM	|	Deutsche Mark	|	DE	|	GERMANY	|	O
currency	|	GRD	|	Drachma	|	GR	|	GREECE	|	O
currency	|	GNS	|	Syli	|	GN	|	GUINEA	|	O
currency	|	GWE	|	Guinea Escudo	|	GW	|	GUINEA-BISSAU	|	O
currency	|	IEP	|	Irish Pound	|	IE	|	IRELAND	|	O
currency	|	ILP	|	Pound	|	IL	|	ISRAEL	|	O
currency	|	ITL	|	Italian Lira	|	IT	|	ITALY	|	O
currency	|	LVR	|	Latvian Ruble	|	LV	|	LATVIA	|	O
currency	|	LTT	|	Talonas	|	LT	|	LITHUANIA	|	O
currency	|	LUC	|	Convertible Franc	|	LU	|	LUXEMBOURG	|	O
currency	|	LUF	|	Luxembourg Franc	|	LU	|	LUXEMBOURG	|	O
currency	|	LUL	|	Financial Franc	|	LU	|	LUXEMBOURG	|	O
currency	|	MLF	|	Mali Franc	|	ML	|	MALI	|	O
currency	|	MTP	|	Maltese Pound	|	MT	|	MALTA	|	O
currency	|	MXP	|	Mexican Peso	|	MX	|	MEXICO	|	O
currency	|	MZE	|	Mozambique Escudo	|	MZ	|	MOZAMBIQUE	|	O
currency	|	NLG	|	Netherlands Guilder	|	NL	|	NETHERLANDS	|	O
currency	|	NIC	|	Cordoba	|	NI	|	NICARAGUA	|	O
currency	|	PEI	|	Inti	|	PE	|	PERU	|	O
currency	|	PES	|	Sol	|	PE	|	PERU	|	O
currency	|	PLZ	|	Zloty	|	PL	|	POLAND	|	O
currency	|	PTE	|	Portuguese Escudo	|	PT	|	PORTUGAL	|	O
currency	|	RUR	|	Russian Ruble	|	RU	|	RUSSIAN FEDERATION	|	O
currency	|	ZAL	|	Financial Rand	|	ZA	|	SOUTH AFRICA	|	O
currency	|	RHD	|	Rhodesian Dollar	|	RH	|	SOUTHERN RHODESIA	|	O
currency	|	ESA	|	Spanish Peseta (“A�? Account)	|	ES	|	SPAIN	|	O
currency	|	ESB	|	Spanish Peseta (convertible Peseta Accounts)	|	ES	|	SPAIN	|	O
currency	|	ESP	|	Spanish Peseta	|	ES	|	SPAIN	|	O
currency	|	SDP	|	Sudanese Pound	|	SD	|	SUDAN	|	O
currency	|	SRG	|	Suriname Guilder	|	SR	|	SURINAME	|	O
currency	|	UGS	|	Uganda Shilling	|	UG	|	UGANDA	|	O
currency	|	UAK	|	Karbovanet	|	UA	|	UKRAINE	|	O
currency	|	SUR	|	Rouble	|	SU	|	UNION OF SOVIET SOCIALIST REPUBLICS	|	O
currency	|	UYP	|	Uruguayan Peso	|	UY	|	URUGUAY	|	O
currency	|	YDD	|	Yemeni Dinar	|	YE	|	YEMEN, DEMOCRATIC	|	O
currency	|	YUD	|	New Yugoslavian Dinar	|	YU	|	YUGOSLAVIA	|	O
currency	|	YUM	|	Yugoslavian Dinar	|	YU	|	YUGOSLAVIA	|	O
currency	|	YUN	|	Yugoslavian Dinar	|	YU	|	YUGOSLAVIA	|	O
currency	|	ZRN	|	New Zaïre	|	ZR	|	ZAIRE	|	O
currency	|	ZRZ	|	Zaïre	|	ZR	|	ZAIRE	|	O
currency	|	ALK	|	Old Lek	|	AL	|	ALBANIA	|	X
currency	|	CNX	|	Peoples Bank Dollar	|	CN	|	CHINA	|	X
currency	|	ISJ	|	Old Krona	|	IS	|	ICELAND	|	X
currency	|	ILR	|	Old Shekel	|	IL	|	ISRAEL	|	X
currency	|	MAF	|	Mali Franc	|	ML	|	MALI	|	X
# the following three are not in ISO 4217, so these are guesses
currency	|	XXX	|	No universal currency	|	GS	|	SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS	|	C
currency	|	XXX	|	No universal currency	|	PS	|	PALESTINIAN TERRITORY, OCCUPIED	|	C
currency	|	EUR	|	Euro	|	AX	|	ÅLAND ISLANDS	|	C
# new codes, May 2007
currency	|	RSD	|	Serbian Dinar	|	RS	|	SERBIA	|	C
currency	|	SDG	|	Sudanese Pound	|	SD	|	SUDAN	|	C
currency	|	UYI	|	Uruguay Peso en Unidades Indexadas	|	UY	|	URUGUAY	|	F
currency	|	TMT	|	Manat	|	TM	|	TURKMENISTAN	|	C
currency	|	ZWR	|	Zimbabwe Dollar	|	ZW	|	ZIMBABWE	|	O
currency	|	ZWL	|	Zimbabwe Dollar	|	ZW	|	ZIMBABWE	|	C
currency	|	CUC	|	Peso Convertible	|	CU	|	CUBA	|	C
# the following were added based on http://www.unicode.org/cldr/data/dropbox/misc/CurrencyHistoryProposal.html
currency	|	ARL	|	Peso Ley	|	AR	|	ARGENTINA	|	O
currency	|	ARM	|	Peso Moneda Nacional	|	AR	|	ARGENTINA	|	O
currency	|	BAN	|	New Dinar	|	BA	|	BOSNIA & HERZEGOVINA	|	O
currency	|	BGM	|	Socialist Lev	|	BG	|	BULRARIA	|	O
currency	|	BGO	|	Old Lev	|	BG	|	BULRARIA	|	O
currency	|	BOL	|	Old Boliviano	|	BO	|	BOLIVIA	|	O
currency	|	BRZ	|	Old Cruziero	|	BR	|	BRAZIL	|	O
currency	|	CLE	|	Chilean Escudo	|	CL	|	CHILE	|	O
currency	|	KRH	|	Hwan	|	KR	|	KOREA, REPUBLIC OF	|	O
currency	|	KRO	|	Old Hwan	|	KR	|	KOREA, REPUBLIC OF	|	O
currency	|	MCF	|	Monaco Franc	|	MC	|	MONACO	|	O
currency	|	MDC	|	Moldovan Cupon	|	MD	|	MOLDOVA, REPUBLIC OF	|	O
currency	|	MKN	|	Old Macedonian Denar	|	MK	|	MACEDONIA, THE FORMER YUGOSLAV REPUBLIC OF	|	O
currency	|	VNN	|	Old Dong	|	VN	|	VIETNAM	|	O
currency	|	YUR	|	Reformed Dinar	|	YU	|	YUGOSLAVIA	|	O
";
        #endregion
    }

    #endregion
}
#endregion
