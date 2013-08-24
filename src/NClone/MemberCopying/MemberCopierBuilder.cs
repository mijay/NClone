using System;
using System.Reflection;
using System.Reflection.Emit;
using NClone.Shared;

namespace NClone.MemberCopying
{
    /// <summary>
    /// Implementation of <see cref="IMemberCopierBuilder"/>.
    /// </summary>
    internal class MemberCopierBuilder: IMemberCopierBuilder
    {
        public IMemberCopier<TEntity> BuildFor<TEntity>(FieldInfo field)
        {
            Guard.AgainstNull(field, "field");
            Guard.AgainstFalse(field.DeclaringType.IsAssignableFrom(typeof (TEntity)), "Only fields of {0} can be copied", typeof (TEntity));

            return new MemberCopier<TEntity>(CreateCopyMethod<TEntity>(field));
        }

        private static Func<TEntity, TEntity, TEntity> CreateCopyMethod<TEntity>(FieldInfo field)
        {
            var method = new DynamicMethod(
                string.Format("memberCopier_For[{0}.{1}]_In[{2}]", field.DeclaringType.FullName, field.Name, typeof (TEntity).FullName),
                typeof (TEntity), new[] { typeof (TEntity), typeof (TEntity) },
                typeof (TEntity), true);
            var ilGenerator = method.GetILGenerator();

            EmitCopyMethod(ilGenerator, typeof (TEntity), field);

            return (Func<TEntity, TEntity, TEntity>) method.CreateDelegate(typeof (Func<TEntity, TEntity, TEntity>));
        }

        private static void EmitCopyMethod(ILGenerator ilGenerator, Type argumentType, FieldInfo fieldToCopy)
        {
            ilGenerator.EmitLoadArgumentAddress(argumentType, 1);
            ilGenerator.EmitLoadArgumentAddress(argumentType, 0);
            ilGenerator.EmitLoadFieldValue(fieldToCopy);
            ilGenerator.EmitStoreFieldValue(fieldToCopy);

            ilGenerator.EmitLoadArgument(1);
            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}