using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using io.unlaunch.utils;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class LessThanOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void DateTime()
        {
            var unixTime = UnixTime.Get();
            CreateLessThanCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, UnixTime.GetUtcDateTime(unixTime - 1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date()
        {
            var unixTime = UnixTime.Get();
            CreateLessThanCondition(AttributeType.Date, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, UnixTime.GetUtcDateTime(unixTime).AddDays(-1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateLessThanCondition(AttributeType.Number, "1.001");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.000)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateLessThanCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.LT,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}