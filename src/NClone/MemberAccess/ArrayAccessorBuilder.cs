using System;
using System.Reflection.Emit;
using mijay.Utils;
using mijay.Utils.Reflection;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Factory for <see cref="IArrayAccessor"/>s.
    /// </summary>
    public static class ArrayAccessorBuilder
    {
        /// <summary>
        /// Builds <see cref="IArrayAccessor"/> for arrays with elements of type <paramref name="arrayElementType"/>.
        /// </summary>
        public static IArrayAccessor BuildForArrayOf(Type arrayElementType)
        {
            Guard.AgainstNull(arrayElementType, "arrayElementType");

            Type arrayType = arrayElementType.MakeArrayType();
            return new ArrayAccessor(arrayElementType, BuildGetMethod(arrayType, arrayElementType),
                BuildSetMethod(arrayType, arrayElementType));
        }

        private static Func<Array, int, object> BuildGetMethod(Type arrayType, Type arrayElementType)
        {
            var readerDeclaration = new DynamicMethod(
                "getFromArrayOf_" + arrayElementType.FullName,
                typeof (object), new[] { typeof (Array), typeof (int) },
                typeof (ArrayAccessorBuilder), true);

            readerDeclaration.GetILGenerator()
                .LoadArgument(0)
                .CastDownPointer(arrayType)
                .LoadArgument(1)
                .LoadArrayElement(arrayElementType)
                .BoxValue(arrayElementType)
                .Return();

            return (Func<Array, int, object>) readerDeclaration.CreateDelegate(typeof (Func<Array, int, object>));
        }

        private static Action<Array, int, object> BuildSetMethod(Type arrayType, Type arrayElementType)
        {
            var writerDeclaration = new DynamicMethod(
                "setToArrayOf_" + arrayElementType.FullName,
                null, new[] { typeof (Array), typeof (int), typeof (object) },
                typeof (ArrayAccessorBuilder), true);

            writerDeclaration.GetILGenerator()
                .LoadArgument(0)
                .CastDownPointer(arrayType)
                .LoadArgument(1)
                .LoadArgument(2)
                .CastDownPointer(arrayElementType)
                .LoadValueByPointer(arrayElementType)
                .StoreArrayElement(arrayElementType)
                .Return();

            return (Action<Array, int, object>) writerDeclaration.CreateDelegate(typeof (Action<Array, int, object>));
        }
    }
}