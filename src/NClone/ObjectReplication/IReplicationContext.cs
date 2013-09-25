namespace NClone.ObjectReplication
{
    /// <summary>
    /// Represents one context of replication, which tracks replicated objects and resolves circular references.
    /// </summary>
    internal interface IReplicationContext
    {
        /// <summary>
        /// Replicate <paramref name="source"/> or return its cached replica.
        /// </summary>
        object Replicate(object source);
    }
}