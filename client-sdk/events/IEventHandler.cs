using System;

namespace io.unlaunch.events
{
    public interface IEventHandler : IDisposable
    {
        bool Handle(UnlaunchEvent unlaunchEvent);
        void Flush();
    }
}