#nullable enable
using System;

namespace Baumeister.Examples.AddressAggregate
{
    public partial class AddressBuilder
    {
        public static AddressBuilder New()
        {
            return new AddressBuilder();
        }

        public AddressBuilder WithEqualityContract(System.Type equalitycontract)
        {
            this.With("EqualityContract", equalitycontract);
            return this;
        }

        public AddressBuilder WithStreet(Baumeister.Examples.AddressAggregate.Street street)
        {
            this.With("Street", street);
            return this;
        }

        public AddressBuilder WithCity(Baumeister.Examples.AddressAggregate.City city)
        {
            this.With("City", city);
            return this;
        }

        public AddressBuilder WithZipCode(Baumeister.Examples.AddressAggregate.ZipCode zipcode)
        {
            this.With("ZipCode", zipcode);
            return this;
        }
    }
}
#nullable restore
