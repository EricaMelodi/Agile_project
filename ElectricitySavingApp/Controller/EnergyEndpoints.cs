namespace ElectricitySavingApp.Controller;

using System.Net.Sockets;
using ElectricitySavingApp.Model;
using ElectricitySavingApp.Services;

public static class EnergyEndpoints
{
	// Register the endpoint that combines address lookup, zone detection,
	// current electricity pricing, and carbon-intensity lookup.
	public static async Task MapEnergyEndpoints(this WebApplication app)
	{
		// The address is supplied as one query-string value, for example:
		// /api/current-energy?address=Street%201,%20Gothenburg
		app.MapGet("/api/current-energy", async (
			string address,
			IGeoCodeService geoService,
			IZoneFinder zoneFinder,
			IElectricityPriceService priceService,
			ICarbonIntensityService carbonService,
			CancellationToken cancellationToken) =>
		{
			// Reject an empty address before calling an external service.
			if (string.IsNullOrWhiteSpace(address))
			{
				return Results.BadRequest(
					"An address containing street name, street number, and city is required.");
			}

			// Remove accidental spaces before sending the address to the geocoder.
			address = address.Trim();
			GeoEntry? location = await geoService.GetGeoData(address);

			// A null result means that the geocoder could not find the address.
			if (location is null)
				return Results.NotFound("Address was not found.");

			// Convert the coordinates into a Swedish electricity bidding zone.
			string zone = await zoneFinder.GetZone(
				location.latitude,
				location.longitude,
				cancellationToken);

			// The rest of this application currently supports Swedish zones only.
			if (zone is not ("SE1" or "SE2" or "SE3" or "SE4"))
			{
				return Results.BadRequest(
					$"The address resolved to unsupported electricity zone '{zone}'.");
			}

			// The price API uses the local Swedish calendar date, not UTC date.
			TimeZoneInfo localTimeZone = GetSwedishTimeZone();
			DateTimeOffset localTimeNow = TimeZoneInfo.ConvertTime(
				DateTimeOffset.UtcNow,
				localTimeZone);

			// Build the exact date and price-area input expected by the price service.
			UserInput userInput = new()
			{
				Year = localTimeNow.Year,
				Month = localTimeNow.Month,
				Day = localTimeNow.Day,
				PriceArea = zone
			};

			// Download all intervals for today in the resolved price area.
			List<PriceEntry> prices = await priceService
				.GetElectricityPrices(userInput);

			// Select the interval that contains the current local time.
			PriceEntry? currentPrice = FindCurrentPrice(prices, localTimeNow);

			if (currentPrice is null)
				return Results.NotFound("No current electricity price was found.");

			// Electricity Maps provides the latest carbon-intensity measurement
			// for the same zone. It may be null when no API key is configured.
			CarbonIntensityEntry? carbonIntensity =
				await carbonService.GetCarbonIntensity(zone);

			return Results.Ok(new
			{
				address,
				zone,
				latitude = location.latitude,
				longitude = location.longitude,
				localTime = localTimeNow,
				price = currentPrice,
				carbonIntensity
			});
		});

		app.MapGet("/api/price-forecast", async (string address, IGeoCodeService geoService, IZoneFinder zonefinder, IElectricityPriceService priceService, CancellationToken cancellationToken) =>
		{
			if (string.IsNullOrWhiteSpace(address))
			{
				return Results.BadRequest("An address is required.");
			}
		

			address = address.Trim();

			GeoEntry? location = await geoService.GetGeoData(address);

			if (location is null)
			{
				return Results.NotFound("Address was not found.");
			}

			string zone = await zonefinder.GetZone(location.latitude, location.longitude, cancellationToken);

			if(zone is not ("SE1" or "SE2" or "SE3" or "SE4"))
			{
				return Results.BadRequest($"Unsupported electricity zone '{zone}'.");
			}

			TimeZoneInfo timeZone = GetSwedishTimeZone();

			DateTimeOffset now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone);

			UserInput todayInput = new()
			{
				Year = now.Year,
				Month = now.Month,
				Day = now.Day,
				PriceArea = zone
			};

			List<PriceEntry> prices = await priceService.GetElectricityPrices(todayInput);

			DateTime tomorrow = now.Date.AddDays(1);

			UserInput tomorrowInput = new()
			{
				Year = tomorrow.Year,
				Month = tomorrow.Month,
				Day = tomorrow.Day,
				PriceArea = zone
			};

			try
			{
				List<PriceEntry> tomorrowPrices = await priceService.GetElectricityPrices(tomorrowInput);
				prices.AddRange(tomorrowPrices);
			}
			catch (HttpRequestException)
			{
				//Keep today's prices if tomorrow's prices are unavailable
			}

			DateTimeOffset end = now.AddHours(24);

			List<PriceEntry> forecast = prices.Where(p => p.TimeStart >= now && p.TimeStart < end).OrderBy(p => p.TimeStart).ToList();

			return Results.Ok(new
			{
				address, zone, generatedAt = now, forecast
			});

		});


	}

	private static PriceEntry? FindCurrentPrice(
		IReadOnlyList<PriceEntry> prices,
		DateTimeOffset localNow)
	{
		// Sort defensively because external API responses should not be assumed
		// to arrive in chronological order.
		List<PriceEntry> orderedPrices = prices
			.OrderBy(price => price.TimeStart)
			.ToList();

		for (int index = 0; index < orderedPrices.Count; index++)
		{
			// The next price start marks the end of this interval. The final
			// entry is assumed to cover the normal 15-minute period.
			DateTimeOffset startsAt = orderedPrices[index].TimeStart;
			DateTimeOffset endsAt = index + 1 < orderedPrices.Count
				? orderedPrices[index + 1].TimeStart
				: startsAt.AddMinutes(15);

			if (startsAt <= localNow && localNow < endsAt)
				return orderedPrices[index];
		}

		return null;
	}

	private static TimeZoneInfo GetSwedishTimeZone()
	{
		// Linux/macOS use the IANA identifier. Windows uses a different name,
		// so the fallback keeps the application portable across operating systems.
		try
		{
			return TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");
		}
		catch (TimeZoneNotFoundException)
		{
			return TimeZoneInfo.FindSystemTimeZoneById(
				"W. Europe Standard Time");
		}
	}
}