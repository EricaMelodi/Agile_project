using ElectricitySavingApp.Model;
using ElectricitySavingApp.Services;
namespace ElectricitySavingApp.Controller;

public static class GeoLocationEndpoints
{

	// Register endpoints that expose geocoding and zone lookup separately.
    public static void MapGeoLocationEndpoint(this WebApplication app)
    {
        // Return only the coordinates for an address.
        app.MapGet("/search", async (
            string address,
            IGeoCodeService geoService
        ) =>
        {
            
            GeoEntry? location = await geoService.GetGeoData(address);

            // Use 404 when the geocoder returns no matching location.
            return location is not null
                ? Results.Ok(location)
                : Results.NotFound("Address was not found");
            
        });

        // Resolve an address all the way to its electricity zone.
        app.MapGet("/search/zone", async (
            string address,
            IGeoCodeService geoService,
            IZoneFinder zoneFinder,
            CancellationToken cancellationToken) =>
        {
            GeoEntry? location = await geoService.GetGeoData(address);

            if (location is null)
                return Results.NotFound("Address was not found");

            // The zone finder performs a point-in-polygon lookup on the coordinates.
            string zone = await zoneFinder.GetZone(
                location.latitude,
                location.longitude,
                cancellationToken);

            return Results.Ok(new
            {
                address,
                latitude = location.latitude,
                longitude = location.longitude,
                zone
            });
        });
    }

}