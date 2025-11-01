using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;

namespace GalaxyOfBinsServer.MelbourneOpenData
{
    public class MelbourneOpenDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<MelbourneOpenDataService> _logger;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        public MelbourneOpenDataService(
            IHttpClientFactory httpClientFactory,
            ILogger<MelbourneOpenDataService> logger
        )
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<NetvoxR718xBinSensorResponse?> GetBinDataAsync(
            string? where,
            string? orderBy,
            string? groupBy,
            int? offset,
            int? limit
        )
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MelbourneOpenData");

                var uri = new UriBuilder(
                    "https://data.melbourne.vic.gov.au/api/explore/v2.1/catalog/datasets/netvox-r718x-bin-sensor/records"
                );

                if (where != null)
                {
                    if (uri.Query != null && uri.Query.Length > 1)
                        uri.Query = uri.Query + "&where=" + HttpUtility.UrlEncode(where);
                    else
                        uri.Query = "?where=" + HttpUtility.UrlEncode(where);
                }

                if (orderBy != null)
                {
                    if (uri.Query != null && uri.Query.Length > 1)
                        uri.Query = uri.Query + "&order_by=" + HttpUtility.UrlEncode(orderBy);
                    else
                        uri.Query = "?order_by=" + HttpUtility.UrlEncode(orderBy);
                }

                if (groupBy != null)
                {
                    if (uri.Query != null && uri.Query.Length > 1)
                        uri.Query = uri.Query + "&group_by=" + HttpUtility.UrlEncode(groupBy);
                    else
                        uri.Query = "?group_by=" + HttpUtility.UrlEncode(groupBy);
                }

                if (offset != null)
                {
                    if (uri.Query != null && uri.Query.Length > 1)
                        uri.Query = uri.Query + "&offset=" + offset;
                    else
                        uri.Query = "?offset=" + offset;
                }

                if (limit != null)
                {
                    if (uri.Query != null && uri.Query.Length > 1)
                        uri.Query = uri.Query + "&limit=" + limit;
                    else
                        uri.Query = "?limit=" + limit;
                }

                _logger.LogInformation(uri.ToString());

                var json = await client.GetStringAsync(uri.ToString());
                var response = JsonSerializer.Deserialize<NetvoxR718xBinSensorResponse>(
                    json,
                    jsonSerializerOptions
                );
                return response;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "An error occurred while fetching bin data");
                return null;
            }
        }
    }

    public class NetvoxR718xBinSensorResponse
    {
        public float? TotalCount { get; set; }
        public NetvoxR718xBinSensorData[]? Results { get; set; }

        public class NetvoxR718xBinSensorData
        {
            public string? DevId { get; set; }
            public string? Time { get; set; }
            public float? Temperature { get; set; }
            public float? Distance { get; set; }
            public float? Battery { get; set; }
            public LatLongInfo? LatLong { get; set; }
            public string? SensorName { get; set; }
            public float? FillLevel { get; set; }

            public class LatLongInfo
            {
                public float Lat { get; set; }
                public float Lon { get; set; }
            }
        }
    }
}
