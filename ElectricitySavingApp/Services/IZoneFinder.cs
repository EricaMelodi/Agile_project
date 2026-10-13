namespace ElectricitySavingApp.Services;

public interface IZoneFinder
{
    // Find the electricity bidding zone containing the supplied coordinate.
    Task<string> GetZone(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default
        );
}