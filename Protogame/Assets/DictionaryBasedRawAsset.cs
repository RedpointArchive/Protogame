namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

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

        /// <summary>
        /// A read-only copy of the properties associated with this raw asset.
        /// </summary>
        public ReadOnlyCollection<KeyValuePair<string, object>> Properties
        {
            get
            {
                return new ReadOnlyCollection<KeyValuePair<string, object>>(this.m_Dictionary.ToList());
            }
        }

        public T GetProperty<T>(string name, T defaultValue = default(T))
        {
            if (this.m_Dictionary.ContainsKey(name))
            {
                try
                {
                    if (typeof(T).IsEnum)
                    {
                        if (this.m_Dictionary[name] is string)
                        {
                            return (T)Enum.Parse(typeof(T), (string)this.m_Dictionary[name]);
                        }

                        if (this.m_Dictionary[name] is long)
                        {
                            return (T)Enum.ToObject(typeof(T), (long)this.m_Dictionary[name]);
                        }

                        throw new InvalidOperationException(
                            "Enumeration was stored as " + this.m_Dictionary[name].GetType());
                    }

                    if (typeof(T) == typeof(int) && this.m_Dictionary[name] is string)
                    {
                        return
                            (T)
                            (object)
                            int.Parse((string)this.m_Dictionary[name], CultureInfo.InvariantCulture.NumberFormat);
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
                catch (InvalidCastException)
                {
                    var obj = this.m_Dictionary[name];
                    var type = obj == null ? "<null>" : obj.GetType().FullName;

                    throw new InvalidCastException("Attempted to cast property '" + name + "' stored as " + type + " to target type " + typeof(T).FullName);
                }
            }

            return defaultValue;
        }
    }
}