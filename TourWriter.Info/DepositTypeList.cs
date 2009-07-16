using System.Collections.Generic;

namespace TourWriter.Info
{

    public class DepositTypeList
    {
        public static List<DepositType> depositType;

        public static List<DepositType> DepositTypes
        {
            get
            {
                if (depositType == null)
                {
                    depositType = new List<DepositType>();
                    depositType.Add(new DepositType('c', "$"));
                    depositType.Add(new DepositType('p', "%"));
                }
                return depositType;
            }
        }
    }

    public class DepositType
    {
        private char _id;
        private string _text;

        public char ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public DepositType(char id, string text)
        {
            _id = id;
            _text = text;
        }
    }
}
