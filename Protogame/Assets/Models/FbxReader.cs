namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
#if PLATFORM_LINUX
    using System.Reflection;
#endif
    using Assimp;
#if PLATFORM_LINUX
    using Assimp.Unmanaged;
#endif
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics.PackedVector;
    using Quaternion = Microsoft.Xna.Framework.Quaternion;

    /// <summary>
    /// Reads an FBX file using AssImp and converts it to an <see cref="IModel"/>, which can be used
    /// at runtime for rendering, or serialized for storage as an asset.
    /// </summary>
    public class FbxReader
    {
        /// <summary>
        /// Load an FBX file from an array of bytes.
        /// </summary>
        /// <param name="data">
        /// The byte array containing the FBX data.
        /// </param>
        /// <param name="extension">
        /// The file extension for raw model data.
        /// </param>
        /// <param name="rawAdditionalAnimations">
        /// A dictionary mapping of animation names to byte arrays for additional FBX files to load.
        /// </param>
        /// <returns>
        /// The loaded <see cref="IModel"/>.
        /// </returns>
        public IModel Load(byte[] data, string extension, Dictionary<string, byte[]> rawAdditionalAnimations)
        {
            var file = Path.GetTempFileName() + "." + extension;
            using (var stream = new FileStream(file, FileMode.Create))
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }

            var additionalAnimationFiles = new Dictionary<string, string>();
            if (rawAdditionalAnimations != null)
            {
                foreach (var kv in rawAdditionalAnimations)
                {
                    var tempFile = Path.GetTempFileName() + "." + extension;
                    using (var stream = new FileStream(tempFile, FileMode.Create))
                    {
                        stream.Write(kv.Value, 0, kv.Value.Length);
                        stream.Close();
                    }

                    additionalAnimationFiles.Add(kv.Key, tempFile);
                }
            }

            var model = this.Load(file, additionalAnimationFiles);

            File.Delete(file);

            foreach (var kv in additionalAnimationFiles)
            {
                File.Delete(kv.Value);
            }

            return model;
        }

        /// <summary>
        /// Load an FBX file from a file on disk.
        /// </summary>
        /// <param name="filename">
        /// The filename of the FBX file to load.
        /// </param>
        /// <param name="additionalAnimationFiles">
        /// A dictionary mapping of animation names to filenames for additional FBX files to load.
        /// </param>
        /// <returns>
        /// The loaded <see cref="IModel"/>.
        /// </returns>
        public IModel Load(string filename, Dictionary<string, string> additionalAnimationFiles)
        {
            this.LoadAssimpLibrary();

            // Import the scene via AssImp.
            var importer = new AssimpContext();
            const PostProcessSteps ProcessFlags =
                PostProcessSteps.FlipUVs | PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.Triangulate
                | PostProcessSteps.SortByPrimitiveType | PostProcessSteps.FindInvalidData
                | PostProcessSteps.FlipWindingOrder;
            var scene = importer.ImportFile(filename, ProcessFlags);

            var boneWeightingMap = this.BuildBoneWeightingMap(scene);

            var boneHierarchy = this.ImportBoneHierarchy(scene.RootNode, scene.Meshes[0]);

            // Create the list of animations, including the null animation.
            var animations = new List<IAnimation>();

            // Import the basic animation.
            if (scene.AnimationCount > 0)
            {
                animations.AddRange(
                    scene.Animations.Select(
                        assimpAnimation => this.ImportAnimation(assimpAnimation.Name, assimpAnimation)));
            }

            // For each additional animation file, import that and add the animation to the existing scene.
            animations.AddRange(
                from kv in additionalAnimationFiles
                let animationImporter = new AssimpContext()
                let additionalScene = animationImporter.ImportFile(kv.Value, ProcessFlags)
                where additionalScene.AnimationCount == 1
                select this.ImportAnimation(kv.Key, additionalScene.Animations[0]));

            // Return the resulting model.
            return new Model(
                new AnimationCollection(animations), 
                boneHierarchy, 
                this.ImportVertexes(scene, boneWeightingMap), 
                this.ImportIndices(scene));
        }

        /// <summary>
        /// Imports the FBX bone hierarchy of the specified mesh, converting each node in the hierarchy
        /// to a model bone.
        /// </summary>
        /// <remarks>
        /// This function recursively traverses the hierarchy, calling itself each time with a different <see cref="node"/>
        /// argument.
        /// </remarks>
        /// <param name="node">
        /// The current node to import.
        /// </param>
        /// <param name="mesh">
        /// The mesh that is being imported.
        /// </param>
        /// <returns>
        /// The imported bone hierarchy.
        /// </returns>
        private IModelBone ImportBoneHierarchy(Node node, Mesh mesh)
        {
            var childBones =
                node.Children.Select(child => this.ImportBoneHierarchy(child, mesh)).ToDictionary(k => k.Name, v => v);

            if (mesh.Bones.Count > 48)
            {
                throw new InvalidOperationException("This model contains more bones than the supported maximum (48).");
            }

            var boneIndex = -1;
            var offsetMatrix = Matrix.Identity;
            for (var i = 0; i < mesh.Bones.Count; i++)
            {
                if (mesh.Bones[i].Name == node.Name)
                {
                    boneIndex = i;
                    offsetMatrix = this.MatrixFromAssImpMatrix(mesh.Bones[i].OffsetMatrix);
                    break;
                }
            }

            var transform = this.MatrixFromAssImpMatrix(node.Transform);
            Vector3 translation, scale;
            Quaternion rotation;
            transform.Decompose(out scale, out rotation, out translation);

            return new ModelBone(boneIndex, node.Name, childBones, offsetMatrix, translation, rotation, scale);
        }

        /// <summary>
        /// Builds a bone weighting map.  A bone weighting map is a map of each vertex to the
        /// bone indexes and weightings that apply to it.
        /// </summary>
        /// <remarks>
        /// This is used to build up the map which is assigned against the vertexes in the model.  These
        /// bone indices and weightings are then pushed through to the hardware in the BLENDWEIGHTS
        /// and BLENDINDICES semantic fields.
        /// </remarks>
        /// <param name="scene">
        /// The scene to build the map from.
        /// </param>
        /// <returns>
        /// The bone weighting map.
        /// </returns>
        private Dictionary<Vector3, KeyValuePair<Byte4, Vector4>> BuildBoneWeightingMap(Scene scene)
        {
            var map = new Dictionary<Vector3, List<KeyValuePair<int, float>>>();

            for (var i = 0; i < scene.Meshes[0].BoneCount; i++)
            {
                var bone = scene.Meshes[0].Bones[i];

                for (var w = 0; w < bone.VertexWeightCount; w++)
                {
                    var assimpVertex = scene.Meshes[0].Vertices[bone.VertexWeights[w].VertexID];
                    var vertex = new Vector3(assimpVertex.X, assimpVertex.Y, assimpVertex.Z);

                    if (!map.ContainsKey(vertex))
                    {
                        map[vertex] = new List<KeyValuePair<int, float>>();
                    }

                    var list = map[vertex];

                    if (list.All(x => x.Key != i))
                    {
                        list.Add(new KeyValuePair<int, float>(i, bone.VertexWeights[w].Weight));
                    }
                }
            }

            var boneWeightMap = new Dictionary<Vector3, KeyValuePair<Byte4, Vector4>>();

            foreach (var kv in map)
            {
                var indices = new Vector4();
                var weights = new Vector4();

                if (kv.Value.Count >= 1)
                {
                    indices.X = kv.Value[0].Key;
                    weights.X = kv.Value[0].Value;
                }

                if (kv.Value.Count >= 2)
                {
                    indices.Y = kv.Value[1].Key;
                    weights.Y = kv.Value[1].Value;
                }

                if (kv.Value.Count >= 3)
                {
                    indices.Z = kv.Value[2].Key;
                    weights.Z = kv.Value[2].Value;
                }

                if (kv.Value.Count >= 4)
                {
                    indices.W = kv.Value[3].Key;
                    weights.W = kv.Value[3].Value;
                }

                if (kv.Value.Count >= 5)
                {
                    throw new InvalidOperationException("A vertex has more than 4 bones associated with it.");
                }

                boneWeightMap.Add(kv.Key, new KeyValuePair<Byte4, Vector4>(new Byte4(indices), weights));
            }

            return boneWeightMap;
        }

        /// <summary>
        /// Imports the mesh indices from the scene.
        /// </summary>
        /// <param name="scene">
        /// The scene that contains the mesh.
        /// </param>
        /// <returns>
        /// The imported indices.
        /// </returns>
        private int[] ImportIndices(Scene scene)
        {
            var mesh = scene.Meshes[0];

            return mesh.GetIndices();
        }

        /// <summary>
        /// Imports the mesh vertexes from the scene and adds the appropriate bone weighting data to each vertex.
        /// </summary>
        /// <param name="scene">
        /// The scene that contains the mesh.
        /// </param>
        /// <param name="boneWeightingMap">
        /// The bone weighting map.
        /// </param>
        /// <returns>
        /// The imported vertexes.
        /// </returns>
        private VertexPositionNormalTextureBlendable[] ImportVertexes(
            Scene scene, 
            Dictionary<Vector3, KeyValuePair<Byte4, Vector4>> boneWeightingMap)
        {
            var mesh = scene.Meshes[0];

            var vertexes = new List<VertexPositionNormalTextureBlendable>();

            // Import vertexes.
            // TODO: What to do with multiple texture coords?
            var uvs = mesh.TextureCoordinateChannels[0];
            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var pos = mesh.Vertices[i];
                var normal = mesh.Normals[i];
                var uv = i < uvs.Count ? uvs[i] : new Vector3D(0, 0, 0);

                var posVector = new Vector3(pos.X, pos.Y, pos.Z);

                var boneIndices = new Byte4();
                var boneWeights = new Vector4();

                if (boneWeightingMap.ContainsKey(posVector))
                {
                    boneIndices = boneWeightingMap[posVector].Key;
                    boneWeights = boneWeightingMap[posVector].Value;
                }

                vertexes.Add(
                    new VertexPositionNormalTextureBlendable(
                        posVector, 
                        new Vector3(normal.X, normal.Y, normal.Z), 
                        new Vector2(uv.X, uv.Y), 
                        boneWeights, 
                        boneIndices));
            }

            return vertexes.ToArray();
        }

        /// <summary>
        /// Creates a <see cref="Matrix"/> from an AssImp <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="matrix">
        /// The AssImp matrix to convert.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/> representing the same transformation.
        /// </returns>
        private Matrix MatrixFromAssImpMatrix(Matrix4x4 matrix)
        {
            return new Matrix(
                matrix.A1, 
                matrix.B1, 
                matrix.C1, 
                matrix.D1, 
                matrix.A2, 
                matrix.B2, 
                matrix.C2, 
                matrix.D2, 
                matrix.A3, 
                matrix.B3, 
                matrix.C3, 
                matrix.D3, 
                matrix.A4, 
                matrix.B4, 
                matrix.C4, 
                matrix.D4);
        }

        /// <summary>
        /// Imports an FBX animation, storing the transformations that apply to each bone at each time.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <param name="assimpAnimation">
        /// The AssImp animation to apply to the scene.
        /// </param>
        /// <returns>
        /// The <see cref="IAnimation"/> representing the imported animation.
        /// </returns>
        private IAnimation ImportAnimation(string name, Assimp.Animation assimpAnimation)
        {
            var translation = new Dictionary<string, IDictionary<double, Vector3>>();
            var rotation = new Dictionary<string, IDictionary<double, Quaternion>>();
            var scale = new Dictionary<string, IDictionary<double, Vector3>>();

            foreach (var animChannel in assimpAnimation.NodeAnimationChannels)
            {
                if (animChannel.HasPositionKeys)
                {
                    if (!translation.ContainsKey(animChannel.NodeName))
                    {
                        translation[animChannel.NodeName] = new Dictionary<double, Vector3>();
                    }

                    foreach (var positionKey in animChannel.PositionKeys)
                    {
                        var vector = new Vector3(positionKey.Value.X, positionKey.Value.Y, positionKey.Value.Z);

                        translation[animChannel.NodeName].Add(positionKey.Time, vector);
                    }
                }

                if (animChannel.HasRotationKeys)
                {
                    if (!rotation.ContainsKey(animChannel.NodeName))
                    {
                        rotation[animChannel.NodeName] = new Dictionary<double, Quaternion>();
                    }

                    foreach (var rotationKey in animChannel.RotationKeys)
                    {
                        var quaternion = new Quaternion(
                            rotationKey.Value.X, 
                            rotationKey.Value.Y, 
                            rotationKey.Value.Z, 
                            rotationKey.Value.W);

                        rotation[animChannel.NodeName].Add(rotationKey.Time, quaternion);
                    }
                }

                if (animChannel.HasScalingKeys)
                {
                    if (!scale.ContainsKey(animChannel.NodeName))
                    {
                        scale[animChannel.NodeName] = new Dictionary<double, Vector3>();
                    }

                    foreach (var scaleKey in animChannel.ScalingKeys)
                    {
                        var vector = new Vector3(scaleKey.Value.X, scaleKey.Value.Y, scaleKey.Value.Z);

                        scale[animChannel.NodeName].Add(scaleKey.Time, vector);
                    }
                }
            }

            return new Animation(
                name, 
                assimpAnimation.TicksPerSecond, 
                assimpAnimation.DurationInTicks, 
                translation, 
                rotation, 
                scale);
        }

#if PLATFORM_LINUX
        /// <summary>
        /// Loads the AssImp library.
        /// </summary>
        private void LoadAssimpLibrary()
        {
            var targetDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;

            try
            {
                AssimpLibrary.Instance.LoadLibrary(
                    Path.Combine(targetDir, "libassimp_32.so.3.0.1"), 
                    Path.Combine(targetDir, "libassimp_64.so.3.0.1"));
            }
            catch (AssimpException ex)
            {
                if (ex.Message == "Assimp library already loaded.")
                    return;
                throw;
            }
        }
#elif PLATFORM_WINDOWS
        /// <summary>
        /// Loads the AssImp library.
        /// </summary>
        private void LoadAssimpLibrary()
        {
            // Assimp.NET already has the correct values for Windows.
        }

#else
        /// <summary>
        /// Loads the AssImp library.
        /// </summary>
        private void LoadAssimpLibrary()
        {
            // Assimp.NET will not work on this platform anyway.
        }
#endif
    }
}