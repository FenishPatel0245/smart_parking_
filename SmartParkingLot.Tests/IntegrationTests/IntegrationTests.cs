using Xunit;
using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Infrastructure.Data;
using SmartParkingLot.Infrastructure.Repositories;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Domain.Enums;

namespace SmartParkingLot.Tests.IntegrationTests;

public class DeviceRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DeviceRepository _repository;

    public DeviceRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        var factory = new TestDbContextFactory(options);
        _repository = new DeviceRepository(factory);
    }

    [Fact]
    public async Task AddDevice_ShouldPersistToDatabase()
    {
        // Arrange
        var device = new Device
        {
            DeviceId = "TEST-001",
            Name = "Test Temperature Sensor",
            Type = DeviceType.TemperatureSensor,
            Location = "Test Lab",
            IsControllable = false,
            Unit = "°F",
            WarningThreshold = 80.0,
            CriticalThreshold = 90.0,
            IsActive = true
        };

        // Act
        var result = await _repository.AddAsync(device);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        
        var retrieved = await _repository.GetByIdAsync(result.Id);
        Assert.NotNull(retrieved);
        Assert.Equal("TEST-001", retrieved.DeviceId);
    }

    [Fact]
    public async Task GetActiveDevices_ShouldReturnOnlyActiveDevices()
    {
        // Arrange
        await _repository.AddAsync(new Device
        {
            DeviceId = "ACTIVE-001",
            Name = "Active Device",
            Type = DeviceType.TemperatureSensor,
            IsActive = true
        });

        await _repository.AddAsync(new Device
        {
            DeviceId = "INACTIVE-001",
            Name = "Inactive Device",
            Type = DeviceType.HumiditySensor,
            IsActive = false
        });

        // Act
        var activeDevices = await _repository.GetActiveDevicesAsync();

        // Assert
        Assert.Single(activeDevices);
        Assert.Equal("ACTIVE-001", activeDevices.First().DeviceId);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

public class TelemetryServiceIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DeviceRepository _deviceRepository;
    private readonly TelemetryRepository _telemetryRepository;

    public TelemetryServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        var factory = new TestDbContextFactory(options);
        _deviceRepository = new DeviceRepository(factory);
        _telemetryRepository = new TelemetryRepository(factory);
    }

    [Fact]
    public async Task TelemetryReading_ShouldLinkToDevice()
    {
        // Arrange
        var device = await _deviceRepository.AddAsync(new Device
        {
            DeviceId = "TEMP-001",
            Name = "Temperature Sensor",
            Type = DeviceType.TemperatureSensor,
            IsActive = true
        });

        var reading = new TelemetryReading
        {
            DeviceId = device.Id,
            Value = 72.5,
            Timestamp = DateTime.UtcNow
        };

        // Act
        await _telemetryRepository.AddAsync(reading);
        var latestReading = await _telemetryRepository.GetLatestByDeviceIdAsync(device.Id);

        // Assert
        Assert.NotNull(latestReading);
        Assert.Equal(72.5, latestReading.Value);
        Assert.Equal(device.Id, latestReading.DeviceId);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
