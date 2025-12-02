using Microsoft.AspNetCore.SignalR;
using SmartParkingLot.Application.Services;
using SmartParkingLot.Web.Hubs;

namespace SmartParkingLot.Web.Services
{
    public class SignalRBroadcastService : IBroadcastService
    {
        private readonly IHubContext<TelemetryHub> _hubContext;

        public SignalRBroadcastService(IHubContext<TelemetryHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task BroadcastDashboardUpdateAsync()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveDashboardUpdate");
        }
    }
}
