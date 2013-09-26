using System;
using System.Diagnostics;
using NClone.MetadataProviders;
using NClone.ObjectReplication;
using NClone.Shared;
using NUnit.Framework;

namespace NClone.Tests.ObjectReplication
{
    public class ObjectReplicatorIntegrationTest: TestBase
    {
        private ObjectReplicator objectReplicator;

        protected override void SetUp()
        {
            base.SetUp();
            objectReplicator = new ObjectReplicator(new AttributeBasedMetadataProvider());
        }

        [Test]
        public void SourceIsNull_NullReturned()
        {
            var result = objectReplicator.Replicate<Class>(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void SourceIsStruct_ItIsReplicated()
        {
            var source = new Struct { field1 = "some string", field2 = new Class() };

            Struct result = objectReplicator.Replicate(source);

            Assert.That(result.field1, Is.SameAs(source.field1));
            Assert.That(result.field2, Is.Not.SameAs(source.field2));
        }

        [Test]
        public void SourceIsClass_ItIsReplicated()
        {
            var source = new Class { field = "some string" };

            Class result = objectReplicator.Replicate(source);

            Assert.That(result, Is.Not.SameAs(source));
            Assert.That(result.field, Is.SameAs(source.field));
        }

        [Test]
        public void SourceHasObjectFieldWithObjectInIt_ItsValueIsCopied()
        {
            var source = new Class { field = new object() };

            Class result = objectReplicator.Replicate(source);

            Assert.That(result, Is.Not.SameAs(source));
            Assert.That(result.field, Is.SameAs(source.field));
        }

        [Test]
        public void SourceHasObjectFieldWithSomeClassInIt_ItsValueIsReplicated()
        {
            var source = new Class { field = new Class { field = "some string" } };

            Class result = objectReplicator.Replicate(source);

            Assert.That(result.field, Is.Not.SameAs(source.field));
            Assert.That(result.field.As<Class>().field, Is.SameAs(source.field.As<Class>().field));
        }

        [Test]
        public void SourceHasObjectFieldWithSomeStructInIt_ItsValueIsReplicated()
        {
            var source = new Class { field = new Struct { field1 = "some string" } };

            Class result = objectReplicator.Replicate(source);

            Assert.That(result.field.As<Struct>().field1, Is.SameAs(source.field.As<Struct>().field1));
        }

        [Test]
        public void SourceContainsNullableFields_CanBeReplicated()
        {
            var source = new ClassWithNullables
                         {
                             field1 = null,
                             field2 = new Struct { field1 = "some string", field2 = new Class() }
                         };

            ClassWithNullables result = objectReplicator.Replicate(source);

            Assert.That(result.field1, Is.Null);
            Assert.That(result.field2, Is.Not.Null);
            Assert.That(result.field2.Value.field1, Is.SameAs(source.field2.Value.field1));
            Assert.That(result.field2.Value.field2, Is.Not.SameAs(source.field2.Value.field2));
        }

        [Test]
        public void ObjectIsUsedTwiceInObjectGraph_ReplicatedValueIsAlsoUsedTwice()
        {
            var obj = new Class { field = "some string" };
            Tuple<Class, Class> source = Tuple.Create(obj, obj);

            Tuple<Class, Class> result = objectReplicator.Replicate(source);

            Assert.That(result.Item1.field, Is.SameAs(obj.field));
            Assert.That(result.Item1, Is.SameAs(result.Item2));
        }

        [Test]
        public void SourceContainsCircularReferences_ItIsDetected()
        {
            var source = new Class();
            source.field = source;

            TestDelegate action = () => objectReplicator.Replicate(source);

            Assert.That(action, Throws.InstanceOf<CircularReferenceFoundException>());
        }

        [Test]
        public void SourceIsMarkedAsCopy_SameObjectReturned()
        {
            var source = new ClassWithAttribute();

            ClassWithAttribute result = objectReplicator.Replicate(source);

            Assert.That(result, Is.SameAs(source));
        }

        [Test]
        public void SourceFieldIsMarkedAsIgnored_ItIsNullInReplica()
        {
            var source = new StructWithAttribute
                         {
                             field1 = "some string",
                             field2 = new Class()
                         };

            StructWithAttribute result = objectReplicator.Replicate(source);

            Assert.That(result.field1, Is.SameAs(source.field1));
            Assert.That(result.field2, Is.Null);
        }

        [Test]
        public void SourceIsReplicatedTwice_SecondReplicationIsFaster()
        {
            var source = new ClassWithNullables { field2 = new Struct { field2 = new Class() } };
            var stopwatch = new Stopwatch();
            objectReplicator.Replicate(DateTime.Today);
            objectReplicator.Replicate(DateTime.Today);

            stopwatch.Start();
            objectReplicator.Replicate(source);
            var firstTime = stopwatch.ElapsedTicks;
            stopwatch.Restart();
            objectReplicator.Replicate(source);
            var secondTime = stopwatch.ElapsedTicks;

            stopwatch.Stop();
            Console.Write(firstTime + "\n" + secondTime);
            Assert.That(secondTime, Is.LessThan(firstTime / 10));
        }

        private class Class
        {
            public object field;
        }

        [CustomReplicationBehavior(ReplicationBehavior.Copy)]
        private class ClassWithAttribute { }

        private class ClassWithNullables
        {
            public Struct? field1;
            public Struct? field2;
        }

        private struct Struct
        {
            public string field1;
            public Class field2;
        }

        private struct StructWithAttribute
        {
            public string field1;

            [CustomReplicationBehavior(ReplicationBehavior.Ignore)]
            public Class field2;
        }
    }
}