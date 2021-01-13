namespace io.unlaunch
{
    public interface IUnlaunchDynamicConfig
    {
        bool ContainsKey(string key);

        bool GetBoolean(string key);

        bool GetBoolean(string key, bool defaultValue);


        double GetDouble(string key);

        double GetDouble(string key, double defaultValue);

        float GetFloat(string key);

        float GetFloat(string key, float defaultValue);

        int GetInt(string key);

        int GetInt(string key, int defaultValue);

        long GetLong(string key);

        long GetLong(string key, long defaultValue);

        string GetString(string key);

        string GetString(string key, string defaultValue);

        bool IsEmpty();

        bool IsAny();

        int Size();

        int Count();
    }
}
