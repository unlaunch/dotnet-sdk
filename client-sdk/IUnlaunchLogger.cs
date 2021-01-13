using System;

namespace io.unlaunch
{
    public interface IUnlaunchLogger
    {
        void Debug(string message);
        void Debug(string message, Exception e);
        void Info(string message);
        void Info(string message, Exception e);
        void Trace(string message);
        void Trace(string message, Exception e);
        void Warn(string message);
        void Warn(string message, Exception e);
        void Error(string message);
        void Error(string message, Exception e);
    }
}
