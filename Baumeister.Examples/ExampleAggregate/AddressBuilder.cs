using Baumeister.Abstractions.Building;

namespace Baumeister.Examples.ExampleAggregate
{
    [Builder(typeof(Address))]
    public class AddressBuilder : BuilderBase<AddressBuilder, Address>
    {
    }
}
