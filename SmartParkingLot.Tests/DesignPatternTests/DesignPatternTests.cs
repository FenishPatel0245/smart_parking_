using Xunit;
using SmartParkingLot.Application.DesignPatterns;
using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Tests.DesignPatternTests;

public class DeviceFactoryTests
{
    [Fact]
    public void CreateDevice_TemperatureSensor_ShouldReturnCorrectType()
    {
        // Arrange
        var factory = new DeviceFactory();

        // Act
        var device = factory.CreateDevice(DeviceType.TemperatureSensor);

        // Assert
        Assert.NotNull(device);
        Assert.Equal(DeviceType.TemperatureSensor, device.Type);
    }

    [Fact]
    public void CreateDevice_AllTypes_ShouldCreateSuccessfully()
    {
        // Arrange
        var factory = new DeviceFactory();
        var deviceTypes = Enum.GetValues<DeviceType>();

        // Act & Assert
        foreach (var type in deviceTypes)
        {
            var device = factory.CreateDevice(type);
            Assert.NotNull(device);
            Assert.Equal(type, device.Type);
        }
    }
}

public class AlertStrategyTests
{
    [Fact]
    public void ThresholdAlertStrategy_BelowThreshold_ShouldNotGenerateAlert()
    {
        // Arrange
        var strategy = new ThresholdAlertStrategy();

        // Act
        var shouldAlert = strategy.ShouldGenerateAlert(50.0, 80.0, 90.0);

        // Assert
        Assert.False(shouldAlert);
    }

    [Fact]
    public void ThresholdAlertStrategy_ExceedsWarning_ShouldGenerateAlert()
    {
        // Arrange
        var strategy = new ThresholdAlertStrategy();

        // Act
        var shouldAlert = strategy.ShouldGenerateAlert(85.0, 80.0, 90.0);

        // Assert
        Assert.True(shouldAlert);
    }

    [Fact]
    public void ThresholdAlertStrategy_ExceedsCritical_ShouldReturnCriticalSeverity()
    {
        // Arrange
        var strategy = new ThresholdAlertStrategy();

        // Act
        var severity = strategy.GetSeverity(95.0, 80.0, 90.0);

        // Assert
        Assert.Equal(AlertSeverity.Critical, severity);
    }

    [Fact]
    public void ThresholdAlertStrategy_ExceedsWarningOnly_ShouldReturnWarningSeverity()
    {
        // Arrange
        var strategy = new ThresholdAlertStrategy();

        // Act
        var severity = strategy.GetSeverity(85.0, 80.0, 90.0);

        // Assert
        Assert.Equal(AlertSeverity.Warning, severity);
    }
}

public class ConfigurationManagerTests
{
    [Fact]
    public void ConfigurationManager_ShouldBeSingleton()
    {
        // Arrange & Act
        var instance1 = ConfigurationManager.Instance;
        var instance2 = ConfigurationManager.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void ConfigurationManager_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var config = ConfigurationManager.Instance;

        // Assert
        Assert.Equal(3, config.TelemetryPollingIntervalSeconds);
        Assert.Equal(1000, config.MaxTelemetryHistoryCount);
        Assert.True(config.EnableRealTimeUpdates);
    }
}
