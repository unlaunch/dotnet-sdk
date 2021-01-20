using System;
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
        public void DateTime_userValue_is_one_second_ahead()
        {
            var unixTime = UnixTime.Get();
            CreateGreaterThanCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, UnixTime.GetUtcDateTime(unixTime + 1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void DateTime_userValue_is_the_same()
        {
            var unixTime = UnixTime.Get();
            CreateGreaterThanCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, UnixTime.GetUtcDateTime(unixTime))
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void DateTime_userValue_is_less()
        {
            var unixTime = UnixTime.Get();
            CreateGreaterThanCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, UnixTime.GetUtcDateTime(unixTime - 1000))
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_greater_unixTime()
        {
            var unixTime = UnixTime.Get();
            CreateGreaterThanCondition(AttributeType.Date, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, UnixTime.GetUtcDateTime(unixTime).AddDays(1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_greater_dateTime()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateGreaterThanCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date.AddDays(1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_less()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateGreaterThanCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date.AddDays(-1))
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_one_hour_less_which_is_previous_day()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateGreaterThanCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date.AddHours(1)) // one hour more but still in same day
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateGreaterThanCondition(AttributeType.Number, "1.00");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.0001)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateGreaterThanCondition(AttributeType type, string userValue)
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