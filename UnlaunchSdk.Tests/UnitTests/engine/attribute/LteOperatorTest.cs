using System;
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
        public void DateTime_userValue_is_same_unixTime()
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
        public void DateTime_userValue_is_greater_dateTime()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateLteCondition(AttributeType.DateTime, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, date.AddMilliseconds(1))
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void DateTime_userValue_is_greater_unixTime()
        {
            var unixTime = UnixTime.Get();
            CreateLteCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, unixTime + 1)
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_same_unixTime()
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
        public void Date_userValue_is_same_dateTime()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateLteCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_greater_dateTime()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateLteCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date.AddDays(1))
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_greater_unixTime()
        {
            var unixTime = UnixTime.Get();
            CreateLteCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, unixTime + 86400*1000)
            };

            OffVariationTargetingRulesNotMatch(attributes);
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