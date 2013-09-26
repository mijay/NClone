using NClone.ObjectReplication;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Defines how <see cref="ObjectReplicator"/> should treat specific types/members.
    /// </summary>
    public enum ReplicationBehavior
    {
        /// <summary>
        /// Ignore during replication: default value used for fields, default value returned for types.
        /// </summary>
        Ignore,

        /// <summary>
        /// Value should be copied, no replication occur.
        /// </summary>
        Copy,

        /// <summary>
        /// Value should be replicated (deep copied).
        /// </summary>
        Replicate
    }
}