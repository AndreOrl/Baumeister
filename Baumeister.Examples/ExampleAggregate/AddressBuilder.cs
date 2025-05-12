using Baumeister.Abstractions.Building;

namespace Baumeister.Examples.ExampleAggregate
{
    [Builder(typeof(Address))]
    public partial class AddressBuilder : BuilderBase<Address>
    {
    }
}
