using System;
using System.Linq;
using io.unlaunch;
using io.unlaunch.engine;
using io.unlaunch.model;
using io.unlaunch.utils;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class GteOperatorTest : UnlaunchContext
    {
        private const string AttributeKey = "attributeKey";

        [Fact]
        public void DateTime_userValue_is_the_same_unixTime()
        {
            var unixTime = UnixTime.Get();
            CreateGteCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, UnixTime.GetUtcDateTime(unixTime))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void DateTime_userValue_is_the_same_dateTime()
        {
            var date = DateTime.UtcNow;
            CreateGteCondition(AttributeType.DateTime, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, date)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void DateTime_userValue_is_greater()
        {
            var date = DateTime.UtcNow;
            CreateGteCondition(AttributeType.DateTime, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, date.AddSeconds(1))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void DateTime_userValue_is_less()
        {
            var date = DateTime.UtcNow;
            CreateGteCondition(AttributeType.DateTime, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, date.AddMilliseconds(-1))
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact] public void Date_userValue_is_the_same_unixTime()
        {
            var unixTime = UnixTime.Get();
            CreateGteCondition(AttributeType.Date, unixTime.ToString());

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
            CreateGteCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_greater()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateGteCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date.AddDays(2))
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        [Fact]
        public void Date_userValue_is_smaller()
        {
            var date = DateTime.SpecifyKind(new DateTime(2019, 9, 26), DateTimeKind.Utc);
            CreateGteCondition(AttributeType.Date, UnixTime.Get(date).ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDate(AttributeKey, date.AddDays(-2))
            };

            OffVariationTargetingRulesNotMatch(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateGteCondition(AttributeType.Number, "1.00");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.000)
            };

            OnVariationTargetingRulesMatch(attributes);
        }

        private void CreateGteCondition(AttributeType type, string userValue)
        {
            var flag = FlagResponse.data.flags.First();
            flag.rules.First().conditions = new [] { new TargetRuleConditionDto
            {
                id = 119,
                attribute = AttributeKey,
                type = type,
                op = Operator.GTE,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}