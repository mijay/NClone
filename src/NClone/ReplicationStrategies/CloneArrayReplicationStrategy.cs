using System;
using mijay.Utils;
using mijay.Utils.Tasks;
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
            var sourceArray = source.As<Array>();
            var resultingArray = Array.CreateInstance(elementType, sourceArray.Length);
            for (int i = resultingArray.Length - 1; i >= 0; i--)
            {
                object sourceElement = getElement(sourceArray, i);
                var localI = i;
                context.ReplicateAsync(sourceElement)
                    .Then(resultingElement => setElement(resultingArray, localI, resultingElement));
            }
            return resultingArray;
        }
    }
}
