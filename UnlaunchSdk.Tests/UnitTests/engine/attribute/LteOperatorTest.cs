using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using io.unlaunch.utils;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class LteOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void DateTime()
        {
            var unixTime = UnixTime.Get();
            CreateLteCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, unixTime)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date()
        {
            var unixTime = UnixTime.Get();
            CreateLteCondition(AttributeType.Date, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, UnixTime.GetUtcDateTime(unixTime))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateLteCondition(AttributeType.Number, "1.00");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.000)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateLteCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.LTE,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}