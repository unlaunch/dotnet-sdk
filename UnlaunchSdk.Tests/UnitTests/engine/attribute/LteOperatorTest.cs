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
            CreateEqualsCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, unixTime)
            };

            OnVariation(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateEqualsCondition(AttributeType.Number, "1.00");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.000)
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
                op = Operator.LTE,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}