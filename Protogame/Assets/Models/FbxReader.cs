using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quaternion = Microsoft.Xna.Framework.Quaternion;

namespace Protogame
{
    public class FbxReader
    {
        public IModel Load(byte[] data)
        {
            var file = Path.GetTempFileName() + ".fbx";
            using (var stream = new FileStream(file, FileMode.Create))
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }

            var model = this.Load(file);

            File.Delete(file);

            return model;
        }

        public IModel Load(string filename)
        {
            // Import the scene via AssImp.
            var importer = new AssimpImporter();
            importer.AttachLogStream(new LogStream((msg, userData) => { }));
            const PostProcessSteps ProcessFlags =
                PostProcessSteps.FlipUVs | PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.Triangulate
                | PostProcessSteps.SortByPrimitiveType | PostProcessSteps.FindInvalidData;
            var scene = importer.ImportFile(filename, ProcessFlags);

            // Create the list of animations.
            var animations = new List<IAnimation>();

            // Create the null animation (just the basic mesh).
            animations.Add(this.CreateNullAnimation(scene));

            // Import the basic animation.
            foreach (var assimpAnimation in scene.Animations)
            {
                animations.Add(this.ImportAnimation(scene, assimpAnimation));
            }

            // Return the resulting model.
            return new Model(new AnimationCollection(animations));
        }

