using System.Text.Json;

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

                var url =
                    "/api/explore/v2.1/catalog/datasets/netvox-r718x-bin-sensor/records"
                    + (where != null ? "?where=" + where : "")
                    + (orderBy != null ? "?orderBy=" + orderBy : "")
                    + (groupBy != null ? "?groupBy=" + groupBy : "")
                    + (offset != null ? "?offset=" + offset : "")
                    + (limit != null ? "?limit=" + limit : "");

                _logger.LogInformation(url);

                var json = await client.GetStringAsync(url);
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
