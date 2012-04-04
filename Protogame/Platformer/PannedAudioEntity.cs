using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using Protogame.Platformer;
using Microsoft.Xna.Framework.Audio;

namespace Protogame.Platformer
{
    public class PannedAudioEntity : AudioEntity
    {
        public PannedAudioEntity(World world, string name)
            : base(world, name)
        {
        }

        public override void Update(World rawWorld)
        {
            PlayerWorld world = rawWorld as PlayerWorld;

            // Check to see if player is null and we should center the pan.
            if (world.Player == null)
            {
                this.m_Instance.Pan = 0.0f;
                return;
            }

            // Calculate standard position.
            float v = Math.Abs(this.X - world.Player.X) / 300f;
            if (v > 1) v = 1;
            this.m_Instance.Pan = (this.X > world.Player.X) ? v : -v;
            this.m_Instance.Volume = (1 - v) * this.VolumeMax;

            // Calculate wrapped position if needed.
            if (1 - v == 0)
            {
                // Potentially needs to do some wrapping calculations, offset by
                // the room width.
                v = Math.Abs(this.X - (world.Player.X - Tileset.TILESET_PIXEL_WIDTH)) / 300f;
                if (v > 1)
                {
                    // Other side.
                    v = Math.Abs(this.X - (world.Player.X + Tileset.TILESET_PIXEL_WIDTH)) / 300f;
                    if (v > 1) v = 1;
                    this.m_Instance.Pan = -v;
                }
                else
                    this.m_Instance.Pan = v;
                this.m_Instance.Volume = (1 - v) * this.VolumeMax;
            }

            if (this.m_IsPlaying && this.m_Instance.State != SoundState.Playing)
            {
                this.m_Instance.Play();
                this.RaiseOnPlay();
                this.m_IsPlaying = false;
            }
            else if (!this.m_IsPlaying && !this.IsLooped && this.m_Instance.State == SoundState.Stopped && this.m_HasPlayed)
            {
                if (this.DeleteWhenFinished)
                    this.m_World.Entities.Remove(this);
            }
        }
    }
}
