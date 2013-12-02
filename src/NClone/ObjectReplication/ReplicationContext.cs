using System.Collections.Generic;
using mijay.Utils;
using NClone.ReplicationStrategies;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Implementation of <see cref="IReplicationContext"/>.
    /// </summary>
    internal class ReplicationContext: IReplicationContext
    {
        private static readonly object objectIsReplicatingMarker = new object();
        private readonly IDictionary<object, object> replicatedEntries = new Dictionary<object, object>();
        private readonly IReplicationStrategyFactory replicationStrategyFactory;

        public ReplicationContext(IReplicationStrategyFactory replicationStrategyFactory)
        {
            Guard.AgainstNull(replicationStrategyFactory, "replicationStrategyFactory");
            this.replicationStrategyFactory = replicationStrategyFactory;
        }

        public object Replicate(object source)
        {
            if (source == null)
                return null;

            object result;
            if (TryGetReplicatedValue(source, out result))
                return result;

            IReplicationStrategy replicationStrategy = replicationStrategyFactory.StrategyForType(source.GetType());
            result = replicationStrategy.Replicate(source, this);

            StoreReplicatedValue(source, result);
            return result;
        }

        private bool TryGetReplicatedValue(object source, out object result)
        {
            if (replicatedEntries.TryGetValue(source, out result)) {
                if (ReferenceEquals(result, objectIsReplicatingMarker))
                    throw new CircularReferenceFoundException();
                return true;
            }
            replicatedEntries.Add(source, objectIsReplicatingMarker);
            return false;
        }

        private void StoreReplicatedValue(object source, object result)
        {
            replicatedEntries[source] = result;
        }
    }
}