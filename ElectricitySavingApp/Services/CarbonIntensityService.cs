namespace ElectricitySavingApp.Services;

using System.Text.Json;
using ElectricitySavingApp.Model;

// Retrieves current carbon intensity from Electricity Maps API.
public class CarbonIntensityService : ICarbonIntensityService
{

    // Sends requests to Electricity Maps using its configured base address.
    private readonly HttpClient httpClient;


    // Attach the Electricity Maps API key to every request from this service.
    public CarbonIntensityService(HttpClient httpClient, string? apiKey)
    {
        this.httpClient = httpClient;
        this.httpClient.DefaultRequestHeaders.Add("auth-token", apiKey);
    }


    // Retrieve the latest carbon-intensity measurement for one Swedish area.
    public async Task<CarbonIntensityEntry?> GetCarbonIntensity(string priceArea)
    {
        // Build the provider URL and safely encode the zone query value.
        string apiRequestUrl =
            $"v4/carbon-intensity/latest?zone=SE-{Uri.EscapeDataString(priceArea)}";
        try {

            // Deserialize the provider response into the application's carbon model.
            CarbonIntensityEntry? carbonIntensityResult = await httpClient.GetFromJsonAsync<CarbonIntensityEntry>(apiRequestUrl);

            return carbonIntensityResult;

        } 
        catch (HttpRequestException)
        {   //Carbon intensity is unavailable; continue without carbon data.
            return null;
        }
        catch (JsonException)
        {   //Provider returned invalid response; continue without carbon data.
            return null;
        }
        catch (TaskCanceledException)
        {   //Request timeout; continue without carbon data.
            return null;
        }
    }

    public async Task<CarbonIntensityForecast?> GetCarbonIntensityForecast(string priceArea)
    {
        string apiRequestUrl =
            $"v4/carbon-intensity/forecast" +
            $"?zone=SE-{Uri.EscapeDataString(priceArea)}" +
            "&horizonHours=24";
        
        return await httpClient.GetFromJsonAsync<CarbonIntensityForecast>(apiRequestUrl);
    }
    
}