using System.Collections.Generic;
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
            IEnumerable<MemberInformation> result = metadataProvider.GetMembers(typeof (Class));

            Assert.That(result.Single().Behavior, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [CustomReplicationBehavior(ReplicationBehavior.Ignore)]
        private class Class
        {
            [CustomReplicationBehavior(ReplicationBehavior.Copy)]
            private readonly int field;
        }
    }
}