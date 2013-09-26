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
            ReplicationBehavior result = metadataProvider.GetBehavior(typeof (Class));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Ignore));
        }

        [Test]
        public void WhenFieldIsMarkedWithAttribute_GetMembersReturnsBehaviorFromAttribute()
        {
            MemberInformation result = metadataProvider.GetMembers(typeof (Class)).Single();

            Assert.That(result.Behavior, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void WhenAutopropertyIsMarkedWithAttribute_GetMembersReturnsBehaviorFromAttribute()
        {
            MemberInformation result = metadataProvider.GetMembers(typeof (ClassWithProperty)).Single();

            Assert.That(result.Behavior, Is.EqualTo(ReplicationBehavior.Copy));
            Assert.That(result.Member.Name, Is.StringContaining("Property"));
        }

        [Test]
        public void WhenInheritedAutopropertyIsMarkedWithAttribute_GetMembersReturnsBehaviorFromAttribute()
        {
            MemberInformation result = metadataProvider.GetMembers(typeof (ClassWithInheritedProperty)).Single();

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
    }
}