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
    public class Currencies
    {
        public static Currency Single(string currencyCode)
        {
            var c = Cache.ToolSet.Currency.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
            return new Currency(c.CurrencyCode, c.CurrencyName, c.Symbol);
        }
        
        public static List<Currency> All()
        {
            return Cache.ToolSet.Currency.Select(c => new Currency(c.CurrencyCode, c.CurrencyName, c.Symbol)).ToList();
        }

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
            return !entity.IsCurrencyCodeNull() && !string.IsNullOrEmpty(entity.CurrencyCode.Trim()) ? entity.CurrencyCode : null;
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


        public class Currency
        {
            public string CurrencyCode { get; set; }
            public string CurrencyName { get; set; }
            public string PortableCurrencyPattern { get; set; }
            public Currency(string code, string name, string pattern) { CurrencyCode = code; CurrencyName = name; PortableCurrencyPattern = pattern; }
        }

        #region Import list
        public static readonly List<Currency> CurrencyImportList =
            new List<Currency>
                {
                    new Currency("AED", "UAE Dirham", "'DH'#,##0.00"),
                    new Currency("AFN", "Afghani", "'AFN'#,##0.00"),
                    new Currency("ALL", "Lek", "'Lek'#,##0.00"),
                    new Currency("AMD", "Armenian Dram", "#,##0.00'dram'"),
                    new Currency("ANG", "Netherlands Antillian Guikder", "'NAƒ'#,##0.00"),
                    new Currency("AOA", "Kwanza", "'Kz'#,##0.00"),
                    new Currency("ARS", "Argentine Peso", "'AR$'#,##0.00"),
                    new Currency("AUD", "Australian Dollar", "'AU$'#,##0.00"),
                    new Currency("AWG", "Aruban Guilder", "'Afl.'#,##0.00"),
                    new Currency("AZN", "Azerbaijanian Manat", "'man'#,##0.00"),
                    new Currency("BAM", "Convertible Marks", "'KM'#,##0.00"),
                    new Currency("BBD", "Barbados Dollar", "'Bds$'#,##0.00"),
                    new Currency("BDT", "Taka", "'Tk'#,##0.00"),
                    new Currency("BGN", "Bulgarian Lev", "#,##0.00'лв'"),
                    new Currency("BHD", "Bahraini Dinar", "'BD'#,##0.000"),
                    new Currency("BIF", "Burundi Franc", "'FBu'#,##0"),
                    new Currency("BMD", "Bermudian Dollar)", "'BD$'#,##0.00"),
                    new Currency("BND", "Brunei Dollar", "'B$'#,##0.00"),
                    new Currency("BOB", "Boliviano", "'B$'#,##0.00"),
                    new Currency("BRL", "Brazilian Real", "'R$'#,##0.00"),
                    new Currency("BSD", "Bahamian Dollar", "'B$'#,##0.00"),
                    new Currency("BTN", "Ngultrum", "'Nu.'#,##0.00"),
                    new Currency("BWP", "Pula", "'pula'#,##0.00"),
                    new Currency("BYR", "Belarussian Ruble", "'Br'#,##0"),
                    new Currency("BZD", "Belize Dollar", "'BZ$'#,##0.00"),
                    new Currency("CAD", "Canadian Dollar", "'C$'#,##0.00"),
                    new Currency("CDF", "Franc Congolais", "'CDF'#,##0.00"),
                    new Currency("CHF", "Swiss Franc", "'CHF'#,##0.00"),
                    new Currency("CLP", "Chilean Peso", "'CL$'#,##0"),
                    new Currency("CNY", "Yuan Renminbi", "'RMB¥'#,##0.00"),
                    new Currency("COP", "Colombian Peso", "'COL$'#,##0.00"),
                    new Currency("CRC", "Costa Rican Colon", "'CR₡'#,##0.00"),
                    new Currency("CUP", "Cuban Peso", "'$MN'#,##0.00"),
                    new Currency("CVE", "Cape Verde Escudo", "'Esc'#,##0.00"),
                    new Currency("CZK", "Czech Koruna", "#,##0.00'Kč'"),
                    new Currency("DJF", "Djibouti Franc", "'Fdj'#,##0"),
                    new Currency("DKK", "Danish Krone", "#,##0.00'kr'"),
                    new Currency("DOP", "Dominican Peso", "'RD$'#,##0.00"),
                    new Currency("DZD", "Algerian Dinar", "'DA'#,##0.00"),
                    new Currency("EEK", "Kroon", "#,##0.00'EEK'"),
                    new Currency("EGP", "Egyptian Pound", "'LE'#,##0.00"),
                    new Currency("ERN", "Nakfa", "'Nfk'#,##0.00"),
                    new Currency("ETB", "Ethiopian Birr", "'Br'#,##0.00"),
                    new Currency("EUR", "Euro", "#,##0.00'€'"),
                    new Currency("FJD", "Fiji Dollar", "'FJ$'#,##0.00"),
                    new Currency("FKP", "Falkland Islands Pound", "'FK£'#,##0.00"),
                    new Currency("GBP", "Pound Sterling", "'GB£'#,##0.00"),
                    new Currency("GEL", "Lari", "'GEL'#,##0.00"),
                    new Currency("GHS", "Cedi", "'GHS¢'#,##0.00"),
                    new Currency("GIP", "Gibraltar Pound", "'GI£'#,##0.00"),
                    new Currency("GMD", "Dalasi", "'GMD'#,##0.00"),
                    new Currency("GNF", "Guinea Franc", "'FG'#,##0"),
                    new Currency("GTQ", "Quetzal", "'GTQ'#,##0.00"),
                    new Currency("GYD", "Guyana Dollar", "'GY$'#,##0.00"),
                    new Currency("HKD", "Hong Kong Dollar", "'HK$'#,##0.00"),
                    new Currency("HNL", "Lempira", "'HNL'#,##0.00"),
                    new Currency("HRK", "Croatian Kuna", "'kn'#,##0.00"),
                    new Currency("HTG", "Gourde", "'HTG'#,##0.00"),
                    new Currency("HUF", "Forint", "#,##0.00'Ft'"),
                    new Currency("IDR", "Rupiah", "'Rp'#,##0.00"),
                    new Currency("ILS", "New Israeli Sheqel", "#,##0.00'IL₪'"),
                    new Currency("INR", "Indian Rupee", "'Rs'#,##0.00"),
                    new Currency("IQD", "Iraqi Dinar", "'IQD'#,##0.000"),
                    new Currency("IRR", "Iranian Rial", "'IRR'#,##0.00"),
                    new Currency("ISK", "Iceland Krona", "#,##0.00'kr'"),
                    new Currency("JMD", "Jamaican Dollar", "'JA$'#,##0.00"),
                    new Currency("JOD", "Jordanian Dinar", "'JOD'#,##0.000"),
                    new Currency("JPY", "Yen", "'JP¥'#,##0"),
                    new Currency("KES", "Kenyan Shilling", "'KSh'#,##0.00"),
                    new Currency("KGS", "Som", "'som'#,##0.00"),
                    new Currency("KHR", "Riel", "#,##0.00'KHR'"),
                    new Currency("KMF", "Comoro Franc", "'KMF'#,##0"),
                    new Currency("KPW", "North Korean Won", "'KPW'#,##0.00"),
                    new Currency("KRW", "Won", "'KR₩'#,##0"),
                    new Currency("KWD", "Kuwaiti Dinar", "'KWD'#,##0.000"),
                    new Currency("KYD", "Cayman Islands Dollar", "'CI$'#,##0.00"),
                    new Currency("KZT", "Tenge", "#,##0.00'KZT'"),
                    new Currency("LAK", "Kip", "'LA₭'#,##0.00"),
                    new Currency("LBP", "Lebanese Pound", "'LBP'#,##0.00"),
                    new Currency("LKR", "Sri Lanka Rupee", "'SLRs'#,##0.00"),
                    new Currency("LRD", "Liberian Dollar", "'L$'#,##0.00"),
                    new Currency("LSL", "Loti", "'LSL'#,##0.00"),
                    new Currency("LTL", "Lithuanian Litas", "#,##0.00'Lt'"),
                    new Currency("LVL", "Latvian Lats", "#,##0.00'Ls'"),
                    new Currency("LYD", "Libyan Dinar", "'LD'#,##0.000"),
                    new Currency("MAD", "Moroccan Dirham", "'MAD'#,##0.00"),
                    new Currency("MDL", "Moldovan Leu", "'MDL'#,##0.00"),
                    new Currency("MGA", "Malagasy Ariary", "'MGA'#,##0.0"),
                    new Currency("MKD", "Denar", "'MKD'#,##0.00"),
                    new Currency("MMK", "Kyat", "'MMK'#,##0.00"),
                    new Currency("MNT", "Tugrik", "'MN₮'#,##0.00"),
                    new Currency("MOP", "Pataca", "'MOP$'#,##0.00"),
                    new Currency("MRO", "Ouguiya", "'UM'#,##0.0"),
                    new Currency("MUR", "Mauritius Rupee", "'MURs'#,##0.00"),
                    new Currency("MVR", "Rufiyaa", "'MRF'#,##0.00"),
                    new Currency("MWK", "Kwacha", "'MK'#,##0.00"),
                    new Currency("MXN", "Mexican Peso", "'Mex$'#,##0.00"),
                    new Currency("MYR", "Malaysian Ringgit", "'RM'#,##0.00"),
                    new Currency("MZN", "Merical", "'MTn'#,##0.00"),
                    new Currency("NAD", "Namibian Dollar", "'N$'#,##0.00"),
                    new Currency("NGN", "Naira", "'NG₦'#,##0.00"),
                    new Currency("NIO", "Cordoba Oro", "'C$'#,##0.00"),
                    new Currency("NOK", "Norwegian Krone", "#,##0.00'NOkr'"),
                    new Currency("NPR", "Nepalese Rupee", "'NPRs'#,##0.00"),
                    new Currency("NZD", "New Zealand Dollar", "'NZ$'#,##0.00"),
                    new Currency("OMR", "Rial Omani", "'OMR'#,##0.000"),
                    new Currency("PAB", "Balboa", "'B/.'#,##0.00"),
                    new Currency("PEN", "Nuevo Sol", "'S/.'#,##0.00"),
                    new Currency("PGK", "Kina", "'PGK'#,##0.00"),
                    new Currency("PHP", "Philippine Peso", "'PHP'#,##0.00"),
                    new Currency("PKR", "Pakistan Rupee", "'PKRs.'#,##0.00"),
                    new Currency("PLN", "Zloty", "#,##0.00'zł'"),
                    new Currency("PYG", "Guarani", "'PYG'#,##0"),
                    new Currency("QAR", "Qatari Rial", "'QR'#,##0.00"),
                    new Currency("RON", "Leu", "'RON'#,##0.00"),
                    new Currency("RSD", "Serbian Dinar", "'RSD'#,##0.00"),
                    new Currency("RUB", "Russian Ruble", "#,##0.00'руб'"),
                    new Currency("RWF", "Rwanda Franc", "'RF'#,##0"),
                    new Currency("SAR", "Saudi Riyal", "'SR'#,##0.00"),
                    new Currency("SBD", "Solomon Islands Dollar", "'SI$'#,##0.00"),
                    new Currency("SCR", "Seychelles Rupee", "'SCR'#,##0.00"),
                    new Currency("SDG", "Sudanese Pound", "'SDG'#,##0.00"),
                    new Currency("SEK", "Swedish Krona", "#,##0.00'kr'"),
                    new Currency("SGD", "Singapore Dollar", "'S$'#,##0.00"),
                    new Currency("SHP", "Saint Helena Pound", "'SH£'#,##0.00"),
                    new Currency("SKK", "Slovak Koruna", "#,##0.00'Sk'"),
                    new Currency("SLL", "Leone", "'Le'#,##0.00"),
                    new Currency("SOS", "Somali Shilling", "'So. Sh.'#,##0.00"),
                    new Currency("SRD", "Surinam Dollar", "'SR$'#,##0.00"),
                    new Currency("STD", "Dobra", "'Db'#,##0.00"),
                    new Currency("SYP", "Syrian Pound", "'SYP'#,##0.00"),
                    new Currency("SZL", "Lilangeni", "'SZL'#,##0.00"),
                    new Currency("THB", "Baht", "'THB'#,##0.00"),
                    new Currency("TJS", "Somoni", "'TJS'#,##0.00"),
                    new Currency("TMM", "Manat", "'TMM'#,##0.00"),
                    new Currency("TND", "Tunisian Dinar", "'DT'#,##0.000"),
                    new Currency("TOP", "Tongan Pa'anga", "'T$'#,##0.00"),
                    new Currency("TRY", "new Turkish Lira", "'YTL'#,##0.00"),
                    new Currency("TTD", "Trinidad and Tobago Dollar", "'TT$'#,##0.00"),
                    new Currency("TWD", "New Taiwan Dollar", "'NT$'#,##0.00"),
                    new Currency("TZS", "Tanzanian Shilling", "#,##0.00'TZS'"),
                    new Currency("UAH", "Hryvnia", "#,##0.00'грн'"),
                    new Currency("UGX", "Uganda Shilling", "'USh'#,##0.00"),
                    new Currency("USD", "US Dollar", "'US$'#,##0.00"),
                    new Currency("UYU", "Peso Uruguayo", "'UY$'#,##0.00"),
                    new Currency("UZS", "Uzbekistan Sum", "'UZS'#,##0.00"),
                    new Currency("VEF", "Bolivar Fuerte", "'Bs.F'#,##0.00"),
                    new Currency("VND", "Dong", "#,##0.00'VN₫'"),
                    new Currency("VUV", "Vatu", "'Vt'#,##0"),
                    new Currency("WST", "Tala", "'WS$'#,##0.00"),
                    new Currency("XAF", "CFA Franc", "'FCFA'#,##0"),
                    new Currency("XCD", "East Caribbean Dollar", "'EC$'#,##0.00"),
                    new Currency("XOF", "CFA Franc BCEAO †", "'CFA'#,##0"),
                    new Currency("XPF", "CFP Franc", "'XPF'#,##0"),
                    new Currency("YER", "Yemeni Rial", "'YER'#,##0.00"),
                    new Currency("ZAR", "Rand", "'ZAR'#,##0.00"),
                    new Currency("ZMK", "Kwacha", "'ZK'#,##0.00"),
                    new Currency("ZWL", "Zimbabwe Dollar", "'ZW$'#,##0.00"),
                };
        #endregion
    }
}