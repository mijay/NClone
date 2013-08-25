using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace NClone.Shared
{
    [DebuggerStepThrough]
    public static class EnumerableExtensions
    {
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        public static void ForEach<T>([InstantHandle] this IEnumerable<T> source, [InstantHandle] Action<T> action)
        {
            foreach (var element in source)
                action(element);
        }

        public static void ForEach<T>([InstantHandle] this IEnumerable<T> source, [InstantHandle] Action<T, int> action)
        {
            var index = 0;
            foreach (var element in source)
                action(element, index++);
        }
    }
}