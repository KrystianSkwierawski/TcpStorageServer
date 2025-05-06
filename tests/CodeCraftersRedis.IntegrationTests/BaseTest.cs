using System.Net.Sockets;
using System.Text;

namespace CodeCraftersRedis.IntegrationTests
{
    public class BaseTest
    {
        private TcpClient _client;
        private NetworkStream _stream; 

        protected async Task<string> SendRequestAsync(string request)
        {
            var data = Encoding.UTF8.GetBytes(request);
            await _stream.WriteAsync(data, 0, data.Length);

            var buffer = new byte[1024];
            var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            return response;
        }

        protected async Task ConnectAsync()
        {
            _ = Task.Run(async () =>
            {
                using var server = new Server();
                await server.StartAsync();
            });

            await Task.Delay(1000);

            _client = new TcpClient();

            await _client.ConnectAsync("127.0.0.1", 6379);
            _stream = _client.GetStream();
        }

        protected void Disconnect()
        {
            _client.Dispose();
            _stream.Dispose();
        }
    }
}
