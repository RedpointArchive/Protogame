using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;

namespace Protogame.Platformer
{
    public abstract class PlayerWorld : FadingWorld
    {
        /// <summary>
        /// A reference to the player entity.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Loads the specified Ogmo Editor level from the Resources/ folder, performing transition
        /// effects as this world defines them.
        /// </summary>
        /// <param name="name">The name of the Ogmo Editor level as it appears on disk, without the extension or leading path.</param>
        public override void LoadLevel(string name)
        {
            this.Player = null;
            base.LoadLevel(name);
        }

        /// <summary>
        /// Spawns a specified player at the specified location, replacing the existing
        /// player instance in the world.
        /// </summary>
        /// <typeparam name="T">The type of player to spawn.</typeparam>
        /// <param name="x">The X position to spawn the player at.</param>
        /// <param name="y">The Y position to spawn the player at.</param>
        public void SpawnPlayer<T>(float x, float y) where T : Player, new()
        {
            if (this.Player != null)
                this.Entities.Remove(this.Player);
            this.Player = new T() { X = x, Y = y };
            this.Entities.Add(this.Player);
        }
    }
}
