using System;
using System.Linq;
using System.Reflection;
using NClone.MemberCopying;
using NClone.Tests.ExternalAssembly;
using NUnit.Framework;

namespace NClone.Tests.MemberCopying
{
    public class MemberCopyingBuilderTest: TestBase
    {
        private MemberCopierBuilder memberCopierBuilder;

        protected override void SetUp()
        {
            base.SetUp();
            memberCopierBuilder = new MemberCopierBuilder();
        }

        private FieldInfo GetField<TEntity>()
        {
            return typeof (TEntity).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Single();
        }

        public class ArgumentValidationTest: MemberCopyingBuilderTest
        {
            private class Class1
            {
            }

            private class Class2
            {
                public int field;
            }

            private class Class3: Class2
            {
            }

            [Test]
            public void BuildCopierForFieldFromDifferentClass_Exception()
            {
                Assert.Throws<ArgumentException>(() => memberCopierBuilder.BuildFor<Class1>(GetField<Class2>()));
            }

            [Test]
            public void BuildCopierForFieldFromParentClass_NoException()
            {
                Assert.DoesNotThrow(() => memberCopierBuilder.BuildFor<Class3>(GetField<Class2>()));
            }
        }

        public class CopyingFromEntityOfReferenceTypeTest: MemberCopyingBuilderTest
        {
            public class Class1
            {
                public int field;
            }

            [Test]
            public void BuildCopierForPublicField_CopierWorks()
            {
                var source = new Class1() { field = RandomInt() };

                var result = memberCopierBuilder.BuildFor<Class1>(GetField<Class1>()).Copy(source, new Class1());

                Assert.That(result.field, Is.EqualTo(source.field));
            }

            public class Class2
            {
                public Class2(int field)
                {
                    this.field = field;
                }

                private int field;

                public int GetField()
                {
                    return field;
                }
            }

            [Test]
            public void BuildCopierForPrivateField_CopierWorks()
            {
                var source = new Class2(RandomInt());

                var result = memberCopierBuilder.BuildFor<Class2>(GetField<Class2>()).Copy(source, new Class2(0));

                Assert.That(result.GetField(), Is.EqualTo(source.GetField()));
            }

            public class Class3
            {
                public Class3(int field)
                {
                    this.field = field;
                }

                public readonly int field;
            }

            [Test]
            public void BuildCopierForReadonlyField_CopierWorks()
            {
                var source = new Class3(RandomInt());

                var result = memberCopierBuilder.BuildFor<Class3>(GetField<Class3>()).Copy(source, new Class3(0));

                Assert.That(result.field, Is.EqualTo(source.field));
            }

            public class Class4
            {
                private readonly int field;

                public Class4(int field)
                {
                    this.field = field;
                }

                public int GetField()
                {
                    return field;
                }
            }

            public class Class5: ClassWithInternalReadonlyField
            {
                public Class5(int field): base(field)
                {
                }
            }

            [Test]
            public void BuildCopierForInternalReadonlyFieldOfBase_CopierWorks()
            {
                var source = new Class5(RandomInt());

                var result = memberCopierBuilder.BuildFor<Class5>(GetField<ClassWithInternalReadonlyField>()).Copy(source, new Class5(0));

                Assert.That(result.GetField(), Is.EqualTo(source.GetField()));
            }
        }

        public class CopyingFromEntryOfReferenceTypeTest: MemberCopyingBuilderTest
        {
            public struct Struct1
            {
                public int field;
            }

            [Test]
            public void BuildCopierForPublicField_CopierWorks()
            {
                var source = new Struct1 { field = RandomInt() };

                var result = memberCopierBuilder.BuildFor<Struct1>(GetField<Struct1>()).Copy(source, new Struct1());

                Assert.That(result.field, Is.EqualTo(source.field));
            }
        }

        public class ReplicationTest: MemberCopyingBuilderTest
        {
            [Test]
            public void BuildCopierForReferenceTypeField_ItIsReplicating()
            {
                Assert.Fail();
            }
        }
    }
}