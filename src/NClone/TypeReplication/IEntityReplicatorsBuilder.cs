using System;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Builds <see cref="IEntityReplicator"/> for a given actualType.
    /// </summary>
    internal interface IEntityReplicatorsBuilder
    {
        /// <summary>
        /// Builds <see cref="IEntityReplicator"/> for the given <paramref name="entityType"/>.
        /// </summary>
        IEntityReplicator BuildFor(Type entityType);
    }
}