using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class InvalidAttributeKeyTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void InvalidAttributeKey()
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new[] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = AttributeType.Boolean,
                op = Operator.EQ,
                value = "True"
            }};

            LoadFeatureFlags();

            var attributes = new[]
            {
                UnlaunchAttribute.NewBoolean("wrongKey", true)
            };

            OffVariation(attributes);
        }
    }
}
