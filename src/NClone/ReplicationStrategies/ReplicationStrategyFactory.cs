using System;
using System.Collections.Concurrent;
using NClone.MetadataProviders;
using NClone.Utils;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategyFactory"/>.
    /// </summary>
    internal class ReplicationStrategyFactory: IReplicationStrategyFactory
    {
        private readonly ConcurrentDictionary<Type, IReplicationStrategy> entityReplicators =
            new ConcurrentDictionary<Type, IReplicationStrategy>();

        private readonly IMetadataProvider metadataProvider;

        public ReplicationStrategyFactory(IMetadataProvider metadataProvider)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            this.metadataProvider = metadataProvider;
        }

        public IReplicationStrategy StrategyForType(Type type)
        {
            Guard.AgainstNull(type, "type");
            return entityReplicators.GetOrAdd(type, BuildEntityReplicator);
        }

        private IReplicationStrategy BuildEntityReplicator(Type type)
        {
            ReplicationBehavior behavior = metadataProvider.GetPerTypeBehavior(type);
            switch (behavior) {
                case ReplicationBehavior.Ignore:
                    return IgnoringReplicationStrategy.Instance;
                case ReplicationBehavior.Copy:
                    return CopyOnlyReplicationStrategy.Instance;
                case ReplicationBehavior.Replicate:
                    return new CommonReplicationStrategy(metadataProvider,
                        type.IsNullable() ? type.GetNullableUnderlyingType() : type);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}