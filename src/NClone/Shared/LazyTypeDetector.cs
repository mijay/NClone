using System;
using System.Collections;
using System.Collections.Generic;

namespace NClone.Shared
{
    internal static class LazyTypeDetector
    {
        public static bool IsLazyEnumerable(Type type)
        {
            if (type.ImplementsGenericInterface(typeof (IEnumerator<>)) || typeof (IEnumerator).IsAssignableFrom(type))
                return true;

            if (type.ImplementsGenericInterface(typeof(IEnumerable<>)) || typeof(IEnumerable).IsAssignableFrom(type))
                if (!type.ImplementsGenericInterface(typeof (ICollection<>))
                    && !typeof (ICollection).IsAssignableFrom(type))
                    return true;

            return false;
        }
    }
}