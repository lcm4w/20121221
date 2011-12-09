using System;
using System.Text;
using System.Web;
using System.Data.SqlClient;

namespace Travelmesh
{
    class TrialDatabase
    {
        public static string Create(string companyName, string userName, string userEmail)
        {
            var data = string.Format("&companyName={0}&userName={1}&userEmail={2}", 
                HttpUtility.UrlEncode(companyName), HttpUtility.UrlEncode(userName), HttpUtility.UrlEncode(userEmail));
            
            var response = ApiWrapper.ApiRequest("/tourwriter/db", "POST", data, "application/x-www-form-urlencoded").Response;
            return response["key"].ToString();
        }

        public static bool TestConnection(string key, string expectedCompanyName)
        {
            using (var conn = new SqlConnection(DecodeKey(key)))
            {
                conn.Open();
                var cmd = new SqlCommand("select top(1) InstallName from [AppSettings]", conn);
                var rdr = cmd.ExecuteReader();
                rdr.Read();
                return expectedCompanyName == (string) rdr[0];
            }
        }
        
        private static string DecodeKey(string key)
        {
            key = key.Replace("\r\n", "").Replace("\n", "");
            var split = key.IndexOf(":", StringComparison.Ordinal) + 1;
            var enc = key.Substring(split, key.Length - split);
            var conn = Encoding.ASCII.GetString(Convert.FromBase64String(enc));
            return conn;
        }
    }
}
