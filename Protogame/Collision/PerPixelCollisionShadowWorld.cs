using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class PerPixelCollisionShadowWorld : IDisposable
    {
        private readonly IHierarchy _hierarchy;
        private readonly IEventEngine<IPerPixelCollisionEventContext> _perPixelCollisionEventEngine;
        private readonly IDebugRenderer _debugRenderer;
        private HashSet<WeakReference<IPerPixelCollisionComponent>> _components;

        private bool _waitForChanges;
        private List<Action> _pendingChanges;

        public PerPixelCollisionShadowWorld(
            IHierarchy hierarchy,
            IEventEngine<IPerPixelCollisionEventContext> perPixelCollisionEventEngine,
            IDebugRenderer debugRenderer)
        {
            _hierarchy = hierarchy;
            _perPixelCollisionEventEngine = perPixelCollisionEventEngine;
            _debugRenderer = debugRenderer;
            _components = new HashSet<WeakReference<IPerPixelCollisionComponent>>();

            _waitForChanges = false;
            _pendingChanges = new List<Action>();
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var toRemove = new List<WeakReference<IPerPixelCollisionComponent>>();

            _waitForChanges = true;

            foreach (var aWR in _components)
            {
                IPerPixelCollisionComponent a;
                if (!aWR.TryGetTarget(out a))
                {
                    toRemove.Add(aWR);
                    continue;
                }

                if (!(a.Texture?.IsReady ?? false))
                {
                    continue;
                }

                var aParent = _hierarchy.Lookup(a)?.Parent?.UntypedValue as IHasTransform;

                var aRectangle = CalculateBoundingBox(aParent, a);

                foreach (var bWR in _components)
                {
                    IPerPixelCollisionComponent b;
                    if (!bWR.TryGetTarget(out b))
                    {
                        toRemove.Add(bWR);
                        continue;
                    }

                    if (ReferenceEquals(a, b))
                    {
                        continue;
                    }

                    if (!(b.Texture?.IsReady ?? false))
                    {
                        continue;
                    }

                    var bParent = _hierarchy.Lookup(b)?.Parent?.UntypedValue as IHasTransform;

                    var bRectangle = CalculateBoundingBox(bParent, b);

                    if (aRectangle.Intersects(bRectangle))
                    {
                        if (aParent != null && bParent != null)
                        {
                            if (IntersectPixels(aParent, a, bParent, b))
                            {
                                _perPixelCollisionEventEngine.Fire(
                                    new DefaultPerPixelCollisionEventContext(),
                                    new PerPixelCollisionEvent(
                                        gameContext,
                                        null,
                                        updateContext,
                                        aParent,
                                        bParent));
                            }
                        }
                    }
                }
            }

            foreach (var tr in toRemove.Distinct())
            {
                _components.Remove(tr);
            }

            if (_pendingChanges.Count > 0)
            {
                foreach (var v in _pendingChanges)
                {
                    v();
                }
                _pendingChanges.Clear();
            }

            _waitForChanges = false;
        }

        private Rectangle CalculateBoundingBox(IHasTransform parent, IPerPixelCollisionComponent collisionComponent)
        {
            var transform =
                Matrix.CreateTranslation(-new Vector3(collisionComponent.RotationAnchor ?? Vector2.Zero, 0)) *
                Matrix.CreateFromQuaternion(parent.FinalTransform.AbsoluteRotation) *
                //Matrix.CreateTranslation(new Vector3(collisionComponent.RotationAnchor ?? Vector2.Zero, 0)) *
                Matrix.CreateTranslation(parent.FinalTransform.AbsolutePosition);

            var width = collisionComponent.GetPixelWidth();
            var height = collisionComponent.GetPixelHeight();

            var tl = Vector2.Transform(Vector2.Zero, transform);
            var tr = Vector2.Transform(new Vector2(width, 0), transform);
            var bl = Vector2.Transform(new Vector2(0, height), transform);
            var br = Vector2.Transform(new Vector2(width, height), transform);

            var minX = Math.Min(Math.Min(tl.X, tr.X), Math.Min(bl.X, br.X));
            var minY = Math.Min(Math.Min(tl.Y, tr.Y), Math.Min(bl.Y, br.Y));
            var maxX = Math.Max(Math.Max(tl.X, tr.X), Math.Max(bl.X, br.X));
            var maxY = Math.Max(Math.Max(tl.Y, tr.Y), Math.Max(bl.Y, br.Y));

            return new Rectangle(
                (int)Math.Floor(minX),
                (int)Math.Floor(minY),
                (int)Math.Ceiling(maxX) - (int)Math.Floor(minX),
                (int)Math.Ceiling(maxY) - (int)Math.Floor(minY));
        }

        private bool IntersectPixels(
            IHasTransform aParent,
            IPerPixelCollisionComponent a,
            IHasTransform bParent,
            IPerPixelCollisionComponent b)
        {
            var transformA =
                Matrix.CreateTranslation(-new Vector3(a.RotationAnchor ?? Vector2.Zero, 0)) *
                Matrix.CreateFromQuaternion(aParent.FinalTransform.AbsoluteRotation) *
                //Matrix.CreateTranslation(new Vector3(a.RotationAnchor ?? Vector2.Zero, 0)) *
                Matrix.CreateTranslation(aParent.FinalTransform.AbsolutePosition);
            var transformB =
                Matrix.CreateTranslation(-new Vector3(b.RotationAnchor ?? Vector2.Zero, 0)) *
                Matrix.CreateFromQuaternion(bParent.FinalTransform.AbsoluteRotation) *
                //Matrix.CreateTranslation(new Vector3(b.RotationAnchor ?? Vector2.Zero, 0)) *
                Matrix.CreateTranslation(bParent.FinalTransform.AbsolutePosition);

            var widthA = a.GetPixelWidth();
            var heightA = a.GetPixelHeight();
            var widthB = b.GetPixelWidth();
            var heightB = b.GetPixelHeight();

            var dataA = a.GetPixelData();
            var dataB = b.GetPixelData();

            var transformAToB = transformA * Matrix.Invert(transformB);
            
            var stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            var stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);
            
            var yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);
            
            for (var yA = 0; yA < heightA; yA++)
            {
                var posInB = yPosInB;
                
                for (var xA = 0; xA < widthA; xA++)
                {
                    var xB = (int)Math.Round(posInB.X);
                    var yB = (int)Math.Round(posInB.Y);
                    
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        var colorA = dataA[xA + yA * widthA];
                        var colorB = dataB[xB + yB * widthB];
                        
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            return true;
                        }
                    }
                    
                    posInB += stepX;
                }
                
                yPosInB += stepY;
            }
            
            return false;
        }

        public void Dispose()
        {
        }

        public void UnregisterComponentInCurrentWorld(IPerPixelCollisionComponent collisionComponent)
        {
            if (_waitForChanges)
            {
                _pendingChanges.Add(() =>
                {
                    var toRemove = new List<WeakReference<IPerPixelCollisionComponent>>();

                    foreach (var wr in _components)
                    {
                        IPerPixelCollisionComponent ppc;
                        if (wr.TryGetTarget(out ppc))
                        {
                            if (ReferenceEquals(ppc, collisionComponent))
                            {
                                toRemove.Add(wr);
                            }
                        }
                        else
                        {
                            toRemove.Add(wr);
                        }
                    }

                    foreach (var tr in toRemove)
                    {
                        _components.Remove(tr);
                    }
                });
            }
            else
            {
                var toRemove = new List<WeakReference<IPerPixelCollisionComponent>>();

                foreach (var wr in _components)
                {
                    IPerPixelCollisionComponent ppc;
                    if (wr.TryGetTarget(out ppc))
                    {
                        if (ReferenceEquals(ppc, collisionComponent))
                        {
                            toRemove.Add(wr);
                        }
                    }
                    else
                    {
                        toRemove.Add(wr);
                    }
                }

                foreach (var tr in toRemove)
                {
                    _components.Remove(tr);
                }
            }
        }

        public void RegisterComponentInCurrentWorld(IPerPixelCollisionComponent collisionComponent)
        {
            if (_waitForChanges)
            {
                _pendingChanges.Add(() =>
                {
                    _components.Add(new WeakReference<IPerPixelCollisionComponent>(collisionComponent));
                });
            }
            else
            {
                _components.Add(new WeakReference<IPerPixelCollisionComponent>(collisionComponent));
            }
        }

        public void DebugRender(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<IDebugRenderPass>())
            {
                var debugRenderPass = renderContext.GetCurrentRenderPass<IDebugRenderPass>();

                if (debugRenderPass.EnabledLayers.OfType<PerPixelCollisionDebugLayer>().Any())
                {
                    foreach (var aWR in _components)
                    {
                        IPerPixelCollisionComponent a;
                        if (!aWR.TryGetTarget(out a))
                        {
                            continue;
                        }

                        if (!(a.Texture?.IsReady ?? false))
                        {
                            continue;
                        }

                        var aParent = _hierarchy.Lookup(a)?.Parent?.UntypedValue as IHasTransform;

                        var aRectangle = CalculateBoundingBox(aParent, a);

                        _debugRenderer.RenderDebugTriangle(
                            renderContext,
                            new Vector3(aRectangle.X, aRectangle.Y, 0),
                            new Vector3(aRectangle.X + aRectangle.Width, aRectangle.Y, 0),
                            new Vector3(aRectangle.X, aRectangle.Y + aRectangle.Height, 0),
                            Color.Orange,
                            Color.Orange,
                            Color.Orange);
                        _debugRenderer.RenderDebugTriangle(
                            renderContext,
                            new Vector3(aRectangle.X + aRectangle.Width, aRectangle.Y + aRectangle.Height, 0),
                            new Vector3(aRectangle.X + aRectangle.Width, aRectangle.Y, 0),
                            new Vector3(aRectangle.X, aRectangle.Y + aRectangle.Height, 0),
                            Color.Orange,
                            Color.Orange,
                            Color.Orange);
                    }
                }
            }
        }
    }
}
