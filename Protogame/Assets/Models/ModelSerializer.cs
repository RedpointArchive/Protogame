namespace Protogame
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Serializes and deserializes <see cref="IModel"/> for storage in a binary format.
    /// </summary>
    public class ModelSerializer
    {
        /// <summary>
        /// Deserialize the specified byte array into a concrete <see cref="Model"/> implementation.
        /// </summary>
        /// <param name="data">
        /// The byte array to deserialize.
        /// </param>
        /// <returns>
        /// The deserialized <see cref="Model"/>.
        /// </returns>
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

        /// <summary>
        /// Serializes the specified <see cref="IModel"/> into a byte array.
        /// </summary>
        /// <param name="model">
        /// The model to serialize.
        /// </param>
        /// <returns>
        /// The resulting byte array.
        /// </returns>
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

        /// <summary>
        /// The deserialize animations.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The deserialized animations.
        /// </returns>
        private IEnumerable<IAnimation> DeserializeAnimations(BinaryReader reader)
        {
            var animationCount = reader.ReadInt32();

            for (var i = 0; i < animationCount; i++)
            {
                var name = reader.ReadString();
                var ticksPerSecond = reader.ReadDouble();
                var durationInTicks = reader.ReadDouble();
                var frames = this.DeserializeFrames(reader);

                yield return new Animation(name, frames, ticksPerSecond, durationInTicks);
            }
        }

        /// <summary>
        /// The deserialize frames.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The deserialized frames.
        /// </returns>
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

        /// <summary>
        /// The deserialize indices.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The deserialized indices.
        /// </returns>
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

        /// <summary>
        /// The deserialize model.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The <see cref="Model"/>.
        /// </returns>
        private Model DeserializeModel(BinaryReader reader)
        {
            return new Model(new AnimationCollection(this.DeserializeAnimations(reader)));
        }

        /// <summary>
        /// The deserialize vertexes.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// The deserialized vertexes.
        /// </returns>
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

                vertexes.Add(
                    new VertexPositionNormalTexture(
                        new Vector3(posX, posY, posZ), 
                        new Vector3(normalX, normalY, normalZ), 
                        new Vector2(uvX, uvY)));
            }

            return vertexes.ToArray();
        }

        /// <summary>
        /// The serialize animations.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="availableAnimations">
        /// The available animations.
        /// </param>
        private void SerializeAnimations(BinaryWriter writer, IAnimationCollection availableAnimations)
        {
            writer.Write(availableAnimations.Count());

            foreach (var animation in availableAnimations)
            {
                writer.Write(animation.Name);
                writer.Write(animation.TicksPerSecond);
                writer.Write(animation.DurationInTicks);
                this.SerializeFrames(writer, animation.Frames);
            }
        }

        /// <summary>
        /// The serialize frames.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="frames">
        /// The frames.
        /// </param>
        private void SerializeFrames(BinaryWriter writer, IFrame[] frames)
        {
            writer.Write(frames.Length);

            for (var i = 0; i < frames.Length; i++)
            {
                this.SerializeVertexes(writer, frames[i].Vertexes);
                this.SerializeIndices(writer, frames[i].Indices);
            }
        }

        /// <summary>
        /// The serialize indices.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="indices">
        /// The indices.
        /// </param>
        private void SerializeIndices(BinaryWriter writer, int[] indices)
        {
            writer.Write(indices.Length);

            for (var i = 0; i < indices.Length; i++)
            {
                writer.Write(indices[i]);
            }
        }

        /// <summary>
        /// The serialize model.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        private void SerializeModel(BinaryWriter writer, IModel model)
        {
            this.SerializeAnimations(writer, model.AvailableAnimations);
        }

        /// <summary>
        /// The serialize vertexes.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="vertexes">
        /// The vertexes.
        /// </param>
        private void SerializeVertexes(BinaryWriter writer, VertexPositionNormalTexture[] vertexes)
        {
            writer.Write(vertexes.Length);

            for (var i = 0; i < vertexes.Length; i++)
            {
                writer.Write(vertexes[i].Position.X);
                writer.Write(vertexes[i].Position.Y);
                writer.Write(vertexes[i].Position.Z);
                writer.Write(vertexes[i].Normal.X);
                writer.Write(vertexes[i].Normal.Y);
                writer.Write(vertexes[i].Normal.Z);
                writer.Write(vertexes[i].TextureCoordinate.X);
                writer.Write(vertexes[i].TextureCoordinate.Y);
            }
        }
    }
}