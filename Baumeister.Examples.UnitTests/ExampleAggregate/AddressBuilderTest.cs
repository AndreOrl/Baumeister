using Baumeister.Examples.ExampleAggregate;

namespace Baumeister.Examples.UnitTests.ExampleAggregate
{
    public class AddressBuilderTest
    {
        [Test]
        public void ExplicitMethods_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .WithStreet(new Street("Main Street"))
                .WithCity(new City("Springfield"))
                .WithZipCode(new ZipCode("12345"))
                .Build();

            Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
            Assert.That(address.City.Value, Is.EqualTo("Springfield"));
            Assert.That(address.ZipCode.Value, Is.EqualTo("12345"));
        }

        [Test]
        public void GenericMethods_WithAllPropertiesSet_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .With(new City("Springfield"))
                .With(new Street("Main Street"))
                .With(new ZipCode("12345"))
                .Build();

            Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
            Assert.That(address.City.Value, Is.EqualTo("Springfield"));
            Assert.That(address.ZipCode.Value, Is.EqualTo("12345"));
        }

        [Test]
        public void GenericMethods_WithMissingProperties_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .With(new Street("Main Street"))
                .With(new City("Springfield"))
                .Build();

            Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
            Assert.That(address.City.Value, Is.EqualTo("Springfield"));
        }
    }
}
