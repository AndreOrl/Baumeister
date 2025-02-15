using System;

namespace Baumeister.Examples.ExampleAggregate
{
    public static class AddressBuilderExtensions
    {

        public static AddressBuilder WithEqualityContract(this AddressBuilder builder, System.Type equalitycontract)
        {
            builder.With(equalitycontract);
            return builder;
        }

        public static AddressBuilder WithStreet(this AddressBuilder builder, Baumeister.Examples.ExampleAggregate.Street street)
        {
            builder.With(street);
            return builder;
        }

        public static AddressBuilder WithCity(this AddressBuilder builder, Baumeister.Examples.ExampleAggregate.City city)
        {
            builder.With(city);
            return builder;
        }

        public static AddressBuilder WithZipCode(this AddressBuilder builder, Baumeister.Examples.ExampleAggregate.ZipCode zipcode)
        {
            builder.With(zipcode);
            return builder;
        }
    }
}
