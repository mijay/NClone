namespace NClone.MemberCopying
{
    /// <summary>
    /// Object which is able to copy value of a single field between entities of type <typeparamref name="TEntity"/>.
    /// Value is optionally replicated (deep copied) during copying.
    /// </summary>
    internal interface IMemberCopier<TEntity>
    {
        /// <summary>
        /// Copies a value of a single field from <paramref name="source"/> to <paramref name="destination"/>
        /// and return modified <paramref name="destination"/>.
        /// Value of a field is replicated (deep copied) during copying if <see cref="Replicate"/> is <c>true</c>.
        /// </summary>
        TEntity Copy(TEntity source, TEntity destination);

        /// <summary>
        /// Indicates whether this <see cref="IMemberCopier{TContainer}"/> performs replication (deep copying) of field value
        /// during <see cref="Copy"/>.
        /// </summary>
        bool Replicate { get; }
    }
}