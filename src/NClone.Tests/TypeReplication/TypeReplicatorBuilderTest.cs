using FakeItEasy;
using NClone.Annotation;
using NClone.MemberCopying;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.TypeReplication
{
    public class TypeReplicatorBuilderTest: TestBase
    {
        private TypeReplicatorBuilder typeReplicatorBuilder;

        protected override void SetUp()
        {
            base.SetUp();
            typeReplicatorBuilder = new TypeReplicatorBuilder(A.Fake<IMetadataProvider>(), A.Fake<IMemberCopierBuilder>());
        }

        [Test]
        public void BuildReplicatorForString_GetTrivial()
        {
            var result = typeReplicatorBuilder.BuildFor<string>();

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
            var result = typeReplicatorBuilder.BuildFor<TestEnum>();

            Assert.That(result, Is.TypeOf<TrivialReplicator<TestEnum>>());
        }

        [Test]
        public void BuildReplicatorForInt_GetTrivial()
        {
            var result = typeReplicatorBuilder.BuildFor<int>();

            Assert.That(result, Is.TypeOf<TrivialReplicator<int>>());
        }

        private struct TestStructure
        {
        }

        [Test]
        public void BuildReplicatorForStructure_GetStructureReplicator()
        {
            var result = typeReplicatorBuilder.BuildFor<TestStructure>();

            Assert.That(result, Is.TypeOf<StructureReplicator<TestStructure>>());
        }

        private class TestObject
        {
        }

        [Test]
        public void BuildReplicatorForObject_GetObjectReplicator()
        {
            var result = typeReplicatorBuilder.BuildFor<TestObject>();

            Assert.That(result, Is.TypeOf<ObjectReplicator<TestObject>>());
        }

        [Test]
        public void BuildReplicatorForSameTypeTwice_GetSameReplicator()
        {
            var result1 = typeReplicatorBuilder.BuildFor<TestObject>();
            var result2 = typeReplicatorBuilder.BuildFor<TestObject>();

            Assert.That(result2, Is.SameAs(result1));
        }
    }
}