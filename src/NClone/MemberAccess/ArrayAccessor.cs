using System;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Implementation of <see cref="IArrayAccessor"/>
    /// </summary>
    internal class ArrayAccessor: IArrayAccessor
    {
        private readonly Func<Array, int, object> getMethod;
        private readonly Action<Array, int, object> setMethod;

        public ArrayAccessor(Type elementType, Func<Array, int, object> getMethod, Action<Array, int, object> setMethod)
        {
            ElementType = elementType;
            this.setMethod = setMethod;
            this.getMethod = getMethod;
        }

        public Type ElementType { get; private set; }

        public object GetElement(Array array, int index)
        {
            return getMethod(array, index);
        }

        public void SetElement(Array array, int index, object value)
        {
            setMethod(array, index, value);
        }
    }
}