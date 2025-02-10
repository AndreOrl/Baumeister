using Baumeister.Abstractions.Building;

namespace Baumeister.Examples.ExampleAggregate
{
    public class AddressBuilder : BuilderBase<AddressBuilder, Address>
    {
        private Street street;
        private City city;
        private ZipCode zipCode;

        public AddressBuilder WithStreet(Street street)
        {
            this.street = street;
            return this;
        }

        public AddressBuilder WithCity(City city)
        {
            this.city = city;
            return this;
        }

        public AddressBuilder WithZipCode(ZipCode zipCode)
        {
            this.zipCode = zipCode;
            return this;
        }

        public override Address Build()
        {
            return new Address(street, city, zipCode);
        }
    }
}
