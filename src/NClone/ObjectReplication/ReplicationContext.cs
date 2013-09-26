using System;
using System.Collections.Generic;
using NClone.ReplicationStrategies;
using NClone.Shared;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Implementation of <see cref="IReplicationContext"/>.
    /// </summary>
    internal class ReplicationContext: IReplicationContext
    {
        private readonly CircularReferenceDetector circularReferenceDetector = new CircularReferenceDetector();
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
            if (replicatedEntries.TryGetValue(source, out result))
                return result;

            using (circularReferenceDetector.EnterContext(source)) {

                IReplicationStrategy replicationStrategy = replicationStrategyFactory.StrategyForType(source.GetType());
                result = replicationStrategy.Replicate(source, this);

                replicatedEntries.Add(source, result);
                return result;
            }
        }

        private class CircularReferenceDetector: IDisposable
        {
            private readonly Stack<object> replicatingObjectsStack = new Stack<object>();

            public void Dispose()
            {
                replicatingObjectsStack.Pop();
            }

            public IDisposable EnterContext(object replicatingObject)
            {
                if (replicatingObjectsStack.Contains(replicatingObject))
                    throw new CircularReferenceFoundException();
                replicatingObjectsStack.Push(replicatingObject);
                return this;
            }
        }
    }
}