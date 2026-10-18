namespace ElectricitySavingApp.Model;

public sealed class HourlyPriceSummary
{
    public DateTimeOffset TimeStart { get; init; }

    public DateTimeOffset TimeEnd { get; init; }

    public decimal SekPerkWh { get; init; }
}
