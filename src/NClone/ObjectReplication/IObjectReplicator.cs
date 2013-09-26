namespace NClone.ObjectReplication
{
    /// <summary>
    /// Object which is able to replicate (deep copy) any arbitrary entity.
    /// </summary>
    public interface IObjectReplicator
    {
        /// <summary>
        /// Replicate (deep copy) <paramref name="source"/>.
        /// </summary>
        /// <exception cref="CircularReferenceFoundException">
        /// Is thrown when the reference cycle is found in source object graph.
        /// </exception>
        T Replicate<T>(T source);
    }
}