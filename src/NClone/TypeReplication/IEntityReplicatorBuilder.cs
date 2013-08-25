namespace NClone.TypeReplication
{
    /// <summary>
    /// Builds <see cref="IEntityReplicator{TType}"/> for a given actualType.
    /// </summary>
    internal interface IEntityReplicatorBuilder
    {
        /// <summary>
        /// Builds <see cref="IEntityReplicator{TType}"/> for the given <typeparamref name="TType"/>.
        /// </summary>
        IEntityReplicator<TType> BuildFor<TType>();
    }
}