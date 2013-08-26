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
            var result = entityReplicatorsBuilder.BuildFor(typeof(string));

            Assert.That(result, Is.TypeOf<TrivialReplicator>());
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
            var result = entityReplicatorsBuilder.BuildFor(typeof(TestEnum));

            Assert.That(result, Is.TypeOf<TrivialReplicator>());
        }

        [Test]
        public void BuildReplicatorForInt_GetTrivial()
        {
            var result = entityReplicatorsBuilder.BuildFor(typeof(int));

            Assert.That(result, Is.TypeOf<TrivialReplicator>());
        }

        private struct TestStructure
        {
        }

        [Test]
        public void BuildReplicatorForStructure_GetStructureReplicator()
        {
            var result = entityReplicatorsBuilder.BuildFor(typeof(TestStructure));

            Assert.That(result, Is.TypeOf<StructureReplicator>());
            Assert.That(result.EntityType, Is.EqualTo(typeof(TestStructure)));
        }

        private class TestObject
        {
        }

        [Test]
        public void BuildReplicatorForObject_GetObjectReplicator()
        {
            var result = entityReplicatorsBuilder.BuildFor(typeof(TestObject));

            Assert.That(result, Is.TypeOf<ObjectReplicator>());
            Assert.That(result.EntityType, Is.EqualTo(typeof(TestObject)));
        }

        [Test]
        public void BuildReplicatorForSameTypeTwice_GetSameReplicator()
        {
            var result1 = entityReplicatorsBuilder.BuildFor(typeof(TestObject));
            var result2 = entityReplicatorsBuilder.BuildFor(typeof(TestObject));
            var result3 = entityReplicatorsBuilder.BuildFor(typeof(TestEnum));

            Assert.That(result2, Is.SameAs(result1));
            Assert.That(result3, Is.Not.SameAs(result1));
        }
    }
}