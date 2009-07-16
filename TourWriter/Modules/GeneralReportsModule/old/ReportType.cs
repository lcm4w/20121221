using System.Collections.Generic;

namespace TourWriter.Modules.ReportsModule
{
    public class ReportType
    {
        public string SqlViewName { get; set; }
        public string DisplayName { get; set; }

        public static List<ReportType> GetList()
        {
            return new List<ReportType>
            {
                new ReportType { SqlViewName = "ItineraryClientDetail", DisplayName = "Itinerary clients" },
                new ReportType { SqlViewName = "ItineraryDetail", DisplayName = "Itinerary details" },
                new ReportType { SqlViewName = "ItineraryPaymentDetail" , DisplayName = "Itinerary payments" },
                new ReportType { SqlViewName = "ItinerarySaleAllocationDetail" , DisplayName = "Itinerary sales" },
                new ReportType { SqlViewName = "ItineraryServiceTypeDetail" , DisplayName = "Itinerary service types" },
                new ReportType { SqlViewName = "PurchaseItemDetail" , DisplayName = "Booking items" },
                new ReportType { SqlViewName = "PurchaseItemPaymentsDetail" , DisplayName = "Supplier payments" },
                new ReportType { SqlViewName = "SupplierDetail" , DisplayName = "Supplier details" },
                new ReportType { SqlViewName = "SupplierRatesDetail" , DisplayName = "Supplier rates" },
            };
        }
    }
}
