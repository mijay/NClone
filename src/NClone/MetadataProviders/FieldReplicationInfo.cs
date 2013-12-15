using System.Reflection;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Value-object for describing <see cref="FieldInfo"/> that should be affected during replication
    /// and per-member <see cref="ReplicationBehavior"/> assigned to it.
    /// </summary>
    public class FieldReplicationInfo
    {
        /// <summary>
        /// Constructor for <see cref="FieldReplicationInfo"/>
        /// </summary>
        public FieldReplicationInfo(FieldInfo field, ReplicationBehavior behavior)
        {
            Field = field;
            Behavior = behavior;
        }

        /// <summary>
        /// Field that should be affected during replication.
        /// </summary>
        public FieldInfo Field { get; private set; }

        /// <summary>
        /// <see cref="ReplicationBehavior"/> that should be applied to <see cref="Field"/>.
        /// </summary>
        public ReplicationBehavior Behavior { get; private set; }
    }
}