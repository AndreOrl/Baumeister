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
            this.With(equalitycontract);
            return this;
        }

        public PersonBuilder WithName(Baumeister.Examples.PersonAggregate.Name name)
        {
            this.With(name);
            return this;
        }

        public PersonBuilder WithAge(Baumeister.Examples.PersonAggregate.Age age)
        {
            this.With(age);
            return this;
        }
    }
}
