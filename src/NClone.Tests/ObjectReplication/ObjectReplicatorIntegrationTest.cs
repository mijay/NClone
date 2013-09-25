using System;
using NUnit.Framework;

namespace NClone.Tests.ObjectReplication
{
    public class ObjectReplicatorIntegrationTest: TestBase
    {
        //private static ReplicationStrategyFactory ReplicatorFor<T>(ReplicationBehavior markedAs,
        //                                                 Expression<Func<T, object>> thatCopiesField = null)
        //{
        //    var metadataProvider = A.Fake<IMetadataProvider>(x => x.Strict());
        //    metadataProvider
        //        .CallsTo(x => x.GetBehavior(typeof(T)))
        //        .Returns(markedAs);

        //    if (thatCopiesField != null)
        //    {
        //        var fieldInfo = thatCopiesField.Body.As<MemberExpression>().Member.As<FieldInfo>();
        //        metadataProvider
        //            .CallsTo(x => x.GetMembers(typeof(T)))
        //            .Returns(new[] { new MemberInformation(fieldInfo, ReplicationBehavior.Copy) });
        //    }

        //    return new ReplicationStrategyFactory(metadataProvider);
        //}

        [Test]
        public void SourceContainsNullableFields_CanBeReplicated()
        {
            //var source = new ClassWithNullables
            //             {
            //                 field1 = null,
            //                 field2 = DateTime.Today
            //             };

            //object result = replicator.Replicate(source);

            //Assert.That(result.As<ClassWithNullables>().field1, Is.Null);
            //Assert.That(result.As<ClassWithNullables>().field2, Is.EqualTo(source.field2));
            Assert.Fail();
        }

        [Test]
        public void SourceIsNull_NullReturned()
        {
            //object result = new ReplicationStrategyFactory(A.Fake<IMetadataProvider>()).Replicate(null);

            //Assert.That(result, Is.Null);
            Assert.Fail();
        }

        [Test]
        public void SourceIsMarkedAsReplicate_ReplicaReturned()
        {
            //ReplicationStrategyFactory replicator = StrategyWhere<ReplicationStrategyFactoryTest.Class>(
            //    isMarkedAs: ReplicationBehavior.Replicate, thatCopiesField: x => x.field);

            //var source = new ReplicationStrategyFactoryTest.Class() { field = new object() };
            //object result = replicator.Replicate(source);

            //Assert.That(result, Is.Not.SameAs(source));
            //Assert.That(result.As<ReplicationStrategyFactoryTest.Class>().field, Is.SameAs(source.field));
            Assert.Fail();
        }

        private class Class
        {
            public object field;
        }

        private class ClassWithNullables
        {
            public DateTime? field1;
            public DateTime? field2;
        }
    }
}