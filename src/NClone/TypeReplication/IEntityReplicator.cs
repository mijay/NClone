using System;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Object which is able to replicate (deep copy) an entity of type <see cref="EntityType"/>.
    /// </summary>
    internal interface IEntityReplicator
    {
        /// <summary>
        /// <see cref="Type"/> of entities processed by the current <see cref="IEntityReplicator"/>.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Replicate (deep copy) <paramref name="source"/>.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// If <paramref name="source"/> is not of type <see cref="EntityType"/>.
        /// </exception>
        object Replicate(object source);

        /// <summary>
        /// Indicates, whether current <see cref="IEntityReplicator"/> is trivial,
        /// i.e. whether its <see cref="Replicate"/> just returns given source,
        /// because copying can be omitted.
        /// </summary>
        /// <remarks><see cref="IsTrivial"/> is true for immutable entities.</remarks>
        bool IsTrivial { get; }
    }
}