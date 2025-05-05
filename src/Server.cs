using System.Net;
using System.Net.Sockets;
using codecrafters_redis.src;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();
var socket = await server.AcceptSocketAsync(); // wait for client

while (socket.Connected)
{
    await socket.SendMessageAsync("PONG\r\n");
}

