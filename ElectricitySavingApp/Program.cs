
WebApplicationBuilder builder = WebApplication.CreateBuilder();

builder.Services.AddHttpClient<IElectricityPriceService, ElprisetJustNuService>(
			httpClient =>
			{
				httpClient.BaseAddress = 
					new Uri("https://www.elprisetjustnu.se/");
			}
		);

builder.Services.AddHttpClient("electricityMaps",
			httpClient => 
			{
				httpClient.BaseAddress = 
					new Uri("https://api.electricitymaps.com/");

			}
		);
 


builder.Services.AddScoped<ICarbonIntensityService>(
			services =>
			{
				string? apiKey =
					services.GetRequiredService<IConfiguration>()
						["ELECTRICITY_MAPS_API_KEY"];

				if (string.IsNullOrWhiteSpace(apiKey))
				{
					return new UnconfiguredCarbonIntensityService();
				}

				HttpClient httpClient =
					services.GetRequiredService<IHttpClientFactory>().CreateClient("electricityMaps");
				
				return new CarbonIntensityService(httpClient, apiKey);

			}
		);


WebApplication app = builder.Build();

app.MapPriceEndpoints();
app.MapCarbonIntensityEndpoints();

app.Run();