using NClone.MetadataProviders;
using NClone.ReplicationStrategies;
using NClone.Shared;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Implementation of <see cref="IObjectReplicator"/>.
    /// </summary>
    public class ObjectReplicator: IObjectReplicator
    {
        private readonly ReplicationStrategyFactory replicationStrategyFactory;

        public ObjectReplicator(IMetadataProvider metadataProvider)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            replicationStrategyFactory = new ReplicationStrategyFactory(metadataProvider);
        }

        public T Replicate<T>(T source)
        {
            return new ReplicationContext(replicationStrategyFactory).Replicate(source).As<T>();
        }
    }
}