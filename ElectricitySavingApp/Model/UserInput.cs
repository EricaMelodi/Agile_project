namespace ElectricitySavingApp.Model;

// Represents the selected date and electricity price area.
public class UserInput
{
    // Calendar date used to construct the price API URL.
    public int Year { get; set; }
    // Calendar month used to construct the price API URL.
    public int Month { get; set; }
    // Calendar day used to construct the price API URL.
    public int Day { get; set; }
    // Swedish electricity bidding zone, such as SE3.
    public string? PriceArea { get; set; }
}