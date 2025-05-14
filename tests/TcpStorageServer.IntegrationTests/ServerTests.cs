using FluentAssertions;
using TcpStorageServer.Common.Consts;

namespace TcpStorageServer.IntegrationTests
{
    public class ServerTests
    {
        [Test]
        [TestCase("test message")]
        [Order(0)]
        public async Task EchoTest(string message)
        {
            // Arrange
            using var client = new TestClient();
            var request = $"{CommandsConst.Echo} {message}\n";

            // Act
            var result = await client.SendRequestAsync(request);

            // Assert
            result.Should().Be($"\r\n{message}\r\n");
        }

        [Test]
        [TestCase("testkey", "testvalue")]
        [Order(1)]
        public async Task SetWithoutParamsTest(string key, string value)
        {
            // Arrange
            using var client = new TestClient();
            var request = $"{CommandsConst.Set} {key} {value}\n";

            // Act
            var result = await client.SendRequestAsync(request);

            // Assert
            result.Should().Contain(key);
        }

        [Test]
        [TestCase("testkey", "testvalue")]
        [Order(2)]
        public async Task GetWithoutParamsTest(string key, string value)
        {
            // Arrange
            using var client = new TestClient();
            var request = $"{CommandsConst.Get} {key}\n";

            // Act
            await client.SendRequestAsync($"{CommandsConst.Set} {key} {value}\n");
            var result = await client.SendRequestAsync(request);

            // Assert
            result.Should().Be($"\r\n{value}\r\n");
        }

        [Test]
        [TestCase("testkey", "testvalue", 1000)]
        [Order(3)]
        public async Task GetWithExpiry(string key, string value, int expiry)
        {
            // Arrange
            using var client = new TestClient();
            var request = $"{CommandsConst.Get} {key}\n";

            // Act
            await client.SendRequestAsync($"{CommandsConst.Set} {key} {value} {expiry}\n");
            await Task.Delay(expiry);
            var result = await client.SendRequestAsync(request);

            // Assert
            result.Should().Be("\r\n$-1\r\n");
        }

        [Test]
        [TestCase("testkey", "testvalue", 5)]
        [Order(4)]
        public async Task MultipleClientsTest(string key, string value, int clientsCount)
        {
            // Arrange
            var clients = new List<TestClient>();

            for (int i = 0; i < clientsCount; i++)
            {
                var client = new TestClient();
                clients.Add(client);

                await client.SendRequestAsync($"{CommandsConst.Set} {key} {value}_client{i}\n");
            }

            // Act & Assert
            for (int i = 0; i < clientsCount; i++)
            {
                var request = $"{CommandsConst.Get} {key}\n";
                var result = await clients[i].SendRequestAsync(request);

                result.Should().Be($"\r\n{value}_client{i}\r\n");
            }

            clients.ForEach(client => client.Dispose());
        }
    }
}
