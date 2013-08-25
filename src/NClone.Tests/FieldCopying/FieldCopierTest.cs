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

        public class InheritedClass: ClassWithSimpleField { }

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

        private void ConfigureBuilderToReturn<TEntity>(IEntityReplicator<TEntity> entityReplicator)
        {
            entityReplicatorBuilder
                .Configure()
                .CallsTo(x => x.BuildFor<TEntity>())
                .Returns(entityReplicator);
        }

        private static IEntityReplicator<TEntity> BuildEntityReplicator<TEntity>(bool isTrivial = false)
        {
            var entityReplicator = A.Fake<IEntityReplicator<TEntity>>(x => x.Strict());
            entityReplicator
                .Configure()
                .CallsTo(x => x.IsTrivial)
                .Returns(isTrivial);
            return entityReplicator;
        }

        private FieldCopier<TEntity, TMember> BuildFieldCopier<TEntity, TMember>()
        {
            var field = typeof (TEntity)
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Single(x => x.FieldType == typeof (TMember));
            return new FieldCopier<TEntity, TMember>(entityReplicatorBuilder,
                FieldAccessorBuilder.BuildFor<TEntity, TMember>(field));
        }

        [Test]
        public void CreateFieldCopierForTypeWithTrivialReplicator_ItIsNotReplicating()
        {
            var entityReplicator = BuildEntityReplicator<ClassWithSimpleField>(true);
            ConfigureBuilderToReturn(entityReplicator);

            var fieldCopier = BuildFieldCopier<ClassWithField, ClassWithSimpleField>();
            Assert.That(fieldCopier.Replicating, Is.False);

            var source = new ClassWithField { field = new ClassWithSimpleField() };
            var result = fieldCopier.Copy(source, new ClassWithField());
            Assert.That(result.field, Is.SameAs(source.field));
        }

        [Test]
        public void CopierForFieldWithReplicator_CopyNull_ItIsNotReplicated()
        {
            var entityReplicator = BuildEntityReplicator<ClassWithSimpleField>();
            ConfigureBuilderToReturn(entityReplicator);

            var fieldCopier = BuildFieldCopier<ClassWithField, ClassWithSimpleField>();

            var result = fieldCopier.Copy(new ClassWithField { field = null }, new ClassWithField());
            Assert.That(result.field, Is.Null);
        }

        [Test]
        public void CreateFieldCopierForTypeWithReplicator_ReplicatingIsTrue()
        {
            var entityReplicator = BuildEntityReplicator<ClassWithSimpleField>();
            ConfigureBuilderToReturn(entityReplicator);

            var fieldCopier = BuildFieldCopier<ClassWithField, ClassWithSimpleField>();

            Assert.That(fieldCopier.Replicating, Is.True);
        }

        [Test]
        public void CopierForTypeWithReplicator_CopyField_ReplicateIsCalled()
        {
            var entityReplicator = BuildEntityReplicator<ClassWithSimpleField>();
            ConfigureBuilderToReturn(entityReplicator);

            var source = new ClassWithField { field = new ClassWithSimpleField { field = RandomInt() } };
            var fakeReplicatedField = new ClassWithSimpleField { field = RandomInt() };
            entityReplicator
                .Configure()
                .CallsTo(x => x.Replicate(source.field))
                .Returns(fakeReplicatedField);

            var fieldCopier = BuildFieldCopier<ClassWithField, ClassWithSimpleField>();
            var result = fieldCopier.Copy(source, new ClassWithField());

            Assert.That(result.field, Is.SameAs(fakeReplicatedField));
        }

        [Test]
        public void CopierForFieldOfTypeT_CopyFieldWithSuccessorOfT_ReplicateIsBuiltAndCalledForSuccessor()
        {
            var replicatorOfT = BuildEntityReplicator<ClassWithSimpleField>();
            var replicatorOfSuccessor = BuildEntityReplicator<InheritedClass>();
            ConfigureBuilderToReturn(replicatorOfT);
            ConfigureBuilderToReturn(replicatorOfSuccessor);

            var sourceField = new InheritedClass { field = RandomInt() };
            var source = new ClassWithField { field = sourceField };
            var fakeReplicatedField = new InheritedClass { field = RandomInt() };
            replicatorOfSuccessor
                .Configure()
                .CallsTo(x => x.Replicate(sourceField))
                .Returns(fakeReplicatedField);

            var fieldCopier = BuildFieldCopier<ClassWithField, ClassWithSimpleField>();
            var result = fieldCopier.Copy(source, new ClassWithField());

            Assert.That(result.field, Is.SameAs(fakeReplicatedField));
        }
    }
}