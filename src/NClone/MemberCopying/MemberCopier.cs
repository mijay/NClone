using System;
using NClone.Shared;

namespace NClone.MemberCopying
{
    /// <summary>
    /// Implementation of <see cref="IMemberCopier{TContainer}"/>
    /// </summary>
    internal class MemberCopier<TEntity>: IMemberCopier<TEntity>
    {
        private readonly Func<TEntity, TEntity, TEntity> copyDelegate;

        public MemberCopier(Func<TEntity, TEntity, TEntity> copyDelegate)
        {
            this.copyDelegate = copyDelegate;
            Guard.AgainstNull(copyDelegate, "copyDelegate");
        }

        public TEntity Copy(TEntity source, TEntity destination)
        {
            return copyDelegate(source, destination);
        }

        public bool Replicate
        {
            get { throw new NotImplementedException(); }
        }
    }
}