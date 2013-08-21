using System;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NClone.MemberCopying;
using NClone.TypeReplication;
using NUnit.Framework;

namespace NClone.Tests.TypeReplication
{
    public class ObjectReplicatorTest: TestBase
    {
        private static ObjectReplicator<T> BuildReplicator<T>(Func<FieldInfo, IMemberCopier<T>> configuration)
        {
            var memberCopierBuilder = A.Fake<IMemberCopierBuilder>();
            memberCopierBuilder
                .CallsTo(x => x.BuildFor<T>(A<FieldInfo>.Ignored))
                .ReturnsLazily(configuration);
            return new ObjectReplicator<T>(memberCopierBuilder);
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
        public void ReplicateClassWithDefaultCtor_CtorIsNotCalled()
        {
            var objectReplicator = BuildReplicator(_ => A.Fake<IMemberCopier<ClassWithCtor>>());
            var source = new ClassWithCtor();

            var result = objectReplicator.Replicate(source);

            Assert.That(result.ctorCalled, Is.False);
        }

        [Test]
        public void ReplicateClassWithPublicField_FieldBuilderIsCreatedAndCalled()
        {
            Assert.Fail();
        }

        [Test]
        public void ReplicateClassWithPrivateField_FieldBuilderIsCreatedAndCalled()
        {
            Assert.Fail();
        }

        [Test]
        public void ReplicateClassWithProperty_NoFieldBuildersCreated()
        {
            Assert.Fail();
        }

        [Test]
        public void ReplicateClassWithNewField_TwoFieldBuilderCreatedAndCalled()
        {
            Assert.Fail();
        }

        [Test]
        public void ReplicateTwice_FieldBuilderCalledOnce()
        {
            Assert.Fail();
        }
    }
}