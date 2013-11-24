using System;
using System.Linq;
using NClone.MetadataProviders;
using NUnit.Framework;

namespace NClone.Tests.MetadataProviders
{
    public class ConventionalMetadataProviderTest: TestBase
    {
        private ConventionalMetadataProvider metadataProvider;

        protected override void SetUp()
        {
            base.SetUp();
            metadataProvider = new ConventionalMetadataProvider();
        }

        [Test]
        public void GetBehaviorForDelegate_IgnoreReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (Func<int>));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Ignore));
        }

        [Test]
        public void GetBehaviorForStruct_CopyReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (ConsoleKeyInfo));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.Copy));
        }

        [Test]
        public void GetBehaviorForAnnotatedStruct_BehaviorFromAnnotationReturned()
        {
            ReplicationBehavior result = metadataProvider.GetPerTypeBehavior(typeof (AnnotatedStruct));

            Assert.That(result, Is.EqualTo(ReplicationBehavior.DeepCopy));
        }

        [Test]
        public void GetBehaviorForLazyEnumerable_ExceptionThrown()
        {
            TestDelegate action = () => metadataProvider.GetPerTypeBehavior(new[] { 0 }.Select(x => x).GetType());

            Assert.That(action, Throws.InstanceOf<LazyTypeFoundException>());
        }

        [CustomReplicationBehavior(ReplicationBehavior.DeepCopy)]
        private struct AnnotatedStruct { }
    }
}