using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

namespace Protogame
{
    public class TilesetXmlLoader
    {
        public enum XmlLoadMode
        {
            Root,
            Solids,
            Tiles,
            Entities,
            Ignore
        }

        private static Dictionary<TileNameAttribute, Type> TileTypes = null;

        private XmlLoadMode m_Current = XmlLoadMode.Root;
        private string m_TilesetType = null;

        private TilesetXmlLoader()
        {
            this.m_Current = XmlLoadMode.Root;
        }

        private Tileset LoadXml(string mode, string filename)
        {
            Tileset tileset = new Tileset();
            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToLower() == "level")
                                this.m_Current = XmlLoadMode.Root;
                            else if (reader.Name.ToLower() == mode.ToLower() + "solids")
                                this.m_Current = XmlLoadMode.Solids;
                            else if (reader.Name.ToLower() == mode.ToLower() + "tiles")
                            {
                                this.m_Current = XmlLoadMode.Tiles;
                                this.m_TilesetType = reader.GetAttribute("tileset").ToLower();
                            }
                            else if (reader.Name.ToLower() == mode.ToLower() + "entities")
                                this.m_Current = XmlLoadMode.Entities;
                            else if (reader.Name.ToLower().EndsWith("solids") ||
                                reader.Name.ToLower().EndsWith("tiles") ||
                                reader.Name.ToLower().EndsWith("entities"))
                                this.m_Current = XmlLoadMode.Ignore;
                            else if (this.m_Current != XmlLoadMode.Ignore)
                                this.ProcessNode(reader, tileset);
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name.ToLower() == "level" || reader.Name.ToLower() == mode.ToLower() + "solids" ||
                                reader.Name.ToLower() == mode.ToLower() + "tiles" || reader.Name.ToLower() == mode.ToLower() + "entities" ||
                                reader.Name.ToLower().EndsWith("solids") || reader.Name.ToLower().EndsWith("tiles") ||
                                reader.Name.ToLower().EndsWith("entities"))
                                this.m_Current = XmlLoadMode.Root;
                            break;
                    }
                }
            }
            return tileset;
        }

        private void ProcessNode(XmlReader reader, Tileset tileset)
        {
            Tile t = this.GenerateTile(reader);
            if (t == null)
                throw new ApplicationException("Unable to create tile for '" + reader.Name + "', there's nothing to handle this!");

            // TODO: This might need to be changed depending on whether a tileset can have multiple tiles in the same position.
            int x = (int)t.X / Tileset.TILESET_CELL_WIDTH;
            int y = (int)t.Y / Tileset.TILESET_CELL_HEIGHT;
            int z = tileset.DepthAt[x, y];
            tileset[x, y, z] = t;
            tileset.DepthAt[x, y] += 1;
        }

        private Tile GenerateTile(XmlReader reader)
        {
            foreach (KeyValuePair<TileNameAttribute, Type> kv in TilesetXmlLoader.TileTypes)
            {
                bool willHandle = false;
                if (kv.Key.Type != this.m_Current)
                    continue;
                if (this.m_Current == XmlLoadMode.Tiles)
                    willHandle = kv.Key.Name.ToLower() == this.m_TilesetType.ToLower();
                else
                    willHandle = kv.Key.Name.ToLower() == reader.Name.ToLower();
                if (willHandle)
                {
                    Dictionary<string, string> attributes = new Dictionary<string, string>();
                    bool b = reader.MoveToFirstAttribute();
                    if (b == false)
                    {
                        // no attributes
                    }
                    else
                    {
                        do
                        {
                            string val = reader.Value;
                            if (this.m_Current == XmlLoadMode.Tiles &&
                                (reader.Name == "x" || reader.Name == "y"))
                            {
                                if (reader.Name == "x")
                                    val = (Convert.ToInt32(val) * Tileset.TILESET_CELL_WIDTH).ToString();
                                else if (reader.Name == "y")
                                    val = (Convert.ToInt32(val) * Tileset.TILESET_CELL_HEIGHT).ToString();
                            }
                            attributes.Add(reader.Name, val);
                        }
                        while (reader.MoveToNextAttribute());
                    }

                    // Create the tile.
                    return kv.Value.GetConstructor(new Type[] { typeof(Dictionary<string, string>) }).Invoke(new object[] { attributes }) as Tile;
                }
            }

            return null;
        }

        private static void LoadTypes()
        {
            TilesetXmlLoader.TileTypes = new Dictionary<TileNameAttribute, Type>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (typeof(Tile).IsAssignableFrom(t))
                    {
                        object[] tnas = t.GetCustomAttributes(typeof(TileNameAttribute), false);
                        if (tnas.Length < 1) continue;
                        TilesetXmlLoader.TileTypes.Add(tnas[0] as TileNameAttribute, t);
                    }
                }
            }
        }

        public static Tileset Load(string filename)
        {
            if (TilesetXmlLoader.TileTypes == null)
                TilesetXmlLoader.LoadTypes();
            return new TilesetXmlLoader().LoadXml("", filename);
        }

        public static Tileset[] MultiLoad(string filename, string[] levelNames)
        {
            if (TilesetXmlLoader.TileTypes == null)
                TilesetXmlLoader.LoadTypes();
            List<Tileset> tilesets = new List<Tileset>();
            for (int i = 0; i < levelNames.Length; i++)
                tilesets.Add(new TilesetXmlLoader().LoadXml(levelNames[i], filename));
            return tilesets.ToArray();
        }
    }
}
