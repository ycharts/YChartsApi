using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}
