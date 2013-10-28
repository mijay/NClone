using System;
using System.Reflection;
using System.Reflection.Emit;
using NClone.Shared;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Factory for <see cref="IMemberAccessor"/>s to fields.
    /// </summary>
    public static class FieldAccessorBuilder
    {
        /// <summary>
        /// Build <see cref="IMemberAccessor"/> to access <paramref name="field"/> in container of type <paramref name="containerType"/>.
        /// </summary>
        /// <param name="containerType">
        /// Type of entity, in which <see cref="IMemberAccessor"/> will get and/or set field value.
        /// </param>
        /// <param name="field">
        /// Field of <paramref name="containerType"/> or its base classes, for which <see cref="IMemberAccessor"/> is built.
        /// </param>
        /// <param name="skipAccessibilityChecks">
        /// Flag that indicates, whether returned <see cref="IMemberAccessor"/> should ignore visibility and <c>readonly</c> checks.
        /// </param>
        /// <remarks>
        /// <para><paramref name="field"/> should be defined in <paramref name="containerType"/> or in one of its base types.</para>
        /// 
        /// <para>Returned <see cref="IMemberAccessor"/> will be able to access containers of type <paramref name="containerType"/>
        /// or inherited, despite the fact where <paramref name="field"/> is defined.</para>
        /// </remarks>
        public static IMemberAccessor BuildFor(Type containerType, FieldInfo field, bool skipAccessibilityChecks = false)
        {
            Guard.AgainstNull(containerType, "containerType");
            Guard.AgainstNull(field, "field");
            Guard.AgainstViolation(field.DeclaringType.IsAssignableFrom(containerType),
                "IMemberAccessor for entity [{0}] can't access field from entity [{1}]",
                containerType.FullName, field.DeclaringType.FullName);

            Func<object, object> getMethod = skipAccessibilityChecks || CanGet(field, containerType)
                ? CreateGetMethod(containerType, field)
                : null;
            Func<object, object, object> setMethod = skipAccessibilityChecks || CanSet(field, containerType)
                ? CreateSetMethod(containerType, field)
                : null;

            return new MemberAccessor(getMethod, setMethod);
        }

        private static Func<object, object> CreateGetMethod(Type containerType, FieldInfo field)
        {
            var method = new DynamicMethod(
                BuildDynamicMethodName("getMember", containerType, field),
                typeof (object), new[] { typeof (object) },
                containerType, true);
            ILGenerator ilGenerator = method.GetILGenerator();

            ilGenerator.LoadArgument(0)
                .CastDownPointer(containerType)
                .GetFieldValue(field)
                .BoxValue(field.FieldType)
                .Return();

            return (Func<object, object>) method.CreateDelegate(typeof (Func<object, object>));
        }

        private static Func<object, object, object> CreateSetMethod(Type containerType, FieldInfo field)
        {
            var method = new DynamicMethod(
                BuildDynamicMethodName("setMember", containerType, field),
                typeof (object), new[] { typeof (object), typeof (object) },
                containerType, true);
            ILGenerator ilGenerator = method.GetILGenerator();

            ilGenerator.LoadArgument(0)
                .CastDownPointer(containerType)
                .DuplicateItemOnStack()
                .LoadArgument(1)
                .CastDownPointer(field.FieldType)
                .LoadValueByPointer(field.FieldType)
                .SetFieldValue(field)
                .LoadValueByPointer(containerType)
                .BoxValue(containerType)
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