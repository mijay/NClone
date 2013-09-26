using System;

namespace NClone.MetadataProviders
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CustomReplicationBehaviorAttribute: Attribute
    {
        private readonly ReplicationBehavior replicationBehavior;

        public CustomReplicationBehaviorAttribute(ReplicationBehavior replicationBehavior)
        {
            this.replicationBehavior = replicationBehavior;
        }

        public ReplicationBehavior GetReplicationBehavior()
        {
            return replicationBehavior;
        }
    }
}