# YChartsApi
YCharts API Client for .Net Runtime 4.5+.

## Dependencies
- Visual Studio: this will install .Net Runtime if you don't have it already(particularly relevant for Mac OS)
- Newtonsoft.Json(10.0.3, autoinstalled via NuGet

## Examples

### Config/Common Code
Lines 1 & 2 are used everytime you use anything from the libray. Line 3 is used just once in a client's app.
```
using YCharts.Api;
using Newtonsoft.Json.Linq;
YChartsConfig.ApiKey = "<API key>";
```

### Company Client

```
JObject dividends = await YChartsCompanyClient.GetDividends(new List<string>() { "AAPL" }, new DateTime(2013, 01, 21));
JObject splits = await YChartsCompanyClient.GetStockSplits(new List<string>() { "AAPL" },  new DateTime(2013, 01, 21), new DateTime(2013, 02, 27));
JObject spinoffs = await YChartsCompanyClient.GetStockSpinoffs(new List<string>() { "GOOG" },  new DateTime(2013, 01, 21), new DateTime(2013, 02, 27));
JObject points = await YChartsCompanyClient.GetPoints(new List<string>() { "AAPL" }, new List<string>() { "price" }, new DateTime(2013, 01, 21));
JObject series = await YChartsCompanyClient.GetSeries(new List<string>() { "AAPL" }, new List<string>() { "price" }, new DateTime(2013, 01, 21), new DateTime(2013, 02, 27));
JObject info = await YChartsCompanyClient.GetInfo(new List<string>() { "AAPL" }, new List<string>() { "name" });
```

### Mutual Fund Client

```
JObject points = await YChartsMutualFundClient.GetPoints(new List<string>() { "M:VFIAX" }, new List<string>() { "price" }, new DateTime(2013, 01, 21));
JObject series = await YChartsMutualFundClient.GetSeries(new List<string>() { "M:VFIAX" }, new List<string>() { "price" }, new DateTime(2013, 01, 21), new DateTime(2013, 02, 27));
JObject info = await YChartsMutualFundClient.GetInfo(new List<string>() { "M:VFIAX" }, new List<string>() { "name" });
```
