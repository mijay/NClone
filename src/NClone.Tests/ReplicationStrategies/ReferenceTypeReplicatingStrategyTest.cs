using System;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using mijay.Utils;
using NClone.MetadataProviders;
using NClone.ObjectReplication;
using NClone.ReplicationStrategies;
using NUnit.Framework;

namespace NClone.Tests.ReplicationStrategies
{
    public class ReferenceTypeReplicatingStrategyTest: TestBase
    {
        private IReplicationContext dummyContext;

        protected override void SetUp()
        {
            base.SetUp();
            dummyContext = A.Fake<IReplicationContext>(x => x.Strict());
        }

        private static ReferenceTypeReplicationStrategy ReplicatorFor<T>(IMetadataProvider metadataProvider = null)
        {
            return new ReferenceTypeReplicationStrategy(metadataProvider ?? A.Fake<IMetadataProvider>(), typeof (T));
        }

        private static IMetadataProvider MetadataProviderFor<T>(Expression<Func<T, object>> member, ReplicationBehavior returnsBehavior)
        {
            Expression memberAccess = member.Body is UnaryExpression
                ? member.Body.As<UnaryExpression>().Operand
                : member.Body;
            var fieldInfo = memberAccess.As<MemberExpression>().Member.As<FieldInfo>();

            var metadataProvider = A.Fake<IMetadataProvider>();
            metadataProvider
                .CallsTo(x => x.GetFieldsReplicationInfo(typeof (T)))
                .Returns(new[] { new FieldReplicationInfo(fieldInfo, returnsBehavior) });

            return metadataProvider;
        }

        private static IReplicationContext ReplicationContextThat<T>(T onReceiving, T returns)
        {
            var replicationContext = A.Fake<IReplicationContext>(x => x.Strict());
            replicationContext
                .CallsTo(x => x.ReplicateAsync(onReceiving))
                .Returns(returns);
            return replicationContext;
        }

        [Test]
        public void ReplicatorForBaseType_ReceivesInheritedType_Exception()
        {
            var replicator = ReplicatorFor<Class>();

            Assert.Throws<ArgumentException>(() => replicator.Replicate(new InheritedClass(), dummyContext));
        }

        [Test]
        public void SourceHasCtor_SourceIsReplicated_CtorWasNotCalledDuringReplication()
        {
            var replicator = ReplicatorFor<ClassWithCtor>();

            var result = replicator.Replicate(new ClassWithCtor(), dummyContext).As<ClassWithCtor>();

            Assert.That(result.CtorWasCalled, Is.False);
        }

        [Test]
        public void SourceHasFieldMarkedForCopy_SourceIsReplicated_FieldWasCopied()
        {
            IMetadataProvider metadataProvider = MetadataProviderFor<ClassWithField>(
                member: x => x.field, returnsBehavior: ReplicationBehavior.Copy);
            var replicator = ReplicatorFor<ClassWithField>(metadataProvider);

            var source = new ClassWithField { field = new Class() };
            var result = replicator.Replicate(source, dummyContext).As<ClassWithField>();

            Assert.That(result.field, Is.SameAs(source.field));
        }

        [Test]
        public void SourceHasFieldMarkedForReplication_SourceIsReplicated_FieldWasReplicated()
        {
            var sourceField = new Class();
            var resultField = new Class();

            var metadataProvider = MetadataProviderFor<ClassWithField>(x => x.field, ReplicationBehavior.DeepCopy);
            var replicator = ReplicatorFor<ClassWithField>(metadataProvider);
            var context = ReplicationContextThat(onReceiving: sourceField, returns: resultField);

            var source = new ClassWithField { field = sourceField };
            var result = replicator.Replicate(source, context).As<ClassWithField>();

            Assert.That(result.field, Is.SameAs(resultField));
        }

        [Test]
        public void SourceIsValueType_SourceIsReplicated_FieldWasReplicated()
        {
            DateTime sourceField = DateTime.Today;
            DateTime resultField = DateTime.Today.AddDays(1);

            var metadataProvider = MetadataProviderFor<Struct>(x => x.field, ReplicationBehavior.DeepCopy);
            var replicator = ReplicatorFor<Struct>(metadataProvider);
            var context = ReplicationContextThat(onReceiving: sourceField, returns: resultField);

            var source = new Struct { field = sourceField };
            var result = replicator.Replicate(source, context).As<Struct>();

            Assert.That(result.field, Is.EqualTo(resultField));
        }

        private class Class { }

        private class ClassWithCtor
        {
            public readonly bool CtorWasCalled;

            public ClassWithCtor()
            {
                CtorWasCalled = true;
            }
        }

        private class ClassWithField
        {
            public Class field;
        }

        private class InheritedClass: Class { }

        private struct Struct
        {
            public DateTime field;
        }
    }
}