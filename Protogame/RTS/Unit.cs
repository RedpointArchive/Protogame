using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Protogame;
using Protogame.MultiLevel;
using Protogame.RTS;
using Protogame.RTS.Multiplayer;
#if MULTIPLAYER
using Process4.Collections;
#endif

namespace Protogame.RTS
{
    public abstract class Unit : MultiLevelEntity, IDynamicRenderingEntity
    {
        public Unit(Level initial)
            : base(initial)
        {
        }

        /// <summary>
        /// The unit's team.
        /// </summary>
        public Team Team
        {
            get;
            set;
        }

        public UnitSynchronisationData SynchronisationData
        {
            get;
            private set;
        }

        #region Multiplayer Event Handling
        
#if MULTIPLAYER
        public void __Event_MoveSynchronisationHandler(object sender, EventArgs ev)
        {
            if (this is MovableUnit)
                (this as MovableUnit).MoveTo(new Vector2((ev as MoveEventArgs).X, (ev as MoveEventArgs).Y));
        }

        public void __Event_AttackSynchronisationHandler(object sender, EventArgs ev)
        {
            if (this is AttackingUnit)
            {
                foreach (IMultiLevelEntity e in this.Level.Entities)
                {
                    if (e is Unit && (e as Unit).SynchronisationData != null)
                    {
                        string udata = ((e as Unit).SynchronisationData as Process4.Interfaces.ITransparent).NetworkName;
                        if (udata == (ev as AttackEventArgs).UnitNetworkName)
                        {
                            (this as AttackingUnit).AttackTarget = e as Unit;
                            (this.Level.World as RTSWorld).UiManager.Log("received remote attack request and started attack.");
                            return;
                        }
                    }
                }
            }

            (this.Level.World as RTSWorld).UiManager.Log("received remote attack request but no appropriate unit found.");
        }
#endif

        public void SetSynchronisationName(string name)
        {
            // Register and switch synchronisation data.
#if MULTIPLAYER
            if (!this.LocallyOwned && this.SynchronisationData != null)
                this.SynchronisationData.Move -= __Event_MoveSynchronisationHandler;
            if (!this.LocallyOwned && this.SynchronisationData != null)
                this.SynchronisationData.Attack -= __Event_AttackSynchronisationHandler;
            this.SynchronisationData = new Distributed<UnitSynchronisationData>(name);
            if (!this.LocallyOwned && this.SynchronisationData != null)
                this.SynchronisationData.Move += __Event_MoveSynchronisationHandler;
            if (!this.LocallyOwned && this.SynchronisationData != null)
                this.SynchronisationData.Attack += __Event_AttackSynchronisationHandler;
#else
            this.SynchronisationData = new UnitSynchronisationData();
#endif
        }

        #endregion

        /// <summary>
        /// The maximum amount of health points this unit has (field).
        /// </summary>
        private float m_MaxHealth;

        /// <summary>
        /// The maximum amount of health points this unit has.
        /// </summary>
        public virtual float MaxHealth
        {
            get
            {
                return this.m_MaxHealth;
            }
            protected set
            {
                this.m_MaxHealth = value;
                if (this.CurrentHealth == 0)
                    this.CurrentHealth = value;
            }
        }

        /// <summary>
        /// The current health of the unit.
        /// </summary>
        public virtual float CurrentHealth
        {
            get;
            private set;
        }

        /// <summary>
        /// The unit armor.
        /// </summary>
        public virtual float Armor
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether the unit is considered dead / killed.
        /// </summary>
        public bool Killed
        {
            get
            {
                return this.CurrentHealth == 0;
            }
        }

        /// <summary>
        /// Returns whether the default sprite should be rendered for this unit.
        /// </summary>
        /// <param name="world">The world that owns this entity.</param>
        /// <returns>Whether the default sprite should be rendered for this unit.</returns>
        public bool ShouldRender(World world)
        {
            return (world as MultiLevelWorld).ActiveLevel == this.Level;
        }

        /// <summary>
        /// Called when the "left" action occurs in the UI manager.  Usually
        /// reserved for selection.
        /// </summary>
        /// <returns>Whether the action has been handled.</returns>
        public abstract bool LeftAction(RTSWorld world);

        /// <summary>
        /// Called when the "right" action occurs in the UI manager.  Usually
        /// reserved for attack or movement.
        /// </summary>
        /// <returns>Whether the action has been handled.</returns>
        public abstract bool RightAction(RTSWorld world);

        /// <summary>
        /// Damages this unit by the specified amount, killing it if applicable.
        /// </summary>
        /// <param name="source">The unit which damaged this unit, if applicable (null otherwise).</param>
        /// <param name="amount">The initial damage done to the unit, prior to damage reduction.</param>
        public virtual void Damage(Unit source, float amount)
        {
            this.CurrentHealth -= Math.Max(amount - this.Armor, 0);
            (this.Level.World as RTSWorld).UiManager.Log("unit took " + Math.Max(amount - this.Armor, 0) + " damage.");

            // Check to see if we are dead.
            if (this.CurrentHealth < 0)
                this.Kill();
        }

        /// <summary>
        /// Heals this unit by the specified amount.
        /// </summary>
        /// <param name="amount">The healing done to the unit.</param>
        public void Heal(float amount)
        {
            this.CurrentHealth += Math.Max(amount, 0);
        }

        /// <summary>
        /// Handles death of the unit.
        /// </summary>
        protected virtual void Kill()
        {
            // TODO: You probably want to do something more advanced like
            // a death animation here.
            this.Level.Entities.Remove(this);
        }

        public override void Draw(World world, XnaGraphics graphics)
        {
            base.Draw(world, graphics);

            if (this.Width == 16 && this.CurrentHealth > 0 && this.MaxHealth > 0)
                graphics.DrawSprite((int)this.X, (int)this.Y, this.Width, this.Height, "meters.health.i" + ((int)(16 - (this.CurrentHealth / this.MaxHealth * 15))).ToString(), Color.White, false);

            // You can override to show more meters in subclasses.
        }

        /// <summary>
        /// Whether the instance of this unit is owned by the local player.
        /// </summary>
        public bool LocallyOwned
        {
            get
            {
#if MULTIPLAYER
                return this.Team.SynchronisationData.PlayerID == (this.Level.World as RTSWorld).LocalPlayer.PlayerID;
#else
                return this.Team.SynchronisationData.PlayerID == Player.ID_PLAYER;
#endif
            }
        }

        /// <summary>
        /// Force resynchronsiation of data with global session.
        /// </summary>
        public void ForceSynchronisation()
        {
            // Make sure this is a synchronised unit.
            if (this.SynchronisationData == null ||
                this.Team.SynchronisationData == null)
                throw new InvalidOperationException("Unable to synchronise unit that does not have associated synchronisation data.");

            // Find team instance with the specified player ID.
            Team teamInstance = (this.Level.World as RTSWorld).Teams.DefaultIfEmpty(null).First(v => (v.SynchronisationData.PlayerID == this.Team.SynchronisationData.PlayerID));
            if (teamInstance == null)
                throw new ProtogameException("Unable to find team instance with player ID '" + this.Team.SynchronisationData.PlayerID + "'.");
            this.Team = teamInstance;
        }
    }
}
