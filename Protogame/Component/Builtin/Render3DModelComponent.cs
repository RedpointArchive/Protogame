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

        private bool _useDefaultEffects;

        private EffectAsset _defaultTextureSkinnedEffectAsset;

        private EffectAsset _defaultTextureNormalSkinnedEffectAsset;

        private EffectAsset _defaultColorSkinnedEffectAsset;

        private EffectAsset _defaultDiffuseSkinnedEffectAsset;

        private EffectAsset _defaultTextureEffectAsset;

        private EffectAsset _defaultTextureNormalEffectAsset;

        private EffectAsset _defaultColorEffectAsset;

        private EffectAsset _defaultDiffuseEffectAsset;

        private string _mode;

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

                if (_useDefaultEffects && _defaultTextureSkinnedEffectAsset == null)
                {
                    _defaultTextureSkinnedEffectAsset = _assetManager.Get<EffectAsset>("effect.TextureSkinned");
                    _defaultTextureNormalSkinnedEffectAsset = _assetManager.Get<EffectAsset>("effect.TextureNormalSkinned");
                    _defaultColorSkinnedEffectAsset = _assetManager.Get<EffectAsset>("effect.ColorSkinned");
                    _defaultDiffuseSkinnedEffectAsset = _assetManager.Get<EffectAsset>("effect.DiffuseSkinned");
                    _defaultTextureEffectAsset = _assetManager.Get<EffectAsset>("effect.Texture");
                    _defaultTextureNormalEffectAsset = _assetManager.Get<EffectAsset>("effect.TextureNormal");
                    _defaultColorEffectAsset = _assetManager.Get<EffectAsset>("effect.Color");
                    _defaultDiffuseEffectAsset = _assetManager.Get<EffectAsset>("effect.Diffuse");
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
                            
                            if (material.TextureNormal.TextureAsset != null)
                            {
                                _lastCachedNormalMapTexture = material.TextureNormal.TextureAsset;
                            }
                            else
                            {
                                _lastCachedNormalMapTexture =
                                    _textureFromHintPath.GetTextureFromHintPath(material.TextureNormal);
                            }

                            _mode = "texture-normal";
                        }
                        else if (material.TextureDiffuse != null)
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

                    Effect effect;

                    if (!_useDefaultEffects)
                    {
                        effect = Effect.Effect;
                    }
                    else
                    {
                        if (_lastCachedModel.Bones == null)
                        {
                            switch (_mode)
                            {
                                case "texture":
                                    effect = _defaultTextureEffectAsset.Effect;
                                    break;
                                case "texture-normal":
                                    effect = _defaultTextureNormalEffectAsset.Effect;
                                    break;
                                case "color":
                                    effect = _defaultColorEffectAsset.Effect;
                                    break;
                                case "diffuse":
                                    effect = _defaultDiffuseEffectAsset.Effect;
                                    break;
                                default:
                                    throw new InvalidOperationException("Unknown default effect type.");
                            }
                        }
                        else
                        {
                            switch (_mode)
                            {
                                case "texture":
                                    effect = _defaultTextureSkinnedEffectAsset.Effect;
                                    break;
                                case "texture-normal":
                                    effect = _defaultTextureNormalSkinnedEffectAsset.Effect;
                                    break;
                                case "color":
                                    effect = _defaultColorSkinnedEffectAsset.Effect;
                                    break;
                                case "diffuse":
                                    effect = _defaultDiffuseSkinnedEffectAsset.Effect;
                                    break;
                                default:
                                    throw new InvalidOperationException("Unknown default effect type.");
                            }
                        }
                    }

                    var semanticEffect = effect as EffectWithSemantics;
                    if (semanticEffect != null)
                    {
                        if (semanticEffect.HasSemantic<ITextureEffectSemantic>())
                        {
                            if (_lastCachedDiffuseTexture.Texture != null)
                            {
                                semanticEffect.GetSemantic<ITextureEffectSemantic>().Texture =
                                    _lastCachedDiffuseTexture.Texture;
                            }
                        }

                        if (semanticEffect.HasSemantic<INormalMapEffectSemantic>())
                        {
                            if (_lastCachedNormalMapTexture.Texture != null)
                            {
                                semanticEffect.GetSemantic<INormalMapEffectSemantic>().NormalMap =
                                    _lastCachedNormalMapTexture.Texture;
                            }
                        }

                        if (semanticEffect.HasSemantic<IColorDiffuseEffectSemantic>())
                        {
                            semanticEffect.GetSemantic<IColorDiffuseEffectSemantic>().Diffuse =
                                material.ColorDiffuse ?? Color.Black;
                        }
                    }

                    renderContext.PushEffect(effect);
                    _renderBatcher.QueueRequest(
                        renderContext,
                        Model.CreateRenderRequest(renderContext, matrix));
                    renderContext.PopEffect();
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
