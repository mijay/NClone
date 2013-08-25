using System;
using System.Linq;
using System.Reflection;
using NClone.MemberAccess;
using NClone.Tests.ExternalAssembly;
using NUnit.Framework;

namespace NClone.Tests.MemberAccess
{
    public class FieldAccessorBuilderTest: TestBase
    {
        #region Test classes

        private class ClassWithPublicField
        {
            public int field;
        }

        private class ClassWithPublicReadonlyField
        {
            public readonly int field;

            public ClassWithPublicReadonlyField(int field)
            {
                this.field = field;
            }
        }

        private class ClassWithPrivateField
        {
            private int field;

            public ClassWithPrivateField(int field)
            {
                this.field = field;
            }

            public int GetField()
            {
                return field;
            }
        }

        private class ClassWithPrivateReadonlyField
        {
            private readonly int field;

            public ClassWithPrivateReadonlyField(int field)
            {
                this.field = field;
            }

            public int GetField()
            {
                return field;
            }
        }

        private class ClassWithInheritedPrivateReadonlyField: ClassWithPrivateReadonlyField
        {
            public ClassWithInheritedPrivateReadonlyField(int field): base(field)
            {
            }
        }

        private class ClassWithInheritedInternalReadonlyField: ClassWithInternalReadonlyField
        {
            public ClassWithInheritedInternalReadonlyField(int field): base(field)
            {
            }
        }

        private class ClassWithInheritedInternalField: ClassWithInternalField
        {
            public ClassWithInheritedInternalField(int field): base(field)
            {
            }
        }

        private struct Structure
        {
            public int field;
        }

        #endregion

        private FieldInfo GetField<TEntity>()
        {
            return typeof (TEntity).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Single();
        }

        public class ArgumentValidationTest: FieldAccessorBuilderTest
        {
            [Test]
            public void BuildAccessorForFieldOfDifferentType_Exception()
            {
                var field = GetField<ClassWithPublicField>();
                Assert.Throws<ArgumentException>(
                    () => FieldAccessorBuilder.BuildFor<ClassWithPublicField, bool>(field, true));
            }

            [Test]
            public void BuildAccessorForDifferentEntity_Exception()
            {
                var field = GetField<ClassWithPublicField>();
                Assert.Throws<ArgumentException>(
                    () => FieldAccessorBuilder.BuildFor<ClassWithPrivateReadonlyField, int>(field, true));
            }

            [Test]
            public void BuildAccessorForInheritedEntity_NoException()
            {
                var field = GetField<ClassWithPrivateReadonlyField>();
                Assert.DoesNotThrow(
                    () => FieldAccessorBuilder.BuildFor<ClassWithInheritedPrivateReadonlyField, int>(field, true));
            }
        }

        public class BuildAccessorForEntityOfReferenceTypeTest: FieldAccessorBuilderTest
        {
            [Test]
            public void BuildAccessorForPublicField_ItCanReadAndWrite()
            {
                var field = GetField<ClassWithPublicField>();

                var accessor = FieldAccessorBuilder.BuildFor<ClassWithPublicField, int>(field, true);

                Assert.That(accessor.CanGet && accessor.CanSet);
                var data = new ClassWithPublicField { field = RandomInt() };
                Assert.That(accessor.GetMember(data), Is.EqualTo(data.field));
                var fieldValue = RandomInt();
                accessor.SetMember(data, fieldValue);
                Assert.That(data.field, Is.EqualTo(fieldValue));
            }

            [Test]
            public void BuildAccessorForPrivateField_ItCanReadAndWrite()
            {
                var field = GetField<ClassWithPrivateReadonlyField>();

                var accessor = FieldAccessorBuilder.BuildFor<ClassWithPrivateReadonlyField, int>(field, true);

                var data = new ClassWithPrivateReadonlyField(RandomInt());
                Assert.That(accessor.GetMember(data), Is.EqualTo(data.GetField()));
                var fieldValue = RandomInt();
                accessor.SetMember(data, fieldValue);
                Assert.That(data.GetField(), Is.EqualTo(fieldValue));
            }

            [Test]
            public void BuildAccessorForPrivateFieldOfBase_ItCanReadAndWrite()
            {
                var field = GetField<ClassWithPrivateReadonlyField>();

                var accessor = FieldAccessorBuilder.BuildFor<ClassWithInheritedPrivateReadonlyField, int>(field, true);

                var data = new ClassWithInheritedPrivateReadonlyField(RandomInt());
                Assert.That(accessor.GetMember(data), Is.EqualTo(data.GetField()));
                var fieldValue = RandomInt();
                accessor.SetMember(data, fieldValue);
                Assert.That(data.GetField(), Is.EqualTo(fieldValue));
            }

            [Test]
            public void BuildAccessorForInternalFieldOfBase_ItCanReadAndWrite()
            {
                var field = GetField<ClassWithInheritedInternalReadonlyField>();

                var accessor = FieldAccessorBuilder.BuildFor<ClassWithInheritedInternalReadonlyField, int>(field, true);

                var data = new ClassWithInheritedInternalReadonlyField(RandomInt());
                Assert.That(accessor.GetMember(data), Is.EqualTo(data.GetField()));
                var fieldValue = RandomInt();
                accessor.SetMember(data, fieldValue);
                Assert.That(data.GetField(), Is.EqualTo(fieldValue));
            }
        }

        public class BuildAccessorForEntityOfValueTypeTest: FieldAccessorBuilderTest
        {
            [Test]
            public void BuildAccessorForStructureField_ItCanReadAndWrite()
            {
                var field = GetField<Structure>();

                var accessor = FieldAccessorBuilder.BuildFor<Structure, int>(field, true);

                var source = new Structure { field = RandomInt() };
                Assert.That(accessor.GetMember(source), Is.EqualTo(source.field));
                var fieldValue = RandomInt();
                var result = accessor.SetMember(source, fieldValue);
                Assert.That(result.field, Is.EqualTo(fieldValue));
            }
        }

        public class BuildAccessorWithRestrictionsTest: FieldAccessorBuilderTest
        {
            [Test]
            public void BuildAccessorForPrivateField_CantGetAndSet()
            {
                var field = GetField<ClassWithPrivateField>();

                var result = FieldAccessorBuilder.BuildFor<ClassWithPrivateField, int>(field);

                Assert.That(result.CanGet, Is.False);
                Assert.That(result.CanSet, Is.False);
            }

            [Test]
            public void BuildAccessorForPublicReadonlyField_CantSet()
            {
                var field = GetField<ClassWithPublicReadonlyField>();

                var result = FieldAccessorBuilder.BuildFor<ClassWithPublicReadonlyField, int>(field);

                Assert.That(result.CanGet, Is.True);
                Assert.That(result.CanSet, Is.False);
            }

            [Test]
            public void BuildAccessorForInheritedInternalField_CantGetAndSet()
            {
                var field = GetField<ClassWithInheritedInternalField>();

                var result = FieldAccessorBuilder.BuildFor<ClassWithInheritedInternalField, int>(field);

                Assert.That(result.CanGet, Is.False);
                Assert.That(result.CanSet, Is.False);
            }
        }
    }
}