using JetBrains.Annotations;

namespace NClone.SpecificTypeReplicators
{
    /// <summary>
    /// Strategy of replicating (seep copying) instances of a single type.
    /// </summary>
    internal interface IReplicationStrategy
    {
        /// <summary>
        /// Replicate (deep copy) <paramref name="source"/>.
        /// </summary>
        object Replicate([NotNull] object source);
    }
}