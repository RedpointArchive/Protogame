using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    }
}
