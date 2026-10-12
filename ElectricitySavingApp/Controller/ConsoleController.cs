/* 
// Controls the overall console workflow:
// read input, fetch prices, and print the result.
public class ConsoleController
{
    private readonly IElectricityPriceService priceService;
    private readonly ICarbonIntensityService carbonIntensityService;
    private readonly IPrinter printer;
    private readonly IConsoleInputReader inputReader;

    public ConsoleController(
        IElectricityPriceService priceService,
        ICarbonIntensityService carbonIntensityService,
        IPrinter pricePrinter,
        IConsoleInputReader inputReader)
    {
        this.priceService = priceService;
        this.carbonIntensityService = carbonIntensityService;
        this.printer = pricePrinter;
        this.inputReader = inputReader;
    }

    // Runs the main application flow and handles errors.
    public async Task RunAsync()
    {
        try
        {
            UserInput userInput = inputReader.Read();

            List<PriceEntry> prices =
                await priceService.GetElectricityPrices(userInput);

            printer.PrintPrices(prices);

            try
            {
                CarbonIntensity? carbonIntensity =
                    await carbonIntensityService.GetCarbonIntensity(userInput.PriceArea!);

                if (carbonIntensity is not null)
                {
                    printer.PrintCarbonIntensity(carbonIntensity);
                }
            }
            catch (HttpRequestException)
            {
                printer.PrintError("Could not retrieve grid carbon intensity.");
            }
        }
        catch (ArgumentException exception)
        {
            printer.PrintError(exception.Message);
        }
        catch (HttpRequestException)
        {
            printer.PrintError("Could not retrieve electricity prices.");
        }
    }
} */