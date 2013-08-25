using FakeItEasy;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.TypeReplication
{
    public class EntityReplicatorBuilderTest: TestBase
    {
        private EntityReplicatorBuilder entityReplicatorBuilder;

        protected override void SetUp()
        {
            base.SetUp();
            entityReplicatorBuilder = new EntityReplicatorBuilder(A.Fake<IMetadataProvider>(), A.Fake<IFieldCopiersBuilder>());
        }

        [Test]
        public void BuildReplicatorForString_GetTrivial()
        {
            var result = entityReplicatorBuilder.BuildFor<string>();

            Assert.That(result, Is.TypeOf<TrivialReplicator<string>>());
        }


        private enum TestEnum
        {
            One,
            Two,
            Three
        }

        [Test]
        public void BuildReplicatorForEnum_GetTrivial()
        {
            var result = entityReplicatorBuilder.BuildFor<TestEnum>();

            Assert.That(result, Is.TypeOf<TrivialReplicator<TestEnum>>());
        }

        [Test]
        public void BuildReplicatorForInt_GetTrivial()
        {
            var result = entityReplicatorBuilder.BuildFor<int>();

            Assert.That(result, Is.TypeOf<TrivialReplicator<int>>());
        }

        private struct TestStructure
        {
        }

        [Test]
        public void BuildReplicatorForStructure_GetStructureReplicator()
        {
            var result = entityReplicatorBuilder.BuildFor<TestStructure>();

            Assert.That(result, Is.TypeOf<StructureReplicator<TestStructure>>());
        }

        private class TestObject
        {
        }

        [Test]
        public void BuildReplicatorForObject_GetObjectReplicator()
        {
            var result = entityReplicatorBuilder.BuildFor<TestObject>();

            Assert.That(result, Is.TypeOf<ObjectReplicator<TestObject>>());
        }

        [Test]
        public void BuildReplicatorForSameTypeTwice_GetSameReplicator()
        {
            var result1 = entityReplicatorBuilder.BuildFor<TestObject>();
            var result2 = entityReplicatorBuilder.BuildFor<TestObject>();

            Assert.That(result2, Is.SameAs(result1));
        }
    }
}