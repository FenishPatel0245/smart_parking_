namespace SmartParkingLot.Application.DesignPatterns;

/// <summary>
/// Singleton pattern for configuration management
/// </summary>
public sealed class ConfigurationManager
{
    private static readonly Lazy<ConfigurationManager> _instance = 
        new Lazy<ConfigurationManager>(() => new ConfigurationManager());

    public static ConfigurationManager Instance => _instance.Value;

    private ConfigurationManager()
    {
        // Initialize default configuration
        TelemetryPollingIntervalSeconds = 3;
        MaxTelemetryHistoryCount = 1000;
        AlertRetentionDays = 30;
        EnableRealTimeUpdates = true;
        DatabasePath = "scada_monitoring.db";
    }

    public int TelemetryPollingIntervalSeconds { get; set; }
    public int MaxTelemetryHistoryCount { get; set; }
    public int AlertRetentionDays { get; set; }
    public bool EnableRealTimeUpdates { get; set; }
    public string DatabasePath { get; set; }
    public string TelemetryDataPath { get; set; } = "TelemetryData";

    public void LoadConfiguration(Dictionary<string, string>? config)
    {
        if (config == null) return;

        if (config.TryGetValue("TelemetryPollingIntervalSeconds", out var interval))
        {
            TelemetryPollingIntervalSeconds = int.Parse(interval);
        }

        if (config.TryGetValue("MaxTelemetryHistoryCount", out var maxCount))
        {
            MaxTelemetryHistoryCount = int.Parse(maxCount);
        }

        if (config.TryGetValue("AlertRetentionDays", out var retention))
        {
            AlertRetentionDays = int.Parse(retention);
        }

        if (config.TryGetValue("EnableRealTimeUpdates", out var enableRT))
        {
            EnableRealTimeUpdates = bool.Parse(enableRT);
        }

        if (config.TryGetValue("DatabasePath", out var dbPath))
        {
            DatabasePath = dbPath;
        }

        if (config.TryGetValue("TelemetryDataPath", out var dataPath))
        {
            TelemetryDataPath = dataPath;
        }
    }
}
