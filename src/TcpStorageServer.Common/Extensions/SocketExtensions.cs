using System.Net.Sockets;
using System.Text;

namespace TcpStorageServer.Common.Extensions
{
    public static class SocketExtensions
    {
        public static async Task SendResponseAsync(this Socket socket, string? message)
        {
            var buffer = Encoding.ASCII.GetBytes($"\r\n{message ?? "$-1"}\r\n");
            await socket.SendAsync(buffer);
        }
    }
}
