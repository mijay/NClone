using System;
using mijay.Utils;
using NClone.MemberAccess;
using NClone.ObjectReplication;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> that deep copying arrays.
    /// </summary>
    internal class CloneArrayReplicationStrategy: IReplicationStrategy
    {
        private readonly Type elementType;
        private readonly Func<Array, int, object> arrayElementReader;
        private readonly Action<Array, int, object> arrayElementWriter;

        public CloneArrayReplicationStrategy(Type elementType)
        {
            this.elementType = elementType;
            arrayElementReader = ArrayAccessorBuilder.BuildArrayElementReader(elementType);
            arrayElementWriter = ArrayAccessorBuilder.BuildArrayElementWriter(elementType);
        }

        public object Replicate(object source, IReplicationContext context)
        {
            Guard.AgainstViolation(source.GetType().GetElementType() == elementType,
                "This replicator can copy only arrays of elements of type {0}, but {1} received",
                elementType, source.GetType().GetElementType());

            var sourceArray = source.As<Array>();
            Array resultingArray = Array.CreateInstance(elementType, sourceArray.Length);
            for (int i = sourceArray.Length - 1; i >= 0; i--) {
                object sourceElement = arrayElementReader(sourceArray, i);
                var resultingElement = context.Replicate(sourceElement);
                arrayElementWriter(resultingArray, i, resultingElement);
            }
            return resultingArray;
        }
    }
}