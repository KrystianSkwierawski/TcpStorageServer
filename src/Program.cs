using System.Net;
using System.Net.Sockets;
using codecrafters_redis.src;

TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();

Console.WriteLine("Server started on port 6379");

while (true)
{
    var socket = await server.AcceptSocketAsync();

    IStorageService service = new StorageService();

    _ = service.HandleClientAsync(socket);
}



