using System;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.ExtensionSyntax.Full;
using NClone.Annotation;
using NClone.MemberCopying;
using NClone.Shared;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.TypeReplication
{
    public class StructureReplicatorTest: TestBase
    {
        private IMetadataProvider metadataProvider;
        private IMemberCopierBuilder memberCopierBuilder;

        protected override void SetUp()
        {
            base.SetUp();
            metadataProvider = A.Fake<IMetadataProvider>(x => x.Strict());
            memberCopierBuilder = A.Fake<IMemberCopierBuilder>(x => x.Strict());
        }

        private StructureReplicator<TType> BuildReplicator<TType>()
        {
            return new StructureReplicator<TType>(metadataProvider, memberCopierBuilder);
        }

        public class Class
        {
        }

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
        public void ReplicateEntity_MemberCopierBuildAndCalledForEachReplicatingField()
        {
            var fakeMembers = typeof (Structure).GetFields();
            var fakeCopiers = Repeat.Twice(() => A.Fake<IMemberCopier<Structure>>(x => x.Strict()));
            var fakeResult = new Structure { field1 = RandomInt() };
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (Structure)))
                .Returns(fakeMembers);
            memberCopierBuilder
                .Configure()
                .CallsTo(x => x.BuildFor<Structure>(A<FieldInfo>.Ignored))
                .ReturnsLazily((FieldInfo fieldInfo) => fakeCopiers[Array.IndexOf(fakeMembers, fieldInfo)]);
            fakeCopiers[0]
                .Configure()
                .CallsTo(x => x.Replicating)
                .Returns(false);
            fakeCopiers[1]
                .CallsTo(x => x.Replicating)
                .Returns(true);
            fakeCopiers[1]
                .CallsTo(x => x.Copy(A<Structure>.Ignored, A<Structure>.Ignored))
                .ReturnsLazily((Structure _, Structure __) => fakeResult);

            var objectReplicator = BuildReplicator<Structure>();
            var result = objectReplicator.Replicate(new Structure());

            Assert.That(result, Is.EqualTo(fakeResult));
        }

        [Test]
        public void ReplicateTwice_MetadataReadOnceAndCopiersBuiltOnce()
        {
            var fakeMember = typeof(Structure).GetFields().First();
            var fakeCopier = A.Fake<IMemberCopier<Structure>>();
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof(Structure)))
                .Returns(new[] { fakeMember })
                .NumberOfTimes(1);
            memberCopierBuilder
                .Configure()
                .CallsTo(x => x.BuildFor<Structure>(fakeMember))
                .Returns(fakeCopier)
                .NumberOfTimes(1);

            var objectReplicator = BuildReplicator<Structure>();

            objectReplicator.Replicate(new Structure());
            objectReplicator.Replicate(new Structure());
        }
    }
}