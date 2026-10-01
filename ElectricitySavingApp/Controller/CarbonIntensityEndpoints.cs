
public static class CarbonIntensityEndpoints
{

    public static void MapCarbonIntensityEndpoints(this WebApplication app)
    {
        
        app.MapGet("/api/carbon-intensity", async (
            string priceArea,
            ICarbonIntensityService carbonIntensityService
            ) =>
        {
            string formattedPriceArea = priceArea.ToUpperInvariant();

            if (formattedPriceArea is not ("SE1" or "SE2" or "SE3" or "SE4") )
            {
                return Results.BadRequest(
                    "Price area must be one of these: SE1, SE2, SE3, or SE4"
                );
            }

            CarbonIntensity? carbonIntensityResult = 
                await carbonIntensityService.GetCarbonIntensity(formattedPriceArea);

                return carbonIntensityResult is null ? Results.NotFound("Carbon intensity unvailable")
                    : Results.Ok(carbonIntensityResult);

        }
        );

    }

}