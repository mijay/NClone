using System;
using mijay.Utils;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Implementation of <see cref="IMemberAccessor"/>
    /// </summary>
    internal class MemberAccessor: IMemberAccessor
    {
        private readonly Func<object, object> getMethod;
        private readonly Func<object, object, object> setMethod;

        public MemberAccessor(Type memberType, Func<object, object> getMethod, Func<object, object, object> setMethod)
        {
            MemberType = memberType;
            this.setMethod = setMethod;
            this.getMethod = getMethod;
        }

        public Type MemberType { get; private set; }

        public object SetMember(object container, object memberValue)
        {
            Guard.AgainstViolation(CanSet, "Can not set member when CanSet is false");
            return setMethod(container, memberValue);
        }

        public object GetMember(object container)
        {
            Guard.AgainstViolation(CanGet, "Can not get member when CanGet is false");
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