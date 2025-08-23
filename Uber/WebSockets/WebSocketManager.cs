using System.Collections.Concurrent;
using Uber.Models.Domain;
using Uber.Services.Interfaces;

namespace Uber.WebSockets
{
    public class webSocketManager
    {

        private ConcurrentDictionary<string,List< IWebSocketService>> _connections = new ConcurrentDictionary<string, List<IWebSocketService>>();
        private ConcurrentDictionary<string, IWebSocketService> _driverConnections = new ConcurrentDictionary<string, IWebSocketService>();


        public void AddConnectionOnTripId(string guid, IWebSocketService webSockets)
        {
            if (!_connections.ContainsKey(guid))
                _connections.TryAdd(guid, [webSockets]);
            else 
            {
                _connections[guid].Add(webSockets);
            }
        }
        public void AddConnectionDriver(string guid, IWebSocketService webSockets)
        {
            _driverConnections.TryAdd(guid, webSockets);
        }
        public void RemoveConnectionOnTrip(string guid, IWebSocketService _webSocket)
        {

            if (!_connections.ContainsKey(guid))
                return;

            foreach (var _web in _connections[guid])
                if (_web.Equals(_webSocket)) 
                { 
                    _web.closeConnection();
                    _connections[guid].Remove(_web);
                    break;
                }
            if (_connections[guid].Count==0)
                _connections.TryRemove(guid, out List<IWebSocketService>? _webSocketList);
        }
        public void RemoveAllConnectionOnTrip(string guid)
        {
            _connections.TryRemove(guid, out List< IWebSocketService>? _webSocketList);
            foreach(var _web in _webSocketList)
             _web.closeConnection();
        }
        public void RemoveConnectionDriver(string guid)
        {
            _driverConnections.TryRemove(guid, out IWebSocketService? _web);
            _web?.closeConnection();
 
        }
        public async Task BroadcastdataToTripChannel(string guid,object data)
        {
            if (_connections.TryGetValue(guid, out var webSocketList))
            {
                foreach(var webSocketService in webSocketList)
                 await webSocketService.Broadcastdata(data);
            }
        }
        public async Task BroadcastdataToDrivers( object data)
        {
            foreach (var webSocket in _driverConnections.Values) 
            {

                if(webSocket == null) continue;
                await webSocket.Broadcastdata(data);
            }
        }
        public async Task BroadcastdataToDriver(string driverId,object data)
        {
          if(_driverConnections.ContainsKey(driverId))
            await _driverConnections[driverId].Broadcastdata(data);
            
        }
    }
}

