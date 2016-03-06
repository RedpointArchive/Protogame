using Protoinject;

namespace Protogame
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;

    public class Agent
    {
        private readonly IHierarchy _hierarchy;

        private Vector2[] m_WallFeelers = null;

        public Agent(IHierarchy hierarchy, IEntity entity, IAI ai, float maxSpeed)
        {
            _hierarchy = hierarchy;
            this.Entity = entity;
            this.AI = ai;
            this.MaxSpeed = maxSpeed;
        }

        public IAI AI { get; set; }

        public Vector2 Heading { get; set; }

        public Vector2 LastAccel { get; set; }

        public float MaxSpeed { get; set; }

        public IEntity Entity { get; set; }

        public Vector2 Position
        {
            get
            {
                return new Vector2(this.Entity.LocalMatrix.Translation.X, this.Entity.LocalMatrix.Translation.Y);
            }

            set
            {
                this.Entity.LocalMatrix *= Matrix.CreateTranslation(
                    new Vector3(value.X, value.Y, 0) - this.Entity.LocalMatrix.Translation);
            }
        }

        public Vector2 Side
        {
            get
            {
                return new Vector2(-this.Heading.Y, this.Heading.X);
            }
        }

        public Vector2 Velocity { get; set; }

        public void EnableWallAvoidance(Vector2[] feelers = null)
        {
            if (feelers == null)
            {
                feelers = new[] {new Vector2(16, 0), new Vector2(8, -8), new Vector2(8, 8)};
            }
            
            m_WallFeelers = feelers;
        }

        public bool CanSee(IGameContext gameContext, Agent other)
        {
            // Agents can't see behind themselves.
            if (this.Unproject(other.Position).X < 0)
            {
                return false;
            }

            // Check with a raycast.
            Vector2? collisionPoint;
            IEntity closestEntity;
            float? closestIntersection;
            if (this.Raycast(
                gameContext,
                other.Position - this.Position,
                out closestEntity,
                out closestIntersection,
                out collisionPoint))
            {
                return closestEntity == other.Entity;
            }

            return false;
        }

        public bool CanSee(IGameContext gameContext, IEntity other)
        {
            // Agents can't see behind themselves.
            if (this.Unproject(new Vector2(other.LocalMatrix.Translation.X, other.LocalMatrix.Translation.Y)).X < 0)
            {
                return false;
            }

            // Check with a raycast.
            Vector2? collisionPoint;
            IEntity closestEntity;
            float? closestIntersection;
            if (this.Raycast(
                gameContext,
                new Vector2(other.LocalMatrix.Translation.X, other.LocalMatrix.Translation.Y) - this.Position,
                out closestEntity,
                out closestIntersection,
                out collisionPoint))
            {
                return closestEntity == other;
            }

            return false;
        }

        public bool Raycast(IGameContext gameContext, Vector2 direction, out IEntity closestEntity, out float? closestIntersection, out Vector2? collisionPoint)
        {
            // Project all agents and walls into local space.
            var entities = gameContext.World.GetEntitiesForWorld(_hierarchy);

            closestEntity = null;
            closestIntersection = null;
            collisionPoint = null;

            direction = Vector2.Normalize(direction);

            foreach (var wall in entities.OfType<Wall>())
            {
                var start = this.Unproject(wall.Start, direction);
                var end = this.Unproject(wall.End, direction);

                if ((start.Y > 0 && end.Y > 0) || (start.Y < 0 && end.Y < 0))
                {
                    // Wall does not cross 0 on the Y, therefore no intersection.
                    continue;
                }

                if (Wall.NormalOf(start, end).X > 0)
                {
                    // Wall is facing away from us, ignore.
                    continue;
                }

                var intersect = (-start.Y / ((end.Y - start.Y) / (end.X - start.X))) + start.X;

                if (intersect >= 0)
                {
                    // Found an intersection.
                    if (closestIntersection == null || intersect < closestIntersection.Value)
                    {
                        closestEntity = wall;
                        closestIntersection = intersect;
                        collisionPoint = this.Project(new Vector2(intersect, 0), direction);
                    }
                }
            }

            foreach (var agent in entities.OfType<Agent>().Where(x => x != this))
            {
                var pos = this.Unproject(agent.Position, direction);

                if (pos.Y > -8f && pos.Y < 8f)
                {
                    var yi = pos.Y;
                    var xi = pos.X - Math.Abs(Math.Cos(Math.Asin(Math.Abs(pos.Y / 10f))) * 10f);

                    var intersect = (float)xi;

                    if (intersect >= 0)
                    {
                        // Found an intersection with another agent.
                        if (closestIntersection == null || intersect < closestIntersection.Value)
                        {
                            closestEntity = agent.Entity;
                            closestIntersection = intersect;
                            collisionPoint = this.Project(new Vector2((float)xi, yi), direction);
                        }
                    }
                }
            }

            return collisionPoint != null;
        }

        public Vector2 Project(Vector2 local, Vector2? heading = null)
        {
            heading = heading ?? this.Heading;

            var matrix = Matrix.Identity;
            matrix *= Matrix.CreateRotationY(-(float)Math.Atan2(heading.Value.Y, heading.Value.X));
            matrix *= Matrix.CreateTranslation(new Vector3(this.Entity.LocalMatrix.Translation.X, 0, this.Entity.LocalMatrix.Translation.Y));

            var projected = Vector3.Transform(new Vector3(local.X, 0, local.Y), matrix);
            return new Vector2(projected.X, projected.Z);
        }

        public Vector2 ProjectAcceleration(Vector2 local)
        {
            var matrix = Matrix.Identity;
            matrix *= Matrix.CreateRotationY(-(float)Math.Atan2(this.Heading.Y, this.Heading.X));

            var projected = Vector3.Transform(new Vector3(local.X, 0, local.Y), matrix);
            return new Vector2(projected.X, projected.Z);
        }

        public void RenderDebug(IGameContext gameContext, IRenderContext renderContext, I2DRenderUtilities twodRenderUtilities)
        {
            for (var i = 0; i < 10; i++)
            {
                var r = MathHelper.ToRadians(36 * i);
                var r2 = MathHelper.ToRadians(36 * (i + 1));
                twodRenderUtilities.RenderLine(
                    renderContext,
                    this.Position + (new Vector2((float)Math.Sin(r), (float)Math.Cos(r)) * 8f),
                    this.Position + (new Vector2((float)Math.Sin(r2), (float)Math.Cos(r2)) * 8f),
                    Color.Gray);
            }

            twodRenderUtilities.RenderLine(
                renderContext,
                this.Project(new Vector2(0, 0)),
                this.Project(new Vector2(10, 0)),
                Color.OrangeRed);

            twodRenderUtilities.RenderLine(
                renderContext,
                this.Position,
                this.Position + (this.Heading * 10f),
                Color.Cyan);

            twodRenderUtilities.RenderLine(
                renderContext,
                this.Position,
                this.Position + (this.LastAccel * 10f),
                Color.Blue);

            twodRenderUtilities.RenderLine(
                renderContext,
                this.Position,
                this.Position + (this.Velocity * 10f),
                Color.Pink);

            foreach (var feeler in this.m_WallFeelers)
            {
                twodRenderUtilities.RenderLine(renderContext, this.Position, this.Project(feeler), Color.Orange);
            }

            this.AI.RenderDebug(gameContext, renderContext, this, twodRenderUtilities);
        }

        public Vector2 Unproject(Vector2 world, Vector2? heading = null)
        {
            heading = heading ?? this.Heading;

            var matrix = Matrix.Identity;
            matrix *= Matrix.CreateTranslation(new Vector3(-this.Entity.LocalMatrix.Translation.X, 0, -this.Entity.LocalMatrix.Translation.Y));
            matrix *= Matrix.CreateRotationY((float)Math.Atan2(heading.Value.Y, heading.Value.X));

            var unprojected = Vector3.Transform(new Vector3(world.X, 0, world.Y), matrix);
            return new Vector2(unprojected.X, unprojected.Z);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var accel = this.AI.Update(gameContext, updateContext, this);
            if (this.m_WallFeelers != null)
            {
                accel += this.AvoidWalls(gameContext);
            }

            this.LastAccel = accel;

            this.Velocity += accel;
            if (this.Velocity.Length() > this.MaxSpeed)
            {
                this.Velocity = Vector2.Normalize(this.Velocity) * this.MaxSpeed;
            }

            this.Position += this.Velocity;
            if (this.Velocity.Length() > 0.001f)
            {
                this.Heading = Vector2.Normalize(this.Velocity);
            }
        }

        private Vector2 AvoidWalls(IGameContext gameContext)
        {
            var walls = gameContext.World.GetEntitiesForWorld(_hierarchy).OfType<Wall>().ToList();
            var accel = Vector2.Zero;

            foreach (var feeler in this.m_WallFeelers)
            {
                Wall closestWall = null;
                Vector2? wallEmergeVector = null;
                float? closestIntersection = null;

                foreach (var wall in walls)
                {
                    var start = this.UnprojectFeeler(wall.Start, feeler);
                    var end = this.UnprojectFeeler(wall.End, feeler);

                    if ((start.Y > 0 && end.Y > 0) || (start.Y < 0 && end.Y < 0))
                    {
                        // Wall does not cross 0 on the Y, therefore no intersection.
                        continue;
                    }

                    if (Wall.NormalOf(start, end).X > 0)
                    {
                        // Wall is facing away from us, ignore.
                        continue;
                    }

                    var intersect = (-start.Y / ((end.Y - start.Y) / (end.X - start.X))) + start.X;

                    if (intersect >= 0 && intersect < feeler.Length())
                    {
                        // Found an intersection.
                        if (closestIntersection == null || intersect < closestIntersection.Value)
                        {
                            closestWall = wall;
                            closestIntersection = intersect;

                            var penetrationAmount = feeler.Length() - intersect;
                            var reversePenetration = new Vector2(-penetrationAmount, 0);

                            var rot = Matrix.CreateFromAxisAngle(
                                new Vector3(Wall.NormalOf(start, end), 0),
                                MathHelper.Pi);
                            var result = Vector3.Transform(new Vector3(reversePenetration, 0), rot);

                            wallEmergeVector = new Vector2(result.X, result.Y);
                        }
                    }
                }

                if (closestWall != null)
                {
                    accel += this.ProjectAcceleration(wallEmergeVector.Value);
                }
            }

            return accel;
        }

        private Vector2 UnprojectFeeler(Vector2 world, Vector2 feeler)
        {
            var matrix = Matrix.Identity;
            matrix *= Matrix.CreateTranslation(new Vector3(-this.Entity.LocalMatrix.Translation.X, 0, -this.Entity.LocalMatrix.Translation.Y));
            matrix *= Matrix.CreateRotationY((float)Math.Atan2(this.Heading.Y, this.Heading.X));
            matrix *= Matrix.CreateRotationY((float)Math.Atan2(feeler.Y, feeler.X));

            var unprojected = Vector3.Transform(new Vector3(world.X, 0, world.Y), matrix);
            return new Vector2(unprojected.X, unprojected.Z);
        }
    }
}