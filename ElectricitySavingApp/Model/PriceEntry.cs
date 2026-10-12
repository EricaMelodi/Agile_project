namespace ElectricitySavingApp.Model;

using System.Text.Json.Serialization; // Needed to map JSON field names to C# properties.


// Represents one electricity price entry returned by the API.
public class PriceEntry
{
    // Maps the JSON field "time_start" to the TimeStart property.
    [JsonPropertyName("time_start")]
    public DateTimeOffset TimeStart { get; set; }

    // Maps the JSON field "SEK_per_kWh" to the SEK price property.
    [JsonPropertyName("SEK_per_kWh")]
    public decimal SekPerkWh { get; set; }

    // Maps the JSON field "price_area" to the PriceArea property.
    [JsonPropertyName("price_area")]
    public string? PriceArea { get; set; }
}

