using System;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Represents access to a member of <typeparamref name="TEntity"/>.
    /// Type of member is <typeparamref name="TMember"/>.
    /// </summary>
    public interface IMemberAccessor<TEntity, TMember>
    {
        /// <summary>
        /// Sets value of accessed member in <paramref name="entity"/> to <paramref name="memberValue"/>
        /// and returns modified <paramref name="entity"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When <see cref="CanSet"/> is <c>false</c>.</exception>
        TEntity SetMember(TEntity entity, TMember memberValue);

        /// <summary>
        /// Gets value of accessed member in <paramref name="entity"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When <see cref="CanGet"/> is <c>false</c>.</exception>
        TMember GetMember(TEntity entity);

        /// <summary>
        /// Indicates whether current <see cref="IMemberAccessor{TEntity,TMember}"/> can <see cref="GetMember"/>
        /// </summary>
        bool CanGet { get; }

        /// <summary>
        /// Indicates whether current <see cref="IMemberAccessor{TEntity,TMember}"/> can <see cref="SetMember"/>
        /// </summary>
        bool CanSet { get; }
    }
}