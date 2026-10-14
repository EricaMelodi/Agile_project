namespace ElectricitySavingApp.Services;

using ElectricitySavingApp.Model;

// Defines the operation required by the application to retrieve price intervals.
public interface IElectricityPriceService
{
    // Fetch all prices for the requested date and price area.
    Task<List<PriceEntry>> GetElectricityPrices(UserInput userInput);
}