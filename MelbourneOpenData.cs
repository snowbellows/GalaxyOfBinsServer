public class MelbourneOpenDataService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MelbourneOpenDataService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetDatasetAsync(string dataset)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MelbourneOpenData");

            using HttpResponseMessage response = await client.GetAsync(
                "/api/explore/v2.1/catalog/datasets/" + dataset + "/records"
            );
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return "Unable to fetch bin data";
        }
    }
}
