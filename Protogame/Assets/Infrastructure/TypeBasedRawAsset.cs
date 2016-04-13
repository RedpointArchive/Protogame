namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class TypeBasedRawAsset : IRawAsset
    {
        private readonly string m_Name;

        private readonly Type m_Type;

        public TypeBasedRawAsset(string name, Type type)
        {
            this.m_Name = name;
            this.m_Type = type;
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
                return new ReadOnlyCollection<KeyValuePair<string, object>>(new[]
                {
                    new KeyValuePair<string, object>("Name", this.m_Name),
                    new KeyValuePair<string, object>("Type", this.m_Type), 
                });
            }
        }

        public T GetProperty<T>(string name, T defaultValue = default(T))
        {
            switch (name)
            {
                case "Name":
                    if (typeof(T) != typeof(string))
                    {
                        throw new InvalidCastException();
                    }

                    return (T)(object)this.m_Name;
                case "Type":
                    if (typeof(T) != typeof(Type))
                    {
                        throw new InvalidCastException();
                    }

                    return (T)(object)this.m_Type;
                default:
                    throw new MissingMemberException(this.GetType().FullName, name);
            }
        }

        public object GetProperty(System.Type type, string name, object defaultValue = default(object))
        {
            switch (name)
            {
                case "Name":
                    if (type != typeof(string))
                    {
                        throw new InvalidCastException();
                    }

                    return this.m_Name;
                case "Type":
                    if (type != typeof(Type))
                    {
                        throw new InvalidCastException();
                    }

                    return this.m_Type;
                default:
                    throw new MissingMemberException(this.GetType().FullName, name);
            }
        }
    }
}