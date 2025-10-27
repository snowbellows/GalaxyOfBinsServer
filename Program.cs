using GalaxyOfBinsServer.MelbourneOpenData;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var MODAPIKey = builder.Configuration["MelbourneOpenData:APIKey"];

builder.Services.AddHttpClient(
    "MelbourneOpenData",
    client =>
    {
        client.BaseAddress = new Uri("https://data.melbourne.vic.gov.au");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("Authorization", "Apikey " + MODAPIKey);
    }
);

builder.Services.AddScoped<MelbourneOpenDataService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.MapGet("/", () => "Hello World!");

app.MapGet(
    "/melbourne-open-data/bin-data",
    async (
        string? where,
        string? orderBy,
        string? groupBy,
        int? offset,
        int? limit,
        MelbourneOpenDataService melbourneOpenDataService
    ) =>
    {
        var data = await melbourneOpenDataService.GetBinDataAsync(
            where,
            orderBy,
            groupBy,
            offset,
            limit
        );
        return Results.Ok(data);
    }
);

app.Run();
