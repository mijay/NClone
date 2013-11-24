using NClone.ObjectReplication;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Dummy implementation of <see cref="IReplicationStrategy"/>, which <see cref="Replicate"/> method just returns a given argument.
    /// </summary>
    internal class CopyOnlyReplicationStrategy: IReplicationStrategy
    {
        /// <summary>
        /// The single instance of <see cref="CopyOnlyReplicationStrategy"/>.
        /// </summary>
        public static readonly IReplicationStrategy Instance = new CopyOnlyReplicationStrategy();

        private CopyOnlyReplicationStrategy()
        {
        }

        public object Replicate(object source, IReplicationContext context)
        {
            return source;
        }
    }
}