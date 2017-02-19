using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A post-processing render pass for a custom shader.  You can create
    /// this shader via <see cref="IGraphicsFactory"/> if you don't want to
    /// strongly type your shader.
    /// </summary>
    /// <module>Graphics</module>
    public interface ICustomPostProcessingRenderPass : IRenderPass
    {
        IAssetReference<EffectAsset> Effect { get; }

        /// <summary>
        /// Sets the custom shader parameter to the specified boolean.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new boolean value.</param>
        void SetValue(string name, bool value);

        /// <summary>
        /// Sets the custom shader parameter to the specified integer.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new integer value.</param>
        void SetValue(string name, int value);

        /// <summary>
        /// Sets the custom shader parameter to the specified matrix.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new matrix.</param>
        void SetValue(string name, Matrix value);

        /// <summary>
        /// Sets the custom shader parameter to the specified array of matrixes.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new array of matrixes.</param>
        void SetValue(string name, Matrix[] value);

        /// <summary>
        /// Sets the custom shader parameter to the specified quaternion.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new quaternion.</param>
        void SetValue(string name, Quaternion value);

        /// <summary>
        /// Sets the custom shader parameter to the specified floating point value.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new floating point value.</param>
        void SetValue(string name, float value);

        /// <summary>
        /// Sets the custom shader parameter to the specified array of floating point values.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new array of floating point values.</param>
        void SetValue(string name, float[] value);

        /// <summary>
        /// Sets the custom shader parameter to the specified texture.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new texture.</param>
        void SetValue(string name, Texture value);

        /// <summary>
        /// Sets the custom shader parameter to the specified 2-dimensional vector.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new 2-dimensional vector.</param>
        void SetValue(string name, Vector2 value);
        
        /// <summary>
        /// Sets the custom shader parameter to the specified array of 2-dimensional vectors.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new array of 2-dimensional vectors.</param>
        void SetValue(string name, Vector2[] value);

        /// <summary>
        /// Sets the custom shader parameter to the specified 3-dimensional vector.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new 3-dimensional vector.</param>
        void SetValue(string name, Vector3 value);

        /// <summary>
        /// Sets the custom shader parameter to the specified array of 3-dimensional vectors.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new array of 3-dimensional vectors.</param>
        void SetValue(string name, Vector3[] value);

        /// <summary>
        /// Sets the custom shader parameter to the specified 4-dimensional vector.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new 4-dimensional vector.</param>
        void SetValue(string name, Vector4 value);

        /// <summary>
        /// Sets the custom shader parameter to the specified array of 4-dimensional vectors.
        /// </summary>
        /// <param name="name">The name of the parameter to set.</param>
        /// <param name="value">The new array of 4-dimensional vectors.</param>
        void SetValue(string name, Vector4[] value);

    }
}
