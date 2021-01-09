using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using io.unlaunch.utils;

namespace io.unlaunch.engine
{
    static class OperatorExtension
    {
        public static bool Apply(this Operator op, string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (userValue == null || value == null)
            {
                return false;
            }
            
            switch (op)
            {
                case Operator.EQ:
                    return Equals(value, userValue, type);
                case Operator.NEQ:
                    return !Equals(value, userValue, type);
                case Operator.GT:
                    return GreaterThan(value, userValue, type);
                case Operator.GTE:
                    return GreaterThanOrEqual(value, userValue, type);
                case Operator.LT:
                    return !GreaterThanOrEqual(value, userValue, type);
                case Operator.LTE:
                    return !GreaterThan(value, userValue, type);
                case Operator.SW:
                    return StartsWith(value, userValue, type);
                case Operator.EW:
                    return EndsWith(value, userValue, type);
                case Operator.CON:
                    return Contains(value, userValue, type);
                case Operator.NCON:
                    return !Contains(value, userValue, type);
                case Operator.NSW:
                    return !StartsWith(value, userValue, type);
                case Operator.NEW:
                    return !EndsWith(value, userValue, type);
                case Operator.PO:
                    return IsPartOf(value, userValue, type);
                case Operator.NPO:
                    return !IsPartOf(value, userValue, type);
                case Operator.HA:
                    return HasAnyOf(value, userValue, type);
                case Operator.NHA:
                    return !HasAnyOf(value, userValue, type);
                case Operator.AO:
                    return !NotHaveAllOf(value, userValue, type);
                case Operator.NAO:
                    return NotHaveAllOf(value, userValue, type);
                default:
                    return false;
            }
        }

        private static bool Equals(string value, IUnlaunchValue userValue, AttributeType type)
        {
            switch (type)
            {
                // Equals operator is not for DateTime from backend
                case AttributeType.DateTime: 
                {
                    var userDateTime = (DateTime) userValue.Get();
                    return userDateTime == UnixTime.GetDateTimeUtcFromMs(long.Parse(value));
                }
                case AttributeType.Set:
                {
                    var userSet = (ISet<string>) userValue.Get();
                    return userSet.SetEquals(value.Split(','));
                }
                case AttributeType.Number:
                {
                    var userNumber = (double) userValue.Get();
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    return userNumber == double.Parse(value);
                }
                case AttributeType.Boolean:
                {
                    var userBoolean = (bool) userValue.Get();
                    return userBoolean == (value.ToLower() == "true");
                }
                default:
                    return (string) userValue.Get() == value;
            }
        }

        private static bool GreaterThan(string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (type == AttributeType.DateTime)
            {
                var userDateTime = (DateTime) userValue.Get();
                return userDateTime > UnixTime.GetDateTimeUtcFromMs(long.Parse(value));
            }

            if (type != AttributeType.Number)
            {
                return false;
            }

            var userNumber = (double) userValue.Get();
            return userNumber > double.Parse(value);
        }

        private static bool GreaterThanOrEqual(string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (type == AttributeType.DateTime)
            {
                var userDateTime = (DateTime) userValue.Get();
                return userDateTime >= UnixTime.GetDateTimeUtcFromMs(long.Parse(value));
            }

            if (type != AttributeType.Number)
            {
                return false;
            }

            var userNumber = (double)userValue.Get();
            return userNumber >= double.Parse(value);
        }

        private static bool StartsWith(string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (type != AttributeType.String)
            {
                return false;
            }

            var userString = (string) userValue.Get();
            return userString.StartsWith(value);
        }

        private static bool EndsWith(string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (type != AttributeType.String)
            {
                return false;
            }

            var userString = (string)userValue.Get();
            return userString.EndsWith(value);
        }

        private static bool Contains(string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (type != AttributeType.String)
            {
                return false;
            }

            var userString = (string)userValue.Get();
            return userString.Contains(value);
        }

        private static bool IsPartOf(string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (userValue.GetType() != typeof(UnlaunchSetValue) || type != AttributeType.Set) {
                return false;
            }

            var userSet = (ISet<string>) userValue.Get();
            var intersection = value.Split(',').Distinct().Where(x => userSet.Contains(x));
            return userSet.SetEquals(intersection);
        }

        private static bool HasAnyOf(string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (userValue.GetType() != typeof(UnlaunchSetValue) || type != AttributeType.Set)
            {
                return false;
            }

            var userSet = (ISet<string>)userValue.Get();
            return value.Split(',').Distinct().Any(x => userSet.Contains(x));
        }

        private static bool NotHaveAllOf(string value, IUnlaunchValue userValue, AttributeType type)
        {
            if (userValue.GetType() != typeof(UnlaunchSetValue) || type != AttributeType.Set)
            {
                return false;
            }

            var userSet = (ISet<string>)userValue.Get();
            return value.Split(',').Distinct().Any(x => !userSet.Contains(x));
        }
    }
}
