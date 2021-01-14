using System;
using System.Collections.Generic;

namespace io.unlaunch
{
    public interface IUnlaunchClient : IDisposable
    {
        bool IsReady();
        void AwaitUntilReady(int millisecondsTimeout);
        void AwaitUntilReady(TimeSpan timeout);
        void Shutdown();
        AccountDetails AccountDetails();
        UnlaunchFeature GetFeature(string flagKey, string identity);
        UnlaunchFeature GetFeature(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes);
        string GetVariation(string flagKey, string identity);
        string GetVariation(string flagKey, string identity, IEnumerable<UnlaunchAttribute> attributes);
    }
}