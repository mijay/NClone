using System;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Factory that builds (and caches) <see cref="IReplicationStrategy"/> for the given <see cref="Type"/>.
    /// </summary>
    internal interface IReplicationStrategyFactory
    {
        /// <summary>
        /// Builds (or get from cache) <see cref="IReplicationStrategy"/> for <paramref name="type"/>.
        /// </summary>
        IReplicationStrategy StrategyForType(Type type);
    }
}