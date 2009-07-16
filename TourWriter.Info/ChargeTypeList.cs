using System.Collections.Generic;

namespace TourWriter.Info
{
    public static class ChargeTypeList
    {
        private static List<string> chargeTypes;

        public static List<string> ChargeTypes
        {
            get
            {
                if (chargeTypes == null)
                {
                    chargeTypes = new List<string> { "GROUP", "PAX", "ROOM" };
                }
                return chargeTypes;
            }
        }
    }
}
