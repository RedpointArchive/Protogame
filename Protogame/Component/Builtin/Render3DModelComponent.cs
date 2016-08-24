using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;

namespace Protogame
{
    public class Render3DModelComponent : IRenderableComponent, IEnabledComponent, IHasTransform
    {
        private readonly INode _node;

        private readonly I3DRenderUtilities _renderUtilities;

        private readonly ITextureFromHintPath _textureFromHintPath;
        private readonly IRenderBatcher _renderBatcher;

        private readonly IAssetManager _assetManager;

        private ModelAsset _lastCachedModel;

        private TextureAsset _lastCachedDiffuseTexture;

        private TextureAsset _lastCachedNormalMapTexture;

        private TextureAsset _lastCachedSpecularColorMapTexture;

        private Color? _lastCachedSpecularColor;

        private float? _lastCachedSpecularPower;

        private bool _useDefaultEffects;

        private UberEffectAsset _uberEffectAsset;

        private string _mode;

        private IEffectParameterSet _cachedEffectParameterSet;

        private IEffect _effectUsedForParameterSetCache;

        private Texture2D _lastSetDiffuseTexture;

        private Texture2D _lastSetNormalMap;

        private Texture2D _lastSetSpecularColorMap;

        private Color? _lastSetSpecularColor;

        private Color? _lastSetDiffuseColor;

        private float? _lastSetSpecularPower;

        private bool _lastDidSetDiffuseTexture;

        private bool _lastDidSetNormalMap;

        private bool _lastDidSetSpecularColorMap;

        private bool _lastDidSetSpecularColor;

        private bool _lastDidSetDiffuseColor;

        private bool _lastDidSetSpecularPower;

        private IEffect _cachedEffect;

        private Matrix _lastWorldMatrix;

        private IRenderRequest _renderRequest;

        public Render3DModelComponent(
            INode node,
            I3DRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider,
            ITextureFromHintPath textureFromHintPath,
            IRenderBatcher renderBatcher)
        {
            _node = node;
            _renderUtilities = renderUtilities;
            _textureFromHintPath = textureFromHintPath;
            _renderBatcher = renderBatcher;
            _assetManager = assetManagerProvider.GetAssetManager();

            Enabled = true;
            Transform = new DefaultTransform();
        }
        
        public ModelAsset Model { get; set; }

        public EffectAsset Effect { get; set; }

        public bool Enabled { get; set; }

        public Material OverrideMaterial { get; set; }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!Enabled)
            {
                return;
            }

            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                if (Effect == null)
                {
                    _useDefaultEffects = true;
                }
                else
                {
                    _useDefaultEffects = false;
                }

                if (_useDefaultEffects && _uberEffectAsset == null)
                {
                    _uberEffectAsset = _assetManager.Get<UberEffectAsset>("effect.BuiltinSurface");
                }

