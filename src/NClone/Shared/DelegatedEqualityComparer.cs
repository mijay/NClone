using System;
using System.Collections.Generic;

namespace NClone.Shared
{
    internal class DelegatedEqualityComparer<TSource, TComparable>: IEqualityComparer<TSource>
    {
        private readonly Func<TSource, TComparable> selector;
        private readonly IEqualityComparer<TComparable> underlyingComparer;

        public DelegatedEqualityComparer(Func<TSource, TComparable> selector, IEqualityComparer<TComparable> underlyingComparer)
        {
            Guard.AgainstNull(selector, "selector");
            Guard.AgainstNull(underlyingComparer, "underlyingComparer");
            this.selector = selector;
            this.underlyingComparer = underlyingComparer;
        }

        public bool Equals(TSource x, TSource y)
        {
            return underlyingComparer.Equals(selector(x), selector(y));
        }

        public int GetHashCode(TSource obj)
        {
            return underlyingComparer.GetHashCode(selector(obj));
        }
    }
}