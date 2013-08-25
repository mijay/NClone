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
        #region Test data

        public struct Structure { }

        public class ClassWithCtor
        {
            public bool ctorCalled;

            public ClassWithCtor()
            {
                ctorCalled = false;
            }
        }

        public class Class
        {
            public object field;
        }

        public class InheritedClass: Class { }

        #endregion

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

        [Test]
        public void CreateObjectReplicatorForValueType_ExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => BuildObjectReplicator<Structure>());
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

        [Test]
        public void ReplicateEntity_MemberCopierBuildAndCalledForEachField()
        {
            var fakeMember = typeof (Class).GetFields().Single();
            var fakeCopier = A.Fake<IMemberCopier<Class>>(x => x.Strict());
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (Class)))
                .Returns(new[] { fakeMember });
            memberCopierBuilder
                .Configure()
                .CallsTo(x => x.BuildFor<Class>(fakeMember))
                .Returns(fakeCopier);
            fakeCopier
                .Configure()
                .CallsTo(x => x.Copy(A<Class>.Ignored, A<Class>.Ignored));

            var objectReplicator = BuildObjectReplicator<Class>();

            objectReplicator.Replicate(new Class());
        }

        [Test]
        public void ReplicateTwice_MetadataReadOnceAndCopiersBuiltOnce()
        {
            var fakeMember = typeof (Class).GetFields().Single();
            var fakeCopier = A.Fake<IMemberCopier<Class>>();
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (Class)))
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

        [Test]
        public void ReplicateEntityOfInheritedType_Exception()
        {
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (Class)))
                .Returns(new FieldInfo[0]);
            var objectReplicator = BuildObjectReplicator<Class>();

            Assert.Throws<InvalidCastException>(() => objectReplicator.Replicate(new InheritedClass()));
        }

        [Test]
        public void ReplicateNull_NullReturned()
        {
            metadataProvider
                .Configure()
                .CallsTo(x => x.GetReplicatingMembers(typeof (Class)))
                .Returns(new FieldInfo[0]);
            var objectReplicator = BuildObjectReplicator<Class>();

            Assert.That(objectReplicator.Replicate(null), Is.Null);
        }
    }
}