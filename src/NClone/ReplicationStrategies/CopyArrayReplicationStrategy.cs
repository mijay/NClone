using System;
using NClone.ObjectReplication;
using NClone.Utils;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for shallow copying arrays.
    /// </summary>
    internal class CopyArrayReplicationStrategy: IReplicationStrategy
    {
        /// <summary>
        /// The single instance of <see cref="CopyArrayReplicationStrategy"/>.
        /// </summary>
        public static readonly IReplicationStrategy Instance = new CopyArrayReplicationStrategy();

        private CopyArrayReplicationStrategy()
        {
        }

        public object Replicate(object source, IReplicationContext context)
        {
            return source.As<Array>().Clone();
        }
    }
}