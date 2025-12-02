using System.Threading.Tasks;

namespace SmartParkingLot.Application.Services
{
    public interface IBroadcastService
    {
        Task BroadcastDashboardUpdateAsync();
    }
}
