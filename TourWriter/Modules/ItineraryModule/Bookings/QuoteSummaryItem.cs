using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TourWriter.Modules.ItineraryModule.Bookings
{
    internal class QuoteSummaryItem
    {
        public string Key { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Margin { get; set; }
        public decimal Markup { get; set; }
        public decimal Override { get; set; }

        public decimal Total
        {
            get
            {
                if (this.Override > 0)
                {
                    return this.Override;
                }

                return (this.Subtotal + this.Margin) * (1 + this.Markup / 100);
            }
        }
    }
}
