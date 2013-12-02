using FakeItEasy;
using mijay.Utils;
using NClone.ObjectReplication;
using NClone.ReplicationStrategies;
using NUnit.Framework;

namespace NClone.Tests.ReplicationStrategies
{
    public class CopyArrayReplicationStrategyTest: TestBase
    {
        private readonly IReplicationContext dummyContext = A.Fake<IReplicationContext>(c => c.Strict());

        [Test]
        public void SourceArrayContainsElements_NewArrayWithSameElementsReturned()
        {
            var source = new[] { new SomeClass(), new SomeClass() };

            var result = CopyArrayReplicationStrategy.Instance.Replicate(source, dummyContext).As<SomeClass[]>();

            Assert.That(result, Is.Not.SameAs(source));
            Assert.That(result, Is.EqualTo(source));
        }

        [Test]
        public void SourceArrayIsEmpty_NewEmptyArrayReturned()
        {
            var source = new SomeClass[0];

            var result = CopyArrayReplicationStrategy.Instance.Replicate(source, dummyContext).As<SomeClass[]>();

            Assert.That(result, Is.Not.SameAs(source));
            Assert.That(result, Is.Empty);
        }

        private class SomeClass
        {
        }
    }
}