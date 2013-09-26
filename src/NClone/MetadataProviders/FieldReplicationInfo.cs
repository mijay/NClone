using System.Reflection;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Value-object for describing <see cref="FieldInfo"/> that should be affected during replication
    /// and per-member <see cref="ReplicationBehavior"/> assigned to it.
    /// </summary>
    public class FieldReplicationInfo
    {
        public FieldReplicationInfo(FieldInfo member, ReplicationBehavior behavior)
        {
            Member = member;
            Behavior = behavior;
        }

        public FieldInfo Member { get; private set; }
        public ReplicationBehavior Behavior { get; private set; }
    }
}