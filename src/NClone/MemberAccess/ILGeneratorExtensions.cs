﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NClone.MemberAccess
{
    internal static class ILGeneratorExtensions
    {
        public static ILGenerator LoadArgumentAddress(this ILGenerator ilGenerator, Type argumentType, int argumentIndex)
        {
            ilGenerator.Emit(argumentType.IsValueType ? OpCodes.Ldarga : OpCodes.Ldarg, argumentIndex);
            return ilGenerator;
        }

        public static ILGenerator LoadArgument(this ILGenerator ilGenerator, int argumentIndex)
        {
            ilGenerator.Emit(OpCodes.Ldarg, argumentIndex);
            return ilGenerator;
        }

        public static ILGenerator GetFieldValue(this ILGenerator ilGenerator, FieldInfo fieldInfo)
        {
            ilGenerator.Emit(OpCodes.Ldfld, fieldInfo);
            return ilGenerator;
        }

        public static ILGenerator SetFieldValue(this ILGenerator ilGenerator, FieldInfo fieldInfo)
        {
            ilGenerator.Emit(OpCodes.Stfld, fieldInfo);
            return ilGenerator;
        }

        public static ILGenerator CastDownReference(this ILGenerator ilGenerator, Type targetType)
        {
            if (targetType.IsValueType) {
                ilGenerator.Emit(OpCodes.Unbox, targetType);
            }
            return ilGenerator;
        }

        public static ILGenerator LoadValueType(this ILGenerator ilGenerator, Type targetType)
        {
            if (targetType.IsValueType) {
                ilGenerator.Emit(OpCodes.Ldobj, targetType);
            }
            return ilGenerator;
        }

        public static ILGenerator Box(this ILGenerator ilGenerator, Type sourceType)
        {
            if (sourceType.IsValueType)
                ilGenerator.Emit(OpCodes.Box, sourceType);
            return ilGenerator;
        }

        public static ILGenerator StoreInVariable(this ILGenerator ilGenerator, int localValueIndex)
        {
            ilGenerator.Emit(OpCodes.Stloc, localValueIndex);
            return ilGenerator;
        }

        public static ILGenerator LoadAddressOfVariable(this ILGenerator ilGenerator, int localValueIndex, Type typeOfLocal)
        {
            ilGenerator.Emit(typeOfLocal.IsValueType ? OpCodes.Ldloca : OpCodes.Ldloc, localValueIndex);
            return ilGenerator;
        }

        public static ILGenerator LoadVariable(this ILGenerator ilGenerator, int localValueIndex)
        {
            ilGenerator.Emit(OpCodes.Ldloc, localValueIndex);
            return ilGenerator;
        }

        public static void Return(this ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Ret);
        }

        public static ILGenerator Pop(this ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Pop);
            return ilGenerator;
        }

        public static ILGenerator DuplicateItemOnStack(this ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Dup);
            return ilGenerator;
        }

        public static ILGenerator LoadConstant(this ILGenerator ilGenerator, int constant)
        {
            ilGenerator.Emit(OpCodes.Ldc_I4, constant);
            return ilGenerator;
        }
    }
}