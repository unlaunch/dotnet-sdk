namespace io.unlaunch.engine
{
    class UnlaunchNumberValue : IUnlaunchValue
    {
        private readonly double _value;

        public UnlaunchNumberValue(double value)
        {
            _value = value;
        }


        public object Get()
        {
            return _value;
        }
    }
}
