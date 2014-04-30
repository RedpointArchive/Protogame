namespace Protogame
{
    using System;

    public class VariableAsset : IAsset
    {
        private object m_CachedValue;

        private bool m_HasCached;

        public VariableAsset(string name, object value)
        {
            this.Name = name;
            this.RawValue = value;
        }

        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        public string Name { get; private set; }

        public object RawValue { get; private set; }

        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(VariableAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to VariableAsset.");
        }

        public T Value<T>()
        {
            if (!this.m_HasCached)
            {
                this.Cache(typeof(T));
                this.m_HasCached = true;
            }

            return (T)this.m_CachedValue;
        }

        private void Cache(Type type)
        {
            if (type == typeof(Single))
            {
                this.m_CachedValue = Convert.ToSingle(this.RawValue);
            }
            else if (type == typeof(Double))
            {
                this.m_CachedValue = Convert.ToDouble(this.RawValue);
            }
            else if (type == typeof(Int16))
            {
                this.m_CachedValue = Convert.ToInt16(this.RawValue);
            }
            else if (type == typeof(Int32))
            {
                this.m_CachedValue = Convert.ToInt32(this.RawValue);
            }
            else if (type == typeof(Int64))
            {
                this.m_CachedValue = Convert.ToInt64(this.RawValue);
            }
            else if (type == typeof(UInt16))
            {
                this.m_CachedValue = Convert.ToUInt16(this.RawValue);
            }
            else if (type == typeof(UInt32))
            {
                this.m_CachedValue = Convert.ToUInt32(this.RawValue);
            }
            else if (type == typeof(UInt64))
            {
                this.m_CachedValue = Convert.ToUInt64(this.RawValue);
            }
            else if (type == typeof(String))
            {
                this.m_CachedValue = Convert.ToString(this.RawValue);
            }
            else
            {
                throw new NotSupportedException("Requested variable asset conversion to " + type.FullName + ", but this is not supported!");
            }
        }
    }
}