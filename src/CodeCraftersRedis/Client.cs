using System.Net.Sockets;
using System.Text;
using CodeCraftersRedis.Common.Consts;
using CodeCraftersRedis.Common.Extensions;
using CodeCraftersRedis.Common.Models;

namespace CodeCraftersRedis
{
    public class Client
    {
        private readonly Dictionary<string, StorageValue> _storage = new Dictionary<string, StorageValue>();

        public async Task HandleClientAsync(Socket socket)
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
                    await socket.SendResponseAsync(result);
                }
            }
        }

        private string? HandleCommand(string request)
        {
            var command = request.Split(' ')[0].ToLower();

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
            var value = parts[2];
            var expiry = parts.ElementAtOrDefault(3) ?? "300000";

            if (_storage.ContainsKey(key))
            {
                _storage.Remove(key);
            }

            var storageValue = new StorageValue
            {
                Value = value,
                ExpiryDate = DateTime.Now.AddMicroseconds(int.Parse(expiry))
            };

            _storage.Add(key, storageValue);

            return $"+{key} -> ExpiryDate: {storageValue.ExpiryDate}";
        }

        private string? Get(string request)
        {
            var parts = request.Split(' ');

            var key = parts[1];

            _storage.TryGetValue(key, out var cache);

            if (cache?.ExpiryDate > DateTime.Now)
            {
                return cache.Value;
            }

            return null;
        }
    }
}
