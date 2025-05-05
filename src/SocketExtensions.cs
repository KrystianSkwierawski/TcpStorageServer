using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src
{
    public static class SocketExtensions
    {
        public static async Task SendMessageAsync(this Socket socket, string message)
        {
            var buffer = Encoding.ASCII.GetBytes($"\r\n{message}\r\n");
            await socket.SendAsync(buffer);
        }
    }
}
