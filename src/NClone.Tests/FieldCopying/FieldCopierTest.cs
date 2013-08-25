using System.Linq;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax;
using NClone.FieldCopying;
using NClone.MemberAccess;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.FieldCopying
{
    public class FieldCopierTest: TestBase
    {
        #region Test data

        public class ClassWithSimpleField
        {
            public int field;
        }

        public class ClassWithField
        {
            public ClassWithSimpleField field;
        }

        #endregion

        private IEntityReplicatorsBuilder entityReplicatorBuilder;

        protected override void SetUp()
        {
            base.SetUp();
            entityReplicatorBuilder = A.Fake<IEntityReplicatorsBuilder>(x => x.Strict());
        }

        private FieldCopier<TEntity, TMember> BuildMemberCopier<TEntity, TMember>()
        {
            var field = typeof (TEntity)
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Single(x => x.FieldType == typeof (TMember));
            return new FieldCopier<TEntity, TMember>(entityReplicatorBuilder,
                FieldAccessorBuilder.BuildFor<TEntity, TMember>(field));
        }

        [Test]
        public void CreateMemberCopierForTypeWithRedundantReplicator_ItWontReplicate()
        {
            var entityReplicator = A.Fake<IEntityReplicator<ClassWithSimpleField>>(x => x.Strict());
            entityReplicatorBuilder
                .Configure()
                .CallsTo(x => x.BuildFor<ClassWithSimpleField>())
                .Returns(entityReplicator)
                .NumberOfTimes(1);
            entityReplicator
                .Configure()
                .CallsTo(x => x.IsTrivial)
                .Returns(true);

            var memberCopier = BuildMemberCopier<ClassWithField, ClassWithSimpleField>();

            Assert.That(memberCopier.Replicating, Is.False);
            Assert.DoesNotThrow(() => memberCopier.Copy(new ClassWithField { field = new ClassWithSimpleField() }, new ClassWithField()));
        }

        [Test]
        public void Member()
        {
            
        }
    }
}