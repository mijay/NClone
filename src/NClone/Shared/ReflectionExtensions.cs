using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NClone.Shared
{
    public static class ReflectionExtensions
    {
        private static readonly Regex backingFieldRegex = new Regex(@"\<(\w+)\>k__BackingField", RegexOptions.Compiled);

        /// <summary>
        /// Checks whether <paramref name="member"/> has attribute <typeparamref name="TAttribute"/>.
        /// </summary>
        public static bool HasAttribute<TAttribute>(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof (TAttribute), true).Any();
        }

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
            while (currentType != typeof (object)) {
                yield return currentType;
                currentType = currentType.BaseType;
            }
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

        /// <summary>
        /// Checks whether <paramref name="property"/> is auto-implemented property.
        /// </summary>
        public static bool IsAutoProperty(this PropertyInfo property)
        {
            Guard.AgainstNull(property, "property");

            MethodInfo setMethod = property.GetSetMethod(true);
            MethodInfo getMethod = property.GetGetMethod(true);
            return setMethod != null && setMethod.HasAttribute<CompilerGeneratedAttribute>()
                   && getMethod != null && getMethod.HasAttribute<CompilerGeneratedAttribute>();
        }

        /// <summary>
        /// Returns <see cref="FieldInfo"/> of backing field for given auto-property.
        /// </summary>
        public static FieldInfo GetBackingField(this PropertyInfo property)
        {
            Guard.AgainstNull(property, "property");
            Guard.AgainstViolation(property.IsAutoProperty(), "Only auto-properties can have backing field");

            FieldInfo result = property.DeclaringType.GetField(
                string.Format("<{0}>k__BackingField", property.Name),
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (result == null)
                throw new ArgumentException(
                    string.Format("Cannot find backing field for property {0} in {1}",
                        property.Name, property.DeclaringType.FullName));
            return result;
        }

        /// <summary>
        /// Checks whether <paramref name="field"/> is a backing field for some auto-implemented property.
        /// </summary>
        public static bool IsBackingField(this FieldInfo field)
        {
            Guard.AgainstNull(field, "field");

            return field.HasAttribute<CompilerGeneratedAttribute>() && backingFieldRegex.IsMatch(field.Name);
        }
    }
}