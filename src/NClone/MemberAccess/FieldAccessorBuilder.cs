using System;
using System.Reflection;
using System.Reflection.Emit;
using NClone.Shared;

namespace NClone.MemberAccess
{
    public static class FieldAccessorBuilder
    {
        public static IMemberAccessor<TEntity, TMember> BuildFor<TEntity, TMember>(FieldInfo field, bool skipRestrictions = false)
        {
            Guard.AgainstNull(field, "field");
            Guard.AgainstFalse(field.FieldType == typeof (TMember),
                "IMemberAccessor for field of type [{0}] can't access field of type [{1}]",
                typeof (TMember).FullName, field.FieldType.FullName);
            Guard.AgainstFalse(field.DeclaringType.IsAssignableFrom(typeof (TEntity)),
                "IMemberAccessor for entity [{0}] can't access field from entity [{1}]",
                typeof (TEntity).FullName, field.DeclaringType.FullName);

            var getMethod = skipRestrictions || CanGet(field, typeof (TEntity)) ? CreateGetMethod<TEntity, TMember>(field) : null;
            var setMethod = skipRestrictions || CanSet(field, typeof (TEntity)) ? CreateSetMethod<TEntity, TMember>(field) : null;

            return new MemberAccessor<TEntity, TMember>(getMethod, setMethod);
        }

        private static Func<TEntity, TMember> CreateGetMethod<TEntity, TMember>(FieldInfo field)
        {
            var method = new DynamicMethod(
                BuildDynamicMethodName("getMember", typeof (TEntity), field),
                typeof (TMember), new[] { typeof (TEntity) },
                typeof (TEntity), true);
            var ilGenerator = method.GetILGenerator();

            ilGenerator.EmitLoadArgumentAddress(typeof (TEntity), 0);
            ilGenerator.EmitLoadFieldValue(field);
            ilGenerator.Emit(OpCodes.Ret);

            return (Func<TEntity, TMember>) method.CreateDelegate(typeof (Func<TEntity, TMember>));
        }

        private static Func<TEntity, TMember, TEntity> CreateSetMethod<TEntity, TMember>(FieldInfo field)
        {
            var method = new DynamicMethod(
                BuildDynamicMethodName("setMember", typeof (TEntity), field),
                typeof (TEntity), new[] { typeof (TEntity), typeof (TMember) },
                typeof (TEntity), true);
            var ilGenerator = method.GetILGenerator();

            ilGenerator.EmitLoadArgumentAddress(typeof (TEntity), 0);
            ilGenerator.EmitLoadArgument(1);
            ilGenerator.EmitStoreFieldValue(field);
            ilGenerator.EmitLoadArgument(0);
            ilGenerator.Emit(OpCodes.Ret);

            return (Func<TEntity, TMember, TEntity>) method.CreateDelegate(typeof (Func<TEntity, TMember, TEntity>));
        }

        private static string BuildDynamicMethodName(string action, Type container, FieldInfo field)
        {
            return string.Format("{0}_[{1}.{2}]_From_[{3}]", action, field.DeclaringType.FullName, field.Name, container.FullName);
        }

        private static bool CanGet(FieldInfo field, Type containerType)
        {
            if(field.IsPublic || field.IsFamily)
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