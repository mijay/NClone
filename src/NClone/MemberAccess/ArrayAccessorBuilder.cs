using System;
using System.Reflection.Emit;
using mijay.Utils;
using mijay.Utils.Reflection;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Factory for getArrayElement and setArrayElement methods.
    /// </summary>
    public static class ArrayAccessorBuilder
    {
        /// <summary>
        /// Builds getArrayElement method for arrays with elements of type <paramref name="arrayElementType"/>.
        /// </summary>
        /// <remarks>
        /// getArrayElement function gets two arguments: untyped array (which should contain elements of
        /// type <paramref name="arrayElementType"/>) and integer index in it. It returns value (casted to
        /// <c>object</c>) stored in the array by the index.
        /// </remarks>
        public static Func<Array, int, object> BuildArrayElementReader(Type arrayElementType)
        {
            Guard.AgainstNull(arrayElementType, "arrayElementType");

            Type arrayType = arrayElementType.MakeArrayType();

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

        /// <summary>
        /// Builds setArrayElement method for arrays with elements of type <paramref name="arrayElementType"/>.
        /// </summary>
        /// <remarks>
        /// setArrayElement function gets three arguments: untyped array (which should contain elements of
        /// type <paramref name="arrayElementType"/>), integer index in it and value to be written (of type
        /// <paramref name="arrayElementType"/>, but casted to <c>object</c>). It stores the given value in
        /// the array by the given index.
        /// </remarks>
        public static Action<Array, int, object> BuildArrayElementWriter(Type arrayElementType)
        {
            Guard.AgainstNull(arrayElementType, "arrayElementType");

            Type arrayType = arrayElementType.MakeArrayType();

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