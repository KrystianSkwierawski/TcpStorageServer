using System.Net.Sockets;
using System.Text;

namespace TcpStorageServer.IntegrationTests
{
    public class TestClient : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public TestClient()
        {
            _client = new TcpClient();
            _client.Connect("127.0.0.1", 6379);
            _stream = _client.GetStream();
        }

        public async Task<string> SendRequestAsync(string request)
        {
            var data = Encoding.UTF8.GetBytes(request);
            await _stream.WriteAsync(data, 0, data.Length);

            var buffer = new byte[1024];
            var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            return response;
        }

        public void Dispose()
        {
            _client.Dispose();
            _stream.Dispose();
        }
    }
}
