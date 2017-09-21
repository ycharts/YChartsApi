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
    /// <summary>Base class for all security-specific client classes. It cannot be instantiated.</summary>
    public class BaseClient
    {
        // Static
        private const string UrlBase = "https://ycharts.com/api/v3/";
        private static HttpClient Client = null;
        // Instance
        private string ApiKey { get; set; }
        /// <summary></summary>
        protected string SecurityCollectionPath { get; set; }
        
        /// <summary></summary>
        protected BaseClient(string apiKey)
        {
            this.ApiKey = apiKey;
        }

        /// <summary>Requests data from the /info endpoint</summary>
        /// <param name="symbols">List of strings of security symbols</param>
        /// <param name="fields">List of strings of field names corresponding to a metric or info field</param>
        /// <returns>a JObject representing the JSON response from the server</returns>
        /// <remarks>Note that this method is async and can throw a variety of ApiException instances.
        /// Many of those exception instances will have an associated StatusCode value that a consumer
        /// should check.</remarks>
        public async Task<JObject> GetInfo(List<string> symbols, List<string> fields)
        {
            // Some basic input validation
            if (symbols.Count < 1 || fields.Count < 1)
            {
                throw new ApiException("symbols List and fields List must both contain at least 1 item");
            }

            // Form the URL path
            string symbolsParam = string.Join(",", symbols);
            string fieldsParam = string.Join(",", fields);
            string path = string.Format("{0}/info/{1}", symbolsParam, fieldsParam);
            // Make the Request
            JObject infoData = await GetApiData(path);

            return infoData;
        }

        private async Task<JObject> GetApiData(string path)
        {
            JObject rspData = null;
            string completePath = string.Format("{0}/{1}", this.SecurityCollectionPath, path);
            HttpClient client = GetHTTPClient(this.ApiKey);

            try
            {
                // Make the async request
                HttpResponseMessage rsp = await client.GetAsync(completePath);
                if (!rsp.IsSuccessStatusCode)
                {
                    int statusCode = (int)rsp.StatusCode;
                    switch(rsp.StatusCode)
                    {
                        case HttpStatusCode.RequestUriTooLong:
							throw new ApiException("Too many symbol or fields, ensure 100 or less of each", statusCode);
						case HttpStatusCode.Forbidden:
							throw new ApiException("Api key is insufficient for requested access", statusCode);
						case HttpStatusCode.Unauthorized:
							throw new ApiException("Missing API key header", statusCode);
                        default:
                            throw new ApiException("Connection Error, please try again", statusCode);
                    }
                }
                // Now, Deserialize the response into a JObject
                string content = await rsp.Content.ReadAsStringAsync();
                rspData = JObject.Parse(content);
            }
            catch (HttpRequestException)
            {
                throw new ApiException("API is down for maintenance, please try again");
            }
            return rspData;
        }

        private static bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // This pads us from the effects of a CA change. This is practically
            // not an issue, but if a client uses this library, it might be in place for years
            return true;
        }

        private static HttpClient GetHTTPClient(string apiKey)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            if (Client == null)
            {
                Client = new HttpClient();
                Client.BaseAddress = new Uri(UrlBase);
            }

            Client.DefaultRequestHeaders.Add("X-YCHARTSAUTHORIZATION", apiKey);

            return Client;
        }

    }
}
