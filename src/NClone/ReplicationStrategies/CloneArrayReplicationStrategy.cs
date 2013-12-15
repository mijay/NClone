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
        private readonly Func<Array, int, object> getElement;
        private readonly Action<Array, int, object> setElement;

        public CloneArrayReplicationStrategy(Type elementType)
        {
            this.elementType = elementType;
            IArrayAccessor arrayAccessor = ArrayAccessorBuilder.BuildForArrayOf(elementType);
            getElement = arrayAccessor.GetElement;
            setElement = arrayAccessor.SetElement;
        }

        public object Replicate(object source, IReplicationContext context)
        {
            Guard.AgainstViolation(source.GetType().GetElementType() == elementType,
                "This replicator can copy only arrays of elements of type {0}, but {1} received",
                elementType, source.GetType().GetElementType());
            var resultingArray = source.As<Array>().Clone().As<Array>();
            for (int i = resultingArray.Length - 1; i >= 0; i--) {
                object sourceElement = getElement(resultingArray, i);
                object resultingElement = context.Replicate(sourceElement);
                setElement(resultingArray, i, resultingElement);
            }
            return resultingArray;
        }
    }
}