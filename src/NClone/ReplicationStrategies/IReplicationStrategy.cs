using JetBrains.Annotations;
using NClone.ObjectReplicators;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Strategy of replicating instances of a single type.
    /// </summary>
    internal interface IReplicationStrategy
    {
        /// <summary>
        /// Apply current <see cref="IReplicationStrategy"/> to <paramref name="source"/>.
        /// </summary>
        object Replicate([NotNull] object source, IReplicationContext context);
    }
}