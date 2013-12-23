using System;

namespace Protogame
{
    public class AssetAttribute : Attribute
    {
        public AssetAttribute(string name)
        {
            this.Name = name;
        }

        public string Name
        {
            get;
            private set;
        }
    }
}

