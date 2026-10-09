/* 
// Prints electricity prices, carbon instensity, and error messages to the console.
public class Printer : IPrinter
{
    // Prints the first five price entries.
    public void PrintPrices(List<PriceEntry> prices)
    {
        foreach (PriceEntry price in prices.Take(5))
        {
            Console.WriteLine(
                $"{price.TimeStart:yyyy-MM-dd HH:mm}: " +
                $"{price.SekPerkWh:F3} SEK/kWh");
        }
    }

    // Prints the carbon intensity value and a status message.

    public void PrintCarbonIntensity(CarbonIntensity carbonIntensity)
    {
        string status = carbonIntensity.CarbonIntensityValue switch
        {
            < 50 => "Very clean grid",
            < 150 => "Clean grid",
            < 300 => "Moderate carbon intensity",
            _ => "High carbon intensity"
        };

        string estimateLabel = carbonIntensity.IsEstimated ? " (estimated)" : "";

        Console.WriteLine(
            $"Grid carbon intensity: {carbonIntensity.CarbonIntensityValue:F0} " +
            $"{carbonIntensity.CarbonIntensityUnit}{estimateLabel} - {status}");
    }

    // Prints error message.
    public void PrintError(string message)
    {
        Console.WriteLine(message);
    }
} */