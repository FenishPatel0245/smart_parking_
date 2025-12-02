using Xunit;
using SmartParkingLot.Devices;
using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Tests.DeviceTests;

public class TemperatureSensorTests
{
    [Fact]
    public void TemperatureSensor_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var sensor = new TemperatureSensor();

        // Assert
        Assert.Equal(DeviceType.TemperatureSensor, sensor.Type);
        Assert.Equal("°F", sensor.Unit);
        Assert.False(sensor.IsControllable);
    }

    [Fact]
    public void UpdateStatus_WhenValueBelowWarning_ShouldBeNormal()
    {
        // Arrange
        var sensor = new TemperatureSensor
        {
            WarningThreshold = 80.0,
            CriticalThreshold = 90.0
        };

        // Act
        typeof(BaseDevice).GetProperty("CurrentValue")!.SetValue(sensor, 75.0);
        sensor.UpdateStatus();

        // Assert
        Assert.Equal(DeviceStatus.Normal, sensor.Status);
    }

    [Fact]
    public void UpdateStatus_WhenValueExceedsWarning_ShouldBeWarning()
    {
        // Arrange
        var sensor = new TemperatureSensor
        {
            WarningThreshold = 80.0,
            CriticalThreshold = 90.0
        };

        // Act
        typeof(BaseDevice).GetProperty("CurrentValue")!.SetValue(sensor, 85.0);
        sensor.UpdateStatus();

        // Assert
        Assert.Equal(DeviceStatus.Warning, sensor.Status);
    }

    [Fact]
    public void UpdateStatus_WhenValueExceedsCritical_ShouldBeCritical()
    {
        // Arrange
        var sensor = new TemperatureSensor
        {
            WarningThreshold = 80.0,
            CriticalThreshold = 90.0
        };

        // Act
        typeof(BaseDevice).GetProperty("CurrentValue")!.SetValue(sensor, 95.0);
        sensor.UpdateStatus();

        // Assert
        Assert.Equal(DeviceStatus.Critical, sensor.Status);
    }
}

public class VentilationFanTests
{
    [Fact]
    public void VentilationFan_ShouldBeControllable()
    {
        // Arrange & Act
        var fan = new VentilationFan();

        // Assert
        Assert.True(fan.IsControllable);
        Assert.Equal(DeviceType.VentilationFan, fan.Type);
    }

    [Fact]
    public async Task ControlAsync_StartCommand_ShouldStartFan()
    {
        // Arrange
        var fan = new VentilationFan();

        // Act
        var result = await fan.ControlAsync("start");

        // Assert
        Assert.True(result);
        Assert.True(fan.IsRunning);
    }

    [Fact]
    public async Task ControlAsync_StopCommand_ShouldStopFan()
    {
        // Arrange
        var fan = new VentilationFan();
        await fan.ControlAsync("start");

        // Act
        var result = await fan.ControlAsync("stop");

        // Assert
        Assert.True(result);
        Assert.False(fan.IsRunning);
        Assert.Equal(0, fan.FanSpeed);
    }
}

public class EntryGateBarrierTests
{
    [Fact]
    public async Task ControlAsync_OpenCommand_ShouldOpenGate()
    {
        // Arrange
        var gate = new EntryGateBarrier();

        // Act
        var result = await gate.ControlAsync("open");

        // Assert
        Assert.True(result);
        Assert.Equal(100, gate.BarrierPosition);
        Assert.True(gate.IsOpen);
    }

    [Fact]
    public async Task ControlAsync_CloseCommand_ShouldCloseGate()
    {
        // Arrange
        var gate = new EntryGateBarrier();
        await gate.ControlAsync("open");

        // Act
        var result = await gate.ControlAsync("close");

        // Assert
        Assert.True(result);
        Assert.Equal(0, gate.BarrierPosition);
        Assert.False(gate.IsOpen);
    }

    [Fact]
    public async Task ControlAsync_SetPosition_ShouldSetCorrectPosition()
    {
        // Arrange
        var gate = new EntryGateBarrier();

        // Act
        var result = await gate.ControlAsync("setposition", 50.0);

        // Assert
        Assert.True(result);
        Assert.Equal(50, gate.BarrierPosition);
    }
}

public class SmartLightingTests
{
    [Fact]
    public async Task ControlAsync_OnCommand_ShouldTurnOnLight()
    {
        // Arrange
        var light = new SmartLighting();

        // Act
        var result = await light.ControlAsync("on");

        // Assert
        Assert.True(result);
        Assert.Equal(100, light.BrightnessLevel);
        Assert.True(light.IsOn);
    }

    [Fact]
    public async Task ControlAsync_DimCommand_ShouldSetBrightness()
    {
        // Arrange
        var light = new SmartLighting();

        // Act
        var result = await light.ControlAsync("dim", 60.0);

        // Assert
        Assert.True(result);
        Assert.Equal(60, light.BrightnessLevel);
    }
}