                if (Model != null)
                {
                    var matrix = FinalTransform.AbsoluteMatrix;
                    
                    bool changedRenderRequest = _lastWorldMatrix != matrix;
                    string changedRenderRequestBy = changedRenderRequest ? "matrix" : "";

                    var material = OverrideMaterial ?? Model.Material;

                    UpdateCachedModel(material, ref changedRenderRequest, ref changedRenderRequestBy);

                    var effect = GetEffect(ref changedRenderRequest, ref changedRenderRequestBy);

                    var parameterSet = GetEffectParameterSet(material, ref changedRenderRequest, ref changedRenderRequestBy);

                    if (changedRenderRequest || _renderRequest == null)
                    {
                        _renderRequest = Model.CreateRenderRequest(renderContext, effect, parameterSet, matrix);
                    }

                    _lastWorldMatrix = matrix;

                    _renderBatcher.QueueRequest(
                        renderContext,
                        _renderRequest);
                }
                else
                {
                    _lastCachedModel = null;
                    _lastCachedDiffuseTexture = null;
                }
            }
        }

        private IEffectParameterSet GetEffectParameterSet(IMaterial material, ref bool changedRenderRequest, ref string changedRenderRequestBy)
        {
            if (_effectUsedForParameterSetCache == _cachedEffect &&
                changedRenderRequest == false &&
                (!_lastDidSetDiffuseTexture || _lastSetDiffuseTexture == _lastCachedDiffuseTexture?.Texture) &&
                (!_lastDidSetNormalMap || _lastSetNormalMap == _lastCachedNormalMapTexture?.Texture) &&
                (!_lastDidSetSpecularPower || _lastSetSpecularPower == _lastCachedSpecularPower) &&
                (!_lastDidSetSpecularColorMap || _lastSetSpecularColorMap == _lastCachedSpecularColorMapTexture?.Texture) &&
                (!_lastDidSetSpecularColor || _lastSetSpecularColor == _lastCachedSpecularColor) &&
                (!_lastDidSetDiffuseColor || _lastSetDiffuseColor == (material.ColorDiffuse ?? Color.Black)))
            {
                // Reuse the existing parameter set.
                return _cachedEffectParameterSet;
            }

            changedRenderRequest = true;
            changedRenderRequestBy += ":parameterset";

            // Create a new parameter set and cache it.
            _cachedEffectParameterSet = _cachedEffect.CreateParameterSet();
            _effectUsedForParameterSetCache = _cachedEffect;

            _lastSetDiffuseTexture = null;
            _lastSetNormalMap = null;
            _lastSetSpecularPower = null;
            _lastSetSpecularColorMap = null;
            _lastSetSpecularColor = null;
            _lastSetDiffuseColor = null;
            _lastDidSetDiffuseTexture = false;
            _lastDidSetNormalMap = false;
            _lastDidSetSpecularPower = false;
            _lastDidSetSpecularColorMap = false;
            _lastDidSetSpecularColor = false;
            _lastDidSetDiffuseColor = false;

            if (_cachedEffectParameterSet.HasSemantic<ITextureEffectSemantic>())
            {
                if (_lastCachedDiffuseTexture?.Texture != null)
                {
                    _cachedEffectParameterSet.GetSemantic<ITextureEffectSemantic>().Texture =
                        _lastCachedDiffuseTexture.Texture;
                    _lastSetDiffuseTexture = _lastCachedDiffuseTexture.Texture;
                    _lastDidSetDiffuseTexture = true;
                }
            }

            if (_cachedEffectParameterSet.HasSemantic<INormalMapEffectSemantic>())
            {
                if (_lastCachedNormalMapTexture?.Texture != null)
                {
                    _cachedEffectParameterSet.GetSemantic<INormalMapEffectSemantic>().NormalMap =
                        _lastCachedNormalMapTexture.Texture;
                    _lastSetNormalMap = _lastCachedNormalMapTexture.Texture;
                    _lastDidSetNormalMap = true;
                }
            }

            if (_cachedEffectParameterSet.HasSemantic<ISpecularEffectSemantic>())
            {
                if (_lastCachedSpecularPower != null)
                {
                    var semantic = _cachedEffectParameterSet.GetSemantic<ISpecularEffectSemantic>();
                    semantic.SpecularPower = _lastCachedSpecularPower.Value;
                    _lastSetSpecularPower = _lastCachedSpecularPower.Value;
                    _lastDidSetSpecularPower = true;

                    if (_lastCachedSpecularColorMapTexture != null)
                    {
                        semantic.SpecularColorMap = _lastCachedSpecularColorMapTexture.Texture;
                        _lastSetSpecularColorMap = _lastCachedSpecularColorMapTexture.Texture;
                        _lastDidSetSpecularColorMap = true;
                    }
                    else if (_lastCachedSpecularColor != null)
                    {
                        semantic.SpecularColor = _lastCachedSpecularColor.Value;
                        _lastSetSpecularColor = _lastCachedSpecularColor.Value;
                        _lastDidSetSpecularColor = true;
                    }
                }
            }

            if (_cachedEffectParameterSet.HasSemantic<IColorDiffuseEffectSemantic>())
            {
                var v = material.ColorDiffuse ?? Color.Black;
                _cachedEffectParameterSet.GetSemantic<IColorDiffuseEffectSemantic>().Diffuse = v;
                _lastSetDiffuseColor = v;
                _lastDidSetDiffuseColor = true;
            }

            return _cachedEffectParameterSet;
        }

        private void UpdateCachedModel(IMaterial material, ref bool changedRenderRequest, ref string changedRenderRequestBy)
        {
            if (_lastCachedModel != Model)
            {
                changedRenderRequest = true;
                changedRenderRequestBy += ":model";

                if (material.TextureDiffuse != null && material.TextureNormal != null)
                {
                    if (material.TextureDiffuse.TextureAsset != null)
                    {
                        _lastCachedDiffuseTexture = material.TextureDiffuse.TextureAsset;
                    }
                    else
                    {
                        _lastCachedDiffuseTexture =
                            _textureFromHintPath.GetTextureFromHintPath(material.TextureDiffuse);
                    }

                    if (material.TextureNormal != null)
                    {
                        if (material.TextureNormal.TextureAsset != null)
                        {
                            _lastCachedNormalMapTexture = material.TextureNormal.TextureAsset;
                        }
                        else
                        {
                            _lastCachedNormalMapTexture =
                                _textureFromHintPath.GetTextureFromHintPath(material.TextureNormal);
                        }
                    }
                    else
                    {
                        _lastCachedNormalMapTexture = null;
                    }

                    if (material.PowerSpecular != null)
                    {
                        _lastCachedSpecularPower = material.PowerSpecular.Value;

                        if (material.TextureSpecular != null)
                        {
                            if (material.TextureSpecular.TextureAsset != null)
                            {
                                _lastCachedSpecularColorMapTexture = material.TextureSpecular.TextureAsset;
                            }
                            else
                            {
                                _lastCachedSpecularColorMapTexture =
                                    _textureFromHintPath.GetTextureFromHintPath(material.TextureSpecular);
                            }
                        }
                        else if (material.ColorSpecular != null)
                        {
                            _lastCachedSpecularColor = material.ColorSpecular.Value;
                        }
                        else
                        {
                            _lastCachedSpecularColor = null;
                        }
                    }
                    else
                    {
                        _lastCachedSpecularPower = null;
                    }

                    _mode = "texture";
                }
                else if (material.ColorDiffuse != null)
                {
                    _mode = "diffuse";
                }
                else
                {
                    _mode = "color";
                }
                _lastCachedModel = Model;
            }
        }

        private IEffect GetEffect(ref bool changedRenderRequest, ref string changedRenderRequestBy)
        {
            if (_cachedEffect != null)
            {
                return _cachedEffect;
            }

            changedRenderRequest = true;
            changedRenderRequestBy += ":effect";

            IEffect effect;
            if (!_useDefaultEffects)
            {
                effect = Effect.Effect;
            }
            else
            {
                var skinnedSuffix = _lastCachedModel.Bones == null ? null : "Skinned";

                switch (_mode)
                {
                    case "texture":
                        if (_lastCachedNormalMapTexture != null && _lastCachedSpecularPower != null)
                        {
                            if (_lastCachedSpecularColorMapTexture != null)
                            {
                                effect =
                                    _uberEffectAsset.Effects["TextureNormalSpecColMap" + skinnedSuffix];
                            }
                            else if (_lastCachedSpecularColor != null)
                            {
                                effect =
                                    _uberEffectAsset.Effects["TextureNormalSpecColCon" + skinnedSuffix];
                            }
                            else
                            {
                                effect =
                                    _uberEffectAsset.Effects["TextureNormalSpecColDef" + skinnedSuffix];
                            }
                        }
                        else if (_lastCachedNormalMapTexture != null)
                        {
                            effect = _uberEffectAsset.Effects["TextureNormal" + skinnedSuffix];
                        }
                        else
                        {
                            effect = _uberEffectAsset.Effects["Texture" + skinnedSuffix];
                        }
                        break;
                    case "color":
                        effect = _uberEffectAsset.Effects["Color" + skinnedSuffix];
                        break;
                    case "diffuse":
                        effect = _uberEffectAsset.Effects["Diffuse" + skinnedSuffix];
                        break;
                    default:
                        throw new InvalidOperationException("Unknown default effect type.");
                }
            }

            _cachedEffect = effect;
            return _cachedEffect;
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => this.GetAttachedFinalTransformImplementation(_node);
    }
}
