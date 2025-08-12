using System.Net.WebSockets;
using Uber.Models.Domain;

namespace Uber.WebSockets.Interfaces
{
    public interface IWebSocketService
    {
        public Task BroadcastTrip(Tender tender);
        public Task assignSocketAsync(WebSocket socket, Guid tripid);
        public void closeConnection();
    }
}
