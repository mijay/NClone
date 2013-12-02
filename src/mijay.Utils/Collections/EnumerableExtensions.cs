using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using mijay.Utils.Comparers;

namespace mijay.Utils.Collections
{
    /// <summary>
    /// Collection of extension methods for <see cref="IEnumerable{T}"/>s and <see cref="IEnumerable"/>s.
    /// </summary>
    [DebuggerStepThrough]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Executes <paramref name="action"/> for each element of the <paramref name="source"/>.
        /// </summary>
        public static void ForEach<T>([InstantHandle] this IEnumerable<T> source, [InstantHandle] Action<T> action)
        {
            foreach (var element in source)
                action(element);
        }

        /// <summary>
        /// Tries to get single value from <paramref name="source"/>.
        /// Throws <see cref="InvalidOperationException"/>, if more than one element found.
        /// </summary>
        public static bool TrySingle<T>([InstantHandle] this IEnumerable<T> source, out T value)
        {
            T[] values = source.Take(2).ToArray();
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

        /// <summary>
        /// Returns <see cref="IEnumerable{T}"/> of values from <paramref name="source"/> with distinct keys.
        /// Keys are selected using <paramref name="keySelector"/>.
        /// </summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                                     Func<TSource, TKey> keySelector,
                                                                     IEqualityComparer<TKey> keyComparer = null)
        {
            return source.Distinct(new WrapperEqualityComparer<TSource, TKey>(keySelector, keyComparer));
        }

        /// <summary>
        /// Concatenates strings listed in <paramref name="source"/>, using the specified <paramref name="separator"/> between each member.
        /// </summary>
        public static string JoinStrings([InstantHandle] this IEnumerable<string> source, string separator)
        {
            return String.Join(separator, source);
        }

        /// <summary>
        /// Adds <paramref name="item"/> to the tail of <paramref name="source"/> and returns new <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
        {
            return source.Concat(new[] { item });
        }

        /// <summary>
        /// Adds <paramref name="item"/> to the head of <paramref name="source"/> and returns new <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
        {
            return new[] { item }.Concat(source);
        }

        /// <summary>
        /// Materialize, i.e. make non-lazy, the given <paramref name="source"/>.
        /// </summary>
        public static IEnumerable<T> Materialize<T>([InstantHandle] this IEnumerable<T> source)
        {
            if (source is ICollection<T> || source is IReadOnlyCollection<T> || source is ICollection)
                return source;
            return source.ToArray();
        }

        /// <summary>
        /// Checks whether the given <paramref name="source"/> is empty.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        /// <summary>
        /// Checks whether the given <paramref name="source"/> contains any element
        /// from <paramref name="elements"/> list.
        /// </summary>
        public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] elements)
        {
            var elementsHash = new HashSet<T>(elements);
            return source.Any(elementsHash.Contains);
        }
    }
}