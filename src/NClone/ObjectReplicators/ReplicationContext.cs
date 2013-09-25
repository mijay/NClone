using System.Collections.Generic;
using NClone.ReplicationStrategies;

namespace NClone.ObjectReplicators
{
    /// <summary>
    /// Implementation of <see cref="IReplicationContext"/>.
    /// </summary>
    internal class ReplicationContext: IReplicationContext
    {
        private readonly IDictionary<object, object> replicatedEntries = new Dictionary<object, object>();
        private readonly IReplicationStrategyFactory replicationStrategyFactory;

        public ReplicationContext(IReplicationStrategyFactory replicationStrategyFactory)
        {
            this.replicationStrategyFactory = replicationStrategyFactory;
        }

        public object Replicate(object source)
        {
            if (source == null)
                return null;

            object result;
            if (replicatedEntries.TryGetValue(source, out result))
                return result;

            IReplicationStrategy replicationStrategy = replicationStrategyFactory.StrategyForType(source.GetType());
            result = replicationStrategy.Replicate(source, this);

            replicatedEntries.Add(source, result);
            return result;
        }
    }
}