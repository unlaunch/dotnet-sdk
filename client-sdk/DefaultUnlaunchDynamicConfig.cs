using System.Collections.Generic;
using System.Linq;

namespace io.unlaunch
{
    class DefaultUnlaunchDynamicConfig : IUnlaunchDynamicConfig
    {
        private readonly IDictionary<string, string> _configMap;

        public DefaultUnlaunchDynamicConfig(IDictionary<string, string> configMap)
        {
            _configMap = configMap ?? new Dictionary<string, string>(1);
        }

        public bool ContainsKey(string key)
        {
            return _configMap.ContainsKey(key);
        }

        public bool GetBoolean(string key)
        {
            return _configMap[key].ToLower() == "true";
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            if (!_configMap.ContainsKey(key))
            {
                return defaultValue;
            }

            return _configMap[key].ToLower() == "true";
        }

        public double GetDouble(string key)
        {
            return double.Parse(_configMap[key]);
        }

        public double GetDouble(string key, double defaultValue)
        {
            try
            {
                return double.Parse(_configMap[key]);
            }
            catch
            {
                return defaultValue;
            }
        }

        public float GetFloat(string key)
        {
            return float.Parse(_configMap[key]);
        }

        public float GetFloat(string key, float defaultValue)
        {
            try
            {
                return float.Parse(_configMap[key]);
            }
            catch
            {
                return defaultValue;
            }
        }

        public int GetInt(string key)
        {
            return int.Parse(_configMap[key]);
        }

        public int GetInt(string key, int defaultValue)
        {
            try
            {
                return int.Parse(_configMap[key]);
            }
            catch
            {
                return defaultValue;
            }
        }

        public long GetLong(string key)
        {
            return long.Parse(_configMap[key]);
        }

        public long GetLong(string key, long defaultValue)
        {
            try
            {
                return long.Parse(_configMap[key]);
            }
            catch
            {
                return defaultValue;
            }
        }

        public string GetString(string key)
        {
            return _configMap[key];
        }

        public string GetString(string key, string defaultValue)
        {
            return _configMap.ContainsKey(key) ? _configMap[key] : defaultValue;
        }

        public bool IsEmpty()
        {
            return !_configMap.Any();
        }

        public bool IsAny()
        {
            return _configMap.Any();
        }

        public int Size()
        {
            return _configMap.Count;
        }

        public int Count()
        {
            return _configMap.Count;
        }
    }
}
