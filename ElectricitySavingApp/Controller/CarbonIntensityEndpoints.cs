namespace ElectricitySavingApp.Controller;

using ElectricitySavingApp.Services;
using ElectricitySavingApp.Model;


public static class CarbonIntensityEndpoints
{

	// Register the endpoint for the latest carbon-intensity measurement.
    public static void MapCarbonIntensityEndpoints(this WebApplication app)
    {
        
        app.MapGet("/api/carbon-intensity", async (
            string priceArea,
            ICarbonIntensityService carbonIntensityService
            ) =>
        {
            // Normalize the area before validating and querying the provider.
            string formattedPriceArea = priceArea.ToUpperInvariant();

            if (formattedPriceArea is not ("SE1" or "SE2" or "SE3" or "SE4") )
            {
                return Results.BadRequest(
                    "Price area must be one of these: SE1, SE2, SE3, or SE4"
                );
            }

            // The result may be null when the optional carbon service is unconfigured.
            CarbonIntensityEntry? carbonIntensityResult = 
                await carbonIntensityService.GetCarbonIntensity(formattedPriceArea);

                return carbonIntensityResult is null ? Results.NotFound("Carbon intensity unvailable")
                    : Results.Ok(carbonIntensityResult);

        }
        );

    }

}