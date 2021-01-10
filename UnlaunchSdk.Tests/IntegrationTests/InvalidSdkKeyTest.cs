using FluentAssertions;
using io.unlaunch;
using Xunit;

namespace UnlaunchSdk.Tests.IntegrationTests
{
    public class InvalidSdkKeyTest
    {
        [Fact]
        public void Initialize()
        {
            var client = UnlaunchClient.Create(System.Guid.NewGuid().ToString());

            var variation = client.GetVariation("flagKey", "userId123");
            variation.Should().Be(UnlaunchConstants.FlagDefaultReturnType);

            client.Shutdown();
        }
    }
}
