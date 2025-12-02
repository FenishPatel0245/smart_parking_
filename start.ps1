# SCADA Monitoring System - Quick Start Script
# This script will build and run the application

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Smart Parking Lot System Setup" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET 8 is installed
Write-Host "Checking .NET 8 SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK version: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 8 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Red
    exit 1
}
Write-Host "✓ .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Restore dependencies
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore packages" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Packages restored successfully" -ForegroundColor Green
Write-Host ""

# Build solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build completed successfully" -ForegroundColor Green
Write-Host ""

# Run tests
Write-Host "Running tests..." -ForegroundColor Yellow
dotnet test --no-build --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "WARNING: Some tests failed" -ForegroundColor Yellow
}
else {
    Write-Host "✓ All tests passed" -ForegroundColor Green
}
Write-Host ""

# Display login credentials
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Login Credentials:" -ForegroundColor Yellow
Write-Host "  Admin:    admin / admin123" -ForegroundColor White
Write-Host "  Operator: operator / operator123" -ForegroundColor White
Write-Host ""
Write-Host "Starting application..." -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor DarkGray
Write-Host ""

# Run the application
Set-Location SmartParkingLot.Web
dotnet run
