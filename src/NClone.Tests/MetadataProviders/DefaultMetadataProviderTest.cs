using System;
using System.Collections.Generic;
using System.Linq;
using NClone.MetadataProviders;
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

        #region GetPerTypeBehavior tests

        [Test]
        public void GetBehaviorForPrimitiveType_CopyOnlyReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (UInt64));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForEnum_CopyOnlyReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (Enum));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForString_CopyOnlyReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (string));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForNullableInt_CopyOnlyReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (int?));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForNullableStruct_ReplicateReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (SomeValueType?));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Replicate));
        }

        [Test]
        public void GetBehaviorForArray_ReplicateReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (char[]));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Replicate));
        }

        private enum Enum: long
        {
            A,
            B,
            C
        }

        private struct SomeValueType
        {
        }

        #endregion

        #region GetAllFields tests

        [Test]
        public void ClassWithPrivateField_GetFieldsReturnsIt()
        {
            DefaultMetadataProvider.CopyableFieldDescription[] result = TestingMetadataProvider.GetAllFields<ClassWithPrivateField>();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().BackingField, Is.SameAs(result.Single().DeclaringMember));
            Assert.That(result.Single().DeclaringMember.Name, Is.EqualTo("field"));
        }

        [Test]
        public void ClassWithPublicField_GetFieldsReturnsIt()
        {
            DefaultMetadataProvider.CopyableFieldDescription[] result = TestingMetadataProvider.GetAllFields<ClassWithPublicField>();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().BackingField, Is.SameAs(result.Single().DeclaringMember));
            Assert.That(result.Single().DeclaringMember.Name, Is.EqualTo("field"));
        }

        [Test]
        public void ClassWithStaticField_GetFieldsDoesNotReturnIt()
        {
            DefaultMetadataProvider.CopyableFieldDescription[] result =
                TestingMetadataProvider.GetAllFields<ClassWithStaticField>();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ClassWhichBaseHasPrivateField_GetFieldsReturnsIt()
        {
            DefaultMetadataProvider.CopyableFieldDescription[] result =
                TestingMetadataProvider.GetAllFields<ClassWithPrivateInheritedField>();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().BackingField, Is.SameAs(result.Single().DeclaringMember));
            Assert.That(result.Single().DeclaringMember.Name, Is.EqualTo("field"));
        }

        [Test]
        public void ClassWhichBaseHasPublicField_GetFieldsReturnsIt()
        {
            DefaultMetadataProvider.CopyableFieldDescription[] result =
                TestingMetadataProvider.GetAllFields<ClassWithPublicInheritedField>();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().BackingField, Is.SameAs(result.Single().DeclaringMember));
            Assert.That(result.Single().DeclaringMember.Name, Is.EqualTo("field"));
        }

        [Test]
        public void ClassWithPrivateAutoproperty_GetFieldsReturnsIt()
        {
            DefaultMetadataProvider.CopyableFieldDescription[] result =
                TestingMetadataProvider.GetAllFields<ClassWithPrivateProperty>();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().BackingField, Is.Not.SameAs(result.Single().DeclaringMember));
            Assert.That(result.Single().DeclaringMember.Name, Is.EqualTo("Property"));
            Assert.That(result.Single().BackingField.Name, Is.StringContaining("Property"));
        }

        [Test]
        public void ClassWithPublicAutoevent_GetFieldsReturnsBackingField()
        {
            DefaultMetadataProvider.CopyableFieldDescription[] result =
                TestingMetadataProvider.GetAllFields<ClassWithPublicEvent>();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.Single().BackingField, Is.SameAs(result.Single().DeclaringMember));
            Assert.That(result.Single().DeclaringMember.Name, Is.EqualTo("Event"));
        }

        private class ClassWithPrivateField
        {
            private readonly int field;
        }

        private class ClassWithPrivateInheritedField: ClassWithPrivateField
        {
        }

        private class ClassWithPrivateProperty
        {
            private int Property { get; set; }
        }

        private class ClassWithPublicEvent
        {
            public event Action Event;
        }

        private class ClassWithPublicField
        {
            public readonly int field;
        }

        private class ClassWithPublicInheritedField: ClassWithPublicField
        {
        }

        private class ClassWithStaticField
        {
            public static readonly int field;
        }

        private class TestingMetadataProvider: DefaultMetadataProvider
        {
            private readonly IList<CopyableFieldDescription> allFields = new List<CopyableFieldDescription>();

            public static CopyableFieldDescription[] GetAllFields<T>()
            {
                var spyProvider = new TestingMetadataProvider();
                spyProvider
                    .GetFieldsReplicationInfo(typeof (T))
                    .ToArray();
                return spyProvider.allFields.ToArray();
            }

            protected override ReplicationBehavior? TryGetPerMemberReplicationBehavior(CopyableFieldDescription fieldDescription)
            {
                allFields.Add(fieldDescription);
                return base.TryGetPerMemberReplicationBehavior(fieldDescription);
            }
        }

        #endregion

        #region GetFieldsReplicationInfo tests

        [Test]
        public void ClassWithFields_GetFieldsReplicationInfoReturnsAllFields()
        {
            FieldReplicationInfo result = metadataProvider.GetFieldsReplicationInfo(typeof (ClassWithPublicInheritedField)).Single();

            Assert.That(result.Behavior, Is.EqualTo(ReplicationBehavior.Replicate));
            Assert.That(result.Member.Name, Is.EqualTo("field"));
        }

        #endregion
    }
}