using System.Collections.Generic;

namespace Protogame
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                return default(TValue);
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue @default)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                return @default;
            }
        }
    }
}
