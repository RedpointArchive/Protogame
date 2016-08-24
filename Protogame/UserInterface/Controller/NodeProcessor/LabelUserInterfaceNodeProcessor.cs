using System;
using System.IO;
using System.Xml;

namespace Protogame
{
    public class LabelUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        private readonly INodeColorParser _nodeColorParser;
        private readonly IAssetManager _assetManager;

        public LabelUserInterfaceNodeProcessor(
            IAssetManagerProvider assetManagerProvider,
            INodeColorParser nodeColorParser)
        {
            _nodeColorParser = nodeColorParser;
            _assetManager = assetManagerProvider.GetAssetManager();
        }

        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            var label = new Label();

            label.Text = node?.Attributes?["text"]?.Value ?? string.Empty;

            var fontName = node?.Attributes?["font"]?.Value;
            if (!string.IsNullOrWhiteSpace(fontName))
            {
                label.Font = _assetManager.TryGet<FontAsset>(fontName);
            }

            var textColorRaw = node?.Attributes?["color"]?.Value;
            if (!string.IsNullOrWhiteSpace(textColorRaw))
            {
                var textColor = _nodeColorParser.Parse(textColorRaw);
                if (textColor != null)
                {
                    label.TextColor = textColor.Value;
                }
            }

            var shadowEnabled = node?.Attributes?["shadowEnabled"]?.Value;
            if (shadowEnabled == "false")
            {
                label.RenderShadow = false;
            }
            else if (shadowEnabled == "true")
            {
                label.RenderShadow = true;
            }

            var shadowColorRaw = node?.Attributes?["shadowColor"]?.Value;
            if (!string.IsNullOrWhiteSpace(shadowColorRaw))
            {
                var shadowColor = _nodeColorParser.Parse(textColorRaw);
                if (shadowColor != null)
                {
                    label.ShadowColor = shadowColor.Value;
                }
            }

            switch (node?.Attributes?["halign"]?.Value)
            {
                case "left":
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case "center":
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case "right":
                    label.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
            }

            switch (node?.Attributes?["valign"]?.Value)
            {
                case "top":
                    label.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case "center":
                    label.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case "bottom":
                    label.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
            }

            processChild = (xmlNode, container) =>
            {
                throw new InvalidDataException("The '" + xmlNode.LocalName + "' control can not have any children.");
            };
            return label;
        }
    }
}
