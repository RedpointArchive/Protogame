namespace Protogame
{
    using System.Runtime.InteropServices;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a vertex with only position and normal information.
    /// </summary>
    /// <remarks>
    /// This is the structure used for model vertexes that is passed to the GPU.  It
    /// incorporates the position and normal.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormal : IVertexType
    {
        /// <summary>
        /// The vertex declaration structure.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration;

        /// <summary>
        /// The position of the vertex.
        /// </summary>
        public readonly Vector3 Position;

        /// <summary>
        /// The normal of the vertex.
        /// </summary>
        public readonly Vector3 Normal;

        /// <summary>
        /// Initializes static members of the <see cref="VertexPositionNormal"/> struct.
        /// </summary>
        static VertexPositionNormal()
        {
            VertexElement[] elements =
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), 
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPositionNormal"/> struct.
        /// </summary>
        /// <param name="position">
        /// The position of the vertex.
        /// </param>
        /// <param name="normal">
        /// The normal of the vertex.
        /// </param>
        public VertexPositionNormal(
            Vector3 position, 
            Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
        }

        /// <summary>
        /// Gets the vertex declaration.
        /// </summary>
        /// <value>
        /// The vertex declaration.
        /// </value>
        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        /// <summary>
        /// The equality operator for this vertex structure.
        /// </summary>
        /// <param name="left">
        /// The first vertex to compare.
        /// </param>
        /// <param name="right">
        /// The second vertex to compare.
        /// </param>
        /// <returns>
        /// Whether or not the vertexes are equal.
        /// </returns>
        public static bool operator ==(VertexPositionNormal left, VertexPositionNormal right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// The inequality operator for this vertex structure.
        /// </summary>
        /// <param name="left">
        /// The first vertex to compare.
        /// </param>
        /// <param name="right">
        /// The second vertex to compare.
        /// </param>
        /// <returns>
        /// Whether or not the vertexes are not equal.
        /// </returns>
        public static bool operator !=(VertexPositionNormal left, VertexPositionNormal right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a string representation of the vertex.
        /// </summary>
        /// <returns>
        /// The string representing the vertex.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{{Position:{0} Normal:{1}}}", 
                new object[] { this.Position, this.Normal });
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare this instance to.
        /// </param>
        /// <returns>
        /// Whether or not this instance is equal to the specified object.
        /// </returns>
        public bool Equals(VertexPositionNormal other)
        {
            return this.Position.Equals(other.Position) && this.Normal.Equals(other.Normal);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare this instance to.
        /// </param>
        /// <returns>
        /// Whether or not this instance is equal to the specified object.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is VertexPositionNormal && this.Equals((VertexPositionNormal)obj);
        }

        /// <summary>
        /// Calculates the hash code representing the current instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Position.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Normal.GetHashCode();
                return hashCode;
            }
        }
    }
}