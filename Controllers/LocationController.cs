using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using WebSocket_Demo.Services;

namespace WebSocket_Demo.Controllers
{
    [Route("ws/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IWebSocketService _webSocketService;
        public LocationController(IWebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
        }

        [HttpGet("connect")]
        public async Task Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _webSocketService.HandleWebSocketConnection(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
