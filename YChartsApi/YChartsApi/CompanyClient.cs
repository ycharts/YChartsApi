namespace YCharts.Api
{
    /// <summary>Api Client for all Company data endpoints</summary>
    public sealed class CompanyClient : BaseClient
    {
        /// <summary></summary>
        /// <param name="apiKey">YCharts provided Api Key</param>
        public CompanyClient(string apiKey) : base(apiKey)
        {
            this.SecurityCollectionPath = "companies";

        }
    }
}
