using System;
using NClone.Shared;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Implementation of <see cref="IMemberAccessor"/>
    /// </summary>
    internal class MemberAccessor: IMemberAccessor
    {
        private readonly Func<object, object, object> setMethod;
        private readonly Func<object, object> getMethod;

        public MemberAccessor(Func<object, object> getMethod, Func<object, object, object> setMethod)
        {
            this.setMethod = setMethod;
            this.getMethod = getMethod;
        }

        public object SetMember(object container, object memberValue)
        {
            Guard.AgainstViolation<InvalidOperationException>(setMethod != null, "Can not set member when CanSet is false");
            return setMethod(container, memberValue);
        }

        public object GetMember(object container)
        {
            Guard.AgainstViolation<InvalidOperationException>(getMethod != null, "Can not get member when CanGet is false");
            return getMethod(container);
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