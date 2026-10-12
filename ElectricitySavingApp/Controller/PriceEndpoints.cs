namespace ElectricitySavingApp.Controller;

using ElectricitySavingApp.Services;
using ElectricitySavingApp.Model;

// Defines the URL endpoints for retrieving electricity prices.
public static class PriceEndpoints
{

    // Maps the GET endpoint for fetching electricity prices based on user input.
    public static void MapPriceEndpoints(this WebApplication app)
    {
        // Maps the GET endpoint for fetching electricity prices based on user input.
        app.MapGet("/api/prices", async (
            int year,
            int month,
            int day,
            string priceArea,
            IElectricityPriceService priceService
            ) =>
        {
            string formatedPriceArea = priceArea.ToUpperInvariant();

            if (formatedPriceArea is not ("SE1" or "SE2" or "SE3" or "SE4"))
            {
                return Results.BadRequest(
                    "Price area must be one of these: SE1, SE2, SE3, or SE4"
                );
            }

            UserInput userInput = new UserInput
            {
                Year = year,
                Month = month,
                Day = day,
                PriceArea = formatedPriceArea   
            };


            List<PriceEntry> prices = await priceService.GetElectricityPrices(userInput);

            return Results.Ok(prices);

        }

        );
        
    }

    
    
}