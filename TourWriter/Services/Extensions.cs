namespace TourWriter.Services
{
    public static class Extensions
    {
        #region Strings

        /// <summary>
        /// Encodes a string for Xml.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string XmlEncode(this string s)
        {
            //return System.Security.SecurityElement.Escape(s); // no Unescape method!
            return System.Web.HttpUtility.HtmlEncode(s);
        }

        /// <summary>
        /// Decodes a string from Xml.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string XmlDecode(this string s)
        {
            return System.Web.HttpUtility.HtmlDecode(s);
        }

        #endregion
    }
}
