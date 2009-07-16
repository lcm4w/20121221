using System;

namespace TourWriter.Services
{
    public class ExcelHelper
    {
        public static readonly object Missing = System.Reflection.Missing.Value;

        /// <summary>
        /// Searches for a tag in a specified excel range, and replaces it with a value.
        /// If the range is null, it will search the entire excel sheet.
        /// </summary>
        public static void ReplaceTag(string tagName, object value, Excel.Range range)
        {
            Excel.Range firstFind = null;
            Excel.Range currentFind;

            currentFind = range.Find(tagName, Missing, Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false, Missing, Missing);

            while (currentFind != null)
            {
                if (firstFind == null)
                    firstFind = currentFind;

                // break if the finder returns to the first find
                else if (currentFind.get_Address(Missing, Missing, Excel.XlReferenceStyle.xlA1, Missing, Missing)
                    == firstFind.get_Address(Missing, Missing, Excel.XlReferenceStyle.xlA1, Missing, Missing))
                {
                    break;
                }

                currentFind.Value2 = currentFind.Value2.ToString().Replace(tagName, value.ToString());
                currentFind = range.FindNext(currentFind);
            }
        }

        /// <summary>
        /// Converts a numeric row/column to a string representation.
        /// e.g. "row 5, column 1" becomes "A5"
        /// </summary>
        public static string GetCellPosition(int row, int column)
        {
            return String.Format("{0}{1}", (char)(column + 64), row);
        }

    }
}
