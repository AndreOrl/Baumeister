namespace Baumeister.Examples.PersonAggregate
{
    public readonly struct Name
    {
        public string Value { get; }

        public Name(string value) => Value = value;
    }
}
