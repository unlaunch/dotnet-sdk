using System;
using System.Collections.Generic;
using System.Linq;
using io.unlaunch;
using Xunit;

namespace UnlaunchSdk.Tests.UnitTests.engine.attribute
{
    public class InvalidUserAttributeValueTest
    {
        [Fact]
        public void UserSet_is_null()
        {
            Assert.Throws<ArgumentException>(() => UnlaunchAttribute.NewSet("attributeKey", null));
        }

        [Fact]
        public void Set_userSet_is_empty()
        {
            Assert.Throws<ArgumentException>(() => UnlaunchAttribute.NewSet("attributeKey", new HashSet<string>()));

            Assert.Throws<ArgumentException>(() => UnlaunchAttribute.NewSet("attributeKey", Enumerable.Empty<string>()));
        }

        [Fact]
        public void UserString_is_null()
        {
            Assert.Throws<ArgumentException>(() => UnlaunchAttribute.NewString("attributeKey", null));
        }
    }
}
