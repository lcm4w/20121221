using System.IO;
using System.Net;

namespace Travelmesh
{
    class ApiWrapper
    {
        private static string TravelmeshUrl = (TourWriter.App.RuntimeMode == TourWriter.App.RuntimeModes.Release) ?
                                        "http://api.travelmesh.com" : // server: live/relese
                                        "http://localhost:17795";     // local: test

        const string ApiKey = "115ea538cedd5feae0ed4504ea3092223c4b5e9a";
        static readonly CookieContainer CookieJar = new CookieContainer();
        
        public static T ApiRequest<T>(string url, string httpMethod, string postData, string contentType)
        {
            var json = ApiRequest(url, httpMethod, postData, contentType);
            return ConvertFromJson<T>(json);
        }

        public static string ApiRequest(string url, string httpMethod, string postData, string contentType)
        {
            url = string.Format("{0}/{1}?key={2}", TravelmeshUrl.Trim('/'), url.Trim('/'), ApiKey);
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (request == null) return "";

            request.Timeout = 30000;
            request.Method = httpMethod;
            request.ContentType = contentType;
            request.CookieContainer = CookieJar;

            if (postData != null)
            {
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                    sw.Write(postData);
            }

            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                HttpWebResponse e; // for testing...
                if (ex.Response is HttpWebResponse)
                    e = ex.Response as HttpWebResponse;

                throw;
            }
            using (var s = response.GetResponseStream())
            using (var sr = new StreamReader(s))
                return sr.ReadToEnd();
        }

        public static string ConvertToJson(object o)
        {
            var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            return ser.Serialize(o);
        }

        public static T ConvertFromJson<T>(string json)
        {
            return new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<T>(json);
        }

        public static void PingServerAysnc()
        {
            new System.Threading.Thread(() =>
            {
                try { System.Diagnostics.Debug.WriteLine("Server said: " + ApiRequest("/hello", "GET", null, "application/json; charset=utf-8")); }
                catch (System.Exception ex) { }
            }).Start();
        }
    }
}
