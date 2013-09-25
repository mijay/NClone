using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NClone.MetadataProviders;
using NClone.ReplicationStrategies;
using NUnit.Framework;

namespace NClone.Tests.ReplicationStrategies
{
    public class ReplicationStrategyFactoryTest: TestBase
    {
        private static ReplicationStrategyFactory StrategyWhere<T>(ReplicationBehavior isMarkedAs)
        {
            var metadataProvider = A.Fake<IMetadataProvider>(x => x.Strict());
            metadataProvider
                .CallsTo(x => x.GetBehavior(typeof (T)))
                .Returns(isMarkedAs);
            metadataProvider
                .CallsTo(x => x.GetMembers(typeof (T)))
                .WithAnyArguments()
                .Returns(new MemberInformation[0]);

            return new ReplicationStrategyFactory(metadataProvider);
        }

        [Test]
        public void TypeIsMarkedAsIgnored_IgnoringStrategyReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyWhere<Class>(isMarkedAs: ReplicationBehavior.Ignore);

            IReplicationStrategy result = strategyFactory.StrategyForType(typeof (Class));

            Assert.That(result, Is.InstanceOf<IgnoringReplicationStrategy>());
        }

        [Test]
        public void SourceIsMarkedAsCopy_CopyStrategyReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyWhere<Class>(isMarkedAs: ReplicationBehavior.Copy);

            IReplicationStrategy result = strategyFactory.StrategyForType(typeof (Class));

            Assert.That(result, Is.InstanceOf<CopyOnlyReplicationStrategy>());
        }

        [Test]
        public void SourceIsMarkedAsReplicate_CommonStrategyReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyWhere<Class>(isMarkedAs: ReplicationBehavior.Replicate);

            IReplicationStrategy result = strategyFactory.StrategyForType(typeof (Class));

            Assert.That(result, Is.InstanceOf<CommonReplicationStrategy>());
        }

        [Test]
        public void GetStrategyTwiceForTheSameType_SameStrategiesReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyWhere<Class>(isMarkedAs: ReplicationBehavior.Replicate);

            IReplicationStrategy result1 = strategyFactory.StrategyForType(typeof (Class));
            IReplicationStrategy result2 = strategyFactory.StrategyForType(typeof (Class));

            Assert.That(result2, Is.SameAs(result1));
        }

        private class Class
        {
            public object field;
        }
    }
}