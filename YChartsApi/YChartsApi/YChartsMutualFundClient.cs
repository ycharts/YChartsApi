using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace YCharts.Api
{
    /// <summary>API client for all mutual fund data endpoints</summary>
    public static class YChartsMutualFundClient
    {
        private const string SecurityCollectionsPath = "mutual_funds";

        /// <summary>Requests data from the /info endpoint</summary>
        /// <param name="symbols">List of strings of mutual fund symbols</param>
        /// <param name="fields">List of strings corresponding to info field names</param>
        /// <returns>JObject representing the JSON response from the API</returns>
        /// <remarks>Note that this method is async and can throw a variety of YChartsException instances.
        /// Many of those exception instances will have an associated StatusCode value that a consumer
        /// should check.</remarks>
        public static async Task<JObject> GetInfo(List<string> symbols, List<string> fields)
        {
            return await RequestService.GetInfo(SecurityCollectionsPath, symbols, fields);
        }

        /// <summary>Requests data from the /points endpoint</summary>
        /// <param name="symbols">List of strings of mutual fund symbols</param>
        /// <param name="metrics">List of strings corresponding to metric names</param>
        /// <param name="date">DateTime associated with the metric value</param>
        /// <returns>JObject representing the JSON response from the API</returns>
        /// <remarks>Note that this method is async and can throw a variety of YChartsException instances.
        /// Many of those exception instances will have an associated StatusCode value that a consumer
        /// should check.</remarks>
        public static async Task<JObject> GetPoints(List<string> symbols, List<string> metrics, DateTime? date = null)
        {
            return await RequestService.GetPoints(SecurityCollectionsPath, symbols, metrics, date);
        }

        /// <summary>Requests data from the /series endpoint</summary>
        /// <param name="symbols">List of strings of mutual fund symbols</param>
        /// <param name="metrics">List of strings corresponding to metric names</param>
        /// <param name="startDate">DateTime start date of the series data range</param>
        /// <param name="endDate">DateTime end date of the series data range</param>
        /// <returns>JObject representing the JSON response from the API</returns>
        public static async Task<JObject> GetSeries(List<string> symbols, List<string> metrics, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await RequestService.GetSeries(SecurityCollectionsPath, symbols, metrics, startDate, endDate);
        }
    }
}
