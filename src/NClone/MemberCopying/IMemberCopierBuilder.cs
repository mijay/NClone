using System.Reflection;

namespace NClone.MemberCopying
{
    /// <summary>
    /// Builds <see cref="IMemberCopier{TEntity}"/> for a given field of a given type.
    /// </summary>
    internal interface IMemberCopierBuilder
    {
        /// <summary>
        /// Builds <see cref="IMemberCopier{TEntity}"/> for the <paramref name="field"/> of the <typeparamref name="TEntity"/>.
        /// </summary>
        IMemberCopier<TEntity> BuildFor<TEntity>(FieldInfo field);
    }
}