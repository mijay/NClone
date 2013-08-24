using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NClone.Shared
{
    public static class ILGeneratorExtensions
    {
        public static void EmitLoadArgumentAddress(this ILGenerator ilGenerator, Type argumentType, int argumentIndex)
        {
            ilGenerator.Emit(argumentType.IsValueType ? OpCodes.Ldarga : OpCodes.Ldarg, argumentIndex);
        }

        public static void EmitLoadArgument(this ILGenerator ilGenerator, int argumentIndex)
        {
            ilGenerator.Emit(OpCodes.Ldarg, argumentIndex);
        }

        public static void EmitLoadFieldValue(this ILGenerator ilGenerator, FieldInfo fieldInfo)
        {
            ilGenerator.Emit(OpCodes.Ldfld, fieldInfo);
        }

        public static void EmitStoreFieldValue(this ILGenerator ilGenerator, FieldInfo fieldInfo)
        {
            ilGenerator.Emit(OpCodes.Stfld, fieldInfo);
        }
    }
}