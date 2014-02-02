namespace Protogame
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A utility class for reading an arbitrary value as a byte array.  This is used to
    /// convert JSON objects into byte arrays.
    /// </summary>
    public static class ByteReader
    {
        /// <summary>
        /// Read the specified object as a byte array.
        /// </summary>
        /// <param name="value">
        /// The value to read.
        /// </param>
        /// <returns>
        /// The byte array.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the input value can not be read as a byte array.
        /// </exception>
        public static byte[] ReadAsByteArray(dynamic value)
        {
            var array = value as JArray;
            if (array != null)
            {
                return array.Select(x => (byte)x).ToArray();
            }

            if (value is JValue)
            {
                return Convert.FromBase64String(value.ToString());
            }

            if (value is byte[])
            {
                return value;
            }

            if (value == null)
            {
                return null;
            }

            throw new InvalidOperationException("Unknown type to convert to byte array: " + value.GetType().FullName);
        }
    }
}