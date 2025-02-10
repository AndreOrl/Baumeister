using Baumeister.Abstractions.Building;

namespace Baumeister.Examples.ExampleAggregate
{
    public class AddressBuilder : BuilderBase<AddressBuilder, Address>
    {
        public AddressBuilder WithStreet(Street street)
        {
            With(street);
            return this;
        }

        public AddressBuilder WithCity(City city)
        {
            With(city);
            return this;
        }

        public AddressBuilder WithZipCode(ZipCode zipCode)
        {
            With(zipCode);
            return this;
        }
    }
}
