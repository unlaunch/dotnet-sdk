using System;

namespace io.unlaunch
{
    public class LoggerProvider
    {
        public static IUnlaunchLogger For<T>()
        {
            return GetLogger(typeof(T));
        }

        static IUnlaunchLogger GetLogger(Type type)
        {
#if NETSTANDARD
            return new MsLogger(type);
#else
            return new CommonLogger(type);
#endif
        }
    }
}
