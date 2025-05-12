using Baumeister.Examples.PersonAggregate;

namespace Baumeister.Examples.UnitTests.PersonAggregate
{
    public class PersonBuilderTest
    {
        [Test]
        public void ExplicitWithMethods_WithCtorParametersSet_ShouldBuildCorrectly()
        {
            var person = PersonBuilder.New()
                .WithName(new Name("John Doe"))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(person.Name.Value, Is.EqualTo("John Doe"));
            });
        }

        [Test]
        public void ExplicitWithMethods_WithSettablePropertyParametersSet_ShouldBuildCorrectly()
        {
            var person = PersonBuilder.New()
                .WithName(new Name("John Doe"))
                .WithAge(new Age(30))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(person.Name.Value, Is.EqualTo("John Doe"));
                Assert.That(person.Age.Value, Is.EqualTo(30));
            });
        }
    }
}
