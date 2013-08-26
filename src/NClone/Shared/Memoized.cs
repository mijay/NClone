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
            GuardAgainstEnumerable<TResult>();
            return (TResult) memoizedFunctions.GetOrAdd(@delegate, x => ((Func<TResult>) x)());
        }

        public static TResult Delegate<TArgument, TResult>(Func<TArgument, TResult> @delegate, TArgument argument)
        {
            GuardAgainstEnumerable<TResult>();
            return (TResult) memoizedFunctions.GetOrAdd(Tuple.Create(@delegate, argument),
                x => {
                    var tuple = (Tuple<Func<TArgument, TResult>, TArgument>) x;
                    return tuple.Item1(tuple.Item2);
                });
        }

        private static void GuardAgainstEnumerable<TResult>()
        {
            Guard.Against<InvalidOperationException>(
                typeof (TResult).IsGenericType && typeof (TResult).GetGenericTypeDefinition() == typeof (IEnumerable<>),
                "You can not memoize enumerable - they are lazy");
            Guard.Against<InvalidOperationException>(
                typeof (TResult) == typeof (IEnumerable),
                "You can not memoize enumerable - they are lazy");
        }
    }
}