namespace NClone.Annotation
{
    /// <summary>
    /// Defines how <see cref="DefaultObjectReplicator"/> should treat specific types/members.
    /// </summary>
    public enum ReplicationBehavior
    {
        /// <summary>
        /// Ignore during replication: default value used for fields, default value returned for types.
        /// </summary>
        Ignore,

        /// <summary>
        /// Value should be copied, no deep-copy occur.
        /// </summary>
        Copy,

        /// <summary>
        /// value should be deep-copied.
        /// </summary>
        DeepCopy
    }
}