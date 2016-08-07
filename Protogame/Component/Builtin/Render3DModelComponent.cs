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

                    var material = OverrideMaterial ?? Model.Material;

                    if (_lastCachedModel != Model)
                    {
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
                                        _lastCachedSpecularColorMapTexture = material.TextureNormal.TextureAsset;
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

                    IEffectParameterSet parameterSet;
                    if (_effectUsedForParameterSetCache == effect)
                    {
                        // Reuse the existing parameter set.
                        parameterSet = _cachedEffectParameterSet;
                        parameterSet.Unlock();
                    }
                    else
                    {
                        // Create a new parameter set and cache it.
                        parameterSet = effect.CreateParameterSet();
                        _cachedEffectParameterSet = parameterSet;
                        _effectUsedForParameterSetCache = effect;
                    }

                    if (parameterSet.HasSemantic<ITextureEffectSemantic>())
                    {
                        if (_lastCachedDiffuseTexture?.Texture != null)
                        {
                            parameterSet.GetSemantic<ITextureEffectSemantic>().Texture =
                                _lastCachedDiffuseTexture.Texture;
                        }
                    }

                    if (parameterSet.HasSemantic<INormalMapEffectSemantic>())
                    {
                        if (_lastCachedNormalMapTexture?.Texture != null)
                        {
                            parameterSet.GetSemantic<INormalMapEffectSemantic>().NormalMap =
                                _lastCachedNormalMapTexture.Texture;
                        }
                    }

                    if (parameterSet.HasSemantic<ISpecularEffectSemantic>())
                    {
                        if (_lastCachedSpecularPower != null)
                        {
                            var semantic = parameterSet.GetSemantic<ISpecularEffectSemantic>();
                            semantic.SpecularPower = _lastCachedSpecularPower.Value;

                            if (_lastCachedSpecularColorMapTexture != null)
                            {
                                semantic.SpecularColorMap = _lastCachedSpecularColorMapTexture.Texture;
                            }
                            else if (_lastCachedSpecularColor != null)
                            {
                                semantic.SpecularColor = _lastCachedSpecularColor.Value;
                            }
                        }
                    }

                    if (parameterSet.HasSemantic<IColorDiffuseEffectSemantic>())
                    {
                        parameterSet.GetSemantic<IColorDiffuseEffectSemantic>().Diffuse =
                            material.ColorDiffuse ?? Color.Black;
                    }
                    
                    _renderBatcher.QueueRequest(
                        renderContext,
                        Model.CreateRenderRequest(renderContext, effect, parameterSet, matrix));
                }
                else
                {
                    _lastCachedModel = null;
                    _lastCachedDiffuseTexture = null;
                }
            }
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => this.GetAttachedFinalTransformImplementation(_node);
    }
}
