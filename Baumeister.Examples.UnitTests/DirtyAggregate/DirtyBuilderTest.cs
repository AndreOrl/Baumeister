using Baumeister.Examples.DirtyAggregate;

namespace Baumeister.Examples.UnitTests.DirtyAggregate
{
    public class DirtyBuilderTest
    {
        [Test]
        public void ExplicitWithMethods_WithAllParametersSet_ShouldBuildCorrectly()
        {
            var dirty = DirtyBuilder.New()
                .WithCtor1("Ctor1")
                .WithCtor2("Ctor2")
                .WithProperty1("Property1")
                .WithProperty2("Property2")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(dirty.Ctor1, Is.EqualTo("Ctor1"));
                Assert.That(dirty.Ctor2, Is.EqualTo("Ctor2"));
                Assert.That(dirty.Property1, Is.EqualTo("Property1"));
                Assert.That(dirty.Property2, Is.EqualTo("Property2"));
            });
        }

        [Test]
        public void ExplicitWithMethods_WithSomeParametersSet_ShouldBuildCorrectly()
        {
            var dirty = DirtyBuilder.New()
                .WithCtor1("Ctor1")
                .WithProperty1("Property1")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(dirty.Ctor1, Is.EqualTo("Ctor1"));
                Assert.That(dirty.Property1, Is.EqualTo("Property1"));
                Assert.That(dirty.Ctor2, Is.EqualTo(default(string)));
                Assert.That(dirty.Property2, Is.EqualTo(default(string)));
            });
        }
    }
}
