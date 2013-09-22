using System;
using System.Collections.Concurrent;
using NClone.MetadataProviders;
using NClone.ReplicationStrategies;
using NClone.Shared;

namespace NClone.ObjectReplicators
{
    /// <summary>
    /// Implementation of <see cref="IObjectReplicator"/>.
    /// </summary>
    public class ObjectReplicator: IObjectReplicator
    {
        private readonly ConcurrentDictionary<Type, IReplicationStrategy> entityReplicators =
            new ConcurrentDictionary<Type, IReplicationStrategy>();

        private readonly IMetadataProvider metadataProvider;

        public ObjectReplicator(IMetadataProvider metadataProvider)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            this.metadataProvider = metadataProvider;
        }

        public object Replicate(object source)
        {
            if (ReferenceEquals(source, null))
                return null;
            Type type = source.GetType();
            IReplicationStrategy entityReplicator = entityReplicators.GetOrAdd(type, BuildEntityReplicator);
            return entityReplicator.Replicate(source);
        }

        private IReplicationStrategy BuildEntityReplicator(Type type)
        {
            ReplicationBehavior behavior = metadataProvider.GetBehavior(type);
            switch (behavior) {
                case ReplicationBehavior.Ignore:
                    return IgnoringReplicationStrategy.Instance;
                case ReplicationBehavior.Copy:
                    return CopyOnlyReplicationStategy.Instance;
                case ReplicationBehavior.Replicate:
                    return new CommonReplicationStrategy(metadataProvider, this,
                        type.IsNullable() ? type.GetNullableUnderlyingType() : type);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}