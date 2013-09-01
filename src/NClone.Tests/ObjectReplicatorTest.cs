using System;
using NUnit.Framework;

namespace NClone.Tests
{
    public class ObjectReplicatorTest: TestBase
    {

        [Test]
        public void SourceIsString_SameReturned()
        {
            var source = "blah-blah";

            var result = ObjectReplicator.Replicate(source);

            Assert.That(result, Is.SameAs(source));
        }

        [Test]
        public void SourceIsNumber_CanBeCloned()
        {
            var source = 42;

            var result = ObjectReplicator.Replicate(source);

            Assert.That(result, Is.EqualTo(source));
        }

        [Test]
        public void SourceIsNullableNumber_CanBeCloned()
        {
            var sourceWithValue = new int?(42);
            var sourceWithoutValue = new int?();

            var resultWithValue = ObjectReplicator.Replicate(sourceWithValue);
            var resultWithoutValue = ObjectReplicator.Replicate(sourceWithoutValue);

            Assert.That(resultWithValue, Is.EqualTo(sourceWithValue));
            Assert.That(resultWithoutValue, Is.EqualTo(sourceWithoutValue));
        }

        [Test]
        public void SourceIsValueType_Copied()
        {
            var source = DateTime.Now;

            var result = ObjectReplicator.Replicate(source);

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

            var result = ObjectReplicator.Replicate(source);

            Assert.That(result, Is.Not.SameAs(source));
            Assert.That(result.field, Is.Not.SameAs(source.field));
            Assert.That(result.field.number, Is.EqualTo(source.field.number));
        }

        [Test]
        public void SourceIsNull_NullReturned()
        {
            var result = ObjectReplicator.Replicate(null);

            Assert.That(result, Is.Null);
        }
    }
}