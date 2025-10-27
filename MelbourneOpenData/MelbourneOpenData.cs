using System.Text.Json;

namespace GalaxyOfBinsServer.MelbourneOpenData
{
    public class MelbourneOpenDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        public MelbourneOpenDataService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<NetvoxR718xBinSensorResponse?> GetBinDataAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("MelbourneOpenData");

                var json = await client.GetStringAsync(
                    "/api/explore/v2.1/catalog/datasets/netvox-r718x-bin-sensor/records"
                );
                var response = JsonSerializer.Deserialize<NetvoxR718xBinSensorResponse>(
                    json,
                    jsonSerializerOptions
                );
                return response;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
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
