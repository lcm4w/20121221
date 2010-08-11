using System.Collections.Generic;
using System.Globalization;

namespace TourWriter.Info
{

    public class DepositTypeList
    {
        public static List<DepositType> GetDepositTypes(CultureInfo cultureInfo)
        {
            return new List<DepositType>
                       {
                           new DepositType('c', cultureInfo.NumberFormat.CurrencySymbol),
                           new DepositType('p', cultureInfo.NumberFormat.PercentSymbol)
                       };
        }
    }

    public class DepositType
    {
        public char Id { get; set; }

        public string Text { get; set; }

        public DepositType(char id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}
