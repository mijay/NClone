using System;
using System.Collections.Generic;
using System.Linq;
using NClone.MetadataProviders;
using NClone.Tests.ExternalAssembly;
using NUnit.Framework;

namespace NClone.Tests.MetadataProviders
{
    public class DefaultMetadataProviderTest: TestBase
    {
        private DefaultMetadataProvider metadataProvider;

        protected override void SetUp()
        {
            base.SetUp();
            metadataProvider = new DefaultMetadataProvider();
        }

        #region GetBehavior tests

        [Test]
        public void GetBehaviorForPrimitiveType_GetCopyOnly()
        {
            ReplicationBehavior result = metadataProvider.GetBehavior(typeof (UInt64));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForEnum_GetCopyOnly()
        {
            ReplicationBehavior result = metadataProvider.GetBehavior(typeof (Enum));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForString_GetCopyOnly()
        {
            ReplicationBehavior result = metadataProvider.GetBehavior(typeof (string));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForObject_GetCopyOnly()
        {
            ReplicationBehavior result = metadataProvider.GetBehavior(typeof (object));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForNullableInt_GetCopyOnly()
        {
            ReplicationBehavior result = metadataProvider.GetBehavior(typeof (int?));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForNullableStruct_GetDeepCopy()
        {
            ReplicationBehavior result = metadataProvider.GetBehavior(typeof (SomeValueType?));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.DeepCopy));
        }

        [Test]
        public void GetBehaviorForArray_GetDeepCopy()
        {
            ReplicationBehavior result = metadataProvider.GetBehavior(typeof (char[]));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.DeepCopy));
        }

        private enum Enum: long
        {
            A,
            B,
            C
        }

        private struct SomeValueType { }

        #endregion

        #region GetMembers tests

        [Test]
        public void ClassWithPrivateField_GetMembersReturnsIt()
        {
            IEnumerable<MemberInformation> result = metadataProvider.GetMembers(typeof (ClassWithPrivateField));

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ClassWhichBaseHasPrivateField_GetMembersReturnsIt()
        {
            IEnumerable<MemberInformation> result = metadataProvider.GetMembers(typeof (ClassWithPrivateInheritedField));

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().Member.DeclaringType, Is.EqualTo(typeof (ClassWithPrivateField)));
        }

        [Test]
        public void ClassWhichBaseHasInternalField_GetMembersReturnsIt()
        {
            IEnumerable<MemberInformation> result = metadataProvider.GetMembers(typeof (ClassWithInternalInheritedField));

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().Member.DeclaringType, Is.EqualTo(typeof (ClassWithInternalReadonlyField)));
        }

        [Test]
        public void ClassWithPublicField_GetMembersReturnsIt()
        {
            IEnumerable<MemberInformation> result = metadataProvider.GetMembers(typeof (ClassWithPublicField));

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ClassThatHidesBaseField_GetMembersReturnsBoth()
        {
            IEnumerable<MemberInformation> result = metadataProvider.GetMembers(typeof (ClassWithNewField));

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Select(x => x.Member.DeclaringType),
                Is.EquivalentTo(new[] { typeof (ClassWithNewField), typeof (ClassWithPublicField) }));
        }

        private class ClassWithInternalInheritedField: ClassWithInternalReadonlyField
        {
            public ClassWithInternalInheritedField(): base(12) { }
        }

        private class ClassWithNewField: ClassWithPublicField
        {
            public new readonly int field;
        }

        private class ClassWithPrivateField
        {
            private readonly int field;
        }

        private class ClassWithPrivateInheritedField: ClassWithPrivateField { }

        private class ClassWithPublicField
        {
            public readonly int field;
        }

        #endregion
    }
}