        private IAnimation CreateNullAnimation(Scene scene)
        {
            var vertexes = new List<VertexPositionNormalTexture>();
            var indices = new List<int>();

            // TODO: Assume one mesh for now.
            var mesh = scene.Meshes[0];

            // Import vertexes.
            // TODO: What to do with multiple texture coords?
            var uvs = mesh.GetTextureCoords(0);
            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var pos = mesh.Vertices[i];
                var normal = mesh.Normals[i];
                var uv = uvs[i];

                vertexes.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(pos.X, pos.Y, pos.Z),
                        new Vector3(normal.X, normal.Y, normal.Z),
                        new Vector2(uv.X, uv.Y)));
            }

            // Import indicies.
            indices.AddRange(mesh.GetIntIndices());

            // Create frame 0.
            var frame = new Frame(vertexes.ToArray(), indices.ToArray());

            return new Animation(Animation.AnimationNullName, new[] { frame }, 0, 0);
        }

        private IAnimation ImportAnimation(Scene scene, Assimp.Animation assimpAnimation)
        {
            var baseVertexes = new List<VertexPositionNormalTexture>();
            var baseIndices = new List<int>();

            // TODO: Assume one mesh for now.
            var mesh = scene.Meshes[0];

            // Import vertexes.
            // TODO: What to do with multiple texture coords?
            var uvs = mesh.GetTextureCoords(0);
            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var pos = mesh.Vertices[i];
                var normal = mesh.Normals[i];
                var uv = uvs[i];

                baseVertexes.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(pos.X, pos.Y, pos.Z),
                        new Vector3(normal.X, normal.Y, normal.Z),
                        new Vector2(uv.X, uv.Y)));
            }

            // Import indicies.
            baseIndices.AddRange(mesh.GetIntIndices());

            // For each of the frames in the animation, calculate what they look like and
            // add them.
            var frames = new List<Frame>();
            for (var i = 0; i < assimpAnimation.DurationInTicks; i++)
            {
                frames.Add(this.ImportFrame(baseVertexes, baseIndices, scene, assimpAnimation, i + 1));
            }

            return new Animation(assimpAnimation.Name, frames.ToArray(), assimpAnimation.TicksPerSecond, assimpAnimation.DurationInTicks);
        }

        private Frame ImportFrame(List<VertexPositionNormalTexture> baseVertexes, List<int> baseIndices, Scene scene, Assimp.Animation assimpAnimation, int i)
        {
            var vertexes = this.CopyVertexes(baseVertexes);
            var indices = this.CopyIndices(baseIndices);

            var mesh = scene.Meshes[0];

            // Calculate the hierarchy as it would appear at a given point in time.  All transformations
            // in the tree are applicable to that particular node, but do not incorporate the transformations
            // of it's parent.
            var hierarchyAtTime = new NodeAtTime();
            this.CalculateHierarchyAtTime(scene, hierarchyAtTime, scene.RootNode, mesh, assimpAnimation, i);

            // Now traverse the nodes in the hierarchy and apply all of their translations to the applicable
            // vertexes and bones.
            this.TraverseHierarchyAndApplyTransforms(vertexes, hierarchyAtTime, mesh);

            return new Frame(vertexes.ToArray(), indices.ToArray());
        }

        private void CalculateHierarchyAtTime(Scene scene, NodeAtTime nodeAtTime, Node assimpNode, Mesh mesh, Assimp.Animation assimpAnimation, int i)
        {
            nodeAtTime.Name = assimpNode.Name;
            nodeAtTime.Transform = this.MatrixFromAssImpMatrix(assimpNode.Transform);

            Vector3 translation, scale;
            Quaternion rotation;
            nodeAtTime.Transform.Decompose(out scale, out rotation, out translation);

            var channel = assimpAnimation.NodeAnimationChannels.FirstOrDefault(x => x.NodeName == nodeAtTime.Name);

            if (channel != null)
            {
                var bone = mesh.Bones.FirstOrDefault(x => assimpNode.Name.StartsWith(x.Name + "_$"));
                if (bone != null)
                {
                    Vector3 boneTranslation, boneScale;
                    Quaternion boneRotation;
                    this.MatrixFromAssImpMatrix(bone.OffsetMatrix)
                        .Decompose(out boneScale, out boneRotation, out boneTranslation);
                }

                nodeAtTime.Transform = Matrix.Identity;

                nodeAtTime.Transform *= this.CalculateInterpolatedPosition(channel.PositionKeys, i);
                nodeAtTime.Transform *= this.CalculateInterpolatedRotation(channel.RotationKeys, i);
                nodeAtTime.Transform *= this.CalculateInterpolatedScaling(channel.ScalingKeys, i);
            }

            if (assimpNode.ChildCount > 0)
            {
                foreach (var child in assimpNode.Children)
                {
                    var childNodeAtTime = new NodeAtTime();
                    childNodeAtTime.Parent = nodeAtTime;
                    nodeAtTime.Children.Add(childNodeAtTime);
                    this.CalculateHierarchyAtTime(scene, childNodeAtTime, child, mesh, assimpAnimation, i);
                }
            }
        }

        private Matrix CalculateInterpolatedPosition(VectorKey[] positionKeys, int i)
        {
            if (positionKeys.Length == 1)
            {
                return Matrix.CreateTranslation(new Vector3(
                    positionKeys[0].Value.X,
                    positionKeys[0].Value.Y,
                    positionKeys[0].Value.Z));
            }

            var currentIndex = i - 1;
            var nextIndex = currentIndex + 1 >= positionKeys.Length ? 0 : currentIndex + 1;
            
            // TODO: Actual interpolation.

            return Matrix.CreateTranslation(new Vector3(
                positionKeys[currentIndex].Value.X,
                positionKeys[currentIndex].Value.Y,
                positionKeys[currentIndex].Value.Z));
        }

        private Matrix CalculateInterpolatedRotation(QuaternionKey[] rotationKeys, int i)
        {
            if (rotationKeys.Length == 1)
            {
                return Matrix.CreateFromQuaternion(new Quaternion(
                    rotationKeys[0].Value.X,
                    rotationKeys[0].Value.Y,
                    rotationKeys[0].Value.Z,
                    rotationKeys[0].Value.W));
            }

            var currentIndex = i - 1;
            var nextIndex = currentIndex + 1 >= rotationKeys.Length ? 0 : currentIndex + 1;

            // TODO: Actual interpolation.

            return Matrix.CreateFromQuaternion(new Quaternion(
                rotationKeys[currentIndex].Value.X,
                rotationKeys[currentIndex].Value.Y,
                rotationKeys[currentIndex].Value.Z,
                rotationKeys[currentIndex].Value.W));
        }

        private Matrix CalculateInterpolatedScaling(VectorKey[] scalingKeys, int i)
        {
            if (scalingKeys.Length == 1)
            {
                return Matrix.CreateScale(new Vector3(
                    scalingKeys[0].Value.X,
                    scalingKeys[0].Value.Y,
                    scalingKeys[0].Value.Z));
            }

            var currentIndex = i - 1;
            var nextIndex = currentIndex + 1 >= scalingKeys.Length ? 0 : currentIndex + 1;

            // TODO: Actual interpolation.

            return Matrix.CreateScale(new Vector3(
                scalingKeys[currentIndex].Value.X,
                scalingKeys[currentIndex].Value.Y,
                scalingKeys[currentIndex].Value.Z));
        }

        private Matrix MatrixFromAssImpMatrix(Assimp.Matrix4x4 matrix)
        {
            return new Matrix(
                 matrix.A1, matrix.B1, matrix.C1, matrix.D1,
                 matrix.A2, matrix.B2, matrix.C2, matrix.D2,
                 matrix.A3, matrix.B3, matrix.C3, matrix.D3,
                 matrix.A4, matrix.B4, matrix.C4, matrix.D4);
        }

        private class NodeAtTime
        {
            public NodeAtTime Parent { get; set; }

            public List<NodeAtTime> Children { get; set; } 

            public Matrix Transform { get; set; }

            public string Name { get; set; }

            public NodeAtTime()
            {
                this.Children = new List<NodeAtTime>();
            }
        }

        private void TraverseHierarchyAndApplyTransforms(
            List<VertexPositionNormalTexture> vertexes,
            NodeAtTime node,
            Mesh mesh)
        {
            // Find the bone associated with this node.  Each transformation is split up
            // in the FBX format, but the bone maps to the deepest node.  Thus when we
            // match exactly, we'll apply all of the parent transformations for the bone
            // that are relevant.
            var bone = mesh.Bones.FirstOrDefault(x => x.Name == node.Name);

            if (bone != null)
            {
                var finalMatrix = this.MatrixFromAssImpMatrix(bone.OffsetMatrix);

                var applicationNode = node;
                while (applicationNode != null)
                {
                    finalMatrix *= applicationNode.Transform;
                    applicationNode = applicationNode.Parent;
                }

                foreach (var vertexWeight in bone.VertexWeights)
                {
                    var vertex = vertexes[(int)vertexWeight.VertexID];

                    // TODO: What about normals!?!?!?
                    vertexes[(int)vertexWeight.VertexID] =
                        new VertexPositionNormalTexture(
                            Vector3.Transform(vertex.Position, finalMatrix),
                            vertex.Normal,
                            vertex.TextureCoordinate);
                }
            }

            // Descend into the hierarchy further.
            foreach (var child in node.Children)
            {
                this.TraverseHierarchyAndApplyTransforms(vertexes, child, mesh);
            }
        }

        private List<VertexPositionNormalTexture> CopyVertexes(List<VertexPositionNormalTexture> baseVertexes)
        {
            return baseVertexes.Select(vertex => new VertexPositionNormalTexture(vertex.Position, vertex.Normal, vertex.TextureCoordinate)).ToList();
        }

        private List<int> CopyIndices(List<int> baseIndices)
        {
            return baseIndices.Select(index => index).ToList();
        }
    }
}
