namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class DictionaryBasedRawAsset : IRawAsset
    {
        private readonly Dictionary<string, object> m_Dictionary;

        public DictionaryBasedRawAsset(Dictionary<string, object> dictionary)
        {
            this.m_Dictionary = dictionary;
        }

        public bool IsCompiled
        {
            get
            {
                return false;
            }
        }

        public T GetProperty<T>(string name, T defaultValue = default(T))
        {
            if (this.m_Dictionary.ContainsKey(name))
            {
                if (typeof(T).IsEnum)
                {
                    return (T)Enum.Parse(typeof(T), (string)this.m_Dictionary[name]);
                }

                if (typeof(T) == typeof(int) && this.m_Dictionary[name] is string)
                {
                    return (T)(object)int.Parse((string)this.m_Dictionary[name], CultureInfo.InvariantCulture.NumberFormat);
                }

                if (typeof(T) == typeof(int) && this.m_Dictionary[name] is long)
                {
                    return (T)(object)(int)((long)this.m_Dictionary[name]);
                }

                if (typeof(T) == typeof(short) && this.m_Dictionary[name] is long)
                {
                    return (T)(object)(short)((long)this.m_Dictionary[name]);
                }

                if (typeof(T) == typeof(float) && this.m_Dictionary[name] is long)
                {
                    return (T)(object)(float)((long)this.m_Dictionary[name]);
                }

                return (T)this.m_Dictionary[name];
            }

            return defaultValue;
        }
    }
}