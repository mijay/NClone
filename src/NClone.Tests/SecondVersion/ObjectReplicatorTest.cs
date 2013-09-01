using System;
using NClone.SecondVersion;
using NUnit.Framework;

namespace NClone.Tests.SecondVersion
{
    public class ObjectReplicatorTest: TestBase
    {
        #region Test data

        private class ClassWithCtor
        {
            public static int ctorCounter;

            public ClassWithCtor()
            {
                ctorCounter++;
            }
        }

        private class Class
        {
            private int field;

            public Class(int field)
            {
                this.field = field;
            }

            public int GetField()
            {
                return field;
            }
        }

        private class InheritedClass: Class
        {
            public InheritedClass(int field): base(field) { }
        }

        private class ClassWithPublicField
        {
            public int field;

            public ClassWithPublicField(int field)
            {
                this.field = field;
            }

            public int GetField()
            {
                return field;
            }
        }

        private class InheritedClassWithNew: ClassWithPublicField
        {
            public InheritedClassWithNew(int field): base(field)
            {
                this.field = field + 1;
            }

            public new int field;
        }

        #endregion

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

        [Test]
        public void SourceHasPrivateField_Copied()
        {
            var source = new Class(RandomInt());

            var result = ObjectReplicator.Replicate(source);

            Assert.That(result.GetField(), Is.EqualTo(source.GetField()));
        }

        [Test]
        public void SourceHasCtor_ItWasNotCalledDuringReplication()
        {
            ObjectReplicator.Replicate(new ClassWithCtor());

            Assert.That(ClassWithCtor.ctorCounter, Is.EqualTo(1));
        }

        [Test]
        public void SourceHasPrivateInheritedField_Copied()
        {
            var source = new InheritedClass(RandomInt());

            var result = ObjectReplicator.Replicate(source);

            Assert.That(result.GetField(), Is.EqualTo(source.GetField()));
        }

        [Test]
        public void SourceHasPrivateAndInheritedFields_BothCopied()
        {
            var source = new InheritedClassWithNew(RandomInt());

            var result = ObjectReplicator.Replicate(source);

            Assert.That(result.GetField(), Is.EqualTo(source.GetField()));
            Assert.That(result.field, Is.EqualTo(source.field));
        }
    }
}