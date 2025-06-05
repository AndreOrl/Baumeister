using Baumeister.Abstractions.Building;

namespace Baumeister.Examples.AddressAggregate
{
    public partial class AddressBuilder : BuilderBase<Address>
    {
        // Demonstrates type-safe, attribute-free builder defaults!
        partial void OnInitializeDefaults()
        {
            WithCountry(new Country("Germany"));
        }
    }
}