using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace mijay.Utils.Collections
{
    public static class CollectionExtensions
    {
#if NET45
        public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
        {
            Guard.AgainstNull(collection, "collection");
            return collection is IReadOnlyCollection<T>
                ? (IReadOnlyCollection<T>) collection
                : new ReadOnlyCollection<T>(collection.ToArray());
        }
#else
        public static ICollection<T> AsReadOnly<T>(this ICollection<T> collection)
        {
            Guard.AgainstNull(collection, "collection");
            return collection.IsReadOnly
                ? collection
                : new ReadOnlyCollection<T>(collection.ToArray());
        }
#endif
    }
}