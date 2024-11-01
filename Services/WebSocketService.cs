using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using WebSocket_Demo.Models;

namespace WebSocket_Demo.Services
{
    public interface IWebSocketService
    {
        Task HandleWebSocketConnection(WebSocket webSocket);
    }
    public class WebSocketService : IWebSocketService
    {
        private readonly IMongoDbService _mongoDbService;
        public WebSocketService(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }
        public async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            try
            {
                var buffer = new byte[1024 * 4];

                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    }
                    else
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var locationUpdate = JsonSerializer.Deserialize<LocationUpdate>(receivedMessage,options);

                        if (locationUpdate != null)
                        {
                            await _mongoDbService.SaveLocationUpdate(locationUpdate);

                            var responseMessage = Encoding.UTF8.GetBytes("Location update saved to MongoDB");
                            await webSocket.SendAsync(new ArraySegment<byte>(responseMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
