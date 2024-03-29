﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
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

        public static async Task<JObject> GetPoints(string securityCollectionPath, List<string> symbols, List<string> metrics, DateTime? date = null)
        {
            // Some basic input validation
            if (symbols.Count < 1 || metrics.Count < 1)
            {
                throw new YChartsException("symbols List and metrics List must both contain at least 1 item");
            }

            // Form the URL path
            string symbolsParam = string.Join(",", symbols);
            string fieldsParam = string.Join(",", metrics);
            string endpointPath = string.Format("{0}/points/{1}", symbolsParam, fieldsParam);
            NameValueCollection queryParams = HttpUtility.ParseQueryString("");

            // convert the date to a string if we have it
            if (date.HasValue)
            {
                string queryDate = date.Value.ToString("yyyy-MM-dd");
                queryParams["date"] = queryDate;
            }

            if (queryParams.Count > 0)
            {
                // This ToString method is special; it url encodes and adds =, &
                endpointPath = string.Format("{0}?{1}", endpointPath, queryParams.ToString());
            }
            // Make the Request
            JObject pointData = await GetApiData(securityCollectionPath, endpointPath);

            return pointData;
        }

        public static async Task<JObject> GetSeries(string securityCollectionPath, List<string> symbols, List<string> metrics, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Some basic input validation
            if (symbols.Count < 1 || metrics.Count < 1)
            {
                throw new YChartsException("symbols List and metrics List must both contain at least 1 item");
            }

            // Form the URL path
            string symbolsParam = string.Join(",", symbols);
            string fieldsParam = string.Join(",", metrics);
            string endpointPath = string.Format("{0}/series/{1}", symbolsParam, fieldsParam);
            // Handle query params
            NameValueCollection queryParams = HttpUtility.ParseQueryString("");

            // convert the date to a string if we have it
            if (startDate.HasValue)
            {
                queryParams["start_date"] = startDate.Value.ToString("yyyy-MM-dd");
            }

            if (endDate.HasValue)
            {
                queryParams["end_date"] = endDate.Value.ToString("yyyy-MM-dd");
            }

            if (queryParams.Count > 0)
            {
                // This ToString method is special; it url encodes and adds =, &
                endpointPath = string.Format("{0}?{1}", endpointPath, queryParams.ToString());
            }

            // Make the Request
            JObject seriesData = await GetApiData(securityCollectionPath, endpointPath);

            return seriesData;
        }

        public static async Task<JObject> GetApiData(string securityCollectionPath, string path)
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
                            throw new YChartsException("Too many symbols or fields, ensure 100 or less of each", statusCode);
                        case HttpStatusCode.Forbidden:
                            throw new YChartsException("API key is insufficient for requested access", statusCode);
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
            if (string.IsNullOrWhiteSpace(YChartsConfig.ApiKey))
            {
                throw new YChartsException("API key has not been provided");

            }
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            if (Client == null)
            {
                Client = new HttpClient()
                {
                    BaseAddress = new Uri(UrlBase)
                };
                Client.DefaultRequestHeaders.Add("X-YCHARTSAUTHORIZATION", YChartsConfig.ApiKey);
            }

            return Client;
        }
    }
}
