using System;
using System.Collections.Generic;

namespace NClone.Shared
{
    internal class DelegateEqualityComparer<TSource, TComparable>: IEqualityComparer<TSource>
    {
        private readonly Func<TSource, TComparable> selector;
        private readonly IEqualityComparer<TComparable> equalityComparer;

        public DelegateEqualityComparer(Func<TSource, TComparable> selector, IEqualityComparer<TComparable> equalityComparer)
        {
            Guard.AgainstNull(selector, "selector");
            Guard.AgainstNull(equalityComparer, "equalityComparer");
            this.selector = selector;
            this.equalityComparer = equalityComparer;
        }

        public bool Equals(TSource x, TSource y)
        {
            return equalityComparer.Equals(selector(x), selector(y));
        }

        public int GetHashCode(TSource obj)
        {
            return equalityComparer.GetHashCode(selector(obj));
        }
    }
}