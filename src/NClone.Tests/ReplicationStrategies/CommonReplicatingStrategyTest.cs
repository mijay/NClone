using System;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NClone.MetadataProviders;
using NClone.ObjectReplicators;
using NClone.ReplicationStrategies;
using NClone.Shared;
using NUnit.Framework;

namespace NClone.Tests.ReplicationStrategies
{
    public class CommonReplicatingStrategyTest: TestBase
    {
        private static CommonReplicationStrategy ReplicatorFor<T>(IMetadataProvider metadataProvider = null,
                                                                  IObjectReplicator objectReplicator = null)
        {
            return new CommonReplicationStrategy(metadataProvider ?? A.Fake<IMetadataProvider>(),
                objectReplicator ?? A.Fake<IObjectReplicator>(x => x.Strict()),
                typeof (T));
        }

        private static IMetadataProvider MetadataProviderFor<T>(Expression<Func<T, object>> member, ReplicationBehavior returnsBehavior)
        {
            var memberAccess = member.Body is UnaryExpression
                ? member.Body.As<UnaryExpression>().Operand
                : member.Body;
            var fieldInfo = memberAccess.As<MemberExpression>().Member.As<FieldInfo>();

            var metadataProvider = A.Fake<IMetadataProvider>();
            metadataProvider
                .CallsTo(x => x.GetMembers(typeof (T)))
                .Returns(new[] { new MemberInformation(fieldInfo, returnsBehavior) });

            return metadataProvider;
        }

        private static IObjectReplicator ObjectReplicatorThat<T>(T onReceiving, T returns)
        {
            var objectReplicator = A.Fake<IObjectReplicator>(x => x.Strict());
            objectReplicator
                .CallsTo(x => x.Replicate(onReceiving))
                .Returns(returns);
            return objectReplicator;
        }

        [Test]
        public void ReplicatorForBaseType_ReceivesInheritedType_Exception()
        {
            CommonReplicationStrategy replicator = ReplicatorFor<Class>();

            Assert.Throws<ArgumentException>(() => replicator.Replicate(new InheritedClass()));
        }

        [Test]
        public void SourceHasCtor_SourceIsReplicated_CtorWasNotCalledDuringReplication()
        {
            CommonReplicationStrategy replicator = ReplicatorFor<ClassWithCtor>();

            var result = replicator.Replicate(new ClassWithCtor()).As<ClassWithCtor>();

            Assert.That(result.CtorWasCalled, Is.False);
        }

        [Test]
        public void SourceHasFieldMarkedForCopy_SourceIsReplicated_FieldWasCopied()
        {
            IMetadataProvider metadataProvider = MetadataProviderFor<ClassWithField>(
                member: x => x.field, returnsBehavior: ReplicationBehavior.Copy);
            CommonReplicationStrategy replicator = ReplicatorFor<ClassWithField>(metadataProvider);

            var source = new ClassWithField { field = new Class() };
            var result = replicator.Replicate(source).As<ClassWithField>();

            Assert.That(result.field, Is.SameAs(source.field));
        }

        [Test]
        public void SourceHasFieldMarkedForReplication_SourceIsReplicated_FieldWasReplicated()
        {
            var sourceField = new Class();
            var resultField = new Class();

            IMetadataProvider metadataProvider = MetadataProviderFor<ClassWithField>(x => x.field, ReplicationBehavior.DeepCopy);
            IObjectReplicator objectReplicator = ObjectReplicatorThat(onReceiving: sourceField, returns: resultField);
            CommonReplicationStrategy replicator = ReplicatorFor<ClassWithField>(metadataProvider, objectReplicator);

            var source = new ClassWithField { field = sourceField };
            var result = replicator.Replicate(source).As<ClassWithField>();

            Assert.That(result.field, Is.SameAs(resultField));
        }

        [Test]
        public void SourceIsValueType_SourceIsReplicated_FieldWasReplicated()
        {
            var sourceField = DateTime.Today;
            var resultField = DateTime.Today.AddDays(1);

            IMetadataProvider metadataProvider = MetadataProviderFor<Struct>(x => x.field, ReplicationBehavior.DeepCopy);
            IObjectReplicator objectReplicator = ObjectReplicatorThat(onReceiving: sourceField, returns: resultField);
            CommonReplicationStrategy replicator = ReplicatorFor<Struct>(metadataProvider, objectReplicator);

            var source = new Struct { field = sourceField };
            var result = replicator.Replicate(source).As<Struct>();

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