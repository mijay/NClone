using System;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using NClone.FieldCopying;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.FieldCopying
{
    public class FieldCopiersBuilderTest: TestBase
    {
        #region Test data

        private class EmptyClass { }

        private class ClassWithField
        {
            public int field;
        }

        private class InheritedClass: ClassWithField { }

        #endregion

        private FieldCopiersBuilder fieldCopiersBuilder;
        private IEntityReplicatorsBuilder entityReplicatorBuilder;
        private readonly FieldInfo field = typeof (ClassWithField).GetFields().Single();

        protected override void SetUp()
        {
            base.SetUp();
            entityReplicatorBuilder = A.Fake<IEntityReplicatorsBuilder>();
            fieldCopiersBuilder = new FieldCopiersBuilder(new Lazy<IEntityReplicatorsBuilder>(() => entityReplicatorBuilder));
        }

        [Test]
        public void BuildCopierForFieldFromDifferentClass_Exception()
        {
            Assert.Throws<ArgumentException>(() => fieldCopiersBuilder.BuildFor<EmptyClass>(field));
        }

        [Test]
        public void BuildCopierForFieldFromParentClass_NoException()
        {
            Assert.DoesNotThrow(() => fieldCopiersBuilder.BuildFor<InheritedClass>(field));
        }
    }
}