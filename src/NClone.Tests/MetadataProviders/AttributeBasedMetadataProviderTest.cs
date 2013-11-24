using System;
using System.Linq;
using NClone.MetadataProviders;
using NUnit.Framework;

namespace NClone.Tests.MetadataProviders
{
    public class AttributeBasedMetadataProviderTest: TestBase
    {
        private AttributeBasedMetadataProvider metadataProvider;

        protected override void SetUp()
        {
            base.SetUp();
            metadataProvider = new AttributeBasedMetadataProvider();
        }

        [Test]
        public void WhenTypeIsMarkedWithAttribute_GetBehaviorReturnsBehaviorFromAttribute()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (Class));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Ignore));
        }

        [Test]
        public void WhenFieldIsMarkedWithAttribute_GetMembersReturnsBehaviorFromAttribute()
        {
            FieldReplicationInfo result = metadataProvider.GetFieldsReplicationInfo(typeof (Class)).Single();

            Assert.That(result.Behavior, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void WhenAutopropertyIsMarkedWithAttribute_GetMembersReturnsBehaviorFromAttribute()
        {
            FieldReplicationInfo result = metadataProvider.GetFieldsReplicationInfo(typeof (ClassWithProperty)).Single();

            Assert.That(result.Behavior, Is.EqualTo(ReplicationBehavior.Copy));
            Assert.That(result.Member.Name, Is.StringContaining("Property"));
        }

        [Test]
        public void WhenAutoeventFieldIsMarkedWithAttribute_GetMembersReturnsBehaviorFromAttribute()
        {
            FieldReplicationInfo result = metadataProvider.GetFieldsReplicationInfo(typeof(ClassWithEvent)).Single();

            Assert.That(result.Behavior, Is.EqualTo(ReplicationBehavior.Copy));
            Assert.That(result.Member.Name, Is.StringContaining("Event"));
        }

        [Test]
        public void WhenInheritedAutopropertyIsMarkedWithAttribute_GetMembersReturnsBehaviorFromAttribute()
        {
            FieldReplicationInfo result = metadataProvider.GetFieldsReplicationInfo(typeof (ClassWithInheritedProperty)).Single();

            Assert.That(result.Behavior, Is.EqualTo(ReplicationBehavior.Copy));
            Assert.That(result.Member.Name, Is.StringContaining("Property"));
        }

        [CustomReplicationBehavior(ReplicationBehavior.Ignore)]
        private class Class
        {
            [CustomReplicationBehavior(ReplicationBehavior.Copy)]
            private readonly int field;
        }

        private class ClassWithInheritedProperty: ClassWithProperty { }

        private class ClassWithProperty
        {
            [CustomReplicationBehavior(ReplicationBehavior.Copy)]
            private int Property { get; set; }
        }

        private class ClassWithEvent
        {
            [CustomReplicationBehavior(ReplicationBehavior.Copy)]
            public event Action Event;
        }
    }
}