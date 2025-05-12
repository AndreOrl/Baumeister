namespace Baumeister.Examples.PersonAggregate
{
    public sealed record Person
    {
        public Name Name { get; init; }

        public Age Age { get; set; }
        
        public Person(Name name)
        {
            Name = name;
        }
    }
}
