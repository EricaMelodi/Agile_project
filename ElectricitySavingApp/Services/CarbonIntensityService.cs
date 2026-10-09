namespace ElectricitySavingApp.Services;

using ElectricitySavingApp.Model;

// Retrieves current carbon intensity from Electricity Maps API.
public class CarbonIntensityService : ICarbonIntensityService
{

    // HttpClient is used to send HTTP requests and receive responses from the API.
    private readonly HttpClient httpClient;


    // Constructor that initializes the HttpClient and sets the API key in the request headers.
    public CarbonIntensityService(HttpClient httpClient, string? apiKey)
    {
        this.httpClient = httpClient;
        this.httpClient.DefaultRequestHeaders.Add("auth-token", apiKey);
    }


    // Retrieves the current carbon intensity for the specified price area.
    public async Task<CarbonIntensityEntry?> GetCarbonIntensity(string priceArea)
    {
        // Builds the URL for the API request and escapes the price area to ensure it's safe for use in a URL.
        string apiRequestUrl =
            $"v4/carbon-intensity/latest?zone=SE-{Uri.EscapeDataString(priceArea)}";

        // Sends the request to the API and deserializes the JSON response into a carbonIntensityResult object.
        CarbonIntensityEntry? carbonIntensityResult = await httpClient.GetFromJsonAsync<CarbonIntensityEntry>(apiRequestUrl);

        return carbonIntensityResult;
    }
    
}