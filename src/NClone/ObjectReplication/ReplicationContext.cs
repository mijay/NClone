using System.Collections.Generic;
using System.Threading.Tasks;
using mijay.Utils;
using mijay.Utils.Comparers;
using mijay.Utils.Tasks;
using NClone.ReplicationStrategies;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Implementation of <see cref="IReplicationContext"/>.
    /// </summary>
    internal class ReplicationContext: IReplicationContext
    {
        private static readonly Task<object> nullTask = TaskEx.FromResult<object>(null);

        private readonly IDictionary<object, TaskCompletionSource<object>> computingEntities =
            new Dictionary<object, TaskCompletionSource<object>>(ReferenceEqualityComparer.Instance);

        private readonly IReplicationStrategyFactory replicationStrategyFactory;

        public ReplicationContext(IReplicationStrategyFactory replicationStrategyFactory)
        {
            Guard.AgainstNull(replicationStrategyFactory, "replicationStrategyFactory");
            this.replicationStrategyFactory = replicationStrategyFactory;
        }

        public Task<object> ReplicateAsync(object source)
        {
            if (ReferenceEquals(source, null))
                return nullTask;

            TaskCompletionSource<object> resultAsync;
            if (computingEntities.TryGetValue(source, out resultAsync))
                return resultAsync.Task;

            resultAsync = new TaskCompletionSource<object>();
            computingEntities.Add(source, resultAsync);

            var result = Replicate(source);

            resultAsync.SetResult(result);
            return resultAsync.Task;
        }

        private object Replicate(object source)
        {
            IReplicationStrategy replicationStrategy = replicationStrategyFactory.StrategyForType(source.GetType());
            return replicationStrategy.Replicate(source, this);
        }
    }
}
