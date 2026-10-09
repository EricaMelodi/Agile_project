namespace ElectricitySavingApp.Services;

using ElectricitySavingApp.Model;

// Keeps carbon-intensity support optional when no API key is configured.
public class UnconfiguredCarbonIntensityService : ICarbonIntensityService
{
    public Task<CarbonIntensityEntry?> GetCarbonIntensity(string priceArea)
    {
        Console.WriteLine(
            "Grid carbon intensity unavailable. Set ELECTRICITY_MAPS_API_KEY to enable it.");

        return Task.FromResult<CarbonIntensityEntry?>(null);
    }
}