namespace Protogame
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An implementation of <see cref="IModelBone"/>.
    /// </summary>
    public class ModelBone : IModelBone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBone"/> class.
        /// </summary>
        /// <remarks>
        /// The ID of a model bone is -1 if there are no vertexes that are impacted by it's value.
        /// This means we actually send a subset of the bones over to the GPU; only the bones that
        /// have a non-negative ID are indexed.
        /// </remarks>
        /// <param name="id">
        /// The unique ID of the model bone, or -1 if this bone does not impact vertexes.
        /// </param>
        /// <param name="name">
        /// The name of the bone.
        /// </param>
        /// <param name="children">
        /// The children of this bone.
        /// </param>
        /// <param name="boneOffset">
        /// The bone offset matrix which is used as the initial value before applying
        /// this bone and all of it's parent's transformations.
        /// </param>
        /// <param name="defaultTranslation">
        /// The default translation value.
        /// </param>
        /// <param name="defaultRotation">
        /// The default rotation value.
        /// </param>
        /// <param name="defaultScale">
        /// The default scaling value.
        /// </param>
        public ModelBone(
            int id, 
            string name, 
            IDictionary<string, IModelBone> children, 
            Matrix boneOffset, 
            Vector3 defaultTranslation, 
            Quaternion defaultRotation, 
            Vector3 defaultScale)
        {
            this.ID = id;
            this.Name = name;
            this.Children = children;
            this.BoneOffset = boneOffset;
            this.DefaultTranslation = defaultTranslation;
            this.DefaultRotation = defaultRotation;
            this.DefaultScale = defaultScale;

            this.CurrentTranslation = this.DefaultTranslation;
            this.CurrentRotation = this.DefaultRotation;
            this.CurrentScale = this.DefaultScale;

            foreach (var child in this.Children)
            {
                child.Value.Parent = this;
            }
        }

        /// <summary>
        /// Gets the bone offset matrix for this bone.
        /// </summary>
        /// <value>
        /// The bone offset matrix for this bone.
        /// </value>
        public Matrix BoneOffset { get; private set; }

        /// <summary>
        /// Gets the children of this bone.
        /// </summary>
        /// <value>
        /// The children of this bone.
        /// </value>
        public IDictionary<string, IModelBone> Children { get; private set; }

        /// <summary>
        /// Gets or sets the current rotation quaternion for this bone.
        /// </summary>
        /// <value>
        /// The current rotation quaternion for this bone.
        /// </value>
        public Quaternion CurrentRotation { get; set; }

        /// <summary>
        /// Gets or sets the current scale vector for this bone.
        /// </summary>
        /// <value>
        /// The current scale vector for this bone.
        /// </value>
        public Vector3 CurrentScale { get; set; }

        /// <summary>
        /// Gets or sets the current translation vector for this bone.
        /// </summary>
        /// <value>
        /// The current translation vector for this bone.
        /// </value>
        public Vector3 CurrentTranslation { get; set; }

        /// <summary>
        /// Gets the default rotation quaternion for this bone.
        /// </summary>
        /// <value>
        /// The default rotation quaternion for this bone.
        /// </value>
        public Quaternion DefaultRotation { get; private set; }

        /// <summary>
        /// Gets the default scale vector for this bone.
        /// </summary>
        /// <value>
        /// The default scale vector for this bone.
        /// </value>
        public Vector3 DefaultScale { get; private set; }

        /// <summary>
        /// Gets the default translation vector for this bone.
        /// </summary>
        /// <value>
        /// The default translation vector for this bone.
        /// </value>
        public Vector3 DefaultTranslation { get; private set; }

        /// <summary>
        /// Gets the ID of the skeletal bone.
        /// </summary>
        /// <value>
        /// The ID of the skeletal bone.
        /// </value>
        public int ID { get; private set; }

        /// <summary>
        /// Gets the name of the skeletal bone.
        /// </summary>
        /// <value>
        /// The name of the skeletal bone.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the parent of the bone.
        /// </summary>
        /// <remarks>
        /// You should not change this after the construction of the model hierarchy.
        /// </remarks>
        /// <value>
        /// The parent of this bone.
        /// </value>
        public IModelBone Parent { get; set; }

        /// <summary>
        /// Return the bone structure as a linear array.
        /// </summary>
        /// <returns>
        /// The bone structure as a linear array.
        /// </returns>
        public IModelBone[] Flatten()
        {
            return PerformFlatten(this).ToArray();
        }

        /// <summary>
        /// Converts the current translation, rotation and scale into a matrix.
        /// </summary>
        /// <returns>
        /// A matrix representing the transformation for this individual bone.
        /// </returns>
        public Matrix GetBoneMatrix()
        {
            return Matrix.CreateTranslation(this.CurrentTranslation) * Matrix.CreateFromQuaternion(this.CurrentRotation)
                   * Matrix.CreateScale(this.CurrentScale);
        }

        /// <summary>
        /// Combines the current matrixes into a final matrix for vertex transformation.
        /// </summary>
        /// <returns>
        /// A matrix representing the transformation to be applied to a vertex.
        /// </returns>
        public Matrix GetFinalMatrix()
        {
            var finalMatrix = this.BoneOffset;

            IModelBone applicationNode = this;
            while (applicationNode != null)
            {
                var transform = applicationNode.GetBoneMatrix();

                finalMatrix *= transform;

                applicationNode = applicationNode.Parent;
            }

            return finalMatrix;
        }

        /// <summary>
        /// Performs the actual flattening of the bone hierarchy.
        /// </summary>
        /// <param name="modelBone">
        /// The current bone being flattened.
        /// </param>
        /// <returns>
        /// The flattened hierarchy.
        /// </returns>
        private static IEnumerable<IModelBone> PerformFlatten(IModelBone modelBone)
        {
            yield return modelBone;

            foreach (var child in modelBone.Children)
            {
                foreach (var entry in PerformFlatten(child.Value))
                {
                    yield return entry;
                }
            }
        }
    }
}