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
                case ReplicationBehavior.DeepCopy:
                    if (type.IsArray)
                        return BuildArrayReplicator(type);
                    if (type.IsNullable())
                        return new CommonReplicationStrategy(metadataProvider, type.GetNullableUnderlyingType());
                    return new CommonReplicationStrategy(metadataProvider, type);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IReplicationStrategy BuildArrayReplicator(Type type)
        {
            Type elementType = type.GetElementType();
            switch (metadataProvider.GetPerTypeBehavior(elementType)) {
                case ReplicationBehavior.Ignore:
                    return IgnoringReplicationStrategy.Instance;
                case ReplicationBehavior.Copy:
                    return CopyArrayReplicationStrategy.Instance;
                case ReplicationBehavior.DeepCopy:
                    return new CloneArrayReplicationStrategy(elementType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}