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
    public class ValueTypeReplicatingStrategyTest: TestBase
    {
        private IReplicationContext dummyContext;

        protected override void SetUp()
        {
            base.SetUp();
            dummyContext = A.Fake<IReplicationContext>(x => x.Strict());
        }

        private static ValueTypeReplicationStrategy ReplicatorFor<T>(IMetadataProvider with = null)
        {
            return new ValueTypeReplicationStrategy(with ?? A.Fake<IMetadataProvider>(), typeof (T));
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

        private static IReplicationContext ReplicationContextThat(Struct onReceiving, TaskCompletionSource<object> returnsTask)
        {
            var context = A.Fake<IReplicationContext>(x => x.Strict());
            context
                .CallsTo(x => x.ReplicateAsync(onReceiving))
                .ReturnsLazily(_ => returnsTask.Task);
            return context;
        }

        [Test]
        public void SourceHasFieldMarkedForCopy_SourceIsReplicated_FieldIsCopied()
        {
            var replicator = ReplicatorFor<StructWithField>(with: MetadataProviderFor<StructWithField>(x => x.field, ReplicationBehavior.Copy));

            var source = new StructWithField { field = new Struct { id = RandomInt() } };
            var result = replicator.Replicate(source, dummyContext).As<StructWithField>();

            Assert.That(result.field.id, Is.EqualTo(source.field.id));
        }

        [Test]
        public void SourceHasFieldMarkedForReplication_SourceIsReplicated_FieldIsReplicated()
        {
            var sourceField = new Struct { id = RandomInt() };
            var resultField = new Struct { id = RandomInt() };
            var resultTaskSource = new TaskCompletionSource<object>();
            resultTaskSource.SetResult(resultField);

            var replicator = ReplicatorFor<StructWithField>(with: MetadataProviderFor<StructWithField>(x => x.field, ReplicationBehavior.DeepCopy));
            var context = ReplicationContextThat(onReceiving: sourceField, returnsTask: resultTaskSource);

            var source = new StructWithField { field = sourceField };
            var result = replicator.Replicate(source, context).As<StructWithField>();

            Assert.That(result.field.id, Is.EqualTo(resultField.id));
        }

        [Test]
        public void SourceHasFieldMarkedForReplication_SourceIsReplicatedButFieldIsNotYet_Exception()
        {
            var sourceField = new Struct();
            var resultTaskSource = new TaskCompletionSource<object>();

            var replicator = ReplicatorFor<StructWithField>(with: MetadataProviderFor<StructWithField>(x => x.field, ReplicationBehavior.DeepCopy));
            var context = ReplicationContextThat(onReceiving: sourceField, returnsTask: resultTaskSource);

            var source = new StructWithField { field = sourceField };
            TestDelegate action = () => replicator.Replicate(source, context).As<StructWithField>();

            Assert.That(action, Throws.InstanceOf<CircularReferenceFoundException>());
        }

        private struct Struct
        {
            public int id;
        }

        private struct StructWithField
        {
            public Struct field;
        }
    }
}