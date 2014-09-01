using System;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NClone.ObjectReplication;
using NClone.ReplicationStrategies;
using NUnit.Framework;

namespace NClone.Tests.ObjectReplication
{
    public class ReplicatorContextTest: TestBase
    {
        private ReplicationContext replicationContext;
        private IReplicationStrategy replicationStrategy;

        protected override void SetUp()
        {
            base.SetUp();
            replicationStrategy = A.Fake<IReplicationStrategy>();
            var simpleFactory = A.Fake<IReplicationStrategyFactory>(x => x.Strict());
            simpleFactory
                .CallsTo(x => x.StrategyForType(A<Type>.Ignored))
                .Returns(replicationStrategy);
            replicationContext = new ReplicationContext(simpleFactory);
        }

        [Test]
        public void SourceIsNull_NullReturned()
        {
            object result = replicationContext.Replicate(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SourceIsNotNull_Replicated()
        {
            var source = new Class();
            var sourceReplica = new Class();
            replicationStrategy
                .CallsTo(x => x.Replicate(source, A<IReplicationContext>.Ignored))
                .Returns(sourceReplica);

            object result = replicationContext.Replicate(source);

            Assert.That(result, Is.SameAs(sourceReplica));
        }

        [Test]
        public void ReplicateSameSourceTwice_FactoryIsCalledOnlyOnce()
        {
            var source = new Class();

            replicationContext.Replicate(source);
            replicationContext.Replicate(source);

            replicationStrategy
                .CallsTo(x => x.Replicate(null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void ReplicatesEqualButNotSameObjects_FactoryIsCalledForEach()
        {
            var source1 = new AlwaysEqualsClass();
            var source2 = new AlwaysEqualsClass();

            replicationContext.Replicate(source1);
            replicationContext.Replicate(source2);

            replicationStrategy
                .CallsTo(x => x.Replicate(null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void SourceContainsCircularReference_ExceptionIsThrown()
        {
            var source = new Class();
            replicationStrategy
                .CallsTo(x => x.Replicate(source, A<IReplicationContext>.Ignored))
                .Invokes((object _, IReplicationContext context) => context.Replicate(source));

            TestDelegate action = () => replicationContext.Replicate(source);

            Assert.That(action, Throws.InstanceOf<CircularReferenceFoundException>());
        }

        private class AlwaysEqualsClass
        {
            private bool Equals(AlwaysEqualsClass other)
            {
                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((AlwaysEqualsClass) obj);
            }

            public override int GetHashCode()
            {
                return 42;
            }
        }

        private class Class
        {
        }
    }
}