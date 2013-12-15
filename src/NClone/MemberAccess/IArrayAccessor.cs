using System;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Represents access to an array of specific type.
    /// </summary>
    public interface IArrayAccessor
    {
        /// <summary>
        /// Declared <see cref="Type"/> of elements in array that can be accessed.
        /// </summary>
        Type ElementType { get; }

        /// <summary>
        /// Gets element from <paramref name="array"/> at specified <paramref name="index"/>.
        /// </summary>
        object GetElement(Array array, int index);

        /// <summary>
        /// Sets element in <paramref name="array"/> at specified <paramref name="index"/> to <paramref name="value"/>.
        /// </summary>
        void SetElement(Array array, int index, object value);
    }
}