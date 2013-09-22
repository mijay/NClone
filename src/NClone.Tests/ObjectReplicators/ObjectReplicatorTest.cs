using System;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NClone.MetadataProviders;
using NClone.ObjectReplicators;
using NClone.Shared;
using NUnit.Framework;

namespace NClone.Tests.ObjectReplicators
{
    public class ObjectReplicatorTest: TestBase
    {
        private static ObjectReplicator ReplicatorFor<T>(ReplicationBehavior markedAs,
                                                         Expression<Func<T, object>> thatCopiesField = null)
        {
            var metadataProvider = A.Fake<IMetadataProvider>(x => x.Strict());
            metadataProvider
                .CallsTo(x => x.GetBehavior(typeof (T)))
                .Returns(markedAs);

            if (thatCopiesField != null) {
                var fieldInfo = thatCopiesField.Body.As<MemberExpression>().Member.As<FieldInfo>();
                metadataProvider
                    .CallsTo(x => x.GetMembers(typeof (T)))
                    .Returns(new[] { new MemberInformation(fieldInfo, ReplicationBehavior.Copy) });
            }

            return new ObjectReplicator(metadataProvider);
        }

        [Test]
        public void SourceIsNull_NullReturned()
        {
            object result = new ObjectReplicator(A.Fake<IMetadataProvider>()).Replicate(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SourceIsMarkedAsIgnored_NullReturned()
        {
            ObjectReplicator replicator = ReplicatorFor<Class>(markedAs: ReplicationBehavior.Ignore);

            var source = new Class();
            object result = replicator.Replicate(source);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SourceIsMarkedAsCopy_OriginalReturned()
        {
            ObjectReplicator replicator = ReplicatorFor<Class>(markedAs: ReplicationBehavior.Copy);

            var source = new Class();
            object result = replicator.Replicate(source);

            Assert.That(result, Is.SameAs(source));
        }

        [Test]
        public void SourceIsMarkedAsReplicate_ReplicaReturned()
        {
            ObjectReplicator replicator = ReplicatorFor<Class>(
                markedAs: ReplicationBehavior.Replicate, thatCopiesField: x => x.field);

            var source = new Class() { field = new object() };
            object result = replicator.Replicate(source);

            Assert.That(result, Is.Not.SameAs(source));
            Assert.That(result.As<Class>().field, Is.SameAs(source.field));
        }

        private class Class
        {
            public object field;
        }
    }
}