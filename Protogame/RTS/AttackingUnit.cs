using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Protogame.MultiLevel;

namespace Protogame.RTS
{
    public class AttackingUnit : Unit
    {
        public AttackingUnit(Level initial)
            : base(initial)
        {
        }

        /// <summary>
        /// The attack range in pixels of this unit.
        /// </summary>
        public virtual float AttackRange
        {
            get;
            set;
        }

        /// <summary>
        /// The attack power (damage done per attack) of this unit.
        /// </summary>
        public virtual float AttackPower
        {
            get;
            set;
        }

        /// <summary>
        /// The number of attacks that this unit does in a second.
        /// </summary>
        public virtual float AttackSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// The current attack unit.
        /// </summary>
        public Unit AttackTarget
        {
            get;
            set;
        }

        public override void Update(World world)
        {
            // Check to see if we're attacking anything,
            if (this.AttackTarget != null && this.AttackTarget.Team != this.Team && !this.Killed)
            {
                // Check to see if we need to move closer.
                if ((new Vector2(this.AttackTarget.X, this.AttackTarget.Y) - new Vector2(this.X, this.Y)).Length() > this.AttackRange)
                {
                    // Need to move closer, find a spot near the unit that
                    // is currently free.
                    if (this is MovableUnit)
                    {
                        // Check to make sure we're not already moving.
                        if (!(this as MovableUnit).MoveIsPathing)
                        {
                            (this.Level.World as RTSWorld).UiManager.Log("unit started pathing toward attack target.");
                            (this as MovableUnit).MoveTo(new Vector2(this.AttackTarget.X, this.AttackTarget.Y));
                        }
                        else
                            (this.Level.World as RTSWorld).UiManager.Log("unit attacking, out-of-range and pathing.");
                    }
                    else
                    {
                        (this.Level.World as RTSWorld).UiManager.Log("immovable unit attempting to attack unit outside range; cancelling.");
                        this.AttackTarget = null;
                    }
                }
                else
                {
                    // Otherwise, we are in range of the attack target, so we
                    // can kill it.
                    // TODO: Use a timer with attack speed here.
                    (this.Level.World as RTSWorld).UiManager.Log("unit damaging it's attack target.");
                    if (this is MovableUnit)
                        (this as MovableUnit).MoveCancel();
                    this.AttackTarget.Damage(this, this.AttackPower);
                }
            }

            base.Update(world);
        }

        public override void Damage(Unit source, float amount)
        {
            base.Damage(source, amount);

            // If an enemy attacked us, we should automatically attack back.
            // Change this if you want to have certain orders for units (for
            // example to force them to stand ground even when attacked).
            if (this.AttackTarget == null && source != null)
                this.AttackTarget = source;
        }

        public override bool LeftAction(RTSWorld world)
        {
            return false;
        }

        public override bool RightAction(RTSWorld world)
        {
            MouseState mouse = Mouse.GetState();

            // Cancel existing attack target.
            this.AttackTarget = null;

            // Handle enemy unit detection under mouse cursor here.
            foreach (IEntity e in world.Entities)
            {
                if (e is Unit)
                {
                    Unit u = e as Unit;
                    
                    // Check to see if the unit is under the mouse cursor.
                    if (u.CollidesAt<Unit>(world, mouse.X, mouse.Y) != u)
                        continue;
                    
                    // Check to see if this unit is on our team.
                    if (u.Team == this.Team)
                        continue;

                    // Check to make sure unit is synchronised across machines.
                    if (u.SynchronisationData == null)
                        throw new InvalidOperationException("Attack requested against unit that is not synchronised on network.  Ensure it was spawned correctly.");

                    // Set this unit as our target.
                    this.AttackTarget = u;
                    if (this is MovableUnit)
                        (this as MovableUnit).MoveCancel();
                    if (this.SynchronisationData != null && u.SynchronisationData != null && this.LocallyOwned)
                        this.SynchronisationData.BroadcastAttack(u.SynchronisationData);

                    // We are attacking this unit.
                    return true;
                }
            }

            return false;
        }
    }
}
