// Defines the operation required to retrieve grid carbon intensity.
public interface ICarbonIntensityService
{
	Task<CarbonIntensity?> GetCarbonIntensity(string priceArea);
}
