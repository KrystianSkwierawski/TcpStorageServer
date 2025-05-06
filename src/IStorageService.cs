using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src
{
    public interface IStorageService
    {
        Task HandleClientAsync(Socket socket);
    }

    public class StorageService : IStorageService
    {
        private readonly Dictionary<string, StorageObject> _storage = new Dictionary<string, StorageObject>();

        public async Task HandleClientAsync(Socket socket)
        {
            try
            {
                Console.WriteLine("New client connected");

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

                        var result = HandleCommand(request);

                        if (result != null)
                        {
                            await socket.SendMessageAsync(result);
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

        private string? HandleCommand(string request)
        {
            var command = request.Split(' ')[0];

            return command switch
            {
                CommandsConst.Echo => Echo(request),
                CommandsConst.Set => Set(request),
                CommandsConst.Get => Get(request),
                _ => null,
            };
        }

        private string Echo(string request)
        {
            var parts = request.Split(' ');

            return string.Join(' ', parts.Skip(1));
        }

        private string Set(string request)
        {
            var parts = request.Split(' ');

            var key = parts[1];

            var value = string.Join(' ', parts.Skip(2));

            Console.WriteLine($"Set -> Key: {key}, Value: {value}");

            _storage.Add(key, new StorageObject
            {
                Value = value,
                ExpiryDate = DateTime.Now.AddDays(1)
            });

            return "+OK\r\n";
        }

        private string Get(string request)
        {
            var parts = request.Split(' ');

            var key = parts[1];

            _storage.TryGetValue(key, out var cache);

            return cache != null && cache.ExpiryDate > DateTime.Now
                ? $"+{cache.Value}\r\n"
                : "$-1\r\n";
        }
    }
}
