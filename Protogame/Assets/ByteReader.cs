using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Protogame
{
    public static class ByteReader
    {
        public static byte[] ReadAsByteArray(dynamic value)
        {
            if (value is JArray)
                return ((JArray)value).Select(x => (byte)x).ToArray();
            else if (value is JValue)
                return Convert.FromBase64String(value.ToString());
            else if (value is byte[])
                return value;
            else if (value == null)
                return null;
            else
                throw new Exception("Unknown type to convert to byte array: " + value.GetType().FullName);
        }
    }
}
