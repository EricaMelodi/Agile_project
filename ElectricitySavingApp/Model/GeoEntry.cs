namespace ElectricitySavingApp.Model;

using System.Text.Json.Serialization; // Needed to map JSON field names to C# properties.

public class GeoEntry
{
    // Latitude returned by the geocoding provider.
    [JsonPropertyName("lat")]
    public double latitude { get; set; }

    // Longitude returned by the geocoding provider.
    [JsonPropertyName ("lon")]
    public double longitude { get; set; }

}