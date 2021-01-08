using System.Collections.Generic;

namespace io.unlaunch.engine
{
    class UnlaunchSetValue : IUnlaunchValue
    {
        private readonly ISet<string> _value;

        public UnlaunchSetValue(ISet<string> value)
        {
            _value = value;
        }

        public object Get()
        {
            return _value;
        }
    }
}
