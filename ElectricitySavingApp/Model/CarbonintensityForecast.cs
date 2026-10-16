namespace ElectricitySavingApp.Model;

using System.Text.Json.Serialization;

public class CarbonIntensityForecast
{
    [JsonPropertyName("zone")]
    public string Zone { get; set; } = "";

    [JsonPropertyName("forecast")]
    public List<CarbonIntensityForecastEntry> Forecast {get; set;} = [];

    [JsonPropertyName("updatedAt")]
    public DateTimeOffset? updatedAt { get; set; }
}

public class CarbonIntensityForecastEntry
{
    [JsonPropertyName("datetime")]
    public DateTimeOffset DateTime { get; set; }

    [JsonPropertyName("carbonIntensity")]
    public decimal CarbonIntensityValue { get; set; }
}