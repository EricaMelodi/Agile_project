namespace ElectricitySavingApp.Services;

using System.Net.Http.Json;
using ElectricitySavingApp.Model; 

public class GeoCodeService : IGeoCodeService
{

    // Sends HTTP requests to the configured geocoding provider.
    private readonly HttpClient httpClient;

    // Require an API key and attach it to requests made by this service.
    public GeoCodeService(HttpClient httpClient, string? apiKey)
    {

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key is required.", nameof(apiKey));
        
        this.httpClient = httpClient;
        this.httpClient.DefaultRequestHeaders.Add("auth-token", apiKey);

    }

    public async Task<GeoEntry?> GetGeoData(string? address)
    {

        // Fail early instead of making an invalid external request.
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty.", nameof(address));
            

        // Encode spaces, commas, and other characters for use in a URL.
        string encodedAddress = Uri.EscapeDataString(address);
        string? apiKey = httpClient.DefaultRequestHeaders.GetValues("auth-token").FirstOrDefault();
        
        // Ask the provider for matching locations and request its JSON format.
        string apiRequestUrl = 
            $"search?q={encodedAddress}&api_key={apiKey}";

            // The first result is used as the best match for the user's address.
            List<GeoEntry>? locationResult = 
                await httpClient.GetFromJsonAsync<List<GeoEntry>>(apiRequestUrl);


            return locationResult?.FirstOrDefault();
            
    }

}