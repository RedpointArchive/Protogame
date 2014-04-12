namespace Protogame
{
    using System.Collections.Generic;

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
                return (T)this.m_Dictionary[name];
            }

            return defaultValue;
        }
    }
}