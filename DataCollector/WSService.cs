using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;

namespace DataCollector
{
    public class WSService
    {
        //private readonly List<string> _symbols;
        private readonly SharedContext _context;
        private readonly ClientWebSocket _webSocket;
        private readonly String streams = "btcusdt@miniTicker/ethusdt@miniTicker/adausdt@miniTicker";

        public WSService(SharedContext context)
        {
            _context = context;
            _webSocket = new ClientWebSocket();
        }

        public async Task StartAsync()
        {
            var uri = new Uri($"wss://stream.binance.com:9443/stream?streams={streams}");
            await _webSocket.ConnectAsync(uri, CancellationToken.None);

            // Start receiving messages
            _ = Task.Run(() => ReceiveMessages());
        }

        private async Task ReceiveMessages()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await OnMessageReceivedAsync(message);
                }
            }
        }


        private async Task OnMessageReceivedAsync(string message)
        {
            var json = JObject.Parse(message);
            var stream = json["stream"]?.ToString();
            var data = json["data"];

            if (stream != null && data != null)
            {
                var symbol = data["s"]?.ToString().ToLower();
                var price = decimal.Parse(data["c"]?.ToString());  // Adjust this depending on the stream type
                var timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)data["E"]).UtcDateTime;

                if (symbol == "btcusdt" || symbol == "adausdt" || symbol == "ethusdt")
                {
                    Console.WriteLine($"Received data for {symbol}: {price}");

                    var priceRecord = new SymbolRecord
                    {
                        Symbol = symbol,
                        Price = price,
                        Timestamp = timestamp
                    };
                    await _context.SymbolRecords.AddAsync(priceRecord);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
