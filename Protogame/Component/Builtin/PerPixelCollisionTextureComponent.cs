using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;
using System;

namespace Protogame
{
    public class PerPixelCollisionTextureComponent : IUpdatableComponent, IEnabledComponent, IPerPixelCollisionComponent
    {
        private readonly INode _node;
        private readonly IPerPixelCollisionEngine _collisionEngine;

        private bool _registeredTexture;
        private Texture2D _cachedTextureAssetRef;

        private int _cachedPixelWidth;
        private int _cachedPixelHeight;
        private Color[] _cachedPixelData = new Color[0];
        private bool _enabled;

        public PerPixelCollisionTextureComponent(INode node, IPerPixelCollisionEngine collisionEngine)
        {
            _node = node;
            _collisionEngine = collisionEngine;

            Enabled = true;
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;

                    if (!_enabled)
                    {
                        if (_registeredTexture)
                        {
                            _collisionEngine.UnregisterComponentInCurrentWorld(this);
                            _registeredTexture = false;
                        }
                    }
                }
            }
        }

        public IAssetReference<TextureAsset> Texture { get; set; }

        public Vector2? RotationAnchor { get; set; }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (Texture != null)
            {
                if (Texture.IsReady)
                {
                    if (Texture.Asset.Texture != _cachedTextureAssetRef)
                    {
                        _cachedTextureAssetRef = Texture.Asset.Texture;
                        _cachedPixelWidth = _cachedTextureAssetRef.Width;
                        _cachedPixelHeight = _cachedTextureAssetRef.Height;
                        if (IsCompressedFormat(_cachedTextureAssetRef.Format))
                        {
                            throw new InvalidOperationException(
                                "The texture provided to the PerPixelCollisionTexture component is a " +
                                "compressed texture.  Only uncompressed textures can be used with the " +
                                "per-pixel collision algorithm, as the texture must be read to the CPU.");
                        }
                        else
                        {
                            _cachedPixelData = new Color[_cachedPixelWidth * _cachedPixelHeight];
                        }
                        _cachedTextureAssetRef.GetData(_cachedPixelData);
                    }
                }
            }

            Update();
        }

        private static bool IsCompressedFormat(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Dxt1:
                case SurfaceFormat.Dxt1a:
                case SurfaceFormat.Dxt1SRgb:
                case SurfaceFormat.Dxt3:
                case SurfaceFormat.Dxt3SRgb:
                case SurfaceFormat.Dxt5:
                case SurfaceFormat.Dxt5SRgb:
                case SurfaceFormat.RgbaAtcExplicitAlpha:
                case SurfaceFormat.RgbaAtcInterpolatedAlpha:
                case SurfaceFormat.RgbaPvrtc2Bpp:
                case SurfaceFormat.RgbaPvrtc4Bpp:
                case SurfaceFormat.RgbEtc1:
                case SurfaceFormat.RgbPvrtc2Bpp:
                case SurfaceFormat.RgbPvrtc4Bpp:
                    return true;
            }
            return false;
        }

        private void Update()
        {
            // Update the parent node's matrix based on the rigid body's state.
            var transformComponent = _node.Parent?.UntypedValue as IHasTransform;
            if (transformComponent != null)
            {
                if (!Enabled)
                {
                    if (_registeredTexture)
                    {
                        _collisionEngine.UnregisterComponentInCurrentWorld(this);
                        _registeredTexture = false;
                    }
                }
                else
                {
                    if (!_registeredTexture)
                    {
                        _collisionEngine.RegisterComponentInCurrentWorld(this);
                        _registeredTexture = true;
                    }
                }
            }
        }

        public int GetPixelWidth()
        {
            return _cachedPixelWidth;
        }

        public int GetPixelHeight()
        {
            return _cachedPixelHeight;
        }

        public Color[] GetPixelData()
        {
            return _cachedPixelData;
        }
    }
}
