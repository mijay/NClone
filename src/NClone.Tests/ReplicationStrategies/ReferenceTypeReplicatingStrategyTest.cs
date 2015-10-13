using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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

        private static ReferenceTypeReplicationStrategy ReplicatorFor<T>(IMetadataProvider with = null)
        {
            return new ReferenceTypeReplicationStrategy(with ?? A.Fake<IMetadataProvider>(), typeof (T));
        }

        private static IMetadataProvider MetadataProviderFor<T>(Expression<Func<T, object>> member, ReplicationBehavior returnsBehavior)
        {
            var memberAccess = member.Body is UnaryExpression
                ? member.Body.As<UnaryExpression>().Operand
                : member.Body;
            var fieldInfo = memberAccess.As<MemberExpression>().Member.As<FieldInfo>();

            var metadataProvider = A.Fake<IMetadataProvider>(x => x.Strict());
            metadataProvider
                .CallsTo(x => x.GetFieldsReplicationInfo(typeof (T)))
                .Returns(new[] { new FieldReplicationInfo(fieldInfo, returnsBehavior) });

            return metadataProvider;
        }

        [Test]
        public void ReplicatorForBaseType_ReceivesInheritedType_Exception()
        {
            var replicator = ReplicatorFor<Class>();

            Assert.Throws<ArgumentException>(() => replicator.Replicate(new InheritedClass(), dummyContext));
        }

        [Test]
        public void SourceHasCtor_SourceIsReplicated_CtorIsNotCalledDuringReplication()
        {
            var replicator = ReplicatorFor<ClassWithCtor>();

            var result = replicator.Replicate(new ClassWithCtor(), dummyContext).As<ClassWithCtor>();

            Assert.That(result.ctorWasCalled, Is.False);
        }

        [Test]
        public void SourceHasFieldMarkedForCopy_SourceIsReplicated_FieldIsCopied()
        {
            var replicator = ReplicatorFor<ClassWithField>(with: MetadataProviderFor<ClassWithField>(x => x.field, ReplicationBehavior.Copy));

            var source = new ClassWithField { field = new Class() };
            var result = replicator.Replicate(source, dummyContext).As<ClassWithField>();

            Assert.That(result.field, Is.SameAs(source.field));
        }

        [Test]
        public void SourceHasFieldMarkedForReplication_SourceIsReplicated_FieldIsSetAfterReplicatedViaContext()
        {
            var sourceField = new Class();
            var resultTaskSource = new TaskCompletionSource<object>();

            var replicator = ReplicatorFor<ClassWithField>(with: MetadataProviderFor<ClassWithField>(x => x.field, ReplicationBehavior.DeepCopy));
            var context = A.Fake<IReplicationContext>(x => x.Strict());
            context
                .CallsTo(x => x.ReplicateAsync(sourceField))
                .ReturnsLazily(_ => resultTaskSource.Task);

            var source = new ClassWithField { field = sourceField };
            var result = replicator.Replicate(source, context).As<ClassWithField>();

            Assert.That(result.field, Is.Null);

            var resultField = new Class();
            resultTaskSource.SetResult(resultField);
            Assert.That(result.field, Is.SameAs(resultField));
        }

        private class Class { }

        private class ClassWithCtor
        {
            public readonly bool ctorWasCalled;

            public ClassWithCtor()
            {
                ctorWasCalled = true;
            }
        }

        private class ClassWithField
        {
            public Class field;
        }

        private class InheritedClass: Class { }
    }
}
