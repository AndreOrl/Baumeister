using Baumeister.Examples.AddressAggregate;

namespace Baumeister.Examples.UnitTests.AddressAggregate
{
    public class PersonBuilderTest
    {
        [Test]
        public void Ctor_WithDefaultValues_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New().Build();

            Assert.That(address.Country.Value, Is.EqualTo("Germany"));
        }

        [Test]
        public void ExplicitWithMethods_WithAllPropertiesSet_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .WithCountry(new Country("France"))
                .WithStreet(new Street("Main Street"))
                .WithCity(new City("Springfield"))
                .WithZipCode(new ZipCode("12345"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(address.Country.Value, Is.EqualTo("France"));
                Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
                Assert.That(address.City.Value, Is.EqualTo("Springfield"));
                Assert.That(address.ZipCode.Value, Is.EqualTo("12345"));
            });
        }

        [Test]
        public void GenericWithMethods_WithAllPropertiesSet_ShouldBuildCorrectly()
        {
            var address = AddressBuilder.New()
                .With("Country", new Country("USA"))
                .With("City", new City("Springfield"))
                .With("Street", new Street("Main Street"))
                .With("ZipCode", new ZipCode("12345"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(address.Country.Value, Is.EqualTo("USA"));
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
                .With("Street", new Street("Main Street"))
                .With("City", new City("Springfield"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(address.Street.Value, Is.EqualTo("Main Street"));
                Assert.That(address.City.Value, Is.EqualTo("Springfield"));
            });
        }
    }
}
