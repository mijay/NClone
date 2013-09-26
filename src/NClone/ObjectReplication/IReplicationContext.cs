namespace NClone.ObjectReplication
{
    /// <summary>
    /// Represents one context of replication, which tracks replicated objects,
    /// reuses them in case of receiving the same source (e.g. when one object is referenced twice in source object graph),
    /// and detects circular references.
    /// </summary>
    internal interface IReplicationContext
    {
        /// <summary>
        /// Replicate <paramref name="source"/> or return its cached replica.
        /// </summary>
        /// <exception cref="CircularReferenceFoundException">
        /// Is thrown when the reference cycle is found in source object graph.
        /// </exception>
        object Replicate(object source);
    }
}