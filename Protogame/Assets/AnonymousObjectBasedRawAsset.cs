namespace Protogame
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public class AnonymousObjectBasedRawAsset : IRawAsset
    {
        private readonly Dictionary<string, object> m_Dictionary;

        public AnonymousObjectBasedRawAsset(object obj)
        {
            this.m_Dictionary = TypeDescriptor.GetProperties(obj)
                .OfType<PropertyDescriptor>()
                .ToDictionary(k => k.Name, v => v.GetValue(obj));
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