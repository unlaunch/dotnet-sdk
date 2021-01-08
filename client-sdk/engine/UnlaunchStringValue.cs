namespace io.unlaunch.engine
{
    class UnlaunchStringValue : IUnlaunchValue
    {
        private readonly string _value;

        public UnlaunchStringValue(string value)
        {
            _value = value;
        }

        public object Get()
        {
            return _value;
        }
    }
}