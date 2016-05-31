using System;
using System.IO;
using System.Linq;
using Protogame.ATFLevelEditor;

namespace Protogame
{
    public class LocatorEntity : ComponentizedEntity
    {
        private readonly Render3DModelComponent _modelComponent;

        public LocatorEntity(
            IEditorQuery<LocatorEntity> editorQuery,
            IAssetManagerProvider assetManagerProvider,
            Render3DModelComponent modelComponent)
        {
            if (editorQuery.Mode != EditorQueryMode.BakingSchema)
            {
                _modelComponent = modelComponent;
                RegisterPrivateComponent(_modelComponent);

                var assetManager = assetManagerProvider.GetAssetManager();
                editorQuery.MapMatrix(this, x => this.LocalMatrix = x);

                var modelUri = editorQuery.GetRawResourceUris().FirstOrDefault();
                if (modelUri != null)
                {
                    var extIndex = modelUri.LastIndexOf(".", StringComparison.InvariantCulture);
                    if (extIndex != -1)
                    {
                        modelUri = modelUri.Substring(0, extIndex);
                    }
                    var pathComponents = modelUri.Split('/').ToList();
                    while (pathComponents[0] == "..")
                    {
                        pathComponents.RemoveAt(0);
                    }

                    while (pathComponents.Count > 0)
                    {
                        var attemptAsset = string.Join(".", pathComponents);
                        var resultAsset = assetManager.TryGet<ModelAsset>(attemptAsset);
                        if (resultAsset == null)
                        {
                            pathComponents.RemoveAt(0);
                        }
                        else
                        {
                            _modelComponent.Model = resultAsset;
                            break;
                        }
                    }
                }
            }
        }
    }
}
