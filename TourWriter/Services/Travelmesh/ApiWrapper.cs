using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using TourWriter;
using TourWriter.Properties;
using TourWriter.Utilities.Encryption;

namespace Travelmesh
{
    class ApiWrapper
    {
        private const string ApiKey = "BAS7A9TKNJP57UBB4TMX";
        private static string TravelmeshUrl = (App.RuntimeMode == App.RuntimeModes.Release) ?
                                        "http://api.travelmesh.com" : // server: live/relese
                                        "http://localhost:17795";     // local: test
        
        static readonly CookieContainer CookieJar = new CookieContainer();

        public static ApiResponse ApiRequest(string url, string httpMethod, string postData, string contentType)
        {
            var fullUrl = String.Format("{0}/{1}", TravelmeshUrl.Trim('/'), url.Trim('/'));
            var request = WebRequest.Create(fullUrl) as HttpWebRequest;
            if (request == null) throw new Exception("Failed to create Request object");

            request.Timeout = 30000; 
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = httpMethod;
            request.ContentType = contentType;
            //request.CookieContainer = CookieJar;
            request.Headers["Authorization"] = GetAuth(ApiKey);
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

            // handle auth
            if (apiResponse.Meta.Status == 401)
            {
                if (Login()) 
                    return ApiRequest(url, httpMethod, postData, contentType);
                throw new Exception("Not logged in");
            }

            // handle error
            else if (apiResponse.Meta.Status != 200)
            {
                var meta = apiResponse.Meta;
                var ex = new ApiException(meta.Status, meta.ErrorMessage + ". Detail: " + meta.ErrorDetail);
                throw ex;
            }

            return apiResponse;
        }

        public static string ConvertToJson(object o)
        {
            var ser = new JavaScriptSerializer();
            return ser.Serialize(o);
        }

        public static T ConvertFromJson<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }

        public static void PingServerAysnc()
        {
            new Thread(() =>
            {
                try { Debug.WriteLine("Server said: " + ApiRequest("/hello", "GET", null, "application/json; charset=utf-8")); }
                catch (Exception ex) { }
            }).Start();
        }

        private static bool Login()
        {
            var login = new Login();
            if (login.ShowDialog() == DialogResult.OK)
            {
                SetAuth(login.User, login.Pass);
                return true;
            }
            return false;
        }

        private static string GetAuth(string apiKey)
        {
            // get user/pass from properties
            string user = "", pass = "";
            var x = Settings.Default.TmSyncUser;
            if (x.Length > 0) x = EncryptionHelper.DecryptString(x);
            var s = x.Split(':');
            if (s.Length > 1)
            {
                user = s[0];
                pass = s[1];
            }

            // format basic auth
            var auth = String.Format("{0}:{1}:{2}", apiKey, user, pass);
            var enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));
            return "Basic " + enc;
        }

        private static void SetAuth(string user, string pass)
        {
            var x = EncryptionHelper.EncryptString(String.Format("{0}:{1}", user, pass));
            Settings.Default.TmSyncUser = x;
            Settings.Default.Save();
        }

        public class ApiResponse
        {
            public Meta Meta { get; set; }
            public Dictionary<string, object> Response { get; set; }
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
