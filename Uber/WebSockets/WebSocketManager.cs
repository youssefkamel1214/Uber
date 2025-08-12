using System.Collections.Concurrent;
using Uber.Models.Domain;
using Uber.Services.Interfaces;

namespace Uber.WebSockets
{
    public class webSocketManager
    {

        private ConcurrentDictionary<string, IWebSocketService> _connections = new ConcurrentDictionary<string, IWebSocketService>();
        private ConcurrentDictionary<string, IWebSocketService> _driverConnections = new ConcurrentDictionary<string, IWebSocketService>();


        public void AddConnection(string guid, IWebSocketService webSockets)
        {
            _connections.TryAdd(guid, webSockets);
        }
        public void AddConnectionDriver(string guid, IWebSocketService webSockets)
        {
            _driverConnections.TryAdd(guid, webSockets);
        }
        public void RemoveConnection(string guid)
        {
            _connections.TryRemove(guid, out IWebSocketService? _web);
            _web.closeConnection();
        }
        public void RemoveConnectionDriver(string guid)
        {
            _driverConnections.TryRemove(guid, out IWebSocketService? _web);
            _web?.closeConnection();
 
        }
        public async Task BroadcastdataToTripChannel(string guid,object data)
        {
            if (_connections.TryGetValue(guid, out var webSocketService))
            {
                await webSocketService.Broadcastdata(data);
            }
        }
        public async Task BroadcastdataToDriver( object data)
        {
            foreach (var webSocket in _driverConnections.Values) 
            {

                if(webSocket == null) continue;
                await webSocket.Broadcastdata(data);
            }
        }
    }
}

