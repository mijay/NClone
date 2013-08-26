using System;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.ExtensionSyntax.Full;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.TypeReplication
{
    public class StructureReplicatorTest: TestBase
    {
        private IMetadataProvider metadataProvider;
        private IFieldCopiersBuilder fieldCopiersBuilder;

        protected override void SetUp()
        {
            base.SetUp();
            metadataProvider = A.Fake<IMetadataProvider>(x => x.Strict());
            fieldCopiersBuilder = A.Fake<IFieldCopiersBuilder>(x => x.Strict());
        }

        private StructureReplicator BuildReplicator<TType>()
        {
            return new StructureReplicator(metadataProvider, fieldCopiersBuilder, typeof (TType));
        }

        public class Class { }

        [Test]
        public void CreateStructureReplicatorForReferenceType_ExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => BuildReplicator<Class>());
        }

        public struct Structure
        {
            public int field1;
            public object field2;
        }

        [Test]
        public void ReplicateEntity_FieldCopierBuildAndCalledForEachReplicatingField()
        {
            var fakeMembers = typeof (Structure).GetFields();
            var fakeCopiers = Repeat.Twice(() => A.Fake<IFieldCopier>(x => x.Strict()));
            var fakeResult = new Structure { field1 = RandomInt() };
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (Structure)))
                .Returns(fakeMembers);
            fieldCopiersBuilder
                .Configure()
                .CallsTo(x => x.BuildFor(typeof (Structure), A<FieldInfo>.Ignored))
                .ReturnsLazily((Type _, FieldInfo fieldInfo) => fakeCopiers[Array.IndexOf(fakeMembers, fieldInfo)]);
            fakeCopiers[0]
                .Configure()
                .CallsTo(x => x.Replicating)
                .Returns(false);
            fakeCopiers[1]
                .CallsTo(x => x.Replicating)
                .Returns(true);
            fakeCopiers[1]
                .CallsTo(x => x.Copy(A<object>.Ignored, A<object>.Ignored))
                .ReturnsLazily(() => fakeResult);

            var objectReplicator = BuildReplicator<Structure>();
            var result = objectReplicator.Replicate(new Structure());

            Assert.That(result, Is.EqualTo(fakeResult));
        }

        [Test]
        public void ReplicateTwice_MetadataReadOnceAndCopiersBuiltOnce()
        {
            var fakeMember = typeof (Structure).GetFields().First();
            var fakeCopier = A.Fake<IFieldCopier>();
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (Structure)))
                .Returns(new[] { fakeMember })
                .NumberOfTimes(1);
            fieldCopiersBuilder
                .Configure()
                .CallsTo(x => x.BuildFor(typeof (Structure), fakeMember))
                .Returns(fakeCopier)
                .NumberOfTimes(1);

            var objectReplicator = BuildReplicator<Structure>();

            objectReplicator.Replicate(new Structure());
            objectReplicator.Replicate(new Structure());
        }
    }
}