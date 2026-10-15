
using ElectricitySavingApp.Services;
using ElectricitySavingApp.Controller;

// Create the ASP.NET Core application builder. The builder collects
// configuration and service registrations before the web server starts.
WebApplicationBuilder builder = WebApplication.CreateBuilder();

// Register the service that downloads daily Swedish electricity prices.
// The base address lets the service use relative API paths later.
builder.Services.AddHttpClient<IElectricityPriceService, ElprisetJustNuService>(
			httpClient =>
			{
				httpClient.BaseAddress = 
					new Uri("https://www.elprisetjustnu.se/");
			}
		);

// Register a named client for Electricity Maps. The API key is added later
// when CarbonIntensityService is created because it comes from configuration.
builder.Services.AddHttpClient("electricityMaps",
			httpClient => 
			{
				httpClient.BaseAddress = 
					new Uri("https://api.electricitymaps.com/");

			}
		);
 

// Register a named client for the address geocoding provider.
builder.Services.AddHttpClient("geoLocation",
			httpClient =>
			{
				httpClient.BaseAddress =
					new Uri("https://geocode.maps.co/");

			}
		);

// Register the client used to download electricity-zone boundary polygons.
builder.Services.AddHttpClient<ZoneFinder>(
			httpClient =>
			{
				httpClient.BaseAddress =
					new Uri("https://electricity-spot.com/");
			}
		);

// Keep one ZoneFinder instance alive so its downloaded GeoJSON boundary data
// can be reused instead of downloaded for every request.
builder.Services.AddSingleton<IZoneFinder>(
		services => services.GetRequiredService<ZoneFinder>());

// Allow the separately running frontend development server to call this API.
builder.Services.AddCors(options =>
	{
		options.AddPolicy("frontend", policy =>
			policy.WithOrigins("http://localhost:3000")
				.AllowAnyHeader()
				.AllowAnyMethod());
	});

		// Carbon intensity is optional. Without an API key, use a fallback service
		// that returns null instead of preventing the application from starting.
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



// Create the geocoding service with the API key stored in configuration.
builder.Services.AddScoped<IGeoCodeService>(
	services =>
	{
		string? apiKey =
			services.GetRequiredService<IConfiguration>()
				["GEO_CODING_API_KEY"];

		if (string.IsNullOrWhiteSpace(apiKey))
		{
			throw new InvalidOperationException("GeoCoding API key is not configured.");
		}

		HttpClient httpClient =
			services.GetRequiredService<IHttpClientFactory>().CreateClient("geoLocation");

		return new GeoCodeService(httpClient, apiKey);

	}
);


// Build the configured application and middleware pipeline.
WebApplication app = builder.Build();

// Apply the frontend CORS policy before handling requests.
app.UseCors("frontend");

// Register the grouped endpoint definitions with the web application.
app.MapPriceEndpoints();
app.MapCarbonIntensityEndpoints();
app.MapGeoLocationEndpoint();
app.MapEnergyEndpoints();

app.Run();