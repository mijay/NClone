using System;
using NClone.ObjectReplication;
using NClone.Utils;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for arrays.
    /// </summary>
    internal class ArrayReplicationStrategy: IReplicationStrategy
    {
        private readonly Type elementType;

        public ArrayReplicationStrategy(Type arrayType)
        {
            Guard.AgainstViolation(arrayType.IsArray, "ArrayReplicationStrategy is applicable only to array types");
            elementType = arrayType.GetElementType();
        }

        public object Replicate(object source, IReplicationContext context)
        {
            Guard.AgainstViolation(source.GetType().GetElementType() == elementType,
                "This replicator can copy only arrays of elements of type {0}, but {1} received",
                elementType, source.GetType().GetElementType());

            var sourceArray = source.As<Array>();
            var result = Array.CreateInstance(elementType, sourceArray.Length);
            for (int i = sourceArray.Length - 1; i >= 0; i--) {
                var sourceElement = sourceArray.GetValue(i);
                result.SetValue(context.Replicate(sourceElement), i);
            }
            return result;
        }
    }
}