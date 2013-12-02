using System;
using mijay.Utils;
using NClone.ObjectReplication;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> that deep copying arrays.
    /// </summary>
    internal class CloneArrayReplicationStrategy: IReplicationStrategy
    {
        private readonly Type elementType;

        public CloneArrayReplicationStrategy(Type elementType)
        {
            this.elementType = elementType;
        }

        public object Replicate(object source, IReplicationContext context)
        {
            Guard.AgainstViolation(source.GetType().GetElementType() == elementType,
                "This replicator can copy only arrays of elements of type {0}, but {1} received",
                elementType, source.GetType().GetElementType());

            var resultingArray = source.As<Array>().Clone().As<object[]>();
            for (int i = resultingArray.Length - 1; i >= 0; i--) {
                object resultingElement = context.Replicate(resultingArray[i]);
                resultingArray[i] = resultingElement;
            }
            return resultingArray;
        }
    }
}