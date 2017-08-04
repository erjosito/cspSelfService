using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using cspWeb.Properties;
using System.Threading.Tasks;

namespace cspWeb.Helpers
{
    public static class REST
    {

        public static string ApplicationDomain = System.Configuration.ConfigurationManager.AppSettings["CspTenantId"];
        public static string ApplicationId = System.Configuration.ConfigurationManager.AppSettings["AppId"];
        public static string ApplicationSecret = System.Configuration.ConfigurationManager.AppSettings["AppSecret"];
        public static string CspUsername = System.Configuration.ConfigurationManager.AppSettings["CspUsername"];
        public static string CspPassword = System.Configuration.ConfigurationManager.AppSettings["CspPassword"];
        public static string PowershellAppId = System.Configuration.ConfigurationManager.AppSettings["PowershellAppId"];


        public static JObject sendHttpRequest (string method, string url, string token = null, string payload = null, string contentType = "application/json", string accept = "application/json")
        {
            var request = WebRequest.Create(url);
            request.Method = method;
            if (token != null)
            {
                request.Headers.Add("Authorization", "Bearer " + token);
            }
            request.ContentType = contentType;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(payload);
            }
            try
            {
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    var adResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(responseContent);
                    return adResponse;
                }
            }
            catch (WebException webException)
            {
                if (webException.Response != null)
                {
                    using (var reader = new StreamReader(webException.Response.GetResponseStream()))
                    {
                        var responseContent = reader.ReadToEnd();
                        var adResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(responseContent);
                        return adResponse;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public static async Task<JObject> sendHttpRequestAsync(string method, string url, string token = null, string payload = null, string contentType = "application/json", string accept = "application/json")
        {
            var request = WebRequest.Create(url);
            request.Method = method;
            if (token != null)
            {
                request.Headers.Add("Authorization", "Bearer " + token);
            }
            request.ContentType = contentType;
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(payload);
            }
            try
            {
                var response = await request.GetResponseAsync();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    var adResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(responseContent);
                    return adResponse;
                }
            }
            catch (WebException webException)
            {
                if (webException.Response != null)
                {
                    using (var reader = new StreamReader(webException.Response.GetResponseStream()))
                    {
                        var responseContent = reader.ReadToEnd();
                        var adResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(responseContent);
                        return adResponse;
                    }
                }
                else
                {
                    return null;
                }
            }
        }


        public static string getCspToken()
        {
            string contentType = "application/x-www-form-urlencoded";
            string cspUrl = "https://login.windows.net/" + ApplicationDomain + "/oauth2/token";
            string resource = "https%3A%2F%2Fgraph.windows.net";
            string payload = "grant_type=client_credentials&resource=" + resource + "&client_id=" + ApplicationId + "&client_secret=" + ApplicationSecret;
            JObject jsonResponse = sendHttpRequest("POST", cspUrl, payload: payload, contentType: contentType);
            string token = jsonResponse["access_token"].ToString();
            if (token != null)
            {
                return token;
            }
            else
            {
                return null;
            }
        }

        public static async Task<string> getCspTokenAsync()
        {
            string contentType = "application/x-www-form-urlencoded";
            string cspUrl = "https://login.windows.net/" + ApplicationDomain + "/oauth2/token";
            string resource = "https%3A%2F%2Fgraph.windows.net";
            string payload = "grant_type=client_credentials&resource=" + resource + "&client_id=" + ApplicationId + "&client_secret=" + ApplicationSecret;
            JObject jsonResponse = await sendHttpRequestAsync("POST", cspUrl, payload: payload, contentType: contentType);
            string token = jsonResponse["access_token"].ToString();
            if (token != null)
            {
                return token;
            }
            else
            {
                return null;
            }
        }

        public static string getArmToken(string TenantId, bool UserAuth = false)
        {
            string contentType = "application/x-www-form-urlencoded";
            string url = "";
            string payload = "";
            if (UserAuth)
            //App+User auth
            {
                url = "https://login.windows.net/" + TenantId + "/oauth2/token?api-version=1.0";
                string resource = "https://management.azure.com/";
                payload = "grant_type=password&resource=" + resource
                           + "&username=" + CspUsername + "&password=" + CspPassword
                           + "&client_id=" + PowershellAppId + "&scope=openid";
            }
            else
            //App-only auth
            {
                url = "https://login.windows.net/" + TenantId + "/oauth2/token";
                string resource = "https://management.azure.com/";
                payload = "grant_type=client_credentials&resource=" + resource + "&client_id=" 
                          + ApplicationId + "&client_secret=" + ApplicationSecret;
            }
            JObject jsonResponse = sendHttpRequest("POST", url, payload: payload, contentType: contentType);
            string token = jsonResponse["access_token"].ToString();
            if (token != null)
            {
                return token;
            }
            else
            {
                return null;
            }
        }


        public static async Task<string> getArmTokenAsync(string TenantId, bool UserAuth = false)
        {
            string contentType = "application/x-www-form-urlencoded";
            string url = "";
            string payload = "";
            if (UserAuth)
            //App+User auth
            {
                url = "https://login.windows.net/" + TenantId + "/oauth2/token?api-version=1.0";
                string resource = "https://management.azure.com/";
                payload = "grant_type=password&resource=" + resource
                           + "&username=" + CspUsername + "&password=" + CspPassword
                           + "&client_id=" + PowershellAppId + "&scope=openid";
            }
            else
            //App-only auth
            {
                url = "https://login.windows.net/" + TenantId + "/oauth2/token";
                string resource = "https://management.azure.com/";
                payload = "grant_type=client_credentials&resource=" + resource + "&client_id="
                          + ApplicationId + "&client_secret=" + ApplicationSecret;
            }
            JObject jsonResponse = await sendHttpRequestAsync("POST", url, payload: payload, contentType: contentType);
            string token = null;
            try
            {
                token = jsonResponse["access_token"].ToString();
            }
            catch
            {
                return null;
            }
            return token;
        }
    }
}