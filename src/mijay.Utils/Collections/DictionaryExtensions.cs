using System;
using System.Collections.Generic;
using System.Linq;

namespace mijay.Utils.Collections
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TVal2> MapValues<TKey, TVal1, TVal2>(
            this IDictionary<TKey, TVal1> dictionary, Func<TKey, TVal1, TVal2> func)
        {
            Guard.AgainstNull(dictionary, "dictionary");
            Guard.AgainstNull(func, "func");
            return dictionary.ToDictionary(pair => pair.Key, pair => func(pair.Key, pair.Value));
        }

        public static TVal GetOrAdd<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key, Func<TVal> valueGetter)
        {
            Guard.AgainstNull(dictionary, "dictionary");
            Guard.AgainstNull(valueGetter, "valueGetter");

            TVal result;
            if (!dictionary.TryGetValue(key, out result)) {
                result = valueGetter();
                dictionary.Add(key, result);
            }
            return result;
        }
    }
}