﻿using System;
using System.Threading.Tasks;
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
            var resultAsync = replicationContext.ReplicateAsync(null);

            Assert.That(resultAsync.Result, Is.Null);
        }

        [Test]
        public void SourceIsNotNull_Replicated()
        {
            var source = new Class();
            var sourceReplica = new Class();
            replicationStrategy
                .CallsTo(x => x.Replicate(source, A<IReplicationContext>.Ignored))
                .Returns(sourceReplica);

            var resultAsync = replicationContext.ReplicateAsync(source);

            Assert.That(resultAsync.Result, Is.SameAs(sourceReplica));
        }

        [Test]
        public void ReplicateSameSourceTwice_FactoryIsCalledOnlyOnce()
        {
            var source = new Class();

            var resultAsync1 = replicationContext.ReplicateAsync(source);
            var resultAsync2 = replicationContext.ReplicateAsync(source);

            Assert.That(resultAsync1.IsCompleted);
            Assert.That(resultAsync2.IsCompleted);
            replicationStrategy
                .CallsTo(x => x.Replicate(null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void ReplicatesEqualButNotSameObjects_FactoryIsCalledForEach()
        {
            var source1 = new AlwaysEqualsClass();
            var source2 = new AlwaysEqualsClass();

            var resultAsync1 = replicationContext.ReplicateAsync(source1);
            var resultAsync2 = replicationContext.ReplicateAsync(source2);

            Assert.That(resultAsync1.IsCompleted);
            Assert.That(resultAsync2.IsCompleted);
            replicationStrategy
                .CallsTo(x => x.Replicate(null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void ReplicateIsCalledRecursively_SecondTaskIsCompletedWithFirst()
        {
            var source = new Class();

            Task<object> recursiveReplicateAsync = null;
            replicationStrategy
                .CallsTo(x => x.Replicate(source, replicationContext))
                .Invokes(_ =>
                {
                    recursiveReplicateAsync = replicationContext.ReplicateAsync(source);
                    Assert.That(recursiveReplicateAsync.IsCompleted, Is.False);
                })
                .Returns(new object());

            var replicateAsync = replicationContext.ReplicateAsync(source);

            Assert.That(replicateAsync.IsCompleted, Is.True);
            Assert.That(recursiveReplicateAsync.IsCompleted, Is.True);
            Assert.That(recursiveReplicateAsync.Result, Is.SameAs(replicateAsync.Result));
        }

        private class AlwaysEqualsClass
        {
            // ReSharper disable once UnusedParameter.Local
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