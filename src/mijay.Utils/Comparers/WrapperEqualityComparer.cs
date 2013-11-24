using System;
using System.Collections.Generic;

namespace mijay.Utils.Comparers
{
    /// <summary>
    /// <see cref="IEqualityComparer{T}"/> that compares object of type <typeparamref name="TSource"/> by converting them
    /// into <typeparamref name="TComparable"/> and then applying existing comparer.
    /// </summary>
    public class WrapperEqualityComparer<TSource, TComparable>: IEqualityComparer<TSource>
    {
        private readonly Func<TSource, TComparable> selector;
        private readonly IEqualityComparer<TComparable> underlyingComparer;

        /// <summary>
        /// Create instance of <see cref="WrapperEqualityComparer{TSource,TComparable}"/> that uses <paramref name="selector"/>
        /// to convert <typeparamref name="TSource"/> instances to <typeparamref name="TComparable"/>, which afterwards are
        /// processed by <paramref name="underlyingComparer"/> or by default <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        public WrapperEqualityComparer(Func<TSource, TComparable> selector, IEqualityComparer<TComparable> underlyingComparer = null)
        {
            Guard.AgainstNull(selector, "selector");
            this.selector = selector;
            this.underlyingComparer = underlyingComparer ?? EqualityComparer<TComparable>.Default;
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