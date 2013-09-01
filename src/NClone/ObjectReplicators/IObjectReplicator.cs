namespace NClone.ObjectReplicators
{
    /// <summary>
    /// Object which is able to replicate (deep copy) any arbitrary entity.
    /// </summary>
    public interface IObjectReplicator
    {
        /// <summary>
        /// Replicate (deep copy) <paramref name="source"/>.
        /// </summary>
        object Replicate(object source);
    }
}