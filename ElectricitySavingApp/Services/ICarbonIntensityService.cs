namespace ElectricitySavingApp.Services;

using ElectricitySavingApp.Model;

// Defines the operation required to retrieve the latest grid carbon intensity.
public interface ICarbonIntensityService
{
	// Return the latest measurement for a Swedish electricity area.
	Task<CarbonIntensityEntry?> GetCarbonIntensity(string priceArea);

	Task<CarbonIntensityForecast?> GetCarbonIntensityForecast(string priceArea);
}
