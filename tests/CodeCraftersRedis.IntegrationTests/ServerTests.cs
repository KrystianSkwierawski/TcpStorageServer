using CodeCraftersRedis.Common.Consts;
using FluentAssertions;

namespace CodeCraftersRedis.IntegrationTests
{
    public class ServerTests : BaseTest
    {
        [SetUp]
        public async Task SetUp()
        {
            await ConnectAsync();
        }

        [Test]
        [TestCase("asdasdasd 123123")]
        public async Task EchoTests(string message)
        {
            var request = $"{CommandsConst.Echo} {message}\n";

            var result = await SendRequestAsync(request);

            result.Should().Be($"\r\n{message}\r\n");
        }

        [Test]
        [TestCase("testkey", "testvalue")]
        public async Task SetWithourParamsTests(string key, string value)
        {
            var request = $"{CommandsConst.Set} {key} {value}\n";

            var result = await SendRequestAsync(request);

            result.Should().Contain(key);
        }

        [Test]
        [TestCase("testkey", "testvalue")]
        public async Task GetWithoutParamsTests(string key, string value)
        {
            var request = $"{CommandsConst.Get} {key}\n";

            await SendRequestAsync($"{CommandsConst.Set} {key} {value}\n");
            var result = await SendRequestAsync(request);

            result.Should().Be($"\r\n{value}\r\n");
        }


        [TearDown]
        public void TearDown()
        {
            Disconnect();
        }
    }
}
