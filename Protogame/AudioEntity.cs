using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Protogame
{
#if NOT_MIGRATED
    public class AudioEntity : Entity
    {
        internal SoundEffectInstance m_Instance = null;
        internal bool m_IsPlaying = false;
        internal bool m_HasPlayed = false;
        internal IWorld m_World = null;
        private static Dictionary<Type, long> m_UniquePlayPerTickTracker = new Dictionary<Type, long>();

        public event EventHandler OnPlay;
        internal void RaiseOnPlay()
        {
            if (this.OnPlay != null)
                this.OnPlay(this, new EventArgs());
        }

        protected AudioEntity(IWorld world, string name)
        {
            this.m_World = world;
            this.m_Instance = world.GameContext.Sounds[name].CreateInstance();
            this.VolumeMax = 1;
        }

        public float Volume
        {
            get
            {
                return this.m_Instance.Volume;
            }
        }

        protected float VolumeMax { get; set; }
        protected bool DeleteWhenFinished { get; set; }
        protected bool UniquePlayPerTick { get; set; }

        protected bool IsLooped
        {
            get
            {
                return this.m_Instance.IsLooped;
            }
            set
            {
                this.m_Instance.IsLooped = true;
            }
        }

        protected void Play()
        {
            // Unique play per tick checker.
            if (this.UniquePlayPerTick && AudioEntity.m_UniquePlayPerTickTracker.Keys.Contains(this.GetType()) &&
                AudioEntity.m_UniquePlayPerTickTracker[this.GetType()] == this.m_World.Tick)
                return;
            else if (this.UniquePlayPerTick && !AudioEntity.m_UniquePlayPerTickTracker.Keys.Contains(this.GetType()))
                AudioEntity.m_UniquePlayPerTickTracker.Add(this.GetType(), this.m_World.Tick);
            else if (this.UniquePlayPerTick)
                AudioEntity.m_UniquePlayPerTickTracker[this.GetType()] = this.m_World.Tick;

            // Set the sound up for playing if we get to this point.
            this.m_IsPlaying = true;
            this.m_HasPlayed = true;
        }

        public void Stop()
        {
            this.m_IsPlaying = false;
            this.m_Instance.Stop();
        }

        public override string Image
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
#endif
}
