using System;
using System.Collections.Generic;
using System.Linq;

namespace NzbDrone.Common.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<T1, T2> Merge<T1, T2>(this Dictionary<T1, T2> first, Dictionary<T1, T2> second)
        where T1 : notnull
    {
        if (first == null)
        {
            throw new ArgumentNullException(nameof(first));
        }

        if (second == null)
        {
            throw new ArgumentNullException(nameof(second));
        }

        var merged = new Dictionary<T1, T2>();
        first.ToList().ForEach(kv => merged[kv.Key] = kv.Value);
        second.ToList().ForEach(kv => merged[kv.Key] = kv.Value);

        return merged;
    }

    public static void Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> collection, TKey key, TValue value)
        where TKey : notnull
    {
        collection.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public static IDictionary<TNewKey, TNewValue> SelectDictionary<TKey, TValue, TNewKey, TNewValue>(
        this IDictionary<TKey, TValue> dictionary,
        Func<KeyValuePair<TKey, TValue>, ValueTuple<TNewKey, TNewValue>> selection)
        where TNewKey : notnull
    {
        return dictionary.Select(selection).ToDictionary(t => t.Item1, t => t.Item2);
    }

    public static IDictionary<TNewKey, TNewValue> SelectDictionary<TKey, TValue, TNewKey, TNewValue>(
        this IDictionary<TKey, TValue> dictionary,
        Func<KeyValuePair<TKey, TValue>, TNewKey> keySelector,
        Func<KeyValuePair<TKey, TValue>, TNewValue> valueSelector)
        where TNewKey : notnull
    {
        return dictionary.SelectDictionary(p => (keySelector(p), valueSelector(p)));
    }
}
