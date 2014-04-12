namespace Protogame
{
    using System;

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
    }
}