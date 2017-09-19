
namespace YCharts.Api
{
    public class BaseClient
    {
        private string ApiKey { get; set; }

        protected BaseClient(string apiKey)
        {
            this.ApiKey = apiKey;
        }
    }
}
