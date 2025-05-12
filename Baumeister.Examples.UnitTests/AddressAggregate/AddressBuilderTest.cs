using Baumeister.Examples.AddressAggregate;

namespace Baumeister.Examples.UnitTests.AddressAggregate
{
    public class AddressBuilderTest
    {
        [Test]
        public void ExplicitWithMethods_WithAllPropertiesSet_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .WithStreet(new Street("Main Street"))
                .WithCity(new City("Springfield"))
                .WithZipCode(new ZipCode("12345"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
                Assert.That(address.City.Value, Is.EqualTo("Springfield"));
                Assert.That(address.ZipCode.Value, Is.EqualTo("12345"));
            });
        }

        [Test]
        public void GenericWithMethods_WithAllPropertiesSet_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .With(new City("Springfield"))
                .With(new Street("Main Street"))
                .With(new ZipCode("12345"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
                Assert.That(address.City.Value, Is.EqualTo("Springfield"));
                Assert.That(address.ZipCode.Value, Is.EqualTo("12345"));
            });
        }

        [Test]
        public void ExplicitWithMethods_WithMissingProperties_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .WithStreet(new Street("Main Street"))
                .WithCity(new City("Springfield"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
                Assert.That(address.City.Value, Is.EqualTo("Springfield"));
            });
        }

        [Test]
        public void GenericWithMethods_WithMissingProperties_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .With(new Street("Main Street"))
                .With(new City("Springfield"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
                Assert.That(address.City.Value, Is.EqualTo("Springfield"));
            });
        }
    }
}
