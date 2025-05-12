using System;

namespace Baumeister.Examples.PersonAggregate
{
    public partial class PersonBuilder
    {
        public static PersonBuilder New()
        {
            return new PersonBuilder();
        }

        public PersonBuilder WithEqualityContract(System.Type equalitycontract)
        {
            this.With("EqualityContract", equalitycontract);
            return this;
        }

        public PersonBuilder WithName(Baumeister.Examples.PersonAggregate.Name name)
        {
            this.With("Name", name);
            return this;
        }

        public PersonBuilder WithAge(Baumeister.Examples.PersonAggregate.Age age)
        {
            this.With("Age", age);
            return this;
        }
    }
}
