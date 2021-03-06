using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class ContainsOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void StringContains()
        {
            CreateContainsCondition(AttributeType.String, "net");

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, "dotnet-sdk")
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void StringNotContains()
        {
            CreateContainsCondition(AttributeType.String, "dotnet");

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, "dotnet-sdk")
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateContainsCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.CON,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}