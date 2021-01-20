using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using io.unlaunch.utils;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class NotEqualsOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void Boolean()
        {
            CreateNotEqualsCondition(AttributeType.Boolean, "false");

            var attributes = new[]
            {
                UnlaunchAttribute.NewBoolean(AttributeKey, true)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void DateTime_userValue_is_one_millisecond_behind()
        {
            var unixTime = UnixTime.Get();
            CreateNotEqualsCondition(AttributeType.DateTime, unixTime.ToString());

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
            CreateNotEqualsCondition(AttributeType.Date, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, UnixTime.GetUtcDateTime(unixTime).AddDays(-1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateNotEqualsCondition(AttributeType.Number, "1.0001");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.000)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void StringEquals()
        {
            var userValue = "dotnet-sdk";
            CreateNotEqualsCondition(AttributeType.String, userValue);

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, userValue)
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void StringNotEquals()
        {
            var userValue = "dotnet-sdk";
            CreateNotEqualsCondition(AttributeType.String, userValue);

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, "hi " + userValue)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set_userSet_is_not_same()
        {
            CreateNotEqualsCondition(AttributeType.Set, "value1,value3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey,  new[] {"value2", "value1"})
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set_userSet_is_super_set()
        {
            CreateNotEqualsCondition(AttributeType.Set, "value1,value3,value2");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey,  new[] {"value2", "value1"})
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Set_userSet_is_sub_set()
        {
            CreateNotEqualsCondition(AttributeType.Set, "value1");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey,  new[] {"value2", "value1"})
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateNotEqualsCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.NEQ,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}