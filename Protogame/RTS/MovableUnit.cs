using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DeenGames.Utils.AStarPathFinder;
using Protogame;
using Protogame.MultiLevel;

namespace Protogame.RTS
{
    public class MovableUnit : AttackingUnit
    {
        private Stack<PathFinderNode> m_CurrentPath = null;

        public MovableUnit(Level initial)
            : base(initial)
        {
        }

        public virtual float MoveSpeed
        {
            get;
            set;
        }

        public Vector2 MoveTarget
        {
            get;
            private set;
        }

        public bool MoveTo(Vector2 target)
        {
            // If movement speed is 0, this unit can't currently move.
            if (this.MoveSpeed == 0)
            {
                (this.Level.World as RTSWorld).UiManager.Log("path find requested for unit with no move speed; cancelling request.");
                this.m_CurrentPath = null;
                return false;
            }

            // If the level has no pathfinding functionality, we can't solve
            // for the path.
            if (!(this.Level is PathFindingLevel))
            {
                (this.Level.World as RTSWorld).UiManager.Log("path find requested for level that provides no path finding mechanism.");
                this.m_CurrentPath = null;
                return false;
            }

            // Find path otherwise.
            List<PathFinderNode> list = (this.Level as PathFindingLevel).PathFindFast(new Vector2(this.X, this.Y), target);
            if (list == null || list.Count == 1)
            {
                this.m_CurrentPath = null;
                return false;
            }
            else
            {
                this.m_CurrentPath = new Stack<PathFinderNode>(list);
                this.m_CurrentPath.Pop();
                (this.Level.World as RTSWorld).UiManager.Log("" + this.m_CurrentPath.Count + " nodes to path.");
                if (this.SynchronisationData != null && this.LocallyOwned)
                    this.SynchronisationData.BroadcastMove(target.X, target.Y);
                return true;
            }
        }

        /// <summary>
        /// Cancel any currently outstanding movement target.
        /// </summary>
        public void MoveCancel()
        {
            this.m_CurrentPath = null;
        }

        /// <summary>
        /// Returns whether the unit is currently pathing to a location.
        /// </summary>
        public bool MoveIsPathing
        {
            get
            {
                return this.m_CurrentPath != null;
            }
        }

        public override void Update(World world)
        {
            // Enter the movement clause if needed.
            if (this.m_CurrentPath != null && this.MoveSpeed > 0 && !this.Killed)
            {
                // Check to see if we can move to the desired location.
                Unit u = this.CollidesAt<Unit>(world, this.m_CurrentPath.Peek().X * Tileset.TILESET_CELL_WIDTH, this.m_CurrentPath.Peek().Y * Tileset.TILESET_CELL_HEIGHT);
                if (true /*u == null || u == this*/) // FIXME: Units should path around obstacles.
                {
                    // Calculate relative vector to the target.
                    Vector2 max = new Vector2(
                        this.m_CurrentPath.Peek().X * Tileset.TILESET_CELL_WIDTH - this.X,
                        this.m_CurrentPath.Peek().Y * Tileset.TILESET_CELL_HEIGHT - this.Y
                        );

                    // Debugging information.
                    (world as RTSWorld).UiManager.Log("unit is moving to " + this.m_CurrentPath.Peek() + ".");

                    // If we are closer to the target than our move speed, jump directly to 
                    // it and pop the target location.
                    if (max.Length() < this.MoveSpeed)
                    {
                        // Move direct.
                        this.X += max.X;
                        this.Y += max.Y;
                        this.m_CurrentPath.Pop();
                    }
                    else
                    {
                        // Normalize the vector and multiply by the movement speed.
                        max.Normalize();
                        max *= this.MoveSpeed;
                        this.X += max.X;
                        this.Y += max.Y;
                    }
                }

                // Reset list if needed.
                if (this.m_CurrentPath.Count == 0)
                    this.m_CurrentPath = null;
            }

            base.Update(world);
        }

        public override bool LeftAction(RTSWorld world)
        {
            return base.LeftAction(world);
        }

        public override bool RightAction(RTSWorld world)
        {
            if (base.RightAction(world))
                return true;

            MouseState mouse = Mouse.GetState();
            if (!this.MoveTo(new Vector2(mouse.X, mouse.Y)))
            {
                (world as RTSWorld).UiManager.Log("unit can't find path to target.");
                return false;
            }
            else
                return true;
        }
    }
}
