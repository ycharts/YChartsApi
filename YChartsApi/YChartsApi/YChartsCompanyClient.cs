using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace YCharts.Api
{
    /// <summary>API client for all company data endpoints</summary>
    public static class YChartsCompanyClient
    {
        private const string SecurityCollectionsPath = "companies";
        
        /// <summary>Requests data from the /info endpoint</summary>
        /// <param name="symbols">List of strings of company symbols</param>
        /// <param name="fields">List of strings coresponding to info field names</param>
        /// <returns>JObject representing the JSON response from the server</returns>
        /// <remarks>Note that this method is async and can throw a variety of ApiException instances.
        /// Many of those exception instances will have an associated StatusCode value that a consumer
        /// should check.</remarks>
        public static async Task<JObject> GetInfo(List<string> symbols, List<string> fields)
        {
            return await RequestService.GetInfo(SecurityCollectionsPath, symbols, fields);
        }

        /// <summary>Requests data from the /points endpoint</summary>
        /// <param name="symbols">List of strings of company symbols</param>
        /// <param name="metrics">List of strings coresponding to metric names</param>
        /// <param name="date">Datetime associated with the metric value</param>
        /// <returns>JObject representing the JSON response from the server</returns>
        /// <remarks>Note that this method is async and can throw a variety of ApiException instances.
        /// Many of those exception instances will have an associated StatusCode value that a consumer
        /// should check.</remarks>
        public static async Task<JObject> GetPoints(List<string> symbols, List<string> metrics, DateTime? date = null)
        {
            return await RequestService.GetPoints(SecurityCollectionsPath, symbols, metrics, date);
        }

        /// <summary>Requests data from the /series endpoint</summary>
        /// <param name="symbols">List of strings of company symbols</param>
        /// <param name="metrics">List of strings coresponding to metric names</param>
        /// <param name="startDate">DaeTime start date of the series data range</param>
        /// <param name="endDate">DaeTime end date of the series data range</param>
        /// <returns>JObject representing the JSON response from the server</returns>
        public static async Task<JObject> GetSeries(List<string> symbols, List<string> metrics, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await RequestService.GetSeries(SecurityCollectionsPath, symbols, metrics, startDate, endDate);
        }

        /// <summary>Requests data from the /dividends endpoint</summary>
        /// <param name="symbols">List of strings of company symbols</param>
        /// <param name="startDate">DaeTime start date of the series data range</param>
        /// <param name="endDate">DaeTime end date of the series data range</param>
        /// <returns>JObject representing the JSON response from the server</returns>
        public static async Task<JObject> GetDividends(List<string> symbols, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await GetResourceData("dividends", symbols, startDate, endDate);
        }

        /// <summary>Requests data from the /splits endpoint</summary>
        /// <param name="symbols">List of strings of company symbols</param>
        /// <param name="startDate">DaeTime start date of the series data range</param>
        /// <param name="endDate">DaeTime end date of the series data range</param>
        /// <returns>JObject representing the JSON response from the server</returns>
        public static async Task<JObject> GetStockSplits(List<string> symbols, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await GetResourceData("splits", symbols, startDate, endDate);
        }

        /// <summary>Requests data from the /spinoiffs endpoint</summary>
        /// <param name="symbols">List of strings of company symbols</param>
        /// <param name="startDate">DaeTime start date of the series data range</param>
        /// <param name="endDate">DaeTime end date of the series data range</param>
        /// <returns>JObject representing the JSON response from the server</returns>
        public static async Task<JObject> GetStockSpinoffs(List<string> symbols, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await GetResourceData("spinoffs", symbols, startDate, endDate);
        }

        /// <summary>interfaces directly with the Request Service 
        /// to retrieve api data. This is used by the dividends,splits
        /// and spinoffs client methods.</summary>
        private static async Task<JObject> GetResourceData(string resourceCollectionPath, List<string> symbols, DateTime? startDate = null, DateTime? endDate = null)
        {            
            // Some basic input validation
            if (symbols.Count < 1)
            {
                throw new YChartsException("symbols List must contain at least 1 item");
            }

            // Form the URL path
            string symbolsParam = string.Join(",", symbols);
            string endpointPath = string.Format("{0}/{1}", symbolsParam, resourceCollectionPath);
            NameValueCollection queryParams = HttpUtility.ParseQueryString("");

            // convert the date to a string if we have it
            if (startDate.HasValue)
            {
                string queryDate = startDate.Value.ToString("yyyy-MM-dd");
                queryParams["start_date"] = queryDate;
            }

            if (endDate.HasValue)
            {
                string queryDate = endDate.Value.ToString("yyyy-MM-dd");
                queryParams["end_date"] = queryDate;
            }

            if (queryParams.Count > 0)
            {
                // This ToString method is special; it url encodes and adds =, &
                endpointPath = string.Format("{0}?{1}", endpointPath, queryParams.ToString());
            }

            // Make the Request
            JObject resourceData = await RequestService.GetApiData(SecurityCollectionsPath, endpointPath);

            return resourceData;
        }
    }
}
