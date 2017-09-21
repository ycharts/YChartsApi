namespace YCharts.Api
{
    /// <summary>API client for all company data endpoints</summary>
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
