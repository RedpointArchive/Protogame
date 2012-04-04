using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protogame
{
    public class TileNameAttribute : Attribute
    {
        public string Name { get; set; }
        public TilesetXmlLoader.XmlLoadMode Type { get; set; }

        public TileNameAttribute(string name, TilesetXmlLoader.XmlLoadMode type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
