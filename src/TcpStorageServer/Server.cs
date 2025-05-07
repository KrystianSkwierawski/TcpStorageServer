using System.Net;
using System.Net.Sockets;

namespace TcpStorageServer
{
    public class Server
    {
        public async Task StartAsync()
        {
            using var server = new TcpListener(IPAddress.Any, 6379);
            server.Start();

            Console.WriteLine($"Server started");

            while (true)
            {            
                _ = Task.Run(async () =>
                {
                    using var socket = await server.AcceptSocketAsync();
                    var client = new Client();
                    await client.ConnectAsync(socket);
                });
            }
        }
    }
}
