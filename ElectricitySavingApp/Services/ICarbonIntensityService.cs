namespace ElectricitySavingApp.Services;

using ElectricitySavingApp.Model;

// Defines the operation required to retrieve grid carbon intensity.
public interface ICarbonIntensityService
{
	Task<CarbonIntensityEntry?> GetCarbonIntensity(string priceArea);
}
