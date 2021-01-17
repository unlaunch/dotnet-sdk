using System;
using System.Collections.Generic;
using System.Linq;
using io.unlaunch.utils;

namespace io.unlaunch
{
    public class UnlaunchAttribute
    {

        private readonly string _key;
        private readonly object _value;

        UnlaunchAttribute(string key, object value)
        {
            _key = key;
            _value = value;
        }

        private static UnlaunchAttribute Create(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key argument must not be null or empty");
            }

            return new UnlaunchAttribute(key, value);

        }
        public static UnlaunchAttribute NewString(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value argument must not be null or empty");
            }
            return Create(key, value);
        }

        public static UnlaunchAttribute NewSet(string key, ISet<string> value)
        {
            if (value == null || !value.Any())
            {
                throw new ArgumentException("set must not be null or empty");
            }

            return Create(key, value);
        }

        public static UnlaunchAttribute NewSet(string key, IEnumerable<string> value)
        {
            if (value == null || !value.Any())
            {
                throw new ArgumentException("list must not be null or empty");
            }

            return Create(key, new HashSet<string>(value));
        }

        public static UnlaunchAttribute NewNumber(string key, double value)
        {
            return Create(key, value);
        }

        public static UnlaunchAttribute NewBoolean(string key, bool value)
        {
            return Create(key, value);
        }

        public static UnlaunchAttribute NewDateTime(string key, DateTime utcTime)
        {
            return Create(key, utcTime);
        }

        public static UnlaunchAttribute NewDateTime(string key, long millisecondsSinceEpoch)
        {
            return Create(key, UnixTime.GetUtcDateTime(millisecondsSinceEpoch));
        }

        public static UnlaunchAttribute NewDate(string key, DateTime utcDate)
        {
            return Create(key, utcDate.Date);
        }

        public static UnlaunchAttribute NewDate(string key, long millisecondsSinceEpoch)
        {
            return Create(key, UnixTime.GetUtcDateTime(millisecondsSinceEpoch).Date);
        }

        public string GetKey()
        {
            return _key;
        }

        public object GetValue()
        {
            return _value;
        }
    }
}
