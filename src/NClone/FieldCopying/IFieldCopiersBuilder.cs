using System;
using System.Reflection;

namespace NClone.FieldCopying
{
    /// <summary>
    /// Builds <see cref="IFieldCopier"/> for a given field of a given type.
    /// </summary>
    internal interface IFieldCopiersBuilder
    {
        /// <summary>
        /// Builds <see cref="IFieldCopier"/> for the <paramref name="field"/> of the <paramref name="container"/>.
        /// </summary>
        IFieldCopier BuildFor(Type container, FieldInfo field);
    }
}