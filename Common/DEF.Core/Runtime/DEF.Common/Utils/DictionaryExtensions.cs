using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static bool TryGetValue2<TKey, TValue>(this Dictionary<TKey, object> dic, TKey key, out TValue value)
    {
        if (dic != null && dic.TryGetValue(key, out var v))
        {
            if (v is TValue v2)
            {
                value = v2;
                return true;
            }
        }

        value = default;

        return false;
    }

    public static void CopyTo<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> target)
    {
        if (dic == null)
        {
            return;
        }
        foreach (var kv in target)
        {
            dic[kv.Key] = kv.Value;
        }
    }
}