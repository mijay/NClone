using System;

namespace NClone.Annotation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CustomReplicationBehaviorAttribute: Attribute
    {
        public ReplicationBehavior ReplicationBehavior { get; private set; }

        public CustomReplicationBehaviorAttribute(ReplicationBehavior replicationBehavior)
        {
            ReplicationBehavior = replicationBehavior;
        }
    }
}