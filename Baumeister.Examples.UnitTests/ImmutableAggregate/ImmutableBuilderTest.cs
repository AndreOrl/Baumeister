using Baumeister.Examples.ImmutableAggregate;

namespace Baumeister.Examples.UnitTests.ImmutableAggregate
{
    public class ImmutableBuilderTest
    {
        [Test]
        public void ExplicitWithMethods_WithAllParametersSet_ShouldBuildCorrectly()
        {
            var result = ImmutableBuilder.New()
                .WithPrivateField("PrivateField")
                .WithPrivateFieldUnderscore("PrivateFieldUnderscore")
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(result.ToString(), Does.Contain("privateField=PrivateField"));
                Assert.That(result.ToString(), Does.Contain("_privateFieldUnderscore=PrivateFieldUnderscore"));
            });
        }
    }
}
