using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class EndsWithOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void String()
        {
            CreateEndsWithCondition(AttributeType.String, "sdk");

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, "dotnet-sdk")
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateEndsWithCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.EW,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}