using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Protoinject;

namespace Protogame
{
    public class Render3DModelComponent : IRenderableComponent, IEnabledComponent
    {
        private readonly INode _node;

        private readonly I3DRenderUtilities _renderUtilities;

        private readonly ITextureFromHintPath _textureFromHintPath;

        private readonly IAssetManager _assetManager;

        private ModelAsset _lastCachedModel;

        private TextureAsset _lastCachedDiffuseTexture;

        private TextureAsset _lastCachedNormalMapTexture;

        private bool _useDefaultEffects;

        private EffectAsset _defaultTextureSkinnedEffectAsset;

        private EffectAsset _defaultTextureNormalSkinnedEffectAsset;

        private EffectAsset _defaultColorSkinnedEffectAsset;

        private EffectAsset _defaultDiffuseSkinnedEffectAsset;

        private string _mode;

        public Render3DModelComponent(
            INode node,
            I3DRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider,
            ITextureFromHintPath textureFromHintPath)
        {
            _node = node;
            _renderUtilities = renderUtilities;
            _textureFromHintPath = textureFromHintPath;
            _assetManager = assetManagerProvider.GetAssetManager();

            Enabled = true;
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
                }

                if (Model != null)
                {
                    var matrix = Matrix.Identity;
                    var matrixComponent = _node.Parent?.UntypedValue as IHasTransform;
                    if (matrixComponent != null)
                    {
                        matrix *= matrixComponent.FinalTransform.AbsoluteMatrix;
                    }

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

                    switch (_mode)
                    {
                        case "texture":
                            if (_useDefaultEffects)
                            {
                                renderContext.PushEffect(_defaultTextureSkinnedEffectAsset.Effect);
                            }
                            else
                            {
                                renderContext.PushEffect(Effect.Effect);
                            }
                            
                            renderContext.SetActiveTexture(_lastCachedDiffuseTexture.Texture);
                            break;
                        case "texture-normal":
                            if (_useDefaultEffects)
                            {
                                renderContext.PushEffect(_defaultTextureSkinnedEffectAsset.Effect);
                            }
                            else
                            {
                                renderContext.PushEffect(Effect.Effect);
                            }

                            renderContext.SetActiveTexture(_lastCachedDiffuseTexture.Texture);

                            {
                                var semanticEffect = renderContext.Effect as EffectWithSemantics;
                                if (semanticEffect != null && semanticEffect.HasSemantic<INormalMapEffectSemantic>())
                                {
                                    var normalMap = semanticEffect.GetSemantic<INormalMapEffectSemantic>();
                                    if (normalMap != null)
                                    {
                                        normalMap.NormalMap = _lastCachedNormalMapTexture.Texture;
                                    }
                                }
                            }

                            break;
                        case "color":
                            if (_useDefaultEffects)
                            {
                                renderContext.PushEffect(_defaultColorSkinnedEffectAsset.Effect);
                            }
                            else
                            {
                                renderContext.PushEffect(Effect.Effect);
                            }
                            
                            break;
                        case "diffuse":
                            Effect targetEffect;
                            if (_useDefaultEffects)
                            {
                                targetEffect = _defaultDiffuseSkinnedEffectAsset.Effect;
                            }
                            else
                            {
                                targetEffect = Effect.Effect;
                            }

                            {
                                var semanticEffect = targetEffect as EffectWithSemantics;
                                if (semanticEffect != null)
                                {
                                    semanticEffect.GetSemantic<IColorDiffuseEffectSemantic>().Diffuse =
                                        material.ColorDiffuse.Value;
                                }
                            }

                            renderContext.PushEffect(targetEffect);
                            
                            break;
                    }

                    Model.Render(
                        renderContext,
                        matrix);
                    renderContext.PopEffect();
                }
                else
                {
                    _lastCachedModel = null;
                    _lastCachedDiffuseTexture = null;
                }
            }
        }
    }
}
