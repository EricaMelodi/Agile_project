namespace ElectricitySavingApp.Services;

using ElectricitySavingApp.Model;

// Contract for services that fetch electricity prices.
public interface IElectricityPriceService
{
    Task<List<PriceEntry>> GetElectricityPrices(UserInput userInput);
}