using Microsoft.EntityFrameworkCore;
using SmartParkingLot.Domain.Models;

namespace SmartParkingLot.Infrastructure.Data;

/// <summary>
/// Database context for Smart Parking Lot system
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<TelemetryReading> TelemetryReadings { get; set; }
    public DbSet<Alert> Alerts { get; set; }
    public DbSet<EventLog> EventLogs { get; set; }
    public DbSet<ParkingSlot> ParkingSlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        // Device configuration
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DeviceId).IsUnique();
            entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.TelemetryFilePath).HasMaxLength(500);
            entity.Property(e => e.Unit).HasMaxLength(20);
            
            entity.HasMany(e => e.TelemetryReadings)
                .WithOne(e => e.Device)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.Alerts)
                .WithOne(e => e.Device)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TelemetryReading configuration
        modelBuilder.Entity<TelemetryReading>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DeviceId, e.Timestamp });
        });

        // Alert configuration
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DeviceId, e.CreatedAt });
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            
            entity.HasOne(e => e.AcknowledgedByUser)
                .WithMany()
                .HasForeignKey(e => e.AcknowledgedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // EventLog configuration
        modelBuilder.Entity<EventLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Timestamp);
            entity.Property(e => e.EventType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed admin user (password: admin123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = Domain.Enums.UserRole.Admin,
                FullName = "System Administrator",
                Email = "admin@scada.local",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Username = "operator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("operator123"),
                Role = Domain.Enums.UserRole.Operator,
                FullName = "System Operator",
                Email = "operator@scada.local",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 3,
                Username = "user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = Domain.Enums.UserRole.Operator, // Using Operator role for permissions
                FullName = "User",
                Email = "user@scada.local",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed Devices
        modelBuilder.Entity<Device>().HasData(
            new Device
            {
                Id = 1,
                DeviceId = "TEMP-001",
                Name = "B1 Temperature Sensor",
                Type = Domain.Enums.DeviceType.TemperatureSensor,
                Location = "Basement Level 1",
                Status = Domain.Enums.DeviceStatus.Normal,
                IsActive = true,
                TelemetryFilePath = "temperature-sensor-001.txt",
                Unit = "°F",
                WarningThreshold = 85,
                CriticalThreshold = 95,
                IsControllable = false
            },
            new Device
            {
                Id = 2,
                DeviceId = "GATE-001",
                Name = "Main Entry Gate",
                Type = Domain.Enums.DeviceType.EntryGateBarrier,
                Location = "Entrance A",
                Status = Domain.Enums.DeviceStatus.Normal,
                IsActive = true,
                TelemetryFilePath = "entry-gate-001.txt",
                Unit = "%",
                IsControllable = true
            },
            new Device
            {
                Id = 3,
                DeviceId = "FAN-001",
                Name = "Ventilation Fan Zone 1",
                Type = Domain.Enums.DeviceType.VentilationFan,
                Location = "Basement Level 1",
                Status = Domain.Enums.DeviceStatus.Normal,
                IsActive = true,
                TelemetryFilePath = "ventilation-fan-001.txt",
                Unit = "RPM",
                IsControllable = true
            },
            new Device
            {
                Id = 4,
                DeviceId = "LIGHT-001",
                Name = "Zone A Lighting",
                Type = Domain.Enums.DeviceType.SmartLighting,
                Location = "Level 1 Zone A",
                Status = Domain.Enums.DeviceStatus.Normal,
                IsActive = true,
                TelemetryFilePath = "smart-lighting-001.txt",
                Unit = "Lux",
                IsControllable = true
            },
            new Device
            {
                Id = 5,
                DeviceId = "TRAFFIC-001",
                Name = "Exit Traffic Counter",
                Type = Domain.Enums.DeviceType.TrafficCounter,
                Location = "Exit B",
                Status = Domain.Enums.DeviceStatus.Normal,
                IsActive = true,
                TelemetryFilePath = "traffic-counter-001.txt",
                Unit = "Cars/Min",
                IsControllable = true
            },
            new Device
            {
                Id = 6,
                DeviceId = "POWER-001",
                Name = "EV Charging Station 1",
                Type = Domain.Enums.DeviceType.PowerMeter,
                Location = "EV Zone",
                Status = Domain.Enums.DeviceStatus.Normal,
                IsActive = true,
                TelemetryFilePath = "power-meter-001.txt",
                Unit = "kW",
                IsControllable = true
            },
            new Device
            {
                Id = 7,
                DeviceId = "HUM-001",
                Name = "B1 Humidity Sensor",
                Type = Domain.Enums.DeviceType.HumiditySensor,
                Location = "Basement Level 1",
                Status = Domain.Enums.DeviceStatus.Normal,
                IsActive = true,
                TelemetryFilePath = "humidity-sensor-001.txt",
                Unit = "%",
                WarningThreshold = 70,
                CriticalThreshold = 80,
                IsControllable = false
            }
        );


        // Seed Parking Slots
        var slots = new List<ParkingSlot>();
        for (int i = 1; i <= 10; i++)
        {
            var type = "Regular";
            if (i == 1 || i == 2) type = "EV";
            if (i == 3) type = "Accessible";

            slots.Add(new ParkingSlot
            {
                Id = i,
                SlotNumber = $"A-{i:00}",
                IsOccupied = false,
                LastUpdated = DateTime.UtcNow,
                Type = type
            });
        }
        modelBuilder.Entity<ParkingSlot>().HasData(slots);
    }
}
