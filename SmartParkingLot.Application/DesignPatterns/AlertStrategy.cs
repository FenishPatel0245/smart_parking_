using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Domain.Enums;
using SmartParkingLot.Domain.Models;

namespace SmartParkingLot.Application.DesignPatterns;

/// <summary>
/// Strategy pattern for alert rule evaluation
/// </summary>
public interface IAlertStrategy
{
    bool ShouldGenerateAlert(double currentValue, double? warningThreshold, double? criticalThreshold);
    AlertSeverity GetSeverity(double currentValue, double? warningThreshold, double? criticalThreshold);
    string GenerateMessage(string deviceName, double currentValue, string unit, AlertSeverity severity);
}

public class ThresholdAlertStrategy : IAlertStrategy
{
    public bool ShouldGenerateAlert(double currentValue, double? warningThreshold, double? criticalThreshold)
    {
        if (!warningThreshold.HasValue && !criticalThreshold.HasValue)
        {
            return false;
        }

        if (criticalThreshold.HasValue && currentValue >= criticalThreshold.Value)
        {
            return true;
        }

        if (warningThreshold.HasValue && currentValue >= warningThreshold.Value)
        {
            return true;
        }

        return false;
    }

    public AlertSeverity GetSeverity(double currentValue, double? warningThreshold, double? criticalThreshold)
    {
        if (criticalThreshold.HasValue && currentValue >= criticalThreshold.Value)
        {
            return AlertSeverity.Critical;
        }

        if (warningThreshold.HasValue && currentValue >= warningThreshold.Value)
        {
            return AlertSeverity.Warning;
        }

        return AlertSeverity.Info;
    }

    public string GenerateMessage(string deviceName, double currentValue, string unit, AlertSeverity severity)
    {
        return severity switch
        {
            AlertSeverity.Critical => $"CRITICAL: {deviceName} has reached critical level: {currentValue:F2} {unit}",
            AlertSeverity.Warning => $"WARNING: {deviceName} has exceeded warning threshold: {currentValue:F2} {unit}",
            _ => $"INFO: {deviceName} status update: {currentValue:F2} {unit}"
        };
    }
}

public class RateOfChangeAlertStrategy : IAlertStrategy
{
    private readonly Dictionary<string, double> _previousValues = new();
    private readonly double _maxChangeRate;

    public RateOfChangeAlertStrategy(double maxChangeRate = 10.0)
    {
        _maxChangeRate = maxChangeRate;
    }

    public bool ShouldGenerateAlert(double currentValue, double? warningThreshold, double? criticalThreshold)
    {
        // This strategy would check rate of change
        // Simplified implementation for demonstration
        return false;
    }

    public AlertSeverity GetSeverity(double currentValue, double? warningThreshold, double? criticalThreshold)
    {
        return AlertSeverity.Info;
    }

    public string GenerateMessage(string deviceName, double currentValue, string unit, AlertSeverity severity)
    {
        return $"Rate of change alert for {deviceName}: {currentValue:F2} {unit}";
    }
}
