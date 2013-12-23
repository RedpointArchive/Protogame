using System;
using System.Collections.Generic;

namespace Protogame
{
    public class LanguageAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public string Value { get; set; }

        public LanguageAsset(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(LanguageAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to LanguageAsset.");
        }
    }
}

