using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace YCharts.Api
{
    /// <summary>API client for all indicator data endpoints</summary>
    public static class YChartsIndicatorClient
    {
        private const string SecurityCollectionsPath = "indicators";

        /// <summary>Requests data from the /info endpoint</summary>
        /// <param name="codes">List of strings of indicator codes</param>
        /// <param name="fields">List of strings corresponding to info field names</param>
        /// <returns>JObject representing the JSON response from the API</returns>
        /// <remarks>Note that this method is async and can throw a variety of YChartsException instances.
        /// Many of those exception instances will have an associated StatusCode value that a consumer
        /// should check.</remarks>
        public static async Task<JObject> GetInfo(List<string> codes, List<string> fields)
        {
            return await RequestService.GetInfo(SecurityCollectionsPath, codes, fields);
        }

        /// <summary>Requests data from the /points endpoint</summary>
        /// <param name="codes">List of strings of indicator codes</param>
        /// <param name="date">DateTime associated with the metric value</param>
        /// <returns>JObject representing the JSON response from the API</returns>
        /// <remarks>Note that this method is async and can throw a variety of YChartsException instances.
        /// Many of those exception instances will have an associated StatusCode value that a consumer
        /// should check.</remarks>
        public static async Task<JObject> GetPoints(List<string> codes, DateTime? date = null)
        {
            // Some basic input validation
            if (codes.Count < 1)
            {
                throw new YChartsException("codes list must both contain at least 1 item");
            }

            // Form the URL path
            string codesParam = string.Join(",", codes);
            string endpointPath = string.Format("{0}/points", codesParam);
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
            JObject pointData = await RequestService.GetApiData(SecurityCollectionsPath, endpointPath);

            return pointData;
        }

        /// <summary>Requests data from the /series endpoint</summary>
        /// <param name="codes">List of strings of indicator codes</param>
        /// <param name="startDate">DateTime start date of the series data range</param>
        /// <param name="endDate">DateTime end date of the series data range</param>
        /// <returns>JObject representing the JSON response from the API</returns>
        public static async Task<JObject> GetSeries(List<string> codes, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Some basic input validation
            if (codes.Count < 1)
            {
                throw new YChartsException("codes list must both contain at least 1 item");
            }

            // Form the URL path
            string codesParam = string.Join(",", codes);
            string endpointPath = string.Format("{0}/series", codesParam);
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
            JObject seriesData = await RequestService.GetApiData(SecurityCollectionsPath, endpointPath);

            return seriesData;
        }
    }
}
