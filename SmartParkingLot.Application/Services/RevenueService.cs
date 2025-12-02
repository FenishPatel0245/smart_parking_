using SmartParkingLot.Infrastructure.Repositories;
using SmartParkingLot.Domain.Models;

namespace SmartParkingLot.Application.Services;

public interface IRevenueService
{
    Task<decimal> GetTotalRevenueAsync();
    Task RecordTransactionAsync(decimal amount, string paymentMethod, int? parkingSlotId = null);
}

public class RevenueService : IRevenueService
{
    private readonly IParkingTransactionRepository _transactionRepository;
    private readonly IActivityLogService _logService;

    public RevenueService(
        IParkingTransactionRepository transactionRepository,
        IActivityLogService logService)
    {
        _transactionRepository = transactionRepository;
        _logService = logService;
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _transactionRepository.GetTotalRevenueAsync();
    }

    public async Task RecordTransactionAsync(decimal amount, string paymentMethod, int? parkingSlotId = null)
    {
        var transaction = new ParkingTransaction
        {
            Amount = amount,
            PaymentMethod = paymentMethod,
            ParkingSlotId = parkingSlotId,
            TransactionDate = DateTime.UtcNow,
            Status = "Completed"
        };

        await _transactionRepository.AddAsync(transaction);

        _logService.LogActivity(
            "Payment Received", 
            $"Payment of ${amount:N2} received via {paymentMethod}", 
            "Success", 
            "System", 
            "fas fa-dollar-sign");
    }
}
