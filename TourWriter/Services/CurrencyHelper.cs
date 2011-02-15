using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using TourWriter.Global;

namespace TourWriter.Services
{
    public class Currencies
    {
        public static Info.ToolSet.CurrencyRow Single(string currencyCode)
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
    }
}