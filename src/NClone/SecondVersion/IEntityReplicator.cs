using JetBrains.Annotations;

namespace NClone.SecondVersion
{
    /// <summary>
    /// Object which is able to replicate (deep copy) an entity of a specific type.
    /// </summary>
    internal interface IEntityReplicator
    {
        /// <summary>
        /// Replicate (deep copy) <paramref name="source"/>.
        /// </summary>
        object Replicate([NotNull] object source);
    }
}