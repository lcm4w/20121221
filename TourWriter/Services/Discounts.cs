using System;
using System.Collections.Generic;
using System.Linq;
using TourWriter.Info;

namespace TourWriter.Services
{
    public class Discounts
    {
        public static decimal CalcDiscount(decimal bookingQty, IEnumerable<ItinerarySet.DiscountRow> discounts)
        {
            var applicableDiscounts = discounts.Where(x => x.UnitsUsed < bookingQty).OrderByDescending(x => x.UnitsUsed);
            var bestDiscount = applicableDiscounts.FirstOrDefault();

            var available = bookingQty - (bestDiscount != null ? bestDiscount.UnitsUsed : bookingQty);
            var allowed = bestDiscount != null ? bestDiscount.UnitsFree : 0;

            var calcFree = Math.Min(available, allowed);
            if (applicableDiscounts.Count() > 1)
                calcFree = Math.Max(calcFree, applicableDiscounts.ElementAt(1).UnitsFree);
            
            App.Debug(string.Format("Calc discount for qty: {0}, discount {1}/{2}, calc: {3}",
                                    bookingQty,
                                    bestDiscount != null ? bestDiscount.UnitsUsed.ToString() : "_",
                                    bestDiscount != null ? bestDiscount.UnitsFree.ToString() : "_",
                                    calcFree));

            return calcFree;
        }
    }
}
