using System.Collections.Generic;

namespace io.unlaunch.engine
{
    public class Condition
    {
        private readonly string _attribute;
        private readonly Operator _operator;
        private readonly AttributeType _type;
        private readonly string _value;

        public Condition(string attribute, Operator op, AttributeType type, string value)
        {
            _attribute = attribute;
            _operator = op;
            _type = type;
            _value = value;
        }

        public bool Match(UnlaunchUser user)
        {
            return user.GetAttributes().ContainsKey(_attribute) && _operator.Apply(_value, user.GetAttributes()[_attribute], _type);
        }
    }
}
