using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis.src;

TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();

Console.WriteLine("Server started on port 6379");


while (true)
{
    var socket = await server.AcceptSocketAsync();
    Console.WriteLine("New client connected");

    _ = HandleClientAsync(socket);
}

async Task HandleClientAsync(Socket socket)
{
    try
    {
        using var stream = new NetworkStream(socket);
        var stringBuilder = new StringBuilder();
        var buffer = new byte[1024];

        while (socket.Connected)
        {
            var bytesRead = await socket.ReceiveAsync(buffer);
            if (bytesRead == 0)
            {
                break;
            }

            stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            if (stringBuilder.ToString().Contains("\n"))
            {
                var request = stringBuilder.ToString().Trim();
                stringBuilder.Clear();
                var command = "echo ";

                if (request.StartsWith(command, StringComparison.OrdinalIgnoreCase))
                {
                    var message = request.Replace(command, string.Empty);

                    await socket.SendMessageAsync(message);
                }
            }
        }
    }
    finally
    {
        socket.Close();
        Console.WriteLine("Client disconnected");
    }
}
