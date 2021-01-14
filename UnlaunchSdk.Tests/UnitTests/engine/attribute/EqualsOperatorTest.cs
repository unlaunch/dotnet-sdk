using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using io.unlaunch.utils;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class EqualsOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void Boolean()
        {
            CreateEqualsCondition(AttributeType.Boolean, "True");

            var attributes = new[]
            {
                UnlaunchAttribute.NewBoolean(AttributeKey, true)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void DateTime()
        {
            var unixTime = UnixTime.Get();
            CreateEqualsCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, UnixTime.GetUtcDateTime(unixTime))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date()
        {
            var unixTime = UnixTime.Get();
            CreateEqualsCondition(AttributeType.Date, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, UnixTime.GetUtcDateTime(unixTime))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateEqualsCondition(AttributeType.Number, "1.0");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.000)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void String()
        {
            var userValue = "dotnet-sdk";
            CreateEqualsCondition(AttributeType.String, userValue);

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, userValue)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set()
        {
            CreateEqualsCondition(AttributeType.Set, "value1,value2");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey,  new[] {"value2", "value1"})
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
                op = Operator.EQ,
                value = userValue
            }};
            
            LoadFeatureFlags();
        }
    }
}