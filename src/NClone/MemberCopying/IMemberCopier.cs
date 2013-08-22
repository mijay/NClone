namespace NClone.MemberCopying
{
    /// <summary>
    /// Object which is able to copy value of a single field between containers of type <typeparamref name="TContainer"/>.
    /// Value is optionally replicated (deep copied) during copying.
    /// </summary>
    internal interface IMemberCopier<TContainer>
    {
        /// <summary>
        /// Copies a value of a single field from <paramref name="source"/> to <paramref name="destination"/>
        /// and return modified <paramref name="destination"/>.
        /// Value of a field is replicated (deep copied) during copying if <see cref="PerformsReplication"/> is <c>false</c>.
        /// </summary>
        TContainer Copy(TContainer source, TContainer destination);

        /// <summary>
        /// Indicates whether this <see cref="IMemberCopier{TContainer}"/> performs replication (deep copying) of field value
        /// during <see cref="Copy"/>.
        /// </summary>
        bool PerformsReplication { get; }
    }
}