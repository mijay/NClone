using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using NClone.Shared;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Factory for <see cref="IMemberAccessor"/> for members of type <see cref="MemberTypes.Field"/>.
    /// </summary>
    public static class FieldAccessorBuilder
    {
        /// <summary>
        /// Build <see cref="IMemberAccessor"/> to access field <paramref name="field"/> in container <paramref name="containerType"/>.
        /// </summary>
        /// <param name="containerType">
        /// Type of entity, in which <see cref="IMemberAccessor"/> will get and/or set field value.
        /// </param>
        /// <param name="field">
        /// Field of <paramref name="containerType"/> or its base classes, for which <see cref="IMemberAccessor"/> is built.
        /// </param>
        /// <param name="skipAccessibility">
        /// Flag that indicates, whether returned <see cref="IMemberAccessor"/> should ignore visibility and <c>readonly</c> checks.
        /// </param>
        public static IMemberAccessor BuildFor(Type containerType, FieldInfo field, bool skipAccessibility = false)
        {
            Guard.AgainstNull(containerType, "containerType");
            Guard.AgainstNull(field, "field");
            Guard.AgainstViolation(field.DeclaringType.IsAssignableFrom(containerType),
                "IMemberAccessor for entity [{0}] can't access field from entity [{1}]",
                containerType.FullName, field.DeclaringType.FullName);

            var getMethod = skipAccessibility || CanGet(field, containerType) ? CreateGetMethod(containerType, field) : null;
            var setMethod = skipAccessibility || CanSet(field, containerType) ? CreateSetMethod(containerType, field) : null;

            return new MemberAccessor(getMethod, setMethod);
        }

        private static Func<object, object> CreateGetMethod(Type containerType, FieldInfo field)
        {
            var method = new DynamicMethod(
                BuildDynamicMethodName("getMember", containerType, field),
                typeof(object), new[] { typeof(object) },
                containerType, true);
            var ilGenerator = method.GetILGenerator();

            ilGenerator.LoadArgument(0)
                       .CastDown(containerType)
                       .LoadFieldValue(field)
                       .Box(field.FieldType)
                       .Return();

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        private static Func<object, object, object> CreateSetMethod(Type containerType, FieldInfo field)
        {
            var method = new DynamicMethod(
                BuildDynamicMethodName("setMember", containerType, field),
                typeof (object), new[] { typeof (object), typeof (object) },
                containerType, true);
            var ilGenerator = method.GetILGenerator();
            ilGenerator.DeclareLocal(containerType);

            ilGenerator.LoadArgument(0)
                       .CastDown(containerType)
                       .StoreInLocal(0)
                       .LoadAddressOfLocal(0, containerType)
                       .LoadArgument(1)
                       .CastDown(field.FieldType)
                       .StoreFieldValue(field)
                       .LoadLocal(0)
                       .Box(containerType)
                       .Return();

            return (Func<object, object, object>) method.CreateDelegate(typeof (Func<object, object, object>));
        }

        private static string BuildDynamicMethodName(string action, Type container, FieldInfo field)
        {
            return string.Format("{0}_[{1}.{2}]_From_[{3}]", action, field.DeclaringType.FullName, field.Name, container.FullName);
        }

        private static bool CanGet(FieldInfo field, Type containerType)
        {
            if (field.IsPublic || field.IsFamily)
                return true;
            if (field.IsAssembly && field.DeclaringType.Assembly == containerType.Assembly)
                return true;
            return false;
        }

        private static bool CanSet(FieldInfo field, Type containerType)
        {
            if (field.IsInitOnly)
                return false;
            return CanGet(field, containerType);
        }
    }
}