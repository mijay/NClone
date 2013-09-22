using System;
using NClone.MetadataProviders;
using NClone.ObjectReplicators;
using NClone.Shared;
using NUnit.Framework;

namespace NClone.Tests.ObjectReplicators
{
    public class ObjectReplicatorTest: TestBase
    {
        private IObjectReplicator objectReplicator;

        protected override void SetUp()
        {
            base.SetUp();
            objectReplicator = new ObjectReplicator(new ConventionalMetadataProvider());
        }

        [Test]
        public void SourceIsString_SameReturned()
        {
            var source = "blah-blah";

            var result = objectReplicator.Replicate(source);

            Assert.That(result, Is.SameAs(source));
        }

        [Test]
        public void SourceIsNumber_CanBeCloned()
        {
            var source = 42;

            var result = objectReplicator.Replicate(source);

            Assert.That(result, Is.EqualTo(source));
        }

        [Test]
        public void SourceIsNullableNumber_CanBeCloned()
        {
            var sourceWithValue = new int?(42);
            var sourceWithoutValue = new int?();

            var resultWithValue = objectReplicator.Replicate(sourceWithValue);
            var resultWithoutValue = objectReplicator.Replicate(sourceWithoutValue);

            Assert.That(resultWithValue, Is.EqualTo(sourceWithValue));
            Assert.That(resultWithoutValue, Is.EqualTo(sourceWithoutValue));
        }

        [Test]
        public void SourceIsValueType_Copied()
        {
            var source = DateTime.Now;

            var result = objectReplicator.Replicate(source);

            Assert.That(result, Is.EqualTo(source));
        }

        private class Class
        {
            public Class field;
            public int number;
        }

        [Test]
        public void SourceIsReferenceType_Cloned()
        {
            var source = new Class { field = new Class { number = RandomInt() } };

            var result = objectReplicator.Replicate(source).As<Class>();

            Assert.That(result, Is.Not.SameAs(source));
            Assert.That(result.field, Is.Not.SameAs(source.field));
            Assert.That(result.field.number, Is.EqualTo(source.field.number));
        }

        [Test]
        public void SourceIsNull_NullReturned()
        {
            var result = objectReplicator.Replicate(null);

            Assert.That(result, Is.Null);
        }
    }
}