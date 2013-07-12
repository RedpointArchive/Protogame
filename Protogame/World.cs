using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using System.IO;

#if FALSE

namespace Protogame
{
    public abstract class World : IWorld
    {
        /// <summary>
        /// The base directory in which the Resources/ folder is located.
        /// </summary>
        public static string BaseDirectory = "";

        /// <summary>
        /// The base directory in which the Content/ folder is located.
        /// </summary>
        public static string RuntimeDirectory = "";

        /// <summary>
        /// The current game context.
        /// </summary>
        public GameContext GameContext { get; set; }

        /// <summary>
        /// The current game reference.
        /// </summary>
        public Game Game { get; internal set; }

        /// <summary>
        /// The current tileset.
        /// </summary>
        public virtual Tileset Tileset { get; protected set; }

        /// <summary>
        /// A list of entities that are currently active in the world.
        /// </summary>
        public virtual List<IEntity> Entities { get; private set; }

        /// <summary>
        /// The name of the current level that is loaded.
        /// </summary>
        public string CurrentLevel { get; protected set; }

        /// <summary>
        /// The current world tick.
        /// </summary>
        public long Tick { get; internal set; }

        /// <summary>
        /// Creates a new game world.
        /// </summary>
        public World()
        {
            this.Tileset = null;
            this.Entities = new List<IEntity>();
        }

        /// <summary>
        /// Loads the specified Ogmo Editor level from the Resources/ folder, performing transition
        /// effects as this world defines them.
        /// </summary>
        /// <param name="name">The name of the Ogmo Editor level as it appears on disk, without the extension or leading path.</param>
        public virtual void LoadLevel(string name)
        {
            this.LoadLevelImmediate(name);
        }

        /// <summary>
        /// Loads the specified Ogmo Editor level from the Resources/ folder immediately, taking care
        /// of stopping audio entities, clearing the player and recreating the world from the tileset.
        /// </summary>
        /// <param name="name">The name of the Ogmo Editor level as it appears on disk, without the extension or leading path.</param>
        protected void LoadLevelImmediate(string name)
        {
            // Perform the actual level switch.
            foreach (IEntity ee in this.Entities)
                if (ee is AudioEntity)
                    (ee as AudioEntity).Stop();
            this.Entities.Clear();
            this.Tileset = TilesetXmlLoader.Load(Path.Combine(World.BaseDirectory, "Resources/" + name + ".oel"));
            this.CurrentLevel = name;

            foreach (Tile t in this.Tileset.AsLinear())
                if (t is EntityTile)
                    this.Entities.Add(t as EntityTile);
        }

        /// <summary>
        /// Performs any custom drawing events for this world, prior to everything else being drawn.
        /// </summary>
        /// <param name="context">The game context.</param>
        public abstract void DrawBelow(GameContext context);

        /// <summary>
        /// Performs any custom drawing events for this world, after everything else has been drawn.
        /// </summary>
        /// <param name="context">The game context.</param>
        public abstract void DrawAbove(GameContext context);

        /// <summary>
        /// Updates the game world each tick.  Returns whether processing should continue in this tick or
        /// whether entity processing should be skipped.
        /// </summary>
        /// <param name="context">The current context of the game.</param>
        /// <returns>Whether processing should continue in this tick.</returns>
        public abstract bool Update(GameContext context);

        public byte RenderDepthValue { get; set; }
        public byte RenderDepthUpRange { get; set; }
        public byte RenderDepthDownRange { get; set; }
    }
}

#endif
