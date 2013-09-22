using System;
using NClone.MetadataProviders;
using NClone.ObjectReplicators;
using NClone.Shared;
using NUnit.Framework;

namespace NClone.Tests.ObjectReplicators
{
    public class ObjectReplicatorIntegrationTest: TestBase
    {
        private ObjectReplicator replicator;

        protected override void SetUp()
        {
            base.SetUp();
            replicator = new ObjectReplicator(new DefaultMetadataProvider());
        }

        [Test]
        public void SourceContainsNullableFields_CanBeReplicated()
        {
            var source = new ClassWithNullables
                         {
                             field1 = null,
                             field2 = DateTime.Today
                         };

            object result = replicator.Replicate(source);

            Assert.That(result.As<ClassWithNullables>().field1, Is.Null);
            Assert.That(result.As<ClassWithNullables>().field2, Is.EqualTo(source.field2));
        }

        private class ClassWithNullables
        {
            public DateTime? field1;
            public DateTime? field2;
        }
    }
}