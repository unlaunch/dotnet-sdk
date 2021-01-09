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
            CreateEqualsCondition(AttributeType.Boolean, "false");

            var attributes = new[]
            {
                UnlaunchAttribute.NewBoolean(AttributeKey, true)
            };

            OnVariation(attributes);
        }

        [Fact]
        public void DateTime()
        {
            var unixTime = UnixTime.Get();
            CreateEqualsCondition(AttributeType.DateTime, unixTime.ToString());

            var attributes = new[]
            {
                UnlaunchAttribute.NewDateTime(AttributeKey, UnixTime.GetDateTimeUtcFromMs(unixTime - 1))
            };

            OnVariation(attributes);
        }

        [Fact]
        public void Number()
        {
            CreateEqualsCondition(AttributeType.Number, "1.0001");

            var attributes = new[]
            {
                UnlaunchAttribute.NewNumber(AttributeKey, 1.000)
            };

            OnVariation(attributes);
        }

        [Fact]
        public void String()
        {
            var userValue = "dotnet-sdk";
            CreateEqualsCondition(AttributeType.String, userValue);

            var attributes = new[]
            {
                UnlaunchAttribute.NewString(AttributeKey, "hi " + userValue)
            };

            OnVariation(attributes);
        }

        [Fact]
        public void Set()
        {
            CreateEqualsCondition(AttributeType.Set, "value1,value3");

            var attributes = new[]
            {
                UnlaunchAttribute.NewSet(AttributeKey,  new[] {"value2", "value1"})
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
                op = Operator.NEQ,
                value = userValue
            }};

            LoadFeatureFlags();
        }
    }
}