using System;
using NClone.Shared;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Implementation of <see cref="IMemberAccessor{TEntity,TMember}"/>
    /// </summary>
    internal class MemberAccessor<TEntity, TMember>: IMemberAccessor<TEntity, TMember>
    {
        private readonly Func<TEntity, TMember, TEntity> setMethod;
        private readonly Func<TEntity, TMember> getMethod;

        public MemberAccessor(Func<TEntity, TMember> getMethod, Func<TEntity, TMember, TEntity> setMethod)
        {
            this.setMethod = setMethod;
            this.getMethod = getMethod;
        }


        public TEntity SetMember(TEntity entity, TMember memberValue)
        {
            Guard.AgainstFalse(setMethod != null, "Can not set member when CanSet is false");
            return setMethod(entity, memberValue);
        }

        public TMember GetMember(TEntity entity)
        {
            Guard.AgainstFalse(getMethod != null, "Can not get member when CanGet is false");
            return getMethod(entity);
        }

        public bool CanGet
        {
            get { return getMethod != null; }
        }

        public bool CanSet
        {
            get { return setMethod != null; }
        }
    }
}