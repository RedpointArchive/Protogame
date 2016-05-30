namespace Protogame
{
    using System.Runtime.InteropServices;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Graphics.PackedVector;

    /// <summary>
    /// Represents a vertex with bone information attached.
    /// </summary>
    /// <remarks>
    /// This is the structure used for model vertexes that is passed to the GPU.  It
    /// incorporates the position, normal and colors, as well as the bone indices
    /// and bone weightings that impact this vertex.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalColorBlendable : IVertexType
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
        /// The diffuse color of the vertex.
        /// </summary>
        public readonly Vector3 Color;

        /// <summary>
        /// The bone weightings that apply to the vertex.
        /// </summary>
        public readonly Vector4 BoneWeights;

        /// <summary>
        /// The bone IDs that apply to the vertex.
        /// </summary>
        public readonly Byte4 BoneIndices;

        /// <summary>
        /// Initializes static members of the <see cref="VertexPositionNormalColorBlendable"/> struct.
        /// </summary>
        static VertexPositionNormalColorBlendable()
        {
            VertexElement[] elements =
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), 
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), 
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Color, 0), 
                new VertexElement(36, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0), 
                new VertexElement(52, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0)
            };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPositionNormalTextureBlendable"/> struct.
        /// </summary>
        /// <param name="position">
        /// The position of the vertex.
        /// </param>
        /// <param name="normal">
        /// The normal of the vertex.
        /// </param>
        /// <param name="textureCoordinate">
        /// The texture coordinate for the vertex.
        /// </param>
        /// <param name="boneWeight">
        /// The bone weightings that apply to the vertex.
        /// </param>
        /// <param name="boneIndices">
        /// The bone IDs that apply to the vertex.
        /// </param>
        public VertexPositionNormalColorBlendable(
            Vector3 position, 
            Vector3 normal, 
            Color color, 
            Vector4 boneWeight, 
            Byte4 boneIndices)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
            this.BoneWeights = boneWeight;
            this.BoneIndices = boneIndices;
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
        public static bool operator ==(VertexPositionNormalColorBlendable left, VertexPositionNormalColorBlendable right)
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
        public static bool operator !=(VertexPositionNormalColorBlendable left, VertexPositionNormalColorBlendable right)
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
                "{{Position:{0} Normal:{1} Color:{2} BlendWeight:{3} BlendIndexes:{4}}}", 
                new object[] { this.Position, this.Normal, this.Color, this.BoneWeights, this.BoneIndices });
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
        public bool Equals(VertexPositionNormalColorBlendable other)
        {
            return this.Position.Equals(other.Position) && this.Normal.Equals(other.Normal) && this.Color.Equals(other.Color) && this.BoneWeights.Equals(other.BoneWeights) && this.BoneIndices.Equals(other.BoneIndices);
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

            return obj is VertexPositionNormalColorBlendable && this.Equals((VertexPositionNormalColorBlendable)obj);
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
                hashCode = (hashCode * 397) ^ this.Color.GetHashCode();
                hashCode = (hashCode * 397) ^ this.BoneWeights.GetHashCode();
                hashCode = (hashCode * 397) ^ this.BoneIndices.GetHashCode();
                return hashCode;
            }
        }
    }
}