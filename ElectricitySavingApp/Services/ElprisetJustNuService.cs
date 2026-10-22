namespace ElectricitySavingApp.Services;

using System.Text.Json;
using ElectricitySavingApp.Model;

// Fetches price data from the ElprisetJustNu API and maps it to PriceEntry objects.
public class ElprisetJustNuService : IElectricityPriceService
{
    
    // Sends requests to ElprisetJustNu using the base address configured in Program.cs.
    private readonly HttpClient httpClient;

    // Receive the configured client through dependency injection.
    public ElprisetJustNuService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    // Build the provider URL from the requested date and Swedish price area.
    public async Task<List<PriceEntry>> GetElectricityPrices(UserInput userInput)
    {
        string apiRequestUrl = "api/v1/prices/" +
                        $"{userInput.Year:D4}/{userInput.Month:D2}-{userInput.Day:D2}_{userInput.PriceArea}.json";

            // Deserialize the JSON array into the application's price model.
            List<PriceEntry>? prices = await httpClient.GetFromJsonAsync<List<PriceEntry>>(apiRequestUrl);

            // Return an empty list rather than null when the provider has no payload.
            return prices ?? new List<PriceEntry>();
    }

}

