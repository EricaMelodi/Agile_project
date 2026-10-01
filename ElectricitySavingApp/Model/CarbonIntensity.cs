using System.Text.Json.Serialization; // Used to map JSON properties to C# properties.

// Represents the carbon intensity data returned by the Electricity Maps API.
public class CarbonIntensity
{   
    // Maps the JSON property "zone" to the C# property "Zone".
    [JsonPropertyName("zone")]
    public string Zone { get; set; } = "";

    // Maps the JSON property "datetime" to the C# property "DateTime".
    [JsonPropertyName("datetime")]
    public DateTime DateTime { get; set; }

    // Maps the JSON property "carbonIntensity" to the C# property "CarbonIntensityValue".
    [JsonPropertyName("carbonIntensity")]
    public decimal CarbonIntensityValue { get; set; }

    // Maps the JSON property "carbonIntensityUnit" to the C# property "CarbonIntensityUnit".
    [JsonPropertyName("carbonIntensityUnit")]
    public string CarbonIntensityUnit { get; set; } = "gCO2eq/kWh";

    // Maps the JSON property "isEstimated" to the C# property "IsEstimated".
    [JsonPropertyName("isEstimated")]
    public bool IsEstimated { get; set; }
}