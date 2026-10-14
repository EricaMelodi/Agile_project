namespace ElectricitySavingApp.Services;

using ElectricitySavingApp.Model;

public interface IGeoCodeService
{
    // Convert a human-readable address into coordinates, or null if no match exists.
    Task<GeoEntry?> GetGeoData(string? address);
}