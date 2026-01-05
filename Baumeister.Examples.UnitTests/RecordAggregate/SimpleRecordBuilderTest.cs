using Baumeister.Examples.RecordAggregate;
using NUnit.Framework;

namespace Baumeister.Examples.UnitTests.RecordAggregate
{
    public class SimpleRecordBuilderTest
    {
        [Test]
        public void Build_WithProperties_ShouldCreateRecord()
        {
            var record = SimpleRecordBuilder.New()
                .WithName("Test")
                .WithAge(123)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(record.Name, Is.EqualTo("Test"));
                Assert.That(record.Age, Is.EqualTo(123));
            });
        }
    }
}
