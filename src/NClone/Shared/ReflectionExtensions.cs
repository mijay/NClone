using System;
using System.Collections.Generic;

namespace NClone.Shared
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Checks whether the given <see cref="type"/> is <see cref="Nullable{T}"/>.
        /// </summary>
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        /// <summary>
        /// Returns <paramref name="type"/> and all his base types.
        /// </summary>
        public static IEnumerable<Type> GetHierarchy(this Type type)
        {
            var currentType = type;
            do {
                yield return currentType;
                currentType = currentType.BaseType;
            } while (currentType != typeof (object));
        }
    }
}