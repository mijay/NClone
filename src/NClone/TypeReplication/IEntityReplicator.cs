using System;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Object which is able to replicate (deep copy) an entity of type <typeparamref name="TType"/>.
    /// </summary>
    internal interface IEntityReplicator<TType>
    {
        /// <summary>
        /// Replicate (deep copy) <paramref name="source"/>.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// If <paramref name="source"/> is not of type <typeparamref name="TType"/>.
        /// </exception>
        TType Replicate(TType source);

        /// <summary>
        /// Indicates, whether current <see cref="IEntityReplicator{TType}"/> is trivial,
        /// i.e. whether its <see cref="Replicate"/> just returns given source,
        /// because copying can be omitted.
        /// </summary>
        /// <remarks><see cref="IsTrivial"/> is true for immutable entities.</remarks>
        bool IsTrivial { get; }
    }
}