namespace Baumeister.Examples.ExampleAggregate
{
    public record Address
    {
        public Street Street { get; }

        public City City { get; }

        public ZipCode ZipCode { get; }

        public Address(Street street, City city, ZipCode zipCode)
        {
            Street = street;
            City = city;
            ZipCode = zipCode;
        }
    }
}
