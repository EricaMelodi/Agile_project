using System.Net.Http.Json; // Used to deserialize JSON from the API response.

// Fetches price data from the ElprisetJustNu API and maps it to PriceEntry objects.
public class ElprisetJustNuService : IElectricityPriceService
{
    // Http Client used to send Http requests and receive responses from the API.
    private readonly HttpClient httpClient;

    // Constructor that initializes the HttpClient.
    public ElprisetJustNuService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    // Builds the URL, sends the request, and converts the JSON response into an object.
    public async Task<List<PriceEntry>> GetElectricityPrices(UserInput userInput)
    {
        string apiRequestUrl = "api/v1/prices/" +
                        $"{userInput.Year}/{userInput.Month:D2}-{userInput.Day:D2}_{userInput.PriceArea}.json";

        // Sends the request to the API and deserializes the JSON response into a list of PriceEntry objects.
        List<PriceEntry>? prices = await httpClient.GetFromJsonAsync<List<PriceEntry>>(apiRequestUrl);

        // Returns the list of prices or an empty list if the response was null.
        return prices ?? new List<PriceEntry>();
    }
}

