using System.Net;
using System.Net.Sockets;
using codecrafters_redis.src;

TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();

Console.WriteLine("Server started on port 6379");

var clients = new List<Task>();

while (true)
{
    var socket = await server.AcceptSocketAsync();
    Console.WriteLine("New client connected");

    clients.Add(HandleClientAsync(socket));
}

async Task HandleClientAsync(Socket socket)
{
    using (socket)
    {
        while (socket.Connected)
        {
            await socket.SendMessageAsync("PONG\r\n");
        }
    }

    Console.WriteLine("Client disconnected");
}
