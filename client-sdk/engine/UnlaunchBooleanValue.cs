namespace io.unlaunch.engine
{
    public class UnlaunchBooleanValue : IUnlaunchValue
    {
        private readonly bool _value;

        public UnlaunchBooleanValue(bool value)
        {
            _value = value;
        }

        public object Get()
        {
            return _value;
        }
    }
}