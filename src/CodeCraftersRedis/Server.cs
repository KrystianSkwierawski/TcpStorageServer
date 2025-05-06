using System.Net;
using System.Net.Sockets;

namespace CodeCraftersRedis
{
    public class Server : IDisposable
    {
        private readonly TcpListener _server;

        public Server(int port = 6379)
        {
            _server = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartAsync()
        {
            _server.Start();

            Console.WriteLine($"Server started");

            while (true)
            {
                var socket = await _server.AcceptSocketAsync();

                var service = new Client();

                _ = Task.Run(async () => await service.HandleClientAsync(socket));
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
