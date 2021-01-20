using System;
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
        public void DateTime_userValue_is_same_unixTime()
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
        public void DateTime_userValue_is_same_dateTime()
        {
            var date = DateTime.UtcNow;
            CreateEqualsCondition(AttributeType.DateTime, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, date)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void DateTime_userValue_is_one_millisecond_ahead()
        {
            var date = DateTime.UtcNow;
            CreateEqualsCondition(AttributeType.DateTime, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, date.AddMilliseconds(1))
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_same_unixTime()
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
        public void Date_userValue_is_the_same_dateTime()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateEqualsCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_one_hour_more_still_in_same_day()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateEqualsCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date.AddHours(1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_one_hour_less_which_is_previous_day()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateEqualsCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date.AddHours(-1)) // it is different day
            };

            OffVariationTargetingRulesNotMatch(attributes);
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
        public void StringEquals()
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
        public void StringNotEquals()
        {
            var userValue = "dotnet-sdk";
            CreateEqualsCondition(AttributeType.String, userValue.Substring(1));

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, userValue)
            };

            OffVariationTargetingRulesNotMatch(attributes);
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