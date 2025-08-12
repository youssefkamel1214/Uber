using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Uber.Models.Domain;
using Uber.Models.Responses;
using Uber.Repositories.Interfaces;
using Uber.Services.Interfaces;

namespace Uber.Services
{
    public class WebSocketService: IWebSocketService
    {
        private WebSocket _socket;


        public async Task assignSocketAsync(WebSocket socket)
        {
            _socket = socket;
        }

        public async Task Broadcastdata(object data)
        {
            if ( _socket.State == WebSocketState.Open)
            {
                string message = JsonSerializer.Serialize(data);
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);
                await _socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public void closeConnection()
        {
            _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            _socket?.Dispose();
        }
    }
}
