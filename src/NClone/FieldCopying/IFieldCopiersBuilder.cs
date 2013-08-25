using System.Reflection;

namespace NClone.FieldCopying
{
    /// <summary>
    /// Builds <see cref="IFieldCopier{TEntity}"/> for a given field of a given type.
    /// </summary>
    internal interface IFieldCopiersBuilder
    {
        /// <summary>
        /// Builds <see cref="IFieldCopier{TEntity}"/> for the <paramref name="field"/> of the <typeparamref name="TEntity"/>.
        /// </summary>
        IFieldCopier<TEntity> BuildFor<TEntity>(FieldInfo field);
    }
}