using FakeItEasy;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.TypeReplication
{
    public class EntityReplicatorsBuilderTest: TestBase
    {
        private EntityReplicatorsBuilder entityReplicatorsBuilder;

        protected override void SetUp()
        {
            base.SetUp();
            entityReplicatorsBuilder = new EntityReplicatorsBuilder(A.Fake<IMetadataProvider>(), A.Fake<IFieldCopiersBuilder>());
        }

        [Test]
        public void BuildReplicatorForString_GetTrivial()
        {
            var result = entityReplicatorsBuilder.BuildFor<string>();

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
            var result = entityReplicatorsBuilder.BuildFor<TestEnum>();

            Assert.That(result, Is.TypeOf<TrivialReplicator<TestEnum>>());
        }

        [Test]
        public void BuildReplicatorForInt_GetTrivial()
        {
            var result = entityReplicatorsBuilder.BuildFor<int>();

            Assert.That(result, Is.TypeOf<TrivialReplicator<int>>());
        }

        private struct TestStructure
        {
        }

        [Test]
        public void BuildReplicatorForStructure_GetStructureReplicator()
        {
            var result = entityReplicatorsBuilder.BuildFor<TestStructure>();

            Assert.That(result, Is.TypeOf<StructureReplicator<TestStructure>>());
        }

        private class TestObject
        {
        }

        [Test]
        public void BuildReplicatorForObject_GetObjectReplicator()
        {
            var result = entityReplicatorsBuilder.BuildFor<TestObject>();

            Assert.That(result, Is.TypeOf<ObjectReplicator<TestObject>>());
        }

        [Test]
        public void BuildReplicatorForSameTypeTwice_GetSameReplicator()
        {
            var result1 = entityReplicatorsBuilder.BuildFor<TestObject>();
            var result2 = entityReplicatorsBuilder.BuildFor<TestObject>();
            var result3 = entityReplicatorsBuilder.BuildFor<TestEnum>();

            Assert.That(result2, Is.SameAs(result1));
            Assert.That(result3, Is.Not.SameAs(result1));
        }
    }
}