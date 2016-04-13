namespace Protogame
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a skeletal bone in a 3D model.
    /// </summary>
    public interface IModelBone
    {
        /// <summary>
        /// Gets the ID of the skeletal bone.
        /// </summary>
        /// <value>
        /// The ID of the skeletal bone.
        /// </value>
        int ID { get; }

        /// <summary>
        /// Gets the name of the skeletal bone.
        /// </summary>
        /// <value>
        /// The name of the skeletal bone.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the children of this bone.
        /// </summary>
        /// <value>
        /// The children of this bone.
        /// </value>
        IDictionary<string, IModelBone> Children { get; }

        /// <summary>
        /// Gets or sets the parent of the bone.
        /// </summary>
        /// <remarks>
        /// You should not change this after the construction of the model hierarchy.
        /// </remarks>
        /// <value>
        /// The parent of this bone.
        /// </value>
        IModelBone Parent { get; set; }

        /// <summary>
        /// Gets the bone offset matrix for this bone.
        /// </summary>
        /// <value>
        /// The bone offset matrix for this bone.
        /// </value>
        Matrix BoneOffset { get; }

        /// <summary>
        /// Gets the default translation vector for this bone.
        /// </summary>
        /// <value>
        /// The default translation vector for this bone.
        /// </value>
        Vector3 DefaultTranslation { get; }

        /// <summary>
        /// Gets the default rotation quaternion for this bone.
        /// </summary>
        /// <value>
        /// The default rotation quaternion for this bone.
        /// </value>
        Quaternion DefaultRotation { get; }

        /// <summary>
        /// Gets the default scale vector for this bone.
        /// </summary>
        /// <value>
        /// The default scale vector for this bone.
        /// </value>
        Vector3 DefaultScale { get; }

        /// <summary>
        /// Gets or sets the current translation vector for this bone.
        /// </summary>
        /// <value>
        /// The current translation vector for this bone.
        /// </value>
        Vector3 CurrentTranslation { get; set; }

        /// <summary>
        /// Gets or sets the current rotation quaternion for this bone.
        /// </summary>
        /// <value>
        /// The current rotation quaternion for this bone.
        /// </value>
        Quaternion CurrentRotation { get; set; }

        /// <summary>
        /// Gets or sets the current scale vector for this bone.
        /// </summary>
        /// <value>
        /// The current scale vector for this bone.
        /// </value>
        Vector3 CurrentScale { get; set; }

        /// <summary>
        /// Return the bone structure as a linear array.
        /// </summary>
        /// <returns>The bone structure as a linear array.</returns>
        IModelBone[] Flatten();

        /// <summary>
        /// Converts the current translation, rotation and scale into a matrix.
        /// </summary>
        /// <returns>A matrix representing the transformation for this individual bone.</returns>
        Matrix GetBoneMatrix();

        /// <summary>
        /// Combines the current matrixes into a final matrix for vertex transformation.
        /// </summary>
        /// <returns>A matrix representing the transformation to be applied to a vertex.</returns>
        Matrix GetFinalMatrix();
    }
}