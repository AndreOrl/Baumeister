using System;

namespace Baumeister.Examples.ExampleAggregate
{
    public partial class AddressBuilder
    {
        public static AddressBuilder New()
        {
            return new AddressBuilder();
        }

        public AddressBuilder WithEqualityContract(System.Type equalitycontract)
        {
            this.With(equalitycontract);
            return this;
        }

        public AddressBuilder WithStreet(Baumeister.Examples.ExampleAggregate.Street street)
        {
            this.With(street);
            return this;
        }

        public AddressBuilder WithCity(Baumeister.Examples.ExampleAggregate.City city)
        {
            this.With(city);
            return this;
        }

        public AddressBuilder WithZipCode(Baumeister.Examples.ExampleAggregate.ZipCode zipcode)
        {
            this.With(zipcode);
            return this;
        }
    }
}
