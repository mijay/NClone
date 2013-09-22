using System;
using NClone.Annotation;
using NClone.ObjectReplicators;
using NClone.Shared;
using NClone.SpecificTypeReplicators;
using NUnit.Framework;

namespace NClone.Tests.SpecificTypesReplicators
{
    public class CommonReplicatingStrategyTest: TestBase
    {
        #region Test data

        private class ClassWithCtor
        {
            public static int CtorCounter;

            public ClassWithCtor()
            {
                CtorCounter++;
            }
        }

        private class ClassWithPrivateField
        {
            private readonly int field;

            public ClassWithPrivateField(int field)
            {
                this.field = field;
            }

            public int GetField()
            {
                return field;
            }
        }

        private class ClassWithInheritedPrivateField : ClassWithPrivateField
        {
            public ClassWithInheritedPrivateField(int field) : base(field) { }
        }

        private class ClassWithPublicField
        {
            public readonly int field;

            public ClassWithPublicField(int field)
            {
                this.field = field;
            }

            public int GetField()
            {
                return field;
            }
        }

        private class ClassWithNewField : ClassWithPublicField
        {
            public new readonly int field;

            public ClassWithNewField(int field)
                : base(field)
            {
                this.field = field + 1;
            }
        }

        private class ClassWithComplexField
        {
            public ClassWithPrivateField field;
        }

        #endregion

        private static CommonReplicationStrategy ReplicatorFor<T>()
        {
            var metadataProvider = new ConventionalMetadataProvider();
            return new CommonReplicationStrategy(metadataProvider, new ObjectReplicator(metadataProvider), typeof (T));
        }

        private static T Replicate<T>(T source)
        {
            return ReplicatorFor<T>().Replicate(source).As<T>();
        }

        [Test]
        public void ReplicatorForBaseType_ReceivesInheritedType_Exception()
        {
            var replicator = ReplicatorFor<ClassWithPublicField>();

            Assert.Throws<ArgumentException>(
                () => replicator.Replicate(new ClassWithNewField(RandomInt())));
        }

        [Test]
        public void SourceHasCtor_ItWasNotCalledDuringReplication()
        {
            Replicate(new ClassWithCtor());

            Assert.That(ClassWithCtor.CtorCounter, Is.EqualTo(1));
        }

        [Test]
        public void SourceHasPrivateField_ItsValueIsCopied()
        {
            var source = new ClassWithPrivateField(RandomInt());

            var result = Replicate(source);

            Assert.That(result.GetField(), Is.EqualTo(source.GetField()));
        }

        [Test]
        public void SourceBaseHasPrivateField_ItsValueIsCopied()
        {
            var source = new ClassWithInheritedPrivateField(RandomInt());

            var result = Replicate(source);

            Assert.That(result.GetField(), Is.EqualTo(source.GetField()));
        }

        [Test]
        public void SourceAndBaseHasPrivateFieldsWithSameName_BothValuesAreCopied()
        {
            var source = new ClassWithNewField(RandomInt());

            var result = Replicate(source);

            Assert.That(result.GetField(), Is.EqualTo(source.GetField()));
            Assert.That(result.field, Is.EqualTo(source.field));
        }

        [Test]
        public void SourceHasNestedObject_ItIsReplicated()
        {
            var source = new ClassWithComplexField { field = new ClassWithPrivateField(RandomInt()) };

            var result = Replicate(source);

            Assert.That(result.field, Is.Not.SameAs(source.field));
            Assert.That(result.field.GetField(), Is.EqualTo(source.field.GetField()));
        }

        [Test]
        public void SourceWithNestedObject_ContainsObjectOfInheritedType_ItIsReplicated()
        {
            var source = new ClassWithComplexField { field = new ClassWithInheritedPrivateField(RandomInt()) };

            var result = Replicate(source);

            Assert.That(result.field, Is.Not.SameAs(source.field));
            Assert.That(result.field, Is.InstanceOf<ClassWithInheritedPrivateField>());
            Assert.That(result.field.GetField(), Is.EqualTo(source.field.GetField()));
        }
    }
}