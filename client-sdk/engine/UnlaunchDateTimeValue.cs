using System;

namespace io.unlaunch.engine
{
    class UnlaunchDateTimeValue : IUnlaunchValue
    {
        public readonly DateTime _value;

        public UnlaunchDateTimeValue(DateTime value)
        {
            _value = value;
        }

        public object Get()
        {
            return _value;
        }
    }
}
