using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class NotContainsOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void String()
        {
            CreateEqualsCondition(AttributeType.String, ".net");

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, "dotnet-sdk")
            };

            OnVariation(attributes);
        }

        private void CreateEqualsCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.NCON,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}