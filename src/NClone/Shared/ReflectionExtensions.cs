using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Gets the underlying <see cref="ValueType"/> from type of <see cref="Nullable{T}"/>.
        /// </summary>
        public static Type GetNullableUnderlyingType(this Type type)
        {
            Guard.AgainstViolation(type.IsNullable(), "Type should be nullable");
            return type.GetGenericArguments().Single();
        }

        /// <summary>
        /// Returns <paramref name="type"/> and all his base types.
        /// </summary>
        public static IEnumerable<Type> GetHierarchy(this Type type)
        {
            Type currentType = type;
            do {
                yield return currentType;
                currentType = currentType.BaseType;
            } while (currentType != typeof (object));
        }

        /// <summary>
        /// Checks whether <paramref name="type"/> implements interface, which generic definition is <paramref name="genericInterface"/>.
        /// </summary>
        public static bool ImplementsGenericInterface(this Type type, Type genericInterface)
        {
            Guard.AgainstNull(type, "type");
            Guard.AgainstNull(genericInterface, "genericInterface");
            Guard.AgainstViolation(genericInterface.IsGenericTypeDefinition && genericInterface.IsInterface,
                "Only generic interface can be used");

            return type.GetInterfaces()
                       .Where(i => i.IsGenericType)
                       .Select(i => i.GetGenericTypeDefinition())
                       .Contains(genericInterface);
        }
    }
}