using Xunit;
using Moq;
using SmartParkingLot.Application.Services;
using SmartParkingLot.Infrastructure.Repositories;
using SmartParkingLot.Domain.Models;

namespace SmartParkingLot.Tests
{
    public class RevenueTests
    {
        [Fact]
        public async Task GetTotalRevenueAsync_ReturnsCorrectSum()
        {
            // Arrange
            var mockRepo = new Mock<IParkingTransactionRepository>();
            mockRepo.Setup(r => r.GetTotalRevenueAsync())
                .ReturnsAsync(150.00m);

            var mockLog = new Mock<IActivityLogService>();

            var service = new RevenueService(mockRepo.Object, mockLog.Object);

            // Act
            var result = await service.GetTotalRevenueAsync();

            // Assert
            Assert.Equal(150.00m, result);
        }

        [Fact]
        public async Task RecordTransactionAsync_AddsTransactionAndLogs()
        {
            // Arrange
            var mockRepo = new Mock<IParkingTransactionRepository>();
            var mockLog = new Mock<IActivityLogService>();

            var service = new RevenueService(mockRepo.Object, mockLog.Object);
            var amount = 50.00m;
            var method = "Credit Card";

            // Act
            await service.RecordTransactionAsync(amount, method);

            // Assert
            mockRepo.Verify(r => r.AddAsync(It.Is<ParkingTransaction>(t => 
                t.Amount == amount && 
                t.PaymentMethod == method && 
                t.Status == "Completed")), Times.Once);

            mockLog.Verify(l => l.LogActivity(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                "Success", 
                "System", 
                "fas fa-dollar-sign"), Times.Once);
        }
    }
}
