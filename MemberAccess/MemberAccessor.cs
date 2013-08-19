using System;

namespace Core.MemberAccess
{
    internal class MemberAccessor: IMemberAccessor
    {
        private readonly Func<object, object> getter;
        private readonly Action<object, object> setter;

        public MemberAccessor(Func<object, object> getter, Action<object, object> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public object GetValue(object container)
        {
            return getter(container);
        }

        public void SetValue(object value, object container)
        {
            setter(value, container);
        }
    }
}