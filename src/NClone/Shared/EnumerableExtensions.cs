using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace NClone.Shared
{
    [DebuggerStepThrough]
    public static class EnumerableExtensions
    {
        public static void ForEach<T>([InstantHandle] this IEnumerable<T> source, [InstantHandle] Action<T> action)
        {
            foreach (var element in source)
                action(element);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TComparable>(this IEnumerable<TSource> source,
                                                                            Func<TSource, TComparable> comparableSelector,
                                                                            IEqualityComparer<TComparable> equalityComparer = null)
        {
            return source.Distinct(new DelegateEqualityComparer<TSource, TComparable>(
                comparableSelector, equalityComparer ?? EqualityComparer<TComparable>.Default));
        }

        public static IEnumerable<T> Materialize<T>([InstantHandle] this IEnumerable<T> source)
        {
            if (source is ICollection<T> || source is IReadOnlyCollection<T> || source is ICollection)
                return source;
            return source.ToArray();
        }
    }
}