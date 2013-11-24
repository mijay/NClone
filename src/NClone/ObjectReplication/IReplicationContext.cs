namespace NClone.ObjectReplication
{
    /// <summary>
    /// Represents the context of replication of a single object graph. It tracks cloned objects to preserve
    /// object graph topology in cloned graph (e.g. when one object is referenced twice in source object graph,
    /// it should also be reference twice in resulting graph), and detects reference cycles.
    /// </summary>
    internal interface IReplicationContext
    {
        /// <summary>
        /// Replicate <paramref name="source"/> or return its cached replica.
        /// </summary>
        /// <exception cref="CircularReferenceFoundException">
        /// Is thrown when the reference cycle is found in the source object graph.
        /// </exception>
        object Replicate(object source);
    }
}