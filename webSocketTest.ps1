# Define WebSocket URL and duration
$webSocketUrl = "wss://localhost:7176/ws/Location/connect"
$duration = 12 * 60  # Duration in seconds (12 minutes)
$interval = 1        # Interval in seconds (now every second)
$userId = "user123"

# Load WebSocket functions
Add-Type @"
using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
public class WebSocketClient {
    public static async System.Threading.Tasks.Task<string> SendMessageAsync(string url, string message) {
        using (ClientWebSocket ws = new ClientWebSocket()) {
            await ws.ConnectAsync(new Uri(url), CancellationToken.None);
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

            // Buffer to receive the response
            byte[] buffer = new byte[1024];
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string response = Encoding.UTF8.GetString(buffer, 0, result.Count);

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
            return response;
        }
    }
}
"@

# Function to generate random latitude and longitude within a given range
function Get-RandomCoordinates {
    $latitude = Get-Random -Minimum 40.70 -Maximum 40.73
    $longitude = Get-Random -Minimum -74.01 -Maximum -73.98
    return @{
        latitude = [Math]::Round($latitude, 4)
        longitude = [Math]::Round($longitude, 4)
    }
}

# Loop to send messages every second for 12 minutes
for ($i = 0; $i -lt $duration; $i += $interval) {
    $coords = Get-RandomCoordinates
    $message = @{
        userId = $userId
        latitude = $coords.latitude
        longitude = $coords.longitude
    } | ConvertTo-Json

    # Call WebSocket send function and print response
    $response = [WebSocketClient]::SendMessageAsync($webSocketUrl, $message).GetAwaiter().GetResult()
    Write-Output "Response at second $i: $response"
    
    Start-Sleep -Seconds $interval
}
