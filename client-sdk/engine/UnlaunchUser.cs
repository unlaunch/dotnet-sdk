using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace io.unlaunch.engine
{
    public class UnlaunchUser
    {
        private ISet<Type> ValidTypes = new HashSet<Type>(new[]
        {
            typeof(string), typeof(bool), typeof(double), typeof(DateTime)
        });
        
        private string _id;
        private bool _anonymous;
        private readonly IDictionary<string, IUnlaunchValue> _attributes = new ConcurrentDictionary<string, IUnlaunchValue>();

        UnlaunchUser(string id) : this(id, false)
        {
            
        }

        UnlaunchUser(string id, bool anon)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The argument `id` must be a valid string.");
            }

            _id = id;
            _anonymous = anon;
        }

        UnlaunchUser(string id, bool anon, IDictionary<string, object> attributes)
        {

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The argument `id` must be a valid string.");
            }

            if (attributes == null)
            {
                throw new ArgumentException("The argument `id` must not be null");
            }

            _id = id;
            _anonymous = anon;
            ResetAndSetAttributesMap(attributes);
        }

        public static UnlaunchUser Create(string id)
        {
            return new UnlaunchUser(id);
        }

        public static UnlaunchUser CreateWithAttributes(string id, IEnumerable<UnlaunchAttribute> attributes)
        {
            var map = attributes?.ToDictionary(attribute => attribute.GetKey(), attribute => attribute.GetValue());

            return new UnlaunchUser(id, false, (map ?? new Dictionary<string, object>()));
        }

        public IDictionary<string, IUnlaunchValue> GetAttributes()
        {
            return _attributes;
        }

        public string GetId()
        {
            return _id;
        }

        public bool IsAnonymous()
        {
            return _anonymous;
        }

        public void ClearAllAttributes()
        {
            _attributes.Clear();
        }

        private void ResetAndSetAttributesMap(IDictionary<string, object> map)
        {
            ClearAllAttributes();
            foreach (var pair in map)
            {
                _attributes.Add(pair.Key, GetUnlaunchValue(pair.Value));
            }
        }

        private IUnlaunchValue GetUnlaunchValue(object value)
        {
            var valueType = value.GetType();
            if (!ValidTypes.Contains(valueType) && !typeof(ISet<string>).IsAssignableFrom(valueType))
            {
                throw new ArgumentException($"value `{value}` must be of type string, number, boolean, or set");
            }

            if (valueType == typeof(string))
            {
                return new UnlaunchStringValue((string)value);
            } 
            if (valueType == typeof(bool))
            {
                return new UnlaunchBooleanValue((bool)value);
            }
            if (valueType == typeof(double))
            {
                return new UnlaunchNumberValue((double) value);
            }
            if (valueType == typeof(DateTime))
            {
                return new UnlaunchDateTimeValue((DateTime) value);
            }
            if (typeof(ISet<string>).IsAssignableFrom(valueType))
            {
                return new UnlaunchSetValue((ISet<string>) value);
            }

            throw new ArgumentException($"value `{value}` must be of type string, number, boolean, or set");
        }
    }
}