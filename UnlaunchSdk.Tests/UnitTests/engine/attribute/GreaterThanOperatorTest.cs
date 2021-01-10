using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using io.unlaunch.utils;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class GreaterThanOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void DateTime()
        {
            var unixTime = UnixTime.Get();
            CreateEqualsCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, UnixTime.GetDateTimeUtcFromMs(unixTime + 1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateEqualsCondition(AttributeType.Number, "1.00");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.0001)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateEqualsCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.GT,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}