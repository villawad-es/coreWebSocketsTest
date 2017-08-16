using System;
using System.Threading.Tasks;

using System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;

namespace WsWeb
{
    public class SocketHandler
    {
        public const int BufferSize = 4096;

        WebSocket socket;

        static UTF8Encoding encoder = new UTF8Encoding();

        SocketHandler(WebSocket socket)
        {
            this.socket = socket;
        }

        async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);

            while (this.socket.State == WebSocketState.Open)
            {
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                var text0 = encoder.GetString(buffer);
                var textSplit = text0.Split('\0');
                var textSplit0 = textSplit[0];
                var objt1 = JsonConvert.DeserializeObject(textSplit0);
                var outgoing = new ArraySegment<byte>(buffer, 0, incoming.Count);
                var text1 = encoder.GetString(outgoing.Array);

                string resp1 = "{\"roto\" : true}";
                byte[] arr1 = Encoding.UTF8.GetBytes(resp1);
                int len1 = arr1.GetLength(0);
                var out1 = new ArraySegment<byte>(arr1, 0, len1);
                //var text2 = encoder.GetString(incoming);
                //await this.socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);

                await this.socket.SendAsync(out1, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new SocketHandler(socket);
            await h.EchoLoop();
        }

        /// <summary>
        /// branches the request pipeline for this SocketHandler usage
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(SocketHandler.Acceptor);
        }
    }
}
