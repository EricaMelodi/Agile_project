/* // Reads and validates console input before creating a UserInput object.
public class ConsoleInputReader : IConsoleInputReader
{
    // Reads the date and electricity price area from the console and
    // returns a UserInput object initialized with the values read.
    public UserInput Read()
    {
        Console.WriteLine("Enter the date and price area for electricity prices.\n\n" +
            "Year (YYYY), Month (MM), Day (DD),\n" +
            "PriceArea (SE1, SE2, SE3, or SE4)\n");

        int year = ReadInteger("Year: ");
        int month = ReadInteger("Month: ");
        int day = ReadInteger("Day: ");

        Console.Write("PriceArea: ");
        string priceArea = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

        // Check that the entered numbers create a valid date. 
        if (!DateTime.TryParseExact(
            $"{year:D4}-{month:D2}-{day:D2}",
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out _))
        {
            throw new ArgumentException("The date is not valid.");
        }

        // Only allow valid Swedish electricity price areas.
        if (priceArea is not ("SE1" or "SE2" or "SE3" or "SE4"))
        {
            throw new ArgumentException("The price area must be SE1, SE2, SE3, or SE4.");
        }

        Console.WriteLine();


        return new UserInput
        {
            Year = year,
            Month = month,
            Day = day,
            PriceArea = priceArea
        };
    }



    // Helper function. Reads a valid integer value from the console.
    private static int ReadInteger(string prompt)
    
    {
        Console.Write(prompt);

        if (int.TryParse(Console.ReadLine(), out int value))
        {
            return value;
        }

        throw new ArgumentException("The value must be a number.");
    }
} */