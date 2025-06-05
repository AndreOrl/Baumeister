namespace Baumeister.Examples.AddressAggregate
{
    public record Address
    {
        public Country Country { get; }

        public Street Street { get; }

        public City City { get; }

        public ZipCode ZipCode { get; }

        public Address(Country country, Street street, City city, ZipCode zipCode)
        {
            Country = country;
            Street = street;
            City = city;
            ZipCode = zipCode;
        }
    }
}
