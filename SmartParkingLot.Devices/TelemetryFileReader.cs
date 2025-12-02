namespace SmartParkingLot.Devices;

/// <summary>
/// Utility class to read telemetry from looping ASCII files
/// </summary>
public static class TelemetryFileReader
{
    /// <summary>
    /// Read next value from telemetry file (loops to beginning when reaching end)
    /// </summary>
    public static async Task<(double value, int newPosition)> ReadNextValueAsync(string filePath, int currentPosition)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Telemetry file not found: {filePath}");
        }

        var lines = await File.ReadAllLinesAsync(filePath);
        var validLines = lines
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.TrimStart().StartsWith("#"))
            .ToList();

        if (validLines.Count == 0)
        {
            throw new InvalidOperationException($"No valid data in telemetry file: {filePath}");
        }

        // Loop back to start if we've reached the end
        if (currentPosition >= validLines.Count)
        {
            currentPosition = 0;
        }

        var line = validLines[currentPosition];
        if (double.TryParse(line.Trim(), out double value))
        {
            return (value, currentPosition + 1);
        }

        throw new FormatException($"Invalid numeric value in telemetry file: {line}");
    }

    /// <summary>
    /// Read random value from telemetry file (for simulation)
    /// </summary>
    public static async Task<double> ReadRandomValueAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Telemetry file not found: {filePath}");
        }

        var lines = await File.ReadAllLinesAsync(filePath);
        var validLines = lines
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.TrimStart().StartsWith("#"))
            .ToList();

        if (validLines.Count == 0)
        {
            throw new InvalidOperationException($"No valid data in telemetry file: {filePath}");
        }

        var random = new Random();
        var randomLine = validLines[random.Next(validLines.Count)];
        
        if (double.TryParse(randomLine.Trim(), out double value))
        {
            return value;
        }

        throw new FormatException($"Invalid numeric value in telemetry file: {randomLine}");
    }
}
