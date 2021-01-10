using System;
using FluentAssertions;
using io.unlaunch;

namespace UnlaunchSdk.Tests.IntegrationTests
{
    public class UnlaunchClientTest
    {
        private const string On = "on";
        private const string Off = "off";
        
        private readonly IUnlaunchClient _client;
        
        string userId = "10";
        UnlaunchAttribute attr1;
        UnlaunchAttribute attr2;
        UnlaunchAttribute attr3;

        string user2Id = "70";
        UnlaunchAttribute attr2_1;
        UnlaunchAttribute attr2_2;
        UnlaunchAttribute attr2_3;

        string user3Id = "90";
        UnlaunchAttribute attr3_1;
        UnlaunchAttribute attr3_2;
        UnlaunchAttribute attr3_3;

        public UnlaunchClientTest()
        {
            _client = UnlaunchClient.Builder().Host("https://api-qa.unlaunch.io").
                SdkKey("test-sdk-ff367fd3-accc-43e2-88d4-24edda0206c3").Build();

            attr1 = UnlaunchAttribute.NewString("account_type", "prepaid");
            attr2 = UnlaunchAttribute.NewNumber("max_loan", 500);
            attr3 = UnlaunchAttribute.NewNumber("min_loan", 50);

            attr2_1 = UnlaunchAttribute.NewString("account_type", "postpaid");
            attr2_2 = UnlaunchAttribute.NewNumber("max_loan", 500);
            attr2_3 = UnlaunchAttribute.NewNumber("min_loan", 50);

            attr3_1 = UnlaunchAttribute.NewString("account_type", "prepaid");
            attr3_2 = UnlaunchAttribute.NewNumber("max_loan", 400);
            attr3_3 = UnlaunchAttribute.NewNumber("min_loan", 50);

            _client.IsReady().Should().BeFalse();
            
            try
            {
                _client.AwaitUntilReady(10000);
            }
            catch (TimeoutException e) {
                Console.WriteLine("Client was not ready");
                e.Should().BeNull();
            }

            _client.IsReady().Should().BeTrue();
        }
        
        public void BoolEvaluate()
        {
            var varKey = _client.GetVariation("bolsas", userId, new [] {attr1, attr2, attr3});
            varKey.Should().Be(On);

            varKey = _client.GetVariation("bolsas", user3Id, new[] {attr3_1, attr3_2, attr3_3});
            varKey.Should().Be(On);
        }
    }
}
