using System.Net.WebSockets;
using Uber.Models.Domain;

namespace Uber.Services.Interfaces
{
    public interface IWebSocketService
    {
        public  Task assignSocketAsync(WebSocket socket);
        public  Task Broadcastdata(object data);
        public void closeConnection();
    }
}
