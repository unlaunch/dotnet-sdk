using System.Linq;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.evaluator
{
    public class AllowListTest : UnlaunchContext
    {
        [Fact]
        public void UserInAllowList()
        {
            CreateVariations("user1,user2");
            OnVariationUserInAllowList("user1");
        }

        [Fact]
        public void UserNotInAllowList()
        {
            CreateVariations("user1,user2");
            OffVariationUserNotInAllowList("user3");
        }

        private void CreateVariations(string allowList)
        {
            var flag = FlagResponse.data.flags.First();
            flag.variations = new[]
            {
                new VariationDto
                {
                    id = OnVariationId,
                    key = "on",
                    name = "variation1",
                    allowList = allowList
                },
                new VariationDto
                {
                    id = OffVariationId,
                    key = "off",
                    name = "variation2"
                }
            };
            
            LoadFeatureFlags();
        }
    }
}
