namespace NClone.FieldCopying
{
    /// <summary>
    /// Object which is able to copy value of a specific field between containers of a specific type.
    /// Value is optionally replicated (deep copied) during copying.
    /// </summary>
    internal interface IFieldCopier
    {
        /// <summary>
        /// Copies a value of a field from <paramref name="source"/> to <paramref name="destination"/>
        /// and return modified <paramref name="destination"/>.
        /// Value of a field is replicated (deep copied) during copying if <see cref="Replicating"/> is <c>true</c>.
        /// </summary>
        //todo: exceptions
        object Copy(object source, object destination);

        /// <summary>
        /// Indicates whether this <see cref="IFieldCopier"/> performs replication (deep copying) of field value
        /// during <see cref="Copy"/>.
        /// </summary>
        bool Replicating { get; }
    }
}