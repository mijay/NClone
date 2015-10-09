using System;
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
                .CallsTo(x => x.GetPerTypeBehavior(typeof (T)))
                .Returns(isMarkedAs);
            metadataProvider
                .CallsTo(x => x.GetFieldsReplicationInfo(typeof (T)))
                .Returns(new FieldReplicationInfo[0]);

            return new ReplicationStrategyFactory(metadataProvider);
        }

        private static ReplicationStrategyFactory StrategyForArrayOf<T>(ReplicationBehavior whereElementsAreMarkedAs)
        {
            var metadataProvider = A.Fake<IMetadataProvider>(x => x.Strict());
            metadataProvider
                .CallsTo(x => x.GetPerTypeBehavior(typeof (T[])))
                .Returns(ReplicationBehavior.DeepCopy);
            metadataProvider
                .CallsTo(x => x.GetPerTypeBehavior(typeof (T)))
                .Returns(whereElementsAreMarkedAs);
            metadataProvider
                .CallsTo(x => x.GetFieldsReplicationInfo(typeof(T)))
                .Returns(new FieldReplicationInfo[0]);

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
        public void SourceIsMarkedAsReplicate_ReferenceTypeStrategyReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyWhere<Class>(isMarkedAs: ReplicationBehavior.DeepCopy);

            IReplicationStrategy result = strategyFactory.StrategyForType(typeof (Class));

            Assert.That(result, Is.InstanceOf<ReferenceTypeReplicationStrategy>());
        }

        [Test]
        public void SourceStructIsMarkedAsReplicate_ValueTypeStrategyReturned()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void SourceNullableStructIsMarkedAsReplicate_ValueTypeStrategyReturned()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GetStrategyTwiceForTheSameType_SameStrategiesReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyWhere<Class>(isMarkedAs: ReplicationBehavior.DeepCopy);

            IReplicationStrategy result1 = strategyFactory.StrategyForType(typeof (Class));
            IReplicationStrategy result2 = strategyFactory.StrategyForType(typeof (Class));

            Assert.That(result2, Is.SameAs(result1));
        }

        [Test]
        public void SourceIsArrayOfIgnoredTypes_CopyArrayStrategyReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyForArrayOf<Class>(whereElementsAreMarkedAs: ReplicationBehavior.Ignore);

            IReplicationStrategy result = strategyFactory.StrategyForType(typeof (Class[]));

            Assert.That(result, Is.InstanceOf<IgnoringReplicationStrategy>());
        }

        [Test]
        public void SourceIsArrayOfCopiedTypes_CopyArrayStrategyReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyForArrayOf<Class>(whereElementsAreMarkedAs: ReplicationBehavior.Copy);

            IReplicationStrategy result = strategyFactory.StrategyForType(typeof (Class[]));

            Assert.That(result, Is.InstanceOf<CopyArrayReplicationStrategy>());
        }

        [Test]
        public void SourceIsArrayOfClonnedTypes_CloneArrayStrategyReturned()
        {
            ReplicationStrategyFactory strategyFactory = StrategyForArrayOf<Class>(whereElementsAreMarkedAs : ReplicationBehavior.DeepCopy);

            IReplicationStrategy result = strategyFactory.StrategyForType(typeof (Class[]));

            Assert.That(result, Is.InstanceOf<CloneArrayReplicationStrategy>());
        }

        private class Class
        {
            public object field;
        }
    }
}