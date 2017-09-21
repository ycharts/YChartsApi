using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace YCharts.Api
{
    internal static class RequestService
    {        
        private const string UrlBase = "https://ycharts.com/api/v3/";
        private static HttpClient Client = null;

        public static async Task<JObject> GetInfo(string securityCollectionPath, List<string> symbols, List<string> fields)
        {
            // Some basic input validation
            if (symbols.Count < 1 || fields.Count < 1)
            {
                throw new YChartsException("symbols List and fields List must both contain at least 1 item");
            }

            // Form the URL path
            string symbolsParam = string.Join(",", symbols);
            string fieldsParam = string.Join(",", fields);
            string endpointPath = string.Format("{0}/info/{1}", symbolsParam, fieldsParam);
            // Make the Request
            JObject infoData = await GetApiData(securityCollectionPath, endpointPath);

            return infoData;
        }

        private static async Task<JObject> GetApiData(string securityCollectionPath, string path)
        {
            JObject rspData = null;
            string completePath = string.Format("{0}/{1}", securityCollectionPath, path);
            HttpClient client = GetHTTPClient();

            try
            {
                // Make the async request
                HttpResponseMessage rsp = await client.GetAsync(completePath);
                if (!rsp.IsSuccessStatusCode)
                {
                    int statusCode = (int)rsp.StatusCode;
                    switch (rsp.StatusCode)
                    {
                        case HttpStatusCode.RequestUriTooLong:
                            throw new YChartsException("Too many symbol or fields, ensure 100 or less of each", statusCode);
                        case HttpStatusCode.Forbidden:
                            throw new YChartsException("Api key is insufficient for requested access", statusCode);
                        case HttpStatusCode.Unauthorized:
                            throw new YChartsException("Missing API key header", statusCode);
                        default:
                            throw new YChartsException("Connection Error, please try again", statusCode);
                    }
                }
                // Now, Deserialize the response into a JObject
                string content = await rsp.Content.ReadAsStringAsync();
                rspData = JObject.Parse(content);
            }
            catch (HttpRequestException)
            {
                throw new YChartsException("API is down for maintenance, please try again");
            }
            return rspData;
        }

        private static bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // This pads us from the effects of a CA change. This is practically
            // not an issue, but if a client uses this library, it might be in place for years
            return true;
        }

        private static HttpClient GetHTTPClient()
        {
            if (string.IsNullOrEmpty(YChartsConfig.ApiKey))
            {
                throw new YChartsException("API key has not been provided");

            }
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            if (Client == null)
            {
                Client = new HttpClient();
                Client.BaseAddress = new Uri(UrlBase);
                Client.DefaultRequestHeaders.Add("X-YCHARTSAUTHORIZATION", YChartsConfig.ApiKey);
            }

            return Client;
        }
    }
}
