using System;

namespace NClone.MemberCopying
{
    /// <summary>
    /// Implementation of <see cref="IMemberCopier{TContainer}"/>
    /// </summary>
    internal class MemberCopier<TEntity>: IMemberCopier<TEntity>
    {
        public TEntity Copy(TEntity source, TEntity destination)
        {
            throw new NotImplementedException();
        }

        public bool Replicate
        {
            get { throw new NotImplementedException(); }
        }
    }
}