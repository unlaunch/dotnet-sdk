using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class NewOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void StringEndsWith()
        {
            CreateNewCondition(AttributeType.String, "sdk");

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, "dotnet-sdk")
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void StringNotEndsWith()
        {
            CreateNewCondition(AttributeType.String, "dotnet");

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, "dotnet-sdk")
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateNewCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.NEW,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}