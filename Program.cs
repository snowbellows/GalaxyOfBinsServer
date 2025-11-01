using GalaxyOfBinsServer.MelbourneOpenData;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            name: MyAllowSpecificOrigins,
            policy =>
            {
                policy
                    .SetIsOriginAllowed(origin => new Uri(origin).IsLoopback)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }
        );
    });
}
else
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            name: MyAllowSpecificOrigins,
            policy =>
            {
                policy
                    .WithOrigins("https://galaxy-of-bins.netlify.app/")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }
        );
    });
}
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/", () => "Hello World!");

app.MapGet(
    "/melbourne-open-data/bin-data",
    async (
        string? where,
        string? order_by,
        string? group_by,
        int? offset,
        int? limit,
        MelbourneOpenDataService melbourneOpenDataService
    ) =>
    {
        var data = await melbourneOpenDataService.GetBinDataAsync(
            where,
            order_by,
            group_by,
            offset,
            limit
        );
        return Results.Ok(data);
    }
);

app.Run();
