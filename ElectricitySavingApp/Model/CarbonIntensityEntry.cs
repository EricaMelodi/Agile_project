namespace ElectricitySavingApp.Model;

using System.Text.Json.Serialization; // Used to map JSON properties to C# properties.

// Represents the carbon intensity data returned by the Electricity Maps API.
public class CarbonIntensityEntry
{   
    // Electricity Maps zone identifier, for example SE-SE3.
    // Maps the JSON property "zone" to the C# property "Zone".
    [JsonPropertyName("zone")]
    public string Zone { get; set; } = "";

    // Maps the JSON property "datetime" to the C# property "DateTime".
    [JsonPropertyName("datetime")]
    public DateTime DateTime { get; set; }

    // Carbon intensity expressed in the unit provided by CarbonIntensityUnit.
    // Maps the JSON property "carbonIntensity" to the C# property "CarbonIntensityValue".
    [JsonPropertyName("carbonIntensity")]
    public decimal CarbonIntensityValue { get; set; }

    // Maps the JSON property "carbonIntensityUnit" to the C# property "CarbonIntensityUnit".
    [JsonPropertyName("carbonIntensityUnit")]
    public string CarbonIntensityUnit { get; set; } = "gCO2eq/kWh";

    // Indicates whether the provider marked this value as estimated.
    // Maps the JSON property "isEstimated" to the C# property "IsEstimated".
    [JsonPropertyName("isEstimated")]
    public bool IsEstimated { get; set; }
}