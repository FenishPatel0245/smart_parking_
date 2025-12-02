# Smart Parking Lot System

A comprehensive, real-time monitoring and management system for modern smart parking facilities. Built with C# .NET 8, Blazor Server, and Clean Architecture.

## 🚀 Features

- **Real-Time Monitoring**: Live status updates of all parking facility devices.
- **Device Control**: Remote control for barriers, fans, lighting, and more.
- **Role-Based Access**:
  - **Manager (Admin)**: Full system configuration, device management, user management.
  - **Attendant (Operator)**: Daily operations, monitoring, and basic controls.
- **Smart Alerts**: Automatic detection of critical conditions (e.g., high CO levels, unauthorized entry).
- **Dark Mode UI**: Professional dashboard optimized for control room environments.

## 🏗️ Architecture

The solution follows **Clean Architecture** principles:

- **SmartParkingLot.Domain**: Core entities (Device, User, Alert) and enums.
- **SmartParkingLot.Infrastructure**: Database context (SQLite) and repositories.
- **SmartParkingLot.Application**: Business logic, services, and design patterns.
- **SmartParkingLot.Devices**: Device drivers and simulation logic.
- **SmartParkingLot.Web**: Blazor Server UI with SignalR for real-time updates.

## 🔧 Device Modules

The system monitors and controls 7 types of smart facility equipment:

1. **Entry Gate Barrier**: Control open/close status and position.
2. **Ventilation Fan**: Manage air quality control fans (Start/Stop/Speed).
3. **Smart Lighting**: Control zone lighting (On/Off/Dim).
4. **Traffic Counter**: Monitor vehicle flow rates at entry/exit points.
5. **Power Meter**: Track energy consumption of EV charging stations.
6. **Temperature Sensor**: Monitor facility temperature.
7. **Humidity Sensor**: Monitor facility humidity levels.

## 🏁 Quick Start

### Prerequisites
- .NET 8.0 SDK

### Run the Application

**Option 1: PowerShell Script (Recommended)**
```powershell
.\start.ps1
```

**Option 2: Manual**
```bash
dotnet restore
dotnet build
cd SmartParkingLot.Web
dotnet run
```

### Login Credentials

| Role | Username | Password |
|------|----------|----------|
| **Manager** | `admin` | `admin123` |
| **Attendant** | `operator` | `operator123` |

## 🧪 Testing

Run the test suite to verify system integrity:

```bash
dotnet test
```

## 📝 Configuration

System settings can be adjusted in `appsettings.json`:
- Telemetry polling interval
- Data retention policies
- Feature flags

---
Built for the **Smart Parking Lot** initiative.
