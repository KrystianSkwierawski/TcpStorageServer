using System.Net.Sockets;
using System.Text;

namespace TcpStorageServer.Common.Extensions
{
    public static class SocketExtensions
    {
        public static async Task SendResponseAsync(this Socket socket, string? message = "$-1")
        {
            var buffer = Encoding.ASCII.GetBytes($"\r\n{message}\r\n");
            await socket.SendAsync(buffer);
        }
    }
}
