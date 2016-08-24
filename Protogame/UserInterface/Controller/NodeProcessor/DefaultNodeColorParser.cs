using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class DefaultNodeColorParser : INodeColorParser
    {
        public Color? Parse(string text)
        {
            try
            {
                if (text.StartsWith("#"))
                {
                    if (text.Length == 7)
                    {
                        return new Color(
                            Convert.ToInt32(text.Substring(1, 2), 16),
                            Convert.ToInt32(text.Substring(3, 2), 16),
                            Convert.ToInt32(text.Substring(5, 2), 16));
                    }

                    if (text.Length == 9)
                    {
                        return new Color(
                            Convert.ToInt32(text.Substring(1, 2), 16),
                            Convert.ToInt32(text.Substring(3, 2), 16),
                            Convert.ToInt32(text.Substring(5, 2), 16),
                            Convert.ToInt32(text.Substring(7, 2), 16));
                    }
                }

                if (text.Contains(","))
                {
                    var components = text.Split(',');
                    if (components.Length == 3)
                    {
                        return new Color(new Vector3(
                            float.Parse(components[0]),
                            float.Parse(components[1]),
                            float.Parse(components[2])));
                    }

                    if (components.Length == 4)
                    {
                        return new Color(new Vector3(
                            float.Parse(components[0]),
                            float.Parse(components[1]),
                            float.Parse(components[2])));
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}