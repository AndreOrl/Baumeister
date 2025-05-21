namespace Baumeister.Abstractions.Building
{
    public class ValueMapping
    {
        public string Name { get; }
               
        public object Value { get; }

        public ValueMapping(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
