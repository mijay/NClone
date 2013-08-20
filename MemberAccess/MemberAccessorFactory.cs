using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Core.MemberAccess
{
    public static class MemberAccessorFactory
    {
        public static IMemberAccessor BuildAccessor(FieldInfo member)
        {
            return new MemberAccessor(BuildGetter(member), BuildSetter(member));
        }


        private static Func<object, object> BuildGetter(FieldInfo member)
        {
            var method = new DynamicMethod("getterFrom_" + member.DeclaringType.FullName + "_of_" + member.Name,
                                           typeof (object), new[] { typeof (object) },
                                           member.DeclaringType, true);
            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            EmitCastReferenceDown(ilGenerator, member.DeclaringType);
            ilGenerator.Emit(OpCodes.Ldfld, member);
            EmitCastReferenceUp(ilGenerator, member.FieldType);
            ilGenerator.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof (Func<object, object>)).CastTo<Func<object, object>>();
        }

        private static Action<object, object> BuildSetter(FieldInfo member)
        {
            var method = new DynamicMethod("setterFor_" + member.Name + "_of_" + member.DeclaringType.FullName,
                                           typeof (void), new[] { typeof (object), typeof (object) },
                                           member.DeclaringType, true);
            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_1);
            EmitCastReferenceDown(ilGenerator, member.DeclaringType);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            EmitCastReferenceDown(ilGenerator, member.FieldType);
            EmitLoadValue(ilGenerator, member.FieldType);
            ilGenerator.Emit(OpCodes.Stfld, member);
            ilGenerator.Emit(OpCodes.Ret);

            return method.CreateDelegate(typeof (Action<object, object>)).CastTo<Action<object, object>>();
        }

        private static void EmitCastReferenceDown(ILGenerator ilGenerator, Type type)
        {
            ilGenerator.Emit(!type.IsValueType ? OpCodes.Castclass : OpCodes.Unbox, type);
        }

        private static void EmitCastReferenceUp(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
                ilGenerator.Emit(OpCodes.Box, type);
        }

        private static void EmitLoadValue(ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
                ilGenerator.Emit(OpCodes.Ldobj, type);
        }
    }
}
