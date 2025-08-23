using Uber.Data;
using Uber.Models.Domain;
using Uber.Services.Interfaces;
using Uber.WebSockets;

namespace Uber.Services
{
    public class NotificationManger : INotificationManger
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationManger(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task notifyDriverChannel(string driverId, Dictionary<string, object> dataToSendToDriverChannel)
        {
            using var scope = _serviceProvider.CreateScope();
            {
                webSocketManager webSocketManager= scope.ServiceProvider.GetRequiredService<webSocketManager>();
                await webSocketManager.BroadcastdataToDriver(driverId, dataToSendToDriverChannel);
            }
            
        }

        public async Task notifyDriversChannel(Dictionary<string, object> dict)
        {
            using var scope = _serviceProvider.CreateScope();
            {
                webSocketManager webSocketManager = scope.ServiceProvider.GetRequiredService<webSocketManager>();
                await webSocketManager.BroadcastdataToDrivers(dict);
            }
        }

        public async Task notifyTripChannel(string tripID, Dictionary<string, object> dict)
        {
            using var scope = _serviceProvider.CreateScope();
            {
                webSocketManager webSocketManager = scope.ServiceProvider.GetRequiredService<webSocketManager>();
                await webSocketManager.BroadcastdataToTripChannel(tripID, dict);
            }
        }
    }
}
