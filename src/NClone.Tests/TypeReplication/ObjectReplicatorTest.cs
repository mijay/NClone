using System;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax;
using NClone.Annotation;
using NClone.MemberCopying;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.TypeReplication
{
    public class ObjectReplicatorTest: TestBase
    {
        private IMetadataProvider metadataProvider;
        private IMemberCopierBuilder memberCopierBuilder;

        protected override void SetUp()
        {
            base.SetUp();
            metadataProvider = A.Fake<IMetadataProvider>(x => x.Strict());
            memberCopierBuilder = A.Fake<IMemberCopierBuilder>(x => x.Strict());
        }

        private ObjectReplicator<TType> BuildObjectReplicator<TType>()
        {
            return new ObjectReplicator<TType>(metadataProvider, memberCopierBuilder);
        }

        public struct Structure
        {
        }

        [Test]
        public void CreateObjectReplicatorForValueType_ExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => BuildObjectReplicator<Structure>());
        }

        public class ClassWithCtor
        {
            public bool ctorCalled;

            public ClassWithCtor()
            {
                ctorCalled = false;
            }
        }

        [Test]
        public void ReplicateEntityWithDefaultCtor_CtorIsNotCalled()
        {
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (ClassWithCtor)))
                .Returns(new FieldInfo[0]);
            var objectReplicator = BuildObjectReplicator<ClassWithCtor>();

            var result = objectReplicator.Replicate(new ClassWithCtor());

            Assert.That(result.ctorCalled, Is.False);
        }

        public class Class
        {
            public object field;
        }

        [Test]
        public void ReplicateEntity_MemberCopierBuildAndCalledForEachReplicatingField()
        {
            var fakeMember = typeof (Class).GetFields().Single();
            var fakeCopier = A.Fake<IMemberCopier<Class>>();
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (Class)))
                .Returns(new[] { fakeMember });
            memberCopierBuilder
                .Configure()
                .CallsTo(x => x.BuildFor<Class>(fakeMember))
                .Returns(fakeCopier);

            var objectReplicator = BuildObjectReplicator<Class>();

            objectReplicator.Replicate(new Class());
        }

        [Test]
        public void ReplicateTwice_MetadataReadedOnceAndCopiersBuildedOnce()
        {
            var fakeMember = typeof(Class).GetFields().Single();
            var fakeCopier = A.Fake<IMemberCopier<Class>>();
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof(Class)))
                .Returns(new[] { fakeMember })
                .NumberOfTimes(1);
            memberCopierBuilder
                .Configure()
                .CallsTo(x => x.BuildFor<Class>(fakeMember))
                .Returns(fakeCopier)
                .NumberOfTimes(1);

            var objectReplicator = BuildObjectReplicator<Class>();

            objectReplicator.Replicate(new Class());
            objectReplicator.Replicate(new Class());
        }
    }
}