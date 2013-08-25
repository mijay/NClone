using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NClone.Shared
{
    public static class Memoized
    {
        private static readonly ConcurrentDictionary<object, object> memoizedFunctions =
            new ConcurrentDictionary<object, object>();

        public static TResult Delegate<TResult>(Func<TResult> @delegate)
        {
            Guard.Against<InvalidOperationException>(
                typeof (TResult).IsGenericType && typeof (TResult).GetGenericTypeDefinition() == typeof (IEnumerable<>),
                "You can not memoize enumerable - they are lazy");
            Guard.Against<InvalidOperationException>(
                typeof (TResult) == typeof (IEnumerable),
                "You can not memoize enumerable - they are lazy");
            return (TResult) memoizedFunctions.GetOrAdd(@delegate, x => ((Func<TResult>) x)());
        }
    }
}