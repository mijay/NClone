using System;
using System.Collections.Generic;

namespace mijay.Utils.Comparers
{
    /// <summary>
    /// <see cref="IComparer{T}"/> that delegates <see cref="Compare"/> function to the provided delegated.
    /// </summary>
    public class DelegatedComparer<T>: IComparer<T>
    {
        private readonly Func<T, T, int> comparer;

        public DelegatedComparer(Func<T, T, int> comparer)
        {
            Guard.AgainstNull(comparer, "comparer");
            this.comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return comparer(x, y);
        }
    }
}