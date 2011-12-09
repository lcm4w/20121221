using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Travelmesh
{
    class ApiWrapper
    {
        private static string TravelmeshUrl = (TourWriter.App.RuntimeMode == TourWriter.App.RuntimeModes.Release) ?
                                        "http://api.travelmesh.com" : // server: live/relese
                                        "http://localhost:17795";     // local: test


        static readonly string Auth = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(
            string.Format("{0}:{1}:{2}", "BAS7A9TKNJP57UBB4TMX", "seba@tourwriter.com", "seba")));

        static readonly CookieContainer CookieJar = new CookieContainer();
        
/*      public static T ApiRequest<T>(string url, string httpMethod, string postData, string contentType)
        {
            var json = ApiRequest(url, httpMethod, postData, contentType);
            var t = ConvertFromJson<T>(json);
            return t;
        }
*/
        public static ApiResponse ApiRequest(string url, string httpMethod, string postData, string contentType)
        {
            url = string.Format("{0}/{1}", TravelmeshUrl.Trim('/'), url.Trim('/'));
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (request == null) throw new Exception("Failed to create Request object");

            request.Timeout = 30000;
            request.Method = httpMethod;
            request.ContentType = contentType;
            //request.CookieContainer = CookieJar;
            request.Headers["Authorization"] = Auth;
            if (postData != null)
            {
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                    sw.Write(postData);
            }

            var responseText = "";
            HttpContext httpContext;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response; // api return error
                if (response == null) throw ex; // local error
            }
            finally
            {
                // get response string
                if (response != null)
                    using (var reader = new StreamReader(response.GetResponseStream()))
                        responseText = reader.ReadToEnd();
                httpContext = HttpContext.Current;
            }

            var apiResponse = ConvertFromJson<ApiResponse>(responseText);

            // handle error
            if (apiResponse.Meta.Status != 200)
            {
                var meta = apiResponse.Meta;
                var ex = new ApiException(meta.Status, meta.ErrorMessage + ". Detail: " + meta.ErrorDetail);
                throw ex;
            }

            return apiResponse;
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

        public class ApiResponse
        {
            public Meta Meta { get; set; }
            public Dictionary<string,object> Response { get; set; }
        }

        public class Meta
        {
            public int Status;
            public string ErrorMessage;
            public string ErrorDetail;
            public Exception LocalException;
        }

        public class ApiException : HttpException
        {
            public string Detail { get { return Data["Detail"].ToString(); } }

            public ApiException(int httpCode, string message) : base(httpCode, message) { }

            public bool IsValidationException
            {
                get { return GetHttpCode() == 400; }
            }
        }
    }
}
