namespace Baumeister.Examples.PersonAggregate
{
    public readonly struct Age
    {
        public int Value { get; }
        
        public Age(int value) => Value = value;
    }
}
