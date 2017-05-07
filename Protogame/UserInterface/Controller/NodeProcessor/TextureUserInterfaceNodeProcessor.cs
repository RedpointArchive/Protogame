using System;
using System.Xml;

namespace Protogame
{
    public class TextureUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly IAssetManager _assetManager;

        public TextureUserInterfaceNodeProcessor(I2DRenderUtilities renderUtilities,
            IAssetManager assetManager)
        {
            _renderUtilities = renderUtilities;
            _assetManager = assetManager;
        }

        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            var textureName = node?.Attributes?["texture"]?.Value ?? string.Empty;
            var fit = node?.Attributes?["fit"]?.Value ?? "stretch";
            var texture = _assetManager.Get<TextureAsset>(textureName);

            var textureContainer = new TextureContainer(_renderUtilities, texture, fit);
            processChild = (xmlNode, container) =>
            {
                throw new Exception("The '" + xmlNode.LocalName + "' control can not have any children.");
            };
            return textureContainer;
        }
    }
}
