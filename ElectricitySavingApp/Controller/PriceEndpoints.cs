namespace ElectricitySavingApp.Controller;

using ElectricitySavingApp.Services;
using ElectricitySavingApp.Model;

// Defines the URL endpoints for retrieving electricity prices.
public static class PriceEndpoints
{

    // Register the endpoint for requesting a complete day's prices for one area.
    public static void MapPriceEndpoints(this WebApplication app)
    {
        app.MapGet("/api/prices", async (
            int year,
            int month,
            int day,
            string priceArea,
            IElectricityPriceService priceService
            ) =>
        {
            // Normalize input so "se3" and "SE3" behave identically.
            string formatedPriceArea = priceArea.ToUpperInvariant();

            // Prevent unsupported areas from being sent to the external API.
            if (formatedPriceArea is not ("SE1" or "SE2" or "SE3" or "SE4"))
            {
                return Results.BadRequest(
                    "Price area must be one of these: SE1, SE2, SE3, or SE4"
                );
            }

            // Package the query parameters into the service's input model.
            UserInput userInput = new UserInput
            {
                Year = year,
                Month = month,
                Day = day,
                PriceArea = formatedPriceArea   
            };


            // The service retrieves all price intervals for this date and area.
            List<PriceEntry> prices = await priceService.GetElectricityPrices(userInput);

            return Results.Ok(prices);

        }

        );
        
    }

    
    
}