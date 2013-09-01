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

        public static bool TrySingle<T>([InstantHandle] this IEnumerable<T> source, out T value)
        {
            var values = source.Take(2).ToArray();
            switch (values.Length) {
                case 0:
                    value = default(T);
                    return false;
                case 1:
                    value = values[0];
                    return true;
                default:
                    throw new InvalidOperationException("Source contains more than one element");
            }
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