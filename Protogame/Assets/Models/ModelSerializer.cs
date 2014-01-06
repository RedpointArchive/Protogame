using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class ModelSerializer
    {
        public Model Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(memory))
                {
                    return this.DeserializeModel(reader);
                }
            }
        }

        private Model DeserializeModel(BinaryReader reader)
        {
            return new Model(new AnimationCollection(this.DeserializeAnimations(reader)));
        }

        private IEnumerable<IAnimation> DeserializeAnimations(BinaryReader reader)
        {
            var animationCount = reader.ReadInt32();

            for (var i = 0; i < animationCount; i++)
            {
                var name = reader.ReadString();
                var ticksPerSecond = reader.ReadDouble();
                var durationInTicks = reader.ReadDouble();
                var frames = this.DeserializeFrames(reader);

                yield return new Animation(
                    name,
                    frames,
                    ticksPerSecond,
                    durationInTicks);
            }
        }

        private IFrame[] DeserializeFrames(BinaryReader reader)
        {
            var frameCount = reader.ReadInt32();
            var frames = new List<IFrame>();

            for (var i = 0; i < frameCount; i++)
            {
                var vertexes = this.DeserializeVertexes(reader);
                var indices = this.DeserializeIndices(reader);

                frames.Add(new Frame(vertexes, indices));
            }

            return frames.ToArray();
        }

        private VertexPositionNormalTexture[] DeserializeVertexes(BinaryReader reader)
        {
            var vertexCount = reader.ReadInt32();
            var vertexes = new List<VertexPositionNormalTexture>();

            for (var i = 0; i < vertexCount; i++)
            {
                var posX = reader.ReadSingle();
                var posY = reader.ReadSingle();
                var posZ = reader.ReadSingle();
                var normalX = reader.ReadSingle();
                var normalY = reader.ReadSingle();
                var normalZ = reader.ReadSingle();
                var uvX = reader.ReadSingle();
                var uvY = reader.ReadSingle();

                vertexes.Add(new VertexPositionNormalTexture(
                    new Vector3(posX, posY, posZ),
                    new Vector3(normalX, normalY, normalZ),
                    new Vector2(uvX, uvY)));
            }

            return vertexes.ToArray();
        }

        private int[] DeserializeIndices(BinaryReader reader)
        {
            var indexCount = reader.ReadInt32();
            var indices = new List<int>();

            for (var i = 0; i < indexCount; i++)
            {
                indices.Add(reader.ReadInt32());
            }

            return indices.ToArray();
        }

        public byte[] Serialize(IModel model)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memory))
                {
                    this.SerializeModel(writer, model);

                    var length = memory.Position;
                    memory.Seek(0, SeekOrigin.Begin);
                    var bytes = new byte[length];
                    memory.Read(bytes, 0, (int)length);
                    return bytes;
                }
            }
        }

        private void SerializeModel(BinaryWriter writer, IModel model)
        {
            this.SerializeAnimations(writer, model.AvailableAnimations);
        }

        private void SerializeAnimations(BinaryWriter writer, IAnimationCollection availableAnimations)
        {
            writer.Write((int)availableAnimations.Count());

            foreach (var animation in availableAnimations)
            {
                writer.Write(animation.Name);
                writer.Write(animation.TicksPerSecond);
                writer.Write(animation.DurationInTicks);
                this.SerializeFrames(writer, animation.Frames);
            }
        }

        private void SerializeFrames(BinaryWriter writer, IFrame[] frames)
        {
            writer.Write((int)frames.Length);

            for (var i = 0; i < frames.Length; i++)
            {
                this.SerializeVertexes(writer, frames[i].Vertexes);
                this.SerializeIndices(writer, frames[i].Indices);
            }
        }

        private void SerializeVertexes(BinaryWriter writer, VertexPositionNormalTexture[] vertexes)
        {
            writer.Write((int)vertexes.Length);

            for (var i = 0; i < vertexes.Length; i++)
            {
                writer.Write((float)vertexes[i].Position.X);
                writer.Write((float)vertexes[i].Position.Y);
                writer.Write((float)vertexes[i].Position.Z);
                writer.Write((float)vertexes[i].Normal.X);
                writer.Write((float)vertexes[i].Normal.Y);
                writer.Write((float)vertexes[i].Normal.Z);
                writer.Write((float)vertexes[i].TextureCoordinate.X);
                writer.Write((float)vertexes[i].TextureCoordinate.Y);
            }
        }

        private void SerializeIndices(BinaryWriter writer, int[] indices)
        {
            writer.Write((int)indices.Length);

            for (var i = 0; i < indices.Length; i++)
            {
                writer.Write((int)indices[i]);
            }
        }
    }
}
